using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Model;

internal static class PdfModelHelper
{
	internal static T Locate<T>(this PdfDictionary source, params object[] path) where T : PdfObject {
		PdfObject s = source;
		if (s == null) {
			return null;
		}

		foreach (object item in path) {
			PdfName n = item as PdfName;
			if (s == null) {
				return null;
			}

			if (n != null) {
				if (s.Type != PdfObject.DICTIONARY && s.Type != PdfObject.STREAM) {
					return null;
				}

				s = (s as PdfDictionary).GetDirectObject(n);
				continue;
			}

			if (item is int == false) {
				throw new ArgumentException("参数类型必须为 Int32 或 PdfName");
			}

			int i = (int)item;
			if (s.Type != PdfObject.ARRAY) {
				return null;
			}

			s = (s as PdfArray).GetDirectObject(i);
		}

		return s as T;
	}

	internal static T Locate<T>(this PdfDictionary source, params PdfName[] path) where T : PdfObject {
		return source.Locate<T>(true, path);
	}

	internal static T Locate<T>(this PdfDictionary source, bool resolveRef, params PdfName[] path) where T : PdfObject {
		PdfObject o = null;
		foreach (PdfName item in path) {
			if (source == null) {
				return null;
			}

			o = resolveRef ? source.GetDirectObject(item) : source.Get(item);
			source = o as PdfDictionary;
		}

		return o as T;
	}

	internal static T Locate<T>(this PdfArray source, int index) where T : PdfObject {
		return source.Locate<T>(true, index);
	}

	internal static T Locate<T>(this PdfArray source, bool resolveRef, int index) where T : PdfObject {
		if (source == null) {
			return null;
		}

		return (resolveRef ? source.GetDirectObject(index) : source[index]) as T;
	}

	internal static T CastAs<T>(this PdfIndirectReference pdfRef) where T : PdfObject {
		return PdfReader.GetPdfObject(pdfRef) as T;
	}

	internal static bool ValueIs(this PdfNumber obj, double value) {
		return obj != null && obj.DoubleValue == value;
	}

	internal static bool ValueIs(this PdfBoolean obj, bool value) {
		return obj != null && obj.BooleanValue == value;
	}

	internal static bool ValueIs(this PdfObject obj, PdfName value) {
		return value.Equals(obj);
	}

	internal static int TryGetInt32(this PdfDictionary source, PdfName key, int defaultValue) {
		PdfNumber w = source.GetAsNumber(key);
		return w?.IntValue ?? defaultValue;
	}

	internal static bool TryGetBoolean(this PdfDictionary source, PdfName key, bool defaultValue) {
		PdfBoolean b = source.GetAsBoolean(key);
		return b?.BooleanValue ?? defaultValue;
	}

	internal static string Decode(this PdfString text, Encoding encoding) {
		if (text == null) {
			return string.Empty;
		}

		byte[] bytes = text.GetBytes();
		using MemoryStream ms = new(bytes);
		if (encoding == null) {
			if (bytes.Length < 2 ||
				((bytes[0] != 0xFF || bytes[1] != 0xFE) && (bytes[0] != 0xFE || bytes[1] != 0xFF))) {
				return PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_PDFDOCENCODING);
			}

			using TextReader r = new StreamReader(ms, true);
			return r.ReadToEnd();
		}

		switch (bytes.Length) {
			// 忽略字节顺序标记
			case >= 2 when ((bytes[0] == 0xFF && bytes[1] == 0xFE) || (bytes[0] == 0xFE && bytes[1] == 0xFF)):
				ms.Position += 2;
				break;
			case >= 3 when bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf:
				ms.Position += 3;
				break;
			case >= 4 when bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0xfe && bytes[3] == 0xff:
				ms.Position += 4;
				break;
		}

		using (TextReader r = new StreamReader(ms, encoding)) {
			return r.ReadToEnd();
		}
	}

	internal static PdfString ToPdfString(this string text) {
		if (string.IsNullOrEmpty(text)) {
			return new PdfString();
		}

		bool u = false;
		foreach (char c in text) {
			if (c <= 127) {
				continue;
			}

			u = true;
			break;
		}

		return u ? new PdfString(text, PdfObject.TEXT_UNICODE) : new PdfString(text);
	}
}