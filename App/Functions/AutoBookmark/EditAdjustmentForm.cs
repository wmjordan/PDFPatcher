using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

public partial class EditAdjustmentForm : Form
{
	internal static string[] FilterNames = { "字体名称", "文本尺寸", "文本位置", "页码范围", "文本内容" };

	internal static string[] FilterIDs = {
		"_FontNameFilter", "_FontSizeFilter", "_FontPositionFilter", "_PageRangeFilter", "_TextFilter"
	};

	private readonly Dictionary<Type, IFilterConditionEditor> _filterEditors = new();

	private AutoBookmarkCondition.MultiCondition conditions;

	public EditAdjustmentForm(AutoBookmarkOptions.LevelAdjustmentOption filter) {
		InitializeComponent();
		int i = 0;
		foreach (string item in FilterNames) {
			_AddFilterMenuItem.DropDownItems.Add(item).Name = FilterIDs[i++];
		}

		_FilterBox.BeforeSorting += (sender, e) => e.Canceled = true;
		_ConditionColumn.AspectGetter = x => x is AutoBookmarkCondition f ? f.Description : (object)null;
		_IsInclusiveColumn.AspectGetter = x => {
			if (x is AutoBookmarkCondition f) {
				return f.IsInclusive ? "包含匹配项" : "过滤匹配项";
			}

			return null;
		};
		_TypeColumn.AspectGetter = x => x is AutoBookmarkCondition f ? f.Name : null;
		Filter = new AutoBookmarkOptions.LevelAdjustmentOption();
		if (filter != null) {
			Filter.AdjustmentLevel = filter.AdjustmentLevel;
			Filter.RelativeAdjustment = filter.RelativeAdjustment;
			conditions = new AutoBookmarkCondition.MultiCondition(filter.Condition);
			_FilterBox.Objects = conditions.Conditions;
		}

		if (_FilterBox.Items.Count > 0) {
			_FilterBox.SelectedIndex = 0;
		}
	}

	internal AutoBookmarkOptions.LevelAdjustmentOption Filter { get; }

	private void EditAdjustmentForm_Load(object sender, EventArgs e) {
	}

	protected void _OkButton_Click(Object source, EventArgs args) {
		DialogResult = DialogResult.OK;
		conditions = new AutoBookmarkCondition.MultiCondition();
		foreach (ListViewItem item in _FilterBox.Items) {
			conditions.Conditions.Add((AutoBookmarkCondition)_FilterBox.GetModelObject(item.Index));
		}

		Filter.Condition = conditions.Conditions.Count switch {
			1 => conditions.Conditions[0],
			0 => null,
			_ => conditions
		};
		Close();
	}

	protected void _CancelButton_Click(Object source, EventArgs args) {
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void _AddFilterMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		AutoBookmarkCondition c = CreateCondition(e.ClickedItem.Name);
		if (c != null) {
			_FilterBox.AddObject(c);
		}
	}

	private void _FilterBox_SelectedIndexChanged(object sender, EventArgs e) {
		_EditFilterPanel.Controls.Clear();
		object o = _FilterBox.SelectedObject;
		if (o == null) {
			return;
		}

		IFilterConditionEditor ed = GetFilterEditor(o as AutoBookmarkCondition);
		if (ed == null) {
			return;
		}

		_EditFilterPanel.Controls.Add(ed.EditorControl);
		ed.EditorControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
		ed.EditorControl.Left = ed.EditorControl.Top = 0;
		ed.EditorControl.Size = _EditFilterPanel.ClientSize;
	}

	internal static AutoBookmarkCondition CreateCondition(string name) {
		return name switch {
			"_FontNameFilter" => new AutoBookmarkCondition.FontNameCondition("SimSun", false),
			"_FontSizeFilter" => new AutoBookmarkCondition.TextSizeCondition(0, 10),
			"_FontPositionFilter" => new AutoBookmarkCondition.TextPositionCondition(1, -9999, 9999),
			"_PageRangeFilter" => new AutoBookmarkCondition.PageRangeCondition(),
			"_TextFilter" => new AutoBookmarkCondition.TextCondition {
				Pattern = new MatchPattern("筛选条件", false, false, false)
			},
			_ => null
		};
	}

	internal static void UpdateFilter(IFilterConditionEditor filter) {
		if (filter.EditorControl?.FindForm() is EditAdjustmentForm f) {
			f._FilterBox.RefreshSelectedObjects();
		}
	}

	private IFilterConditionEditor GetFilterEditor(AutoBookmarkCondition filter) {
		Type t = filter.GetType();
		if (_filterEditors.TryGetValue(t, out IFilterConditionEditor c)) {
			goto SetEditor;
			// return c;
		}

		if (t == typeof(AutoBookmarkCondition.FontNameCondition)) {
			c = new FontNameConditionEditor();
		}
		else if (t == typeof(AutoBookmarkCondition.TextSizeCondition)) {
			c = new TextSizeConditionEditor();
		}
		else if (t == typeof(AutoBookmarkCondition.TextPositionCondition)) {
			c = new TextPositionConditionEditor();
		}
		else if (t == typeof(AutoBookmarkCondition.PageRangeCondition)) {
			c = new PageRangeConditionEditor();
		}
		else if (t == typeof(AutoBookmarkCondition.TextCondition)) {
			c = new TextConditionEditor();
		}
		else {
			FormHelper.ErrorBox("无法编辑选中的筛选条件。");
			return null;
		}

	SetEditor:
		_filterEditors[t] = c;
		c.Filter = filter;
		return c;
	}

	private void _RemoveButton_Click(object sender, EventArgs e) {
		_FilterBox.RemoveObjects(_FilterBox.SelectedObjects);
	}
}