namespace PDFPatcher.Functions.Editor;

partial class ReaderOptionForm
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing) {
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
			this._MainTab = new System.Windows.Forms.TabControl();
			this._DefaultPage = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this._ShowAnnotationBox = new System.Windows.Forms.CheckBox();
			this._ShowTextBorderBox = new System.Windows.Forms.CheckBox();
			this._GrayScaleBox = new System.Windows.Forms.CheckBox();
			this._FullPageScrollBox = new System.Windows.Forms.CheckBox();
			this._DirectionBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this._ZoomRateBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._BookmarkPage = new System.Windows.Forms.TabPage();
			this._LocateOnBookmarkEditBox = new System.Windows.Forms.CheckBox();
			this._AutoEditNextBookmarkBox = new System.Windows.Forms.CheckBox();
			this._BookmarkFontBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this._ShowBookmarkBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._MainTab.SuspendLayout();
			this._DefaultPage.SuspendLayout();
			this._BookmarkPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// _MainTab
			// 
			this._MainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._MainTab.Controls.Add(this._DefaultPage);
			this._MainTab.Controls.Add(this._BookmarkPage);
			this._MainTab.Location = new System.Drawing.Point(12, 12);
			this._MainTab.Name = "_MainTab";
			this._MainTab.SelectedIndex = 0;
			this._MainTab.Size = new System.Drawing.Size(450, 319);
			this._MainTab.TabIndex = 0;
			// 
			// _DefaultPage
			// 
			this._DefaultPage.Controls.Add(this.label5);
			this._DefaultPage.Controls.Add(this._ShowAnnotationBox);
			this._DefaultPage.Controls.Add(this._ShowTextBorderBox);
			this._DefaultPage.Controls.Add(this._GrayScaleBox);
			this._DefaultPage.Controls.Add(this._FullPageScrollBox);
			this._DefaultPage.Controls.Add(this._DirectionBox);
			this._DefaultPage.Controls.Add(this.label2);
			this._DefaultPage.Controls.Add(this._ZoomRateBox);
			this._DefaultPage.Controls.Add(this.label1);
			this._DefaultPage.Location = new System.Drawing.Point(4, 22);
			this._DefaultPage.Name = "_DefaultPage";
			this._DefaultPage.Padding = new System.Windows.Forms.Padding(3);
			this._DefaultPage.Size = new System.Drawing.Size(442, 293);
			this._DefaultPage.TabIndex = 0;
			this._DefaultPage.Text = "默认设置";
			this._DefaultPage.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.SystemColors.ControlLight;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Location = new System.Drawing.Point(6, 260);
			this.label5.Margin = new System.Windows.Forms.Padding(3);
			this.label5.Name = "label5";
			this.label5.Padding = new System.Windows.Forms.Padding(6);
			this.label5.Size = new System.Drawing.Size(430, 27);
			this.label5.TabIndex = 8;
			this.label5.Text = "说明：本页的设置仅在打开文档时应用，不影响已打开的文档。";
			// 
			// _ShowAnnotationBox
			// 
			this._ShowAnnotationBox.AutoSize = true;
			this._ShowAnnotationBox.Location = new System.Drawing.Point(8, 54);
			this._ShowAnnotationBox.Name = "_ShowAnnotationBox";
			this._ShowAnnotationBox.Size = new System.Drawing.Size(96, 16);
			this._ShowAnnotationBox.TabIndex = 5;
			this._ShowAnnotationBox.Text = "隐藏文档批注";
			this._ShowAnnotationBox.UseVisualStyleBackColor = true;
			// 
			// _ShowTextBorderBox
			// 
			this._ShowTextBorderBox.AutoSize = true;
			this._ShowTextBorderBox.Location = new System.Drawing.Point(8, 32);
			this._ShowTextBorderBox.Name = "_ShowTextBorderBox";
			this._ShowTextBorderBox.Size = new System.Drawing.Size(96, 16);
			this._ShowTextBorderBox.TabIndex = 4;
			this._ShowTextBorderBox.Text = "显示文本边框";
			this._ShowTextBorderBox.UseVisualStyleBackColor = true;
			// 
			// _GrayScaleBox
			// 
			this._GrayScaleBox.AutoSize = true;
			this._GrayScaleBox.Location = new System.Drawing.Point(8, 76);
			this._GrayScaleBox.Name = "_GrayScaleBox";
			this._GrayScaleBox.Size = new System.Drawing.Size(72, 16);
			this._GrayScaleBox.TabIndex = 6;
			this._GrayScaleBox.Text = "黑白显示";
			this._GrayScaleBox.UseVisualStyleBackColor = true;
			// 
			// _FullPageScrollBox
			// 
			this._FullPageScrollBox.AutoSize = true;
			this._FullPageScrollBox.Location = new System.Drawing.Point(8, 98);
			this._FullPageScrollBox.Name = "_FullPageScrollBox";
			this._FullPageScrollBox.Size = new System.Drawing.Size(108, 16);
			this._FullPageScrollBox.TabIndex = 7;
			this._FullPageScrollBox.Text = "翻页键整页翻页";
			this._FullPageScrollBox.UseVisualStyleBackColor = true;
			// 
			// _DirectionBox
			// 
			this._DirectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._DirectionBox.FormattingEnabled = true;
			this._DirectionBox.Items.AddRange(new object[] {
            "由文档设定",
            "从上到下",
            "从左到右水平",
            "从右到左"});
			this._DirectionBox.Location = new System.Drawing.Point(296, 6);
			this._DirectionBox.Name = "_DirectionBox";
			this._DirectionBox.Size = new System.Drawing.Size(121, 20);
			this._DirectionBox.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(225, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "阅读方向：";
			// 
			// _ZoomRateBox
			// 
			this._ZoomRateBox.FormattingEnabled = true;
			this._ZoomRateBox.Location = new System.Drawing.Point(77, 6);
			this._ZoomRateBox.Name = "_ZoomRateBox";
			this._ZoomRateBox.Size = new System.Drawing.Size(121, 20);
			this._ZoomRateBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "缩放比例：";
			// 
			// _BookmarkPage
			// 
			this._BookmarkPage.Controls.Add(this._LocateOnBookmarkEditBox);
			this._BookmarkPage.Controls.Add(this._AutoEditNextBookmarkBox);
			this._BookmarkPage.Controls.Add(this._BookmarkFontBox);
			this._BookmarkPage.Controls.Add(this.label4);
			this._BookmarkPage.Controls.Add(this._ShowBookmarkBox);
			this._BookmarkPage.Controls.Add(this.label3);
			this._BookmarkPage.Location = new System.Drawing.Point(4, 22);
			this._BookmarkPage.Name = "_BookmarkPage";
			this._BookmarkPage.Padding = new System.Windows.Forms.Padding(3);
			this._BookmarkPage.Size = new System.Drawing.Size(442, 293);
			this._BookmarkPage.TabIndex = 1;
			this._BookmarkPage.Text = "书签编辑器";
			this._BookmarkPage.UseVisualStyleBackColor = true;
			// 
			// _LocateOnBookmarkEditBox
			// 
			this._LocateOnBookmarkEditBox.AutoSize = true;
			this._LocateOnBookmarkEditBox.Location = new System.Drawing.Point(8, 32);
			this._LocateOnBookmarkEditBox.Name = "_LocateOnBookmarkEditBox";
			this._LocateOnBookmarkEditBox.Size = new System.Drawing.Size(204, 16);
			this._LocateOnBookmarkEditBox.TabIndex = 4;
			this._LocateOnBookmarkEditBox.Text = "编辑书签文本时，滚动到对应页面";
			this._LocateOnBookmarkEditBox.UseVisualStyleBackColor = true;
			// 
			// _AutoEditNextBookmarkBox
			// 
			this._AutoEditNextBookmarkBox.AutoSize = true;
			this._AutoEditNextBookmarkBox.Location = new System.Drawing.Point(8, 54);
			this._AutoEditNextBookmarkBox.Name = "_AutoEditNextBookmarkBox";
			this._AutoEditNextBookmarkBox.Size = new System.Drawing.Size(204, 16);
			this._AutoEditNextBookmarkBox.TabIndex = 5;
			this._AutoEditNextBookmarkBox.Text = "修改书签文本后，编辑下一个书签";
			this._AutoEditNextBookmarkBox.UseVisualStyleBackColor = true;
			// 
			// _BookmarkFontBox
			// 
			this._BookmarkFontBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._BookmarkFontBox.FormattingEnabled = true;
			this._BookmarkFontBox.Location = new System.Drawing.Point(296, 6);
			this._BookmarkFontBox.Name = "_BookmarkFontBox";
			this._BookmarkFontBox.Size = new System.Drawing.Size(121, 20);
			this._BookmarkFontBox.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(225, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "书签字体：";
			// 
			// _ShowBookmarkBox
			// 
			this._ShowBookmarkBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ShowBookmarkBox.FormattingEnabled = true;
			this._ShowBookmarkBox.Items.AddRange(new object[] {
            "由文档设定",
            "显示",
            "隐藏"});
			this._ShowBookmarkBox.Location = new System.Drawing.Point(77, 6);
			this._ShowBookmarkBox.Name = "_ShowBookmarkBox";
			this._ShowBookmarkBox.Size = new System.Drawing.Size(121, 20);
			this._ShowBookmarkBox.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "书签栏：";
			// 
			// ReaderOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(474, 343);
			this.Controls.Add(this._MainTab);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReaderOptionForm";
			this.ShowInTaskbar = false;
			this.Text = "阅读器选项";
			this._MainTab.ResumeLayout(false);
			this._DefaultPage.ResumeLayout(false);
			this._DefaultPage.PerformLayout();
			this._BookmarkPage.ResumeLayout(false);
			this._BookmarkPage.PerformLayout();
			this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.TabControl _MainTab;
	private System.Windows.Forms.TabPage _DefaultPage;
	private System.Windows.Forms.CheckBox _GrayScaleBox;
	private System.Windows.Forms.CheckBox _FullPageScrollBox;
	private System.Windows.Forms.ComboBox _DirectionBox;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.ComboBox _ZoomRateBox;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.CheckBox _ShowAnnotationBox;
	private System.Windows.Forms.CheckBox _ShowTextBorderBox;
	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.TabPage _BookmarkPage;
	private System.Windows.Forms.CheckBox _AutoEditNextBookmarkBox;
	private System.Windows.Forms.ComboBox _BookmarkFontBox;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.ComboBox _ShowBookmarkBox;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.CheckBox _LocateOnBookmarkEditBox;
}