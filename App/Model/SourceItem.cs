using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	public abstract class SourceItem
	{
		List<SourceItem> _Items;

		public FilePath FilePath { get; }
		public string FileName { get; }
		public string FolderName { get; }
		public BookmarkSettings Bookmark { get; set; }
		public int PageCount { get; private set; }
		public abstract int FileSize { get; }
		public abstract DateTime FileTime { get; }
		public List<SourceItem> Items {
			get {
				if (_Items == null) {
					_Items = new List<SourceItem>();
				}
				return _Items;
			}
		}

		public bool HasSubItems => _Items.HasContent();

		public abstract ItemType Type { get; }
		public static void SortFileList(string[] fileList) {
			if (AppContext.Merger.CajSort && CajSort(fileList)) {
				return;
			}

			if (AppContext.Merger.NumericAwareSort) {
				Array.Sort(fileList, FileHelper.NumericAwareComparePath);
			}
			else {
				Array.Sort(fileList, StringComparer.OrdinalIgnoreCase);
			}
		}

		public void SortItems(SortType options, bool recursive) {
			if (_Items.HasContent() == false) {
				return;
			}
			switch (options) {
				case SortType.Literal:
					_Items.Sort((x, y) => String.Compare(x.FilePath, y.FilePath, StringComparison.OrdinalIgnoreCase));
					break;
				case SortType.NumericAwareSort:
					_Items.Sort((x, y) => FileHelper.NumericAwareComparePath(x.FilePath, y.FilePath));
					break;
				case SortType.CajSort:
					if (CajSort(_Items) == false) {
						goto case SortType.NumericAwareSort;
					}
					break;
				case SortType.FileTime:
					_Items.Sort((x, y) => x.FileTime.CompareTo(y.FileTime));
					break;
				case SortType.Reverse:
					_Items.Reverse();
					break;
			}
			if (recursive == false) {
				return;
			}
			foreach (var item in _Items) {
				if (item._Items.HasContent()) {
					item.SortItems(options, recursive);
				}
			}
		}

		public abstract SourceItem Clone();

		public override string ToString() {
			return FilePath.ToString();
		}

		public enum ItemType
		{
			Empty, Pdf, Image, Folder
		}
		public enum SortType {
			Undefined,
			Literal,
			NumericAwareSort,
			CajSort,
			FileTime,
			Reverse
		}

		/// <summary>
		/// 创建新的空白页。
		/// </summary>
		/// <returns>空白页实例。</returns>
		internal static Empty Create() {
			return new Empty();
		}

		internal static SourceItem Create(string path) {
			return Create(path, true);
		}

		/// <summary>
		/// 根据传入的文件路径创建 <see cref="SourceItem"/> 实例。
		/// </summary>
		/// <param name="path">文件或文件夹路径。</param>
		/// <param name="refresh">是否强制更新。</param>
		/// <returns><see cref="SourceItem"/> 实例。</returns>
		internal static SourceItem Create(FilePath path, bool refresh) {
			if (((string)path).IsNullOrWhiteSpace()) {
				return Create();
			}
			if (path.ExistsDirectory) {
				return new Folder(path.ToString(), refresh);
			}
			if (path.HasExtension(Constants.FileExtensions.Pdf)) {
				try {
					var reader = Processor.PdfHelper.OpenPdfFile(path.ToString(), true, false);
					var c = reader.NumberOfPages;
					string r = refresh ? new PageRange(1, c).ToString() : null;
					var info = Processor.DocInfoExporter.RewriteDocInfoWithEncoding(reader, AppContext.Encodings.DocInfoEncoding);
					reader.Close();
					return new Pdf(path, r, c, info);
				}
				catch (FileNotFoundException) {
					FormHelper.ErrorBox(String.Concat("找不到文件：“", path, "”。"));
				}
				catch (Exception) {
					FormHelper.ErrorBox(String.Concat("打开 PDF 文件“", path, "”时出错。"));
					// ignore corrupted 
				}
				return Create();
			}
			if (path.HasExtension(Constants.FileExtensions.AllSupportedImageExtension)) {
				return new Image(path);
				//try {
				//    using (var i = new FreeImageAPI.FreeImageBitmap (path, (FreeImageAPI.FREE_IMAGE_LOAD_FLAGS)0x0800/*仅加载图像尺寸信息*/)) {
				//        var fc = i.FrameCount;
				//        return new Image (path);
				//    }
				//}
				//catch (Exception) {
				//    Common.FormHelper.ErrorBox (String.Concat ("不支持图片文件“", path, "”。"));
				//    // ignore unsupported images
				//}
			}
			FormHelper.ErrorBox(String.Concat("不支持文件“", path, "”。"));
			return Create();
		}

		internal string GetInfoFileName() {
			// 优先采用与输入文件同名的 XML 信息文件
			var f = new FilePath(FileHelper.CombinePath(FolderName, Path.ChangeExtension(FileName, Constants.FileExtensions.Xml)));
			if (f.ExistsFile == false) {
				// 次之采用与输入文件同名的 TXT 信息文件
				f = f.ChangeExtension(Constants.FileExtensions.Txt);
				if (f.ExistsFile == false) {
					// 次之采用同一个信息文件
					f = FilePath.ChangeExtension(Constants.FileExtensions.Xml);
					if (f.ExistsFile == false) {
						f = FilePath.Empty;
					}
				}
			}
			return f.ToString();
		}

		internal string GetTargetPdfFileName(string targetPath) {
			return FileHelper.HasFileNameMacro(targetPath)
				? targetPath
				: FileHelper.CombinePath(Path.GetDirectoryName(targetPath), FilePath.FileName);
		}

		internal sealed class Empty : SourceItem
		{
			readonly DateTime _Time = DateTime.Now;

			public override ItemType Type => ItemType.Empty;

			public override int FileSize => 0;
			public override DateTime FileTime => _Time;

			public void SetPageCount(int pageCount) {
				PageCount = pageCount;
			}

			public override string ToString() {
				return "<空白页>";
			}

			public override SourceItem Clone() {
				var n = new Empty();
				CopyProperties(n);
				return n;
			}

			internal Empty() : base(null, 1) { }
			internal Empty(int pageCount) : base(null, pageCount) { }
		}

		internal sealed class CropOptions
		{
			public int Left { get; set; }
			public int Right { get; set; }
			public int Top { get; set; }
			public int Bottom { get; set; }
			public int MinHeight { get; set; }
			public int MinWidth { get; set; }

			public bool NeedCropping => Left > 0 || Right > 0 || Top > 0 || Bottom > 0;

			public bool Equals(CropOptions i) {
				return Top == i.Top && Bottom == i.Bottom && Left == i.Left && Right == i.Right &&
					MinHeight == i.MinHeight && MinWidth == i.MinWidth;
			}
			public CropOptions Clone() {
				return (CropOptions)MemberwiseClone();
			}
		}

		internal sealed class Image : SourceItem
		{
			readonly int _FileSize = -1;
			readonly DateTime _FileTime;

			public Image(FilePath path)
				: base(path, 0) {
				Cropping = new CropOptions();
				GetFileInfo(path, out _FileSize, out _FileTime);
			}

			public CropOptions Cropping { get; set; }
			public override ItemType Type => ItemType.Image;
			public override int FileSize => _FileSize;
			public override DateTime FileTime => _FileTime;

			public override SourceItem Clone() {
				return new Image(FilePath) {
					Cropping = Cropping.Clone()
				};
			}
		}

		internal sealed class Pdf : SourceItem
		{
			int _FileSize = -1;
			DateTime _FileTime;

			public Pdf(FilePath path, string pageRanges, int pageCount, Model.GeneralInfo docInfo)
				: base(path, pageCount) {
				PageRanges = pageRanges;
				DocInfo = docInfo;
				ExtractImageOptions = new ImageExtracterOptions() {
					OutputPath = Path.GetDirectoryName(path.ToString()),
					ExtractAnnotationImages = false,
					MergeJpgToPng = true,
					MergeImages = true,
					MinWidth = 50,
					MinHeight = 50
				};
				GetFileInfo(path, out _FileSize, out _FileTime);
			}

			public Pdf(FilePath path) : base(path, 0) {
				Refresh(path.ToString(), AppContext.Encodings.DocInfoEncoding);
			}

			public string PageRanges { get; set; }
			public bool ImportImagesOnly { get; set; }
			public ImageExtracterOptions ExtractImageOptions { get; private set; }
			public Model.GeneralInfo DocInfo { get; private set; }
			public override ItemType Type => ItemType.Pdf;
			public override int FileSize => _FileSize;
			public override DateTime FileTime => _FileTime;

			public void Refresh(Encoding encoding) {
				Refresh(FilePath.ToString(), encoding);
			}

			public override string ToString() {
				return FilePath.IsEmpty ? String.Empty :
					String.IsNullOrEmpty(PageRanges) ? (string)FilePath :
					String.Concat(FilePath, "::", PageRanges);
			}

			public override SourceItem Clone() {
				var n = new Pdf(FilePath, PageRanges, PageCount, DocInfo) {
					ImportImagesOnly = ImportImagesOnly
				};
				CopyProperties(n);
				return n;
			}

			private void Refresh(string path, Encoding encoding) {
				try {
					GetFileInfo(path, out _FileSize, out _FileTime);
					if (_FileSize > 0) {
						using (var reader = Processor.PdfHelper.OpenPdfFile(path, true, false)) {
							DocInfo = Processor.DocInfoExporter.RewriteDocInfoWithEncoding(reader, encoding);
							PageCount = reader.NumberOfPages;
							PageRanges = new PageRange(1, PageCount).ToString();
						}
					}
				}
				catch (Exception) {
					FormHelper.ErrorBox(String.Concat("打开 PDF 文件时“", path, "”出错。"));
					// ignore corrupted 
				}
			}
		}

		internal sealed class Folder : SourceItem
		{
			DateTime _FolderTime;

			public Folder(string path) : base(path, 0) {
				var p = new FilePath(path);
				if (p.ExistsDirectory) {
					_FolderTime = p.ToDirectoryInfo().LastWriteTime;
				}
			}
			public Folder(string path, bool loadSubItems)
				: this(path) {
				if (loadSubItems) {
					Reload();
				}
			}

			public override ItemType Type => ItemType.Folder;

			public override int FileSize => 0;
			public override DateTime FileTime => _FolderTime;

			public void Reload() {
				Items.Clear();
				if (!FilePath.ExistsDirectory) {
					return;
				}

				var p = FilePath.ToString();
				var l = Items;
				switch (AppContext.Merger.SubFolder) {
					case MergerOptions.SubFolderPosition.BeforeFiles:
						AddSubDirectories(p, l);
						AddFiles(p, l);
						break;
					case MergerOptions.SubFolderPosition.WithFiles:
						AddSubDirectoriesAndFiles(p, l);
						break;
					case MergerOptions.SubFolderPosition.Exclude:
						AddFiles(p, l);
						break;
				}
			}

			public override SourceItem Clone() {
				var n = new Folder(FilePath.ToString());
				CopyProperties(n);
				return n;
			}

			private static void AddSubDirectoriesAndFiles(string folderPath, List<SourceItem> list) {
				var fl = Array.FindAll(Directory.GetFiles(folderPath), (i) => {
					var ext = Path.GetExtension(i).ToLowerInvariant();
					return Constants.FileExtensions.Pdf == ext
						|| Constants.FileExtensions.AllSupportedImageExtension.Contains(ext);
				});
				var d = Array.ConvertAll(Directory.GetDirectories(folderPath), (i) => i + "\\");
				var s = new string[fl.Length + d.Length];
				Array.Copy(fl, s, fl.Length);
				Array.Copy(d, 0, s, fl.Length, d.Length);
				SortFileList(s);
				foreach (var item in s) {
					if (item[item.Length - 1] == '\\') {
						list.Add(new Folder(item.Substring(0, item.Length - 1), true));
					}
					else {
						list.Add(Create(item));
					}
				}
			}

			static void AddFiles(string folderPath, List<SourceItem> list) {
				try {
					var fl = Directory.GetFiles(folderPath);
					SortFileList(fl);
					foreach (var item in fl) {
						var ext = Path.GetExtension(item).ToLowerInvariant();
						if (Constants.FileExtensions.Pdf == ext
							|| Constants.FileExtensions.AllSupportedImageExtension.Contains(ext)) {
							list.Add(Create(item));
						}
					}
				}
				catch (UnauthorizedAccessException) { }
				catch (IOException) { }
			}

			static void AddSubDirectories(string folderPath, List<SourceItem> list) {
				try {
					foreach (var item in Directory.EnumerateDirectories(folderPath)) {
						var f = new Folder(item, true);
						list.Add(f);
					}
				}
				catch (UnauthorizedAccessException) { }
				catch (IOException) { }
			}
		}

		protected SourceItem(FilePath path, int pageCount) {
			PageCount = pageCount;
			if (!path.IsValidPath) {
				return;
			}

			FilePath = path;
			FileName = path.FileName;
			FolderName = path.Directory;
			if (AppContext.Merger.AutoBookmarkTitle == false) {
				return;
			}
			var t = path.ExistsDirectory ? FileName : path.FileNameWithoutExtension;
			if (t.Length > 0) {
				Bookmark = CreateBookmarkSettings(t);
			}
		}

		protected static void GetFileInfo(FilePath fileName, out int kilobytes, out DateTime fileTime) {
			if (fileName.ExistsFile == false) {
				kilobytes = 0;
				fileTime = DateTime.MinValue;
			}
			else {
				var f = fileName.ToFileInfo();
				kilobytes = (int)(f.Length >> 10);
				fileTime = f.LastWriteTime;
			}
		}

		protected virtual void CopyProperties(SourceItem target) {
			target._Items = new List<SourceItem>(HasSubItems ? Items.Count : 0);
			if (HasSubItems) {
				foreach (var item in _Items) {
					target._Items.Add(item.Clone());
				}
			}
			if (Bookmark != null) {
				target.Bookmark = Bookmark.Clone();
			}
			target.PageCount = PageCount;
		}
		static BookmarkSettings CreateBookmarkSettings(string t) {
			if (AppContext.Merger.CajSort && t.Length == 6) {
				if (MatchCajPattern(t, Constants.CajNaming.Cover)) {
					return t.EndsWith("001", StringComparison.Ordinal) ? new BookmarkSettings("封面")
						: t.EndsWith("002", StringComparison.Ordinal) ? new BookmarkSettings("封底")
						: null; // 超过2页的，只为第一页和第二页生成书签
				}
				else if (MatchCajPattern(t, Constants.CajNaming.TitlePage)) {
					return t.EndsWith("001", StringComparison.Ordinal) ? new BookmarkSettings("书名") : null;
				}
				else if (MatchCajPattern(t, Constants.CajNaming.CopyrightPage)) {
					return t.EndsWith("001", StringComparison.Ordinal) ? new BookmarkSettings("版权") : null;
				}
				else if (MatchCajPattern(t, Constants.CajNaming.Foreword)) {
					return t.EndsWith("001", StringComparison.Ordinal) ? new BookmarkSettings("前言") : null;
				}
				else if (MatchCajPattern(t, Constants.CajNaming.Contents)) {
					return t.EndsWith("00001", StringComparison.Ordinal) ? new BookmarkSettings("目录") : null;
				}
				else if (MatchCajPattern(t, String.Empty) && t == "000001") {
					return new BookmarkSettings("正文");
				}
			}
			if (AppContext.Merger.IgnoreLeadingNumbers) {
				int i;
				for (i = 0; i < t.Length; i++) {
					if (t[i] > '9' || t[i] < '0') {
						break;
					}
				}
				t = t.Substring(i);
			}
			return new BookmarkSettings(t);
		}
		static bool CajSort(string[] fileList) {
			var m = false; // match Caj naming
			var cov = new List<string>(1);
			var bok = new List<string>(2);
			var leg = new List<string>(1);
			var fow = new List<string>(3);
			var cnt = new List<string>(5);
			var body = new List<string>(fileList.Length);
			foreach (var path in fileList) {
				var f = Path.GetFileNameWithoutExtension(path);
				if (f.Length == 6) {
					if (MatchCajPatternAddPath(path, f, Constants.CajNaming.Cover, cov)
						|| MatchCajPatternAddPath(path, f, Constants.CajNaming.TitlePage, bok)
						|| MatchCajPatternAddPath(path, f, Constants.CajNaming.CopyrightPage, leg)
						|| MatchCajPatternAddPath(path, f, Constants.CajNaming.Foreword, fow)
						|| MatchCajPatternAddPath(path, f, Constants.CajNaming.Contents, cnt)
						) {
						m = true;
						continue;
					}
				}
				body.Add(path);
			}
			if (m == false) {
				return false;
			}
			cov.Sort(StringComparer.OrdinalIgnoreCase);
			bok.Sort(StringComparer.OrdinalIgnoreCase);
			leg.Sort(StringComparer.OrdinalIgnoreCase);
			fow.Sort(StringComparer.OrdinalIgnoreCase);
			cnt.Sort(StringComparer.OrdinalIgnoreCase);
			body.Sort(StringComparer.OrdinalIgnoreCase);
			int p = 0;
			if (cov.Count == 2) {
				fileList[0] = cov[0];
				++p;
			}
			else {
				p = CopyItem(fileList, cov, p);
			}
			p = CopyItem(fileList, bok, p);
			p = CopyItem(fileList, leg, p);
			p = CopyItem(fileList, fow, p);
			p = CopyItem(fileList, cnt, p);
			p = CopyItem(fileList, body, p);
			if (cov.Count == 2) {
				fileList[p] = cov[1];
			}
			return true;
		}

		static int CopyItem(string[] fileList, List<string> list, int position) {
			list.CopyTo(fileList, position);
			position += list.Count;
			return position;
		}

		static bool MatchCajPatternAddPath(string path, string text, string pattern, List<string> container) {
			if (MatchCajPattern(text, pattern)) {
				container.Add(path);
				return true;
			}
			return false;
		}

		static bool CajSort(List<SourceItem> fileList) {
			var m = false; // match Caj naming
			var cov = new List<SourceItem>(1);
			var bok = new List<SourceItem>(2);
			var leg = new List<SourceItem>(1);
			var fow = new List<SourceItem>(3);
			var cnt = new List<SourceItem>(5);
			var body = new List<SourceItem>(fileList.Count);
			foreach (var file in fileList) {
				var path = file.FilePath;
				var f = Path.GetFileNameWithoutExtension(path);
				if (f.Length == 6) {
					if (MatchCajPatternAddPath(file, f, Constants.CajNaming.Cover, cov)
						|| MatchCajPatternAddPath(file, f, Constants.CajNaming.TitlePage, bok)
						|| MatchCajPatternAddPath(file, f, Constants.CajNaming.CopyrightPage, leg)
						|| MatchCajPatternAddPath(file, f, Constants.CajNaming.Foreword, fow)
						|| MatchCajPatternAddPath(file, f, Constants.CajNaming.Contents, cnt)
						) {
						m = true;
						continue;
					}
				}
				body.Add(file);
			}
			if (m == false) {
				return false;
			}
			cov.Sort(CompareFilePath);
			bok.Sort(CompareFilePath);
			leg.Sort(CompareFilePath);
			fow.Sort(CompareFilePath);
			cnt.Sort(CompareFilePath);
			body.Sort(CompareFilePath);
			fileList.Clear();
			if (cov.Count == 2) {
				fileList.Add(cov[0]);
			}
			else {
				fileList.AddRange(cov);
			}
			fileList.AddRange(bok);
			fileList.AddRange(leg);
			fileList.AddRange(fow);
			fileList.AddRange(cnt);
			fileList.AddRange(body);
			if (cov.Count == 2) {
				fileList.Add(cov[1]);
			}
			return true;
		}

		static bool MatchCajPatternAddPath(SourceItem item, string text, string pattern, List<SourceItem> container) {
			if (MatchCajPattern(text, pattern)) {
				container.Add(item);
				return true;
			}
			return false;
		}

		static int CompareFilePath(SourceItem x, SourceItem y) {
			return String.Compare(x.FilePath, y.FilePath, StringComparison.OrdinalIgnoreCase);
		}

		static bool MatchCajPattern(string text, string pattern) {
			if (text.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) == false) {
				return false;
			}
			int l = pattern.Length;
			if (text.Length == l) {
				return false;
			}
			foreach (var ch in text.Substring(l)) {
				if (ch < '0' || ch > '9') {
					return false;
				}
			}
			return true;
		}
	}
}
