﻿using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

public partial class PageRangeConditionEditor : UserControl, IFilterConditionEditor
{
	private AutoBookmarkCondition.PageRangeCondition _condition;
	private bool _lock;

	public PageRangeConditionEditor() {
		InitializeComponent();
	}

	private void ControlChanged(object sender, EventArgs e) {
		if (_lock) {
			return;
		}

		_condition.PageRange = _PageRangeBox.Text;
		EditAdjustmentForm.UpdateFilter(this);
	}

	#region ITextInfoFilterEditor 成员

	public AutoBookmarkCondition Filter {
		get => _condition;
		set {
			_condition = (AutoBookmarkCondition.PageRangeCondition)value;
			_lock = true;
			_PageRangeBox.Text = _condition.PageRange;
			_lock = false;
		}
	}

	public UserControl EditorControl => this;

	#endregion
}