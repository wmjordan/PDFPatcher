using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	sealed partial class FontNameConditionEditor : UserControl, IFilterConditionEditor
	{
		AutoBookmarkCondition.FontNameCondition _filter;
		bool _lock;

		public FontNameConditionEditor() {
			InitializeComponent();
		}

		#region ITextInfoFilterEditor 成员
		public UserControl EditorControl => this;

		public AutoBookmarkCondition Filter {
			get => _filter;
			set {
				_filter = (AutoBookmarkCondition.FontNameCondition)value;
				_lock = true;
				_FontNameBox.Text = _filter.FontName;
				_FullMatchBox.Checked = _filter.MatchFullName;
				_lock = false;
			}
		}

		#endregion

		void ControlChanged(object sender, EventArgs e) {
			if (_lock) {
				return;
			}

			if (sender == _FontNameBox) {
				_filter.FontName = _FontNameBox.Text;
			}
			else if (sender == _FullMatchBox) {
				_filter.MatchFullName = _FullMatchBox.Checked;
			}
			EditAdjustmentForm.UpdateFilter(this);
		}
	}
}
