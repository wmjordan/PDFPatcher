using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Functions.Editor;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	sealed partial class AutoBookmarkForm : DraggableForm
	{
		List<EditModel.AutoBookmarkStyle> _list;
		readonly Controller _controller;

		internal AutoBookmarkForm() {
			InitializeComponent();
		}

		internal AutoBookmarkForm(Controller controller) : this() {
			_controller = controller;
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			MinimumSize = Size;
			_Toolbar.ScaleIcons(16);

			_ConditionColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => $"字体为{o.FontName} 尺寸为{o.FontSize}{(o.MatchPattern != null ? o.MatchPattern.ToString() : String.Empty)}";
			});
			_LevelColumn.CellEditUseWholeCell = true;
			_LevelColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Level;
				c.AspectPutter = (o, v) => o.Level = Convert.ToInt32(v).LimitInRange(1, 10);
			});
			_BoldColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Bookmark.IsBold;
				c.AspectPutter = (o, v) => o.Bookmark.IsBold = (bool)v;
			});
			_ItalicColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Bookmark.IsItalic;
				c.AspectPutter = (o, v) => o.Bookmark.IsItalic = (bool)v;
			});
			_ColorColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => "点击设置颜色";
			});
			_OpenColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Bookmark.IsOpened;
				c.AspectPutter = (o, v) => o.Bookmark.IsOpened = (bool)v;
			});
			_GoToTopColumn.AsTyped<EditModel.AutoBookmarkStyle>(c => {
				c.AspectGetter = o => o.Bookmark.GoToTop;
				c.AspectPutter = (o, v) => o.Bookmark.GoToTop = (bool)v;
			});
			_BookmarkConditionBox.ScaleColumnWidths();
			_BookmarkConditionBox.IsSimpleDragSource = true;
			_BookmarkConditionBox.IsSimpleDropSink = true;
			_BookmarkConditionBox.CellClick += (s, args) => {
				if (args.ColumnIndex == _ColorColumn.Index) {
					var b = ((EditModel.AutoBookmarkStyle)args.Model).Bookmark;
					this.ShowCommonDialog<ColorDialog>(
						f => f.Color = b.ForeColor == Color.Transparent ? Color.White : b.ForeColor,
						f => {
							b.ForeColor = f.Color == Color.White || f.Color == Color.Black ? Color.Transparent : f.Color;
							_BookmarkConditionBox.RefreshItem(args.Item);
						}
						);
				}
			};
			_BookmarkConditionBox.RowFormatter = (r) => {
				var b = ((EditModel.AutoBookmarkStyle)r.RowObject).Bookmark;
				r.UseItemStyleForSubItems = false;
				r.SubItems[_ColorColumn.Index].ForeColor = b.ForeColor == Color.Transparent ? _BookmarkConditionBox.ForeColor : b.ForeColor;
			};
			_BookmarkConditionBox.SelectionChanged += _BookmarkConditionBox_SelectionChanged;
			_LoadListButton.Click += _LoadListButton_Click;
			_SaveListButton.Click += _SaveListButton_Click;

			QuickSelectCommand.RegisterMenuItemsWithPattern(_SetPatternMenu.DropDownItems);
			_SetPatternMenu.DropDownItems.AddRange(new ToolStripItem[] {
				new ToolStripSeparator(),
				new ToolStripMenuItem("自定义文本识别模式") {
					Tag = "CustomPattern"
				},
				new ToolStripMenuItem("清除文本识别模式") {
					Tag = "ClearPattern"
				}
			});
			_SetPatternMenu.DropDownItemClicked += _SetPattern_DropDownItemClicked;
		}

		void _BookmarkConditionBox_SelectionChanged(object sender, EventArgs e) {
			_SetPatternMenu.Enabled = _RemoveButton.Enabled = _BookmarkConditionBox.SelectedItems.Count > 0;
		}

		void _SetPattern_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			if (e.ClickedItem.Tag is MatchPattern p) {
				SetMatchPatternToSelectedBookmarkStyles(p);
				return;
			}
			if (e.ClickedItem.Tag is string s) {
				switch (s) {
					case "ClearPattern":
						SetMatchPatternToSelectedBookmarkStyles(null);
						return;
					case "CustomPattern":
						// todo 自定义匹配模式
						this.ErrorBox("本功能尚未实现");
						return;
				}
			}
		}

		void SetMatchPatternToSelectedBookmarkStyles(MatchPattern p) {
			foreach (EditModel.AutoBookmarkStyle item in _BookmarkConditionBox.SelectedObjects) {
				item.MatchPattern = p;
			}
			_BookmarkConditionBox.RefreshObjects(_BookmarkConditionBox.SelectedObjects);
		}

		internal void SetValues(List<EditModel.AutoBookmarkStyle> list) {
			_BookmarkConditionBox.Objects = _list = list;
		}

		void _RemoveButton_Click(object sender, EventArgs e) {
			_BookmarkConditionBox.SelectedObjects.ForEach<EditModel.AutoBookmarkStyle>(i => _list.Remove(i));
			_BookmarkConditionBox.RemoveObjects(_BookmarkConditionBox.SelectedObjects);
		}

		void _AutoBookmarkButton_Click(object sender, EventArgs e) {
			SyncList();
			_controller.AutoBookmark(_list, _MergeAdjacentTitleBox.Checked, _KeepExistingBookmarksBox.Checked);
		}

		void _SaveListButton_Click(object sender, EventArgs e) {
			this.ShowCommonDialog<SaveFileDialog>(d => {
				d.Title = "请输入需要保存自动书签格式列表的文件名";
				d.Filter = Constants.FileExtensions.XmlFilter;
				d.DefaultExt = Constants.FileExtensions.Xml;
			}, d => {
				try {
					SyncList();
					using (var w = new FilePath(d.FileName).OpenTextWriter(false, null)) {
						Serialize(_list, w);
					}
				}
				catch (Exception ex) {
					this.ErrorBox("保存自动书签格式列表时出现错误", ex);
				}
			});
		}

		static void Serialize(List<EditModel.AutoBookmarkStyle> list, System.IO.StreamWriter writer) {
			using (var x = System.Xml.XmlWriter.Create(writer)) {
				x.WriteStartDocument();
				x.WriteStartElement("autoBookmark");
				foreach (var item in list) {
					x.WriteStartElement("style");
					x.WriteAttributeString("fontName", item.FontName);
					x.WriteAttributeString("fontSize", item.FontSize.ToText());
					x.WriteAttributeString("level", item.Level.ToText());
					item.Bookmark.WriteXml(x);
					item.MatchPattern?.WriteXml(x);
					x.WriteEndElement();
				}
				x.WriteEndElement();
				x.WriteEndDocument();
			}
		}

		void _LoadListButton_Click(object sender, EventArgs e) {
			this.ShowCommonDialog<OpenFileDialog>(d => {
				d.Title = "请选择需要打开的自动书签格式列表";
				d.Filter = Constants.FileExtensions.XmlFilter;
				d.DefaultExt = Constants.FileExtensions.Xml;
			}, d => {
				try {
					SetValues(Deserialize(d.FileName));
				}
				catch (Exception ex) {
					this.ErrorBox("加载自动书签格式列表时出现错误", ex);
				}
			});
		}

		static List<EditModel.AutoBookmarkStyle> Deserialize(FilePath path) {
			var doc = new System.Xml.XmlDocument();
			doc.Load(path);
			var l = new List<EditModel.AutoBookmarkStyle>();
			foreach (System.Xml.XmlElement item in doc.DocumentElement.GetElementsByTagName("style")) {
				var s = new EditModel.AutoBookmarkStyle(
					item.GetValue("level", 1),
					item.GetValue("fontName"),
					item.GetValue("fontSize", 0));
				s.Bookmark.ReadXml(item.GetElement("bookmark").CreateNavigator().ReadSubtree());
				var p = item.GetElement("pattern");
				if (p != null) {
					s.MatchPattern = new Model.MatchPattern();
					s.MatchPattern.ReadXml(p.CreateNavigator().ReadSubtree());
				}

				l.Add(s);
			}
			return l;
		}

		void SyncList() {
			_list.Clear();
			_list.AddRange(new TypedObjectListView<EditModel.AutoBookmarkStyle>(_BookmarkConditionBox).Objects);
		}
	}
}
