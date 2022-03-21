using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Processor;

namespace PDFPatcher.Model
{
	static class PdfDocumentFont
	{
		/// <summary>
		/// 删除字体名称的子集前缀。
		/// </summary>
		/// <param name="name">字体名称。</param>
		internal static string RemoveSubsetPrefix(string name) {
			return name.Length > 7 && name[6] == '+' ? name.Substring(7) : name;
		}

		/// <summary>
		/// 列举指定页面所用的字体。
		/// </summary>
		/// <param name="page">页面对应的 <see cref="PdfDictionary"/>。</param>
		/// <param name="fonts">用于放置字体名称的集合。</param>
		internal static void EnumerateFonts(PdfDictionary page, ICollection<string> fonts) {
			var fl = page.Locate<PdfDictionary>(true, PdfName.RESOURCES, PdfName.FONT);
			if (fl == null) {
				return;
			}
			foreach (var item in fl) {
				var fr = item.Value as PdfIndirectReference;
				if (fr == null) {
					continue;
				}
				var f = PdfReader.GetPdfObject(fr) as PdfDictionary;
				var fn = f.GetAsName(PdfName.BASEFONT);
				if (fn == null) {
					continue;
				}
				fonts.Add(RemoveSubsetPrefix(PdfHelper.GetPdfNameString(fn)));
			}
		}

		internal static bool HasEmbeddedFont(PdfDictionary font) {
			var df = font.Locate<PdfObject>(true, PdfName.DESCENDANTFONTS);
			if (df == null) {
				return IsEmbeddedFont(font);
			}
			if (df.Type == PdfObject.ARRAY) {
				foreach (var item in (df as PdfArray).ArrayList) {
					if (IsEmbeddedFont(PdfReader.GetPdfObjectRelease(item) as PdfDictionary) == false) {
						return false;
					}
				}
				return true;
			}
			df = PdfReader.GetPdfObjectRelease(df);
			return df.Type == PdfObject.DICTIONARY && IsEmbeddedFont(df as PdfDictionary);
		}

		private static bool IsEmbeddedFont(PdfDictionary font) {
			var fd = font.Locate<PdfDictionary>(true, PdfName.FONTDESCRIPTOR);
			if (fd == null) {
				return false;
			}
			return fd.Contains(PdfName.FONTFILE) || fd.Contains(PdfName.FONTFILE2) || fd.Contains(PdfName.FONTFILE3);
		}


	}
}
