using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions
{
	sealed partial class SearchBookmarkForm : Form
	{
		static BookmarkMatcher.MatcherType _matcherType = BookmarkMatcher.MatcherType.Normal;
		static bool _replaceInSelection = true;

		//static char[] __TrimChars = new char[] { ' ', '\t', '\r', '\n', '　' };
		readonly Editor.Controller _controller;
		internal SearchBookmarkForm(Editor.Controller controller) {
			InitializeComponent();
			_controller = controller;
		}

		void _SearchTextBox_TextChanged(object sender, EventArgs e) {
			_ResultLabel.Text = String.Empty;
		}

		private void SearchBookmarkForm_Load(object sender, EventArgs e) {
			MinimumSize = Size;
			ShowInTaskbar = false;
			_SearchTextBox.Contents = AppContext.Recent.SearchPatterns;
			_ReplaceTextBox.Contents = AppContext.Recent.ReplacePatterns;
			var b = _NormalSearchBox;
			switch (_matcherType) {
				case BookmarkMatcher.MatcherType.Normal:
					goto default;
				case BookmarkMatcher.MatcherType.Regex:
					b = _RegexSearchBox;
					goto default;
				case BookmarkMatcher.MatcherType.XPath:
					b = _XPathSearchBox;
					goto default;
				default:
					b.Checked = true;
					break;
			}
			if (_replaceInSelection) {
				_ReplaceInSelectionBox.Checked = true;
			}
			else {
				_ReplaceInAllBox.Checked = true;
			}
			_SearchTextBox.TextChanged += new EventHandler(_SearchTextBox_TextChanged);
		}

		BookmarkMatcher CreateMatcher() {
			return BookmarkMatcher.Create(_SearchTextBox.Text,
				_RegexSearchBox.Checked ? BookmarkMatcher.MatcherType.Regex
				: _XPathSearchBox.Checked ? BookmarkMatcher.MatcherType.XPath
				: BookmarkMatcher.MatcherType.Normal,
				_MatchCaseBox.Checked,
				_FullMatchBox.Checked);
		}

		private void _SearchButton_Click(Object sender, EventArgs args) {
			if (String.IsNullOrEmpty(_SearchTextBox.Text)) {
				Common.FormHelper.InfoBox("请先输入查询关键字。");
				return;
			}
			BookmarkMatcher matcher;
			try {
				matcher = CreateMatcher();
			}
			catch (Exception ex) {
				Common.FormHelper.ErrorBox("搜索表达式有误：" + ex.Message);
				return;
			}
			_SearchTextBox.AddHistoryItem();
			if (sender == _SearchButton) {
				var matches = _controller.View.Bookmark.SearchBookmarks(matcher);
				if (matches.Count > 0) {
					_ResultLabel.Text = "找到 " + matches.Count + " 个匹配的书签。";
					_controller.View.Bookmark.FindForm().Activate();
				}
				else {
					_ResultLabel.Text = "没有找到任何匹配的书签。";
				}
			}
			else {
				var m = _controller.View.Bookmark.SearchBookmark(matcher);
				if (m == null) {
					_ResultLabel.Text = "没有找到对应的书签。";
				}
				else {
					_ResultLabel.Text = String.Empty;
				}
			}
		}


		private void _ReplaceButton_Click(object sender, EventArgs e) {
			BookmarkMatcher matcher;
			try {
				matcher = CreateMatcher();
			}
			catch (Exception ex) {
				Common.FormHelper.ErrorBox("搜索表达式有误：" + ex.Message);
				return;
			}
			int i = ReplaceBookmarks(_replaceInSelection, matcher, _ReplaceTextBox.Text);
			_ResultLabel.Text = i > 0 ? "替换了 " + i + " 个匹配的书签。" : "没有替换任何书签。";
			_SearchTextBox.AddHistoryItem();
			_ReplaceTextBox.AddHistoryItem();
		}

		private void _CloseButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void MatchModeChanged(object sender, EventArgs e) {
			if (_NormalSearchBox.Checked) {
				_matcherType = BookmarkMatcher.MatcherType.Normal;
			}
			else if (_RegexSearchBox.Checked) {
				_matcherType = BookmarkMatcher.MatcherType.Regex;
			}
			else if (_XPathSearchBox.Checked) {
				_matcherType = BookmarkMatcher.MatcherType.XPath;
			}
			_MatchCaseBox.Enabled = _FullMatchBox.Enabled = _ReplaceButton.Enabled = _ReplaceTextBox.Enabled = !_XPathSearchBox.Checked;
		}

		private void ReplaceModeChanged(object sender, EventArgs e) {
			_replaceInSelection = _ReplaceInSelectionBox.Checked;
		}

		private int ReplaceBookmarks(bool replaceInSelection, BookmarkMatcher matcher, string replacement) {
			var b = _controller.View.Bookmark;
			if (b.GetItemCount() == 0) {
				return 0;
			}

			var si = new List<XmlNode>();
			var ol = replaceInSelection ? b.SelectedObjects : (b.GetModelObject(0) as XmlElement).ParentNode.SelectNodes(".//" + Constants.Bookmark) as IEnumerable;
			foreach (XmlNode item in ol) {
				si.Add(item);
			}
			var undo = new UndoActionGroup();
			var p = new ReplaceTitleTextProcessor(matcher, replacement);
			try {
				XmlElement x;
				foreach (XmlNode item in si) {
					x = item as XmlElement;
					if (x == null) {
						continue;
					}
					undo.Add(p.Process(x));
				}
			}
			catch (Exception ex) {
				Common.FormHelper.ErrorBox("在替换匹配文本时出现错误：" + ex.Message);
			}
			if (undo.Count > 0) {
				_controller.Model.Undo.AddUndo(p.Name, undo);
				si.Clear();
				si.AddRange(undo.AffectedElements);
				b.RefreshObjects(si);
				return si.Count;
			}
			return 0;
		}


	}
}
