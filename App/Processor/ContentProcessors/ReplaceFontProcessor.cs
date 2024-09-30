using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.fonts.cmaps;
using PDFPatcher.Common;
using PDFPatcher.Model;
using FontId = System.Tuple<string, bool>;

namespace PDFPatcher.Processor
{
	sealed class ReplaceFontProcessor : IPageProcessor
	{
		static readonly string[] __LegacyFonts = { "宋体", "楷体_GB2312", "仿宋_GB2312", "黑体", "STSONG-LIGHT-GB-EUC-H", "STSONG-LIGHT-GBK-EUC-H" };
		static readonly string[] __AlternativeFonts = { "宋体", "楷体", "仿宋", "微软雅黑", "宋体", "宋体" };
		static readonly PdfName __GbkEncoding = new PdfName("GBK-EUC-H");
		static readonly PdfName __GbEncoding = new PdfName("GB-EUC-H");
		static readonly string[] __BuiltInEncodings = { "78-EUC-H", "78-EUC-V", "78-H", "78ms-RKSJ-H", "78ms-RKSJ-V", "78-RKSJ-H", "78-RKSJ-V", "78-V", "83pv-RKSJ-H", "90msp-RKSJ-H", "90msp-RKSJ-V", "90ms-RKSJ-H", "90ms-RKSJ-V", "90pv-RKSJ-H", "90pv-RKSJ-V", "Add-H", "Add-RKSJ-H", "Add-RKSJ-V", "Add-V", "Adobe-CNS1-0", "Adobe-CNS1-1", "Adobe-CNS1-2", "Adobe-CNS1-3", "Adobe-CNS1-4", "Adobe-CNS1-5", "Adobe-CNS1-6", "Adobe-GB1-0", "Adobe-GB1-1", "Adobe-GB1-2", "Adobe-GB1-3", "Adobe-GB1-4", "Adobe-GB1-5", "Adobe-Japan1-0", "Adobe-Japan1-1", "Adobe-Japan1-2", "Adobe-Japan1-3", "Adobe-Japan1-4", "Adobe-Japan1-5", "Adobe-Japan1-6", "Adobe-Korea1-0", "Adobe-Korea1-1", "Adobe-Korea1-2", "B5-H", "B5pc-H", "B5pc-V", "B5-V", "CNS1-H", "CNS1-V", "CNS2-H", "CNS2-V", "CNS-EUC-H", "CNS-EUC-V", "ETen-B5-H", "ETen-B5-V", "ETenms-B5-H", "ETenms-B5-V", "ETHK-B5-H", "ETHK-B5-V", "EUC-H", "EUC-V", "Ext-H", "Ext-RKSJ-H", "Ext-RKSJ-V", "Ext-V", "GB-EUC-H", "GB-EUC-V", "GB-H", "GBK2K-H", "GBK2K-V", "GBK-EUC-H", "GBK-EUC-V", "GBKp-EUC-H", "GBKp-EUC-V", "GBpc-EUC-H", "GBpc-EUC-V", "GBT-EUC-H", "GBT-EUC-V", "GBT-H", "GBTpc-EUC-H", "GBTpc-EUC-V", "GBT-V", "GB-V", "H", "Hankaku", "Hiragana", "HKdla-B5-H", "HKdla-B5-V", "HKdlb-B5-H", "HKdlb-B5-V", "HKgccs-B5-H", "HKgccs-B5-V", "HKm314-B5-H", "HKm314-B5-V", "HKm471-B5-H", "HKm471-B5-V", "HKscs-B5-H", "HKscs-B5-V", "Katakana", "KSC-EUC-H", "KSC-EUC-V", "KSC-H", "KSC-Johab-H", "KSC-Johab-V", "KSCms-UHC-H", "KSCms-UHC-HW-H", "KSCms-UHC-HW-V", "KSCms-UHC-V", "KSCpc-EUC-H", "KSCpc-EUC-V", "KSC-V", "NWP-H", "NWP-V", "RKSJ-H", "RKSJ-V", "Roman", "UniCNS-UCS2-H", "UniCNS-UCS2-V", "UniCNS-UTF16-H", "UniCNS-UTF16-V", "UniCNS-UTF32-H", "UniCNS-UTF32-V", "UniCNS-UTF8-H", "UniCNS-UTF8-V", "UniGB-UCS2-H", "UniGB-UCS2-V", "UniGB-UTF16-H", "UniGB-UTF16-V", "UniGB-UTF32-H", "UniGB-UTF32-V", "UniGB-UTF8-H", "UniGB-UTF8-V", "UniJIS2004-UTF16-H", "UniJIS2004-UTF16-V", "UniJIS2004-UTF32-H", "UniJIS2004-UTF32-V", "UniJIS2004-UTF8-H", "UniJIS2004-UTF8-V", "UniJISPro-UCS2-HW-V", "UniJISPro-UCS2-V", "UniJISPro-UTF8-V", "UniJIS-UCS2-H", "UniJIS-UCS2-HW-H", "UniJIS-UCS2-HW-V", "UniJIS-UCS2-V", "UniJIS-UTF16-H", "UniJIS-UTF16-V", "UniJIS-UTF32-H", "UniJIS-UTF32-V", "UniJIS-UTF8-H", "UniJIS-UTF8-V", "UniJISX02132004-UTF32-H", "UniJISX02132004-UTF32-V", "UniJISX0213-UTF32-H", "UniJISX0213-UTF32-V", "UniKS-UCS2-H", "UniKS-UCS2-V", "UniKS-UTF16-H", "UniKS-UTF16-V", "UniKS-UTF32-H", "UniKS-UTF32-V", "UniKS-UTF8-H", "UniKS-UTF8-V", "V", "WP-Symbol" };
		static readonly string[] __GbEucEncodings = { "GBK-EUC-H", "GBK-EUC-V", "GBT-EUC-H", "GB-EUC-H", "GB-EUC-V" };
		const string HalfWidthLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		const string FullWidthLetters = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ";
		const string HalfWidthNumbers = "0123456789";
		const string FullWidthNumbers = "０１２３４５６７８９";
		const string HorizontalPunctuations = "，、。：；！？〖〗…‥—–（）｛｝〔〕【】《》〈〉「」『』［］";
		const string VerticalPunctuations = "︐︑︒︓︔︕︖︗︘︙︰︱︲︵︶︷︸︹︺︻︼︽︾︿﹀﹁﹂﹃﹄﹇﹈";

		readonly bool _embedLegacyFonts;
		readonly bool _trimTrailingWhiteSpace;
		NewFont _currentNewFont;
		FontInfo _currentFont;
		FontFactoryImp _fontFactory;
		Dictionary<FontId, NewFont> _newFonts;
		Dictionary<PdfName, NewFont> _fontMap;
		Dictionary<PdfName, int> _fontNameIDMap;
		Dictionary<int, FontInfo> _fontInfoMap;
		Dictionary<string, FontSubstitution> _fontSubstitutions;
		Dictionary<int, NewFont> _fontRefIDMap;
		Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>> _fontDictMap;
		HashSet<int> _bypassFonts, _processedForms;

		public ReplaceFontProcessor(bool embedLegacyFonts, bool trimTrailingWhiteSpace, Dictionary<string, FontSubstitution> fontSubstitutions) {
			_embedLegacyFonts = embedLegacyFonts;
			_trimTrailingWhiteSpace = trimTrailingWhiteSpace;
			_fontSubstitutions = fontSubstitutions;
		}
		#region IPageProcessor 成员
		public string Name => "嵌入汉字库";
		public void BeginProcess(DocProcessorContext context) {
			if (_fontSubstitutions == null) {
				_fontSubstitutions = new Dictionary<string, FontSubstitution>(0);
			}
			var l = __LegacyFonts.Length + _fontSubstitutions.Count;
			_newFonts = new Dictionary<FontId, NewFont>(l);
			_fontMap = new Dictionary<PdfName, NewFont>(l);
			_fontNameIDMap = new Dictionary<PdfName, int>();
			_fontInfoMap = new Dictionary<int, FontInfo>();
			_fontFactory = new FontFactoryImp();
			_fontRefIDMap = new Dictionary<int, NewFont>();
			_fontDictMap = new Dictionary<PdfDictionary, Dictionary<PdfName, PRIndirectReference>>();
			_bypassFonts = new HashSet<int>();
			_processedForms = new HashSet<int>();
			foreach (var item in FontHelper.GetInstalledFonts(true)) {
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
			foreach (var map in _fontDictMap) {
				var d = map.Key;
				foreach (var item in map.Value) {
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
			_currentFont = null;
			_currentNewFont = null;
			_fontNameIDMap.Clear();
			_fontMap.Clear();
			bool modified = false;
			if (ProcessPageContents(context)) {
				context.IsPageContentModified = modified = true;
			}
			ProcessResourceContents(context, context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT), ref modified);
			var annots = context.Page.GetAsArray(PdfName.ANNOTS);
			if (annots != null && annots.Size != 0) {
				foreach (var item in annots) {
					if (PdfReader.GetPdfObjectRelease(item) is PdfDictionary annot) {
						ProcessResourceContents(context, annot, ref modified);
					}
				}
			}
			return modified;
		}

		bool ProcessPageContents(PageProcessorContext context) {
			var fonts = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.FONT);
			if (fonts == null) {
				return false;
			}
			LoadFonts(context, fonts);
			if (_fontMap.Count == 0) {
				return false;
			}
			return ProcessCommands(context.PageCommands.Commands);
		}

		void ProcessResourceContents(PageProcessorContext context, PdfDictionary container, ref bool modified) {
			if (container == null) {
				return;
			}
			foreach (var item in GetForms(container)) {
				if (_processedForms.Add(item.Ref.Number) == false
					|| !(item.Resource is PRStream s)) {
					continue;
				}
				var resources = item.Resource.GetAsDict(PdfName.RESOURCES);
				if (resources == null || resources.Size == 0) {
					continue;
				}
				var fonts = resources.GetAsDict(PdfName.FONT);
				if (fonts == null || fonts.Size == 0) {
					continue;
				}
				LoadFonts(context, fonts);
				var cp = new PdfPageCommandProcessor();
				cp.ProcessContent(PdfReader.GetStreamBytes(s), resources);
				if (ProcessCommands(cp.Commands)) {
					using (var ms = new MemoryStream()) {
						cp.WritePdfCommands(ms);
						ms.Flush();
						s.SetData(ms.ToArray(), ms.Length > 100, 9);
					}
					modified = true;
				}
			}
		}

		#endregion

		bool ProcessCommands(IList<PdfPageCommand> parent) {
			var r = false;
			EnclosingCommand ec;
			var l = parent.Count;
			for (int i = 0; i < l; i++) {
				ec = parent[i] as EnclosingCommand;
				if (ec == null) {
					continue;
				}
				var n = ec.Name.ToString();
				if (n == "BT") {
					foreach (var item in ec.Commands) {
						if (item.Type == PdfPageCommandType.Enclosure) {
							foreach (var sc in ((item as EnclosingCommand).Commands)) {
								ProcessTextCommand(sc);
							}
						}
						else if (ProcessTextCommand(item) == false) {
							continue;
						}
					}
					r = true;
				}
				else if (n == "BDC") {
					r |= ProcessCommands(ec.Commands);
				}
				else {
					var cnf = _currentNewFont;
					var cf = _currentFont;
					r |= ProcessCommands(ec.Commands);
					_currentNewFont = cnf;
					_currentFont = cf;
				}
			}
			return r;
		}

		bool ProcessTextCommand(PdfPageCommand item) {
			var cn = item.Name.ToString();
			if (cn == "Tf") {
				var cf = item.Operands[0] as PdfName;
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
			return true;
		}

		void RewriteTextOut(NewFont ef, FontInfo fontInfo, PdfPageCommand cmd) {
			var ops = cmd.Operands;
			var f = ef.Font.BaseFont as TrueTypeFontUnicode;
			if (ops.Length == 0) {
				return;
			}
			var op = ops[0];
			string t;
			if (op.Type == PdfObject.STRING) {
				t = fontInfo.DecodeText(op as PdfString);
				if (_trimTrailingWhiteSpace) {
					t = t.TrimEnd();
				}
				ops[0] = RewriteText(ef, f, t);
			}
			else if (op.Type == PdfObject.ARRAY) {
				var a = op as PdfArray;
				var l = a.Size;
				for (int i = 0; i < l; i++) {
					op = a[i];
					if (op.Type == PdfObject.STRING) {
						t = fontInfo.DecodeText(op as PdfString);
						if (_trimTrailingWhiteSpace/* && i == l - 1*/) {
							t = t.TrimEnd();
						}
						a[i] = RewriteText(ef, f, t);
					}
				}
			}
		}

		static PdfString RewriteText(NewFont newFont, TrueTypeFontUnicode ttf, string text) {
			var cs = newFont.CharSubstitutions.Count > 0;
			var widths = newFont.GlyphWidths;
			var unicodeMap = newFont.UnicodeCidMap;
			using (var bb = new ByteBuffer()) {
				foreach (var ch in text) {
					if (cs == false || newFont.CharSubstitutions.TryGetValue(ch, out char c) == false) {
						c = ch;
					}
					if (unicodeMap.TryGetValue(c, out int cid) == false) {
						var tt = ttf.GetMetricsTT(c);
						if (tt == null) {
							newFont.AbsentChars.Add(c);
							continue;
						}
						cid = tt[0];
						unicodeMap[c] = cid;
						if (widths.ContainsKey(cid) == false) {
							widths[cid] = tt[1];
						}
					}
					bb.Append((byte)(cid >> 8));
					bb.Append((byte)cid);
				}
				return new PdfString(bb.ToByteArray());
			}
		}

		static bool DetectLegacyCjkFont(PdfDictionary font) {
			var en = font.GetAsName(PdfName.ENCODING);
			if (en == null
				|| (PdfName.WIN_ANSI_ENCODING.Equals(en) || __GbkEncoding.Equals(en) || __GbEncoding.Equals(en)) == false
				) {
				return false;
			}
			return PdfDocumentFont.HasEmbeddedFont(font) == false;
		}

		static IEnumerable<ResourceReference> GetForms(PdfDictionary container) {
			var visitedRefs = new HashSet<PdfIndirectReference>();
			return PdfModelHelper.GetReferencedResources(container, o => PdfName.FORM.Equals(o.GetAsName(PdfName.SUBTYPE)), visitedRefs);
		}

		void LoadFonts(PageProcessorContext context, PdfDictionary fonts) {
			var r = new Dictionary<PdfName, PRIndirectReference>(fonts.Length); // 替代的字体
			foreach (var item in fonts) {
				string sn; // 替换字体名称
				string n; // 字体名称
				bool v; // 是否竖排文字
				var fr = item.Value as PdfIndirectReference;
				if (fr == null
					|| _bypassFonts.Contains(fr.Number)) {
					continue;
				}
				if (_fontRefIDMap.TryGetValue(fr.Number, out NewFont nf) == false) {
					var f = fr.CastAs<PdfDictionary>();
					if (f == null) {
						goto BYPASSFONT;
					}
					var fn = f.GetAsName(PdfName.BASEFONT);
					if (fn == null) {
						goto BYPASSFONT;
					}
					n = PdfDocumentFont.RemoveSubsetPrefix(PdfHelper.GetPdfNameString(fn)); // 字体名称
					var p = -1;
					FontSubstitution fs;
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
					v = f.GetAsName(PdfName.ENCODING)?.ToString().EndsWith("-V") ?? false;
					if (_newFonts.TryGetValue(new FontId(sn ?? n, v), out nf) == false) {
						try {
							Tracker.TraceMessage($"加载字体：{(v ? "@" : String.Empty)}{(sn != null ? $"{sn}(替换 {n})" : n)}");
							if (sn != null) {
								n = sn;
							}
							string sf = null;
							foreach (var font in FontUtility.InstalledFonts) {
								if (font.DisplayName == n) {
									sf = font.OriginalName;
									break;
								}
							}
							nf = new NewFont {
								Font = _fontFactory.GetFont(sf ?? n, v ? BaseFont.IDENTITY_V : BaseFont.IDENTITY_H),
								FontRef = context.Pdf.AddPdfObject(new PdfDictionary()),
								DescendantFontRef = context.Pdf.AddPdfObject(new PdfArray()),
								Vertical = v,
							};
							var fd = f.Locate<PdfDictionary>(PdfName.DESCENDANTFONTS, 0, PdfName.FONTDESCRIPTOR);
							if (fd != null) {
								var num = fd.GetAsNumber(PdfName.ITALICANGLE)?.DoubleValue ?? 0d;
								if (num != 0) {
									nf.ItalicAngle = num;
								}
							}
							if (fs != null) {
								SetupFontSubstitutionMaps(nf, fs);
							}
							if (sn == null && p != -1 && nf.Font.BaseFont == null) {
								nf.Font = _fontFactory.GetFont(__AlternativeFonts[p], v ? BaseFont.IDENTITY_V : BaseFont.IDENTITY_H);
							}
							if (nf.Font.BaseFont == null) {
								throw new FileNotFoundException("无法加载字体：" + n);
							}
							_newFonts.Add(new FontId(n, v), nf);
						}
						catch (Exception) {
							Tracker.TraceMessage(Tracker.Category.Error, "无法加载字体");
							throw;
						}
					}
					r[item.Key] = nf.FontRef;
					if (_fontInfoMap.ContainsKey(fr.Number) == false) {
						var fi = new FontInfo(f, fr.Number);
						_fontInfoMap.Add(fr.Number, fi);
						try {
							ReadSingleByteFontWidths(f, fi, nf);
							ReadCidFontWidths(f, fi, nf);
						}
						catch (NullReferenceException) {
							Tracker.TraceMessage(Tracker.Category.ImportantMessage, $"字体“{n}”的 CID 宽度表错误。");
						}
					}
					_fontRefIDMap[nf.FontRef.Number] = nf;
				}
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

		static void SetupFontSubstitutionMaps(NewFont nf, FontSubstitution fs) {
			if (fs.TraditionalChineseConversion != 0) {
				if (fs.TraditionalChineseConversion > 0) {
					Map(nf.CharSubstitutions, Constants.Chinese.Simplified, Constants.Chinese.Traditional);
				}
				else {
					Map(nf.CharSubstitutions, Constants.Chinese.Traditional, Constants.Chinese.Simplified);
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
					Map(nf.CharSubstitutions, SetCaseProcessor.HalfWidthPunctuations, SetCaseProcessor.FullWidthPunctuations);
				}
				else {
					Map(nf.CharSubstitutions, SetCaseProcessor.FullWidthPunctuations, SetCaseProcessor.HalfWidthPunctuations);
				}
			}
			if (fs.OriginalCharacters != null && fs.SubstituteCharacters != null) {
				var sl = fs.SubstituteCharacters.Length;
				for (int i = 0; i < fs.OriginalCharacters.Length; i++) {
					if (i >= sl) {
						break;
					}
					nf.CharSubstitutions[fs.OriginalCharacters[i]] = fs.SubstituteCharacters[i];
				}
			}
			#region HACK: 将 iText 转码的横向标点转回竖向标点
			if (nf.Vertical) {
				Map(nf.CharSubstitutions, HorizontalPunctuations, VerticalPunctuations);
			}
			#endregion
		}

		static void Map(Dictionary<char, char> map, string from, string to) {
			var i = 0;
			foreach (var item in from) {
				map[item] = to[i++];
			}
		}

		static void ReadSingleByteFontWidths(PdfDictionary font, FontInfo fontInfo, NewFont newFont) {
			var wl = font.GetAsArray(PdfName.WIDTHS);
			if (wl == null) {
				return;
			}
			var fc = font.TryGetInt32(PdfName.FIRSTCHAR, 0);
			var widths = newFont.GlyphWidths;
			foreach (PdfNumber item in wl) {
				if (item == null) {
					continue;
				}
				var s = fontInfo.Decode(new byte[] { (byte)fc }, 0, 1);
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
		static void ReadCidFontWidths(PdfDictionary font, FontInfo fontInfo, NewFont newfont) {
			var w = newfont.Vertical ? PdfName.W2 : PdfName.W;
			var fw = font.GetAsArray(w);
			if (fw == null) {
				fw = font.Locate<PdfArray>(PdfName.DESCENDANTFONTS, 0, w);
				if (fw == null) {
					return;
				}
			}
			var encoding = font.GetAsName(PdfName.ENCODING)?.ToString().Substring(1);
			CMapCidUni cMap = encoding != null && Array.BinarySearch(__BuiltInEncodings, encoding) >= 0
				? CMapCache.GetCachedCMapCidUni(encoding)
				: null;
			if (Array.IndexOf(__GbEucEncodings, encoding) != -1) {
				cMap = new GbkCidUni(cMap);
			}
			var l = fw.Size;
			PdfObject cw;
			int cid;
			var widths = newfont.GlyphWidths;
			for (int i = 0; i < l; i++) {
				cid = (fw[i] as PdfNumber).IntValue;
				if (++i >= l) {
					break;
				}
				cw = fw[i];
				if (cw.Type == PdfObject.ARRAY) {
					foreach (var width in cw as PdfArray) {
						var u = fontInfo.DecodeCidToUnicode(cMap, cid);
						if (u == 0 && cid != 0) {
							Debug.WriteLine(cid.ToText() + "－无法解码CID");
							continue;
						}
						widths[u] = (width as PdfNumber).IntValue;
						Debug.WriteLine(String.Join(" ", new string[] { cid.ToText(), u.ToText("X"), ((char)u).ToString(), widths[u].ToText() }));
						++cid;
					}
				}
				else if (cw.Type == PdfObject.NUMBER) {
					var cid2 = (cw as PdfNumber).IntValue + 1;
					var width = (fw[++i] as PdfNumber).IntValue;
					do {
						var u = fontInfo.DecodeCidToUnicode(cMap, cid);
						if (u == 0 && cid != 0) {
							Debug.WriteLine(cid.ToText() + "－无法解码CID");
							continue;
						}
						widths[u] = width;
						Debug.WriteLine(String.Join(" ", new string[] { cid.ToText(), u.ToText("X"), ((char)u).ToString(), width.ToText() }));
					} while (++cid < cid2);
				}
			}
		}

		static void ChangeLegacyFontDictionary(PdfReader pdf, NewFont font) {
			var f = PdfReader.GetPdfObject(font.FontRef) as PdfDictionary;
			f.Put(PdfName.TYPE, PdfName.FONT);
			f.Put(PdfName.SUBTYPE, PdfName.TYPE0);
			f.Put(PdfName.BASEFONT, new PdfName(font.FontName));
			f.Put(PdfName.ENCODING, new PdfName(font.Vertical ? BaseFont.IDENTITY_V : BaseFont.IDENTITY_H));
			f.Put(PdfName.DESCENDANTFONTS, font.DescendantFontRef);
			var metrics = new int[font.UnicodeCidMap.Count][];
			var i = -1;
			foreach (var m in font.UnicodeCidMap) {
				metrics[++i] = new int[] { m.Value, 0, m.Key };
			}
			var ttf = font.Font.BaseFont as TrueTypeFontUnicode;
			Array.Sort(metrics, ttf);
			var u = pdf.AddPdfObject(ttf.GetToUnicode(metrics));
			f.Put(PdfName.TOUNICODE, u);
		}

		static void WriteCidWidths(NewFont font, PdfDictionary fontDictionary) {
			var l = font.GlyphWidths.Count;
			if (l == 0) {
				return;
			}
			var widths = new CharacterWidth[l];
			var i = -1;
			int width;
			foreach (var item in font.GlyphWidths) {
				if (item.Value == FontInfo.DefaultDefaultWidth
					|| font.UnicodeCidMap.TryGetValue(item.Key, out var cid) == false) {
					continue;
				}
				widths[++i] = new CharacterWidth(cid, item.Value);
			}
			l = ++i;
			Array.Resize(ref widths, l);
			Array.Sort(widths, CharacterWidth.Compare);
			var w = new PdfArray();
			int id, id2;
			CharacterWidth cw;
			for (i = 0; i < l; i++) {
				id = widths[i].ID;
				w.Add(new PdfNumber(id));
				width = widths[i].Width;
				var i2 = i;
				id2 = id;
				var wl = new PdfArray {
					new PdfNumber(width)
				};
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
				fontDictionary.Put(font.Vertical ? PdfName.W2 : PdfName.W, w);
			}
		}

		void SubSetFontData(PdfReader pdf) {
			foreach (var font in _newFonts) {
				var newFont = font.Value;
				Tracker.TraceMessage($"嵌入字体：{newFont.Font.Familyname}({newFont.UnicodeCidMap.Count}字)");
				if (newFont.AbsentChars.Count > 0) {
					Tracker.TraceMessage(Tracker.Category.ImportantMessage, $"丢失{newFont.AbsentChars.Count}字：{String.Concat(newFont.AbsentChars)}");
				}
				ChangeLegacyFontDictionary(pdf, newFont);

				var ttf = newFont.Font.BaseFont as TrueTypeFontUnicode;
				var fa = PdfReader.GetPdfObject(newFont.DescendantFontRef) as PdfArray;
				var df = new PdfDictionary();
				fa.Add(df);
				df.Put(PdfName.TYPE, PdfName.FONT);
				df.Put(PdfName.SUBTYPE, ttf.Cff ? PdfName.CIDFONTTYPE0 : PdfName.CIDFONTTYPE2);
				df.Put(PdfName.BASEFONT, new PdfName(newFont.FontName));
				WriteCidWidths(newFont, df);
				var fs = pdf.AddPdfObject(SubsetFont(newFont, ttf));
				var fd = ttf.GetFontDescriptor(fs, newFont.SubsetPrefix, null);
				if (newFont.ItalicAngle != 0) {
					fd.Put(PdfName.ITALICANGLE, newFont.ItalicAngle);
					fd.Put(PdfName.FLAGS, 1 << 6);
				}
				df.Put(PdfName.FONTDESCRIPTOR, pdf.AddPdfObject(fd));

				var csi = new PdfDictionary();
				csi.Put(PdfName.REGISTRY, new PdfString("Adobe"));
				csi.Put(PdfName.ORDERING, new PdfString("Identity"));
				csi.Put(PdfName.SUPPLEMENT, new PdfNumber(0));
				df.Put(PdfName.CIDSYSTEMINFO, csi);
			}
		}

		static PdfStream SubsetFont(NewFont ef, TrueTypeFontUnicode ttf) {
			byte[] b;
			if (ttf.Cff) {
				int[] metricsTT;
				var d = new Dictionary<int, int[]>(ef.UnicodeCidMap.Count);
				foreach (var item in ef.UnicodeCidMap) {
					metricsTT = ttf.GetMetricsTT(item.Key);
					d.Add(item.Value, new int[] { metricsTT[0], metricsTT[1], item.Key });
				}
				var f = new CFFFontSubset(new RandomAccessFileOrArray(ttf.ReadCffFont()), d);
				b = f.Process(f.GetNames()[0]);
			}
			else {
				var r = new int[ef.UnicodeCidMap.Count];
				ef.UnicodeCidMap.Values.CopyTo(r, 0);
				b = new TrueTypeFontSubSet(ttf.FileName, new RandomAccessFileOrArray(ttf.FileName), new System.util.collections.HashSet2<int>(r), ttf.DirectoryOffset, true, true).Process();
			}
			var s = new PdfStream(b);
			if (ttf.Cff) {
				s.Put(PdfName.SUBTYPE, new PdfName("CIDFontType0C"));
			}
			s.FlateCompress();
			s.Put(PdfName.LENGTH1, new PdfNumber(b.Length));
			return s;
		}

		[DebuggerDisplay("{ID},{Width}")]
		struct CharacterWidth
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
			public Dictionary<int, int> UnicodeCidMap { get; }
			public string SubsetPrefix { get; private set; }
			public string FontName => SubsetPrefix + _Font.Familyname;
			public HashSet<char> AbsentChars { get; }
			public Dictionary<char, char> CharSubstitutions { get; }
			public bool Vertical { get; set; }
			public double ItalicAngle { get; set; }
			Font _Font;
			public Font Font {
				get => _Font;
				set {
					_Font = value;
					SubsetPrefix = BaseFont.CreateSubsetPrefix();
				}
			}

			public NewFont() {
				GlyphWidths = new Dictionary<int, int>();
				FontDictionaries = new Dictionary<int, PdfDictionary>();
				UnicodeCidMap = new Dictionary<int, int>();
				AbsentChars = new HashSet<char>();
				CharSubstitutions = new Dictionary<char, char>();
			}
		}

		sealed class GbkCidUni : CMapCidUni
		{
			readonly CMapCidUni _BaseCMap;

			public GbkCidUni(CMapCidUni baseCMap) {
				_BaseCMap = baseCMap;
			}
			public override int Lookup(int character) {
				return character > 813 && character < 908 ? character - (814 - 0x21)
					: character == 7716 ? ' '
					: _BaseCMap.Lookup(character);
			}
		}
	}
}
