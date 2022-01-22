using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class DocInfoExporter
{
	private const string SimpleBookmarkPageNumLeader = " ………… ";
	private const int OpenDocWorkload = 10;
	private const int BookmarkWorkload = 30;
	private readonly PdfActionExporter _actionExport;
	private readonly PdfContentExport _contentExport;
	private readonly ExporterOptions _options;

	private readonly PdfReader _reader;
	private Dictionary<int, int> _pageReferenceMapper;

	public DocInfoExporter(PdfReader reader, ExporterOptions options) {
		_reader = reader;
		_options = options;
		_contentExport = new PdfContentExport(options);
		_actionExport = new PdfActionExporter(options.UnitConverter);
	}

	private Dictionary<int, int> PageReferenceMapper {
		get {
			if (_pageReferenceMapper == null) {
				_pageReferenceMapper = _reader.GetPageRefMapper();
			}

			return _pageReferenceMapper;
		}
	}

	internal static XmlWriterSettings GetWriterSettings() {
		return new XmlWriterSettings {
			Encoding = AppContext.Exporter.GetEncoding(),
			Indent = true,
			IndentChars = "\t",
			CheckCharacters = false
		};
	}

	internal int EstimateWorkload() {
		int n = _reader.NumberOfPages;
		int load = OpenDocWorkload;
		if (_options.ExportCatalog) {
			load += n / 100;
		}

		if (_options.ExportBookmarks) {
			load += BookmarkWorkload;
		}

		if (_options.ExtractPageLinks) {
			load += n;
		}

		if (_options.ExtractPageContent) {
			load += PageRangeCollection.Parse(_options.ExtractPageRange, 1, n, true).TotalPages;
		}

		if (_options.ExtractPageSettings) {
			load += n;
		}

		return load;
	}

	internal static GeneralInfo RewriteDocInfoWithEncoding(PdfReader pdf, Encoding encoding) {
		try {
			return RewriteDocInfoWithEncoding(pdf.Trailer.GetAsDict(PdfName.INFO), encoding);
		}
		catch (IOException) {
			// 忽略错误的元数据
			return new GeneralInfo();
		}
	}

	internal static GeneralInfo RewriteDocInfoWithEncoding(PdfDictionary info, Encoding encoding) {
		GeneralInfo r = new();
		if (info == null || info.Length == 0) {
			return r;
		}

		PdfDictionary dump = new();
		foreach (KeyValuePair<PdfName, PdfObject> item in info) {
			if (item.Value is not PdfString s) {
				continue;
			}

			PdfName n = item.Key;
			string t = s.Decode(encoding);
			if (PdfName.TITLE.Equals(n)) {
				r.Title = t;
			}
			else if (PdfName.AUTHOR.Equals(n)) {
				r.Author = t;
			}
			else if (PdfName.SUBJECT.Equals(n)) {
				r.Subject = t;
			}
			else if (PdfName.KEYWORDS.Equals(n)) {
				r.Keywords = t;
			}
			else if (PdfName.CREATOR.Equals(n)) {
				r.Creator = t;
			}
			else if (PdfName.PRODUCER.Equals(n)) {
				r.Producer = t;
			}

			dump.Put(n, t);
		}

		if (encoding != null) {
			info.Merge(dump);
		}

		dump = null;
		return r;
	}

	internal static void WriteDocumentInfoAttributes(XmlWriter w, string sourcePath, int numberOfPages) {
		w.WriteAttributeString(Constants.Info.ProductName, Application.ProductName);
		w.WriteAttributeString(Constants.Info.ProductVersion, Constants.InfoDocVersion);
		w.WriteAttributeString(Constants.Info.ExportDate, DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));
		//w.WriteAttributeString (Constants.Info.DocumentName, Path.GetFileNameWithoutExtension (sourceFile));
		w.WriteAttributeString(Constants.Info.DocumentPath, sourcePath);
		w.WriteAttributeString(Constants.Info.PageNumber, numberOfPages.ToText());
	}

	internal void ExportDocument(XmlWriter w) {
		Tracker.IncrementProgress(OpenDocWorkload);
		if (_options.UnitConverter.UnitFactor != 1) {
			w.WriteStartElement(Constants.Units.ThisName);
			w.WriteAttributeString(Constants.Units.Unit, _options.UnitConverter.Unit);
			w.WriteEndElement();
		}

		if (_options.ExportDocProperties) {
			Tracker.TraceMessage("导出文档信息。");
			RewriteDocInfoWithEncoding(_reader, AppContext.Encodings.DocInfoEncoding);
			ExportDocumentInfo(w);
		}

		if (_options.ExportViewerPreferences) {
			Tracker.TraceMessage("导出阅读器设置。");
			ExportViewerPreferences(w);
		}

		if (_options.ExportBookmarks || _options.ExtractPageLinks) {
			if (_options.ConsolidateNamedDestinations) {
				_reader.ConsolidateNamedDestinations();
			}

			if (_options.ExportBookmarks) {
				Tracker.TraceMessage("导出书签。");
				w.WriteStartElement(Constants.DocumentBookmark);
				ExportBookmarks(OutlineManager.GetBookmark(_reader, _options.UnitConverter), w);
				w.WriteEndElement();
				Tracker.IncrementProgress(BookmarkWorkload);
			}

			if (_options.ExtractPageLinks) {
				Tracker.TraceMessage("导出页面连接。");
				ExtractPageLinks(w);
			}

			if (_options.ConsolidateNamedDestinations == false) {
				Tracker.TraceMessage("导出命名目标。");
				ExportNamedDestinations(w);
			}
		}

		if (_options.ExtractPageSettings) {
			Tracker.TraceMessage("导出页面设置。");
			w.WriteStartElement(Constants.Content.PageSettings.ThisName);
			ExtractPageSettings(w);
			w.WriteEndElement();
		}

		if (_options.ExportCatalog) {
			_contentExport.ExportTrailer(w, _reader);
		}

		if (_options.ExtractPageContent) {
			_contentExport.ExportContents(w, _reader);
		}

		Tracker.TraceMessage("完成导出任务。");
	}

	internal void ExportDocumentInfo(XmlWriter w) {
		PdfDictionary info = _reader.Catalog;
		w.WriteStartElement(Constants.Info.ThisName);
		if (info.Contains(PdfName.VERSION)) {
			w.WriteAttributeString(Constants.Version, PdfHelper.GetPdfNameString(info.GetAsName(PdfName.VERSION)));
		}

		info = _reader.Trailer.GetAsDict(PdfName.INFO);
		if (info?.Length > 0) {
			foreach (KeyValuePair<PdfName, PdfObject> item in info) {
				string key = PdfName.DecodeName(item.Key.ToString());
				string val = item.Value.IsString() ? ((PdfString)item.Value).Decode(null) : item.Value.ToString();
				switch (key) {
					case "Title":
						key = Constants.Info.Title;
						break;
					case "Author":
						key = Constants.Info.Author;
						break;
					case "Subject":
						key = Constants.Info.Subject;
						break;
					case "Keywords":
						key = Constants.Info.Keywords;
						break;
					case "Creator":
						key = Constants.Info.Creator;
						break;
					case "Producer":
						key = Constants.Info.Producer;
						break;
					case "CreationDate":
						key = Constants.Info.CreationDate;
						goto case "//DecodeDate";
					case "ModDate":
						key = Constants.Info.ModDate;
						goto case "//DecodeDate";
					case "//DecodeDate":
						try {
							val = PdfDate.Decode(val).ToString("yyyy年MM月dd日 HH:mm:ss");
						}
						catch (Exception) {
							continue;
						}

						break;
				}

				w.WriteAttributeString(XmlConvert.EncodeLocalName(key), PdfHelper.GetValidXmlString(val));
			}

			if (_reader.Metadata?.Length > 0) {
				w.WriteStartElement(Constants.Info.MetaData);
				using (MemoryStream ms = new(_reader.Metadata)) {
					XmlDocument d = new();
					d.Load(ms);
					d.DocumentElement?.WriteContentTo(w);
				}

				w.WriteEndElement();
			}
		}

		w.WriteEndElement();
	}

	internal static void WriteDocumentInfoAttributes(TextWriter w, string sourcePath, int numberOfPages) {
		w.WriteLine("#版本=" + Constants.InfoDocVersion);
		w.WriteLine("#" + Constants.Info.DocumentPath + "=" + sourcePath);
		w.WriteLine("#页数=" + numberOfPages);
		w.WriteLine();
	}

	internal void ExportDocument(TextWriter w) {
		GeneralInfo i = RewriteDocInfoWithEncoding(_reader, AppContext.Encodings.DocInfoEncoding);
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Title, PdfHelper.GetValidXmlString(i.Title));
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Author, PdfHelper.GetValidXmlString(i.Author));
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Subject,
			PdfHelper.GetValidXmlString(i.Subject));
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Keywords,
			PdfHelper.GetValidXmlString(i.Keywords));
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Creator,
			PdfHelper.GetValidXmlString(i.Creator));
		OutlineManager.WriteSimpleBookmarkInstruction(w, Constants.Info.Producer,
			PdfHelper.GetValidXmlString(i.Producer));
	}

	internal void ExportViewerPreferences(XmlWriter w) {
		PdfDictionary catalog = _reader.Catalog;
		if (catalog.Contains(PdfName.VIEWERPREFERENCES) || catalog.Contains(PdfName.PAGELAYOUT) ||
			catalog.Contains(PdfName.PAGEMODE)) {
			w.WriteStartElement(Constants.ViewerPreferences);
			if (catalog.Contains(PdfName.PAGELAYOUT)) {
				w.WriteAttributeString(Constants.PageLayout,
					ValueHelper.MapValue(catalog.GetAsName(PdfName.PAGELAYOUT), Constants.PageLayoutType.PdfNames,
						Constants.PageLayoutType.Names));
			}

			if (catalog.Contains(PdfName.PAGEMODE)) {
				w.WriteAttributeString(Constants.PageMode,
					ValueHelper.MapValue(catalog.GetAsName(PdfName.PAGEMODE), Constants.PageModes.PdfNames,
						Constants.PageModes.Names));
			}

			if (catalog.Contains(PdfName.VIEWERPREFERENCES)) {
				ExportViewerPreferences(catalog.GetAsDict(PdfName.VIEWERPREFERENCES), w);
			}

			w.WriteEndElement();
		}

		if (!catalog.Contains(PdfName.PAGELABELS)) {
			return;
		}

		List<PageLabel> labels = ExtractPageLabels(catalog.GetAsDict(PdfName.PAGELABELS));
		if (labels.Count <= 0) {
			return;
		}

		w.WriteStartElement(Constants.PageLabels);
		foreach (PageLabel item in labels) {
			w.WriteStartElement(Constants.PageLabelsAttributes.Style);
			w.WriteAttributeString(Constants.PageLabelsAttributes.PageNumber, item.PageNumber.ToText());
			if (item.StartPage != 0) {
				w.WriteAttributeString(Constants.PageLabelsAttributes.StartPage, item.StartPage.ToText());
			}

			if (string.IsNullOrEmpty(item.Prefix) == false) {
				w.WriteAttributeString(Constants.PageLabelsAttributes.Prefix, item.Prefix);
			}

			if (string.IsNullOrEmpty(item.Style) == false) {
				w.WriteAttributeString(Constants.PageLabelsAttributes.Style,
					ValueHelper.MapValue(item.Style[0],
						Constants.PageLabelStyles.PdfValues,
						Constants.PageLabelStyles.Names,
						item.Style)
				);
			}

			w.WriteEndElement();
		}

		w.WriteEndElement();
	}

	private static List<PageLabel> ExtractPageLabels(PdfDictionary labels) {
		List<PageLabel> a = new();
		PdfArray ls = labels.GetAsArray(PdfName.NUMS);
		if (ls == null) {
			return new List<PageLabel>();
		}

		for (int i = 0; i < ls.Size; i++) {
			PageLabel l = new() { PageNumber = ls.GetAsNumber(i++).IntValue + 1 };
			PdfDictionary label = ls.GetAsDict(i);
			if (label.Contains(PdfName.ST)) {
				l.StartPage = label.GetAsNumber(PdfName.ST).IntValue;
			}

			if (label.Contains(PdfName.P)) {
				l.Prefix = label.GetAsString(PdfName.P).ToUnicodeString();
			}

			if (label.Contains(PdfName.S)) {
				l.Style = PdfHelper.GetPdfNameString(label.GetAsName(PdfName.S));
			}

			a.Add(l);
			l = null;
		}

		return a;
	}

	internal void ExtractPageSettings(XmlWriter w) {
		int n = _reader.NumberOfPages;
		PageSettings active = null;
		int fromP = 1;
		for (int i = 1; i <= n; i++) {
			Tracker.IncrementProgress(1);
			PageSettings current = PageSettings.FromReader(_reader, i, _options.UnitConverter);
			if (PageSettings.HavingSameDimension(active, current) && i != n) {
				continue;
			}

			if (i == 1) {
				active = current;
				if (n > i) {
					continue;
				}
			}

			int toP = i == n ? n : i - 1;
			active.PageRange = fromP != toP ? string.Concat(fromP.ToText(), '-', toP.ToText()) : toP.ToText();
			w.WriteStartElement(Constants.Content.Page);
			active.WriteXml(w);
			w.WriteEndElement();
			fromP = i;
			active = current;
		}
	}

	private static void ExportViewerPreferences(PdfDictionary preferences, XmlWriter w) {
		foreach (KeyValuePair<PdfName, PdfObject> item in preferences) {
			PdfName nv = item.Value as PdfName;
			if (item.Key.Equals(PdfName.TYPE)) {
				continue;
			}

			string itemName = ValueHelper.MapValue(item.Key, Constants.ViewerPreferencesType.PdfNames,
				Constants.ViewerPreferencesType.Names, PdfName.DecodeName(item.Key.ToString()));
			if (nv != null) {
				if (PdfName.DIRECTION.Equals(item.Key)) {
					w.WriteAttributeString(Constants.ViewerPreferencesType.Direction,
						ValueHelper.MapValue(nv, Constants.ViewerPreferencesType.DirectionType.PdfNames,
							Constants.ViewerPreferencesType.DirectionType.Names));
				}
				else {
					w.WriteAttributeString(itemName, PdfHelper.GetPdfFriendlyName(nv));
				}
			}
			else if (item.Value.IsBoolean()) {
				w.WriteAttributeString(
					itemName,
					((PdfBoolean)item.Value).BooleanValue ? Constants.Boolean.True : Constants.Boolean.False
				);
			}
			else {
				w.WriteAttributeString(itemName, item.Value.ToString());
			}
		}
	}

	internal void ExportNamedDestinations(XmlWriter w) {
		Dictionary<object, PdfObject> nds = _reader.GetNamedDestination();
		if (nds == null || nds.Count <= 0) {
			return;
		}

		w.WriteStartElement(Constants.NamedDestination);
		foreach (KeyValuePair<object, PdfObject> item in nds) {
			w.WriteStartElement("位置");
			w.WriteAttributeString(Constants.DestinationAttributes.Name,
				StringHelper.ReplaceControlAndBomCharacters(item.Key.ToString()));
			_actionExport.ExportGotoAction(item.Value, w, PageReferenceMapper);
			w.WriteEndElement();
		}

		w.WriteEndElement();
	}

	internal void ExportBookmarks(XmlElement bookmarks, TextWriter w, int level, bool isOpen) {
		if (bookmarks == null || bookmarks.HasChildNodes == false) {
			return;
		}

		XmlNodeList childBookmarks = bookmarks.SelectNodes(Constants.Bookmark);
		if (childBookmarks == null || childBookmarks.Count == 0) {
			return;
		}

		foreach (XmlElement item in childBookmarks) {
			string title = item.GetAttribute(Constants.BookmarkAttributes.Title);
			string page = item.GetAttribute(Constants.DestinationAttributes.Page);
			bool open = item.GetAttribute(Constants.BookmarkAttributes.Open) == Constants.Boolean.True;

			if (open != isOpen && item.HasChildNodes) {
				OutlineManager.WriteSimpleBookmarkInstruction(w, "打开书签", open ? "是" : "否");
				isOpen = open;
			}

			if (string.IsNullOrEmpty(title) == false) {
				for (int i = 0; i < level; i++) {
					w.Write('\t');
				}

				w.Write(title.Replace('\n', ' ').Replace('\r', ' '));
				w.Write(SimpleBookmarkPageNumLeader);
				w.WriteLine(page);
			}

			if (childBookmarks == null) {
				continue;
			}

			level++;
			ExportBookmarks(item, w, level, isOpen);
			level--;
		}
	}

	/// <summary>
	///     导出 PDF 书签。
	/// </summary>
	/// <param name="bookmarks"></param>
	/// <param name="w"></param>
	internal void ExportBookmarks(XmlElement bookmarks, XmlWriter w) {
		if (bookmarks == null) {
			return;
		}

		foreach (XmlElement child in bookmarks.ChildNodes) {
			if (child == null) {
				continue;
			}

			w.WriteStartElement(Constants.Bookmark);
			foreach (XmlAttribute entry in child.Attributes) {
				string key = entry.Name;
				string value = entry.Value ?? string.Empty;
				switch (key) {
					case Constants.Coordinates.Bottom:
					case Constants.Coordinates.Left:
					case Constants.Coordinates.Top:
					case Constants.Coordinates.Right:
						if (string.IsNullOrEmpty(value) || value == "null") {
							continue;
						}

						goto default;
					case Constants.Coordinates.ScaleFactor:
						if (string.IsNullOrEmpty(value)) {
							continue;
						}

						goto default;
					case Constants.BookmarkAttributes.Title:
						w.WriteAttributeString(key, PdfHelper.GetValidXmlString(value));
						break;
					default:
						w.WriteAttributeString(key, value);
						break;
				}
			}

			if (child.ChildNodes.Count > 0) {
				ExportBookmarks(child, w);
			}

			w.WriteEndElement();
		}
	}

	/// <summary>
	///     导出 PDF 文档页内连接。
	/// </summary>
	/// <param name="r"></param>
	/// <param name="w"></param>
	internal void ExtractPageLinks(XmlWriter w) {
		w.WriteStartElement(Constants.PageLink);
		int numPages = _reader.NumberOfPages;

		for (int i = 1; i <= numPages; i++) {
			PdfDictionary pageDic = _reader.GetPageNRelease(i);
			Tracker.IncrementProgress(1);
			PdfArray annots = (PdfArray)PdfReader.GetPdfObject(pageDic.Get(PdfName.ANNOTS));
			if (annots == null) {
				continue;
			}

			List<PdfObject> arr = annots.ArrayList;
			foreach (PdfObject item in arr) {
				if (item.IsNull()) {
					continue;
				}

				PdfDictionary annot = (PdfDictionary)PdfReader.GetPdfObjectRelease(item);
				if (!PdfName.LINK.Equals(annot.Get(PdfName.SUBTYPE))) {
					continue;
				}

				w.WriteStartElement(Constants.PageLinkAttributes.Link);
				w.WriteAttributeString(Constants.PageLinkAttributes.PageNumber, i.ToText());
				PdfArray rect = annot.GetAsArray(PdfName.RECT);
				if (rect != null && rect.Size == 4) {
					UnitConverter u = _options.UnitConverter;
					float[] p = new float[4];
					int k = 0;
					foreach (PdfNumber ri in rect.ArrayList) {
						if (ri == null) {
							break;
						}

						p[k] = u.FromPoint(ri.FloatValue);
						k++;
					}

					if (k == 4) {
						w.WriteAttributeString(Constants.Coordinates.Left, p[0].ToText());
						w.WriteAttributeString(Constants.Coordinates.Bottom, p[1].ToText());
						w.WriteAttributeString(Constants.Coordinates.Right, p[2].ToText());
						w.WriteAttributeString(Constants.Coordinates.Top, p[3].ToText());
					}
				}

				if (annot.Contains(PdfName.BORDER)) {
					w.WriteAttributeString(Constants.PageLinkAttributes.Border,
						PdfHelper.GetNumericArrayString(annot.GetAsArray(PdfName.BORDER), 1));
				}

				if (annot.Contains(PdfName.C)) {
					ExportColor(annot.GetAsArray(PdfName.C), w);
				}

				if (annot.Contains(PdfName.H)) {
					string style = PdfHelper.GetPdfNameString(annot.GetAsName(PdfName.H));
					style = ValueHelper.MapValue(style,
						new[] { "N", "I", "O", "P" },
						new[] { "无", "取反内容", "取反边框", "按下" },
						style
					);
					w.WriteAttributeString(Constants.PageLinkAttributes.Style, style);
				}

				//if (annot.Contains (PdfName.M)) {
				//    try {
				//        w.WriteAttributeString ("日期", PdfDate.Decode (annot.GetAsString (PdfName.M).ToString ()).ToString ());
				//    }
				//    catch (Exception) {
				//        w.WriteAttributeString ("日期", annot.GetAsString (PdfName.M).ToString ());
				//    }
				//}
				if (annot.Contains(PdfName.QUADPOINTS)) {
					w.WriteAttributeString(Constants.PageLinkAttributes.QuadPoints,
						PdfHelper.GetNumericArrayString(annot.GetAsArray(PdfName.QUADPOINTS),
							_options.UnitConverter.UnitFactor));
				}

				if (annot.Contains(PdfName.CONTENTS)) {
					w.WriteAttributeString(Constants.PageLinkAttributes.Contents,
						annot.GetAsString(PdfName.CONTENTS).ToUnicodeString());
				}

				ExportLinkAction(annot, w);
				if (annot.Contains(PdfName.BS)) {
					w.WriteStartElement("边框样式");
					PdfDictionary bs = annot.GetAsDict(PdfName.BS);
					if (bs.Contains(PdfName.W)) {
						w.WriteAttributeString("宽度", bs.GetAsNumber(PdfName.W).FloatValue.ToText());
					}

					if (bs.Contains(PdfName.S)) {
						string style = PdfHelper.GetPdfNameString(bs.GetAsName(PdfName.S));
						style = ValueHelper.MapValue(style,
							new[] { "S", "U", "D", "B", "I" },
							new[] { "方框", "下划线", "虚线", "凸起", "凹陷" },
							style
						);
						w.WriteAttributeString("样式", style);
						if (PdfName.D.Equals(bs.GetAsName(PdfName.S)) && bs.Contains(PdfName.D)) {
							w.WriteAttributeString("线型", PdfHelper.GetArrayString(bs.GetAsArray(PdfName.D)));
						}
					}

					w.WriteEndElement();
				}

				w.WriteEndElement();
			}
		}

		w.WriteEndElement();
	}

	internal static void ExportColor(PdfArray color, XmlWriter target) {
		if (color == null) {
			return;
		}

		switch (color.Size) {
			case 0:
				target.WriteAttributeString(Constants.Color, Constants.Colors.Transparent);
				break;
			case 1:
				target.WriteAttributeString(Constants.Colors.Gray, color.GetAsNumber(0).FloatValue.ToText());
				break;
			case 3:
				target.WriteAttributeString(Constants.Colors.Red, color.GetAsNumber(0).FloatValue.ToText());
				target.WriteAttributeString(Constants.Colors.Green, color.GetAsNumber(1).FloatValue.ToText());
				target.WriteAttributeString(Constants.Colors.Blue, color.GetAsNumber(2).FloatValue.ToText());
				break;
			case 4:
				target.WriteAttributeString(Constants.Colors.Cyan, color.GetAsNumber(0).FloatValue.ToText());
				target.WriteAttributeString(Constants.Colors.Magenta, color.GetAsNumber(1).FloatValue.ToText());
				target.WriteAttributeString(Constants.Colors.Yellow, color.GetAsNumber(2).FloatValue.ToText());
				target.WriteAttributeString(Constants.Colors.Black, color.GetAsNumber(3).FloatValue.ToText());
				break;
		}
	}

	/// <summary>
	///     导出 PDF 文档的单个连接信息。
	/// </summary>
	/// <param name="link"></param>
	/// <param name="w"></param>
	private void ExportLinkAction(PdfDictionary link, XmlWriter w) {
		PdfObject dest = PdfReader.GetPdfObjectRelease(link.Get(PdfName.DEST));

		w.WriteStartElement(Constants.PageLinkAttributes.LinkAction);

		if (dest != null) {
			_actionExport.ExportGotoAction(dest, w, PageReferenceMapper);
		}
		else {
			_actionExport.ExportAction((PdfDictionary)PdfReader.GetPdfObjectRelease(link.Get(PdfName.A)),
				PageReferenceMapper, w);
		}

		w.WriteEndElement();
	}
}