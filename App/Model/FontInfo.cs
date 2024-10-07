using System;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model
{
	sealed class FontInfo : CMapAwareDocumentFont
	{
		[Flags]
		internal enum CjkFontType
		{
			Unknown,
			CJK = 0x01,
			Chinese = 0x02 + CJK,
			Gb18030Chinese = 0x0100 + Chinese,
			Big5Chinese = 0x0200 + Chinese,
			Japanese = 0x04 + CJK,
			Korean = 0x08 + CJK,
			Unicode = 0x4000,
			None = 0x8000
		}

		readonly static Encoding __GbkEncoding = System.Text.Encoding.GetEncoding("gbk");
		readonly static PdfName[] __GbkEncodingNames = [new PdfName("GBK-EUC-H"), new PdfName("GBK-EUC-V"), new PdfName("GB-EUC-H"), new PdfName("GB-EUC-V"), PdfName.WIN_ANSI_ENCODING];
		readonly static string[] __gbkFontNames = ["宋体", "黑体", "楷体_GB2312", "仿宋体", "仿宋_GB2312", "隶书", "幼圆"];
		readonly static PdfName[] __IdentityEncodingNames = [new PdfName("Identity-H"), new PdfName("Identity-V")];
		public const int DefaultDefaultWidth = 1000;

		readonly PdfDictionary _Font;
		PdfDictionary _FontDescriptor;
		internal PdfDictionary FontDescriptor {
			get {
				if (_FontDescriptor == null) {
					_FontDescriptor = _Font.Locate<PdfArray>(PdfName.DESCENDANTFONTS).Locate<PdfDictionary>(0).Locate<PdfDictionary>(PdfName.FONTDESCRIPTOR);
					_FontDescriptor ??= new PdfDictionary();
				}
				return _FontDescriptor;
			}
		}
		string _FontName;
		internal string FontName {
			get {
				if (_FontName == null) {
					var f = FontDescriptor.GetAsName(PdfName.FONTNAME);
					if (f != null) {
						_FontName = PdfName.DecodeName(f.ToString());
					}
					else {
						var fn = PostscriptFontName;
						var i = fn.LastIndexOf(',');
						if (i != -1) {
							fn = fn.Substring(0, i);
						}
						_FontName = fn;
					}
					// 删除子集的名称
					_FontName = PdfDocumentFont.RemoveSubsetPrefix(_FontName);
				}
				return _FontName;
			}
		}

		CjkFontType _CjkFontType = CjkFontType.Unknown;
		internal CjkFontType CjkType {
			get {
				if (_CjkFontType == CjkFontType.Unknown) {
					InitCjkFontType();
				}
				return _CjkFontType;
			}
		}

		int _DefaultWidth = -1;
		public int DefaultWidth {
			get {
				if (_DefaultWidth == -1) {
					var w = _Font.Locate<PdfNumber>(PdfName.DESCENDANTFONTS, 0, PdfName.DW);
					_DefaultWidth = w == null ? DefaultDefaultWidth : w.IntValue;
				}
				return _DefaultWidth;
			}
		}

		private void InitCjkFontType() {
			if (_Font.Contains(PdfName.TOUNICODE)) {
				_CjkFontType = CjkFontType.None;
				return;
			}
			var encoding = _Font.GetAsName(PdfName.ENCODING);
			var fn = FontName.ToUpperInvariant();
			var c = __gbkFontNames.Contains(fn) || __GbkEncodingNames.Contains(encoding);
			_CjkFontType = c ? CjkFontType.Chinese : CjkFontType.None;
			if (_CjkFontType != CjkFontType.None) {
				return;
			}
			c = __IdentityEncodingNames.Contains(encoding);
			if (c) {
				_CjkFontType = CjkFontType.Unicode;
			}
		}

		readonly int _FontID = -1;
		internal int FontID => _FontID;

		public FontInfo(PdfDictionary font, int refNumber)
			: base(font) {
			_Font = font;
			_FontID = refNumber;
		}
		public FontInfo(PRIndirectReference refFont) : base(refFont) {
			_Font = (PdfDictionary)PdfReader.GetPdfObjectRelease(refFont);
			_FontID = refFont.Number;
		}

		static byte[] __CidSlot = new byte[2];
		internal int DecodeCidToUnicode(iTextSharp.text.pdf.fonts.cmaps.CMapCidUni cMap, int cid) {
			string s;
			if (AppContext.Encodings.TextEncoding != null) {
				__CidSlot[0] = (byte)(cid >> 8);
				__CidSlot[1] = (byte)cid;
				s = AppContext.Encodings.TextEncoding.GetString(__CidSlot);
			}
			else if (cMap != null) {
				return cMap.Lookup(cid);
			}
			else {
				__CidSlot[0] = (byte)(cid >> 8);
				__CidSlot[1] = (byte)cid;
				s = Decode(__CidSlot, 0, 2);
			}
			if (s.Length == 0) {
				return 0;
			}
			return s[0];
		}

		internal string DecodeTextBytes(byte[] bytes) {
			if (AppContext.Encodings.TextEncoding != null) {
				return AppContext.Encodings.TextEncoding.GetString(bytes);
			}
			if (CjkType == CjkFontType.Chinese) {
				return __GbkEncoding.GetString(bytes);
			}
			return Decode(bytes, 0, bytes.Length);
		}
		internal string DecodeText(PdfString text) {
			return DecodeTextBytes(text.GetBytes());
		}
	}
}
