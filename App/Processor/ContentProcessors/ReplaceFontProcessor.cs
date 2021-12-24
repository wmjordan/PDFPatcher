using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	sealed class ReplaceFontProcessor : IPageProcessor
	{
		static readonly string[] __LegacyFonts = { "宋体", "楷体_GB2312", "仿宋_GB2312", "黑体", "STSONG-LIGHT-GB-EUC-H", "STSONG-LIGHT-GBK-EUC-H" };
		static readonly string[] __AlternativeFonts = { "宋体", "楷体", "仿宋", "微软雅黑", "宋体", "宋体" };
		static readonly PdfName __GbkEncoding = new PdfName ("GBK-EUC-H");
		static readonly PdfName __GbEncoding = new PdfName ("GB-EUC-H");

		readonly bool _embedLegacyFonts;
		readonly bool _trimTrailingWhiteSpace;
		NewFont _currentNewFont;
		FontInfo _currentFont;
		FontFactoryImp _fontFactory;
		Dictionary<string, NewFont> _newFonts;
		Dictionary<PdfName, NewFont> _fontMap;
		Dictionary<PdfName, int> _fontNameIDMap;
		Dictionary<int, FontInfo> _fontInfoMap;
		Dictionary<string, FontSubstitution> _fontSubstitutions;
		Dictionary<int, NewFont> _fontRefIDMap;
		Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>> _fontDictMap;
		HashSet<int> _bypassFonts;
		//bool _usedStyle;

		public ReplaceFontProcessor (bool embedLegacyFonts, bool trimTrailingWhiteSpace, Dictionary<string, FontSubstitution> fontSubstitutions) {
			_embedLegacyFonts = embedLegacyFonts;
			_trimTrailingWhiteSpace = trimTrailingWhiteSpace;
			_fontSubstitutions = fontSubstitutions;
		}
		#region IPageProcessor 成员
		public string Name { get { return "嵌入汉字库"; } }
		public void BeginProcess (DocProcessorContext context) {
			if (_fontSubstitutions == null) {
				_fontSubstitutions = new Dictionary<string, FontSubstitution> (0);
			}
			var l = __LegacyFonts.Length + _fontSubstitutions.Count;
			_newFonts = new Dictionary<string, NewFont> (l, StringComparer.CurrentCultureIgnoreCase);
			_fontMap = new Dictionary<PdfName, NewFont> (l);
			_fontNameIDMap = new Dictionary<PdfName, int> ();
			_fontInfoMap = new Dictionary<int, FontInfo> ();
			_fontFactory = new FontFactoryImp ();
			_fontRefIDMap = new Dictionary<int, NewFont> ();
			_fontDictMap = new Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>> ();
			_bypassFonts = new HashSet<int> ();
			foreach (var item in Common.FontHelper.GetInstalledFonts (true)) {
				try {
					_fontFactory.Register (item.Value, item.Key);
				}
				catch (Exception ex) {
					// ignore
				}
			}
			//_fontFactory.RegisterDirectory (Common.FontHelper.FontDirectory);
		}
		public bool EndProcess (PdfReader pdf) {
			// 用新的字体引用替代字体资源表的字体
			foreach (var map in _fontDictMap) {
				var d = map.Key;
				foreach (var item in map.Value) {
					d.Put (item.Key, item.Value);
				}
			}
			SubSetFontData (pdf);
			return false;
		}

		public int EstimateWorkload (PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		public bool Process (PageProcessorContext context) {
			Tracker.IncrementProgress (1);
			var fonts = context.Page.Locate<PdfDictionary> (PdfName.RESOURCES, PdfName.FONT);
			if (fonts == null) {
				return false;
			}
			//bool hasAnsiCjkFont = DetectLegacyCjkFont (context, fonts);
			//if (hasAnsiCjkFont == false) {
			//    return false;
			//}
			_currentFont = null;
			_currentNewFont = null;
			_fontNameIDMap.Clear ();
			_fontMap.Clear ();
			LoadFonts (context, fonts);
			if (_fontMap.Count == 0) {
				return false;
			}
			if (ProcessCommands (context.PageCommands.Commands)) {
				context.IsPageContentModified = true;
				return true;
			}
			return false;
		}

		#endregion

		bool ProcessCommands (IList<PdfPageCommand> parent) {
			var r = false;
			EnclosingCommand ec;
			var l = parent.Count;
			for (int i = 0; i < l; i++) {
				ec = parent[i] as EnclosingCommand;
				if (ec == null) {
					continue;
				}
				var n = ec.Name.ToString ();
				if (n == "BT") {
					foreach (var item in ec.Commands) {
						if (item.Type == PdfPageCommandType.Enclosure) {
							foreach (var sc in ((item as EnclosingCommand).Commands)) {
								ProcessTextCommand (sc);
							}
						}
						else {
							if (ProcessTextCommand (item) == false) {
								continue;
							}
						}
					}
					//if (_usedStyle) {
					//	_usedStyle = false;
					//}
					r = true;
				}
				else if (n == "BDC") {
					r |= ProcessCommands (ec.Commands);
					//if ((ec.Operands[0] as PdfName)?.ToString() == "/StyleSpan" && (ec.Commands[0] as EnclosingCommand).Commands[1] is MatrixCommand m && (m.Operands[0] as PdfNumber).FloatValue == 6.041f) {
					//	_usedStyle = true;
					//}
				}
				else {
					var cnf = _currentNewFont;
					var cf = _currentFont;
					r |= ProcessCommands (ec.Commands);
					_currentNewFont = cnf;
					_currentFont = cf;
				}
			}
			return r;
		}

		bool ProcessTextCommand (PdfPageCommand item) {
			var cn = item.Name.ToString ();
			if (cn == "Tf") {
				var cf = item.Operands[0] as PdfName;
				if (_fontMap.TryGetValue (cf, out _currentNewFont) == false) {
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
				RewriteTextOut (_currentNewFont, _currentFont, item);
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

		void RewriteTextOut (NewFont ef, FontInfo fontInfo, PdfPageCommand cmd) {
			var ops = cmd.Operands;
			var f = ef.Font.BaseFont as TrueTypeFontUnicode;
			if (ops.Length == 0) {
				return;
			}
			var op = ops[0];
			string t;
			if (op.Type == PdfObject.STRING) {
				t = fontInfo.DecodeText (op as PdfString);//.TrimEnd ();
				if (_trimTrailingWhiteSpace) {
					t = t.TrimEnd();
				}
				ops[0] = RewriteText (ef, f, t);
				//AddCustomDefaultWidth (ef, fontInfo, t);
			}
			else if (op.Type == PdfObject.ARRAY) {
				var a = op as PdfArray;
				var l = a.Size;
				for (int i = 0; i < l; i++) {
					op = a[i];
					if (op.Type == PdfObject.STRING) {
						t = fontInfo.DecodeText (op as PdfString);
						if (_trimTrailingWhiteSpace/* && i == l - 1*/) {
							t = t.TrimEnd();
						}
						a[i] = RewriteText (ef, f, t);
						//AddCustomDefaultWidth (ef, fontInfo, t);
					}
				}
			}
		}

		static void AddCustomDefaultWidth (NewFont newFont, FontInfo fontInfo, string text) {
			if (fontInfo.DefaultWidth == FontInfo.DefaultDefaultWidth) {
				return;
			}
			var dw = fontInfo.DefaultWidth;
			var w = newFont.GlyphWidths;
			//newFont.DefaultWidth = dw;
			foreach (var ch in text) {
				if (w.ContainsKey (ch) == false) {
					w.Add (ch, dw);
				}
			}
		}

		static PdfString RewriteText (NewFont newFont, TrueTypeFontUnicode ttf, string text) {
			var cs = newFont.CharSubstitutions.Count > 0;
			using (var bb = new ByteBuffer ()) {
				foreach (var ch in text) {
					if (cs == false || newFont.CharSubstitutions.TryGetValue (ch, out char c) == false) {
						c = ch;
					}
					if (newFont.UsedCidMap.TryGetValue (c, out int cid) == false) {
						//if (ttf == null) {
						//    ;
						//}
						var tt = ttf.GetMetricsTT (c);
						if (tt == null) {
							newFont.AbsentChars.Add (c);
							continue;
						}
						cid = tt[0];
						newFont.UsedCidMap[c] = cid;
						newFont.GlyphWidths[cid] = tt[1];
					}
					bb.Append ((byte)(cid >> 8));
					bb.Append ((byte)cid);
				}
				return new PdfString (bb.ToByteArray ());
			}
		}

		static bool DetectLegacyCjkFont (PdfDictionary font) {
			var en = font.GetAsName (PdfName.ENCODING);
			if (en == null
				|| (PdfName.WIN_ANSI_ENCODING.Equals (en) || __GbkEncoding.Equals (en) || __GbEncoding.Equals (en)) == false
				) {
				return false;
			}
			return PdfDocumentFont.HasEmbeddedFont (font) == false;
		}

		void LoadFonts (PageProcessorContext context, PdfDictionary fonts) {
			var r = new Dictionary<PdfName, PRIndirectReference> (fonts.Length); // 替代的字体
			foreach (var item in fonts) {
				string sn; // 替换字体名称
				string n; // 字体名称
				var fr = item.Value as PdfIndirectReference;
				if (fr == null
					|| _bypassFonts.Contains(fr.Number)) {
					continue;
				}
				if (_fontRefIDMap.TryGetValue (fr.Number, out NewFont nf) == false) {
					var f = fr.CastAs<PdfDictionary> ();
					if (f == null) {
						goto BYPASSFONT;
					}
					var fn = f.GetAsName (PdfName.BASEFONT);
					if (fn == null) {
						goto BYPASSFONT;
					}
					n = PdfDocumentFont.RemoveSubsetPrefix (PdfHelper.GetPdfNameString (fn)); // 字体名称
					var p = -1;
					FontSubstitution fs;
					if (_fontSubstitutions.TryGetValue (n, out fs)) {
						sn = fs.Substitution;
					}
					else {
						if (_embedLegacyFonts == false || DetectLegacyCjkFont (f) == false) {
							goto BYPASSFONT;
						}
						p = Array.IndexOf (__LegacyFonts, n.ToUpperInvariant ());
						if (p == -1) {
							goto BYPASSFONT;
						}
						sn = null;
					}
					if (_newFonts.TryGetValue (sn ?? n, out nf) == false) {
						try {
							Tracker.TraceMessage ("加载字体：" + (sn != null ? String.Concat (sn, "(替换 ", n, ")") : n));
							if (sn != null) {
								n = sn;
							}
							string sf = null;
							foreach (var font in Common.FontUtility.InstalledFonts) {
								if (font.DisplayName == n) {
									sf = font.OriginalName;
									break;
								}
							}
							nf = new NewFont {
								Font = _fontFactory.GetFont (sf ?? n, BaseFont.IDENTITY_H),
								FontRef = context.Pdf.AddPdfObject (new PdfDictionary ()),
								DescendantFontRef = context.Pdf.AddPdfObject (new PdfArray ())
							};
							if (fs?.OriginalCharacters != null && fs.SubstituteCharacters != null) {
								var sl = fs.SubstituteCharacters.Length;
								for (int i = 0; i < fs.OriginalCharacters.Length; i++) {
									if (i >= sl) {
										break;
									}
									nf.CharSubstitutions[fs.OriginalCharacters[i]] = fs.SubstituteCharacters[i];
								}
							}
							if (sn == null && p != -1 && nf.Font.BaseFont == null) {
								nf.Font = _fontFactory.GetFont (__AlternativeFonts[p], BaseFont.IDENTITY_H);
							}
							if (nf.Font.BaseFont == null) {
								throw new System.IO.FileNotFoundException ("无法加载字体：" + n);
							}
							_newFonts.Add (n, nf);
						}
						catch (Exception ex) {
							Tracker.TraceMessage (Tracker.Category.Error, "无法加载字体");
							throw;
						}
					}
					r[item.Key] = nf.FontRef;
					if (_fontInfoMap.ContainsKey (fr.Number) == false) {
						var fi = new FontInfo (f, fr.Number);
						_fontInfoMap.Add (fr.Number, fi);
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
				_bypassFonts.Add (fr.Number);
			}
			if (r.Count > 0) {
				_fontDictMap[fonts] = r;
			}
		}

		static void ReadSingleByteFontWidths (PdfDictionary font, FontInfo fontInfo, NewFont newfont) {
			var wl = font.GetAsArray (PdfName.WIDTHS);
			if (wl == null) {
				return;
			}
			var fc = font.TryGetInt32(PdfName.FIRSTCHAR, 0);
			var widths = newfont.GlyphWidths;
			foreach (PdfNumber item in wl) {
				if (item == null) {
					continue;
				}
				var s = fontInfo.Decode (new byte[] { (byte)fc }, 0, 1);
				if (s.Length == 0) {
					continue;
				}
				int w;
				if (widths.TryGetValue (s[0], out w) == false || w == 0) {
					widths[s[0]] = item.IntValue;
				}
				++fc;
			}
		}
		static void ReadCidFontWidths (PdfDictionary font, FontInfo fontInfo, NewFont newfont) {
			var w = font.GetAsArray (PdfName.W);
			if (w == null) {
				w = font.Locate<PdfArray> (PdfName.DESCENDANTFONTS, 0, PdfName.W);
				if (w == null) {
					return;
				}
			}
			var l = w.Size;
			PdfObject cw;
			int cid;
			var widths = newfont.GlyphWidths;
			for (int i = 0; i < l; i++) {
				cid = (w[i] as PdfNumber).IntValue;
				if (++i >= l) {
					break;
				}
				cw = w[i];
				if (cw.Type == PdfObject.ARRAY) {
					foreach (var width in cw as PdfArray) {
						var u = fontInfo.DecodeCidToUnicode (cid);
						if (u == 0 && cid != 0) {
							Console.WriteLine (cid.ToString () + "－无法解码CID");
							continue;
						}
						++cid;
						widths[u] = (width as PdfNumber).IntValue;
						Console.WriteLine (String.Join (" ", new string[] { cid.ToString (), ((char)u).ToString (), widths[u].ToString () }));
					}
				}
				else if (cw.Type == PdfObject.NUMBER) {
					var cid2 = (cw as PdfNumber).IntValue + 1;
					var width = (w[++i] as PdfNumber).IntValue;
					do {
						var u = fontInfo.DecodeCidToUnicode (cid);
						if (u == 0 && cid != 0) {
							Console.WriteLine (cid.ToString () + "－无法解码CID");
							continue;
						}
						widths[u] = width;
						Console.WriteLine (String.Join (" ", new string[] { cid.ToString (), ((char)u).ToString (), width.ToString () }));
					} while (++cid < cid2);
				}
			}
		}

		static void ChangeLegacyFontDictionary (PdfReader pdf, NewFont font) {
			var f = PdfReader.GetPdfObject (font.FontRef) as PdfDictionary;
			f.Put (PdfName.TYPE, PdfName.FONT);
			f.Put (PdfName.SUBTYPE, PdfName.TYPE0);
			f.Put (PdfName.BASEFONT, new PdfName (font.FontName));
			f.Put (PdfName.ENCODING, new PdfName (BaseFont.IDENTITY_H));
			f.Put (PdfName.DESCENDANTFONTS, font.DescendantFontRef);
			var metrics = new int[font.UsedCidMap.Count][];
			var i = -1;
			foreach (var m in font.UsedCidMap) {
				metrics[++i] = new int[] { m.Value, 0, m.Key };
			}
			var ttf = (font.Font.BaseFont as TrueTypeFontUnicode);
			Array.Sort (metrics, ttf);
			var u = pdf.AddPdfObject (ttf.GetToUnicode (metrics));
			f.Put (PdfName.TOUNICODE, u);
		}

		static void WriteCidWidths (NewFont font, PdfDictionary fontDictionary) {
			var l = font.GlyphWidths.Count;
			if (l == 0) {
				return;
			}
			var widths = new CharacterWidth[l];
			var i = -1;
			int width;
			foreach (var item in font.GlyphWidths) {
				if (item.Value == FontInfo.DefaultDefaultWidth) {
					continue;
				}
				widths[++i] = new CharacterWidth (item.Key, item.Value);
			}
			l = ++i;
			Array.Resize (ref widths, l);
			Array.Sort (widths, CharacterWidth.Compare);
			var w = new PdfArray ();
			int id, id2;
			CharacterWidth cw;
			for (i = 0; i < l; i++) {
				id = widths[i].ID;
				w.Add (new PdfNumber (id));
				width = widths[i].Width;
				var i2 = i;
				id2 = id;
				var wl = new PdfArray ();
				wl.Add (new PdfNumber (width));
				while (++i2 < l && (cw = widths[i2]).ID == ++id2 && cw.Width != width) {
					wl.Add (new PdfNumber (cw.Width));
				}
				if (wl.Size > 1) {
					w.Add (wl);
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
					w.Add (new PdfNumber (id2));
					w.Add (new PdfNumber (width));
					i = i2;
				}
				else {
					w.Add (wl);
				}
			}
			if (w.Size > 0) {
				fontDictionary.Put (PdfName.W, w);
			}
		}

		void SubSetFontData (PdfReader pdf) {
			foreach (var font in _newFonts) {
				var newFont = font.Value;
				Tracker.TraceMessage ("嵌入字体：" + newFont.Font.Familyname + "(" + newFont.UsedCidMap.Count + "字)");
				if (newFont.AbsentChars.Count > 0) {
					Tracker.TraceMessage (Tracker.Category.ImportantMessage, String.Concat ("丢失", newFont.AbsentChars.Count, "字：", new String (newFont.AbsentChars.ToArray ())));
				}
				ChangeLegacyFontDictionary (pdf, newFont);

				var ttf = newFont.Font.BaseFont as TrueTypeFontUnicode;
				var fa = PdfReader.GetPdfObject (newFont.DescendantFontRef) as PdfArray;
				var df = new PdfDictionary ();
				fa.Add (df);
				df.Put (PdfName.TYPE, PdfName.FONT);
				df.Put (PdfName.SUBTYPE, ttf.Cff ? PdfName.CIDFONTTYPE0 : PdfName.CIDFONTTYPE2);
				df.Put (PdfName.BASEFONT, new PdfName (newFont.FontName));
				df.Put (PdfName.CIDTOGIDMAP, PdfName.IDENTITY);
				df.Put (PdfName.DW, FontInfo.DefaultDefaultWidth);
				var fs = pdf.AddPdfObject (SubsetFont (newFont, ttf));
				var fd = ttf.GetFontDescriptor (fs, newFont.SubsetPrefix, null);
				df.Put (PdfName.FONTDESCRIPTOR, pdf.AddPdfObject (fd));
				WriteCidWidths (newFont, df);

				var csi = new PdfDictionary ();
				csi.Put (PdfName.REGISTRY, new PdfString ("Adobe"));
				csi.Put (PdfName.ORDERING, new PdfString ("Identity"));
				csi.Put (PdfName.SUPPLEMENT, new PdfNumber (0));
				df.Put (PdfName.CIDSYSTEMINFO, csi);

			}
		}

		static PdfStream SubsetFont (NewFont ef, TrueTypeFontUnicode ttf) {
			//ttf.AddSubsetRange (r);
			byte[] b;
			if (ttf.Cff) {
				int[] metricsTT;
				var d = new Dictionary<int, int[]> (ef.UsedCidMap.Count);
				foreach (var item in ef.UsedCidMap) {
					metricsTT = ttf.GetMetricsTT (item.Key);
					d.Add (item.Value, new int[] {metricsTT[0], metricsTT[1], item.Key});
				}
				//ttf.AddRangeUni (d, false, true);
				var f = new CFFFontSubset (new RandomAccessFileOrArray (ttf.ReadCffFont ()), d);
				b = f.Process (f.GetNames ()[0]);
			}
			else {
				var r = new int[ef.UsedCidMap.Count];
				ef.UsedCidMap.Values.CopyTo (r, 0);
				var ts = new TrueTypeFontSubSet (ttf.FileName, new RandomAccessFileOrArray (ttf.FileName), new System.util.collections.HashSet2<int> (r), ttf.DirectoryOffset, true, true);
				b = ts.Process ();
			}
			var s = new PdfStream (b);
			if (ttf.Cff) {
				s.Put (PdfName.SUBTYPE, new PdfName ("CIDFontType0C"));
			}
			s.FlateCompress ();
			s.Put (PdfName.LENGTH1, new PdfNumber (b.Length));
			return s;
		}

		[System.Diagnostics.DebuggerDisplay("{ID},{Width}")]
		struct CharacterWidth
		{
			public int ID, Width;

			public CharacterWidth (int id, int width) {
				ID = id;
				Width = width;
			}
			public static int Compare (CharacterWidth x, CharacterWidth y) {
				return x.ID.CompareTo (y.ID);
			}
		}

		[System.Diagnostics.DebuggerDisplay ("{FontName}")]
		sealed class NewFont
		{
			public Dictionary<int, PdfDictionary> FontDictionaries { get; set; }
			public PRIndirectReference FontRef { get; set; }
			public PdfIndirectReference DescendantFontRef { get; set; }
			/// <summary>
			/// 字体 Unicode 到宽度的映射表。
			/// </summary>
			public Dictionary<int, int> GlyphWidths { get; }
			/// <summary>
			/// 字体 Unicode 和 CID 的映射表。
			/// </summary>
			public Dictionary<int, int> UsedCidMap { get; }
			public string SubsetPrefix { get; private set; }
			public string FontName => SubsetPrefix + _Font.Familyname;
			public HashSet<char> AbsentChars { get; }
			public Dictionary<char, char> CharSubstitutions { get; }
			Font _Font;
			public Font Font {
				get => _Font;
				set {
					_Font = value;
					SubsetPrefix = BaseFont.CreateSubsetPrefix();
				}
			}

			public NewFont () {
				GlyphWidths = new Dictionary<int, int> ();
				FontDictionaries = new Dictionary<int, PdfDictionary> ();
				UsedCidMap = new Dictionary<int, int> ();
				AbsentChars = new HashSet<char> ();
				CharSubstitutions = new Dictionary<char, char> ();
			}
		}
	}
}
