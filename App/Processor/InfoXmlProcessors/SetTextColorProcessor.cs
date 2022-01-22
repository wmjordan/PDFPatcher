using System.Drawing;
using System.Xml;
using PDFPatcher.Common;

namespace PDFPatcher.Processor;

internal sealed class SetTextColorProcessor : IPdfInfoXmlProcessor
{
	private readonly string r, g, b;

	public SetTextColorProcessor(Color color) {
		if (color == Color.Transparent || color == Color.White) {
			return;
		}

		r = (color.R / 255f).ToText();
		g = (color.G / 255f).ToText();
		b = (color.B / 255f).ToText();
	}

	public Color Color { get; private set; }

	#region IInfoDocProcessor 成员

	public string Name => "设置书签文本颜色";

	public IUndoAction Process(XmlElement item) {
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