using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class BookmarkOpenStatusProcessor : IPdfInfoXmlProcessor, IPdfInfoXmlProcessor<bool>
{
	public BookmarkOpenStatusProcessor() {
	}

	public BookmarkOpenStatusProcessor(bool open) {
		Parameter = open;
	}

	/// <summary>
	///     表示处理器是否应打开书签。
	/// </summary>
	public bool Parameter { get; set; }

	#region IInfoDocProcessor 成员

	public string Name => "设置书签状态为" + (Parameter ? "打开" : "关闭");

	public IUndoAction Process(XmlElement item) {
		if (item.SelectSingleNode(Constants.Bookmark) == null) {
			return null;
		}

		string v = item.HasChildNodes && item.SelectSingleNode(Constants.Bookmark) != null && Parameter
			? Constants.Boolean.True
			: null;
		return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Open, v);
	}

	#endregion
}