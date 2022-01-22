using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

internal interface IFilterConditionEditor
{
    AutoBookmarkCondition Filter { get; set; }
    UserControl EditorControl { get; }
}