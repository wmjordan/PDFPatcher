using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using MuPdfSharp;
using PDFPatcher.Common;

namespace PDFPatcher.Model;

public sealed class PdfInfoXmlDocument : XmlDocument
{
	/// <summary>返回已经初始化的 <see cref="PdfInfoXmlDocument" /> 实例。</summary>
	public PdfInfoXmlDocument() {
		Init();
	}

	/// <summary>获取或设置配置文件关联的 PDF 文件路径。</summary>
	public string PdfDocumentPath {
		get => DocumentElement.GetAttribute(Constants.Info.DocumentPath);
		set => DocumentElement.SetAttribute(Constants.Info.DocumentPath, value);
	}

	/// <summary>返回文档信息节点。</summary>
	public DocumentInfoElement InfoNode =>
		DocumentElement.GetOrCreateElement(Constants.Info.ThisName) as DocumentInfoElement;

	/// <summary>返回页码标签节点。</summary>
	public XmlElement PageLabelRoot => DocumentElement.GetOrCreateElement(Constants.PageLabels);

	public XmlNodeList PageLabels =>
		DocumentElement.SelectNodes(Constants.PageLabels + "[1]/" + Constants.PageLabelsAttributes.Style);

	/// <summary>返回书签根节点。</summary>
	public BookmarkRootElement BookmarkRoot =>
		DocumentElement.GetOrCreateElement(Constants.DocumentBookmark) as BookmarkRootElement;

	/// <summary>获取根书签。</summary>
	public XmlNodeList Bookmarks =>
		DocumentElement.SelectNodes(Constants.DocumentBookmark + "[1]/" + Constants.Bookmark);

	private void Init() {
		XmlElement root = DocumentElement;
		if (root == null) {
			root = AppendChild(CreateElement(Constants.PdfInfo)) as XmlElement;
		}

		root.SetAttribute(Constants.Info.ProductName, Application.ProductName);
		root.SetAttribute(Constants.Info.ProductVersion, Constants.InfoDocVersion);
		root.SetAttribute(Constants.Info.ExportDate, DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));
	}

	public BookmarkElement CreateBookmark() {
		return new BookmarkElement(this);
	}

	public BookmarkElement CreateBookmark(BookmarkSettings settings) {
		BookmarkElement b = new(this) {
			Title = settings.Title,
			IsOpen = settings.IsOpened,
			Action = Constants.ActionType.Goto
		};
		if (settings.ForeColor.IsEmptyOrTransparent() == false) {
			b.ForeColor = settings.ForeColor;
		}

		if (settings.IsBold || settings.IsItalic) {
			b.TextStyle = (settings.IsBold ? FontStyle.Bold : FontStyle.Regular) |
						  (settings.IsItalic ? FontStyle.Italic : FontStyle.Regular);
		}

		return b;
	}

	public PageLabelElement CreatePageLabel(MuPdfSharp.PageLabel label) {
		PageLabelElement l = new(this);
		l.SetAttributes(label);
		return l;
	}

	public override XmlElement CreateElement(string prefix, string localName, string namespaceURI) {
		if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(namespaceURI)) {
			switch (localName) {
				case Constants.Bookmark:
					return new BookmarkElement(this);
				case Constants.DocumentBookmark:
					return new BookmarkRootElement(this);
				case Constants.PageLabelsAttributes.Style:
					return new PageLabelElement(this);
				case Constants.Info.ThisName:
					return new DocumentInfoElement(this);
			}
		}

		return base.CreateElement(prefix, localName, namespaceURI);
	}
}

/// <summary>文档元数据属性元素。</summary>
public sealed class DocumentInfoElement : XmlElement
{
	internal DocumentInfoElement(XmlDocument doc)
		: base(string.Empty, Constants.Info.ThisName, string.Empty, doc) {
	}

	public string Title {
		get => this.GetValue(Constants.Info.Title);
		set => this.SetValue(Constants.Info.Title, value, null);
	}

	public string Author {
		get => this.GetValue(Constants.Info.Author);
		set => this.SetValue(Constants.Info.Author, value, null);
	}

	public string Creator {
		get => this.GetValue(Constants.Info.Creator);
		set => this.SetValue(Constants.Info.Creator, value, null);
	}

	public string Keywords {
		get => this.GetValue(Constants.Info.Keywords);
		set => this.SetValue(Constants.Info.Keywords, value, null);
	}

	public string Producer {
		get => this.GetValue(Constants.Info.Producer);
		set => this.SetValue(Constants.Info.Producer, value, null);
	}

	public string Subject {
		get => this.GetValue(Constants.Info.Subject);
		set => this.SetValue(Constants.Info.Subject, value, null);
	}
}

public abstract class BookmarkContainer : XmlElement
{
	protected BookmarkContainer(string name, XmlDocument doc)
		: base(string.Empty, name, string.Empty, doc) {
	}

	/// <summary>获取当前书签容器是否有子书签。</summary>
	public bool HasSubBookmarks => HasChildNodes && SelectSingleNode(Constants.Bookmark) != null;

	/// <summary>获取当前书签容器的子书签。</summary>
	public XmlNodeList SubBookmarks => SelectNodes(Constants.Bookmark);

	public BookmarkElement ParentBookmark => ParentNode as BookmarkElement;
	public BookmarkContainer Parent => ParentNode as BookmarkContainer;

	/// <summary>创建新的下级书签并返回该书签。</summary>
	public BookmarkElement AppendBookmark() {
		return AppendChild((OwnerDocument as PdfInfoXmlDocument).CreateBookmark()) as BookmarkElement;
	}

	/// <summary>使用指定的配置创建新的书签。返回新创建的书签。</summary>
	/// <param name="settings">书签设置。</param>
	public BookmarkElement AppendBookmark(BookmarkSettings settings) {
		return AppendChild((OwnerDocument as PdfInfoXmlDocument).CreateBookmark(settings)) as BookmarkElement;
	}
}

/// <summary>书签的根元素。</summary>
public sealed class BookmarkRootElement : BookmarkContainer
{
	internal BookmarkRootElement(XmlDocument doc)
		: base(Constants.DocumentBookmark, doc) {
	}
}

/// <summary>书签元素。</summary>
[DebuggerDisplay(Constants.Bookmark + "：{Title}")]
public sealed class BookmarkElement : BookmarkContainer
{
	/// <summary>在自动生成书签时标记级别的属性。</summary>
	internal int AutoLevel = 0;

	internal BookmarkElement(XmlDocument doc)
		: base(Constants.Bookmark, doc) {
	}

	/// <summary>获取或设置书签的文本。</summary>
	public string Title {
		get => GetAttribute(Constants.BookmarkAttributes.Title);
		set => SetAttribute(Constants.BookmarkAttributes.Title, value);
	}

	/// <summary>获取或设置书签的颜色。</summary>
	public Color ForeColor {
		get {
			if (HasAttribute(Constants.Colors.Red) || HasAttribute(Constants.Colors.Green) ||
				HasAttribute(Constants.Colors.Blue)) {
				float r = this.GetValue(Constants.Colors.Red, 0f),
					g = this.GetValue(Constants.Colors.Green, 0f),
					b = this.GetValue(Constants.Colors.Blue, 0f);
				return Color.FromArgb((int)(r * 255f), (int)(g * 255f), (int)(b * 255f));
			}

			if (HasAttribute(Constants.Color)) {
				string a = GetAttribute(Constants.Color);
				int c = a.ToInt32(int.MaxValue);
				return c != int.MaxValue ? Color.FromArgb(c) : Color.FromName(a);
			}

			return Color.Transparent;
		}
		set {
			RemoveAttribute(Constants.Color);
			if (value == Color.Transparent) {
				return;
			}

			SetAttribute(Constants.Color, value.ToArgb().ToText());
		}
	}

	/// <summary>获取或设置书签的文本样式。</summary>
	public FontStyle TextStyle {
		get {
			string s = GetAttribute(Constants.BookmarkAttributes.Style);
			if (string.IsNullOrEmpty(s) == false) {
				switch (s) {
					case Constants.BookmarkAttributes.StyleType.Bold: return FontStyle.Bold;
					case Constants.BookmarkAttributes.StyleType.Italic: return FontStyle.Italic;
					case Constants.BookmarkAttributes.StyleType.BoldItalic:
						return FontStyle.Italic | FontStyle.Bold;
				}
			}

			return FontStyle.Regular;
		}
		set {
			string s;
			switch (value) {
				case FontStyle.Bold:
					s = Constants.BookmarkAttributes.StyleType.Bold;
					break;
				case FontStyle.Italic:
					s = Constants.BookmarkAttributes.StyleType.Italic;
					break;
				case FontStyle.Bold | FontStyle.Italic:
					s = Constants.BookmarkAttributes.StyleType.BoldItalic;
					break;
				case FontStyle.Regular:
				case FontStyle.Underline:
				case FontStyle.Strikeout:
				default:
					RemoveAttribute(Constants.BookmarkAttributes.Style);
					return;
			}

			SetAttribute(Constants.BookmarkAttributes.Style, s);
		}
	}

	/// <summary>获取或设置书签的默认打开状态。</summary>
	public bool IsOpen {
		get {
			if (HasChildNodes == false) {
				return false;
			}

			return GetAttribute(Constants.BookmarkAttributes.Open) == Constants.Boolean.True;
		}
		set {
			//if (HasSubBookmarks == false) {
			//	return;
			//}
			if (value) {
				SetAttribute(Constants.BookmarkAttributes.Open, Constants.Boolean.True);
			}
			else {
				RemoveAttribute(Constants.BookmarkAttributes.Open);
			}
		}
	}

	/// <summary>获取或设置目标动作。</summary>
	public string Action {
		get => this.GetValue(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
		set => this.SetValue(Constants.DestinationAttributes.Action, value);
	}

	/// <summary>获取或设置书签的目标页面。</summary>
	public int Page {
		get => this.GetValue(Constants.DestinationAttributes.Page, 0);
		set {
			this.SetValue(Constants.DestinationAttributes.Page, value, 0);
			if (HasAttribute(Constants.DestinationAttributes.Action) == false) {
				SetAttribute(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
			}
		}
	}

	/// <summary>返回或设置目标视图。</summary>
	public string DestinationView {
		get => GetAttribute(Constants.DestinationAttributes.View);
		set => this.SetValue(Constants.DestinationAttributes.View, value);
	}

	/// <summary>获取或设置 XYZ 目标视图下的缩放比例。</summary>
	public float ScaleFactor {
		get => this.GetValue(Constants.Coordinates.ScaleFactor, 1f);
		set {
			SetAttribute(Constants.Coordinates.ScaleFactor, value.ToText());
			SetAttribute(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
		}
	}

	/// <summary>获取或设置跳转目标的上坐标。</summary>
	public float Top {
		get => this.GetValue(Constants.Coordinates.Top, 0f);
		set => this.SetValue(Constants.Coordinates.Top, value, 0);
	}

	/// <summary>获取或设置跳转目标的左坐标。</summary>
	public float Left {
		get => this.GetValue(Constants.Coordinates.Left, 0f);
		set => this.SetValue(Constants.Coordinates.Left, value, 0);
	}

	/// <summary>获取或设置跳转目标的下坐标。</summary>
	public float Bottom {
		get => this.GetValue(Constants.Coordinates.Bottom, 0f);
		set => this.SetValue(Constants.Coordinates.Bottom, value, 0);
	}

	/// <summary>获取或设置跳转目标的右坐标。</summary>
	public float Right {
		get => this.GetValue(Constants.Coordinates.Right, 0f);
		set => this.SetValue(Constants.Coordinates.Right, value, 0);
	}

	public int MarkerColor {
		get => this.GetValue("标记颜色", 0);
		set => this.SetValue("标记颜色", value, 0);
	}

	/// <summary>设置跳转到页面的书签动作。</summary>
	/// <param name="title">书签的标题。</param>
	/// <param name="pageNumber">跳转页面。</param>
	/// <param name="position">跳转位置。</param>
	public void SetTitleAndGotoPagePosition(string title, int pageNumber, float position) {
		SetAttribute(Constants.BookmarkAttributes.Title, title);
		this.SetValue(Constants.DestinationAttributes.Page, pageNumber, 0);
		SetAttribute(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
		if (position != 0) {
			SetAttribute(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
			this.SetValue(Constants.Coordinates.Top, position, 0);
		}
	}
}

/// <summary>页码标签设置集合的根元素。</summary>
public sealed class PageLabelRootElement : XmlElement
{
	internal PageLabelRootElement(XmlDocument doc)
		: base(string.Empty, Constants.DocumentBookmark, string.Empty, doc) {
	}

	public XmlNodeList Labels => SelectNodes(Constants.PageLabelsAttributes.Style);

	public void Add(MuPdfSharp.PageLabel label) {
		foreach (PageLabelElement item in Labels) {
			if (item.PageNumber == label.FromPageNumber) {
				item.SetAttributes(label);
				return;
			}
		}

		(this.AppendElement(Constants.PageLabelsAttributes.Style) as PageLabelElement).SetAttributes(label);
	}
}

/// <summary>页码标签设置元素。</summary>
public sealed class PageLabelElement : XmlElement
{
	internal PageLabelElement(XmlDocument doc)
		: base(string.Empty, Constants.PageLabelsAttributes.Style, string.Empty, doc) {
	}

	/// <summary>获取或指定开始使用页码标签的绝对页码。</summary>
	public int PageNumber {
		get => GetAttribute(Constants.PageLabelsAttributes.PageNumber).ToInt32();
		set => this.SetValue(Constants.PageLabelsAttributes.PageNumber, value < 1 ? 0 : value, 0);
	}

	public string PrefixLabel {
		get => GetAttribute(Constants.PageLabelsAttributes.Prefix);
		set => this.SetValue(Constants.PageLabelsAttributes.Prefix, value);
	}

	/// <summary>获取或指定页码标签样式。</summary>
	public string Style {
		get => GetAttribute(Constants.PageLabelsAttributes.Style);
		set => this.SetValue(Constants.PageLabelsAttributes.Style, value, Constants.PageLabelStyles.Names[0]);
	}

	/// <summary>获取或指定页码标签的起始编号。</summary>
	public int StartNumber {
		get => GetAttribute(Constants.PageLabelsAttributes.StartPage).ToInt32();
		set => this.SetValue(Constants.PageLabelsAttributes.StartPage, value < 1 ? 0 : value, 0);
	}

	public void SetAttributes(MuPdfSharp.PageLabel label) {
		this.SetValue(Constants.PageLabelsAttributes.PageNumber, label.FromPageNumber + 1, 0);
		SetAttribute(Constants.PageLabelsAttributes.Style,
			ValueHelper.MapValue((char)label.NumericStyle, Constants.PageLabelStyles.PdfValues,
				Constants.PageLabelStyles.Names));
		this.SetValue(Constants.PageLabelsAttributes.StartPage, label.StartAt, 0);
		this.SetValue(Constants.PageLabelsAttributes.Prefix, label.Prefix);
	}

	public MuPdfSharp.PageLabel ToPageLabel() {
		return new MuPdfSharp.PageLabel(
			PageNumber - 1,
			StartNumber,
			PrefixLabel,
			(PageLabelStyle)ValueHelper.MapValue(Style, Constants.PageLabelStyles.Names,
				Constants.PageLabelStyles.PdfValues));
	}
}