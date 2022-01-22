using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Processor;

namespace PDFPatcher.Model;

public abstract class SourceItem
{
	public enum ItemType
	{
		Empty, Pdf, Image, Folder
	}

	private List<SourceItem> _Items;

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

		string t = path.ExistsDirectory ? FileName : path.FileNameWithoutExtension;
		if (t.Length > 0) {
			Bookmark = CreateBookmarkSettings(t);
		}
	}

	public FilePath FilePath { get; }
	public string FileName { get; }
	public string FolderName { get; }
	public BookmarkSettings Bookmark { get; set; }
	public int PageCount { get; private set; }
	public abstract int FileSize { get; }

	public List<SourceItem> Items => _Items ??= new List<SourceItem>();

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

	public abstract SourceItem Clone();

	public override string ToString() {
		return FilePath.ToString();
	}

	/// <summary>
	///     创建新的空白页。
	/// </summary>
	/// <returns>空白页实例。</returns>
	internal static Empty Create() {
		return new Empty();
	}

	internal static SourceItem Create(string path) {
		return Create(path, true);
	}

	/// <summary>
	///     根据传入的文件路径创建 <see cref="SourceItem" /> 实例。
	/// </summary>
	/// <param name="path">文件或文件夹路径。</param>
	/// <param name="refresh">是否强制更新。</param>
	/// <returns><see cref="SourceItem" /> 实例。</returns>
	internal static SourceItem Create(FilePath path, bool refresh) {
		if (((string)path).IsNullOrWhiteSpace()) {
			return Create();
		}

		if (path.ExistsDirectory) {
			return new Folder(path.ToString(), refresh);
		}

		if (path.HasExtension(Constants.FileExtensions.Pdf)) {
			try {
				PdfReader reader = PdfHelper.OpenPdfFile(path.ToString(), true, false);
				int c = reader.NumberOfPages;
				string r = null;
				if (refresh) {
					r = new PageRange(1, c).ToString();
				}

				GeneralInfo info =
					DocInfoExporter.RewriteDocInfoWithEncoding(reader, AppContext.Encodings.DocInfoEncoding);
				reader.Close();
				return new Pdf(path, r, c, info);
			}
			catch (FileNotFoundException) {
				FormHelper.ErrorBox(string.Concat("找不到文件：“", path, "”。"));
			}
			catch (Exception) {
				FormHelper.ErrorBox(string.Concat("打开 PDF 文件时“", path, "”出错。"));
				// ignore corrupted 
			}

			return null;
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

		FormHelper.ErrorBox(string.Concat("不支持文件“", path, "”。"));
		return null;
	}

	internal string GetInfoFileName() {
		// 优先采用与输入文件同名的 XML 信息文件
		FilePath f =
			new(FileHelper.CombinePath(FolderName, Path.ChangeExtension(FileName, Constants.FileExtensions.Xml)));
		if (f.ExistsFile) {
			return f.ToString();
		}

		// 次之采用与输入文件同名的 TXT 信息文件
		f = f.ChangeExtension(Constants.FileExtensions.Txt);
		if (f.ExistsFile) {
			return f.ToString();
		}

		// 次之采用同一个信息文件
		f = FilePath.ChangeExtension(Constants.FileExtensions.Xml);
		if (f.ExistsFile == false) {
			f = FilePath.Empty;
		}

		return f.ToString();
	}

	internal string GetTargetPdfFileName(string targetPath) {
		string targetFolder = null;
		bool m = FileHelper.HasFileNameMacro(targetPath); // 包含替换符
		if (m == false) {
			targetFolder = Path.GetDirectoryName(targetPath);
		}

		return m ? targetPath : FileHelper.CombinePath(targetFolder, FilePath.FileName);
	}

	protected static int GetFileKB(FilePath fileName) {
		if (fileName.ExistsFile == false) {
			return 0;
		}

		FileInfo f = fileName.ToFileInfo();
		return (int)Math.Ceiling((double)(f.Length >> 10));
	}

	protected virtual void CopyProperties(SourceItem target) {
		target._Items = new List<SourceItem>(HasSubItems ? Items.Count : 0);
		if (HasSubItems) {
			foreach (SourceItem item in _Items) {
				target._Items.Add(item.Clone());
			}
		}

		if (Bookmark != null) {
			target.Bookmark = Bookmark.Clone();
		}

		target.PageCount = PageCount;
	}

	private static BookmarkSettings CreateBookmarkSettings(string t) {
		if (AppContext.Merger.CajSort && t.Length == 6) {
			if (MatchCajPattern(t, Constants.CajNaming.Cover)) {
				return t.EndsWith("001") ? new BookmarkSettings("封面")
					: t.EndsWith("002") ? new BookmarkSettings("封底")
					: null; // 超过2页的，只为第一页和第二页生成书签
			}

			if (MatchCajPattern(t, Constants.CajNaming.TitlePage)) {
				return t.EndsWith("001") ? new BookmarkSettings("书名") : null;
			}

			if (MatchCajPattern(t, Constants.CajNaming.CopyrightPage)) {
				return t.EndsWith("001") ? new BookmarkSettings("版权") : null;
			}

			if (MatchCajPattern(t, Constants.CajNaming.Foreword)) {
				return t.EndsWith("001") ? new BookmarkSettings("前言") : null;
			}

			if (MatchCajPattern(t, Constants.CajNaming.Contents)) {
				return t.EndsWith("00001") ? new BookmarkSettings("目录") : null;
			}

			if (MatchCajPattern(t, string.Empty) && t == "000001") {
				return new BookmarkSettings("正文");
			}
		}

		if (!AppContext.Merger.IgnoreLeadingNumbers) {
			return new BookmarkSettings(t);
		}

		int i;
		for (i = 0; i < t.Length; i++) {
			if (t[i] > '9' || t[i] < '0') {
				break;
			}
		}

		t = t.Substring(i);

		return new BookmarkSettings(t);
	}

	private static bool CajSort(string[] fileList) {
		bool m = false; // match Caj naming
		List<string> cov = new(1);
		List<string> bok = new(2);
		List<string> leg = new(1);
		List<string> fow = new(3);
		List<string> cnt = new(5);
		List<string> body = new(fileList.Length);
		foreach (string path in fileList) {
			string f = Path.GetFileNameWithoutExtension(path);
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

	private static int CopyItem(string[] fileList, ICollection<string> list, int position) {
		list.CopyTo(fileList, position);
		position += list.Count;
		return position;
	}

	private static bool
		MatchCajPatternAddPath(string path, string text, string pattern, ICollection<string> container) {
		if (!MatchCajPattern(text, pattern)) {
			return false;
		}

		container.Add(path);
		return true;

	}

	private static BookmarkSettings CreateCajBookmark(string text, string pattern, string title) {
		if (!MatchCajPattern(text, pattern) || !text.EndsWith("001")) {
			return new BookmarkSettings(text);
		}

		return text.EndsWith("001") ? new BookmarkSettings(title) : null;
	}

	private static bool MatchCajPattern(string text, string pattern) {
		if (text.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) == false) {
			return false;
		}

		int l = pattern.Length;
		return text.Length != l && text.Substring(l).All(ch => ch is >= '0' and <= '9');
	}

	internal sealed class Empty : SourceItem
	{
		internal Empty() : base(null, 1) { }
		internal Empty(int pageCount) : base(null, pageCount) { }
		public override ItemType Type => ItemType.Empty;

		public override int FileSize => 0;

		public void SetPageCount(int pageCount) {
			PageCount = pageCount;
		}

		public override string ToString() {
			return "<空白页>";
		}

		public override SourceItem Clone() {
			Empty n = new();
			CopyProperties(n);
			return n;
		}
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

		public void CopyTo(CropOptions target) {
			target.Top = Top;
			target.Bottom = Bottom;
			target.Left = Left;
			target.Right = Right;
			target.MinWidth = MinWidth;
			target.MinHeight = MinHeight;
		}
	}

	internal sealed class Image : SourceItem
	{
		private int _FileSize = -1;

		public Image(FilePath path)
			: base(path, 0) {
			Cropping = new CropOptions();
			if (path.ExistsFile) {
				_FileSize = GetFileKB(path);
			}
		}

		public CropOptions Cropping { get; set; }
		public override ItemType Type => ItemType.Image;

		public override int FileSize {
			get {
				if (_FileSize == -1) {
					_FileSize = GetFileKB(FilePath);
				}

				return _FileSize;
			}
		}

		public override SourceItem Clone() {
			Image n = new(FilePath);
			Cropping.CopyTo(n.Cropping);
			return n;
		}
	}

	internal sealed class Pdf : SourceItem
	{
		private int _FileSize = -1;

		public Pdf(FilePath path, string pageRanges, int pageCount, GeneralInfo docInfo)
			: base(path, pageCount) {
			PageRanges = pageRanges;
			DocInfo = docInfo;
			ExtractImageOptions = new ImageExtracterOptions {
				OutputPath = Path.GetDirectoryName(path.ToString()),
				ExtractAnnotationImages = false,
				MergeJpgToPng = true,
				MergeImages = true,
				MinWidth = 50,
				MinHeight = 50
			};
		}

		public Pdf(FilePath path) : base(path, 0) {
			Refresh(path.ToString(), AppContext.Encodings.DocInfoEncoding);
		}

		public string PageRanges { get; set; }
		public bool ImportImagesOnly { get; set; }
		public ImageExtracterOptions ExtractImageOptions { get; }
		public GeneralInfo DocInfo { get; private set; }
		public override ItemType Type => ItemType.Pdf;

		public override int FileSize {
			get {
				if (_FileSize == -1) {
					_FileSize = GetFileKB(FilePath);
				}

				return _FileSize;
			}
		}

		public void Refresh(Encoding encoding) {
			Refresh(FilePath.ToString(), encoding);
		}

		public override string ToString() {
			return FilePath.IsEmpty ? string.Empty :
				string.IsNullOrEmpty(PageRanges) ? FilePath :
				string.Concat(FilePath, "::", PageRanges);
		}

		public override SourceItem Clone() {
			Pdf n = new(FilePath, PageRanges, PageCount, DocInfo) { ImportImagesOnly = ImportImagesOnly };
			CopyProperties(n);
			return n;
		}

		private void Refresh(string path, Encoding encoding) {
			try {
				using PdfReader reader = PdfHelper.OpenPdfFile(path, true, false);
				DocInfo = DocInfoExporter.RewriteDocInfoWithEncoding(reader, encoding);
				PageCount = reader.NumberOfPages;
				PageRanges = new PageRange(1, PageCount).ToString();
			}
			catch (Exception) {
				FormHelper.ErrorBox(string.Concat("打开 PDF 文件时“", path, "”出错。"));
				// ignore corrupted 
			}
		}
	}

	internal sealed class Folder : SourceItem
	{
		public Folder(string path) : base(path, 0) {
		}

		public Folder(string path, bool loadSubItems)
			: this(path) {
			if (loadSubItems) {
				Reload();
			}
		}

		public override ItemType Type => ItemType.Folder;

		public override int FileSize => 0;

		public void Reload() {
			Items.Clear();
			if (!FilePath.ExistsDirectory) {
				return;
			}

			string p = FilePath.ToString();
			List<SourceItem> l = Items;
			if (AppContext.Merger.SubFolderBeforeFiles) {
				AddSubDirectories(p, l);
				AddFiles(p, l);
			}
			else {
				AddSubDirectoriesAndFiles(p, l);
			}
		}

		public override SourceItem Clone() {
			Folder n = new(FilePath.ToString());
			CopyProperties(n);
			return n;
		}

		private static void AddSubDirectoriesAndFiles(string folderPath, ICollection<SourceItem> list) {
			string[] fl = Array.FindAll(Directory.GetFiles(folderPath), i => {
				string ext = Path.GetExtension(i).ToLowerInvariant();
				return Constants.FileExtensions.Pdf == ext
					   || Constants.FileExtensions.AllSupportedImageExtension.Contains(ext);
			});
			string[] d = Array.ConvertAll(Directory.GetDirectories(folderPath), i => i + "\\");
			string[] s = new string[fl.Length + d.Length];
			Array.Copy(fl, s, fl.Length);
			Array.Copy(d, 0, s, fl.Length, d.Length);
			SortFileList(s);
			foreach (string item in s) {
				list.Add(item[item.Length - 1] == '\\'
					? new Folder(item.Substring(0, item.Length - 1), true)
					: Create(item));
			}
		}

		private static void AddFiles(string folderPath, ICollection<SourceItem> list) {
			try {
				string[] fl = Directory.GetFiles(folderPath);
				SortFileList(fl);
				foreach (string item in fl) {
					string ext = Path.GetExtension(item).ToLowerInvariant();
					if (Constants.FileExtensions.Pdf == ext
						|| Constants.FileExtensions.AllSupportedImageExtension.Contains(ext)) {
						list.Add(Create(item));
					}
				}
			}
			catch (UnauthorizedAccessException) { }
			catch (IOException) { }
		}

		private static void AddSubDirectories(string folderPath, ICollection<SourceItem> list) {
			try {
				foreach (string item in Directory.EnumerateDirectories(folderPath)) {
					Folder f = new(item, true);
					list.Add(f);
				}
			}
			catch (UnauthorizedAccessException) { }
			catch (IOException) { }
		}
	}
}