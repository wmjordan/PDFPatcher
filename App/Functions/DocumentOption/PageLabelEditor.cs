using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class PageLabelEditor : UserControl
	{
		readonly TypedObjectListView<Model.PageLabel> _LabelBox;
		List<Model.PageLabel> _Labels;

		[Browsable(false)]
		public List<Model.PageLabel> Labels {
			get => _Labels;
			set { _Labels = value; _PageLabelBox.Objects = value; }
		}

		public PageLabelEditor() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
			_LabelBox = new TypedObjectListView<PDFPatcher.Model.PageLabel>(_PageLabelBox);
			_PageLabelBox.FormatRow += (s, args) => args.Item.SubItems[0].Text = (args.RowIndex + 1).ToText();
			new TypedColumn<Model.PageLabel>(_PageNumColumn) {
				AspectGetter = (o) => o.PageNumber,
				AspectPutter = (o, v) => { int i = v.ToString().ToInt32(); o.PageNumber = i > 0 ? i : 1; }
			};
			new TypedColumn<Model.PageLabel>(_StartNumColumn) {
				AspectGetter = (o) => o.StartPage,
				AspectPutter = (o, v) => { int i = v.ToString().ToInt32(); o.StartPage = i > 0 ? i : 1; }
			};
			new TypedColumn<Model.PageLabel>(_LabelStyleColumn) {
				AspectGetter = (o) => o.Style ?? Constants.PageLabelStyles.Names[0]
			};
			new TypedColumn<Model.PageLabel>(_LabelPrefixColumn) {
				AspectGetter = (o) => o.Prefix,
				AspectPutter = (o, v) => o.Prefix = v as string
			};
		}

		void OnLoad() {
			foreach (var item in Constants.PageLabelStyles.Names) {
				_LabelStyleMenu.Items.Add(item);
			}
			_PageLabelBox.FixEditControlWidth();
			_PageLabelBox.ScaleColumnWidths();
			_PageLabelBox.FullRowSelect = true;
			_PageLabelBox.LabelEdit = false;
			_PageLabelBox.CellClick += (s, args) => {
				if (args.Column == _LabelStyleColumn) {
					var b = _PageLabelBox.GetSubItem(args.RowIndex, args.ColumnIndex).Bounds;
					_LabelStyleMenu.Show(_PageLabelBox, b.Left, b.Bottom);
				}
			};
			_LabelStyleMenu.ItemClicked += (s, args) => {
				_LabelBox.SelectedObject.Style = args.ClickedItem.Text;
				_PageLabelBox.RefreshObject(_PageLabelBox.SelectedObject);
			};
		}

		void _AddPageLabelButton_Click(object sender, EventArgs e) {
			var i = 0;
			foreach (var item in _Labels) {
				if (item.PageNumber > i) {
					i = item.PageNumber;
				}
			}
			++i;
			_Labels.Add(new Model.PageLabel() { PageNumber = i, StartPage = 1 });
			_LabelBox.Objects = _Labels;
		}

		void _RemovePageLabelButton_Click(object sender, EventArgs e) {
			_PageLabelBox.RemoveObjects(_PageLabelBox.SelectedObjects);
			_Labels.Clear();
			_Labels.AddRange(_LabelBox.Objects);
		}

	}
}
