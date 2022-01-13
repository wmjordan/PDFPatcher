﻿using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher;

internal sealed class ListViewItemComparer : System.Collections.IComparer
{
	///<summary>获取或指定排序列的值。</summary>
	public int Col { get; }

	///<summary>获取或指定是否使用智能排序。</summary>
	public bool UseSmartSort { get; }

	///<summary>获取或指定列表排序的方式。</summary>
	public SortOrder SortOrder { get; }

	public ListViewItemComparer() {
		Col = 0;
	}

	public ListViewItemComparer(int column) {
		Col = column;
	}

	public ListViewItemComparer(int column, bool useSmartSort) {
		Col = column;
		UseSmartSort = useSmartSort;
		SortOrder = SortOrder.Ascending;
	}

	public ListViewItemComparer(int column, bool useSmartSort, SortOrder sortOrder) {
		Col = column;
		UseSmartSort = useSmartSort;
		SortOrder = sortOrder;
	}

	#region IComparer 成员

	int System.Collections.IComparer.Compare(object x, object y) {
		if (SortOrder == SortOrder.None) {
			return 0;
		}

		string a = ((ListViewItem)x).SubItems[Col].Text;
		string b = ((ListViewItem)y).SubItems[Col].Text;
		int r = UseSmartSort ? FileHelper.NumericAwareComparePath(a, b) : string.Compare(a, b);
		return SortOrder == SortOrder.Ascending ? r : -r;
	}

	#endregion
}

internal sealed class OlvColumnSmartComparer : System.Collections.IComparer
{
	///<summary>获取排序列。</summary>
	public BrightIdeasSoftware.OLVColumn Column { get; }

	///<summary>获取列表排序的方式。</summary>
	public SortOrder SortOrder { get; }

	public OlvColumnSmartComparer(BrightIdeasSoftware.OLVColumn column, SortOrder sortOrder) {
		Column = column;
		SortOrder = sortOrder;
	}

	#region IComparer 成员

	int System.Collections.IComparer.Compare(object x, object y) {
		if (SortOrder == SortOrder.None) {
			return 0;
		}

		string a = Column.GetStringValue(x);
		string b = Column.GetStringValue(y);
		int r = FileHelper.NumericAwareComparePath(a, b);
		return SortOrder == SortOrder.Ascending ? r : -r;
	}

	#endregion
}