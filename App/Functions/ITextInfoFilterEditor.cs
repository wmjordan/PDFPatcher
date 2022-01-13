using System;

namespace PDFPatcher.Functions
{
	interface IFilterConditionEditor
	{
		PDFPatcher.Model.AutoBookmarkCondition Filter { get; set; }
		System.Windows.Forms.UserControl EditorControl { get; }
	}
}