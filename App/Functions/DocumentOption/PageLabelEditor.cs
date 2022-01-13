using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

public partial class PageLabelEditor : UserControl
{
	private readonly TypedObjectListView<PageLabel> _LabelBox;
	private List<PageLabel> _Labels;

	public PageLabelEditor() {
		InitializeComponent();
		foreach (string item in Constants.PageLabelStyles.Names) {
			_LabelStyleMenu.Items.Add(item);
		}

		new TypedColumn<PageLabel>(_PageNumColumn) {
			AspectGetter = o => o.PageNumber,
			AspectPutter = (o, v) => {
				int i = v.ToString().ToInt32();
				o.PageNumber = i > 0 ? i : 1;
			}
		};
		new TypedColumn<PageLabel>(_StartNumColumn) {
			AspectGetter = o => o.StartPage,
			AspectPutter = (o, v) => {
				int i = v.ToString().ToInt32();
				o.StartPage = i > 0 ? i : 1;
			}
		};
		new TypedColumn<PageLabel>(_LabelStyleColumn) {
			AspectGetter = o => o.Style ?? Constants.PageLabelStyles.Names[0]
		};
		new TypedColumn<PageLabel>(_LabelPrefixColumn) {
			AspectGetter = o => o.Prefix, AspectPutter = (o, v) => o.Prefix = v as string
		};
		_PageLabelBox.FormatRow += (s, args) => args.Item.SubItems[0].Text = (args.RowIndex + 1).ToText();
		_PageLabelBox.FixEditControlWidth();
		_PageLabelBox.FullRowSelect = true;
		_PageLabelBox.LabelEdit = false;
		_PageLabelBox.CellClick += (s, args) => {
			if (args.Column == _LabelStyleColumn) {
				Rectangle b = _PageLabelBox.GetSubItem(args.RowIndex, args.ColumnIndex).Bounds;
				_LabelStyleMenu.Show(_PageLabelBox, b.Left, b.Bottom);
			}
		};
		_LabelBox = new TypedObjectListView<PageLabel>(_PageLabelBox);
		_LabelStyleMenu.ItemClicked += (s, args) => {
			_LabelBox.SelectedObject.Style = args.ClickedItem.Text;
			_PageLabelBox.RefreshObject(_PageLabelBox.SelectedObject);
		};
	}

	[Browsable(false)]
	public List<PageLabel> Labels {
		get => _Labels;
		set {
			_Labels = value;
			_PageLabelBox.Objects = value;
		}
	}

	private void _AddPageLabelButton_Click(object sender, EventArgs e) {
		int i = 0;
		foreach (PageLabel item in _Labels) {
			if (item.PageNumber > i) {
				i = item.PageNumber;
			}
		}

		++i;
		_Labels.Add(new PageLabel {PageNumber = i, StartPage = 1});
		_LabelBox.Objects = _Labels;
	}

	private void _RemovePageLabelButton_Click(object sender, EventArgs e) {
		_PageLabelBox.RemoveObjects(_PageLabelBox.SelectedObjects);
		_Labels.Clear();
		_Labels.AddRange(_LabelBox.Objects);
	}
}