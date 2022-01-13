using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Model;

public enum InsertBookmarkPositionType
{
	NoDefined,
	AfterCurrent,
	AsChild,
	AfterParent,
	BeforeCurrent
}