using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;

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

		static bool IsEmbeddedFont(PdfDictionary font) {
			var fd = font.Locate<PdfDictionary>(true, PdfName.FONTDESCRIPTOR);
			if (fd == null) {
				return false;
			}
			return fd.Contains(PdfName.FONTFILE) || fd.Contains(PdfName.FONTFILE2) || fd.Contains(PdfName.FONTFILE3);
		}

		/// <summary>
		/// 列举指定页面所用的字体。
		/// </summary>
		/// <param name="page">页面对应的 <see cref="PdfDictionary"/>。</param>
		public static IEnumerable<ResourceReference> GetPageFontReferences(PdfDictionary page) {
			var visitedRefs = new HashSet<PdfIndirectReference>();
			var res = page.Locate<PdfDictionary>(true, PdfName.RESOURCES);
			if (res != null) {
				var fonts = res.GetAsDict(PdfName.FONT);
				if (fonts != null) {
					foreach (var fr in fonts) {
						if (fr.Value is PdfIndirectReference r) {
							yield return new ResourceReference(r, fr.Key, PdfReader.GetPdfObjectRelease(r) as PdfDictionary);
						}
					}
				}
				var xObjects = res.GetAsDict(PdfName.XOBJECT);
				if (xObjects != null) {
					foreach (var item in xObjects) {
						if (PdfReader.GetPdfObjectRelease(item.Value) is not PdfDictionary form || PdfName.FORM.Equals(form.GetAsName(PdfName.SUBTYPE)) == false) {
							continue;
						}
						foreach (var font in PdfModelHelper.GetReferencedResources(form, o => PdfName.FONT.Equals(o.GetAsName(PdfName.TYPE)), visitedRefs)) {
							yield return font;
						}
					}
				}
			}
			var annots = page.GetAsArray(PdfName.ANNOTS);
			if (annots != null) {
				foreach (var item in annots) {
					foreach (var font in PdfModelHelper.GetReferencedResources(PdfReader.GetPdfObjectRelease(item) as PdfDictionary, o => PdfName.FONT.Equals(o.GetAsName(PdfName.TYPE)), visitedRefs)) {
						yield return font;
					}
				}
			}
		}

	}
}
