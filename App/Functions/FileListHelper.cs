using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	sealed class FileListHelper
	{
		public delegate void AddFilesCallback(string[] files, bool alertInvalidFiles);

		readonly ObjectListView _fileList;

		public FileListHelper(ObjectListView fileList) {
			_fileList = fileList;
		}

		/// <summary>
		/// 设置 PDF 文件列表的拖放操作。
		/// </summary>
		/// <param name="addFilesCallback">添加文件的回调函数。</param>
		public void SetupDragAndDrop(AddFilesCallback addFilesCallback) {
			_fileList.DragSource = new SimpleDragSource(true);
			var ds = new RearrangingDropSink(false);
			_fileList.DropSink = ds;

			ds.CanDrop += (s, args) => {
				var files = FormHelper.DropFileOver(args.DragEventArgs, Constants.FileExtensions.Pdf);
				if (files.Length > 0) {
					args.Effect = DragDropEffects.Link;
					args.InfoMessage = "添加 " + files.Length.ToString() + " 个文件";
					args.Handled = true;
				}
			};
			ds.Dropped += (s, args) => {
				var files = FormHelper.DropFileOver(args.DragEventArgs, Constants.FileExtensions.Pdf);
				if (files.Length > 0) {
					_fileList.SelectedIndex
						= args.DropTargetLocation == DropTargetLocation.Background
							? _fileList.GetItemCount() - 1
							: args.DropTargetIndex + (args.DropTargetLocation == DropTargetLocation.AboveItem ? -1 : 0);
					addFilesCallback(files, false);
					args.Handled = true;
				}
			};
		}

		/// <summary>
		/// 打开 PDF 文件的 <see cref="ToolStripSplitButton"/> 显示下拉文件列表的事件处理函数。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void OpenPdfButtonDropDownOpeningHandler(object sender, EventArgs e) {
			var b = sender as ToolStripSplitButton;
			var l = b.DropDown.Items;
			l.ClearDropDownItems();
			foreach (var item in AppContext.Recent.SourcePdfFiles) {
				l.Add(FileHelper.GetEllipticPath(item, 50)).ToolTipText = item;
			}

			if (l.Count == 0) {
				b.PerformButtonClick();
			}
		}

		/// <summary>
		/// 以指定编码刷新文件列表的选定项目。
		/// </summary>
		/// <param name="encoding">用于读取文档元数据的 <see cref="Enocding"/>。</param>
		public void RefreshInfo(Encoding encoding) {
			var ol = _fileList.SelectedObjects;
			if (ol.Count == 0) {
				_fileList.SelectAll();
				ol = _fileList.SelectedObjects;
			}

			foreach (SourceItem.Pdf item in ol) {
				item.Refresh(encoding);
			}

			_fileList.RefreshObjects(ol);
		}

		/// <summary>
		/// 为 <see cref="OLVColumn"/> 设置读写处理函数。
		/// </summary>
		/// <param name="columns">需要设置的列。</param>
		public static void SetupCommonPdfColumns(params OLVColumn[] columns) {
			foreach (var item in columns) {
				switch (item.Text) {
					case "源文件名":
						SetupFileNameColumn(item);
						break;
					case "文件夹":
						SetupFolderNameColumn(item);
						break;
					case "标题":
						SetupTitleColumn(item);
						break;
					case "作者":
						SetupAuthorColumn(item);
						break;
					case "主题":
						SetupSubjectColumn(item);
						break;
					case "关键词":
						SetupKeywordsColumn(item);
						break;
					case "页数":
						SetupPageCountColumn(item);
						break;
				}
			}
		}

		static void SetupAuthorColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.DocInfo.Author;
				c.AspectPutter = (o, value) => o.DocInfo.Author = value as string;
			});
		}

		static void SetupKeywordsColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.DocInfo.Keywords;
				c.AspectPutter = (o, value) => o.DocInfo.Keywords = value as string;
			});
		}

		static void SetupSubjectColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.DocInfo.Subject;
				c.AspectPutter = (o, value) => o.DocInfo.Subject = value as string;
			});
		}

		static void SetupTitleColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.DocInfo.Title;
				c.AspectPutter = (o, value) => o.DocInfo.Title = value as string;
			});
		}

		static void SetupPageCountColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.PageCount.ToText();
			});
		}

		static void SetupFileNameColumn(OLVColumn column) {
			column.AsTyped<SourceItem.Pdf>(c => {
				c.AspectGetter = o => o.Type == SourceItem.ItemType.Empty ? "<空白页面>" : o.FileName;
				c.ImageGetter = o => 0;
			});
		}

		static void SetupFolderNameColumn(OLVColumn column) {
			column.AsTyped<SourceItem>(c => {
				c.AspectGetter = o => o.Type != SourceItem.ItemType.Empty ? o.FolderName : String.Empty;
			});
		}

		public void SetupHotkeys() {
			_fileList.KeyUp += (s, args) => {
				switch (args.KeyCode) {
					case Keys.Delete:
						if (_fileList.IsCellEditing || _fileList.Focused == false) {
							return;
						}

						ProcessCommonMenuCommand(Commands.Delete);
						break;
				}
			};
		}

		public bool ProcessCommonMenuCommand(string commandID) {
			switch (commandID) {
				case Commands.Delete:
					if (_fileList.GetItemCount() == 0) {
						return true;
					}

					var l = _fileList.SelectedObjects;
					if (l.Count == 0) {
						if (FormHelper.YesNoBox("是否清空文件列表？") == DialogResult.Yes) {
							_fileList.ClearObjects();
						}
					}
					else {
						_fileList.RemoveObjects(_fileList.SelectedObjects);
					}

					break;
				case "_Copy":
					var sb = new StringBuilder();
					foreach (SourceItem.Pdf item in GetSourceItems<SourceItem>(true)) {
						sb.AppendLine(String.Join("\t",
							new string[] {
								item.FilePath.ToString(), item.PageCount.ToText(), item.DocInfo.Title,
								item.DocInfo.Author, item.DocInfo.Subject, item.DocInfo.Keywords
							}));
					}

					if (sb.Length > 0) {
						Clipboard.SetText(sb.ToString());
					}

					break;
				case Commands.SelectAllItems:
					_fileList.SelectAll();
					break;
				case Commands.InvertSelectItem:
					foreach (ListViewItem item in _fileList.Items) {
						item.Selected = !item.Selected;
					}

					break;
				case Commands.SelectNone:
					_fileList.SelectObjects(null);
					break;
				default:
					return false;
			}

			return true;
		}

		public List<T> GetSourceItems<T>(bool selectedOnly) where T : SourceItem {
			if (_fileList.GetItemCount() == 0) {
				return null;
			}

			var l = (selectedOnly ? _fileList.SelectedObjects : _fileList.Objects);
			var items = new List<T>(selectedOnly ? 10 : _fileList.GetItemCount());
			foreach (T item in l) {
				if (item == null) {
					continue;
				}

				items.Add(item);
			}

			return items;
		}

		public void PrepareSourceFiles() {
			var c = _fileList.GetItemCount();
			if (c == 0) {
				return;
			}

			var f = new string[c];
			var i = 0;
			foreach (SourceItem item in _fileList.Objects) {
				if (item.Type == SourceItem.ItemType.Pdf) {
					f[i++] = item.FilePath.ToString();
				}
			}

			Array.Resize(ref f, i);
			AppContext.SourceFiles = f;
		}

		public void ResizeItemListColumns() {
			var c = _fileList.Columns[0];
			_fileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			if (c.Width < 100) {
				c.Width = 100;
			}

			for (int i = 1; i < _fileList.Columns.Count; i++) {
				c = _fileList.Columns[i];
				if (c.Width < 50) {
					c.Width = 50;
				}
			}
		}
	}
}