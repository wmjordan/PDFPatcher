using System.Drawing;

namespace Cyotek.Windows.Forms.Demo;
// Cyotek ImageBox
// Copyright (c) 2010-2015 Cyotek Ltd.
// http://cyotek.com
// http://cyotek.com/blog/tag/imagebox

// Licensed under the MIT License. See license.txt for the full text.

// If you use this control in your applications, attribution, donations or contributions are welcome.

internal sealed class DragHandle
{
    #region Public Constructors

    public DragHandle(DragHandleAnchor anchor)
        : this() {
        Anchor = anchor;
    }

    #endregion

    #region Protected Constructors

    private DragHandle() {
        Enabled = true;
        Visible = true;
    }

    #endregion

    #region Public Properties

    public DragHandleAnchor Anchor { get; set; }

    public Rectangle Bounds { get; set; }

    public bool Enabled { get; set; }

    public bool Visible { get; set; }

    #endregion
}