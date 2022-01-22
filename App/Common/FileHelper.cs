using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PDFPatcher.Common;

internal static class FileHelper
{
	private static OverwriteType __OverwriteMode;

	public static bool HasExtension(FilePath fileName, string extension) {
		return fileName.HasExtension(extension);
	}

	public static bool HasFileNameMacro(string fileName) {
		char c = '<';
		foreach (char item in fileName.Where(item => item == c)) {
			if (c == '>') {
				return true;
			}

			c = '>';
		}

		return false;
	}

	public static int NumericAwareComparePath(string path1, string path2) {
		const char PathDot = '.';
		int l1 = path1?.Length ?? 0;
		int l2 = path2?.Length ?? 0;
		if (l1 == 0 || l2 == 0) {
			return l1 - l2;
		}

		int n1 = 0, n2 = 0;
		for (int i1 = 0, i2 = 0; i1 < l1 && i2 < l2; i1++, i2++) {
			char x = path1[i1];
			char y = path2[i2];
			if (x is < '0' or > '9') {
				// 不区分大小写的文字比较
				if (x == y) {
					continue;
				}

				x = ToLowerAscii(x);
				y = ToLowerAscii(y);
				if (x == y) {
					continue;
				}

				return x == PathDot
					? -1
					: y == PathDot
						? 1
						: y is < '0' or > '9'
							? LocaleInfo.StringComparer(x.ToString(), y.ToString(), CompareOptions.StringSort)
							// path2 为数字，path1 不为数字，path2 排在前面
							: 1;
			}

			if (IsPathSeparator(x)) {
				if (IsPathSeparator(y)) {
					continue;
				}

				return -1;
			}
			if (IsPathSeparator(y)) {
				return 1;
			}
			// 数字比较
			if (y is >= '0' and <= '9') {
				// 两组均为数字
				do {
					if (x > '0' || n1 > 0) {
						++n1;
					}
				} while (++i1 < l1 && (x = path1[i1]) >= '0' && x <= '9');

				do {
					if (y > '0' || n2 > 0) {
						++n2;
					}
				} while (++i2 < l2 && (y = path2[i2]) >= '0' && y <= '9');

				// 数字位数少的在前面
				if (n1 != n2) {
					return n1 - n2;
				}

				// 全是 0，继续后面的比较
				if (n1 == 0 || n2 == 0) {
					--i1;
					--i2;
					continue;
				}

				i1 -= n1;
				i2 -= n2;
				for (int i = 0; i < n1; i++, i1++, i2++) {
					x = path1[i1];
					y = path2[i2];
					if (x != y) {
						return x - y;
					}
				}

				// 数值相等，比较下一组
				n1 = n2 = 0;
				--i1;
				--i2;
			}
			else {
				// 仅 x 为数字，y 不为数字
				return y == PathDot ? 1 : x - y;
			}
		}

		return path1.Length - path2.Length;
	}

	private static char ToLowerAscii(char c) {
		return c is >= 'A' and <= 'Z' ? (char)(c + ('a' - 'A')) : c;
	}

	internal static bool CheckOverwrite(string targetFile) {
		if (!File.Exists(targetFile)) {
			return true;
		}

		switch (__OverwriteMode) {
			case OverwriteType.Prompt:
				DialogResult r = FormHelper.YesNoCancelBox(string.Join("\n", "是否覆盖目标文件？", targetFile,
					"\n按住 Shift 键重复此对话框的选择，本次操作不再弹出覆盖文件提示。"));
				if (r == DialogResult.No) {
					if (FormHelper.IsShiftKeyDown) {
						__OverwriteMode = OverwriteType.Skip;
					}

					goto case OverwriteType.Skip;
				}
				else if (r == DialogResult.Cancel) {
					throw new OperationCanceledException();
				}

				if (FormHelper.IsShiftKeyDown) {
					__OverwriteMode = OverwriteType.Overwrite;
				}

				break;
			case OverwriteType.Overwrite:
				return true;
			case OverwriteType.Skip:
				Tracker.TraceMessage(Tracker.Category.ImportantMessage, "取消覆盖文件：" + targetFile);
				return false;
			default:
				goto case OverwriteType.Prompt;
		}

		return true;
	}

	public static string CombinePath(string path1, string path2) {
		if (string.IsNullOrEmpty(path2)) {
			return path1 ?? string.Empty;
		}

		if (string.IsNullOrEmpty(path1)) {
			return path2;
		}

		int l2 = path2.Length;
		if (l2 > 0) {
			if (IsPathSeparator(path2[0]) || (l2 > 1 && path2[1] == Path.VolumeSeparatorChar)) {
				return path2;
			}
		}

		char ch = path1[path1.Length - 1];
		if (IsPathSeparator(ch) == false) {
			return path1 + Path.DirectorySeparatorChar + path2;
		}

		return path1 + path2;
	}

	public static bool ComparePath(FilePath path1, FilePath path2) {
		return path1.Equals(path2);
	}

	public static bool IsPathValid(string filePath) {
		return string.IsNullOrEmpty(filePath) == false && filePath.Trim().Length > 0
													   && filePath.IndexOfAny(FilePath.InvalidPathChars) == -1;
	}

	private static bool IsPathSeparator(char c) {
		return c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;
	}

	internal static void ResetOverwriteMode() {
		__OverwriteMode = OverwriteType.Prompt;
	}

	internal static string MakePathRootedAndWithExtension(string path, string basePath, string extName, bool forceExt) {
		string p = Path.Combine(Path.GetDirectoryName(basePath), path);
		if (Path.GetExtension(p).Length == 0 || forceExt) {
			return new FilePath(p).EnsureExtension(extName);
		}

		return p;
	}

	private static FilePath AttachExtensionName(FilePath fileName, string extension) {
		return fileName.EnsureExtension(extension);
	}

	private static string GetNewFileName(string fileName, string extName) {
		FilePath tmpName = AttachExtensionName(fileName, extName);
		int n = 1;
		while (tmpName.ExistsFile) {
			tmpName = AttachExtensionName(fileName + "[" + ++n + "]", extName);
		}

		return tmpName.ToString();
	}

	public static string GetValidFileName(FilePath fileName) {
		return fileName.SubstituteInvalidChars('_').ToString();
	}

	internal static string GetNewFileNameFromSourceFile(string fileName, string extName) {
		string d = Path.GetDirectoryName(fileName);
		return GetNewFileName(
				d + @"\" + Path.GetFileNameWithoutExtension(fileName),
				extName)
			.Replace(@"\\", @"\");
	}

	internal static string GetTempNameFromFileDirectory(string fileName, string extName) {
		string f = Path.GetDirectoryName(fileName);
		string t = Path.GetTempFileName();
		return Path.Combine(f, t + extName);
	}

	public static string GetEllipticPath(string path, int length) {
		if (length < 11) {
			length = 11;
		}

		return path == null ? string.Empty
			: path.Length > length ? path.Substring(0, 7) + "..." + path.Substring(path.Length - (length - 10))
			: path;
	}

	public static byte[] DumpBytes(this byte[] source, FilePath path) {
		return source.DumpBytes(path, 0, source?.Length ?? 0);
	}

	public static byte[] DumpBytes(this byte[] source, FilePath path, int offset, int count) {
		using FileStream f = new(path.ToFullPath(), FileMode.OpenOrCreate, FileAccess.Write);
		if (source == null) {
			return null;
		}

		f.Write(source, offset, count);

		return source;
	}

	public static byte[] DumpHexBinBytes(this byte[] source, FilePath path) {
		using StreamWriter f = new(path.ToFullPath());
		if (source == null) {
			return null;
		}

		for (int i = 0; i < source.Length; i++) {
			if (i > 0) {
				if ((i & 0xF) == 0) {
					f.WriteLine();
				}

				if ((i & 0x1) == 0) {
					f.Write(' ');
				}
			}

			byte b = source[i];
			byte t = (byte)(b >> 4);
			f.Write((char)(t + (t > 9 ? 'A' - 10 : 0x30)));
			t = (byte)(b & 0x0F);
			f.Write((char)(t + (t > 9 ? 'A' - 10 : 0x30)));
		}

		return source;
	}

	private enum OverwriteType
	{
		Prompt, Overwrite, Skip
	}

	private static class LocaleInfo
	{
		public static readonly Func<string, string, CompareOptions, int> StringComparer =
			CultureInfo.CurrentCulture.CompareInfo.Compare;
	}
}