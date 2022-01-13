using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	public partial class TextSizeConditionEditor : UserControl, IFilterConditionEditor
	{
		AutoBookmarkCondition.TextSizeCondition _condition;
		bool _lock;

		public TextSizeConditionEditor() {
			InitializeComponent();
		}

		#region ITextInfoFilterEditor 成员

		public AutoBookmarkCondition Filter {
			get => _condition;
			set {
				_condition = (AutoBookmarkCondition.TextSizeCondition)value;
				_lock = true;
				if (_condition.MinSize == _condition.MaxSize) {
					_SizeBox.Checked = true;
					_SpecificSizeBox.Value = (decimal)_condition.MaxSize;
				}
				else {
					_SizeRangeBox.Checked = true;
					_MaxSizeBox.Value = (decimal)_condition.MaxSize;
					_MinSizeBox.Value = (decimal)_condition.MinSize;
				}

				ToggleControlState();
				_lock = false;
			}
		}

		public UserControl EditorControl => this;

		#endregion

		private void ControlChanged(object sender, EventArgs e) {
			ToggleControlState();
			if (_lock) {
				return;
			}

			if (_SizeBox.Checked) {
				_condition.SetRange((float)_SpecificSizeBox.Value, (float)_SpecificSizeBox.Value);
			}
			else if (_SizeRangeBox.Checked) {
				_condition.SetRange((float)_MinSizeBox.Value, (float)_MaxSizeBox.Value);
			}

			EditAdjustmentForm.UpdateFilter(this);
		}

		private void ToggleControlState() {
			_MinSizeBox.Enabled = _MaxSizeBox.Enabled = _SizeRangeBox.Checked;
			_SpecificSizeBox.Enabled = _SizeBox.Checked;
		}
	}
}