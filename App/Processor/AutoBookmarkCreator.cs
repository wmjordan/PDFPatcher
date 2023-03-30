using System;
using System.Collections.Generic;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class AutoBookmarkCreator
	{
		sealed class SizeOccurrence
		{
			public float Size { get; set; }
			public int FirstPage { get; set; }
			public string FirstInstance { get; set; }
			public int Occurrence { get; set; }
			public SizeOccurrence(float size, int page, string instance) {
				Size = size;
				Occurrence = 1;
				FirstPage = page;
				FirstInstance = instance.Length > 50 ? instance.Substring(0, 50) : instance;
			}
		}
		sealed class FontOccurrence
		{
			readonly Dictionary<string, List<SizeOccurrence>> oc = new Dictionary<string, List<SizeOccurrence>>();
			internal List<SizeOccurrence> GetOccurrence(string fontName) {
				return oc.TryGetValue(fontName, out List<SizeOccurrence> s) ? s : null;
			}

			internal void AddOccurrence(string fontName, float size, int page, string instance) {
				if (oc.ContainsKey(fontName) == false) {
					oc.Add(fontName, new List<SizeOccurrence>() { new SizeOccurrence(size, page, instance) });
				}
				else {
					var o = oc[fontName].Find((s) => { return s.Size == size; });
					if (o != null) {
						o.Occurrence++;
					}
					else {
						oc[fontName].Add(new SizeOccurrence(size, page, instance));
					}
				}
			}
		}
		const string __AddSpaceAfterCharacters = ":.,\"'?!)]};";
		const string __InsertSpaceBeforeCharacters = "\"'([{";

		readonly PdfReader _reader;
		readonly AutoBookmarkOptions _options;
		const int OpenWorkload = 10;
		public AutoBookmarkCreator(PdfReader reader, AutoBookmarkOptions options) {
			_reader = reader;
			_options = options;
			TextLine.DefaultDirection = options.WritingDirection;
		}

		internal int EstimateWorkload() {
			var n = _reader.NumberOfPages;
			var load = 0;
			load += OpenWorkload;
			var t = PageRangeCollection.Parse(_options.PageRanges, 1, n, true).TotalPages;
			load += t > 0 ? t : n;
			return load;
		}

		internal void ExportAutoBookmarks(XmlWriter w, AutoBookmarkOptions options) {
			AutoCreateBookmarks(w, _reader, options);
		}

		internal void AutoCreateBookmarks(XmlWriter writer, PdfReader reader, AutoBookmarkOptions options) {
			Tracker.IncrementProgress(OpenWorkload);
			int pn = reader.NumberOfPages + 1;
			var c = new AutoBookmarkContext() { TotalPageNumber = reader.NumberOfPages };
			var p = new TextToBookmarkProcessor(options, c);
			var lp = new LevelProcessor(options.LevelAdjustment);
			var ranges = PageRangeCollection.Parse(options.PageRanges, 1, reader.NumberOfPages, true);
			var doc = new XmlDocument();
			var be = doc.AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
			float size = -1;
			var sizes = new Stack<float>();
			float yOffset = 1 + options.YOffset;
			int level = 0;
			const string indentString = "　　　　　　　　　　";
			List<MatchPattern.IMatcher> ig;
			var fontOccurrences = new FontOccurrence();
			if (options.IgnorePatterns.Count == 0) {
				ig = null;
			}
			else {
				ig = new List<MatchPattern.IMatcher>();
				foreach (var item in options.IgnorePatterns) {
					if (String.IsNullOrEmpty(item.Text)) {
						continue;
					}
					try {
						ig.Add(item.CreateMatcher());
					}
					catch (ArgumentException ex) {
						Tracker.TraceMessage(Tracker.Category.Alert, (String.Concat("忽略文本（", item, "）无效：", ex.Message)));
					}
				}
			}

			XmlWriter oldWriter = null;
			if (options.ExportTextCoordinates == false) {
				oldWriter = writer;
				writer = new Processor.NullXmlWriter();
			}
			foreach (PageRange r in ranges) {
				for (int i = r.StartValue; i <= r.EndValue && i < pn; i++) {
					if (i == 1 && options.CreateBookmarkForFirstPage) {
						continue;
					}
					//Tracker.TraceMessage (String.Concat ("分析第 ", i, " 页。"));
					var box = reader.GetCropBox(i);
					p.Reset();
					c.PageBox = box;
					c.CurrentPageNumber = i;
					var pr = reader.GetPageRotation(i);
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
					var ptl = p.TextList;
					for (int li = ptl.Count - 1; li >= 0; li--) {
						c.TextInfo = ptl[li];
						if (lp.ChangeSizeLevel(c) < options.TitleThreshold) {
							ptl.RemoveAt(li);
						}
					}
					var tl = MergeTextInfoList(box, ptl, _options);
					// TODO: 筛选文本
					c.IsTextMerged = true;
					for (int li = tl.Count - 1; li >= 0; li--) {
						c.TextLine = tl[li];
						c.TextInfo = c.TextLine.FirstText;
						if ((c.TextInfo.Size = lp.ChangeSizeLevel(c)) < options.TitleThreshold) {
							tl.RemoveAt(li);
						}
					}
					var tr = MergeTextLines(box, tl);
					if (tr != null && tr.Count > 0) {
						if (options.WritingDirection != WritingDirection.Unknown) {
							tr.Sort((a, b) => {
								var ra = a.Region;
								var rb = b.Region;
								if (ra.Middle < rb.Middle) return 1;
								else if (ra.Middle > rb.Middle) return -1;
								else if (ra.Center > rb.Center) return 1;
								else if (ra.Center < rb.Center) return -1;
								else return 0;
							});
						}
						writer.WriteStartElement(Constants.Content.Texts);
						writer.WriteValue(Constants.Content.PageNumber, i);
						foreach (var item in tr) {
							var t = PdfHelper.GetValidXmlString(ConcatRegionText(item)).Trim();
							var f = item.Lines[0].FirstText.Font;
							var s = item.Lines[0].FirstText.Size;
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
								foreach (var rg in ig) {
									if (rg.Matches(t)) {
										ignore = true;
										continue;
									}
								}
								if (ignore) {
									continue;
								}
							}
							if (options.AutoHierarchicalArrangement) {
								do {
									//if (ValueHelper.TryParse (be.GetAttribute (Constants.Font.Size), out size) == false || s < size) {
									if (sizes.Count == 0 || s < (size = sizes.Peek())) {
										be = be.AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
										sizes.Push(s);
										level++;
										break;
									}
									else if (s == size) {
										be = (be.ParentNode ?? be).AppendChild(doc.CreateElement(Constants.Bookmark)) as XmlElement;
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
							be.SetAttribute(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
							be.SetAttribute(Constants.Coordinates.Top, ValueHelper.ToText(item.Region.Top + s * yOffset));
							be.SetAttribute(Constants.Font.Size, s.ToText());
							if (f != null) {
								be.SetAttribute(Constants.Font.ThisName, f.FontID.ToText());
							}
							CountFontOccurrenceInRegion(fontOccurrences, i, item);
#if DEBUG
							Tracker.TraceMessage(String.Concat(item.Direction.ToString()[0], ':', level < 11 ? indentString.Substring(0, level) : indentString, t, " .... ", i.ToText()));
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

			WriteDocumentFontOccurrences(writer, options, p, fontOccurrences);
			SetGoToTop(options, doc);
			writer.WriteStartElement(Constants.DocumentBookmark);
			if (options.CreateBookmarkForFirstPage && String.IsNullOrEmpty(options.FirstPageTitle) == false) {
				writer.WriteStartElement(Constants.Bookmark);
				writer.WriteAttributeString(Constants.BookmarkAttributes.Title, options.FirstPageTitle);
				writer.WriteAttributeString(Constants.DestinationAttributes.Page, "1");
				writer.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
				writer.WriteEndElement();
			}
			doc.DocumentElement.WriteContentTo(writer);
			writer.WriteEndElement();
		}

		static void SetGoToTop(AutoBookmarkOptions options, XmlDocument doc) {
			if (options.PageTopForLevel > 0) {
				var topics = doc.DocumentElement.SelectNodes(".//书签[count(ancestor::书签) < " + (options.PageTopForLevel + 1) + "]");
				foreach (XmlElement t in topics) {
					t.RemoveAttribute(Constants.Coordinates.Top);
				}
			}
		}

		static void WriteDocumentFontOccurrences(XmlWriter writer, AutoBookmarkOptions options, TextToBookmarkProcessor p, FontOccurrence fontOccurrences) {
			writer.WriteStartElement(Constants.Font.DocumentFont);
			Tracker.TraceMessage("\n文档所用的字体");
			var dl = new List<String>();
			foreach (var item in p.FontList) {
				var fo = "0";
				var sl = fontOccurrences.GetOccurrence(item.Value);
				if (sl != null) {
					if (dl.Contains(item.Value) == false) {
						int o = 0;
						foreach (var s in sl) {
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
					Tracker.TraceMessage(String.Concat("编号：", item.Key, "\t出现次数：", fo, "\t名称：", item.Value));
				}
				writer.WriteStartElement(Constants.Font.ThisName);
				writer.WriteAttributeString(Constants.Font.ID, item.Key.ToText());
				writer.WriteAttributeString(Constants.Font.Name, item.Value);
				writer.WriteAttributeString(Constants.FontOccurrence.Count, fo);
				if (sl != null) {
					foreach (var s in sl) {
						writer.WriteStartElement(Constants.Font.Size);
						writer.WriteAttributeString(Constants.Font.Size, s.Size.ToText());
						writer.WriteAttributeString(Constants.FontOccurrence.Count, s.Occurrence.ToText());
						writer.WriteAttributeString(Constants.FontOccurrence.FirstText, s.FirstInstance);
						writer.WriteAttributeString(Constants.FontOccurrence.FirstPage, s.FirstPage.ToText());
						if (options.DisplayFontStatistics && (s.Occurrence > 0 || options.DisplayAllFonts)) {
							Tracker.TraceMessage(String.Concat("\t尺寸：", s.Size.ToText(), "\t出现次数：", s.Occurrence.ToText(), "\t首次出现于第", s.FirstPage.ToText(), "页（", s.FirstInstance, "）"));
						}
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		static void CountFontOccurrenceInRegion(FontOccurrence fontOccurrences, int i, TextRegion item) {
			FontInfo f = null;
			foreach (var il in item.Lines) {
				foreach (var ii in il.Texts) {
					if (ii.Font != null && (f == null || ii.Font.FontID != f.FontID)) {
						fontOccurrences.AddOccurrence(ii.Font.FontName, ii.Size, i, il.Text);
						f = ii.Font;
					}
				}
			}
		}

		static string ConcatRegionText(TextRegion region) {
			var ls = region.Lines;
			if (ls.Count == 0) {
				return String.Empty;
			}
			else if (ls.Count == 1) {
				return ls[0].Text;
			}
			ls = new List<TextLine>(ls);
			if (region.Direction == WritingDirection.Vertical) {
				ls.Sort((a, b) => {
					if (a.Region.Middle < b.Region.Middle) {
						return 1;
					}
					else if (a.Region.Middle > b.Region.Middle) {
						return -1;
					}
					return 0;
				});
			}
			var sb = StringBuilderCache.Acquire();
			sb.Append(ls[0].Text);
			for (int i = 1; i < ls.Count; i++) {
				var l = ls[i].Text;
				var ll = ls[i - 1].Text;
				if (ll.Length > 0 && l.Length > 0) {
					var c1 = l[l.Length - 1];
					var c2 = ll[0];
					if ((__AddSpaceAfterCharacters.IndexOf(c1) != -1
							|| (Char.IsLetterOrDigit(c1) && c1 < 0x4E00 /*非中文字符*/))
						&& (__InsertSpaceBeforeCharacters.IndexOf(c2) != -1
							|| (Char.IsLetterOrDigit(c2) && c2 < 0x4E00))) {
						sb.Append(' ');
					}
				}
				sb.Append(l);
			}
			return StringBuilderCache.GetStringAndRelease(sb);
		}

		/// <summary>
		/// 使用最小距离法将 <paramref name="textInfos"/> 的文本聚类为 <see cref="TextLine"/> 列表。
		/// </summary>
		/// <param name="textInfos">包含文本位置及尺寸信息的 <see cref="TextInfo"/> 集合。</param>
		/// <returns>聚类后所得的 <see cref="TextLine"/> 列表。</returns>
		internal static List<TextLine> MergeTextInfoList(Rectangle pageBox, IList<TextInfo> textInfos, AutoBookmarkOptions options) {
			var ll = new List<TextLine>();
			// 同行合并宽度最小值
			var cw = pageBox.Width / 6;
			var ch = pageBox.Height / 6;

			var dirCount = new int[4];
			// 遍历识别所得的各 TextInfo，使用最小距离聚类方法将其聚类为行
			foreach (var item in textInfos) {
				var ir = item.Region;
				DistanceInfo cd = null; // TextInfo 到 TextLine 的距离
				var md = new DistanceInfo(DistanceInfo.Placement.Unknown, float.MaxValue, float.MaxValue); // 最小距离
				TextLine ml = null; // 最小距离的 TextLine

				// 求最小距离的 TextLine
				float ds = item.Size / 10;
				// 循环只包含了 TextLine，未包含文本 TextInfo 的其余文本
				var end = ll.Count > 5 ? ll.Count - 5 : 0;
				for (int i = ll.Count - 1; i >= end; i--) {
					var li = ll[i];
					// 文本尺寸应在误差范围之内
					if (Math.Abs(item.Size - li.FirstText.Size) > ds && options.MergeDifferentSizeTitles == false) {
						continue;
					}
					if (options.MergeDifferentFontTitles == false && li.FirstText.Font.FontID != item.Font.FontID) {
						break;
					}
					cd = li.GetDistance(ir);
					if ((cd.IsOverlapping // 当前项与文本行交叠
							&& (md.IsOverlapping == false // 最小距离不是交叠
								|| cd.DistanceRadial < md.DistanceRadial) // 当前项与文本行的交叠中心距离小于最小距离
							)
						//&& (options.MergeDifferentFontTitles || li.FirstText.Font.FontID == item.Font.FontID)
						|| ((md.Location == DistanceInfo.Placement.Unknown // 未知最小距离
							|| (cd.IsOverlapping == false
								&& md.IsOverlapping == false
								&& cd.MinDistance < md.MinDistance) // 当前项与文本行的距离小于最小距离
							)
							&& (((cd.IsHorizontallyAligned) // 相对位置为水平
									&& li.Direction != WritingDirection.Vertical // 文本行方向不为纵向
									&& item.Region.IsAlignedWith(li.Region, WritingDirection.Horizontal) // 两者处于同一横行
									)
								|| ((cd.IsVerticallyAligned) // 相对位置为垂直
									&& li.Direction != WritingDirection.Horizontal // 文本行方向不为横向
									&& item.Region.IsAlignedWith(li.Region, WritingDirection.Vertical) // 两者处于同一纵行
																									   // && Math.Abs (item.Region.Middle - li.Region.Middle) < li.Region.Height // 行间距离小于行高
									)
								)
							&& (options.DetectColumns == false || cd.MinDistance < cw)
							&& (options.MergeDifferentFontTitles || li.FirstText.Font.FontID == item.Font.FontID)
						)
						) {
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
				}

				// 否则，用 item 创建新的 TextLine
				if (item.Text.Length == 0) {
					item.Text = " ";
				}
				if (ml != null) {
					// 若存在最小距离的 TextLine 且可合并，则将 item 归入 TextLine
					if (md.IsOverlapping && options.IgnoreOverlappedText) {
						// 检查是否存在交叠重复的文本
						foreach (var t in ml.Texts) {
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
			Next:
				continue;
			}
			return ll;
		}

		internal List<TextRegion> MergeTextLines(Rectangle pageBox, IList<TextLine> textLines) {
			var ll = new List<TextRegion>();
			// 同行合并宽度最小值
			var cw = pageBox.Width / 6;
			var ch = pageBox.Height / 6;

			// 遍历识别所得的各 TextInfo，使用最小距离聚类方法将其聚类为行
			foreach (var item in textLines) {
				var ir = item.Region;
				DistanceInfo cd = null; // TextInfo 到 TextLine 的距离
				var md = new DistanceInfo(DistanceInfo.Placement.Unknown, float.MaxValue, float.MaxValue); // 最小距离
				TextRegion mr = null; // 最小距离的 TextRegion

				// 求最小距离的 TextLine
				float ds = item.FirstText.Size / 10;
				// 循环只包含了 TextLine，未包含文本 TextInfo 的其余文本
				for (int i = ll.Count - 1; i >= 0; i--) {
					var li = ll[i];
					// 文本尺寸应在误差范围之内
					if (Math.Abs(item.FirstText.Size - li.Lines[0].FirstText.Size) > ds && _options.MergeAdjacentTitles) {
						continue;
					}
					if (_options.MergeDifferentFontTitles == false && li.Lines[0].FirstText.Font.FontID != item.FirstText.Font.FontID) {
						break;
					}
					cd = li.Region.GetDistance(ir, li.Direction);
					if ((cd.IsOverlapping // 当前项与文本行交叠
							&& (md.IsOverlapping == false // 最小距离不是交叠
								|| cd.DistanceRadial < md.DistanceRadial) // 当前项与文本行的交叠中心距离小于最小距离
							)
						|| ((md.Location == DistanceInfo.Placement.Unknown // 未知最小距离
							|| (cd.IsOverlapping == false
								&& md.IsOverlapping == false
								&& cd.MinDistance < md.MinDistance) // 当前项与文本行的距离小于最小距离
							)
							&& (((cd.IsHorizontallyAligned) // 相对位置为水平
									&& li.Direction != WritingDirection.Vertical // 文本行方向不为纵向
									&& item.Region.IsAlignedWith(li.Region, WritingDirection.Horizontal) // 两者处于同一横行
									&& cd.MinDistance < item.Region.Width * _options.MaxDistanceBetweenLines // 行间距离小于指定行宽
									&& _options.MergeAdjacentTitles
									&& (_options.MergeDifferentSizeTitles || li.Lines[0].Region.Width == item.Region.Width) // 合并相同尺寸的标题
									)
								|| ((cd.IsVerticallyAligned) // 相对位置为垂直
									&& li.Direction != WritingDirection.Horizontal // 文本行方向不为横向
									&& item.Region.IsAlignedWith(li.Region, WritingDirection.Vertical) // 两者处于同一纵行
									&& cd.MinDistance < item.Region.Height * _options.MaxDistanceBetweenLines // 行间距离小于指定行高
									&& _options.MergeAdjacentTitles
									&& (_options.MergeDifferentSizeTitles || li.Lines[0].Region.Height == item.Region.Height) // 合并相同尺寸的标题
									)
								)
							&& cd.MinDistance < cw
						)
						) {
						md = cd;
						mr = li;
					}
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

		sealed class TextToBookmarkProcessor : PdfContentStreamProcessor
		{
			readonly float _fontSizeThreshold;

			//Rectangle _positionRectangle;
			readonly bool _mergeAdjacentTitles;
			readonly bool _mergeDifferentSizeTitles;
			float _textWidth, _charWidth;
			readonly List<TextInfo> _TextList;
			readonly LevelProcessor _levelProcessor;
			readonly AutoBookmarkContext _context;
			const string __AddSpaceAfterCharacters = ":.,\"'?!)]};";
			const string __InsertSpaceBeforeCharacters = "\"'([{";

			public TextToBookmarkProcessor(AutoBookmarkOptions options, AutoBookmarkContext context) {
				_fontSizeThreshold = options.TitleThreshold;
				//_positionRectangle = options.PositionRectangle;
				_mergeAdjacentTitles = options.MergeAdjacentTitles;
				_mergeDifferentSizeTitles = options.MergeDifferentSizeTitles;
				_levelProcessor = new LevelProcessor(options.LevelAdjustment);
				_TextList = new List<TextInfo>();
				PopulateOperators();
				RegisterContentOperator("TJ", new AccumulatedShowTextArray());
				_context = context;
			}

			/// <summary>
			/// 获取页面内容的文本。
			/// </summary>
			internal List<TextInfo> TextList => _TextList;
			/// <summary>
			/// 获取字体列表。
			/// </summary>
			internal IDictionary<int, string> FontList => base.Fonts;

			public Matrix RotationMatrix { get; set; }

			internal override void Reset() {
				base.Reset();
				_TextList?.Clear();
			}

			protected override void DisplayPdfString(PdfString str) {
				var gs = CurrentGraphicState;
				var font = gs.Font;
				var chars = font.DecodeText(str).ToCharArray();
				float totalWidth = 0, charWidth = 0;
				foreach (var c in chars) {
					float w = font.GetWidth(c) / 1000.0f;
					if (w == 0 && (font.CjkType & FontInfo.CjkFontType.CJK) > 0) {
						w = c < 0xFF ? 0.5f : 1f;
					}
					float wordSpacing = (c == ' ' ? gs.WordSpacing : 0f);
					if (Char.IsLetterOrDigit(c)) {
						charWidth += w * gs.FontSize * gs.HorizontalScaling;
					}
					totalWidth += (w * gs.FontSize + gs.CharacterSpacing + wordSpacing) * gs.HorizontalScaling;
				}

				_textWidth = totalWidth;
				_charWidth = charWidth;
				AdjustTextMatrixX(totalWidth);
			}

			protected override void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
				var o = oper.ToString();
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
				var ti = new TextInfo() {
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
				_TextList.Add(ti);
			Exit:
				return;
			}

			string DecodeTJText(List<PdfObject> operands, float size) {
				//if (size < _fontSizeThreshold) {
				//    goto default;
				//}
				var array = (PdfArray)operands[0];
				float d = size * CurrentGraphicState.HorizontalScaling * 4f / 1000f;
				var t = new string[array.Size];
				int i = 0;
				foreach (PdfObject item in array.ArrayList) {
					if (item.Type == PdfObject.STRING) {
						t[i++] = CurrentGraphicState.Font.DecodeText(item as PdfString);
					}
					else if (item.Type == PdfObject.NUMBER) {
						if (-(item as PdfNumber).FloatValue * d > size) {
							t[i++] = " ";
						}
					}
				}
				return String.Concat(t);
			}

			float GetFontSize(Matrix tm) {
				float size = CurrentGraphicState.FontSize * tm[Matrix.I22];
				if (size < 0) {
					size = -size;
				}
				return size;
			}

			Matrix GetTextMatrix() {
				return RotationMatrix != null
					? RotationMatrix.Multiply(TextMatrix).Multiply(CurrentGraphicState.TransMatrix)
					: TextMatrix.Multiply(CurrentGraphicState.TransMatrix);
			}

			static Bound CreateBoundFromMatrix(Matrix tm, float textWidth, float size) {
				var l = tm[Matrix.I31];
				var b = tm[Matrix.I32];
				var r = tm[Matrix.I31] + textWidth * tm[Matrix.I11];
				var t = tm[Matrix.I32] + size;
				float x;
				if (l > r) {
					x = r; r = l; l = x;
				}
				if (b > t) {
					x = t; t = b; b = x;
				}
				return new Bound(l, b, r, t);
			}

			/// <summary>
			/// 检查 <paramref name="b"/> 是否处于 <paramref name="a"/> 之内。
			/// </summary>
			/// <param name="a">大边框。</param>
			/// <param name="b">小边框。</param>
			/// <returns>小边框完全处于大边框内，则返回 true。</returns>
			internal static bool IsBoundOutOfRectangle(Rectangle a, Bound b) {
				return b.Right < a.Left
						|| b.Top < a.Bottom
						|| b.Bottom > a.Top
						|| b.Left > a.Right;
			}

		}

		sealed class LevelProcessor
		{
			readonly AutoBookmarkFilter[] _filters;
			readonly AutoBookmarkOptions.LevelAdjustmentOption[] _options;

			internal LevelProcessor(IList<AutoBookmarkOptions.LevelAdjustmentOption> options) {
				var l = options.Count;
				_filters = new AutoBookmarkFilter[l];
				_options = new AutoBookmarkOptions.LevelAdjustmentOption[l];
				for (int i = 0; i < l; i++) {
					_filters[i] = options[i].Condition.CreateFilter();
					_options[i] = options[i];
				}
			}

			internal float ChangeSizeLevel(AutoBookmarkContext context) {
				for (int i = 0; i < _options.Length; i++) {
					var o = _options[i];
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
}
