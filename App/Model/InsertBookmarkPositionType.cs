using System;

namespace PDFPatcher.Model
{
	public enum InsertBookmarkPositionType
	{
		Undefined,
		AfterCurrent,
		AsChild,
		AfterParent,
		BeforeCurrent,
		AfterGrandParent,
		LastRoot
	}
}
