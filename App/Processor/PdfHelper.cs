using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal static class PdfHelper
	{
		internal static readonly int[] CompoundTypes = new int[] { PdfObject.DICTIONARY, PdfObject.ARRAY, PdfObject.STREAM };

		static readonly DualKeyDictionary<PdfName, string> __PdfNameMap;
		static readonly Dictionary<string, byte[]> __PdfPasswordCache = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
		static bool __SuppressUnethicalWarning;

		/// <summary>
		/// 切换强制读取加密文档模式。
		/// </summary>
		/// <param name="unethicalreading">是否打开强制读取模式。</param>
		internal static void ToggleUnethicalMode(bool unethicalreading) {
			PdfReader.unethicalreading = unethicalreading;
		}
		/// <summary>
		/// 切换容错模式（忽略 PDF 文档的错误）。
		/// </summary>
		/// <param name="debugMode">是否打开容错模式。</param>
		internal static void ToggleReaderDebugMode(bool debugMode) {
			PdfReader.debugmode = debugMode;
		}

		/// <summary>
		/// 打开 PDF 文件，在有需要时提示输入密码。
		/// </summary>
		/// <param name="sourceFile">需要打开的 PDF 文件。</param>
		/// <param name="partial">是否仅打开文件的部分内容。</param>
		/// <returns><see cref="PdfReader"/> 实例。</returns>
		internal static PdfReader OpenPdfFile(string sourceFile, bool partial, bool removeUnusedObjects) {
			byte[] password;
			__PdfPasswordCache.TryGetValue(sourceFile, out password);
			while (true) {
				try {
					if (File.Exists(sourceFile) == false) {
						throw new FileNotFoundException(String.Concat("找不到文件：", sourceFile));
					}
					PdfReader r;
					if (partial) {
						r = new PdfReader(new RandomAccessFileOrArray(sourceFile), password);
					}
					else {
						r = new PdfReader(sourceFile, password);
					}
					if (password != null && password.Length > 0) {
						__PdfPasswordCache[sourceFile] = password;
					}
					if (removeUnusedObjects) {
						r.RemoveUnusedObjects();
					}
					return r;
				}
				catch (iTextSharp.text.exceptions.BadPasswordException) {
					var f = new PasswordEntryForm(sourceFile);
					if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) {
						throw new iTextSharp.text.exceptions.BadPasswordException("密码错误，没有权限打开 PDF 文件。");
					}
					password = Encoding.Default.GetBytes(f.Password);
				}
				catch (iTextSharp.text.exceptions.InvalidPdfException ex) {
					FormHelper.ErrorBox("PDF 文档已经损坏，或使用了不支持的加密方式：" + ex.Message);
					throw;
				}
			}
		}

		internal static bool ConfirmUnethicalMode(this PdfReader pdf) {
			ToggleUnethicalMode(false);
			var r = pdf.IsOpenedWithFullPermissions
					|| __SuppressUnethicalWarning
					|| FormHelper.ConfirmOKBox(Messages.UserRightRequired);
			ToggleUnethicalMode(true);
			if (__SuppressUnethicalWarning == false && FormHelper.IsCtrlKeyDown) {
				__SuppressUnethicalWarning = true;
			}
			return r;
		}

		internal static MuPdfSharp.MuDocument OpenMuDocument(string sourceFile) {
			var d = new MuPdfSharp.MuDocument(sourceFile);
			if (d.NeedsPassword) {
				if (__PdfPasswordCache.TryGetValue(sourceFile, out byte[] password)) {
					d.AuthenticatePassword(password != null ? Encoding.Default.GetString(password) : String.Empty);
				}
				while (d.IsAuthenticated == false) {
					using (var f = new PasswordEntryForm(sourceFile)) {
						if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) {
							throw new iTextSharp.text.exceptions.BadPasswordException("密码错误，没有权限打开 PDF 文件。");
						}
						__PdfPasswordCache[sourceFile] = password = Encoding.Default.GetBytes(f.Password);
					}
					d.AuthenticatePassword(password != null ? Encoding.Default.GetString(password) : String.Empty);
				}
			}
			return d;
		}

		static PdfHelper() {
			__PdfNameMap = InitPdfNameMap();
		}

		static DualKeyDictionary<PdfName, string> InitPdfNameMap() {
			var m = new DualKeyDictionary<PdfName, string> {
				{ PdfName.PAGELAYOUT, Constants.PageLayout },

				{ PdfName.PAGEMODE, Constants.PageMode },

				{ PdfName.DIRECTION, Constants.ViewerPreferencesType.Direction },

				{ PdfName.ST, Constants.PageLabelsAttributes.StartPage },
				{ PdfName.P, Constants.PageLabelsAttributes.Prefix },
				{ PdfName.S, Constants.PageLabelsAttributes.Style },

				{ PdfName.XYZ, Constants.DestinationAttributes.ViewType.XYZ },
				{ PdfName.FIT, Constants.DestinationAttributes.ViewType.Fit },
				{ PdfName.FITB, Constants.DestinationAttributes.ViewType.FitB },
				{ PdfName.FITBH, Constants.DestinationAttributes.ViewType.FitBH },
				{ PdfName.FITBV, Constants.DestinationAttributes.ViewType.FitBV },
				{ PdfName.FITH, Constants.DestinationAttributes.ViewType.FitH },
				{ PdfName.FITR, Constants.DestinationAttributes.ViewType.FitR },
				{ PdfName.FITV, Constants.DestinationAttributes.ViewType.FitV },

				{ PdfName.GOTO, Constants.ActionType.Goto },
				{ PdfName.GOTOR, Constants.ActionType.GotoR },
				{ PdfName.LAUNCH, Constants.ActionType.Launch },
				{ PdfName.URI, Constants.ActionType.Uri }
			};

			return m;
		}

		internal static string GetTypeName(int t) {
			switch (t) {
				case PdfObject.ARRAY: return "array";
				case PdfObject.BOOLEAN: return "bool";
				case PdfObject.DICTIONARY: return "dictionary";
				case PdfObject.INDIRECT: return "reference";
				case PdfObject.NAME: return "name";
				case PdfObject.NULL: return "null";
				case PdfObject.NUMBER: return "number";
				case PdfObject.STREAM: return "stream";
				case PdfObject.STRING: return "string";
				default: return String.Empty;
			}
		}

		/// <summary>
		/// 获取友好的 PdfName 文本。
		/// </summary>
		internal static string GetPdfFriendlyName(PdfName name) {
			return (__PdfNameMap.ContainsKey(name) ? __PdfNameMap[name] : GetPdfNameString(name));
		}
		/// <summary>
		/// 解析 PdfName。
		/// </summary>
		/// <param name="friendlyName">从 <seealso cref="GetPdfFriendlyName"/> 转换所得的 PdfName 说明文本。</param>
		/// <returns>与文本说明对应的 PdfName。</returns>
		internal static PdfName ResolvePdfName(string friendlyName) {
			return (__PdfNameMap.ContainsValue(friendlyName) ? __PdfNameMap.GetKeyByValue(friendlyName) : new PdfName(friendlyName));
		}

		internal static string DecodeKeyName(object name) {
			if (name is PdfName) {
				return PdfName.DecodeName(name.ToString());
			}
			else
				return name.ToString();
		}

		/// <summary>
		/// 获取 PDF 页面引用与页数的映射关系表。
		/// </summary>
		/// <param name="reader">源 PDF 文档。</param>
		/// <returns>键为 <see cref="PdfIndirectReference"/> 的数值，值为页数的字典。</returns>
		internal static Dictionary<int, int> GetPageRefMapper(this PdfReader reader) {
			int numPages = reader.NumberOfPages;
			var pages = new Dictionary<int, int>(numPages);
			for (int k = 1; k <= numPages; ++k) {
				pages[reader.GetPageOrigRef(k).Number] = k;
				reader.ReleasePage(k);
			}
			return pages;
		}

		/// <summary>获取 <see cref="PdfWriter"/> 实际写出的页数。</summary>
		public static int GetPageCount(this PdfWriter writer) {
			return writer.PageEmpty ? writer.CurrentPageNumber - 1 : writer.CurrentPageNumber;
		}

		/// <summary>
		/// 解析形如“D:20111021090818+08'00'”的日期格式。
		/// </summary>
		/// <param name="date">日期格式。</param>
		/// <returns></returns>
		internal static DateTimeOffset ParseDateTime(string date) {
			if (date == null
				|| date.Length != 23 && date.Length != 16
				|| date.StartsWith("D:") == false) {
				return DateTimeOffset.MinValue;
			}
			try {
				return new DateTimeOffset(
					date.Take(2, 4).ToInt32(), date.Take(6, 2).ToInt32(), date.Take(8, 2).ToInt32(),
					date.Take(10, 2).ToInt32(), date.Take(12, 2).ToInt32(), date.Take(14, 2).ToInt32(),
					new TimeSpan(date.Take(16, 3).ToInt32(), date.Take(20, 2).ToInt32(), 0)
					);
			}
			catch (Exception) {
				return DateTimeOffset.MinValue;
			}
		}

		/// <summary>
		/// 获取解码后的 PDF 名称字符串。
		/// </summary>
		/// <param name="name">需要解码的 PDF 名称。</param>
		/// <returns>解码后的 PDF 名称字符串</returns>
		internal static string GetPdfNameString(PdfName name) {
			return GetPdfNameString(name, PdfName.DefaultEncoding);
		}

		/// <summary>
		/// 获取解码后的 PDF 名称字符串。
		/// </summary>
		/// <param name="name">需要解码的 PDF 名称。</param>
		/// <param name="encoding">用于解码的文本编码。</param>
		/// <returns>解码后的 PDF 名称字符串</returns>
		internal static string GetPdfNameString(PdfName name, Encoding encoding) {
			var s = name.ToString();
			int len = s.Length;
			var buf = new byte[len];
			int l = 0;
			for (int k = 1; k < len; ++k) {
				char c = s[k];
				if (c == '#') {
					buf[l] = (byte)((PRTokeniser.GetHex(s[++k]) << 4) + PRTokeniser.GetHex(s[++k]));
				}
				else {
					buf[l] = (byte)c;
				}
				l++;
			}
			return (encoding ?? System.Text.Encoding.Default).GetString(buf, 0, l);
		}

		/// <summary>
		/// 获取页面可见的边框。
		/// </summary>
		/// <param name="page">页面字典。</param>
		/// <returns>页面的可见边框。</returns>
		internal static iTextSharp.text.Rectangle GetPageVisibleRectangle(this PdfDictionary page) {
			PdfArray box;
			if (page == null
				|| ((box = page.GetAsArray(PdfName.CROPBOX)) == null || box.Size != 4)
					&& ((box = page.GetAsArray(PdfName.MEDIABOX)) == null || box.Size != 4)) {
				return null;
			}
			var c = new float[4];
			for (int i = 0; i < 4; i++) {
				c[i] = box.GetAsNumber(i).FloatValue;
			}
			var r = page.GetAsNumber(PdfName.ROTATE);
			return r != null && r.IntValue != 0 && r.IntValue != 180
				? new iTextSharp.text.Rectangle(c[0], c[1], c[2], c[3], r.IntValue).Rotate()
				: new iTextSharp.text.Rectangle(c[0], c[1], c[2], c[3]);
		}

		internal static void ClearPageLinks(this PdfReader r) {
			int pageCount = r.NumberOfPages + 1;
			for (int i = 1; i < pageCount; i++) {
				ClearPageLinks(r, i);
			}
		}

		internal static void ClearPageLinks(this PdfReader r, int pageNumber) {
			r.ResetReleasePage();
			var pageDic = r.GetPageN(pageNumber);
			var annots = (PdfArray)PdfReader.GetPdfObjectRelease(pageDic.Get(PdfName.ANNOTS));
			if (annots == null) {
				r.ReleasePage(pageNumber);
			}
			else {
				IList<PdfObject> arr = annots.ArrayList;
				for (int j = arr.Count - 1; j >= 0; j--) {
					var item = arr[j];
					var annot = (PdfDictionary)PdfReader.GetPdfObjectRelease(item);
					if (PdfName.LINK.Equals(annot.Get(PdfName.SUBTYPE))) {
						arr.RemoveAt(j);
					}
				}
				if (annots.ArrayList.Count == 0) {
					pageDic.Remove(PdfName.ANNOTS);
				}
			}
			r.ResetReleasePage();
		}

		internal static string GetValidXmlString(string value) {
			if (String.IsNullOrEmpty(value)) {
				return String.Empty;
			}
			var marks = new List<int>(3);
			int p = 0;
			foreach (char c in value) {
				switch (c) {
					case '\t':
					case '\r':
					case '\n':
						break;
					default:
						// invalid character
						if (Char.IsControl(c)) {
							if (marks.Count == 0) {
								marks.Add(0);
							}
							marks.Add(p + 1);
						}
						break;
				}
				p++;
			}
			if (marks.Count > 0) {
				marks.Add(value.Length + 1);
			}
			if (marks.Count > 1) {
				var sb = StringBuilderCache.Acquire();
				for (int i = 1; i < marks.Count; i++) {
					if (i > 1) {
						sb.Append(' ');
					}
					sb.Append(value, marks[i - 1], marks[i] - 1 - marks[i - 1]);
				}
				return StringBuilderCache.GetStringAndRelease(sb);
			}
			else {
				return value;
			}
		}

		internal static string GetArrayString(PdfArray array) {
			return GetArrayString(array.ArrayList);
		}

		internal static string GetArrayString(ICollection<PdfObject> array) {
			var sb = StringBuilderCache.Acquire();
			int k = 0;
			foreach (var item in array) {
				if (++k > 1) {
					sb.Append(' ');
				}
				if (item.Type == PdfObject.ARRAY) {
					sb.Append('[');
					sb.Append(GetArrayString(item as PdfArray));
					sb.Append(']');
				}
				else if (item.Type == PdfObject.DICTIONARY || item.Type == PdfObject.STREAM) {
					sb.Append("<<...>>");
				}
				else {
					sb.Append(item);
				}
			}
			return StringBuilderCache.GetStringAndRelease(sb);
		}

		internal static string GetNumericArrayString(PdfArray a, float unitFactor) {
			var sb = StringBuilderCache.Acquire();
			for (int k = 0; k < a.ArrayList.Count; k++) {
				if (k != 0) {
					sb.Append(' ');
				}
				if (a.GetAsArray(k) != null) {
					sb.Append('[');
					sb.Append(a.GetAsArray(k));
					sb.Append(']');
				}
				else {
					var o = a.ArrayList[k];
					sb.Append(o.Type == PdfObject.NUMBER ? UnitConverter.FromPoint(o.ToString(), unitFactor) : o.ToString());
				}
			}
			return StringBuilderCache.GetStringAndRelease(sb);
		}

		internal static void Put(this PdfDictionary dict, PdfName key, string value) {
			dict.Put(key, value.ToPdfString());
		}
		internal static void Put(this PdfDictionary dict, PdfName key, int value) {
			dict.Put(key, new PdfNumber(value));
		}
		internal static void Put(this PdfDictionary dict, PdfName key, double value) {
			dict.Put(key, new PdfNumber(value));
		}
		internal static void Put(this PdfDictionary dict, PdfName key, float value) {
			dict.Put(key, new PdfNumber(value));
		}
		internal static void Put(this PdfDictionary dict, PdfName key, bool value) {
			dict.Put(key, new PdfBoolean(value));
		}
		internal static void Put(this PdfDictionary dict, PdfName key, float[] values) {
			dict.Put(key, new PdfArray(values));
		}
		internal static void Put(this PdfDictionary dict, PdfName key, int[] values) {
			dict.Put(key, new PdfArray(values));
		}

		internal static PdfDictionary CreateDictionaryPath(this PdfDictionary source, params PdfName[] path) {
			PdfDictionary d;
			foreach (var item in path) {
				if (source.Contains(item)) {
					d = source.GetAsDict(item);
					if (d == null) {
						throw new InvalidCastException(item.ToString() + "不是 PdfDictionary。");
					}
					source = d;
					continue;
				}
				d = new PdfDictionary();
				source.Put(item, d);
				source = d;
				d = null;
			}
			return source;
		}

		internal static bool PdfReferencesAreEqual(PdfIndirectReference r1, PdfIndirectReference r2) {
			if (r1 == null || r2 == null) {
				return false;
			}
			return r1.Number == r2.Number && r1.Generation == r2.Generation;
		}

		internal static int GetPageRotation(PdfDictionary page) {
			return NormalizeRotationNumber(page.TryGetInt32(PdfName.ROTATE, 0));
		}

		internal static int NormalizeRotationNumber(int rotation) {
			switch (rotation) {
				case 0:
				case 90:
				case 180:
				case 270:
					return rotation;
				default:
					rotation = rotation % 90;
					if (rotation < 0) {
						rotation += 360;
					}
					return (rotation / 90 * 90);
			}
		}

		internal static IList<PdfObject> GetObjectDirectOrFromContainerArray(this PdfDictionary d, PdfName name, int pdfObjectType) {
			var results = new List<PdfObject>();

			PdfObject tmp;
			tmp = d.Get(name);
			if (tmp == null) {
			}
			else if (tmp.Type == pdfObjectType) {
				results.Add(tmp);
			}
			else if (tmp.Type == PdfObject.ARRAY) {
				var a = tmp as PdfArray;
				for (int i = 0; i < a.Size; i++) {
					tmp = a.GetDirectObject(i);
					if (tmp == null) {
						continue;
					}
					if (tmp.Type == pdfObjectType) {
						results.Add(tmp);
					}
				}
			}
			return results;
		}

		internal static string MatrixToString(iTextSharp.text.pdf.parser.Matrix ctm) {
			return String.Join(" ", ctm[0].ToText(), ctm[1].ToText(), ctm[3].ToText(), ctm[4].ToText(), ctm[6].ToText(), ctm[7].ToText());
		}

		/// <summary>
		/// 获取未使用对象的列表。
		/// </summary>
		/// <param name="pdf">需要检查的 PDF 文档。</param>
		/// <param name="partial">待检查 PDF 文档是否为部分加载。</param>
		/// <returns>包含未使用对象索引值的列表。</returns>
		internal static List<int> ListUnusedObjects(PdfReader pdf, bool partial) {
			var hits = new bool[pdf.XrefSize];
			GetUnusedNode(pdf, partial, pdf.Trailer, hits);
			int i = 0;
			var result = new List<int>();
			foreach (var item in hits) {
				if (item == false && i > 0) {
					result.Add(i);
				}
				++i;
			}
			return result;
		}

		internal static void SafeSetPageContent(this PdfReader pdf, int pageNumber, byte[] content) {
			var p = pdf.GetPageN(pageNumber);
			var c = p.GetDirectObject(PdfName.CONTENTS);
			if (c != null && c.IsArray()) {
				var cs = new PRStream(pdf, content);
				if (content.Length > 30) {
					cs.FlateCompress();
				}
				p.Put(PdfName.CONTENTS, pdf.AddPdfObject(cs));
			}
			else {
				pdf.SetPageContent(pageNumber, content);
			}
		}

		static void GetUnusedNode(PdfReader pdf, bool partial, PdfObject obj, bool[] hits) {
			var state = new Stack<object>();
			state.Push(obj);
			int oc = pdf.XrefSize;
			while (state.Count != 0) {
				var current = state.Pop();
				if (current == null)
					continue;
				List<PdfObject> ar = null;
				PdfDictionary dic = null;
				PdfName[] keys = null;
				Object[] objs = null;
				int idx = 0;
				if (current is PdfObject pdfObject) {
					obj = pdfObject;
					switch (obj.Type) {
						case PdfObject.DICTIONARY:
						case PdfObject.STREAM:
							dic = (PdfDictionary)obj;
							keys = new PdfName[dic.Size];
							dic.Keys.CopyTo(keys, 0);
							break;
						case PdfObject.ARRAY:
							ar = ((PdfArray)obj).ArrayList;
							break;
						case PdfObject.INDIRECT:
							var refi = (PRIndirectReference)obj;
							int num = refi.Number;
							if (!hits[num]) {
								hits[num] = true;
								state.Push(PdfReader.GetPdfObjectRelease(refi));
							}
							continue;
						default:
							continue;
					}
				}
				else {
					objs = (object[])current;
					if (objs[0] is List<PdfObject>) {
						ar = (List<PdfObject>)objs[0];
						idx = (int)objs[1];
					}
					else {
						keys = (PdfName[])objs[0];
						dic = (PdfDictionary)objs[1];
						idx = (int)objs[2];
					}
				}
				if (ar != null) {
					for (int k = idx; k < ar.Count; ++k) {
						var v = ar[k];
						if (v.IsIndirect()) {
							int num = ((PRIndirectReference)v).Number;
							if (num >= oc || (!partial && pdf.GetPdfObjectRelease(num) == null)) {
								ar[k] = PdfNull.PDFNULL;
								continue;
							}
						}
						if (objs == null) {
							state.Push(new object[] { ar, k + 1 });
						}
						else {
							objs[1] = k + 1;
							state.Push(objs);
						}
						state.Push(v);
						break;
					}
				}
				else {
					for (int k = idx; k < keys.Length; ++k) {
						var key = keys[k];
						var v = dic.Get(key);
						if (v.IsIndirect()) {
							int num = ((PRIndirectReference)v).Number;
							if (num >= oc || (!partial && pdf.GetPdfObjectRelease(num) == null)) {
								dic.Put(key, PdfNull.PDFNULL);
								continue;
							}
						}
						if (objs == null) {
							state.Push(new object[] { keys, dic, k + 1 });
						}
						else {
							objs[2] = k + 1;
							state.Push(objs);
						}
						state.Push(v);
						break;
					}
				}
			}
		}

	}
}
