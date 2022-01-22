using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.util.collections;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class ReplaceFontProcessor : IPageProcessor
{
	private const string HalfWidthLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private const string FullWidthLetters = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ";
	private const string HalfWidthNumbers = "0123456789";
	private const string FullWidthNumbers = "０１２３４５６７８９";

	private static readonly string[] __LegacyFonts = {
		"宋体", "楷体_GB2312", "仿宋_GB2312", "黑体", "STSONG-LIGHT-GB-EUC-H", "STSONG-LIGHT-GBK-EUC-H"
	};

	private static readonly string[] __AlternativeFonts = { "宋体", "楷体", "仿宋", "微软雅黑", "宋体", "宋体" };
	private static readonly PdfName __GbkEncoding = new("GBK-EUC-H");
	private static readonly PdfName __GbEncoding = new("GB-EUC-H");

	private readonly bool _embedLegacyFonts;
	private readonly bool _trimTrailingWhiteSpace;
	private HashSet<int> _bypassFonts;
	private FontInfo _currentFont;
	private NewFont _currentNewFont;
	private Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>> _fontDictMap;
	private FontFactoryImp _fontFactory;
	private Dictionary<int, FontInfo> _fontInfoMap;
	private Dictionary<PdfName, NewFont> _fontMap;
	private Dictionary<PdfName, int> _fontNameIDMap;
	private Dictionary<int, NewFont> _fontRefIDMap;
	private Dictionary<string, FontSubstitution> _fontSubstitutions;

	private Dictionary<string, NewFont> _newFonts;
	//bool _usedStyle;

	public ReplaceFontProcessor(bool embedLegacyFonts, bool trimTrailingWhiteSpace,
		Dictionary<string, FontSubstitution> fontSubstitutions) {
		_embedLegacyFonts = embedLegacyFonts;
		_trimTrailingWhiteSpace = trimTrailingWhiteSpace;
		_fontSubstitutions = fontSubstitutions;
	}

	private bool ProcessCommands(IList<PdfPageCommand> parent) {
		bool r = false;
		int l = parent.Count;
		for (int i = 0; i < l; i++) {
			EnclosingCommand ec = parent[i] as EnclosingCommand;
			if (ec == null) {
				continue;
			}

			string n = ec.Name.ToString();
			if (n == "BT") {
				foreach (PdfPageCommand item in ec.Commands) {
					if (item.Type == PdfPageCommandType.Enclosure) {
						foreach (PdfPageCommand sc in (item as EnclosingCommand).Commands) {
							ProcessTextCommand(sc);
						}
					}
					else {
						if (ProcessTextCommand(item) == false) {
						}
					}
				}

				//if (_usedStyle) {
				//	_usedStyle = false;
				//}
				r = true;
			}
			else if (n == "BDC") {
				r |= ProcessCommands(ec.Commands);
				//if ((ec.Operands[0] as PdfName)?.ToString() == "/StyleSpan" && (ec.Commands[0] as EnclosingCommand).Commands[1] is MatrixCommand m && (m.Operands[0] as PdfNumber).FloatValue == 6.041f) {
				//	_usedStyle = true;
				//}
			}
			else {
				NewFont cnf = _currentNewFont;
				FontInfo cf = _currentFont;
				r |= ProcessCommands(ec.Commands);
				_currentNewFont = cnf;
				_currentFont = cf;
			}
		}

		return r;
	}

	private bool ProcessTextCommand(PdfPageCommand item) {
		string cn = item.Name.ToString();
		if (cn == "Tf") {
			PdfName cf = item.Operands[0] as PdfName;
			if (_fontMap.TryGetValue(cf, out _currentNewFont) == false) {
				//Tracker.TraceMessage ("找不到字体：" + cf.ToString ());
				_currentNewFont = null;
			}

			if (_fontNameIDMap.TryGetValue(cf, out int ni) == false
				|| _fontInfoMap.TryGetValue(ni, out _currentFont) == false) {
				_currentFont = null;
			}
		}
		else if (item.Type == PdfPageCommandType.Text) {
			if (_currentNewFont == null || _currentFont == null) {
				return false;
			}

			RewriteTextOut(_currentNewFont, _currentFont, item);
		}

		//else if (_usedStyle && item.Type == PdfPageCommandType.Matrix) {
		//	var m = item as MatrixCommand;
		//	if (m.Operands[0] is PdfNumber mn && mn.FloatValue == 10.522f) {
		//		m.Operands[5] = new PdfNumber((m.Operands[5] as PdfNumber).FloatValue + 1.05f);
		//	}
		//}
		//else if (_usedStyle && cn == "Td") {
		//	if (item.Operands[1] is PdfNumber dy && dy.FloatValue != 0) {
		//		item.Operands[1] = new PdfNumber(-1.5f);
		//	}
		//}
		return true;
	}

	private void RewriteTextOut(NewFont ef, FontInfo fontInfo, PdfPageCommand cmd) {
		PdfObject[] ops = cmd.Operands;
		TrueTypeFontUnicode f = ef.Font.BaseFont as TrueTypeFontUnicode;
		if (ops.Length == 0) {
			return;
		}

		PdfObject op = ops[0];
		string t;
		if (op.Type == PdfObject.STRING) {
			t = fontInfo.DecodeText(op as PdfString); //.TrimEnd ();
			if (_trimTrailingWhiteSpace) {
				t = t.TrimEnd();
			}

			ops[0] = RewriteText(ef, f, t);
			//AddCustomDefaultWidth (ef, fontInfo, t);
		}
		else if (op.Type == PdfObject.ARRAY) {
			PdfArray a = op as PdfArray;
			int l = a.Size;
			for (int i = 0; i < l; i++) {
				op = a[i];
				if (op.Type == PdfObject.STRING) {
					t = fontInfo.DecodeText(op as PdfString);
					if (_trimTrailingWhiteSpace /* && i == l - 1*/) {
						t = t.TrimEnd();
					}

					a[i] = RewriteText(ef, f, t);
					//AddCustomDefaultWidth (ef, fontInfo, t);
				}
			}
		}
	}

	private static void AddCustomDefaultWidth(NewFont newFont, FontInfo fontInfo, string text) {
		if (fontInfo.DefaultWidth == FontInfo.DefaultDefaultWidth) {
			return;
		}

		int dw = fontInfo.DefaultWidth;
		Dictionary<int, int> w = newFont.GlyphWidths;
		//newFont.DefaultWidth = dw;
		foreach (char ch in text) {
			if (w.ContainsKey(ch) == false) {
				w.Add(ch, dw);
			}
		}
	}

	private static PdfString RewriteText(NewFont newFont, TrueTypeFont ttf, string text) {
		bool cs = newFont.CharSubstitutions.Count > 0;
		using (ByteBuffer bb = new()) {
			foreach (char ch in text) {
				if (cs == false || newFont.CharSubstitutions.TryGetValue(ch, out char c) == false) {
					c = ch;
				}

				if (newFont.UsedCidMap.TryGetValue(c, out int cid) == false) {
					int[] tt = ttf.GetMetricsTT(c);
					if (tt == null) {
						newFont.AbsentChars.Add(c);
						continue;
					}

					cid = tt[0];
					newFont.UsedCidMap[c] = cid;
					newFont.GlyphWidths[cid] = tt[1];
				}

				bb.Append((byte)(cid >> 8));
				bb.Append((byte)cid);
			}

			return new PdfString(bb.ToByteArray());
		}
	}

	private static bool DetectLegacyCjkFont(PdfDictionary font) {
		PdfName en = font.GetAsName(PdfName.ENCODING);
		if (en == null
			|| (PdfName.WIN_ANSI_ENCODING.Equals(en) || __GbkEncoding.Equals(en) || __GbEncoding.Equals(en)) == false
		   ) {
			return false;
		}

		return PdfDocumentFont.HasEmbeddedFont(font) == false;
	}

	private void LoadFonts(PageProcessorContext context, PdfDictionary fonts) {
		Dictionary<PdfName, PRIndirectReference> r = new(fonts.Length); // 替代的字体
		foreach (KeyValuePair<PdfName, PdfObject> item in fonts) {
			if (item.Value is not PdfIndirectReference fr
				|| _bypassFonts.Contains(fr.Number)) {
				continue;
			}

			if (_fontRefIDMap.TryGetValue(fr.Number, out NewFont nf) == false) {
				PdfDictionary f = fr.CastAs<PdfDictionary>();
				if (f == null) {
					goto BYPASSFONT;
				}

				PdfName fn = f.GetAsName(PdfName.BASEFONT);
				if (fn == null) {
					goto BYPASSFONT;
				}

				string n = PdfDocumentFont.RemoveSubsetPrefix(PdfHelper.GetPdfNameString(fn)); // 字体名称
				int p = -1;
				FontSubstitution fs;
				string sn; // 替换字体名称
				if (_fontSubstitutions.TryGetValue(n, out fs)) {
					sn = fs.Substitution;
				}
				else {
					if (_embedLegacyFonts == false || DetectLegacyCjkFont(f) == false) {
						goto BYPASSFONT;
					}

					p = Array.IndexOf(__LegacyFonts, n.ToUpperInvariant());
					if (p == -1) {
						goto BYPASSFONT;
					}

					sn = null;
				}

				if (_newFonts.TryGetValue(sn ?? n, out nf) == false) {
					try {
						Tracker.TraceMessage("加载字体：" + (sn != null ? string.Concat(sn, "(替换 ", n, ")") : n));
						if (sn != null) {
							n = sn;
						}

						string sf = null;
						foreach (FontUtility.FriendlyFontName font in FontUtility.InstalledFonts) {
							if (font.DisplayName == n) {
								sf = font.OriginalName;
								break;
							}
						}

						nf = new NewFont {
							Font = _fontFactory.GetFont(sf ?? n, BaseFont.IDENTITY_H),
							FontRef = context.Pdf.AddPdfObject(new PdfDictionary()),
							DescendantFontRef = context.Pdf.AddPdfObject(new PdfArray())
						};
						if (fs.TraditionalChineseConversion != 0) {
							if (fs.TraditionalChineseConversion > 0) {
								Map(nf.CharSubstitutions, ChineseCharMap.Simplified, ChineseCharMap.Traditional);
							}
							else {
								Map(nf.CharSubstitutions, ChineseCharMap.Traditional, ChineseCharMap.Simplified);
							}
						}

						if (fs.NumericWidthConversion != 0) {
							if (fs.NumericWidthConversion > 0) {
								Map(nf.CharSubstitutions, HalfWidthNumbers, FullWidthNumbers);
							}
							else {
								Map(nf.CharSubstitutions, FullWidthNumbers, HalfWidthNumbers);
							}
						}

						if (fs.AlphabeticWidthConversion != 0) {
							if (fs.AlphabeticWidthConversion > 0) {
								Map(nf.CharSubstitutions, HalfWidthLetters, FullWidthLetters);
							}
							else {
								Map(nf.CharSubstitutions, FullWidthLetters, HalfWidthLetters);
							}
						}

						if (fs.PunctuationWidthConversion != 0) {
							if (fs.PunctuationWidthConversion > 0) {
								Map(nf.CharSubstitutions, SetCaseProcessor.HalfWidthPunctuations,
									SetCaseProcessor.FullWidthPunctuations);
							}
							else {
								Map(nf.CharSubstitutions, SetCaseProcessor.FullWidthPunctuations,
									SetCaseProcessor.HalfWidthPunctuations);
							}
						}

						if (fs?.OriginalCharacters != null && fs.SubstituteCharacters != null) {
							int sl = fs.SubstituteCharacters.Length;
							for (int i = 0; i < fs.OriginalCharacters.Length; i++) {
								if (i >= sl) {
									break;
								}

								nf.CharSubstitutions[fs.OriginalCharacters[i]] = fs.SubstituteCharacters[i];
							}
						}

						if (sn == null && p != -1 && nf.Font.BaseFont == null) {
							nf.Font = _fontFactory.GetFont(__AlternativeFonts[p], BaseFont.IDENTITY_H);
						}

						if (nf.Font.BaseFont == null) {
							throw new FileNotFoundException("无法加载字体：" + n);
						}

						_newFonts.Add(n, nf);
					}
					catch (Exception) {
						Tracker.TraceMessage(Tracker.Category.Error, "无法加载字体");
						throw;
					}
				}

				r[item.Key] = nf.FontRef;
				if (_fontInfoMap.ContainsKey(fr.Number) == false) {
					FontInfo fi = new(f, fr.Number);
					_fontInfoMap.Add(fr.Number, fi);
					//try {
					//	ReadSingleByteFontWidths (f, fi, nf);
					//	ReadCidFontWidths (f, fi, nf);
					//}
					//catch (NullReferenceException) {
					//	Tracker.TraceMessage (Tracker.Category.ImportantMessage, "字体“" + n + "”的 CID 宽度表错误。");
					//}
				}

				_fontRefIDMap[nf.FontRef.Number] = nf;
			}

			//ef.FontDictionaries[(item.Value as PdfIndirectReference).Number] = f;
			_fontMap[item.Key] = nf;
			_fontNameIDMap[item.Key] = fr.Number;
			continue;
		BYPASSFONT:
			_bypassFonts.Add(fr.Number);
		}

		if (r.Count > 0) {
			_fontDictMap[fonts] = r;
		}
	}

	private static void Map(IDictionary<char, char> map, string from, string to) {
		int i = 0;
		foreach (char item in from) {
			map[item] = to[i++];
		}
	}

	private static void ReadSingleByteFontWidths(PdfDictionary font, CMapAwareDocumentFont fontInfo, NewFont newFont) {
		PdfArray wl = font.GetAsArray(PdfName.WIDTHS);
		if (wl == null) {
			return;
		}

		int fc = font.TryGetInt32(PdfName.FIRSTCHAR, 0);
		Dictionary<int, int> widths = newFont.GlyphWidths;
		foreach (PdfNumber item in wl) {
			if (item == null) {
				continue;
			}

			string s = fontInfo.Decode(new[] { (byte)fc }, 0, 1);
			if (s.Length == 0) {
				continue;
			}

			int w;
			if (widths.TryGetValue(s[0], out w) == false || w == 0) {
				widths[s[0]] = item.IntValue;
			}

			++fc;
		}
	}

	private static void ReadCidFontWidths(PdfDictionary font, FontInfo fontInfo, NewFont newfont) {
		PdfArray w = font.GetAsArray(PdfName.W);
		if (w == null) {
			w = font.Locate<PdfArray>(PdfName.DESCENDANTFONTS, 0, PdfName.W);
			if (w == null) {
				return;
			}
		}

		int l = w.Size;
		Dictionary<int, int> widths = newfont.GlyphWidths;
		for (int i = 0; i < l; i++) {
			int cid = (w[i] as PdfNumber).IntValue;
			if (++i >= l) {
				break;
			}

			PdfObject cw = w[i];
			if (cw.Type == PdfObject.ARRAY) {
				foreach (PdfObject width in cw as PdfArray) {
					int u = fontInfo.DecodeCidToUnicode(cid);
					if (u == 0 && cid != 0) {
						Console.WriteLine(cid + "－无法解码CID");
						continue;
					}

					++cid;
					widths[u] = (width as PdfNumber).IntValue;
					Console.WriteLine(string.Join(" ", cid.ToString(), ((char)u).ToString(), widths[u].ToString()));
				}
			}
			else if (cw.Type == PdfObject.NUMBER) {
				int cid2 = (cw as PdfNumber).IntValue + 1;
				int width = (w[++i] as PdfNumber).IntValue;
				do {
					int u = fontInfo.DecodeCidToUnicode(cid);
					if (u == 0 && cid != 0) {
						Console.WriteLine(cid + "－无法解码CID");
						continue;
					}

					widths[u] = width;
					Console.WriteLine(string.Join(" ", cid.ToString(), ((char)u).ToString(), width.ToString()));
				} while (++cid < cid2);
			}
		}
	}

	private static void ChangeLegacyFontDictionary(PdfReader pdf, NewFont font) {
		PdfDictionary f = PdfReader.GetPdfObject(font.FontRef) as PdfDictionary;
		f.Put(PdfName.TYPE, PdfName.FONT);
		f.Put(PdfName.SUBTYPE, PdfName.TYPE0);
		f.Put(PdfName.BASEFONT, new PdfName(font.FontName));
		f.Put(PdfName.ENCODING, new PdfName(BaseFont.IDENTITY_H));
		f.Put(PdfName.DESCENDANTFONTS, font.DescendantFontRef);
		int[][] metrics = new int[font.UsedCidMap.Count][];
		int i = -1;
		foreach (KeyValuePair<int, int> m in font.UsedCidMap) {
			metrics[++i] = new[] { m.Value, 0, m.Key };
		}

		TrueTypeFontUnicode ttf = font.Font.BaseFont as TrueTypeFontUnicode;
		Array.Sort(metrics, ttf);
		PRIndirectReference u = pdf.AddPdfObject(ttf.GetToUnicode(metrics));
		f.Put(PdfName.TOUNICODE, u);
	}

	private static void WriteCidWidths(NewFont font, PdfDictionary fontDictionary) {
		int l = font.GlyphWidths.Count;
		if (l == 0) {
			return;
		}

		CharacterWidth[] widths = new CharacterWidth[l];
		int i = -1;
		foreach (KeyValuePair<int, int> item in font.GlyphWidths) {
			if (item.Value == FontInfo.DefaultDefaultWidth) {
				continue;
			}

			widths[++i] = new CharacterWidth(item.Key, item.Value);
		}

		l = ++i;
		Array.Resize(ref widths, l);
		Array.Sort(widths, CharacterWidth.Compare);
		PdfArray w = new();
		for (i = 0; i < l; i++) {
			int id = widths[i].ID;
			w.Add(new PdfNumber(id));
			int width = widths[i].Width;
			int i2 = i;
			int id2 = id;
			PdfArray wl = new() { new PdfNumber(width) };
			CharacterWidth cw;
			while (++i2 < l && (cw = widths[i2]).ID == ++id2 && cw.Width != width) {
				wl.Add(new PdfNumber(cw.Width));
			}

			if (wl.Size > 1) {
				w.Add(wl);
				i = i2 - 1;
				continue;
			}

			id2 = id;
			for (i2 = i + 1; i2 < l; i2++) {
				cw = widths[i2];
				if (++id2 != cw.ID || cw.Width != width) {
					i2--;
					id2--;
					break;
				}
			}

			if (i2 > i) {
				w.Add(new PdfNumber(id2));
				w.Add(new PdfNumber(width));
				i = i2;
			}
			else {
				w.Add(wl);
			}
		}

		if (w.Size > 0) {
			fontDictionary.Put(PdfName.W, w);
		}
	}

	private void SubSetFontData(PdfReader pdf) {
		foreach (KeyValuePair<string, NewFont> font in _newFonts) {
			NewFont newFont = font.Value;
			Tracker.TraceMessage("嵌入字体：" + newFont.Font.Familyname + "(" + newFont.UsedCidMap.Count + "字)");
			if (newFont.AbsentChars.Count > 0) {
				Tracker.TraceMessage(Tracker.Category.ImportantMessage,
					string.Concat("丢失", newFont.AbsentChars.Count, "字：", new string(newFont.AbsentChars.ToArray())));
			}

			ChangeLegacyFontDictionary(pdf, newFont);

			TrueTypeFontUnicode ttf = newFont.Font.BaseFont as TrueTypeFontUnicode;
			PdfArray fa = PdfReader.GetPdfObject(newFont.DescendantFontRef) as PdfArray;
			PdfDictionary df = new();
			fa.Add(df);
			df.Put(PdfName.TYPE, PdfName.FONT);
			df.Put(PdfName.SUBTYPE, ttf.Cff ? PdfName.CIDFONTTYPE0 : PdfName.CIDFONTTYPE2);
			df.Put(PdfName.BASEFONT, new PdfName(newFont.FontName));
			df.Put(PdfName.CIDTOGIDMAP, PdfName.IDENTITY);
			df.Put(PdfName.DW, FontInfo.DefaultDefaultWidth);
			PRIndirectReference fs = pdf.AddPdfObject(SubsetFont(newFont, ttf));
			PdfDictionary fd = ttf.GetFontDescriptor(fs, newFont.SubsetPrefix, null);
			df.Put(PdfName.FONTDESCRIPTOR, pdf.AddPdfObject(fd));
			WriteCidWidths(newFont, df);

			PdfDictionary csi = new();
			csi.Put(PdfName.REGISTRY, new PdfString("Adobe"));
			csi.Put(PdfName.ORDERING, new PdfString("Identity"));
			csi.Put(PdfName.SUPPLEMENT, new PdfNumber(0));
			df.Put(PdfName.CIDSYSTEMINFO, csi);
		}
	}

	private static PdfStream SubsetFont(NewFont ef, TrueTypeFont ttf) {
		//ttf.AddSubsetRange (r);
		byte[] b;
		if (ttf.Cff) {
			Dictionary<int, int[]> d = new(ef.UsedCidMap.Count);
			foreach (KeyValuePair<int, int> item in ef.UsedCidMap) {
				int[] metricsTT = ttf.GetMetricsTT(item.Key);
				d.Add(item.Value, new[] { metricsTT[0], metricsTT[1], item.Key });
			}

			//ttf.AddRangeUni (d, false, true);
			CFFFontSubset f = new(new RandomAccessFileOrArray(ttf.ReadCffFont()), d);
			b = f.Process(f.GetNames()[0]);
		}
		else {
			int[] r = new int[ef.UsedCidMap.Count];
			ef.UsedCidMap.Values.CopyTo(r, 0);
			TrueTypeFontSubSet ts = new(ttf.FileName, new RandomAccessFileOrArray(ttf.FileName), new HashSet2<int>(r),
				ttf.DirectoryOffset, true, true);
			b = ts.Process();
		}

		PdfStream s = new(b);
		if (ttf.Cff) {
			s.Put(PdfName.SUBTYPE, new PdfName("CIDFontType0C"));
		}

		s.FlateCompress();
		s.Put(PdfName.LENGTH1, new PdfNumber(b.Length));
		return s;
	}

	[DebuggerDisplay("{ID},{Width}")]
	private struct CharacterWidth
	{
		public int ID, Width;

		public CharacterWidth(int id, int width) {
			ID = id;
			Width = width;
		}

		public static int Compare(CharacterWidth x, CharacterWidth y) {
			return x.ID.CompareTo(y.ID);
		}
	}

	[DebuggerDisplay("{FontName}")]
	private sealed class NewFont
	{
		private Font _Font;

		public NewFont() {
			GlyphWidths = new Dictionary<int, int>();
			FontDictionaries = new Dictionary<int, PdfDictionary>();
			UsedCidMap = new Dictionary<int, int>();
			AbsentChars = new HashSet<char>();
			CharSubstitutions = new Dictionary<char, char>();
		}

		private Dictionary<int, PdfDictionary> FontDictionaries { get; }
		public PRIndirectReference FontRef { get; set; }
		public PdfIndirectReference DescendantFontRef { get; set; }

		/// <summary>
		///     字体 Unicode 到宽度的映射表。
		/// </summary>
		public Dictionary<int, int> GlyphWidths { get; }

		/// <summary>
		///     字体 Unicode 和 CID 的映射表。
		/// </summary>
		public Dictionary<int, int> UsedCidMap { get; }

		public string SubsetPrefix { get; private set; }
		public string FontName => SubsetPrefix + _Font.Familyname;
		public HashSet<char> AbsentChars { get; }
		public Dictionary<char, char> CharSubstitutions { get; }

		public Font Font {
			get => _Font;
			set {
				_Font = value;
				SubsetPrefix = BaseFont.CreateSubsetPrefix();
			}
		}
	}

	#region IPageProcessor 成员

	public string Name => "嵌入汉字库";

	public void BeginProcess(DocProcessorContext context) {
		if (_fontSubstitutions == null) {
			_fontSubstitutions = new Dictionary<string, FontSubstitution>(0);
		}

		int l = __LegacyFonts.Length + _fontSubstitutions.Count;
		_newFonts = new Dictionary<string, NewFont>(l, StringComparer.CurrentCultureIgnoreCase);
		_fontMap = new Dictionary<PdfName, NewFont>(l);
		_fontNameIDMap = new Dictionary<PdfName, int>();
		_fontInfoMap = new Dictionary<int, FontInfo>();
		_fontFactory = new FontFactoryImp();
		_fontRefIDMap = new Dictionary<int, NewFont>();
		_fontDictMap = new Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>>();
		_bypassFonts = new HashSet<int>();
		foreach (KeyValuePair<string, string> item in FontHelper.GetInstalledFonts(true)) {
			try {
				_fontFactory.Register(item.Value, item.Key);
			}
			catch (Exception) {
				// ignore
			}
		}
		//_fontFactory.RegisterDirectory (Common.FontHelper.FontDirectory);
	}

	public bool EndProcess(PdfReader pdf) {
		// 用新的字体引用替代字体资源表的字体
		foreach (KeyValuePair<PdfDictionary, Dictionary<PdfName, PRIndirectReference>> map in _fontDictMap) {
			PdfDictionary d = map.Key;
			foreach (KeyValuePair<PdfName, PRIndirectReference> item in map.Value) {
				d.Put(item.Key, item.Value);
			}
		}

		SubSetFontData(pdf);
		return false;
	}

	public int EstimateWorkload(PdfReader pdf) {
		return pdf.NumberOfPages;
	}

	public bool Process(PageProcessorContext context) {
		Tracker.IncrementProgress(1);
		PdfDictionary fonts = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.FONT);
		if (fonts == null) {
			return false;
		}

		//bool hasAnsiCjkFont = DetectLegacyCjkFont (context, fonts);
		//if (hasAnsiCjkFont == false) {
		//    return false;
		//}
		_currentFont = null;
		_currentNewFont = null;
		_fontNameIDMap.Clear();
		_fontMap.Clear();
		LoadFonts(context, fonts);
		if (_fontMap.Count == 0) {
			return false;
		}

		if (ProcessCommands(context.PageCommands.Commands)) {
			context.IsPageContentModified = true;
			return true;
		}

		return false;
	}

	#endregion
}