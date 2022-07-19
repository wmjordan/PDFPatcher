using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Matrix = iTextSharp.text.pdf.parser.Matrix;
using NameValuePair = System.Collections.Generic.KeyValuePair<string, string>;

namespace PDFPatcher.Processor
{
	sealed class PdfContentExport
	{
		readonly ExporterOptions _options;
		readonly Dictionary<string, string> _resolvedReferences = new Dictionary<string, string>();
		readonly List<string> _exportPath = new List<string>();
		ImageExtractor _imageExporter;

		bool AddReferenceRecord(PdfIndirectReference r, string type) {
			var k = String.Concat(r.Number, ' ', r.Generation);
			if (_resolvedReferences.ContainsKey(k)) {
				return false;
			}

			_resolvedReferences.Add(k, type);
			return true;
		}


		public PdfContentExport(ExporterOptions options) {
			_options = options;
		}

		internal void ExportTrailer(XmlWriter writer, PdfReader reader) {
			Tracker.TraceMessage("导出文档总索引。");
			writer.WriteStartElement("Trailer");
			writer.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/", Constants.ContentNamespace);
			foreach (var item in reader.Trailer) {
				if (PdfName.ROOT.Equals(item.Key) == false) {
					ExportPdfDictionaryItem(item, writer);
				}
			}
			ExportCatalog(writer, reader);
			writer.WriteEndElement();
		}

		void ExportCatalog(XmlWriter writer, PdfReader reader) {
			Tracker.TraceMessage("导出文档编录。");
			writer.WriteStartElement(Constants.Catalog);
			writer.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/", Constants.ContentNamespace);
			foreach (var item in reader.Catalog) {
				if (PdfName.OUTLINES.Equals(item.Key)) {
					if (PdfReader.GetPdfObjectRelease(item.Value) is PdfDictionary o) {
						ExportPdfOutline(o, writer);
					}
				}
				else {
					ExportPdfDictionaryItem(item, writer);
				}
			}
			writer.WriteEndElement();
		}

		internal void ExportContents(XmlWriter writer, PdfReader reader) {
			if (_options.ExtractImages) {
				_imageExporter = new ImageExtractor(_options.Images, reader);
			}
			writer.WriteStartElement(Constants.Body);
			writer.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/", Constants.ContentNamespace);

			var ranges = PageRangeCollection.Parse(_options.ExtractPageRange, 1, reader.NumberOfPages, true);
			ExtractPages(reader, ranges, writer);
			writer.WriteEndElement();
		}

		void ExtractPages(PdfReader reader, PageRangeCollection ranges, XmlWriter writer) {
			var p = new ExportProcessor(this, writer, _options);
			foreach (PageRange r in ranges) {
				foreach (var i in r) {
					Tracker.TraceMessage(String.Concat("导出第 ", i, " 页。"));
					ExtractPage(i, reader, writer, p);
					Tracker.IncrementProgress(1);
				}
			}
		}

		void ExportPdfOutline(PdfDictionary outline, XmlWriter writer) {
			while (outline != null) {
				writer.WriteStartElement(Constants.ContentPrefix, "Outline", Constants.ContentNamespace);
				foreach (var i in outline) {
					switch (i.Key.ToString()) {
						case "/First":
						case "/Last":
						case "/Next":
						case "/Parent":
						case "/Prev":
							continue;
						default:
							ExportPdfDictionaryItem(i, writer);
							break;
					}
				}
				if (PdfReader.GetPdfObjectRelease(outline.Get(PdfName.FIRST)) is PdfDictionary f) {
					writer.WriteStartElement(Constants.ContentPrefix, "Outlines", Constants.ContentNamespace);
					ExportPdfOutline(f, writer);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
				outline = PdfReader.GetPdfObjectRelease(outline.Get(PdfName.NEXT)) as PdfDictionary;
			}
		}

		/// <summary>
		/// 导出 PDFDictionary。
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="dict"></param>
		void ExportPdfDictionary(XmlWriter writer, PdfDictionary dict) {
			foreach (var item in dict) {
				ExportPdfDictionaryItem(item, writer);
			}
		}

		void ExportPdfDictionaryItem(KeyValuePair<PdfName, PdfObject> item, XmlWriter writer) {
			var key = PdfHelper.DecodeKeyName(item.Key);
			var text = item.Value.ToString();
			var value = item.Value as PdfObject;
			try {
				writer.WriteStartElement(XmlConvert.VerifyNCName(key));
			}
			catch (XmlException) {
				writer.WriteStartElement(Constants.Content.Item, Constants.ContentNamespace);
				writer.WriteAttributeString(Constants.Content.Name, Constants.ContentNamespace, key);
			}
			_exportPath.Add(key);
			if (value == null) {
				writer.WriteAttributeString(PdfHelper.GetTypeName(value.Type), text);
				goto EndElement;
			}
			switch (value.Type) {
				case PdfObject.ARRAY:
					var a = value as PdfArray;
					if (key == "ColorSpace" && PdfName.INDEXED.Equals(a.GetAsName(0)) && a.ArrayList.Count == 4) {
						ExportColorSpaceContent(writer, a);
					}
					else {
						ExportArray(a.ArrayList, writer);
					}
					break;
				case PdfObject.STRING:
					text = (value as PdfString).ToUnicodeString();
					if (text.StartsWith("<?xml")) {
						writer.WriteStartElement(Constants.ContentPrefix, Constants.Content.Value, Constants.ContentNamespace);
						writer.WriteCData(text);
						writer.WriteEndElement();
					}
					else {
						writer.WriteAttributeString(PdfHelper.GetTypeName(value.Type), PdfHelper.GetValidXmlString(text));
					}
					break;
				case PdfObject.INDIRECT:
					writer.WriteAttributeString(Constants.Content.Type, Constants.ContentNamespace, PdfHelper.GetTypeName(PdfObject.INDIRECT));
					ExportIndirectReference(value as PdfIndirectReference, writer, key);
					break;
				case PdfObject.DICTIONARY:
					ExportPdfDictionary(writer, value as PdfDictionary);
					break;
				default:
					writer.WriteAttributeString(PdfHelper.GetTypeName(value.Type), text);
					break;
			}
		EndElement:
			writer.WriteEndElement();
			_exportPath.RemoveAt(_exportPath.Count - 1);
		}

		void ExportColorSpaceContent(XmlWriter writer, PdfArray a) {
			writer.WriteStartElement(Constants.ContentPrefix, "colorSpace", Constants.ContentNamespace);
			writer.WriteAttributeString("type", a[0].ToString());
			var b = a.GetAsName(1);
			if (b != null) {
				writer.WriteAttributeString("base", a.GetAsName(1).ToString());
			}
			writer.WriteAttributeString("hival", a.GetAsNumber(2).ToString());
			writer.WriteStartAttribute("lookup");
			var cs = a.GetDirectObject(3);
			byte[] l = null;
			if (cs.IsString()) {
				l = (cs as PdfString).GetOriginalBytes();
			}
			else if (cs.IsStream()) {
				l = PdfReader.GetStreamBytes(cs as PRStream);
			}
			if (l != null) {
				int g;
				if (b == null) {
					g = l.Length;
				}
				else if (b.Equals(PdfName.DEVICERGB)) {
					g = 3;
				}
				else if (b.Equals(PdfName.DEVICECMYK)) {
					g = 4;
				}
				else {
					g = l.Length;
				}
				for (int i = 0; i < l.Length; i += g) {
					writer.WriteBinHex(l, i, g);
					writer.WriteString(" ");
				}
			}
			writer.WriteEndAttribute();
			if (a[1].IsArray()) {
				ExportArray(a.GetAsArray(1).ArrayList, writer);
			}
			writer.WriteEndElement();
		}

		void ExportArray(List<PdfObject> array, XmlWriter writer) {
			writer.WriteStartElement(Constants.ContentPrefix, "array", Constants.ContentNamespace);
			ExportArrayContent(array, writer, false);
			writer.WriteEndElement();
		}

		void ExportArrayContent(List<PdfObject> array, XmlWriter writer, bool writeStringBytes) {
			int iType = PdfObject.NULL;
			foreach (PdfObject i in array) {
				if (iType == PdfObject.NULL) {
					iType = i.Type;
				}
				else if (iType != i.Type) {
					iType = PdfObject.NULL;
					break;
				}
			}
			if (ValueHelper.IsInCollection(iType, PdfObject.NAME, PdfObject.NUMBER, PdfObject.BOOLEAN)) {
				writer.WriteAttributeString(Constants.Content.Type, PdfHelper.GetTypeName(iType));
				writer.WriteStartAttribute(Constants.Content.Value);
				var l = array;
				for (int i = 0; i < l.Count; i++) {
					if (i > 0) {
						writer.WriteString(" ");
					}
					writer.WriteString(l[i].ToString());
				}
				writer.WriteEndAttribute();
			}
			else {
				foreach (PdfObject i in array) {
					if (i is not PdfArray subArray) {
						if (PdfHelper.GetTypeName(i.Type).Length > 0) {
							writer.WriteStartElement(Constants.ContentPrefix, PdfHelper.GetTypeName(i.Type), Constants.ContentNamespace);
						}
#if DEBUG
						else {
							writer.WriteElementString("literal", i.ToString());
							continue;
						}
#endif
						switch (i.Type) {
							case PdfObject.INDIRECT:
								ExportIndirectReference(i as PdfIndirectReference, writer, String.Empty);
								break;
							case PdfObject.DICTIONARY:
								ExportPdfDictionary(writer, i as PdfDictionary);
								break;
							case PdfObject.STRING:
								if (writeStringBytes) {
									var bytes = (i as PdfString).GetBytes();
									var bl = bytes.Length;
									if (bl > 0) {
										int l = 2 * bl + ((bl + 1) >> 1) - 1;
										var chArray = new char[l];
										int num = 0, bi = 0;
										foreach (byte b in bytes) {
											if ((bi % 2) == 0 && bi > 0) {
												chArray[num++] = ' ';
											}
											chArray[num++] = "0123456789ABCDEF"[b >> 4];
											chArray[num++] = "0123456789ABCDEF"[b & 15];
											bi++;
										}
										writer.WriteAttributeString(Constants.Content.Type, "字节");
										writer.WriteAttributeString(Constants.Content.Value, new string(chArray));
									}
									break;
								}
								goto default;
							default:
								writer.WriteAttributeString(Constants.Content.Value, PdfHelper.GetValidXmlString(i.ToString()));
								break;
						}
						writer.WriteEndElement();
					}
					else {
						writer.WriteStartElement(Constants.ContentPrefix, "array", Constants.ContentNamespace);
						ExportArrayContent(subArray.ArrayList, writer, writeStringBytes);
						writer.WriteEndElement();
					}
				}
			}
		}

		void ExportIndirectReference(PdfIndirectReference r, XmlWriter writer, string key) {
			var i = PdfReader.GetPdfObjectRelease(r);
			writer.WriteAttributeString(Constants.Content.ResourceID, Constants.ContentNamespace, r.ToString());
			if (AddReferenceRecord(r, key) == false || i == null) {
				return;
			}
			switch (i.Type) {
				case PdfObject.DICTIONARY:
					var type = (i as PdfDictionary).GetAsName(PdfName.TYPE);
					if (type != null) {
						var n = PdfHelper.GetPdfNameString(type);
						writer.WriteAttributeString(Constants.Content.RefType, Constants.ContentNamespace, n);
						switch (n) {
							case "Page":
							case "Pages":
								return;
							default:
								if (key == "Parent") {
									return;
								}
								break;
						}
					}
					ExportPdfDictionary(writer, i as PdfDictionary);
					break;
				case PdfObject.ARRAY:
					ExportArray((i as PdfArray).ArrayList, writer);
					break;
				case PdfObject.STREAM:
					var s = i as PdfStream;
					ExportPdfDictionary(writer, i as PdfDictionary);
					if (_imageExporter != null && PdfName.IMAGE.Equals((i as PdfDictionary).GetAsName(PdfName.SUBTYPE))) {
						writer.WriteStartElement(Constants.ContentPrefix, "image", Constants.ContentNamespace);
						var info = _imageExporter.InfoList.Find(ii => ii.InlineImage.PdfRef == r);
						if (info != null) {
							writer.WriteAttributeString(Constants.Content.Path, Path.GetFileName(info.FileName));
						}
						writer.WriteEndElement();
						break;
					}
					ExportStreamContent(writer, s);
					break;
				default:
					writer.WriteAttributeString(PdfHelper.GetTypeName(i.Type), Constants.ContentNamespace, i.ToString());
					break;
			}
		}

		void ExportStreamContent(XmlWriter writer, PdfStream s) {
			if (s is not PRStream prs || writer is NullXmlWriter) {
				return;
			}
			var key = _exportPath[_exportPath.Count - 1];
			writer.WriteStartElement(Constants.ContentPrefix, "stream", Constants.ContentNamespace);
			byte[] bs;
			bool isRaw = false;
			try {
				bs = PdfReader.GetStreamBytes(prs);
			}
			catch (IOException) {
				bs = PdfReader.GetStreamBytesRaw(prs);
				isRaw = true;
				writer.WriteAttributeString(Constants.Content.Raw, Constants.Boolean.True);
			}
			if (isRaw == false) {
				if (key == "Contents" || key == "ToUnicode" || PdfName.XOBJECT.Equals(s.GetAsName(PdfName.TYPE)) && PdfName.FORM.Equals(s.GetAsName(PdfName.SUBTYPE))) {
					var sb = StringBuilderCache.Acquire();
					byte b;
					int l = bs.Length;
					int p1 = 0, p2 = 0;
					for (int i = 0; i < l; i++) {
						b = bs[i];
						if (b == 0x0A || b == 0x0D || i + 1 == l) {
							p2 = i;
							if (i > 2 && bs[i - 2] == 'T' && (bs[i - 1] == 'J' || bs[i - 1] == 'j')) {
								// is a text operation
								ExportStreamTextContent(bs, sb, p1, p2);
								sb.AppendLine();
							}
							else {
								sb.Append(Encoding.ASCII.GetString(bs, p1, p2 - p1));
								sb.AppendLine();
							}
							if (b == 0x0D && i + 1 < l && bs[i + 1] == 0x0A) {
								i++;
							}
							p1 = i + 1;
						}
					}

					if (sb.Length == 0) {
						if (l > 2 && bs[l - 2] == 'T' && (bs[l - 1] == 'J' || bs[l - 1] == 'j')) {
							ExportStreamTextContent(bs, sb, 0, l);
						}
						else {
							sb.Append(Encoding.Default.GetString(bs, p1, p2));
						}
					}
					writer.WriteCData(StringBuilderCache.GetStringAndRelease(sb));
				}
				else if (key == "Metadata") {
					using (var ms = new MemoryStream(bs))
					using (var sr = new StreamReader(ms, true)) {
						// strip byte-order-marks
						writer.WriteCData(sr.ReadToEnd().Replace("\uFFFE", "&#xFFFE;").Replace("\uFEFF", "&#xFEFF;"));
					}
				}
				else {
					ExportRawStreamContent(writer, bs);
				}
			}
			else {
				//_streamID++;
				//string fileName = String.Concat (_options.ExtractImagePath, (_options.ExtractImagePath.EndsWith ("\\") ? String.Empty : "\\"), (_activePage != 0 ? _activePage.ToString () + "-" : String.Empty), _streamID, ".bin");
				//using (FileStream f = new FileStream (fileName, FileMode.Create)) {
				//    f.Write (bs, 0, bs.Length);
				//}
				//writer.WriteAttributeString ("path", Path.GetFileName (fileName));

				ExportRawStreamContent(writer, bs);
			}
			writer.WriteEndElement();
		}

		void ExportRawStreamContent(XmlWriter writer, byte[] bs) {
			writer.WriteAttributeString(Constants.Content.Length, bs.Length.ToText());
			if (_options.ExportBinaryStream == 0) {
				writer.WriteBinHex(bs, 0, bs.Length);
			}
			else {
				writer.WriteBinHex(bs, 0, bs.Length < _options.ExportBinaryStream ? bs.Length : _options.ExportBinaryStream);
				if (bs.Length > _options.ExportBinaryStream) {
					writer.WriteString("....");
				}
			}
		}

		static void ExportStreamTextContent(byte[] bs, StringBuilder sb, int p1, int p2) {
			bool inText = false;
			bool escape = false;
			char ch;
			int beginTextPosition = 0;
			for (int c = p1; c < p2; c++) {
				ch = (char)bs[c];
				switch (ch) {
					case '\\':
						if (escape) {
							escape = false;
							sb.Append(ch);
						}
						else {
							escape = true;
						}
						continue;
					case '(':
						if (escape) {
							sb.Append(ch);
							escape = false;
						}
						else {
							inText = true;
							beginTextPosition = c;
							sb.Append(ch);
						}
						continue;
					case ')':
						if (escape) {
							escape = false;
						}
						else {
							inText = false;
							//sb.Append (Encoding.BigEndianUnicode.GetString (buffer, beginTextPosition + 1, c - 1 - beginTextPosition));
						}
						sb.Append(ch);
						continue;
					default:
						if (escape) {
							escape = false;
						}
						if (inText == false) {
							sb.Append(ch);
						}
						else {
							sb.Append(((byte)ch).ToString("X2"));
						}
						break;
				}
			}
		}

		internal void ExtractPage(PdfReader reader, XmlWriter writer, params int[] pageNumbers) {
			var p = new ExportProcessor(this, writer, _options);
			foreach (var pageNum in pageNumbers) {
				ExtractPage(pageNum, reader, writer, p);
			}
		}

		void ExtractPage(int pageNum, PdfReader reader, XmlWriter writer, ExportProcessor exportProcessor) {
			writer.WriteStartElement(Constants.Content.Page);
			_exportPath.Add(Constants.Content.Page);
			writer.WriteAttributeString(Constants.Content.PageNumber, pageNum.ToText());
			writer.WriteAttributeString(Constants.Content.ResourceID, reader.GetPageOrigRef(pageNum).ToString());
			_imageExporter?.ExtractPageImages(reader, pageNum);

			if (_options.ExtractPageDictionary) {
				ExportPdfDictionary(writer, reader.GetPageNRelease(pageNum));
			}
			if (_options.ExportContentOperators || _options.ExportDecodedText) {
				if (_options.ExportContentOperators) {
					writer.WriteStartElement(Constants.Content.Operators);
				}
				exportProcessor.Reset();
				exportProcessor.ProcessContent(reader.GetPageContent(pageNum), reader.GetPageNRelease(pageNum).GetAsDict(PdfName.RESOURCES));
				if (_options.ExportContentOperators) {
					exportProcessor.End();
					writer.WriteEndElement();
				}
			}
			//if (_imageExporter != null && _imageExporter.InfoList.Count > 0) {
			//    writer.WriteStartElement ("images", Constants.ContentNamespace);
			//    foreach (var item in _imageExporter.InfoList) {
			//        writer.WriteStartElement ("image", Constants.ContentNamespace);
			//        writer.WriteAttributeString (Constants.Content.ResourceID, Constants.ContentNamespace, item.PdfRef.ToString ());
			//        writer.WriteAttributeString ("位置", String.Concat (item.X, ", ", item.Y, ", ", item.Z));
			//        writer.WriteEndElement ();
			//    }
			//    writer.WriteEndElement ();
			//}
			writer.WriteEndElement();
			_exportPath.RemoveAt(_exportPath.Count - 1);
		}

		sealed class ExportProcessor : PdfContentStreamProcessor
		{
			readonly PdfContentExport _export;
			readonly XmlWriter _writer;
			readonly bool _writeOperators;
			readonly List<TextInfo> _textContainer;
			readonly List<NameValuePair> _operands = new List<NameValuePair>();
			int _writerLevel;
			float _textWidth;


			public ExportProcessor(PdfContentExport export, XmlWriter writer, ExporterOptions options) {
				_export = export;
				_writer = writer;
				_writeOperators = options.ExportContentOperators;
				_textContainer = options.ExportDecodedText ? new List<TextInfo>() : null;
				PopulateOperators();
				RegisterContentOperator("TJ", new AccumulatedShowTextArray());
			}

			public void End() {
				if (_writeOperators == false) {
					return;
				}
				while (_writerLevel > 0) {
					_writer.WriteEndElement();
					_writerLevel--;
				}
			}

			internal List<TextInfo> TextInfoList => _textContainer;
			protected override void DisplayPdfString(PdfString str) {
				var gs = CurrentGraphicState;
				var font = gs.Font;
				float totalWidth = 0;
				foreach (var c in font.DecodeText(str)) {
					float w = font.GetWidth(c) / 1000.0f;
					float wordSpacing = c == ' ' ? gs.WordSpacing : 0f;
					totalWidth += (w * gs.FontSize + gs.CharacterSpacing + wordSpacing) * gs.HorizontalScaling;
				}

				_textWidth = totalWidth;
			}

			protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
				base.InvokeOperator(oper, operands);
				string o, fn, t = null;
				bool open = false;
				bool hasDescriptiveOperands = false;
				o = oper.ToString();
				PdfPageCommand.GetFriendlyCommandName(o, out fn);
				_operands.Clear();
				switch (o) {
					case "BDC":
					case "BMC":
					case "BT":
					case "BX":
					case "q":
						open = true;
						_writerLevel++;
						goto default;
					case "EMC":
					case "ET":
					case "EX":
					case "Q":
						if (_writerLevel > 0 && _writeOperators) {
							_writer.WriteEndElement();
							_writerLevel--;
						}
						return;
					case "TJ":
						var array = (PdfArray)operands[0];
						using (var ms = new MemoryStream(array.Length)) {
							foreach (PdfObject item in array.ArrayList) {
								if (item.Type == PdfObject.STRING) {
									ms.Write((item as PdfString).GetBytes(), 0, item.Length);
								}
							}
							t = CurrentGraphicState.Font.DecodeText(new PdfString(ms.ToArray()));
						}
						AddTextInfo(t);
						_operands.Add(new NameValuePair(Constants.Content.OperandNames.Text, t));
						goto default;
					case "Tj":
					case "'":
					case "\"":
						t = CurrentGraphicState.Font.DecodeText(operands[0] as PdfString);
						AddTextInfo(t);
						_operands.Add(new NameValuePair(Constants.Content.OperandNames.Text, t));
						goto default;
					case "Tf":
						_operands.Add(new NameValuePair(Constants.Font.ThisName, CurrentGraphicState.Font.FontName));
						_operands.Add(new NameValuePair(Constants.Content.OperandNames.ResourceName, operands[0].ToString()));
						_operands.Add(new NameValuePair(Constants.Content.OperandNames.Size, operands[1].ToString()));
						hasDescriptiveOperands = true;
						goto default;
					case "cm":
					case "Tm":
						_operands.Add(new NameValuePair(Constants.Content.Operands, GetOperandsTextValue(operands)));
						hasDescriptiveOperands = true;
						goto default;
					case "BI":
						//case "EI":
						goto default;
					default:
						if (_writeOperators == false) {
							return;
						}
						_writer.WriteStartElement(fn ?? "未知操作符");
						_writer.WriteAttributeString("name", o);
						foreach (var item in _operands) {
							_writer.WriteAttributeString(item.Key, PdfHelper.GetValidXmlString(item.Value));
						}
						break;
				}
				if (hasDescriptiveOperands == false) {
					if (operands.Count > 0) {
						// 删除操作符
						operands.RemoveAt(operands.Count - 1);
					}
					if (operands.Count > 0) {
						_writer.WriteStartElement(Constants.Content.Operands);
						_export.ExportArrayContent(operands, _writer, true);
						_writer.WriteEndElement();
						if (o == "BI") {
							_writer.WriteStartElement(Constants.Content.Raw);
							_export.ExportRawStreamContent(_writer, (operands[0] as PdfImageData).RawBytes);
							_writer.WriteEndElement();
						}
					}
				}
				if (open == false && _writerLevel >= 0) {
					_writer.WriteEndElement();
				}
			}

			static string GetOperandsTextValue(List<PdfObject> operands) {
				operands.RemoveAt(operands.Count - 1);
				return String.Join(" ", operands.ConvertAll((po) => po.Type == PdfObject.NUMBER ? ((PdfNumber)po).DoubleValue.ToText() : null).ToArray());
			}

			void AddTextInfo(string t) {
				if (_textContainer != null && t != null) {
					_textContainer.Add(new TextInfo() {
						Text = t,
						Size = CurrentGraphicState.FontSize * TextMatrix[Matrix.I11],
						Region = new Bound(TextMatrix[Matrix.I31], TextMatrix[Matrix.I32], TextMatrix[Matrix.I31] + _textWidth, 0),
						// note 不要设置该属性，否则可能在文档具有大量字体时占用过高的内存
						//Font = CurrentGraphicState.Font
					});
				}
			}
		}

	}
}
