using System;

namespace PDFPatcher.Functions;

internal interface IFilterConditionEditor
{
	Model.AutoBookmarkCondition Filter { get; set; }
	System.Windows.Forms.UserControl EditorControl { get; }
}