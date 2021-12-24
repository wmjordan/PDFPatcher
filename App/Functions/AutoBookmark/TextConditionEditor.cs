using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem (false)]
	public partial class TextConditionEditor : UserControl, IFilterConditionEditor
	{
		AutoBookmarkCondition.TextCondition _filter;
		bool _lock;

		public TextConditionEditor () {
			InitializeComponent ();
		}

		#region ITextInfoFilterEditor 成员
		public UserControl EditorControl => this;

		public AutoBookmarkCondition Filter {
			get => _filter;
			set {
				_filter = (AutoBookmarkCondition.TextCondition)value;
				_lock = true;
				_PatternBox.Text = _filter.Pattern.Text;
				_FullMatchBox.Checked = _filter.Pattern.FullMatch;
				_MatchCaseBox.Checked = _filter.Pattern.MatchCase;
				_UseRegexBox.Checked = _filter.Pattern.UseRegularExpression;
				_lock = false;
			}
		}

		#endregion

		private void ControlChanged (object sender, EventArgs e) {
			if (_lock == false) {
				if (sender == _PatternBox) {
					_filter.Pattern.Text = _PatternBox.Text;
				}
				else if (sender == _FullMatchBox) {
					_filter.Pattern.FullMatch = _FullMatchBox.Checked;
				}
				else if (sender == _MatchCaseBox) {
					_filter.Pattern.MatchCase = _MatchCaseBox.Checked;
				}
				else if (sender == _UseRegexBox) {
					_filter.Pattern.UseRegularExpression = _UseRegexBox.Checked;
				}
				EditAdjustmentForm.UpdateFilter (this);
			}
		}
	}
}
