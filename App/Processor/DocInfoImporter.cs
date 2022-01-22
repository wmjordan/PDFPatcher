using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Rectangle = iTextSharp.text.Rectangle;

namespace PDFPatcher.Processor;

internal sealed class DocInfoImporter
{
	private static readonly char[] __ValueArraySplitChars = { ' ', '\t', ',', ';' };

	private readonly ImporterOptions _options;
	private readonly float _unitFactor;

	/// <summary>
	///     从 <see cref="XmlDocument" /> 实例创建导入器（支持无信息文件的补丁操作）。
	/// </summary>
	/// <param name="options">导入器的选项。</param>
	/// <param name="infoDoc">包含信息文件的 <see cref="XmlDocument" />。</param>
	internal DocInfoImporter(ImporterOptions options, PdfInfoXmlDocument infoDoc) {
		_options = options;
		_unitFactor = 1;
		InfoDoc = infoDoc;
	}

	internal DocInfoImporter(ImporterOptions options, string infoDocFile) {
		if (string.IsNullOrEmpty(infoDocFile)) {
			throw new FileNotFoundException("找不到信息文件。");
		}

		PdfInfoXmlDocument infoDoc = new();
		if (FileHelper.HasExtension(infoDocFile, Constants.FileExtensions.Txt)) {
			OutlineManager.ImportSimpleBookmarks(infoDocFile, infoDoc);
		}
		else {
			infoDoc.Load(infoDocFile);
		}

		// 设置单位转换因数
		_unitFactor = GetUnitFactor(infoDoc.DocumentElement);
		InfoDoc = infoDoc;
		_options = options;
	}

	internal DocInfoImporter(ImporterOptions importerOptions, PdfReader pdf, PatcherOptions patcherOptions,
		IXPathNavigable bookmarkRoot) {
		ViewerOptions v = patcherOptions.ViewerPreferences;
		ExporterOptions o = new() {
			ExportBookmarks = bookmarkRoot == null
							  && (v.RemoveZoomRate
								  || v.CollapseBookmark != BookmarkStatus.AsIs
								  || v.ForceInternalLink
								  || AppContext.Encodings.BookmarkEncoding != null
								  || (patcherOptions.UnifiedPageSettings.ScaleContent
									  && patcherOptions.UnifiedPageSettings.PaperSize.SpecialSize !=
									  SpecialPaperSize.AsPageSize)),
			ExportDocProperties = false,
			ExtractPageLinks = (v.RemoveZoomRate
								|| v.ForceInternalLink
								|| (patcherOptions.UnifiedPageSettings.ScaleContent
									&& patcherOptions.UnifiedPageSettings.PaperSize.SpecialSize !=
									SpecialPaperSize.AsPageSize))
							   && patcherOptions.RemoveAnnotations == false,
			ExtractPageSettings = false,
			ExportViewerPreferences = true
		};
		o.UnitConverter.Unit = Constants.Units.Point;
		DocInfoExporter exp = new(pdf, o);
		Tracker.SetProgressGoal(exp.EstimateWorkload());
		using MemoryStream ms = new();
		using (XmlWriter w = XmlWriter.Create(ms)) {
			w.WriteStartElement(Constants.PdfInfo);
			w.WriteAttributeString(Constants.Info.ProductVersion, Constants.InfoDocVersion);
			exp.ExportDocument(w);
			if (bookmarkRoot != null) {
				XPathNavigator bookmark = bookmarkRoot.CreateNavigator();
				bookmark.WriteSubtree(w);
				//if (bookmark.MoveToFirstChild()) {
				//	bookmark.WriteSubtree(w);
				//}
				//while (bookmark.MoveToNext()) {
				//	bookmark.WriteSubtree(w);
				//}
			}

			w.WriteEndElement();
		}

		ms.Flush();
		ms.Position = 0;
		PdfInfoXmlDocument x = new();
		x.Load(ms);
		_options = importerOptions;
		_unitFactor = 1;
		InfoDoc = x;
	}

	internal PdfInfoXmlDocument InfoDoc { get; }

	internal static float GetUnitFactor(XmlElement root) {
		XmlNode n = root.SelectSingleNode(Constants.Units.ThisName + "/@" + Constants.Units.Unit);
		string unit = n?.Value;
		if (string.IsNullOrEmpty(unit) == false) {
			return ValueHelper.MapValue(unit, Constants.Units.Names, Constants.Units.Factors, 1);
		}

		return 1;
	}

	internal BookmarkContainer GetBookmarks() {
		if (InfoDoc == null) {
			return null;
		}

		BookmarkContainer be = InfoDoc.BookmarkRoot;
		if (be?.HasSubBookmarks != true) {
			return be;
		}

		if (be.GetAttribute(Constants.DestinationAttributes.FirstPageNumber).TryParse(out int bookmarkPageShift)) {
			bookmarkPageShift--;
		}

		PreprocessBookmark(be, bookmarkPageShift);

		return be;
	}

	/// <summary>
	///     偏移页码位置并转换书签目标的尺寸单位。
	/// </summary>
	/// <param name="source"></param>
	/// <param name="bookmarkPageShift"></param>
	private void PreprocessBookmark(XmlNode source, int bookmarkPageShift) {
		foreach (BookmarkElement b in source.ChildNodes) {
			if (b == null) {
				continue;
			}

			int pageShift = GetPageShift(b, bookmarkPageShift);
			ShiftPageAndConvertUnits(b, pageShift);
			PreprocessBookmark(b, pageShift);
		}
	}

	private static int GetPageShift(XmlElement element, int baseShift) {
		int shift = baseShift;
		string s = element.GetAttribute(Constants.DestinationAttributes.FirstPageNumber);
		if (string.IsNullOrEmpty(s) != false) {
			return shift;
		}

		if (s.TryParse(out shift)) {
			shift--;
		}
		else {
			shift = baseShift;
		}

		return shift;
	}

	private void ShiftPageAndConvertUnits(XmlElement destination, int pageShift) {
		foreach (XmlAttribute a in destination.Attributes) {
			switch (a.Name) {
				case Constants.Coordinates.Top:
				case Constants.Coordinates.Left:
				case Constants.Coordinates.Bottom:
				case Constants.Coordinates.Right:
					if (_unitFactor != 1 && a.Value.TryParse(out float c)) {
						a.Value = (c * _unitFactor).ToText();
					}

					break;
				case Constants.DestinationAttributes.Page:
					if (pageShift != 0 && a.Value.TryParse(out int pageNum) /* && pageNum > 0*/) {
						pageNum += pageShift;
						destination.SetAttribute(a.Name, pageNum.ToText());
					}

					break;
			}
		}
	}

	internal GeneralInfo ImportDocumentInformation() {
		if (_options.ImportDocProperties == false || InfoDoc == null) {
			return null;
		}

		Tracker.TraceMessage("导入文档元数据信息。");
		DocumentInfoElement info = InfoDoc.InfoNode;
		return info == null
			? null
			: new GeneralInfo {
				Title = info.Title,
				Author = info.Author,
				Keywords = info.Keywords,
				Subject = info.Subject,
				Creator = info.Creator,
				Producer = info.Producer
			};
	}

	internal static void ImportDocumentInformation(GeneralInfo info, PdfReader pdf, string pdfFileName) {
		if (info == null) {
			return;
		}

		PdfDictionary d = pdf.Trailer.GetAsDict(PdfName.INFO);
		if (d == null) {
			d = new PdfDictionary();
			pdf.Trailer.Put(PdfName.INFO, d);
		}

		UpdateInfoValue(d, PdfName.TITLE, info.Title, pdfFileName);
		UpdateInfoValue(d, PdfName.SUBJECT, info.Subject, pdfFileName);
		UpdateInfoValue(d, PdfName.AUTHOR, info.Author, pdfFileName);
		UpdateInfoValue(d, PdfName.KEYWORDS, info.Keywords, pdfFileName);
		UpdateInfoValue(d, PdfName.CREATOR, info.Creator, pdfFileName);
		UpdateInfoValue(d, PdfName.PRODUCER, info.Producer, pdfFileName);

		if (info.RewriteXmp == false) {
			return;
		}

		PRStream m = pdf.Catalog.Locate<PRStream>(PdfName.METADATA);
		if (m == null) {
			pdf.Catalog.Put(PdfName.METADATA, new PRStream(pdf, new byte[0]));
		}

		try {
			XmpWriter xw = new(new MemoryStream(), d);
			MemoryStream s = new();
			xw.Serialize(s);
			m.SetData(s.ToArray(), false);
		}
		catch (Exception) {
			Trace.WriteLine("读写 XMP 属性时出现错误。");
		}
	}

	internal static void UpdateInfoValue(PdfDictionary info, PdfName name, string value, FilePath pdfFileName) {
		if (value != null) {
			if (value.Contains(Constants.FileNameMacros.FileName)) {
				value = value.Replace(Constants.FileNameMacros.FileName, pdfFileName.FileNameWithoutExtension);
			}

			if (value.Contains(Constants.FileNameMacros.FolderName)) {
				value = value.Replace(Constants.FileNameMacros.FolderName, pdfFileName.Directory.FileName);
			}
		}

		if (info.Contains(name) && value == null) {
			PdfString s = info.GetAsString(name);
			if (s != null) {
				value = s.ToUnicodeString();
			}
		}

		if (value != null) {
			value = value.Trim();
		}

		info.Put(name, string.IsNullOrEmpty(value) ? null : value.ToPdfString());
	}

	internal static void ImportDocumentInformation(GeneralInfo info, Document doc) {
		if (info == null) {
			return;
		}

		doc.AddTitle(info.Title);
		if (string.IsNullOrEmpty(info.Subject) == false) {
			doc.AddSubject(info.Subject);
		}

		if (string.IsNullOrEmpty(info.Author) == false) {
			doc.AddAuthor(info.Author);
		}

		if (string.IsNullOrEmpty(info.Keywords) == false) {
			doc.AddKeywords(info.Keywords);
		}
	}

	internal PdfPageLabels ImportPageLabels() {
		if (_options.ImportViewerPreferences == false || InfoDoc == null) {
			return null;
		}

		Tracker.TraceMessage("导入页码设置。");
		XmlNodeList pn = InfoDoc.DocumentElement.SelectNodes(
			Constants.PageLabels + "/" + Constants.PageLabelsAttributes.Style + "[@" +
			Constants.PageLabelsAttributes.PageNumber + "]");
		PdfPageLabels pls = new();
		bool hasPageLabels = false;
		foreach (XmlElement item in pn) {
			if (item.GetAttribute(Constants.PageLabelsAttributes.PageNumber).TryParse(out int physicalPage) == false ||
				physicalPage < 1) {
				Trace.WriteLine(string.Concat("在“", Constants.PageLabels, "”的“", Constants.PageLabelsAttributes.Style,
					"”元素中，必须指定大于或等于 1 的“", Constants.PageLabelsAttributes.PageNumber, "”属性。"));
				continue;
			}

			if (item.GetAttribute(Constants.PageLabelsAttributes.StartPage).TryParse(out int firstPage) == false ||
				firstPage < 1) {
				firstPage = 1;
			}

			string prefix = item.GetAttribute(Constants.PageLabelsAttributes.Prefix);
			int numberStyle = ValueHelper.MapValue(
				item.GetAttribute(Constants.PageLabelsAttributes.Style),
				Constants.PageLabelStyles.Names,
				Constants.PageLabelStyles.Values,
				PdfPageLabels.DECIMAL_ARABIC_NUMERALS);
			pls.AddPageLabel(physicalPage, numberStyle, prefix, firstPage);
			hasPageLabels = true;
		}

		return hasPageLabels ? pls : null;
	}

	internal static PdfPageLabels ImportPageLabels(List<PageLabel> labels) {
		if (labels == null || labels.Count == 0) {
			return null;
		}

		PdfPageLabels pls = new();
		int i = 0;
		foreach (var item in labels.Where(item => item.PageNumber > 0 && item.StartPage > 0)) {
			pls.AddPageLabel(item.PageNumber,
				ValueHelper.MapValue(item.Style, Constants.PageLabelStyles.Names, Constants.PageLabelStyles.Values,
					PdfPageLabels.DECIMAL_ARABIC_NUMERALS),
				item.Prefix,
				item.StartPage
			);
			i++;
		}

		return i > 0 ? pls : null;
	}

	internal void ImportPageLinks(PdfReader r, PdfStamper w) {
		if (InfoDoc == null) {
			return;
		}

		XmlNodeList ls = InfoDoc.DocumentElement.SelectNodes(Constants.PageLink + "/" +
															 Constants.PageLinkAttributes.Link + "[@" +
															 Constants.PageLinkAttributes.PageNumber + "]");
		if (ls == null || ls.Count == 0) {
			return;
		}

		Tracker.TraceMessage("导入页面内连接。");
		if (_options.KeepPageLinks == false) {
			r.ClearPageLinks();
		}

		int pageCount = r.NumberOfPages;
		foreach (XmlElement item in ls) {
			if (item.GetAttribute(Constants.PageLinkAttributes.PageNumber).TryParse(out int pageNum) == false) {
				Trace.WriteLine("页码属性格式不正确");
				continue;
			}

			if (item.GetAttribute(Constants.DestinationAttributes.FirstPageNumber).TryParse(out int pageOffset)) {
				pageNum += --pageOffset;
			}

			if (pageNum > pageCount) {
				Trace.WriteLine("页码 " + pageNum + " 超出文档最大页数。");
				continue;
			}

			float[] acc = ImportRectangle(item);
			if (acc == null) {
				Trace.WriteLine("区域坐标不为 4 个。");
				continue;
			}

			acc = Array.ConvertAll(acc, a => UnitConverter.ToPoint(a, _unitFactor));
			Rectangle region = new(acc[0], acc[1], acc[2], acc[3]);

			string border = item.GetAttribute(Constants.PageLinkAttributes.Border);
			PdfAnnotation ann = new(w.Writer, region);
			ann.Put(PdfName.TYPE, PdfName.ANNOT);
			ann.Put(PdfName.SUBTYPE, PdfName.LINK);
			ann.Put(PdfName.P, w.Writer.GetPageReference(pageNum));
			string hl = item.GetAttribute(Constants.PageLinkAttributes.Style);
			if (string.IsNullOrEmpty(hl) == false) {
				PdfName h = hl switch {
					"无" => PdfName.N,
					"取反内容" => PdfName.I,
					"取反边框" => PdfName.O,
					"按下" => PdfName.P,
					_ => PdfName.I
				};

				ann.Put(PdfName.H, h);
			}

			if (item.SelectSingleNode(Constants.PageLinkAttributes.LinkAction) is XmlElement action) {
				ShiftPageAndConvertUnits(action, 0);
				ImportAction(w.Writer, ann, action, pageCount, false);
			}

			if (ann == null) {
				continue;
			}

			ImportColor(item, ann);
			if (string.IsNullOrEmpty(border) == false) {
				ImportBorder(border, ann);
			}
			else {
				if (item.SelectSingleNode("边框样式") is XmlElement bse) {
					PdfDictionary bs = ImportPdfBorderStyle(bse);
					if (bs != null) {
						ann.Put(PdfName.BS, bs);
					}
				}
			}

			w.AddAnnotation(ann, pageNum);
		}
	}

	internal static void ImportColor(XmlElement item, PdfDictionary dict) {
		if (item.HasAttribute(Constants.Color)) {
			string s = item.GetAttribute(Constants.Color);
			if (s == Constants.Colors.Transparent) {
				dict.Put(PdfName.C, new PdfArray());
			}

			Color c = Color.FromName(s);
			dict.Put(PdfName.C, new PdfArray(new[] { c.R / 255f, c.G / 255f, c.B / 255f }));
		}
		else if (item.HasAttribute(Constants.Colors.Red) || item.HasAttribute(Constants.Colors.Green) ||
				 item.HasAttribute(Constants.Colors.Blue)) {
			dict.Put(PdfName.C,
				new PdfArray(new[] {
					item.GetValue(Constants.Colors.Red, 0f), item.GetValue(Constants.Colors.Green, 0f),
					item.GetValue(Constants.Colors.Blue, 0f)
				}));
		}
		else if (item.HasAttribute(Constants.Colors.Gray)) {
			dict.Put(PdfName.C, new PdfArray(new[] { item.GetValue(Constants.Colors.Gray, 0f) }));
		}
		else if (item.HasAttribute(Constants.Colors.Black) || item.HasAttribute(Constants.Colors.Cyan) ||
				 item.HasAttribute(Constants.Colors.Magenta) || item.HasAttribute(Constants.Colors.Yellow)) {
			dict.Put(PdfName.C,
				new PdfArray(new[] {
					item.GetValue(Constants.Colors.Cyan, 0f), item.GetValue(Constants.Colors.Magenta, 0f),
					item.GetValue(Constants.Colors.Yellow, 0f), item.GetValue(Constants.Colors.Black, 0f)
				}));
		}
	}

	private static void ImportBorder(string border, PdfDictionary ann) {
		int[] bs = ToInt32Array(border);
		PdfBorderArray a;
		if (bs == null) {
			return;
		}

		switch (bs.Length) {
			case 3:
				a = new PdfBorderArray(bs[0], bs[1], bs[2]);
				break;
			case 4:
				int[] dp = new int[bs.Length - 3];
				bs.CopyTo(dp, 3);
				a = new PdfBorderArray(bs[0], bs[1], bs[2], GetPdfDashPattern(dp));
				break;
			default:
				a = null;
				break;
		}

		if (a != null) {
			ann.Put(PdfName.BORDER, a);
		}
	}

	internal static void ImportAction(PdfWriter writer, PdfDictionary dict, XmlElement map, int maxPageNumber,
		bool namedAsNames) {
		string action = map.GetAttribute(Constants.DestinationAttributes.Action);
		if (string.IsNullOrEmpty(action)) {
			action = Constants.ActionType.Goto;
		}

		string p;
		PdfDictionary fs;
		switch (action) {
			case Constants.ActionType.Goto:
				if (string.IsNullOrEmpty(p = map.GetAttribute(Constants.DestinationAttributes.Named)) == false) {
					if (namedAsNames) {
						dict.Put(PdfName.DEST, new PdfName(p));
					}
					else {
						dict.Put(PdfName.DEST, p.ToPdfString());
					}
				}
				else if (string.IsNullOrEmpty(p = map.GetAttribute(Constants.DestinationAttributes.Page)) == false) {
					PdfArray ar = new();
					if (p.TryParse(out int pn) == false || pn > maxPageNumber) {
						return;
					}

					CreateDestination(writer, map, p, ar, false);
					dict.Put(PdfName.DEST, ar);
				}

				break;
			case Constants.ActionType.GotoR:
				PdfDictionary dic = new();
				if (string.IsNullOrEmpty(p = map.GetAttribute(Constants.DestinationAttributes.Named)) == false) {
					dic.Put(PdfName.D, p.ToPdfString());
				}
				else if (string.IsNullOrEmpty(p = map.GetAttribute(Constants.DestinationAttributes.NamedN)) == false) {
					dic.Put(PdfName.D, new PdfName(p));
				}
				else if (string.IsNullOrEmpty(p = map.GetAttribute(Constants.DestinationAttributes.Page)) == false) {
					p.TryParse(out int pn);
					if (pn > 0) {
						pn--;
					}

					PdfArray ar = new();
					CreateDestination(writer, map, pn.ToText(), ar, true);
					dic.Put(PdfName.D, ar);
				}

				p = map.GetAttribute(Constants.DestinationAttributes.Path);
				if (dic.Size > 0 && p != null) {
					dic.Put(PdfName.S, PdfName.GOTOR);
					fs = new PdfDictionary(PdfName.FILESPEC);
					fs.Put(PdfName.F, new PdfString(p, Encoding.Default.WebName));
					fs.Put(PdfName.UF, new PdfString(p, PdfObject.TEXT_UNICODE));
					dic.Put(PdfName.F, fs);
					string nw = map.GetAttribute(Constants.DestinationAttributes.NewWindow);
					if (nw != null) {
						dic.Put(PdfName.NEWWINDOW, nw == Constants.Boolean.True);
					}

					dict.Put(PdfName.A, dic);
				}

				break;
			case Constants.ActionType.Uri:
				p = map.GetAttribute(Constants.DestinationAttributes.Path);
				if (string.IsNullOrEmpty(p) == false) {
					PdfDictionary u = new();
					u.Put(PdfName.S, PdfName.URI);
					u.Put(PdfName.URI, p);
					dict.Put(PdfName.A, u);
				}

				break;
			case Constants.ActionType.Launch:
				p = map.GetAttribute(Constants.DestinationAttributes.Path);
				if (string.IsNullOrEmpty(p) == false) {
					PdfDictionary l = new();
					l.Put(PdfName.S, PdfName.LAUNCH);
					fs = new PdfDictionary(PdfName.FILESPEC);
					fs.Put(PdfName.F, new PdfString(p, Encoding.Default.WebName));
					fs.Put(PdfName.UF, new PdfString(p, PdfObject.TEXT_UNICODE));
					l.Put(PdfName.F, fs);
					dict.Put(PdfName.A, l);
				}

				break;
			case Constants.ActionType.Javascript:
				p = map.GetAttribute(Constants.DestinationAttributes.ScriptContent);
				dict.Put(PdfName.A, string.IsNullOrEmpty(p) == false ? PdfAction.JavaScript(p, writer) : null);
				break;
			default:
				Tracker.TraceMessage(Tracker.Category.Alert, string.Concat("不支持动作：", action));
				break;
		}
	}

	private static void CreateDestination(PdfWriter writer, XmlElement map, string p, PdfArray ar, bool isRemote) {
		bool useDefaultPos = false;
		float[] pos = new float[4];
		int posItemCount = 0;
		if (p.TryParse(out int pn) == false || pn < (isRemote ? 0 : 1)) {
			return;
		}

		PdfIndirectReference pr = null;
		if (isRemote == false) {
			pr = writer.GetPageReference(pn);
			ar.Add(pr);
		}
		else {
			ar.Add(new PdfNumber(pn));
		}

		Rectangle box;
		switch (p = map.GetAttribute(Constants.DestinationAttributes.View)) {
			case Constants.DestinationAttributes.ViewType.XYZ:
				goto default;
			case Constants.DestinationAttributes.ViewType.Fit:
			case Constants.DestinationAttributes.ViewType.FitB:
				posItemCount = 0;
				break;
			case Constants.DestinationAttributes.ViewType.FitH:
			case Constants.DestinationAttributes.ViewType.FitBH:
				posItemCount = 1;
				string top = map.GetAttribute(Constants.Coordinates.Top);
				if (top == Constants.Coordinates.Unchanged) {
					pos[0] = float.NaN;
				}
				else if (top.TryParse(out pos[0]) == false) {
					if (pr != null && (box = (PdfReader.GetPdfObject(pr) as PdfDictionary).GetPageVisibleRectangle()) !=
						null) {
						pos[0] = box.Top;
					}
					else {
						useDefaultPos = true;
					}
				}

				break;
			case Constants.DestinationAttributes.ViewType.FitV:
			case Constants.DestinationAttributes.ViewType.FitBV:
				posItemCount = 1;
				string left = map.GetAttribute(Constants.Coordinates.Left);
				if (left == Constants.Coordinates.Unchanged) {
					pos[0] = float.NaN;
				}
				else if (left.TryParse(out pos[0]) == false) {
					if (pr != null && (box = (PdfReader.GetPdfObject(pr) as PdfDictionary).GetPageVisibleRectangle()) !=
						null) {
						pos[0] = box.Left;
					}
					else {
						useDefaultPos = true;
					}
				}

				break;
			case Constants.DestinationAttributes.ViewType.FitR:
				pos = ImportRectangle(map);
				if (pos == null) {
					useDefaultPos = true;
				}

				break;
			default:
				posItemCount = 3;
				left = map.GetAttribute(Constants.Coordinates.Left);
				top = map.GetAttribute(Constants.Coordinates.Top);
				if (left.TryParse(out pos[0]) == false) {
					pos[0] = float.NaN;
				}

				if (top.TryParse(out pos[1]) == false) {
					pos[1] = float.NaN;
				}

				if (map.GetAttribute(Constants.Coordinates.ScaleFactor).TryParse(out pos[2]) == false || pos[2] < 0) {
					pos[2] = float.NaN;
				}

				if (float.IsNaN(pos[0]) && float.IsNaN(pos[1])
										&& left != Constants.Coordinates.Unchanged &&
										top != Constants.Coordinates.Unchanged) {
					useDefaultPos = true;
				}

				break;
		}

		if (useDefaultPos) {
			ar.Add(PdfName.XYZ);
			if (isRemote) {
				ar.Add(new float[] { 0, 10000, 0 });
				return;
			}

			if (PdfReader.GetPdfObject(pr) is PdfDictionary page) {
				box = page.GetPageVisibleRectangle();
				if (box == null) {
					return;
				}

				if (true) {
					// TODO: 检测页面旋转方向并设置正确的目标
				}

				ar.Add(PdfNull.PDFNULL);
				ar.Add(new PdfNumber(box.Top));
				ar.Add(PdfNull.PDFNULL);
			}
			else {
				ar.Add(new float[] { 0, 10000, 0 });
			}
		}
		else {
			ar.Add(ValueHelper.MapValue(p,
				Constants.DestinationAttributes.ViewType.Names,
				Constants.DestinationAttributes.ViewType.PdfNames,
				PdfName.XYZ));
			for (int i = 0; i < posItemCount; i++) {
				ref float v = ref pos[i];
				if (float.IsNaN(v)) {
					ar.Add(PdfNull.PDFNULL);
				}
				else {
					if (v > 10000) {
						v = 10000;
					}

					ar.Add(new PdfNumber(v));
				}
			}
		}
	}

	private static float[] ImportRectangle(XmlElement map) {
		float[] pos = new float[4];
		if (map.GetAttribute(Constants.Coordinates.Left).TryParse(out pos[0]) == false || pos[0] < 0
			|| map.GetAttribute(Constants.Coordinates.Bottom).TryParse(out pos[1]) == false || pos[1] < 0
			|| map.GetAttribute(Constants.Coordinates.Right).TryParse(out pos[2]) == false || pos[2] < 0
			|| map.GetAttribute(Constants.Coordinates.Top).TryParse(out pos[3]) == false || pos[3] < 0) {
			return null;
		}

		return pos;
	}


	private static PdfDictionary ImportPdfBorderStyle(XmlElement item) {
		string borderWidth = item.GetAttribute("宽度");
		string borderStyle = item.GetAttribute("样式");
		string borderPattern = item.GetAttribute("线形");
		PdfDictionary bs = new(PdfName.BS);
		if (borderWidth.TryParse(out float bw)) {
			bs.Put(PdfName.W, new PdfNumber(bw));
		}

		if (string.IsNullOrEmpty(borderStyle)) {
			return bs;
		}

		PdfName s;
		switch (borderStyle) {
			case "方框":
				s = PdfName.S;
				break;
			case "下划线":
				s = PdfName.U;
				break;
			case "凸起":
				s = PdfName.B;
				break;
			case "凹陷":
				s = PdfName.I;
				break;
			case "虚线":
				s = PdfName.D;
				if (string.IsNullOrEmpty(borderPattern) == false) {
					int[] p = ToInt32Array(borderPattern);
					if (p != null) {
						PdfDashPattern dp = GetPdfDashPattern(p);
						if (dp != null) {
							bs.Put(PdfName.D, dp);
						}
					}
				}

				break;
			default:
				s = new PdfName(borderStyle);
				break;
		}

		bs.Put(PdfName.S, s);
		return bs;
	}

	private static PdfDashPattern GetPdfDashPattern(IList<int> p) {
		PdfDashPattern dp = p.Count switch {
			1 => new PdfDashPattern(p[0]),
			2 => new PdfDashPattern(p[0], p[1]),
			3 => new PdfDashPattern(p[0], p[1], p[2]),
			_ => null
		};

		return dp;
	}

	internal void ImportViewerPreferences(PdfReader r) {
		if (_options.ImportViewerPreferences == false || InfoDoc == null) {
			return;
		}

		Tracker.TraceMessage("导入阅读器设置。");
		if (InfoDoc.DocumentElement.SelectSingleNode(Constants.ViewerPreferences) is not XmlElement ps) {
			return;
		}

		foreach (XmlAttribute item in ps.Attributes) {
			PdfName n;
			PdfObject v;
			switch (item.Name) {
				case Constants.PageLayout:
					v = ValueHelper.MapValue(item.Value, Constants.PageLayoutType.Names,
						Constants.PageLayoutType.PdfNames, PdfName.NONE);
					if (PdfName.NONE.Equals(v) == false) {
						r.Catalog.Put(PdfName.PAGELAYOUT, v);
					}

					continue;
				case Constants.PageMode:
					v = ValueHelper.MapValue(item.Value, Constants.PageModes.Names, Constants.PageModes.PdfNames,
						PdfName.NONE);
					if (PdfName.NONE.Equals(v) == false) {
						r.Catalog.Put(PdfName.PAGEMODE, v);
					}

					continue;
				case Constants.ViewerPreferencesType.Direction:
					n = PdfName.DIRECTION;
					v = ValueHelper.MapValue(item.Value, Constants.ViewerPreferencesType.DirectionType.Names,
						Constants.ViewerPreferencesType.DirectionType.PdfNames, PdfName.NONE);
					if (PdfName.NONE.Equals(v)) {
						continue;
					}

					break;
				default:
					n = ValueHelper.MapValue(item.Name, Constants.ViewerPreferencesType.Names,
						Constants.ViewerPreferencesType.PdfNames, new PdfName(item.Name));
					v = item.Value switch {
						Constants.Boolean.True => PdfBoolean.PDFTRUE,
						Constants.Boolean.False => PdfBoolean.PDFFALSE,
						_ => PdfHelper.ResolvePdfName(item.Value)
					};
					break;
			}

			if (r.Catalog.Contains(PdfName.VIEWERPREFERENCES) == false) {
				r.Catalog.Put(PdfName.VIEWERPREFERENCES, new PdfDictionary());
			}

			r.Catalog.GetAsDict(PdfName.VIEWERPREFERENCES).Put(n, v);
		}
	}

	internal static void OverrideViewerPreferences(ViewerOptions options, PdfReader reader, PdfWriter writer) {
		PdfName v = ValueHelper.MapValue(options.InitialView, Constants.PageLayoutType.Names,
			Constants.PageLayoutType.PdfNames, PdfName.NONE);
		if (PdfName.NONE.Equals(v) == false) {
			(reader != null ? reader.Catalog : writer.ExtraCatalog).Put(PdfName.PAGELAYOUT, v);
		}

		v = ValueHelper.MapValue(options.Direction, Constants.ViewerPreferencesType.DirectionType.Names,
			Constants.ViewerPreferencesType.DirectionType.PdfNames, PdfName.NONE);
		if (PdfName.NONE.Equals(v) == false) {
			if (reader != null) {
				PdfDictionary d = reader.Catalog.GetAsDict(PdfName.VIEWERPREFERENCES);
				if (d == null) {
					d = new PdfDictionary();
					reader.Catalog.Put(PdfName.VIEWERPREFERENCES, d);
				}

				d.Put(PdfName.DIRECTION, v);
			}
			else {
				writer.AddViewerPreference(PdfName.DIRECTION, v);
			}
		}

		v = ValueHelper.MapValue(options.InitialMode, Constants.PageModes.Names, Constants.PageModes.PdfNames,
			PdfName.NONE);
		if (PdfName.NONE.Equals(v) == false) {
			(reader != null ? reader.Catalog : writer.ExtraCatalog).Put(PdfName.PAGEMODE, v);
		}

		if (!options.SpecifyViewerPreferences) {
			return;
		}

		{
			PdfDictionary d = reader != null ? reader.Catalog : writer.ExtraCatalog;
			PdfDictionary p = d.GetAsDict(PdfName.VIEWERPREFERENCES);
			if (p == null) {
				p = new PdfDictionary();
				d.Put(PdfName.VIEWERPREFERENCES, p);
			}

			p.Put(PdfName.CENTERWINDOW, options.CenterWindow ? PdfBoolean.PDFTRUE : null);
			p.Put(PdfName.DISPLAYDOCTITLE, options.DisplayDocTitle ? PdfBoolean.PDFTRUE : null);
			p.Put(PdfName.FITWINDOW, options.FitWindow ? PdfBoolean.PDFTRUE : null);
			p.Put(PdfName.HIDEMENUBAR, options.HideMenu ? PdfBoolean.PDFTRUE : null);
			p.Put(PdfName.HIDETOOLBAR, options.HideToolbar ? PdfBoolean.PDFTRUE : null);
			p.Put(PdfName.HIDEWINDOWUI, options.HideUI ? PdfBoolean.PDFTRUE : null);
		}
	}

	internal void ImportNamedDestinations(PdfReader pdf, PdfWriter w) {
		if (InfoDoc == null) {
			return;
		}

		XmlNodeList ds = InfoDoc.DocumentElement.SelectNodes("命名位置/位置[@名称]");
		if (ds.Count == 0) {
			return;
		}

		Dictionary<string, XmlElement> infoDs = new(ds.Count);
		foreach (XmlElement item in ds) {
			infoDs[item.GetAttribute(Constants.DestinationAttributes.Name)] = item;
		}

		Dictionary<object, PdfObject> pdfDs = pdf.GetNamedDestination();
		foreach (KeyValuePair<string, XmlElement> item in infoDs) {
			if (item.Value.GetAttribute(Constants.DestinationAttributes.Page).TryParse(out int targetPn) == false) {
				Trace.WriteLine("“目标页面”属性的数值格式不正确。");
				continue;
			}

			PdfArray prop = new();
			CreateDestination(w, item.Value, targetPn.ToText(), prop, false);
			//PdfName d = PdfHelper.ResolvePdfName (item.Value.GetAttribute (Constants.DestinationDisplayMode));
			//prop.ArrayList.Clear ();
			//prop.Add (w.GetPageReference (targetPn));
			//prop.Add (d);
			//string disp = item.Value.GetAttribute (Constants.DestinationAttributes.Position);
			//if (String.IsNullOrEmpty (disp)) {
			//    prop.Add (new float[] { 0, 10000, 0 });
			//}
			//else {
			//    float[] ps = PdfHelper.ToSingleArray (disp.ToLower ().Replace ("null", "0"), true);
			//    ps = UnitConverter.ConvertUnit (ps, this._unitFactor);
			//    prop.Add (ps);
			//}
			if (!pdfDs.TryGetValue(item.Key, out PdfObject a)) {
				continue;
			}

			PdfArray sourceD = a as PdfArray;
			sourceD.ArrayList.Clear();
			sourceD.ArrayList.AddRange(prop.ArrayList);
		}
	}

	internal void ImportPageSettings(PdfReader pdf) {
		if (_options.ImportPageSettings == false || InfoDoc == null) {
			return;
		}

		XmlNodeList ps = InfoDoc.DocumentElement.SelectNodes(string.Concat(Constants.Content.PageSettings.ThisName, "/",
			Constants.Content.Page));
		if (ps.Count == 0) {
			return;
		}

		Tracker.TraceMessage("导入页面设置。");
		int pn = pdf.NumberOfPages;
		foreach (XmlElement item in ps) {
			List<PageRange> ranges = PageRangeCollection.Parse(item.GetAttribute(Constants.PageRange), 1, pn, true);
			float[] mb = ToSingleArray(item.GetAttribute(Constants.Content.PageSettings.MediaBox), true);
			float[] cb = ToSingleArray(item.GetAttribute(Constants.Content.PageSettings.CropBox), true);
			float[] tb = ToSingleArray(item.GetAttribute(Constants.Content.PageSettings.TrimBox), true);
			float[] ab = ToSingleArray(item.GetAttribute(Constants.Content.PageSettings.ArtBox), true);
			float[] bb = ToSingleArray(item.GetAttribute(Constants.Content.PageSettings.BleedBox), true);
			int pageFilter = ValueHelper.MapValue(item.GetAttribute(Constants.PageFilterTypes.ThisName),
				Constants.PageFilterTypes.Names,
				Constants.PageFilterTypes.Values, -1);
			if (item.GetAttribute(Constants.Content.PageSettings.Rotation).TryParse(out int rotate)) {
				rotate = rotate / 90 * 90;
			}
			else {
				rotate = -1;
			}

			foreach (var p in from r in ranges from i in r where pageFilter == -1 || i % 2 == pageFilter select pdf.GetPageN(i)) {
				ImportPageBox(mb, p, PdfName.MEDIABOX);
				ImportPageBox(cb, p, PdfName.CROPBOX);
				ImportPageBox(tb, p, PdfName.TRIMBOX);
				ImportPageBox(ab, p, PdfName.ARTBOX);
				ImportPageBox(bb, p, PdfName.BLEEDBOX);
				if (rotate != -1) {
					p.Put(PdfName.ROTATE, new PdfNumber(rotate));
				}
			}
		}
	}

	private bool ImportPageBox(float[] array, PdfDictionary pdfDict, PdfName pdfName) {
		if (array == null) {
			return false;
		}

		if (array.Length == 0) {
			pdfDict.Remove(pdfName);
			return true;
		}

		if (array.Length != 4) {
			return false;
		}

		array = Array.ConvertAll(array, a => { return UnitConverter.ToPoint(a, _unitFactor); });
		pdfDict.Put(pdfName, new PdfArray(array));
		return true;

	}

	public static float[] ToSingleArray(string value) { return ToSingleArray(value, false); }

	public static float[] ToSingleArray(string value, bool allowNegativeNumber) {
		if (value == null) {
			return null;
		}

		if (value.Length == 0) {
			return new float[0];
		}

		string[] parts = value.Split(__ValueArraySplitChars, StringSplitOptions.RemoveEmptyEntries);
		float[] vals = new float[parts.Length];
		bool ok = true;
		for (int i = 0; i < vals.Length; i++) {
			if (parts[i].TryParse(out vals[i]) != false && (allowNegativeNumber != false || !(vals[i] < 0))) {
				continue;
			}

			ok = false;
			break;
		}

		return ok == false ? null : vals;
	}

	public static int[] ToInt32Array(string value) {
		return ToInt32Array(value, __ValueArraySplitChars, false);
	}

	public static int[] ToInt32Array(string value, bool allowNegativeNumber) {
		return ToInt32Array(value, __ValueArraySplitChars, allowNegativeNumber);
	}

	public static int[] ToInt32Array(string value, char[] separators, bool allowNegativeNumber) {
		if (value == null) {
			return null;
		}

		string[] parts = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		int[] vals = new int[parts.Length];
		for (int i = 0; i < vals.Length; i++) {
			if (!parts[i].TryParse(out vals[i]) || (!allowNegativeNumber && vals[i] < 0)) {
				return null;
			}
		}

		return vals;
	}
}