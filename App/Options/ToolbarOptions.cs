using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PDFPatcher;

public class ToolbarOptions
{
	public ToolbarOptions() {
		ShowGeneralToolbar = true;
	}

	[XmlAttribute("显示主工具栏")]
	[DefaultValue(true)]
	public bool ShowGeneralToolbar { get; set; }

	[XmlElement("按钮")] public List<ButtonOption> Buttons { get; } = new();

	public void Reset() {
		Buttons.Clear();
		foreach (Toolkit item in Toolkit.Toolkits) {
			Buttons.Add(new ButtonOption(item.Identifier, item.Name, item.ShowText, item.DefaultVisisble));
		}
	}

	internal void RemoveInvalidButtons() {
		if (Buttons.Count == 0) {
			Reset();
			return;
		}

		for (int i = Buttons.Count - 1; i >= 0; i--) {
			if (Buttons[i].GetToolkit() == null) {
				Buttons.RemoveAt(i);
			}
		}
	}

	internal void AddMissedButtons() {
		foreach (Toolkit item in Toolkit.Toolkits) {
			foreach (ButtonOption b in Buttons) {
				if (b.ID == item.Identifier) {
					goto Next;
				}
			}

			Buttons.Add(new ButtonOption(item.Identifier, item.Name, item.ShowText, false));
			Next: ;
		}
	}

	public class ButtonOption
	{
		public ButtonOption() {
		}

		public ButtonOption(string id, string name, bool showText, bool visible) {
			ID = id;
			DisplayName = name;
			ShowText = showText;
			Visible = visible;
		}

		[XmlAttribute("ID")] public string ID { get; set; }
		[XmlAttribute("按钮名称")] public string DisplayName { get; set; }
		[XmlAttribute("显示按钮文字")] public bool ShowText { get; set; }
		[XmlAttribute("显示按钮")] public bool Visible { get; set; }

		internal Toolkit GetToolkit() {
			return Toolkit.Get(ID);
		}

		internal ToolStripButton CreateButton() {
			ToolStripButton b = GetToolkit().CreateButton();
			b.Text = DisplayName;
			b.DisplayStyle = ShowText ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
			return b;
		}
	}
}