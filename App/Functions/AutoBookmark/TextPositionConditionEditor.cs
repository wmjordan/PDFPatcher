using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	public partial class TextPositionConditionEditor : UserControl, IFilterConditionEditor
	{
		AutoBookmarkCondition.TextPositionCondition _condition;
		bool _lock;

		public TextPositionConditionEditor() {
			InitializeComponent();
			_lock = true;
			_PositionBox.SelectedIndex = 0;
			_lock = false;
		}

		#region ITextInfoFilterEditor 成员

		public AutoBookmarkCondition Filter {
			get => _condition;
			set {
				_condition = (AutoBookmarkCondition.TextPositionCondition)value;
				_lock = true;
				_PositionBox.SelectedIndex = _condition.Position - 1;
				if (_condition.MinValue == _condition.MaxValue) {
					_SpecificBox.Checked = true;
					_SpecificValueBox.Value = (decimal)_condition.MaxValue;
				}
				else {
					_RangeBox.Checked = true;
					_MaxBox.Value = (decimal)_condition.MaxValue;
					_MinBox.Value = (decimal)_condition.MinValue;
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
			float min, max;
			if (_SpecificBox.Checked) {
				min = max = (float)_SpecificValueBox.Value;
			}
			else /*if (_YRangeBox.Checked)*/ {
				min = (float)_MinBox.Value;
				max = (float)_MaxBox.Value;
			}
			_condition.SetRange((byte)(_PositionBox.SelectedIndex + 1), min, max);
			EditAdjustmentForm.UpdateFilter(this);
		}

		private void ToggleControlState() {
			_MinBox.Enabled = _MaxBox.Enabled = _RangeBox.Checked;
			_SpecificValueBox.Enabled = _SpecificBox.Checked;
		}
	}
}
