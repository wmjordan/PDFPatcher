using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using SysDirectory = System.IO.Directory;
using NameList = System.Collections.Generic.List<string>;

namespace PDFPatcher.Common
{
	/// <summary>表示文件路径的结构。此结构可隐式转换为字符串、<see cref="FileInfo"/>、<see cref="DirectoryInfo"/> 和 <see cref="Uri"/>。</summary>
	public readonly struct FilePath : IEquatable<FilePath>
	{
		internal static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
		internal static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
		internal static readonly Func<string, string, bool> __PathComparer = StringComparer.OrdinalIgnoreCase.Equals;

		/// <summary>表示匹配任何文件的通配符。</summary>
		public const string Wildcard = "*";
		/// <summary>表示匹配当前目录、递归子目录和任何文件的通配符。</summary>
		public const string RecursiveWildchard = "**";

		/// <summary>表示没有任何内容的路径。</summary>
		public static readonly FilePath Empty = new FilePath(string.Empty, false);

		/// <summary>获取应用程序所在的目录路径。</summary>
		public static readonly FilePath AppRoot = ((FilePath)AppDomain.CurrentDomain.BaseDirectory).AppendPathSeparator();

		/// <summary>获取应用程序的文件路径（对于 Web 应用程序，返回 <see cref="Empty"/>）。</summary>
		public static readonly FilePath AppPath = Assembly.GetEntryAssembly() != null ? (FilePath)Assembly.GetEntryAssembly().Location : Empty;

		static readonly char[] __PathSeparators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
		static readonly string __CurrentPath = "." + Path.DirectorySeparatorChar;
		readonly string _value;

		/// <summary>传入文件路径的字符串形式，创建新的 <see cref="FilePath"/> 实例。在创建实例时，删除传入字符串内所有的前导和尾随空白。</summary>
		/// <param name="path">文件路径的字符串形式。</param>
		public FilePath(string path) : this(path, true) { }

		internal FilePath(string path, bool trim) {
			_value = (path == null || path.Length == 0)
				? string.Empty
				: trim ? path.Trim() : path;
		}

		/// <summary>返回当前路径的目录部分。如目录为相对路径，则先转换为以当前程序所在目录路径为基准的绝对路径。</summary>
		/// <returns>当前路径的目录部分。</returns>
		public FilePath Directory {
			get {
				const int None = 0, EndWithSep = 1, EndWithLetter = 2;
				var p = AppRoot.Combine(_value)._value;
				int s = None;
				for (int i = p.Length - 1; i >= 0; i--) {
					var c = p[i];
					bool d = IsDirectorySeparator(c);
					switch (s) {
						case None:
							if (d) {
								s = EndWithSep;
							}
							else if (Char.IsWhiteSpace(c) == false) {
								s = EndWithLetter;
							}
							continue;
						case EndWithSep:
							if (d) {
								continue;
							}
							else if (c == Path.VolumeSeparatorChar) {
								return Empty;
							}
							else {
								return p.Substring(0, i + 1);
							}
						case EndWithLetter:
							if (d) {
								return p.Substring(0, (i == 2 && p[1] == Path.VolumeSeparatorChar || i == 0) ? i + 1 : i);
							}
							else if (c == Path.VolumeSeparatorChar) {
								return p.Substring(0, i + 1) + Path.DirectorySeparatorChar;
							}
							break;
					}
				}
				return Empty;
			}
		}

		/// <summary>返回当前路径是否以目录分隔符结束。</summary>
		public bool EndsWithPathSeparator {
			get {
				if (String.IsNullOrEmpty(_value)) {
					return false;
				}
				for (int i = _value.Length - 1; i >= 0; i--) {
					var c = _value[i];
					if (Char.IsWhiteSpace(c)) {
						continue;
					}
					return IsDirectorySeparator(c);
				}
				return false;
			}
		}

		/// <summary>检查当前路径对应的文件是否存在。</summary>
		public bool ExistsFile => File.Exists(ToFullPath()._value);

		/// <summary>检查当前路径对应的目录是否存在。</summary>
		public bool ExistsDirectory => SysDirectory.Exists(ToFullPath()._value);

		/// <summary>获取文件路径的文件名部分。</summary>
		public string FileName {
			get {
				if (IsEmpty) {
					return string.Empty;
				}
				char c;
				for (int i = _value.Length - 1; i >= 0; i--) {
					c = _value[i];
					if (IsDirectorySeparator(c) || c == Path.VolumeSeparatorChar) {
						return _value.Substring(++i);
					}
				}
				return _value;
			}
		}

		/// <summary>获取文件路径的文件名（不包含扩展名）部分。</summary>
		public string FileNameWithoutExtension {
			get {
				if (IsEmpty) {
					return string.Empty;
				}
				char c;
				var l = _value.Length;
				var d = l;
				for (int i = d - 1; i >= 0; i--) {
					c = _value[i];
					if (c == '.') {
						if (d == l) {
							d = i;
						}
					}
					else if (IsDirectorySeparator(c) || c == Path.VolumeSeparatorChar) {
						return d != l ? _value.Substring(++i, d - i) : _value.Substring(++i);
					}
				}
				return d != l ? _value.Substring(0, d) : _value;
			}
		}

		/// <summary>获取文件路径的文件扩展名部分。</summary>
		public string FileExtension {
			get {
				if (IsEmpty) {
					return string.Empty;
				}
				char c;
				int i;
				for (i = _value.Length - 1; i >= 0; i--) {
					c = _value[i];
					if (IsDirectorySeparator(c)) {
						i = -1;
						break;
					}
					if (c == '.') {
						break;
					}
				}
				return i > -1 && i < _value.Length - 1 ? _value.Substring(i) : string.Empty;
			}
		}

		/// <summary>返回当前路径是否为空。</summary>
		public bool IsEmpty => String.IsNullOrEmpty(_value);

		/// <summary>返回当前文件路径是否有效。</summary>
		public bool IsValidPath => _value?.Trim().Length > 0 && _value.IndexOfAny(InvalidPathChars) == -1;

		/// <summary>在路径后附加 <see cref="Path.DirectorySeparatorChar"/> 字符。</summary>
		/// <returns>附加了“\”字符的路径。</returns>
		public FilePath AppendPathSeparator() {
			return IsEmpty == false && _value[_value.Length - 1] == Path.DirectorySeparatorChar
				? this
				: (FilePath)(_value + Path.DirectorySeparatorChar);
		}
		/// <summary>删除路径尾部的 <see cref="Path.DirectorySeparatorChar"/> 或 <see cref="Path.AltDirectorySeparatorChar"/> 字符。</summary>
		/// <returns>删除了尾部“\”字符的路径。</returns>
		public FilePath TrimPathSeparator() {
			if (_value != null) {
				string p = _value;
				int i;
				for (i = p.Length - 1; i >= 0; i--) {
					char c = p[i];
					if (!Char.IsWhiteSpace(c) && IsDirectorySeparator(c) == false) {
						return _value.Substring(0, i + 1);
					}
				}
			}
			return Empty;
		}

		/// <summary>替换文件路径的扩展名为新的扩展名。</summary>
		/// <param name="extension">新的扩展名。</param>
		/// <returns>替换扩展名后的路径。</returns>
		public FilePath ChangeExtension(string extension) {
			if (IsEmpty) {
				return Empty;
			}
			if (extension == null) {
				extension = String.Empty;
			}
			else if ((extension = extension.TrimEnd()).Length == 0) {
				extension = ".";
			}
			else if (extension[0] != '.') {
				extension = "." + extension;
			}
			int i;
			char c;
			for (i = _value.Length - 1; i >= 0; i--) {
				c = _value[i];
				if (IsDirectorySeparator(c)) {
					i = -1;
					break;
				}
				if (c == '.') {
					break;
				}
			}
			return new FilePath(i >= 0 ? _value.Substring(0, i) + extension : _value + extension, false);
		}

		/// <inheritdoc cref="Combine(FilePath, bool)"/>
		public FilePath Combine(FilePath path) {
			return Combine(path, false);
		}

		/// <summary>合并两个文件路径。如 <paramref name="path"/> 为绝对路径，则返回该路径。</summary>
		/// <param name="path">子路径。</param>
		/// <param name="rootAsRelative">对于 <paramref name="path"/> 以 <see cref="Path.DirectorySeparatorChar"/> 开头的情况，取值为 <see langword="true"/> 时，视为以当前目录为基础目录；否则将 <paramref name="path"/> 视为从根目录开始，返回 <paramref name="path"/>。</param>
		/// <returns>合并后的路径。</returns>
		public FilePath Combine(FilePath path, bool rootAsRelative) {
			if (path.IsEmpty) {
				return _value != null ? this : Empty;
			}
			if (IsEmpty) {
				return path._value != null ? path : Empty;
			}
			var p2 = path._value;
			var ps = p2[0];
			bool p2r;
			if ((p2r = IsDirectorySeparator(ps)) && rootAsRelative == false // note 不能调转 && 参数的顺序，p2r 在后面有用
				|| p2.Length > 1 && p2[1] == Path.VolumeSeparatorChar) {
				return path;
			}

			var p1 = _value/*.TrimEnd()*/; // _value 已在创建时 Trim 过，不需再 Trim
			if (ps == '.') { // 合并扩展名到当前路径
				return p1 + p2;
			}
			return IsDirectorySeparator(p1[p1.Length - 1]) == false && p2r == false
				? new FilePath(p1 + Path.DirectorySeparatorChar + p2)
				: new FilePath(p1 + p2, false);
		}

		/// <summary>为当前文件路径创建目录。如文件路径为空，则不创建路径。</summary>
		/// <returns>所创建目录的路径。</returns>
		public FilePath CreateDirectory() {
			if (IsEmpty) {
				return Empty;
			}
			var p = ToFullPath();
			if (SysDirectory.Exists(p) == false) {
				SysDirectory.CreateDirectory(p);
			}
			return p;
		}

		/// <summary>为当前文件路径创建其所属的目录。如文件路径为空，则不创建路径。</summary>
		/// <returns>所创建目录的路径。</returns>
		public FilePath CreateContainingDirectory() {
			if (IsEmpty) {
				return Empty;
			}
			var f = Directory;
			if (SysDirectory.Exists(f._value) == false) {
				SysDirectory.CreateDirectory(f._value);
			}
			return f;
		}

		/// <summary>删除当前文件路径对应的文件。</summary>
		public void DeleteFile() {
			var p = ToFullPath()._value;
			File.Delete(p);
		}
		/// <summary>删除当前文件路径对应的目录。如路径指向的目录不存在，不执行任何操作。</summary>
		/// <param name="recursive">是否递归删除子目录的文件</param>
		public void DeleteDirectory(bool recursive) {
			var p = ToFullPath()._value;
			SysDirectory.Delete(p, recursive);
		}

		/// <summary>返回附加指定扩展名的实例。如当前路径已包含指定的扩展名，则返回当前路径，否则返回附加扩展名的实例。</summary>
		/// <param name="extension">需要附加的文件扩展名。</param>
		/// <returns>附加指定扩展名的实例。</returns>
		public FilePath EnsureExtension(string extension) {
			return HasExtension(extension) ? this : new FilePath(_value + extension);
		}

		/// <summary>创建以应用程序所在目录为基准的路径。</summary>
		/// <param name="path">相对路径。</param>
		/// <returns>返回以应用程序所在目录为基准的路径。</returns>
		public static FilePath FromRoot(string path) {
			return AppRoot.Combine(path);
		}

		/// <summary>返回与文件关联的版本说明信息。</summary>
		/// <returns>文件 <see cref="FileVersionInfo"/> 对应的 <see cref="FileVersionInfo.FileDescription"/></returns>
		public string GetDescription() {
			return FileVersionInfo.GetVersionInfo(ToFullPath()).FileDescription;
		}

		/// <summary>获取当前 <see cref="FilePath"/> 下符合匹配模式的文件。在执行匹配前，先将当前实例转换为完整路径。当前用户无权访问的目录将被忽略。</summary>
		/// <param name="pattern">匹配文件用的模式。模式中的“\”用于分隔目录，“**”表示当前目录及其包含的所有目录，“*”匹配 0 到多个字符，“?”匹配 1 个字符。模式为空时，返回所有文件。</param>
		/// <returns>返回匹配模式的所有文件。</returns>
		public FilePath[] GetFiles(string pattern) {
			return GetFiles(pattern, null);
		}

		/// <summary>获取当前 <see cref="FilePath"/> 下符合匹配模式的目录。在执行匹配前，先将当前实例转换为完整路径。当前用户无权访问的目录将被忽略。</summary>
		/// <param name="pattern">匹配目录用的模式。模式中的“\”用于分隔目录，“**”表示当前目录及其包含的所有目录，“*”匹配 0 到多个字符，“?”匹配 1 个字符。模式为空时，返回所有一级子目录。</param>
		/// <returns>返回匹配模式的所有目录。</returns>
		public FilePath[] GetDirectories(string pattern) {
			return GetDirectories(pattern, null);
		}

		/// <summary>获取当前 <see cref="FilePath"/> 下符合匹配模式和筛选条件的文件。在执行匹配前，先将当前实例转换为完整路径。当前用户无权访问的目录将被忽略。</summary>
		/// <param name="pattern">匹配文件用的模式。模式中的“\”用于分隔目录，“**”表示当前目录及其包含的所有目录，“*”匹配 0 到多个字符，“?”匹配 1 个字符。</param>
		/// <param name="filter">用于筛选文件名的委托。</param>
		/// <returns>返回匹配模式的所有文件。</returns>
		public FilePath[] GetFiles(string pattern, Predicate<string> filter) {
			var f = ToFullPath();
			if (String.IsNullOrEmpty(pattern)) {
				return SysDirectory.Exists(f._value)
					? GetFiles(f._value, Wildcard, filter)
#if LIB
					: CollectionHelper.GetEmptyArray<FilePath>();
#else
					: new FilePath[0];
#endif
			}

			string fp;
			bool rp = pattern == RecursiveWildchard;
			var p = new FilePath(pattern).GetParts(false);
			var pl = p.Length;
			var t = GetDirectories(f._value, p, rp ? 1 : pl - 1);
			if (rp) {
				fp = Wildcard;
			}
			else {
				if (t.Count == 0) {
#if LIB
					return CollectionHelper.GetEmptyArray<FilePath>();
#else
					return new FilePath[0];
#endif
				}
				fp = p[pl - 1];
			}
#if LIB
			var r = new ArrayBuilder<FilePath>();
#else
			var r = new List<FilePath>();
#endif
			foreach (var item in t) {
				try {
					r.AddRange(GetFiles(item, fp, filter));
				}
				catch (UnauthorizedAccessException) {
					// continue;
				}
			}
			return r.ToArray();
		}

		static FilePath[] GetFiles(string directory, string filePattern, Predicate<string> filter) {
			return SysDirectory.Exists(directory) ? Array.ConvertAll(
				filter != null ? Array.FindAll(SysDirectory.GetFiles(directory, filePattern), filter) : SysDirectory.GetFiles(directory, filePattern)
				, i => (FilePath)i)
#if LIB
					: CollectionHelper.GetEmptyArray<FilePath>();
#else
					: new FilePath[0];
#endif
		}

		/// <summary>获取当前 <see cref="FilePath"/> 下符合匹配模式和筛选条件的目录。在执行匹配前，先将当前实例转换为完整路径。当前用户无权访问的目录将被忽略。</summary>
		/// <param name="pattern">匹配目录用的模式。模式中的“\”用于分隔目录，“**”表示当前目录及其包含的所有目录，“*”匹配 0 到多个字符，“?”匹配 1 个字符。</param>
		/// <param name="filter">用于筛选目录名的委托。</param>
		/// <returns>返回匹配模式的所有目录。</returns>
		public FilePath[] GetDirectories(string pattern, Predicate<string> filter) {
			var f = ToFullPath();
			if (String.IsNullOrEmpty(pattern)) {
				return SysDirectory.Exists(f._value)
					? GetDirectories(f._value, Wildcard, filter)
#if LIB
					: CollectionHelper.GetEmptyArray<FilePath>();
#else
					: new FilePath[0];
#endif
			}

			string fp;
			bool rp = pattern == RecursiveWildchard;
			var p = new FilePath(pattern).GetParts(false);
			var pl = p.Length;
			var t = GetDirectories(f._value, p, rp ? 1 : pl - 1);
			if (rp) {
				fp = Wildcard;
			}
			else {
				if (t.Count == 0) {
#if LIB
					return CollectionHelper.GetEmptyArray<FilePath>();
#else
					return new FilePath[0];
#endif
				}
				fp = p[p.Length - 1];
			}
#if LIB
			var r = new ArrayBuilder<FilePath>();
#else
			var r = new List<FilePath>();
#endif
			foreach (var item in t) {
				try {
					r.AddRange(GetDirectories(item, fp, filter));
				}
				catch (UnauthorizedAccessException) {
					// continue;
				}
			}
			return r.ToArray();
		}

		static FilePath[] GetDirectories(string directory, string filePattern, Predicate<string> filter) {
			return SysDirectory.Exists(directory)
					? Array.ConvertAll(filter != null ? Array.FindAll(SysDirectory.GetDirectories(directory, filePattern), filter) : SysDirectory.GetDirectories(directory, filePattern), i => (FilePath)i)
#if LIB
					: CollectionHelper.GetEmptyArray<FilePath>();
#else
					: new FilePath[0];
#endif
		}

		static NameList GetDirectories(string path, string[] parts, int partCount) {
			NameList r;
			var t = new NameList(1) { path };
			for (int i = 0; i < partCount; i++) {
				r = new NameList(10);
				var pi = parts[i];
				if (pi.Length == 0) {
					t = new NameList(1) { Path.GetPathRoot(path) };
					continue;
				}
				else if (pi == "..") {
					foreach (var item in t) {
						var n = IsDirectorySeparator(item[item.Length - 1])
							? Path.GetDirectoryName(item.Substring(0, item.Length - 1))
							: Path.GetDirectoryName(item);
						if (n != null && r.Contains(n) == false) {
							r.Add(n);
						}
					}
				}
				else if (pi == RecursiveWildchard) {
					foreach (var item in t) {
						r.Add(item);
						GetDirectoriesRecursively(item, ref r);
					}
				}
				else {
					foreach (var item in t) {
						try {
							r.AddRange(SysDirectory.GetDirectories(item, pi));
						}
						catch (UnauthorizedAccessException) {
							continue;
						}
					}
				}
				t = r;
			}
			return t;
		}

		static void GetDirectoriesRecursively(string directoryPath, ref NameList results) {
			try {
				var r = SysDirectory.GetDirectories(directoryPath, "*");
				results.AddRange(r);
				foreach (var item in r) {
					GetDirectoriesRecursively(item, ref results);
				}
			}
			catch (UnauthorizedAccessException) {
				return;
			}
		}

		/// <summary>将路径按目录拆分为多个部分，并删除其中的无效部分。</summary>
		/// <returns>目录的各个部分。</returns>
		public string[] GetParts() {
			return GetParts(true);
		}

		/// <summary>将路径按目录拆分为多个部分。</summary>
		/// <param name="removeInvalidParts">是否删除无效的部分。</param>
		/// <returns>目录的各个部分。</returns>
		public string[] GetParts(bool removeInvalidParts) {
			if (IsEmpty) {
#if LIB
				return CollectionHelper.GetEmptyArray<string>();
#else
				return new string[0];
#endif
			}
			var p = _value.Split(__PathSeparators);
			string s;
			var r = false;
			var v = 0;
			for (int i = 0; i < p.Length; i++) {
				s = p[i].Trim();
				if (s.Length == 0) {
					// 保留第一个根目录引用
					if (i == 0) {
						r = true;
						++v;
					}
					continue;
				}
				if (s.Length == 1 && s[0] == '.') {
					continue;
				}
				if (s == ".." || (s.StartsWith("..", StringComparison.Ordinal) && s.TrimEnd('.').Length == 0)) {
					// 前一级为根目录
					if (r && v == 1) {
						// 删除根目录级的目录部分
						if (p[0].Length > 2) {
							p[0] = p[0].Substring(0, 2);
						}
						continue;
					}
					// 保留0级或上一级为“..”的目录符
					if (v == 0 || p[v - 1] == "..") {
						s = "..";
						p[v] = s;
						++v;
						continue;
					}
					// 删除前一级
					--v;
					continue;
				}
				s = s.TrimEnd('.');
				if (removeInvalidParts) {
					if (i == 0) {
						if (s.Length > 1) {
							// 根目录
							if (s[1] == Path.VolumeSeparatorChar) {
								if (Array.IndexOf(InvalidFileNameChars, s[0]) != -1
									|| s.IndexOfAny(InvalidFileNameChars, 2) != -1) {
									continue;
								}
								r = true;
							}
							else if (s.IndexOfAny(InvalidFileNameChars) != -1) {
								continue;
							}
						}
					}
					else if (s.IndexOfAny(InvalidFileNameChars) != -1) {
						continue;
					}
				}
				p[v] = s;
				++v;
			}
			if (v < 1) {
#if LIB
				return CollectionHelper.GetEmptyArray<string>();
#else
				return new string[0];
#endif
			}
			if (v < p.Length) {
				Array.Resize(ref p, v);
			}
			return p;
		}

		/// <summary>
		/// <para>
		/// 以当前路径的绝对路径为基准，返回 <paramref name="path"/> 相对于当前路径的相对路径。 如果 <paramref name="path"/>
		/// 与当前路径盘符不一致，返回 <paramref name="path"/>。 在计算相对路径前，将把当前路径和 <paramref name="path"/> 使用 <see
		/// cref="ToFullPath"/> 方法转换为绝对路径。
		/// </para>
		/// <note type="note">如当前路径为目录，但不以 <see cref="Path.DirectorySeparatorChar"/> 结束，应先调用 <see
		/// cref="AppendPathSeparator"/> 方法将目录结束符附加到路径末尾。</note>
		/// </summary>
		/// <param name="path">要计算相对路径的路径。</param>
		/// <returns><paramref name="path"/> 对于当前路径的相对路径。</returns>
		public FilePath GetRelativePath(FilePath path) {
			var p1 = ToFullPath()._value;
			var p2 = path.ToFullPath()._value;
			var p = -1;
			var i = 0;
			var l1 = p1.Length;
			var l2 = p2.Length;
			while (i < l1 && i < l2) {
				var c1 = p1[i];
				var c2 = p2[i];
				if (c1 != c2 && Char.ToLowerInvariant(c1) != char.ToLowerInvariant(c2)) {
					break;
				}
				if (c1 == Path.DirectorySeparatorChar) {
					p = i;
				}
				i++;
			}
			if (i == 0) {
				return p2;
			}
			if (i == l1 && i == l2) {
				return string.Empty;
			}
			var sb = new StringBuilder(32);
			while (i < l1) {
				if (p1[i] == Path.DirectorySeparatorChar) {
					sb.Append("..").Append(Path.DirectorySeparatorChar);
				}
				i++;
			}
			if (sb.Length == 0 && l2 - 1 == p) {
				return __CurrentPath;
			}
			sb.Append(p2, p + 1, l2 - p - 1);
			return sb.ToString();
		}


		/// <summary>检查当前路径是否以指定的扩展名结束（不区分大小写）。</summary>
		/// <param name="extension">文件扩展名。</param>
		public bool HasExtension(string extension) {
			return String.IsNullOrEmpty(extension)
				|| (IsEmpty == false && _value.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
					&& (extension[0] == '.' || _value.Length > extension.Length && _value[_value.Length - extension.Length - 1] == '.')
				);
		}

		/// <summary>返回文件名是否以指定的任意一个扩展名结尾（不区分大小写）。</summary>
		/// <param name="extensions">扩展名列表。</param>
		public bool HasExtension(params string[] extensions) {
			var ext = FileExtension;
			if (extensions == null || extensions.Length == 0) {
				return true;
			}
			if (ext.Length == 0) {
				return false;
			}
			foreach (var item in extensions) {
				if (item == null || item.Length == 0) {
					continue;
				}
				if (ext.EndsWith(item, StringComparison.OrdinalIgnoreCase)
					&& (item[0] == '.' || ext.Length > item.Length && ext[ext.Length - item.Length - 1] == '.')) {
					return true;
				}
			}
			return false;
		}

		/// <summary>检查当前路径是否属于指定的路径（子目录或其下文件）。</summary>
		/// <param name="containingPath">上级目录。</param>
		/// <param name="rootAsRelative">是否将当前目录以 <see cref="Path.DirectorySeparatorChar"/> 开头的情况视为相对路径。</param>
		public bool IsInDirectory(FilePath containingPath, bool rootAsRelative) {
			var p = containingPath.ToFullPath()._value;
			var v = ToFullPath()._value;
			return v.StartsWith(p, StringComparison.OrdinalIgnoreCase)
				&& (IsDirectorySeparator(p[p.Length - 1]) || v.Length > p.Length && IsDirectorySeparator(v[p.Length]) && new FilePath(v.Substring(p.Length)).IsSubPath(rootAsRelative));
		}

		/// <summary>返回指定的字符是否 <see cref="Path.DirectorySeparatorChar"/> 或 <see cref="Path.AltDirectorySeparatorChar"/>。</summary>
		/// <param name="ch">要检查的字符。</param>
		static bool IsDirectorySeparator(char ch) {
			return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar;
		}

		/// <summary>返回当前路径是否为子路径（不会指向当前目录的上级目录）。</summary>
		/// <param name="rootAsRelative">是否将目录以 <see cref="Path.DirectorySeparatorChar"/> 开头的情况视为子路径。</param>
		public bool IsSubPath(bool rootAsRelative) {
			if (String.IsNullOrEmpty(_value)) {
				return true;
			}
			int i, n;
			if (IsDirectorySeparator(_value[i = 0])) {
				// rooted
				if (rootAsRelative) {
					if (_value.Length == 1) {
						return true;
					}
					i = 1;
				}
				else {
					return false;
				}
			}

			if (_value.Length >= 2 && (_value[1] == Path.VolumeSeparatorChar || _value == ".." || _value.Contains("..."))) {
				// rooted, or starts with "..", or contains "..."
				return false;
			}
			int d = 0;
			while ((n = _value.IndexOfAny(__PathSeparators, i)) >= 0) {
				var p = _value.Substring(i, n - i).Trim();
				if (p.Length == 0 // treat double separators as rooted
					|| p == ".." && --d < 0) { // ".." points to parent folder
					return false;
				}
				else if (p.Length != 1 || p[0] != '.') { // ignore "."
					++d;
				}
				i = n + 1;
			}
			return n != -1
				|| _value.Substring(i).TrimStart() != ".."
				|| --d >= 0; // not end with ".."
		}

		static readonly string __DirectorySeparator = Path.DirectorySeparatorChar.ToString();

		/// <summary>将文件路径转换为绝对定位路径，并删除目录名称中的空白。同时将 <see cref="Path.AltDirectorySeparatorChar"/> 转换为 <see cref="Path.DirectorySeparatorChar"/>。</summary>
		/// <returns>标准的绝对定位路径。</returns>
		public FilePath Normalize() {
			if (_value.IsNullOrWhiteSpace()) {
				return AppRoot;
			}
			if (_value.Length == 3 && _value[1] == Path.VolumeSeparatorChar && IsDirectorySeparator(_value[2])) {
				return this;
			}
			var p = GetParts();
			// fixes "?:\" (where ? is active directory drive letter) becomes active directory
			if (EndsWithPathSeparator && p.Length > 0) {
				p[p.Length - 1] += Path.DirectorySeparatorChar;
			}
			return p.Length == 1 && p[0].Length == 3 && p[0][1] == Path.VolumeSeparatorChar
				? new FilePath(p[0] + Path.DirectorySeparatorChar)
				: new FilePath(Path.GetFullPath(AppRoot.Combine(string.Join(__DirectorySeparator, p))._value));
		}

		/// <summary>只读打开文件路径对应的文件，允许读写共享。</summary>
		public Stream OpenFileReader() {
			return new FileStream(ToFullPath()._value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
		/// <summary>打开文件路径对应的文件以准备读写，允许读取共享。</summary>
		/// <param name="overwrite">指定如文件存在时是否创建新的文件（源文件将被 0 长度的文件覆盖）。</param>
		public Stream OpenFileWriter(bool overwrite) {
			return new FileStream(ToFullPath()._value, overwrite ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
		}

		/// <summary>返回读写文件的 <see cref="Stream"/>。</summary>
		/// <inheritdoc cref="OpenFile(FileMode, FileAccess, FileShare, int)"/>
		public Stream OpenFile(FileMode mode, FileAccess access, FileShare share) {
			return new FileStream(ToFullPath()._value, mode, access, share);
		}

		/// <summary>返回读写文件的 <see cref="Stream"/>。</summary>
		/// <param name="mode">指定文件访问方式。</param>
		/// <param name="access">指定文件读写方式。</param>
		/// <param name="share">指定文件访问共享方式。</param>
		/// <param name="bufferSize">读写缓冲区的尺寸。</param>
		public Stream OpenFile(FileMode mode, FileAccess access, FileShare share, int bufferSize) {
			return new FileStream(ToFullPath()._value, mode, access, share, bufferSize);
		}

		/// <summary>创建以指定编码读取文件的 <see cref="StreamReader"/> 实例。</summary>
		/// <param name="encoding">用于读取文件的编码。编码为 null 时采用 UTF-8 编码。</param>
		/// <returns>读取文件的 <see cref="StreamReader"/> 实例。</returns>
		public StreamReader OpenTextReader(Encoding encoding) {
			return new StreamReader(ToFullPath ()._value, encoding ?? Encoding.UTF8, true);
		}

		/// <summary>创建以指定编码写入文件的 <see cref="StreamWriter"/> 实例。</summary>
		/// <param name="append">是否追加到文件结尾。</param>
		/// <param name="encoding">用于写入文件的编码。编码为 null 时采用 UTF-8 编码。</param>
		/// <returns>写入文件的 <see cref="StreamWriter"/> 实例。</returns>
		public StreamWriter OpenTextWriter(bool append, Encoding encoding) {
			var fp = ToFullPath();
			fp.CreateContainingDirectory();
			return new StreamWriter(fp._value, append, encoding ?? Encoding.UTF8);
		}

		/// <summary>打开当前路径对应的文件并读取所有内容为字节数组。如文件不存在，返回 0 长度的字节数组。此方法使用 FileStream 读取文件，打开或读取文件过程中可能返回异常。</summary>
		/// <returns>文件的字节数组。</returns>
		public byte[] ReadAllBytes() { return ReadAllBytes(-1); }
		/// <summary>打开当前路径对应的文件并读取所有内容为字节数组。如文件不存在，返回 0 长度的字节数组。此方法使用 FileStream 读取文件，打开或读取文件过程中可能返回异常。</summary>
		/// <param name="maxBytes">允许读取的最大字节数。如此值非正整数，则按读取文件的大小读取最多 <see cref="int.MaxValue"/> 个字节。</param>
		/// <returns>文件的字节数组。</returns>
		public byte[] ReadAllBytes(int maxBytes) {
			if (ExistsFile == false) {
#if LIB
				return CollectionHelper.GetEmptyArray<byte>();
#else
				return new byte[0];
#endif
			}
			using (var s = new FileStream(ToFullPath ()._value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				if (s.CanRead == false) {
#if LIB
					return CollectionHelper.GetEmptyArray<byte>();
#else
					return new byte[0];
#endif
				}
				var l = s.Length;
				var r = new byte[maxBytes < 1 || maxBytes > l ? l : maxBytes];
				s.Read(r, 0, r.Length);
				return r;
			}
		}

		/// <summary>打开当前路径对应的文件，并以指定编码逐行读取所有行。如文件不存在，返回 <see cref="String.Empty"/>。</summary>
		/// <param name="encoding">用于读取文件的编码。编码为 <see langword="null"/> 时采用 UTF-8 编码。</param>
		/// <returns>包含整个文本文件的字符串。</returns>
		/// <exception cref="PathTooLongException">指定的路径、文件名或者两者都超出了系统定义的最大长度。例如，在基于 Windows 的平台上，路径必须小于 248 个字符，文件名必须小于 260 个字符。</exception>
		/// <exception cref="UnauthorizedAccessException">调用方没有所要求的权限。</exception>
		/// <exception cref="System.Security.SecurityException">调用方没有所要求的权限。</exception>
		public string ReadTextFile(Encoding encoding) {
			return ExistsFile ? File.ReadAllText(ToFullPath()._value, encoding ?? Encoding.UTF8) : String.Empty;
		}

		/// <summary>打开当前路径对应的文件，并以指定编码逐行读取所有内容为字符串集合。如文件不存在，返回 0 长度的字符串数组。</summary>
		/// <param name="encoding">用于读取文件的编码。编码为 null 时采用 UTF-8 编码。</param>
		/// <returns>文件中每行对应一个字符串所构成的集合。</returns>
		[Obsolete("请改用 ReadLines")]
		public IEnumerable<string> ReadAllText(Encoding encoding) {
			return ReadLines(encoding);
		}

		/// <summary>打开当前路径对应的文件，并以指定编码逐行读取所有内容为字符串集合。如文件不存在，返回 0 长度的字符串数组。</summary>
		/// <param name="encoding">用于读取文件的编码。编码为 <see langword="null"/> 时采用 UTF-8 编码。</param>
		/// <returns>文件中每行对应一个字符串所构成的集合。</returns>
		public IEnumerable<string> ReadLines(Encoding encoding) {
			if (ExistsFile == false) {
#if LIB
				return CollectionHelper.GetEmptyArray<string>();
#else
				return new string[0];
#endif
			}
#if !NET40
			return new ReadLinesIterator(ToFullPath()._value, encoding ?? Encoding.UTF8);
#else
			return File.ReadLines(ToFullPath()._value, encoding ?? Encoding.UTF8);
#endif
		}

		/// <summary>将 <paramref name="bytes"/>写入文件。</summary>
		/// <param name="append">是否追加到文件结尾。</param>
		/// <param name="bytes">需要写入的字节数组。此参数为空时，不写入文件内容。</param>
		public void WriteAllBytes(bool append, byte[] bytes) {
			if (bytes == null) {
				return;
			}
			var fp = ToFullPath();
			fp.CreateContainingDirectory();
			using (var s = new FileStream(fp._value, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)) {
				s.Write(bytes, 0, bytes.Length);
			}
		}

		/// <summary>将 <paramref name="text"/> 以指定编码写入文件。</summary>
		/// <param name="append">是否追加到文件结尾。</param>
		/// <param name="encoding">用于写入文件的编码。编码为 <see langword="null"/> 时采用 UTF-8 编码。</param>
		/// <param name="text">需要写入的文本。此参数为空时，不写入文件内容。</param>
		public void WriteAllText(bool append, Encoding encoding, string text) {
			if (text == null) {
				return;
			}
			using (var w = OpenTextWriter(append, encoding)) {
				w.Write(text);
			}
		}

		/// <summary>将 <paramref name="lines"/> 的每项内容后附加换行，以指定编码写入文件。</summary>
		/// <param name="append">是否追加到文件结尾。</param>
		/// <param name="encoding">用于写入文件的编码。编码为 <see langword="null"/> 时采用 UTF-8 编码。</param>
		/// <param name="lines">需要写入的文本。此参数为 <see langword="null"/> 时，不写入文件内容。此参数的项为 <see langword="null"/> 时，写入对应的空行。</param>
		public void WriteAllLines(bool append, Encoding encoding, IEnumerable<string> lines) {
			if (lines == null) {
				return;
			}
			using (var w = OpenTextWriter(append, encoding)) {
				foreach (var item in lines) {
					w.WriteLine(item);
				}
			}
		}

		/// <inheritdoc cref="WriteAllLines(bool, Encoding, IEnumerable{string})"/>
		public void WriteAllLines(bool append, Encoding encoding, params string[] lines) {
			WriteAllLines(append, encoding, (IEnumerable<string>)lines);
		}

		/// <summary>使用关联的程序打开当前路径对应的文件或目录，返回对应的进程。</summary>
		/// <param name="arguments">要传递的额外参数。</param>
		/// <returns><see cref="Process"/> 实例。</returns>
		public Process StartProcess(string arguments) {
			return Process.Start(_value, arguments);
		}

		/// <summary>使用关联的程序打开当前路径对应的文件或目录，返回对应的进程。</summary>
		/// <returns><see cref="Process"/> 实例。</returns>
		public Process StartProcess() {
			return Process.Start(_value);
		}

		/// <summary>将路径中的无效字符替换为 <paramref name="substitution"/>。</summary>
		/// <param name="substitution">用于替换无效字符的字符。</param>
		/// <returns>替换了无效字符的路径。</returns>
		public FilePath SubstituteInvalidChars(char substitution) {
			if (IsEmpty) {
				return Empty;
			}
			var a = _value.ToCharArray();
			var r = false;
			for (int i = 0; i < a.Length; i++) {
				ref var c = ref a[i];
				if (Array.IndexOf(InvalidFileNameChars, c) != -1) {
					c = substitution;
					r = true;
				}
			}
			if (r) {
				return new FilePath(new string(a));
			}
			return this;
		}

		/// <summary>将路径转换为绝对定位的路径。路径的基准位置为 <see cref="AppRoot"/>。执行此方法前，必须确保路径中不包含无效字符，否则将抛出异常。</summary>
		/// <returns>采用绝对定位路径的实例。</returns>
		/// <exception cref="ArgumentException">路径无效。</exception>
		public FilePath ToFullPath() {
#if LIB
			return Path.GetFullPath(FileHelper.CombinePath(AppRoot._value, _value));
#else
			return Path.GetFullPath(Path.Combine(AppRoot._value, _value));
#endif
		}

		/// <summary>
		/// <para>将 <see cref="FilePath"/> 实例转换为完全路径，再隐式转换为 <see cref="FileInfo"/> 实例。路径的基准位置为 <see cref="AppRoot"/>。</para>
		/// <note type="note">事实上，<see cref="FilePath"/> 实例可隐式转换为 <see cref="FileInfo"/> 实例。</note>
		/// </summary>
		/// <returns>将当前路径转换为完全路径后对应的 <see cref="FileInfo"/> 实例。</returns>
		[DebuggerStepThrough]
		public FileInfo ToFileInfo() {
			return this;
		}

		/// <summary>将 <see cref="FilePath"/> 实例转换为完全路径，再获取其对应的 <see cref="FileVersionInfo"/>。</summary>
		/// <returns>与完全路径对应的文件版本信息。</returns>
		/// <exception cref="ArgumentException">路径无效。</exception>
		/// <exception cref="FileNotFoundException">找不到对应的文件。</exception>
		[DebuggerStepThrough]
		public FileVersionInfo ToFileVersionInfo() {
			return FileVersionInfo.GetVersionInfo(ToFullPath());
		}

		/// <summary>
		/// 将 <see cref="FilePath"/> 实例转换为完全路径，再隐式转换为 <see cref="DirectoryInfo"/> 实例。路径的基准位置为 <see cref="AppRoot"/>。
		/// <note type="note">事实上，<see cref="FilePath"/> 实例可隐式转换为 <see cref="DirectoryInfo"/> 实例。</note>
		/// </summary>
		/// <returns>将当前路径转换为完全路径后对应的 <see cref="DirectoryInfo"/> 实例。</returns>
		[DebuggerStepThrough]
		public DirectoryInfo ToDirectoryInfo() {
			return this;
		}

		/// <summary>
		/// 将 <see cref="FilePath"/> 实例转换为 <see cref="Uri"/> 实例。
		/// <note type="note">事实上，<see cref="FilePath"/> 实例可隐式转换为 <see cref="Uri"/> 实例。</note>
		/// </summary>
		/// <returns>与当前路径对应的 <see cref="Uri"/> 实例。</returns>
		[DebuggerStepThrough]
		public Uri ToUri() { return this; }

		#region 类型映射

		/// <summary>将字符串隐式转换为 <see cref="FilePath"/> 实例，删除传入字符串内所有的前导和尾随空白。</summary>
		/// <param name="path">需要转换的路径字符串。</param>
		/// <returns><see cref="FilePath"/> 实例。</returns>
		[DebuggerStepThrough]
		public static implicit operator FilePath(string path) {
			return new FilePath(path);
		}

		/// <summary>将 <see cref="FilePath"/> 实例隐式转换为字符串。</summary>
		/// <param name="path">需要转换的路径。</param>
		/// <returns>以字符串形式表示的实例。</returns>
		[DebuggerStepThrough]
		public static implicit operator string(FilePath path) {
			return path._value;
		}

		/// <summary>将 <see cref="FileInfo"/> 显式转换为 <see cref="FilePath"/> 实例。</summary>
		/// <param name="file">需要转换的路径。</param>
		[DebuggerStepThrough]
		public static explicit operator FilePath(FileInfo file) {
			return file == null ? Empty : new FilePath(file.FullName, false);
		}

		/// <summary>
		/// 将 <see cref="FilePath"/> 实例转换为完全路径，再隐式转换为 <see cref="FileInfo"/> 实例。路径的基准位置为 <see cref="AppRoot"/>。
		/// </summary>
		/// <param name="path">需要转换的路径。</param>
		/// <returns>将当前路径转换为完全路径后对应的 <see cref="FileInfo"/> 实例。</returns>
		/// <seealso cref="ToFullPath"/>
		[DebuggerStepThrough]
		public static implicit operator FileInfo(FilePath path) {
			return new FileInfo(path.ToFullPath()._value);
		}

		/// <summary>将 <see cref="DirectoryInfo"/> 显式转换为 <see cref="FilePath"/> 实例。</summary>
		/// <param name="directory">需要转换的路径。</param>
		[DebuggerStepThrough]
		public static explicit operator FilePath(DirectoryInfo directory) {
			return directory == null ? Empty : new FilePath(directory.FullName, false);
		}

		/// <summary>
		/// 将 <see cref="FilePath"/> 实例转换为完全路径，再隐式转换为 <see cref="DirectoryInfo"/> 实例。路径的基准位置为 <see cref="AppRoot"/>。
		/// </summary>
		/// <param name="path">需要转换的路径。</param>
		/// <returns>将当前路径转换为完全路径后对应的 <see cref="DirectoryInfo"/> 实例。</returns>
		/// <seealso cref="ToFullPath"/>
		[DebuggerStepThrough]
		public static implicit operator DirectoryInfo(FilePath path) {
			return new DirectoryInfo(path.ToFullPath()._value);
		}

		/// <summary>将 <see cref="FilePath"/> 实例隐式转换为 <see cref="Uri"/> 实例。</summary>
		/// <param name="path">需要转换的路径。</param>
		[DebuggerStepThrough]
		public static implicit operator Uri(FilePath path) {
			return new Uri(path._value);
		}
#endregion

#region IEquatable<FilePath> 实现

		/// <summary>比较两个文件路径是否相同。</summary>
		/// <param name="path1">需要比较的第一个路径。</param>
		/// <param name="path2">需要比较的第二个路径。</param>
		/// <returns>相同时，返回 true。</returns>
		[DebuggerStepThrough]
		public static bool operator ==(FilePath path1, FilePath path2) {
			return path1.Equals(path2);
		}

		/// <summary>比较两个文件路径是否不相同。</summary>
		/// <param name="path1">需要比较的第一个路径。</param>
		/// <param name="path2">需要比较的第二个路径。</param>
		/// <returns>不相同时，返回 true。</returns>
		[DebuggerStepThrough]
		public static bool operator !=(FilePath path1, FilePath path2) {
			return !path1.Equals(path2);
		}

		/// <summary>指示当前文件路径是否等于同一类型的另一个文件路径。</summary>
		/// <param name="other">与此对象进行比较的对象。</param>
		/// <returns>如果当前对象等于 <paramref name="other"/> 参数，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
		public bool Equals(FilePath other) {
			return __PathComparer(_value, other._value)
				|| __PathComparer(
					Path.Combine(AppRoot._value, _value ?? string.Empty),
					Path.Combine(AppRoot._value, other._value ?? string.Empty));
		}

		/// <summary>确定当前文件路径是否与另一个实例相等。</summary>
		/// <param name="obj">需要与当前实例比较的对象。</param>
		/// <returns>在两个文件路径相等时，返回 true。</returns>
		[DebuggerStepThrough]
		public override bool Equals(object obj) {
			return obj is FilePath && Equals((FilePath)obj);
		}

		/// <summary>返回路径字符串的散列值。</summary>
		/// <returns>路径字符串的散列值。</returns>
		[DebuggerStepThrough]
		public override int GetHashCode() {
			return _value == null ? 0 : _value.GetHashCode();
		}
#endregion

		/// <summary>返回表示当前文件路径的 <see cref="string"/> 实例。</summary>
		/// <returns>表示当前文件路径的 <see cref="string"/> 实例。</returns>
		[DebuggerStepThrough]
		public override string ToString() {
			return _value ?? string.Empty;
		}

		// see: https://www.codeproject.com/Articles/1383832/System-IO-Directory-Alternative-using-WinAPI
		static class FileSystemEnumerator
		{
			const int Dir = 1, File = 2;

			static IEnumerable<string> GetAll(string path, string searchPattern, SearchOption searchOption) {
				return Enumerate(path, searchPattern, searchOption, 0);
			}
			static IEnumerable<string> GetFile(string path, string searchPattern, SearchOption searchOption) {
				return Enumerate(path, searchPattern, searchOption, File);
			}
			static IEnumerable<string> GetDir(string path, string searchPattern, SearchOption searchOption) {
				return Enumerate(path, searchPattern, searchOption, Dir);
			}
			static IEnumerable<string> Enumerate(string path, string searchPattern, SearchOption searchOption, int entryType) {
				WIN32_FIND_DATA findData;
				if (path[path.Length - 1] != Path.DirectorySeparatorChar) {
					path += Path.DirectorySeparatorChar;
				}
				var hFile = FindFirstFile(path + searchPattern, out findData);
				try {
					if (hFile.ToInt32() == -1) {
						yield break;
					}
					NameList subDirs = new NameList();
					do {
						if (findData.cFileName == "." || findData.cFileName == "..") {
							continue;
						}
						if ((findData.dwFileAttributes & (uint)FileAttributes.Directory) == (uint)FileAttributes.Directory) {
							subDirs.Add(path + findData.cFileName);
							if (entryType != File) {
								yield return path + findData.cFileName;
							}
						}
						else if (entryType != Dir) {
							yield return path + findData.cFileName;
						}
					} while (FindNextFile(hFile, out findData));
					if (subDirs.Count > 0 && searchOption == SearchOption.AllDirectories) {
						foreach (string subdir in subDirs) {
							foreach (var file in Enumerate(subdir, searchPattern, searchOption, entryType)) {
								yield return file;
							}
						}
					}
				}
				finally {
					FindClose(hFile);
				}
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
			struct WIN32_FIND_DATA
			{
				public uint dwFileAttributes;
				public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
				public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
				public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
				public uint nFileSizeHigh;
				public uint nFileSizeLow;
				public uint dwReserved0;
				public uint dwReserved1;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
				public string cFileName;
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
				public string cAlternateFileName;
			}

			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			private static extern bool FindClose(IntPtr hFindFile);

			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			private static extern IntPtr FindFirstFile
			  (string lpFileName, out WIN32_FIND_DATA lpFindFileData);

			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			private static extern bool FindNextFile
			  (IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
		}

#if NET20
		sealed class ReadLinesIterator : IEnumerable<string>, IEnumerator<string>
		{
			readonly string _path;
			readonly Encoding _encoding;
			StreamReader _reader;
			string _current;

			public ReadLinesIterator(string path, Encoding encoding) {
				_path = path;
				_encoding = encoding;
			}

			public bool MoveNext() {
				if (_reader == null) {
					return false;
				}
				_current = _reader.ReadLine();
				if (_current != null) {
					return true;
				}
				Dispose(true);
				return false;
			}

			private void Dispose(bool disposing) {
				try {
					if (disposing && _reader != null) {
						_reader.Dispose();
					}
				}
				finally {
					_reader = null;
				}
			}

			public IEnumerator<string> GetEnumerator() {
				return new ReadLinesIterator(_path, _encoding) { _reader = new StreamReader(_path, _encoding) };
;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public string Current {
				get { return _current; }
			}

			public void Dispose() {
				Dispose(true);
			}

			object System.Collections.IEnumerator.Current {
				get { return _current; }
			}

			public void Reset() {
				if (_reader != null) {
					_reader.Dispose();
				}
				_reader = new StreamReader(_path, _encoding);
				_current = null;
			}
		}
#endif
	}
}