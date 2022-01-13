﻿using System;
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

		private readonly static Encoding __GbkEncoding = System.Text.Encoding.GetEncoding("gbk");

		//private readonly static Encoding __Big5Encoding = System.Text.Encoding.GetEncoding ("big5");
		private readonly static PdfName[] __GbkEncodingNames = {
			new PdfName("GBK-EUC-H"), new PdfName("GBK-EUC-V"), new PdfName("GB-EUC-H"), new PdfName("GB-EUC-V")
		};

		private readonly static string[] __gbkFontNames = {"宋体", "黑体", "楷体_GB2312", "仿宋体", "仿宋_GB2312", "隶书", "幼圆"};
		readonly static PdfName[] __IdentityEncodingNames = {new PdfName("Identity-H"), new PdfName("Identity-V")};
		public const int DefaultDefaultWidth = 1000;

		//private readonly static string[] __big5FontNames = new string[] { "MINGLIU" };

		readonly PdfDictionary _Font;
		PdfDictionary _FontDescriptor;

		internal PdfDictionary FontDescriptor {
			get {
				if (_FontDescriptor == null) {
					_FontDescriptor = _Font.Locate<PdfArray>(PdfName.DESCENDANTFONTS).Locate<PdfDictionary>(0)
						.Locate<PdfDictionary>(PdfName.FONTDESCRIPTOR);
					if (_FontDescriptor == null) {
						_FontDescriptor = new PdfDictionary();
					}
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
					if (w == null) {
						_DefaultWidth = DefaultDefaultWidth;
					}
					else {
						_DefaultWidth = w.IntValue;
					}
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
			//&& PdfName.WIN_ANSI_ENCODING.Equals (this.fontDict.GetAsName (PdfName.ENCODING));
			_CjkFontType = c ? CjkFontType.Chinese : CjkFontType.None;
			if (_CjkFontType != CjkFontType.None) {
				return;
			}

			c = __IdentityEncodingNames.Contains(encoding);
			if (c) {
				_CjkFontType = CjkFontType.Unicode;
			}
			//c = Common.Range.InCollection (fn, __big5FontNames);
			//_CjkFontType = c ? CjkFontType.Big5Chinese : CjkFontType.None;
		}

		readonly int _FontID = -1;
		internal int FontID => _FontID;

		public FontInfo(PdfDictionary font, int refNumber)
			: base(font) {
			_Font = font;
			_FontID = refNumber;
			//this.DefaultWidth = _Font.Locate<PdfArray> (PdfName.DESCENDANTFONTS).Locate<PdfDictionary> (0).Locate<PdfDictionary> (PdfName.FONTDESCRIPTOR).TryGetInt32 (PdfName.W, 1000);
		}

		public FontInfo(PRIndirectReference refFont) : base(refFont) {
			_Font = (PdfDictionary)PdfReader.GetPdfObjectRelease(refFont);
			_FontID = refFont.Number;
			//this.DefaultWidth = _Font.Locate<PdfArray> (PdfName.DESCENDANTFONTS).Locate<PdfDictionary> (0).Locate<PdfDictionary> (PdfName.FONTDESCRIPTOR).TryGetInt32 (PdfName.W, 1000);
		}

		internal int DecodeCidToUnicode(int cid) {
			string s;
			if (AppContext.Encodings.TextEncoding != null) {
				s = AppContext.Encodings.TextEncoding.GetString(new byte[] {(byte)(cid >> 8), (byte)cid});
			}

			//if (CjkType == CjkFontType.Chinese) {
			//	s = __GbkEncoding.GetString (cid < 256 ? new byte[] { (byte)cid } : new byte[] { (byte)cid, (byte)(cid >> 8) });
			//}
			//else {
			s = Decode(new byte[] {(byte)(cid >> 8), (byte)cid}, 0, 2);
			//}
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

			//else if (CjkType == CjkFontType.Big5Chinese) {
			//	return __Big5Encoding.GetString (bytes);
			//}
			return Decode(bytes, 0, bytes.Length);
		}

		internal string DecodeText(PdfString text) {
			return DecodeTextBytes(text.GetBytes());
		}
	}
}