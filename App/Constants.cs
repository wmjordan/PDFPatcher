using iTextSharp.text.pdf;
using E = System.Text.Encoding;

namespace PDFPatcher;

internal enum Function
{
	FrontPage, InfoFileOptions, InfoExchanger, ExtractPages, ExtractImages, RenderPages, EditorOptions, Patcher,
	PatcherOptions, Merger, MergerOptions, About, BookmarkEditor, Options, BookmarkGenerator, Ocr, Inspector,
	Rename, Log, Default, CustomizeToolbar
}

internal static class Constants
{
	internal const string AppName = "PDF 补丁丁";
	internal const string AppEngName = "PDFPatcher";
	internal const string AppHomePage = "http://pdfpatcher.cnblogs.com";
	internal const string AppHubPage = "https://gitee.com/wmjordan/pdfpatcher";
	internal const string AppUpdateFile = "http://files.cnblogs.com/pdfpatcher/pdfpatcher.update.xml";

	/// <summary>
	///     信息文件根元素。
	/// </summary>
	internal const string PdfInfo = "PDF信息";

	internal const string InfoDocVersion = "0.3.3";
	internal const string ContentPrefix = "pdf";
	internal const string ContentNamespace = "pdf:ContentXml";

	internal static class FileExtensions
	{
		internal const string Json = ".json";
		internal const string JsonFilter = "程序配置文件 (*.json)|*.json";
		internal const string Pdf = ".pdf";
		internal const string PdfFilter = "PDF 文件 (*.pdf)|*.pdf";
		internal const string Txt = ".txt";
		internal const string TxtFilter = "简易文本书签文件 (*.txt)|*.txt";
		internal const string Xml = ".xml";
		internal const string XmlFilter = "PDF 信息文件 (*.xml)|*.xml";
		internal const string PdfOrXmlFilter = "PDF 文件或信息文件 (*.pdf, *.xml)|*.pdf;*.xml";
		internal const string XmlOrTxtFilter = "书签文件 (*.xml, *.txt)|*.xml;*.txt";

		internal const string AllEditableFilter = "所有包含 PDF 信息的文件(*.pdf,*.xml,*.txt)|*.pdf;*.xml;*.txt|" +
		                                          PdfFilter + "|" + XmlFilter + "|" + TxtFilter;

		internal const string AllFilter = "所有文件|*.*";

		internal const string ImageFilter =
			"图片文件 (*.jpg, *.jpeg, *.tiff, *.tif, *.png, *.gif)|*.jpg;*.jpeg;*.tiff;*.tif;*.png;*.gif";

		internal const string Tif = ".tif";
		internal const string Tiff = ".tiff";
		internal const string Jpg = ".jpg";
		internal const string Jpeg = ".jpeg";
		internal const string Png = ".png";
		internal const string Gif = ".gif";
		internal const string Jp2 = ".jp2";
		internal const string Bmp = ".bmp";
		internal const string Dat = ".dat";
		internal const string Tmp = ".tmp";
		internal const string Ttf = ".ttf";
		internal const string Ttc = ".ttc";
		internal const string Otf = ".otf";
		internal static readonly string[] AllBookmarkExtension = {".xml", ".txt"};
		internal static readonly string[] PdfAndAllBookmarkExtension = {".pdf", ".xml", ".txt"};
		internal static readonly string[] AllSupportedImageExtension = {Tif, Jpg, Png, Gif, Tiff, Jpeg, Bmp, Jp2};
	}

	#region 功能名称

	private static class Functions
	{
		internal const string FrontPage = "FrontPage";
		internal const string Patcher = "Patcher";
		internal const string Merger = "Merger";
		internal const string ImageExtractor = "ImageExtractor";
		internal const string PageExtractor = "PageExtractor";
		internal const string PageRenderer = "PageRenderer";
		internal const string BookmarkEditor = "BookmarkEditor";
		internal const string BookmarkGenerator = "BookmarkGenerator";
		internal const string Ocr = "Ocr";
		internal const string Inspector = "Inspector";
		internal const string Log = "Log";
		internal const string About = "About";
	}

	#endregion

	#region PDF 对象类型

	internal static class ObjectTypes
	{
		internal static readonly string[] Names = {"字典", "名称", "数值", "文本", "数组", "布尔", "引用"};

		internal static readonly int[] IDs = {
			PdfObject.DICTIONARY, PdfObject.NAME, PdfObject.NUMBER, PdfObject.STRING, PdfObject.ARRAY,
			PdfObject.BOOLEAN, PdfObject.INDIRECT
		};
	}

	#endregion

	#region 文件名替代符

	internal static class FileNameMacros
	{
		internal const string FileName = "<源文件名>";
		internal const string FolderName = "<源目录名>";
		internal const string PathName = "<源目录路径>";
		internal const string TitleProperty = "<" + Info.Title + ">";
		internal const string AuthorProperty = "<" + Info.Author + ">";
		internal const string SubjectProperty = "<" + Info.Subject + ">";
		internal const string KeywordsProperty = "<" + Info.Keywords + ">";
	}

	#endregion

	#region 度量单位

	internal static class Units
	{
		internal const string ThisName = "度量单位";
		internal const string Unit = "单位";
		internal const string Point = "点";
		internal const string CM = "厘米";
		internal const string MM = "毫米";
		internal const string Inch = "英寸";
		internal const float CmToPoint = 72f / 2.54f;
		internal const float MmToPoint = 7.2f / 2.54f;
		internal const float DefaultDpi = 72f;
		internal static readonly string[] Names = {CM, MM, Inch, Point};
		internal static readonly float[] Factors = {CmToPoint, MmToPoint, DefaultDpi, 1};
	}

	#endregion

	#region 对齐方式

	internal static class Alignments
	{
		internal static readonly string[] HorizontalAlignments = {"左对齐", "水平居中", "右对齐"};
		internal static readonly string[] VerticalAlignments = {"置顶", "垂直居中", "置底"};
	}

	#endregion

	#region 方位

	internal static class Coordinates
	{
		internal const string Left = "左";
		internal const string Right = "右";
		internal const string Top = "上";
		internal const string Bottom = "下";
		internal const string Width = "宽";
		internal const string Height = "高";
		internal const string Direction = "方向";
		internal const string Horizontal = "横向";
		internal const string Vertical = "纵向";
		internal const string ScaleFactor = "比例";
		internal const string Unchanged = "保持不变";
	}

	#endregion

	#region 编码

	internal static class Encoding
	{
		internal const string SystemDefault = "系统默认";
		internal const string Automatic = "自动选择";

		internal static readonly string[] EncodingNames = {
			Automatic, SystemDefault, "UTF-16 Big Endian", "UTF-16 Little Endian", "UTF-8", "GB18030", "BIG5"
		};

		internal static readonly E[] Encodings = {
			null, E.Default, E.BigEndianUnicode, E.Unicode, E.UTF8, E.GetEncoding("gb18030"), E.GetEncoding("big5")
		};
	}

	#endregion

	#region 页面内容

	internal static class Content
	{
		internal const string Page = "页面";
		internal const string PageNumber = "页码";
		internal const string ResourceID = "资源编号";
		internal const string RefType = "引用对象类型";
		internal const string Texts = "文本内容";
		internal const string Operators = "命令";
		internal const string Operands = "参数";
		internal const string Name = "名称";
		internal const string Item = "项目";
		internal const string Path = "路径";
		internal const string Type = "类型";
		internal const string Length = "长度";
		internal const string Raw = "原始内容";
		internal const string Value = "值";

		internal static class PageSettings
		{
			internal const string ThisName = "页面设置";
			internal const string MediaBox = "页面边框";
			internal const string CropBox = "截取边框";
			internal const string TrimBox = "裁剪边框";
			internal const string ArtBox = "内容边框";
			internal const string BleedBox = "出血边框";
			internal const string Rotation = "旋转角度";
		}

		internal static class OperandNames
		{
			internal const string Matrix = "矩阵";
			internal const string ResourceName = "资源名称";
			internal const string Size = "尺寸";
			internal const string Text = "文本";
		}

		internal static class RotationDirections
		{
			internal const string ThisName = PageSettings.Rotation;
			internal const string Zero = "保持不变";
			internal const string Right = "顺时针90度";
			internal const string HalfClock = "180度";
			internal const string Left = "逆时针90度";
			internal static readonly string[] Names = {Zero, Right, HalfClock, Left};
			internal static readonly int[] Values = {0, 90, 180, 270};
		}
	}

	#endregion

	#region 光学字符识别

	internal static class Ocr
	{
		internal const int NoLanguage = 0;
		internal const int SimplifiedChineseLangID = 2052;
		internal const int TraditionalChineseLangID = 1028;
		internal const int JapaneseLangID = 1041;
		internal const int KoreanLangID = 1042;
		internal const int EnglishLangID = 1033;

		internal const int DanishLangID = 1030;
		internal const int DutchLangID = 1043;
		internal const int FinnishLangID = 1035;
		internal const int FrenchLangID = 1036;
		internal const int GermanLangID = 1031;
		internal const int ItalianLangID = 1040;
		internal const int NorskLangID = 1044;
		internal const int PortugueseLangID = 1046;
		internal const int SpanishLangID = 3082;
		internal const int SwedishLangID = 1053;
		internal const int CzechLangID = 1029;
		internal const int PolishLangID = 1045;
		internal const int HungarianLangID = 1038;
		internal const int GreekLangID = 1032;
		internal const int RussianLangID = 1049;
		internal const int TurkishLangID = 1055;

		internal const string Result = "识别结果";
		internal const string Text = "文本";
		internal const string Content = "内容";
		internal const string Image = "图片";

		internal static int[] LangIDs = {
			SimplifiedChineseLangID, TraditionalChineseLangID, EnglishLangID, JapaneseLangID, KoreanLangID,
			DanishLangID, DutchLangID, FinnishLangID, FrenchLangID, GermanLangID, ItalianLangID, NorskLangID,
			PortugueseLangID, SpanishLangID, SwedishLangID, CzechLangID, PolishLangID, HungarianLangID, GreekLangID,
			RussianLangID, TurkishLangID
		};

		internal static int[] OcrLangIDs = {
			SimplifiedChineseLangID, TraditionalChineseLangID, 9, 17, 18, 6, 19, 11, 12, 7, 16, 20, 22, 10, 29, 5, 21,
			14, 8, 25, 31
		};

		internal static string[] LangNames = {
			"简体中文", "繁体中文", "英文", "日文", "韩文", "丹麦文", "荷兰文", "芬兰文", "法文", "德文", "意大利文", "挪威文", "葡萄牙文", "西班牙文", "瑞典文",
			"捷克文", "波兰文", "匈牙利文", "希腊文", "俄文", "土耳其文"
		};
	}

	#endregion

	#region 导出为图片

	internal static class ColorSpaces
	{
		internal const string Rgb = "DeviceRGB";
		internal const string Bgr = "DeviceBGR";
		internal const string Cmyk = "DeviceCMYK";
		internal const string Gray = "DeviceGray";
		internal static string[] Names = {Rgb, Gray};
	}

	#endregion

	#region 超星命名规则

	internal static class CajNaming
	{
		internal const string Cover = "cov";
		internal const string TitlePage = "bok";
		internal const string CopyrightPage = "leg";
		internal const string Foreword = "fow";
		internal const string Contents = "!";
	}

	#endregion

	internal static class AutoBookmark
	{
		internal const string ThisName = "自动书签";
		internal const string Group = "条件集合";
		internal const string Name = "名称";
		internal const string Description = "说明";
		internal const string IsInclusive = "正向过滤";
	}

	#region 文档信息

	internal static class Info
	{
		internal const string ThisName = "文档信息";

		internal const string ProductName = "程序名称";
		internal const string ProductVersion = "程序版本";
		internal const string ExportDate = "导出时间";
		internal const string DocumentName = "PDF文件名";
		internal const string DocumentPath = "PDF文件位置";
		internal const string PageNumber = "页数";
		internal const string Title = "标题";
		internal const string Author = "作者";
		internal const string Subject = "主题";
		internal const string Keywords = "关键字";
		internal const string Creator = "创建程序";
		internal const string Producer = "处理程序";
		internal const string CreationDate = "创建日期";
		internal const string ModDate = "最近修改日期";
		internal const string MetaData = "XML元数据";
	}

	internal const string Version = "PDF版本";
	internal const string Catalog = "文档编录";
	internal const string Body = "正文内容";
	internal const string DocumentBookmark = "文档书签";

	#endregion

	#region 阅读器设定

	internal const string PageLayout = "页面布局";

	internal static class PageLayoutType
	{
		internal static readonly string[] Names = {"保持不变", "单页连续", "双页连续", "双页连续首页独置", "单页", "双页", "双页首页独置"};

		internal static readonly PdfName[] PdfNames = {
			PdfName.NONE, PdfName.ONECOLUMN, PdfName.TWOCOLUMNLEFT, PdfName.TWOCOLUMNRIGHT, PdfName.SINGLEPAGE,
			PdfName.TWOPAGELEFT, PdfName.TWOPAGERIGHT
		};
	}

	internal const string PageMode = "初始模式";

	internal static class PageModes
	{
		internal static readonly string[]
			Names = {"保持不变", "不显示边栏", "显示文档书签", "显示页面缩略图", "全屏显示", "显示可选内容组", "显示附件栏"};

		internal static readonly PdfName[] PdfNames = {
			PdfName.NONE, PdfName.USENONE, PdfName.USEOUTLINES, PdfName.USETHUMBS, PdfName.FULLSCREEN, PdfName.USEOC,
			PdfName.USEATTACHMENTS
		};
	}

	internal const string ViewerPreferences = "阅读器设定";

	internal static class ViewerPreferencesType
	{
		internal const string Direction = "阅读方向";
		internal static readonly string[] Names = {"隐藏菜单", "隐藏工具栏", "只显示文档内容", "窗口适合文档首页", "窗口居中", "显示文档标题"};

		internal static readonly PdfName[] PdfNames = {
			PdfName.HIDEMENUBAR, PdfName.HIDETOOLBAR, PdfName.HIDEWINDOWUI, PdfName.FITWINDOW, PdfName.CENTERWINDOW,
			PdfName.DISPLAYDOCTITLE
		};

		internal static class DirectionType
		{
			internal static readonly string[] Names = {"保持不变", "从左到右", "从右到左"};
			internal static readonly PdfName[] PdfNames = {PdfName.NONE, PdfName.L2R, PdfName.R2L};
		}
	}

	#endregion

	#region 页码样式

	internal const string PageLabels = "页码样式";

	internal static class PageLabelStyles
	{
		internal static readonly string[] Names = {"数字", "大写罗马数字", "小写罗马数字", "大写英文字母", "小写英文字母"};
		internal static readonly char[] PdfValues = {'D', 'R', 'r', 'A', 'a'};
		internal static readonly char[] SimpleInfoIdentifiers = {'0', 'I', 'i', 'A', 'a'};

		internal static readonly int[] Values = {
			PdfPageLabels.DECIMAL_ARABIC_NUMERALS, PdfPageLabels.UPPERCASE_ROMAN_NUMERALS,
			PdfPageLabels.LOWERCASE_ROMAN_NUMERALS, PdfPageLabels.UPPERCASE_LETTERS, PdfPageLabels.LOWERCASE_LETTERS
		};
	}

	internal static class PageLabelsAttributes
	{
		internal const string PageNumber = "实际页码";
		internal const string StartPage = "起始页码";
		internal const string Prefix = "页码前缀";
		internal const string Style = "样式";
	}

	#endregion

	#region 页码范围

	internal const string PageRange = "页码范围";

	internal static class PageFilterTypes
	{
		internal const string ThisName = "页码筛选";
		internal const string AllPages = "所有页";
		internal static readonly string[] Names = {AllPages, "单数页", "双数页"};
		internal static readonly int[] Values = {-1, 1, 0};
	}

	#endregion

	#region 目标

	internal const string NamedDestination = "命名位置";

	internal static class DestinationAttributes
	{
		internal const string Page = "页码";
		internal const string FirstPageNumber = "首页页码";
		internal const string Action = "动作";
		internal const string NewWindow = "新窗口";
		internal const string Path = "路径";
		internal const string Name = "名称";
		internal const string Named = "命名位置";
		internal const string NamedN = "PDF名称";
		internal const string View = "显示方式";
		internal const string ScriptContent = "脚本内容";

		internal static class ViewType
		{
			internal const string XYZ = "坐标缩放";
			internal const string Fit = "适合页面";
			internal const string FitH = "适合页宽";
			internal const string FitV = "适合页高";
			internal const string FitB = "适合窗口";
			internal const string FitBH = "适合窗口宽度";
			internal const string FitBV = "适合窗口高度";
			internal const string FitR = "适合区域";
			internal static readonly string[] Names = {XYZ, Fit, FitH, FitV, FitB, FitBH, FitBV, FitR};

			internal static readonly PdfName[] PdfNames = {
				PdfName.XYZ, PdfName.FIT, PdfName.FITH, PdfName.FITV, PdfName.FITB, PdfName.FITBH, PdfName.FITBV,
				PdfName.FITR
			};
		}
	}

	internal static class ActionType
	{
		internal const string Goto = "转到页面";
		internal const string GotoR = "打开外部PDF文档";
		internal const string Launch = "启动程序";
		internal const string Uri = "打开网址";
		internal const string Javascript = "执行脚本";
		internal static readonly string[] Names = {Goto, GotoR, Launch, Uri, Javascript};

		internal static readonly PdfName[] PdfNames = {
			PdfName.GOTO, PdfName.GOTOR, PdfName.LAUNCH, PdfName.URI, PdfName.JAVASCRIPT
		};
	}

	#endregion

	#region 书签

	internal const string Bookmark = "书签";

	internal static class BookmarkAttributes
	{
		internal const string Title = "文本";
		internal const string Open = "默认打开";
		internal const string Style = "样式";

		internal static class StyleType
		{
			internal const string Normal = "常规";
			internal const string Bold = "粗体";
			internal const string BoldItalic = "粗斜体";
			internal const string Italic = "斜体";
			internal static readonly string[] Names = {Normal, Italic, Bold, BoldItalic};
		}
	}

	internal const string Color = "颜色";

	internal static class Colors
	{
		internal const string Red = "红";
		internal const string Green = "绿";
		internal const string Blue = "蓝";
		internal const string Gray = "灰度";
		internal const string Transparent = "透明";
		internal const string Cyan = "青";
		internal const string Magenta = "紫";
		internal const string Yellow = "黄";
		internal const string Black = "黑";
	}

	internal static class Boolean
	{
		internal const string True = "是";
		internal const string False = "否";
	}

	#endregion

	#region 页面链接

	internal const string PageLink = "页面链接";

	internal static class PageLinkAttributes
	{
		internal const string Link = "链接";
		internal const string LinkAction = "链接动作";
		internal const string PageNumber = "页码";
		internal const string Border = "边框";
		internal const string Style = "点击效果";
		internal const string QuadPoints = "四边形坐标";
		internal const string Contents = "文本";
	}

	#endregion

	#region 字体属性

	internal static class Font
	{
		internal const string ThisName = "字体";
		internal const string DocumentFont = "文档字体";
		internal const string ID = "编号";
		internal const string Name = "名称";
		internal const string Size = "文本尺寸";
	}

	internal static class FontOccurance
	{
		internal const string Count = "出现次数";
		internal const string FirstText = "首次出现文本";
		internal const string FirstPage = "首次出现页码";
	}

	#endregion
}

internal static class Messages
{
	internal const string Welcome = "PDF 补丁丁——解除 PDF 文档的烦恼";
	internal const string SourceFileNotFound = "源 PDF 文件不存在，请先指定有效的源 PDF 文件。";
	internal const string InfoDocNotFound = "信息文件不存在，请先指定有效的信息文件。";
	internal const string TargetFileNotSpecified = "请指定输出 PDF 文件的路径。";
	internal const string InfoDocNotSpecified = "请指定输出信息文件的路径。";
	internal const string SourceFileNameInvalid = "源 PDF 文件名无效。";
	internal const string TargetFileNameInvalid = "输出 PDF 文件名无效。";
	internal const string InfoFileNameInvalid = "信息文件的文件名无效。";
	internal const string SourceFileEqualsTargetFile = "输入 PDF 文件和输出 PDF 文件的文件名不能相同。";
	internal const string PasswordInvalid = "输入的密码错误，无法打开 PDF 文档。";

	internal const string UserRightRequired =
		"此 PDF 文件的作者设置了修改文件的权限控制。\n如果您继续操作，您必须得到创建者对该文档进行修改的授权。\n如果您不能保证自己有权修改此文档，请按“否”键退出，否则您需要承担修改此文档带来的一切责任。";

	internal const string PageRanges =
		"在此输入需要处理的页码范围。\n如：“1-100”表示处理第1～100页。\n如有多个页码范围，可用空格、分号或逗号分开。\n如：“1-10;12;14-20”表示处理1～10、12和14～20页。";

	internal const string ReversePageRanges = "此外还可以输入逆序页码，如“100-1”表示从第100页开始倒序处理至第1页。";
	internal const string ModiNotAvailable = "本机尚未安装微软文本识别组件（MODI），无法使用识别文本功能。";
}