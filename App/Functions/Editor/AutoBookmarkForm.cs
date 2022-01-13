using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Functions.Editor;

namespace PDFPatcher.Functions
{
	public sealed partial class AutoBookmarkForm : DraggableForm
	{
		List<EditModel.AutoBookmarkStyle> _list;
		readonly Controller _controller;

		internal AutoBookmarkForm(Controller controller) {
			InitializeComponent();
			_controller = controller;
		}

		private void AutoBookmarkForm_Load(object sender, EventArgs e) {
			MinimumSize = Size;
			_ConditionColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => String.Concat("字体为", o.FontName, "且尺寸为", o.FontSize);
			});
			//_FontSizeColumn.AsTyped<EditModel.TitleStyle>(c => {
			//	c.AspectGetter = o => o.FontSize;
			//	c.AspectPutter = (o, v) => o.FontSize = Convert.ToInt32(v);
			//});
			_LevelColumn.CellEditUseWholeCell = true;
			_LevelColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Level;
				c.AspectPutter = (o, v) => o.Level = Convert.ToInt32(v).LimitInRange(1, 10);
			});
			_BoldColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Style.IsBold;
				c.AspectPutter = (o, v) => o.Style.IsBold = (bool)v;
			});
			_ItalicColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Style.IsItalic;
				c.AspectPutter = (o, v) => o.Style.IsItalic = (bool)v;
			});
			_ColorColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => "点击设置颜色";
			});
			_OpenColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Style.IsOpened;
				c.AspectPutter = (o, v) => o.Style.IsOpened = (bool)v;
			});
			_BookmarkConditionBox.IsSimpleDragSource = true;
			_BookmarkConditionBox.IsSimpleDropSink = true;
			_BookmarkConditionBox.CellClick += (s, args) => {
				var ts = args.Model as EditModel.AutoBookmarkStyle;
				if (args.ColumnIndex == _ColorColumn.Index) {
					this.ShowCommonDialog<ColorDialog>(
						f => f.Color = ts.Style.ForeColor == Color.Transparent ? Color.White : ts.Style.ForeColor,
						f => {
							ts.Style.ForeColor = f.Color == Color.White ? Color.Transparent : f.Color;
							_BookmarkConditionBox.RefreshItem(args.Item);
						}
					);
				}
			};
			_BookmarkConditionBox.RowFormatter = (r) => {
				var ts = r.RowObject as EditModel.AutoBookmarkStyle;
				r.UseItemStyleForSubItems = false;
				r.SubItems[_ColorColumn.Index].ForeColor = ts.Style.ForeColor == Color.Transparent
					? _BookmarkConditionBox.ForeColor
					: ts.Style.ForeColor;
			};
		}

		internal void SetValues(List<EditModel.AutoBookmarkStyle> list) {
			_BookmarkConditionBox.Objects = _list = list;
		}

		private void _RemoveButton_Click(object sender, EventArgs e) {
			_BookmarkConditionBox.SelectedObjects.ForEach<EditModel.AutoBookmarkStyle>(i => _list.Remove(i));
			_BookmarkConditionBox.RemoveObjects(_BookmarkConditionBox.SelectedObjects);
		}

		private void _AutoBookmarkButton_Click(object sender, EventArgs e) {
			_list.Clear();
			_list.AddRange(new TypedObjectListView<EditModel.AutoBookmarkStyle>(_BookmarkConditionBox).Objects);
			_controller.AutoBookmark(_list, _MergeAdjacentTitleBox.Checked);
		}
	}
}