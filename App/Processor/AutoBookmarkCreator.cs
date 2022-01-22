using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Common;
using PDFPatcher.Model;
using GraphicsState = PDFPatcher.Model.GraphicsState;

namespace PDFPatcher.Processor;

internal sealed class AutoBookmarkCreator
{
	private const string __AddSpaceAfterCharacters = ":.,\"'?!)]};";
	private const string __InsertSpaceBeforeCharacters = "\"'([{";
	private const int OpenWorkload = 10;
	private readonly AutoBookmarkOptions _options;

	private readonly PdfReader _reader;

	public AutoBookmarkCreator(PdfReader reader, AutoBookmarkOptions options) {
		_reader = reader;
		_options = options;
		TextLine.DefaultDirection = options.WritingDirection;
	}

	internal int EstimateWorkload() {
		int n = _reader.NumberOfPages;
		int load = 0;
		load += OpenWorkload;
		int t = PageRangeCollection.Parse(_options.PageRanges, 1, n, true).TotalPages;
		load += t > 0 ? t : n;
		return load;
	}

	internal void ExportAutoBookmarks(XmlWriter w, AutoBookmarkOptions options) {
		AutoCreateBookmarks(w, _reader, options);
	}

	internal void AutoCreateBookmarks(XmlWriter writer, PdfReader reader, AutoBookmarkOptions options) {
		Tracker.IncrementProgress(OpenWorkload);
		int pn = reader.NumberOfPages + 1;
		AutoBookmarkContext c = new() { TotalPageNumber = reader.NumberOfPages };
		TextToBookmarkProcessor p = new(options, c);
		LevelProcessor lp = new(options.LevelAdjustment);
		PageRangeCollection ranges = PageRangeCollection.Parse(options.PageRanges, 1, reader.NumberOfPages, true);
		XmlDocument doc = new();
		XmlElement be = doc.AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
		Stack<float> sizes = new();
		float yOffset = 1 + options.YOffset;
		int level = 0;
		const string indentString = "　　　　　　　　　　";
		List<MatchPattern.IMatcher> ig;
		FontOccurance fontOccurances = new();
		if (options.IgnorePatterns.Count == 0) {
			ig = null;
		}
		else {
			ig = new List<MatchPattern.IMatcher>();
			foreach (MatchPattern item in options.IgnorePatterns) {
				if (string.IsNullOrEmpty(item.Text)) {
					continue;
				}

				try {
					ig.Add(item.CreateMatcher());
				}
				catch (ArgumentException ex) {
					Tracker.TraceMessage(Tracker.Category.Alert, string.Concat("忽略文本（", item, "）无效：", ex.Message));
				}
			}
		}

		XmlWriter oldWriter = null;
		if (options.ExportTextCoordinates == false) {
			oldWriter = writer;
			writer = new NullXmlWriter();
		}

		foreach (PageRange r in ranges) {
			for (int i = r.StartValue; i <= r.EndValue && i < pn; i++) {
				if (i == 1 && options.CreateBookmarkForFirstPage) {
					continue;
				}

				//Tracker.TraceMessage (String.Concat ("分析第 ", i, " 页。"));
				Rectangle box = reader.GetCropBox(i);
				p.Reset();
				c.PageBox = box;
				c.CurrentPageNumber = i;
				int pr = reader.GetPageRotation(i);
				pr = PdfHelper.NormalizeRotationNumber(pr);
				if (pr != 0) {
					p.RotationMatrix = pr switch {
						90 => new Matrix(0, 1, -1, 0, 0, 0),
						180 => new Matrix(0, -1, -1, 0, 0, 0),
						270 => new Matrix(0, -1, 1, 0, 0, 0),
						_ => p.RotationMatrix
					};
				}

				p.ProcessContent(reader.GetPageContent(i), reader.GetPageNRelease(i).GetAsDict(PdfName.RESOURCES));
				//p.SortTextList ();
				//p.PostProcessTextList ();

				//var tr = p.TextList;
				c.IsTextMerged = false;
				c.TextLine = null;
				// TODO: 自动根据已知排版方向比例修正排版方向
				// 合并前筛选文本
				List<TextInfo> ptl = p.TextList;
				for (int li = ptl.Count - 1; li >= 0; li--) {
					c.TextInfo = ptl[li];
					if (lp.ChangeSizeLevel(c) < options.TitleThreshold) {
						ptl.RemoveAt(li);
					}
				}

				List<TextLine> tl = MergeTextInfoList(box, ptl, _options);
				// TODO: 筛选文本
				c.IsTextMerged = true;
				for (int li = tl.Count - 1; li >= 0; li--) {
					c.TextLine = tl[li];
					c.TextInfo = c.TextLine.FirstText;
					if ((c.TextInfo.Size = lp.ChangeSizeLevel(c)) < options.TitleThreshold) {
						tl.RemoveAt(li);
					}
				}

				List<TextRegion> tr = MergeTextLines(box, tl);
				if (tr is { Count: > 0 }) {
					if (options.WritingDirection != WritingDirection.Unknown) {
						tr.Sort((a, b) => {
							Bound ra = a.Region;
							Bound rb = b.Region;
							if (ra.Middle < rb.Middle) {
								return 1;
							}

							if (ra.Middle > rb.Middle) {
								return -1;
							}

							if (ra.Center > rb.Center) {
								return 1;
							}

							if (ra.Center < rb.Center) {
								return -1;
							}

							return 0;
						});
					}

					writer.WriteStartElement(Constants.Content.Texts);
					writer.WriteValue(Constants.Content.PageNumber, i);
					foreach (TextRegion item in tr) {
						string t = PdfHelper.GetValidXmlString(ConcatRegionText(item)).Trim();
						FontInfo f = item.Lines[0].FirstText.Font;
						float s = item.Lines[0].FirstText.Size;
						writer.WriteStartElement("文本");
						writer.WriteAttributeString(Constants.Font.ThisName, f != null ? f.FontID.ToText() : "OCR");
						writer.WriteValue(Constants.Coordinates.Top, item.Region.Top);
						writer.WriteValue(Constants.Coordinates.Bottom, item.Region.Bottom);
						writer.WriteValue(Constants.Coordinates.Left, item.Region.Left);
						writer.WriteValue(Constants.Coordinates.Width, item.Region.Width);
						writer.WriteValue(Constants.Coordinates.Height, item.Region.Height);
						writer.WriteValue("尺寸", s);
						writer.WriteString(t);
						writer.WriteEndElement();

						if (t.Length == 0
							|| (t.Length == 1 && options.IgnoreSingleCharacterTitle)
							|| (options.IgnoreNumericTitle && AutoBookmarkOptions.NumericPattern.IsMatch(t))
						   ) {
							continue;
						}

						if (ig != null) {
							bool ignore = false;
							foreach (var rg in ig.Where(rg => rg.Matches(t))) {
								ignore = true;
							}

							if (ignore) {
								continue;
							}
						}

						if (options.AutoHierarchicalArrangement) {
							float size = -1;
							do {
								//if (ValueHelper.TryParse (be.GetAttribute (Constants.Font.Size), out size) == false || s < size) {
								if (sizes.Count == 0 || s < (size = sizes.Peek())) {
									be = be.AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
									sizes.Push(s);
									level++;
									break;
								}

								if (s == size) {
									be = (be.ParentNode ?? be).AppendChild(doc.CreateElement(Constants.Bookmark)) as
										XmlElement;
									break;
								}

								be = be.ParentNode as XmlElement;
								sizes.Pop();
								level--;
							} while (s > size && be.NodeType == XmlNodeType.Element);
						}
						else {
							be = doc.DocumentElement.AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
						}

						be.SetAttribute(Constants.BookmarkAttributes.Title, t);
						be.SetAttribute(Constants.DestinationAttributes.Page, i.ToText());
						be.SetAttribute(Constants.DestinationAttributes.View,
							Constants.DestinationAttributes.ViewType.XYZ);
						be.SetAttribute(Constants.Coordinates.Top, (item.Region.Top + (s * yOffset)).ToText());
						be.SetAttribute(Constants.Font.Size, s.ToText());
						if (f != null) {
							be.SetAttribute(Constants.Font.ThisName, f.FontID.ToText());
							//fontOccurances.AddOccurance (f.FontID, s, i, t);
						}

						CountFontOccuranceInRegion(fontOccurances, i, item);
#if DEBUG
						Tracker.TraceMessage(string.Concat(item.Direction.ToString()[0], ':',
							level < 11 ? indentString.Substring(0, level) : indentString, t, " .... ", i.ToText()));
#else
							Tracker.TraceMessage (String.Concat (level < 11 ? indentString.Substring (0, level) : indentString, t, " .... ", ValueHelper.ToText (i)));
#endif
					}

					writer.WriteEndElement();
				}

				Tracker.IncrementProgress(1);
			}
		}

		if (oldWriter != null) {
			writer = oldWriter;
		}

		WriteDocumentFontOccurances(writer, options, p, fontOccurances);
		SetGoToTop(options, doc);
		writer.WriteStartElement(Constants.DocumentBookmark);
		if (options.CreateBookmarkForFirstPage && string.IsNullOrEmpty(options.FirstPageTitle) == false) {
			writer.WriteStartElement(Constants.Bookmark);
			writer.WriteAttributeString(Constants.BookmarkAttributes.Title, options.FirstPageTitle);
			writer.WriteAttributeString(Constants.DestinationAttributes.Page, "1");
			writer.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
			writer.WriteEndElement();
		}

		doc.DocumentElement.WriteContentTo(writer);
		writer.WriteEndElement();
	}

	private static void SetGoToTop(AutoBookmarkOptions options, XmlDocument doc) {
		if (options.PageTopForLevel <= 0) {
			return;
		}

		XmlNodeList topics =
			doc.DocumentElement.SelectNodes(".//书签[count(ancestor::书签) < " + (options.PageTopForLevel + 1) + "]");
		foreach (XmlElement t in topics) {
			t.RemoveAttribute(Constants.Coordinates.Top);
		}
	}

	private static void WriteDocumentFontOccurances(XmlWriter writer, AutoBookmarkOptions options,
		TextToBookmarkProcessor p, FontOccurance fontOccurances) {
		writer.WriteStartElement(Constants.Font.DocumentFont);
		Tracker.TraceMessage("\n文档所用的字体");
		List<string> dl = new();
		foreach (KeyValuePair<int, string> item in p.FontList) {
			string fo = "0";
			List<SizeOccurrence> sl = fontOccurances.GetOccurance(item.Value);
			if (sl != null) {
				if (dl.Contains(item.Value) == false) {
					int o = 0;
					foreach (SizeOccurrence s in sl) {
						o += s.Occurrence;
					}

					fo = o.ToText();
					dl.Add(item.Value);
				}
				else {
					sl = null;
				}
			}

			if (options.DisplayFontStatistics && (sl != null || options.DisplayAllFonts)) {
				Tracker.TraceMessage(string.Concat("编号：", item.Key, "\t出现次数：", fo, "\t名称：", item.Value));
			}

			writer.WriteStartElement(Constants.Font.ThisName);
			writer.WriteAttributeString(Constants.Font.ID, item.Key.ToText());
			writer.WriteAttributeString(Constants.Font.Name, item.Value);
			writer.WriteAttributeString(Constants.FontOccurance.Count, fo);
			if (sl != null) {
				foreach (SizeOccurrence s in sl) {
					writer.WriteStartElement(Constants.Font.Size);
					writer.WriteAttributeString(Constants.Font.Size, s.Size.ToText());
					writer.WriteAttributeString(Constants.FontOccurance.Count, s.Occurrence.ToText());
					writer.WriteAttributeString(Constants.FontOccurance.FirstText, s.FirstInstance);
					writer.WriteAttributeString(Constants.FontOccurance.FirstPage, s.FirstPage.ToText());
					if (options.DisplayFontStatistics && (s.Occurrence > 0 || options.DisplayAllFonts)) {
						Tracker.TraceMessage(string.Concat("\t尺寸：", s.Size.ToText(), "\t出现次数：", s.Occurrence.ToText(),
							"\t首次出现于第", s.FirstPage.ToText(), "页（", s.FirstInstance, "）"));
					}

					writer.WriteEndElement();
				}
			}

			writer.WriteEndElement();
		}

		writer.WriteEndElement();
	}

	private static void CountFontOccuranceInRegion(FontOccurance fontOccurances, int i, TextRegion item) {
		FontInfo f = null;
		foreach (TextLine il in item.Lines) {
			foreach (TextInfo ii in il.Texts) {
				if (ii.Font == null || (f != null && ii.Font.FontID == f.FontID)) {
					continue;
				}

				fontOccurances.AddOccurance(ii.Font.FontName, ii.Size, i, il.Text);
				f = ii.Font;
			}
		}
	}

	private static string ConcatRegionText(TextRegion region) {
		List<TextLine> ls = region.Lines;
		switch (ls.Count) {
			case 0:
				return string.Empty;
			case 1:
				return ls[0].Text;
		}

		ls = new List<TextLine>(ls);
		if (region.Direction == WritingDirection.Vertical) {
			ls.Sort((a, b) => {
				if (a.Region.Middle < b.Region.Middle) {
					return 1;
				}

				if (a.Region.Middle > b.Region.Middle) {
					return -1;
				}

				return 0;
			});
		}

		StringBuilder sb = new();
		sb.Append(ls[0].Text);
		for (int i = 1; i < ls.Count; i++) {
			string l = ls[i].Text;
			string ll = ls[i - 1].Text;
			if (ll.Length > 0 && l.Length > 0) {
				char c1 = l[l.Length - 1];
				char c2 = ll[0];
				if ((__AddSpaceAfterCharacters.IndexOf(c1) != -1
					 || (char.IsLetterOrDigit(c1) && c1 < 0x4E00 /*非中文字符*/))
					&& (__InsertSpaceBeforeCharacters.IndexOf(c2) != -1
						|| (char.IsLetterOrDigit(c2) && c2 < 0x4E00))) {
					sb.Append(' ');
				}
			}

			sb.Append(l);
		}

		return sb.ToString();
	}

	/// <summary>
	///     使用最小距离法将 <paramref name="textInfos" /> 的文本聚类为 <see cref="TextLine" /> 列表。
	/// </summary>
	/// <param name="textInfos">包含文本位置及尺寸信息的 <see cref="TextInfo" /> 集合。</param>
	/// <returns>聚类后所得的 <see cref="TextLine" /> 列表。</returns>
	internal static List<TextLine> MergeTextInfoList(Rectangle pageBox, IList<TextInfo> textInfos,
		AutoBookmarkOptions options) {
		List<TextLine> ll = new();
		// 同行合并宽度最小值
		float cw = pageBox.Width / 6;
		float ch = pageBox.Height / 6;

		int[] dirCount = new int[4];
		// 遍历识别所得的各 TextInfo，使用最小距离聚类方法将其聚类为行
		foreach (TextInfo item in textInfos) {
			Bound ir = item.Region;
			DistanceInfo md = new(DistanceInfo.Placement.Unknown, float.MaxValue, float.MaxValue); // 最小距离
			TextLine ml = null; // 最小距离的 TextLine

			// 求最小距离的 TextLine
			float ds = item.Size / 10;
			// 循环只包含了 TextLine，未包含文本 TextInfo 的其余文本
			int end = ll.Count > 5 ? ll.Count - 5 : 0;
			for (int i = ll.Count - 1; i >= end; i--) {
				TextLine li = ll[i];
				// 文本尺寸应在误差范围之内
				if (Math.Abs(item.Size - li.FirstText.Size) > ds && options.MergeDifferentSizeTitles == false) {
					continue;
				}

				if (options.MergeDifferentFontTitles == false && li.FirstText.Font.FontID != item.Font.FontID) {
					break;
				}

				DistanceInfo cd = li.GetDistance(ir); // TextInfo 到 TextLine 的距离
				if ((!cd.IsOverlapping || (md.IsOverlapping != false && !(cd.DistanceRadial < md.DistanceRadial))) &&
					((md.Location != DistanceInfo.Placement.Unknown && (cd.IsOverlapping != false ||
																		md.IsOverlapping != false ||
																		!(cd.MinDistance < md.MinDistance))) ||
					 ((!cd.IsHorizontallyAligned || li.Direction == WritingDirection.Vertical ||
					   !item.Region.IsAlignedWith(li.Region, WritingDirection.Horizontal)) &&
					  (!cd.IsVerticallyAligned || li.Direction == WritingDirection.Horizontal ||
					   !item.Region.IsAlignedWith(li.Region, WritingDirection.Vertical))) ||
					 (options.DetectColumns != false && !(cd.MinDistance < cw)) || (!options.MergeDifferentFontTitles &&
						 li.FirstText.Font.FontID != item.Font.FontID))) {
					continue;
				}

				md = cd;
				ml = li;
				if (cd.IsLeft) {
					dirCount[0]++;
				}
				else if (cd.IsRight) {
					dirCount[1]++;
				}
				else if (cd.IsAbove) {
					dirCount[2]++;
				}
				else if (cd.IsBelow) {
					dirCount[3]++;
				}
			}

			// 否则，用 item 创建新的 TextLine
			if (item.Text.Length == 0) {
				item.Text = " ";
			}

			if (ml != null) {
				// 若存在最小距离的 TextLine 且可合并，则将 item 归入 TextLine
				if (md.IsOverlapping && options.IgnoreOverlappedText) {
					// 检查是否存在交叠重复的文本
					foreach (TextInfo t in ml.Texts) {
						if (t.Region.IntersectWith(item.Region) // item 与 TextLine 中某项交叠
							&& (t.Text.Contains(item.Text) || item.Text.Contains(t.Text) // 交叠的项文本和 item 的文本相同
							)
						   ) {
							goto Next; // 忽略此项目
						}
					}
				}

				ml.AddText(item);
			}
			else {
				ll.Add(new TextLine(item));
			}

		Next:;
		}

		return ll;
	}

	internal List<TextRegion> MergeTextLines(Rectangle pageBox, IList<TextLine> textLines) {
		List<TextRegion> ll = new();
		// 同行合并宽度最小值
		float cw = pageBox.Width / 6;
		float ch = pageBox.Height / 6;

		// 遍历识别所得的各 TextInfo，使用最小距离聚类方法将其聚类为行
		foreach (TextLine item in textLines) {
			Bound ir = item.Region;
			DistanceInfo md = new(DistanceInfo.Placement.Unknown, float.MaxValue, float.MaxValue); // 最小距离
			TextRegion mr = null; // 最小距离的 TextRegion

			// 求最小距离的 TextLine
			float ds = item.FirstText.Size / 10;
			// 循环只包含了 TextLine，未包含文本 TextInfo 的其余文本
			for (int i = ll.Count - 1; i >= 0; i--) {
				TextRegion li = ll[i];
				// 文本尺寸应在误差范围之内
				if (Math.Abs(item.FirstText.Size - li.Lines[0].FirstText.Size) > ds && _options.MergeAdjacentTitles) {
					continue;
				}

				if (_options.MergeDifferentFontTitles == false &&
					li.Lines[0].FirstText.Font.FontID != item.FirstText.Font.FontID) {
					break;
				}

				DistanceInfo cd = li.Region.GetDistance(ir, li.Direction); // TextInfo 到 TextLine 的距离
				if ((!cd.IsOverlapping || (md.IsOverlapping != false && !(cd.DistanceRadial < md.DistanceRadial))) &&
					((md.Location != DistanceInfo.Placement.Unknown && (cd.IsOverlapping != false ||
																		md.IsOverlapping != false ||
																		!(cd.MinDistance < md.MinDistance))) ||
					 ((!cd.IsHorizontallyAligned || li.Direction == WritingDirection.Vertical ||
					   !item.Region.IsAlignedWith(li.Region, WritingDirection.Horizontal) ||
					   !(cd.MinDistance < item.Region.Width * _options.MaxDistanceBetweenLines) ||
					   !_options.MergeAdjacentTitles ||
					   (!_options.MergeDifferentSizeTitles && li.Lines[0].Region.Width != item.Region.Width)) &&
					  (!cd.IsVerticallyAligned || li.Direction == WritingDirection.Horizontal ||
					   !item.Region.IsAlignedWith(li.Region, WritingDirection.Vertical) ||
					   !(cd.MinDistance < item.Region.Height * _options.MaxDistanceBetweenLines) ||
					   !_options.MergeAdjacentTitles ||
					   (!_options.MergeDifferentSizeTitles && li.Lines[0].Region.Height != item.Region.Height))) ||
					 !(cd.MinDistance < cw))) {
					continue;
				}

				md = cd;
				mr = li;
			}

			// 否则，用 item 创建新的 TextLine
			if (mr != null) {
				// 若存在最小距离的 TextLine 且可合并，则将 item 归入 TextLine
				mr.AddLine(item);
			}
			else {
				ll.Add(new TextRegion(item));
			}
		}

		return ll;
	}

	private sealed class SizeOccurrence
	{
		public SizeOccurrence(float size, int page, string instance) {
			Size = size;
			Occurrence = 1;
			FirstPage = page;
			FirstInstance = instance.Length > 50 ? instance.Substring(0, 50) : instance;
		}

		public float Size { get; }
		public int FirstPage { get; }
		public string FirstInstance { get; }
		public int Occurrence { get; set; }
	}

	private sealed class FontOccurance
	{
		private readonly Dictionary<string, List<SizeOccurrence>> oc = new();

		internal List<SizeOccurrence> GetOccurance(string fontName) {
			return oc.TryGetValue(fontName, out List<SizeOccurrence> s) ? s : null;
		}

		internal void AddOccurance(string fontName, float size, int page, string instance) {
			if (oc.ContainsKey(fontName) == false) {
				oc.Add(fontName, new List<SizeOccurrence> { new(size, page, instance) });
			}
			else {
				SizeOccurrence o = oc[fontName].Find(s => s.Size == size);
				if (o != null) {
					o.Occurrence++;
				}
				else {
					oc[fontName].Add(new SizeOccurrence(size, page, instance));
				}
			}
		}
	}

	private sealed class TextToBookmarkProcessor : PdfContentStreamProcessor
	{
		private const string __AddSpaceAfterCharacters = ":.,\"'?!)]};";
		private const string __InsertSpaceBeforeCharacters = "\"'([{";
		private readonly AutoBookmarkContext _context;
		private readonly float _fontSizeThreshold;
		private readonly LevelProcessor _levelProcessor;

		//Rectangle _positionRectangle;
		private readonly bool _mergeAdjacentTitles;
		private readonly bool _mergeDifferentSizeTitles;
		private float _textWidth, _charWidth;

		public TextToBookmarkProcessor(AutoBookmarkOptions options, AutoBookmarkContext context) {
			_fontSizeThreshold = options.TitleThreshold;
			//_positionRectangle = options.PositionRectangle;
			_mergeAdjacentTitles = options.MergeAdjacentTitles;
			_mergeDifferentSizeTitles = options.MergeDifferentSizeTitles;
			_levelProcessor = new LevelProcessor(options.LevelAdjustment);
			TextList = new List<TextInfo>();
			PopulateOperators();
			RegisterContentOperator("TJ", new AccumulatedShowTextArray());
			_context = context;
		}

		/// <summary>
		///     获取页面内容的文本。
		/// </summary>
		internal List<TextInfo> TextList { get; }

		/// <summary>
		///     获取字体列表。
		/// </summary>
		internal IDictionary<int, string> FontList => Fonts;

		public Matrix RotationMatrix { get; set; }

		internal override void Reset() {
			base.Reset();
			TextList?.Clear();
		}

		protected override void DisplayPdfString(PdfString str) {
			GraphicsState gs = CurrentGraphicState;
			FontInfo font = gs.Font;
			char[] chars = font.DecodeText(str).ToCharArray();
			float totalWidth = 0, charWidth = 0;
			foreach (char c in chars) {
				float w = font.GetWidth(c) / 1000.0f;
				if (w == 0 && (font.CjkType & FontInfo.CjkFontType.CJK) > 0) {
					w = c < 0xFF ? 0.5f : 1f;
				}

				float wordSpacing = c == ' ' ? gs.WordSpacing : 0f;
				if (char.IsLetterOrDigit(c)) {
					charWidth += w * gs.FontSize * gs.HorizontalScaling;
				}

				totalWidth += ((w * gs.FontSize) + gs.CharacterSpacing + wordSpacing) * gs.HorizontalScaling;
			}

			_textWidth = totalWidth;
			_charWidth = charWidth;
			AdjustTextMatrixX(totalWidth);
		}

		protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
			string o = oper.ToString();
			string text;
			float size;
			Matrix tm;
			switch (o) {
				case "TJ":
					tm = GetTextMatrix();
					size = GetFontSize(tm);
					text = DecodeTJText(operands, size);
					break;
				case "Tj":
				case "'":
				case "\"":
					tm = GetTextMatrix();
					size = GetFontSize(tm);
					if (size < 0) {
						size = -size;
					}

					//if (size < _fontSizeThreshold) {
					//    goto default;
					//}
					text = CurrentGraphicState.Font.DecodeText(operands[0] as PdfString);
					break;
				default:
					// 执行默认的操作指令
					base.InvokeOperator(oper, operands);
					return;
			}

			// 处理文本
			base.InvokeOperator(oper, operands);
			//if (tm[Matrix.I12] != 0 || tm[Matrix.I21] != 0) {
			//    // 忽略非横向文本
			//    goto Exit;
			//}
			if (size < 0.0001) {
				size = 0.0001f;
			}
			else {
				size = (float)Math.Round(size, 4);
			}

			TextInfo ti = new() {
				Text = text.Length > 1 ? text.TrimEnd(' ') : text,
				Size = size,
				Region = CreateBoundFromMatrix(tm, _textWidth, size),
				Font = CurrentGraphicState.Font,
				LetterWidth = _charWidth * tm[Matrix.I22]
			};
			if (ti.LetterWidth < 0) {
				ti.LetterWidth = -ti.LetterWidth;
			}

			if (IsBoundOutOfRectangle(_context.PageBox, ti.Region)) {
				// 文本落在页面之外
				goto Exit;
			}
			//TODO: 筛选文本
			//this._context.TextInfo = ti;
			//ti.Size = _levelProcessor.ChangeSizeLevel (this._context);
			//if (ti.Size < _fontSizeThreshold) {
			//    return;
			//}

			//if (_positionRectangle != null && ti.Region.Right < this._positionRectangle.Left
			//    || ti.Region.Top < this._positionRectangle.Top - this._positionRectangle.Height
			//    || ti.Region.Bottom > this._positionRectangle.Top
			//    || ti.Region.Left > this._positionRectangle.Right) {
			//    // 文本落在范围框之外
			//    goto Exit;
			//}
			TextList.Add(ti);
		Exit:;
		}

		private string DecodeTJText(IList<PdfObject> operands, float size) {
			//if (size < _fontSizeThreshold) {
			//    goto default;
			//}
			PdfArray array = (PdfArray)operands[0];
			float d = size * CurrentGraphicState.HorizontalScaling * 4f / 1000f;
			string[] t = new string[array.Size];
			int i = 0;
			foreach (PdfObject item in array.ArrayList) {
				switch (item.Type) {
					case PdfObject.STRING:
						t[i++] = CurrentGraphicState.Font.DecodeText(item as PdfString);
						break;
					case PdfObject.NUMBER: {
							if (-(item as PdfNumber).FloatValue * d > size) {
								t[i++] = " ";
							}

							break;
						}
				}
			}

			return string.Concat(t);
		}

		private float GetFontSize(Matrix tm) {
			float size = CurrentGraphicState.FontSize * tm[Matrix.I22];
			if (size < 0) {
				size = -size;
			}

			return size;
		}

		private Matrix GetTextMatrix() {
			return RotationMatrix != null
				? RotationMatrix.Multiply(TextMatrix).Multiply(CurrentGraphicState.TransMatrix)
				: TextMatrix.Multiply(CurrentGraphicState.TransMatrix);
		}

		private static Bound CreateBoundFromMatrix(Matrix tm, float textWidth, float size) {
			float l = tm[Matrix.I31];
			float b = tm[Matrix.I32];
			float r = tm[Matrix.I31] + (textWidth * tm[Matrix.I11]);
			float t = tm[Matrix.I32] + size;
			float x;
			if (l > r) {
				x = r;
				r = l;
				l = x;
			}

			if (!(b > t)) {
				return new Bound(l, b, r, t);
			}

			x = t;
			t = b;
			b = x;

			return new Bound(l, b, r, t);
		}

		/// <summary>
		///     检查 <paramref name="b" /> 是否处于 <paramref name="a" /> 之内。
		/// </summary>
		/// <param name="a">大边框。</param>
		/// <param name="b">小边框。</param>
		/// <returns>小边框完全处于大边框内，则返回 true。</returns>
		private static bool IsBoundOutOfRectangle(Rectangle a, Bound b) {
			return b.Right < a.Left
				   || b.Top < a.Bottom
				   || b.Bottom > a.Top
				   || b.Left > a.Right;
		}
	}

	private sealed class LevelProcessor
	{
		private readonly AutoBookmarkFilter[] _filters;
		private readonly AutoBookmarkOptions.LevelAdjustmentOption[] _options;

		internal LevelProcessor(IList<AutoBookmarkOptions.LevelAdjustmentOption> options) {
			int l = options.Count;
			_filters = new AutoBookmarkFilter[l];
			_options = new AutoBookmarkOptions.LevelAdjustmentOption[l];
			for (int i = 0; i < l; i++) {
				_filters[i] = options[i].Condition.CreateFilter();
				_options[i] = options[i];
			}
		}

		internal float ChangeSizeLevel(AutoBookmarkContext context) {
			for (int i = 0; i < _options.Length; i++) {
				AutoBookmarkOptions.LevelAdjustmentOption o = _options[i];
				if (o.FilterBeforeMergeTitle && context.IsTextMerged) {
					continue;
				}

				if (_filters[i].Matches(context)) {
					return o.RelativeAdjustment ? o.AdjustmentLevel + context.TextInfo.Size : o.AdjustmentLevel;
				}
			}

			return context.TextInfo.Size;
		}
	}
}