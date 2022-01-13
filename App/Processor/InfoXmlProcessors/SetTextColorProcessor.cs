using System.Drawing;
using PDFPatcher.Common;

namespace PDFPatcher.Processor;

internal sealed class SetTextColorProcessor : IPdfInfoXmlProcessor
{
	private readonly string r, g, b;
	public Color Color { get; private set; }

	public SetTextColorProcessor(Color color) {
		if (color != Color.Transparent && color != Color.White) {
			r = ValueHelper.ToText(color.R / 255f);
			g = ValueHelper.ToText(color.G / 255f);
			b = ValueHelper.ToText(color.B / 255f);
		}
	}

	#region IInfoDocProcessor 成员

	public string Name => "设置书签文本颜色";

	public IUndoAction Process(System.Xml.XmlElement item) {
		UndoActionGroup undo = new();
		if (string.IsNullOrEmpty(r) == false) {
			undo.SetAttribute(item, Constants.Colors.Red, r);
			undo.SetAttribute(item, Constants.Colors.Green, g);
			undo.SetAttribute(item, Constants.Colors.Blue, b);
		}
		else {
			undo.RemoveAttribute(item, Constants.Colors.Red);
			undo.RemoveAttribute(item, Constants.Colors.Green);
			undo.RemoveAttribute(item, Constants.Colors.Blue);
		}

		undo.RemoveAttribute(item, Constants.Color);
		return undo;
	}

	#endregion
}