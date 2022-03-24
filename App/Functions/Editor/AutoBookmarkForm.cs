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

		void AutoBookmarkForm_Load(object sender, EventArgs e) {
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
			_LoadListButton.Click += _LoadListButton_Click;
			_SaveListButton.Click += _SaveListButton_Click;
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
			_controller.AutoBookmark(_list, _MergeAdjacentTitleBox.Checked);
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
