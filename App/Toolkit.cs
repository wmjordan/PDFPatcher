using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Properties;

namespace PDFPatcher;

internal sealed class Toolkit
{
	internal static readonly Toolkit[] Toolkits = {
		new("编辑器", "BookmarkEditor", "Editor", "创建或修改 PDF 文档的书签，修改 PDF 文档的设置", true),
		new("批量修改文件", "Patcher", "DocumentProcessor", "根据配置批量处理 PDF 文档，生成新的文档", true),
		new("合并文件", "Merger", "Merger", "将多个图片和 PDF 文档合并为新的 PDF 文档", true),
		new("识别文本", "Ocr", "Ocr", "识别扫描 PDF 文档的文字（需微软 Office 2003或2007的光学字符识别（OCR）引擎支持）"),
		new("自动书签", "BookmarkGenerator", "AutoBookmark", "根据 PDF 的文本样式生成书签文件"),
		new("批量重命名", "Rename", "Rename", "根据 PDF 的文本属性更改文件名", false, false),
		new("提取页面或拆分文档", "ExtractPages", "ExtractPages", "提取 PDF 文档的页面或重排页面"),
		new("提取图片", "ExtractImages", "ExtractImage", "无损提取 PDF 文档中的图片", true),
		new("转换页面为图片", "RenderPages", "RenderDocument", "将 PDF 文档的页面转换为图片"),
		new("结构探查器", "Inspector", "DocumentInspector", "探查 PDF 文档的内部结构", false, false),
		new("导出或导入信息文件", "InfoExchanger", "ExportInfoFile", "导出书签、文档元数据、阅读器设定等信息到信息文件"),
		new("程序配置", "Options", "AppOptions", "修改 PDF 补丁丁的程序配置", false, false)
	};

	private Toolkit(string name, string id, string icon, string description)
		: this(name, id, icon, description, false, true) {
	}

	private Toolkit(string name, string id, string icon, string description, bool showText)
		: this(name, id, icon, description, showText, true) {
	}

	private Toolkit(string name, string id, string icon, string description, bool showText, bool defaultVisible) {
		Name = name;
		Identifier = id;
		Icon = icon;
		Description = description;
		ShowText = showText;
		DefaultVisisble = defaultVisible;
	}

	public string Identifier { get; }
	public string Icon { get; }
	public string Name { get; }
	public string Description { get; }
	public bool ShowText { get; }
	public bool DefaultVisisble { get; }

	internal static Toolkit Get(string id) {
		foreach (Toolkit item in Toolkits) {
			if (item.Identifier == id) {
				return item;
			}
		}

		return null;
	}

	internal ToolStripButton CreateButton() {
		return new ToolStripButton(Name,
			Resources.ResourceManager.GetObject(Icon) as Image) {
			Name = Identifier,
			Tag = Identifier,
			ToolTipText = Description,
			DisplayStyle = ShowText ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image
		};
	}
}