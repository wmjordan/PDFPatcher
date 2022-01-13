using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using BO = PDFPatcher.ToolbarOptions.ButtonOption;

namespace PDFPatcher.Functions;

public partial class CustomizeToolbarForm : Form
{
	public CustomizeToolbarForm() {
		InitializeComponent();
	}

	private void _ResetButton_Click(object sender, EventArgs e) {
		AppContext.Toolbar.Reset();
		_ItemListBox.Objects = AppContext.Toolbar.Buttons;
	}

	private void CustomizeToolbarForm_Load(object sender, EventArgs e) {
		foreach (Toolkit item in Toolkit.Toolkits) {
			_ItemListBox.SmallImageList.Images.Add(item.Icon,
				Properties.Resources.ResourceManager.GetObject(item.Icon) as Image);
		}

		new TypedColumn<BO>(_NameColumn) {
			AspectGetter = (o) => o.GetToolkit().Name, ImageGetter = (o) => o.GetToolkit().Icon
		};
		new TypedColumn<BO>(_ShowTextColumn) {
			AspectGetter = (o) => o.ShowText, AspectPutter = (o, v) => o.ShowText = (bool)v
		};
		new TypedColumn<BO>(_VisibleColumn) {
			AspectGetter = (o) => o.Visible, AspectPutter = (o, v) => o.Visible = (bool)v
		};
		new TypedColumn<BO>(_DisplayTextColumn) {
			AspectGetter = (o) => o.DisplayName,
			AspectPutter = (o, v) => o.DisplayName = v as string ?? o.GetToolkit().Name
		};
		AppContext.Toolbar.AddMissedButtons();
		_ItemListBox.IsSimpleDragSource = true;
		_ItemListBox.IsSimpleDropSink = true;
		_ItemListBox.DragSource = new SimpleDragSource(true);
		_ItemListBox.DropSink = new RearrangingDropSink(false) {CanDropBetween = true, CanDropOnItem = false};
		_ItemListBox.Objects = AppContext.Toolbar.Buttons;
	}

	private void _OkButton_Click(object sender, EventArgs e) {
		List<BO> l = new();
		foreach (BO item in _ItemListBox.Objects) {
			l.Add(item);
		}

		AppContext.Toolbar.Buttons.Clear();
		AppContext.Toolbar.Buttons.AddRange(l);
		Close();
	}
}