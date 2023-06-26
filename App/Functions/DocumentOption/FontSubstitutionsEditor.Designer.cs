namespace PDFPatcher.Functions
{
	partial class FontSubstitutionsEditor
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent () {
			this.components = new System.ComponentModel.Container();
			this._FontSubstitutionsBox = new BrightIdeasSoftware.ObjectListView();
			this._SequenceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._OriginalFontColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._SubstitutionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._CharSubstitutionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._FontSubstitutionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._CopySubstitutionFont = new System.Windows.Forms.ToolStripMenuItem();
			this._PasteSubstitutionFont = new System.Windows.Forms.ToolStripMenuItem();
			this._RemoveSubstitutionButton = new System.Windows.Forms.Button();
			this._AddSubstitutionButton = new System.Windows.Forms.Button();
			this._ListDocumentFontButton = new System.Windows.Forms.Button();
			this._EmbedLegacyCjkFontsBox = new System.Windows.Forms.CheckBox();
			this._EnableFontSubstitutionsBox = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this._TrimTrailingWhiteSpaceBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._FontSubstitutionsBox)).BeginInit();
			this._FontSubstitutionMenu.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _FontSubstitutionsBox
			// 
			this._FontSubstitutionsBox.AllColumns.Add(this._SequenceColumn);
			this._FontSubstitutionsBox.AllColumns.Add(this._OriginalFontColumn);
			this._FontSubstitutionsBox.AllColumns.Add(this._SubstitutionColumn);
			this._FontSubstitutionsBox.AllColumns.Add(this._CharSubstitutionColumn);
			this._FontSubstitutionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._FontSubstitutionsBox.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._FontSubstitutionsBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._SequenceColumn,
            this._OriginalFontColumn,
            this._SubstitutionColumn,
            this._CharSubstitutionColumn});
			this._FontSubstitutionsBox.ContextMenuStrip = this._FontSubstitutionMenu;
			this._FontSubstitutionsBox.Cursor = System.Windows.Forms.Cursors.Default;
			this._FontSubstitutionsBox.Enabled = false;
			this._FontSubstitutionsBox.GridLines = true;
			this._FontSubstitutionsBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._FontSubstitutionsBox.HideSelection = false;
			this._FontSubstitutionsBox.LabelEdit = true;
			this._FontSubstitutionsBox.Location = new System.Drawing.Point(4, 69);
			this._FontSubstitutionsBox.Margin = new System.Windows.Forms.Padding(4);
			this._FontSubstitutionsBox.Name = "_FontSubstitutionsBox";
			this._FontSubstitutionsBox.SelectColumnsOnRightClick = false;
			this._FontSubstitutionsBox.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
			this._FontSubstitutionsBox.ShowGroups = false;
			this._FontSubstitutionsBox.Size = new System.Drawing.Size(575, 275);
			this._FontSubstitutionsBox.TabIndex = 5;
			this._FontSubstitutionsBox.UseCompatibleStateImageBehavior = false;
			this._FontSubstitutionsBox.View = System.Windows.Forms.View.Details;
			// 
			// _SequenceColumn
			// 
			this._SequenceColumn.Text = "序号";
			this._SequenceColumn.Width = 40;
			// 
			// _OriginalFontColumn
			// 
			this._OriginalFontColumn.AspectName = "";
			this._OriginalFontColumn.Text = "原字体";
			this._OriginalFontColumn.Width = 160;
			// 
			// _SubstitutionColumn
			// 
			this._SubstitutionColumn.AspectName = "";
			this._SubstitutionColumn.Text = "替换字体";
			this._SubstitutionColumn.Width = 160;
			// 
			// _CharSubstitutionColumn
			// 
			this._CharSubstitutionColumn.Text = "替换字符";
			this._CharSubstitutionColumn.Width = 71;
			// 
			// _FontSubstitutionMenu
			// 
			this._FontSubstitutionMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._FontSubstitutionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._CopySubstitutionFont,
            this._PasteSubstitutionFont});
			this._FontSubstitutionMenu.Name = "_FontSubstitutionMenu";
			this._FontSubstitutionMenu.Size = new System.Drawing.Size(192, 56);
			this._FontSubstitutionMenu.Opening += new System.ComponentModel.CancelEventHandler(this._FontSubstitutionMenu_Opening);
			this._FontSubstitutionMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._FontSubstitutionMenu_ItemClicked);
			// 
			// _CopySubstitutionFont
			// 
			this._CopySubstitutionFont.Image = global::PDFPatcher.Properties.Resources.Copy;
			this._CopySubstitutionFont.Name = "_CopySubstitutionFont";
			this._CopySubstitutionFont.Size = new System.Drawing.Size(191, 26);
			this._CopySubstitutionFont.Text = "复制替换字体(&F)";
			// 
			// _PasteSubstitutionFont
			// 
			this._PasteSubstitutionFont.Image = global::PDFPatcher.Properties.Resources.Paste;
			this._PasteSubstitutionFont.Name = "_PasteSubstitutionFont";
			this._PasteSubstitutionFont.Size = new System.Drawing.Size(191, 26);
			this._PasteSubstitutionFont.Text = "粘贴替换字体(&Z)";
			// 
			// _RemoveSubstitutionButton
			// 
			this._RemoveSubstitutionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._RemoveSubstitutionButton.Enabled = false;
			this._RemoveSubstitutionButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._RemoveSubstitutionButton.Location = new System.Drawing.Point(509, 4);
			this._RemoveSubstitutionButton.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveSubstitutionButton.Name = "_RemoveSubstitutionButton";
			this._RemoveSubstitutionButton.Size = new System.Drawing.Size(71, 29);
			this._RemoveSubstitutionButton.TabIndex = 4;
			this._RemoveSubstitutionButton.Text = "删除";
			this._RemoveSubstitutionButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._RemoveSubstitutionButton.UseVisualStyleBackColor = true;
			this._RemoveSubstitutionButton.Click += new System.EventHandler(this._RemoveSubstitutionButton_Click);
			// 
			// _AddSubstitutionButton
			// 
			this._AddSubstitutionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._AddSubstitutionButton.Enabled = false;
			this._AddSubstitutionButton.Image = global::PDFPatcher.Properties.Resources.Add;
			this._AddSubstitutionButton.Location = new System.Drawing.Point(431, 4);
			this._AddSubstitutionButton.Margin = new System.Windows.Forms.Padding(4);
			this._AddSubstitutionButton.Name = "_AddSubstitutionButton";
			this._AddSubstitutionButton.Size = new System.Drawing.Size(71, 29);
			this._AddSubstitutionButton.TabIndex = 3;
			this._AddSubstitutionButton.Text = "添加";
			this._AddSubstitutionButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._AddSubstitutionButton.UseVisualStyleBackColor = true;
			this._AddSubstitutionButton.Click += new System.EventHandler(this._AddSubstitutionButton_Click);
			// 
			// _ListDocumentFontButton
			// 
			this._ListDocumentFontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ListDocumentFontButton.Location = new System.Drawing.Point(276, 4);
			this._ListDocumentFontButton.Margin = new System.Windows.Forms.Padding(4);
			this._ListDocumentFontButton.Name = "_ListDocumentFontButton";
			this._ListDocumentFontButton.Size = new System.Drawing.Size(147, 29);
			this._ListDocumentFontButton.TabIndex = 2;
			this._ListDocumentFontButton.Text = "列出文档字体";
			this._ListDocumentFontButton.UseVisualStyleBackColor = true;
			this._ListDocumentFontButton.Click += new System.EventHandler(this._ListDocumentFontButton_Click);
			// 
			// _EmbedLegacyCjkFontsBox
			// 
			this._EmbedLegacyCjkFontsBox.AutoSize = true;
			this._EmbedLegacyCjkFontsBox.Location = new System.Drawing.Point(4, 9);
			this._EmbedLegacyCjkFontsBox.Margin = new System.Windows.Forms.Padding(4);
			this._EmbedLegacyCjkFontsBox.Name = "_EmbedLegacyCjkFontsBox";
			this._EmbedLegacyCjkFontsBox.Size = new System.Drawing.Size(104, 19);
			this._EmbedLegacyCjkFontsBox.TabIndex = 0;
			this._EmbedLegacyCjkFontsBox.Text = "嵌入汉字库";
			this._EmbedLegacyCjkFontsBox.UseVisualStyleBackColor = true;
			// 
			// _EnableFontSubstitutionsBox
			// 
			this._EnableFontSubstitutionsBox.AutoSize = true;
			this._EnableFontSubstitutionsBox.Location = new System.Drawing.Point(124, 9);
			this._EnableFontSubstitutionsBox.Margin = new System.Windows.Forms.Padding(4);
			this._EnableFontSubstitutionsBox.Name = "_EnableFontSubstitutionsBox";
			this._EnableFontSubstitutionsBox.Size = new System.Drawing.Size(119, 19);
			this._EnableFontSubstitutionsBox.TabIndex = 1;
			this._EnableFontSubstitutionsBox.Text = "允许替换字体";
			this._EnableFontSubstitutionsBox.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._TrimTrailingWhiteSpaceBox);
			this.panel1.Controls.Add(this._FontSubstitutionsBox);
			this.panel1.Controls.Add(this._EnableFontSubstitutionsBox);
			this.panel1.Controls.Add(this._AddSubstitutionButton);
			this.panel1.Controls.Add(this._EmbedLegacyCjkFontsBox);
			this.panel1.Controls.Add(this._RemoveSubstitutionButton);
			this.panel1.Controls.Add(this._ListDocumentFontButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(584, 349);
			this.panel1.TabIndex = 6;
			// 
			// _TrimTrailingWhiteSpaceBox
			// 
			this._TrimTrailingWhiteSpaceBox.AutoSize = true;
			this._TrimTrailingWhiteSpaceBox.Location = new System.Drawing.Point(4, 35);
			this._TrimTrailingWhiteSpaceBox.Name = "_TrimTrailingWhiteSpaceBox";
			this._TrimTrailingWhiteSpaceBox.Size = new System.Drawing.Size(179, 19);
			this._TrimTrailingWhiteSpaceBox.TabIndex = 6;
			this._TrimTrailingWhiteSpaceBox.Text = "同时删除文本尾随空格";
			this._TrimTrailingWhiteSpaceBox.UseVisualStyleBackColor = true;
			// 
			// FontSubstitutionsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FontSubstitutionsEditor";
			this.Size = new System.Drawing.Size(584, 349);
			this.Load += new System.EventHandler(this.FontSubstitutionsEditor_Load);
			((System.ComponentModel.ISupportInitialize)(this._FontSubstitutionsBox)).EndInit();
			this._FontSubstitutionMenu.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _RemoveSubstitutionButton;
		private System.Windows.Forms.Button _AddSubstitutionButton;
		private BrightIdeasSoftware.ObjectListView _FontSubstitutionsBox;
		private BrightIdeasSoftware.OLVColumn _OriginalFontColumn;
		private BrightIdeasSoftware.OLVColumn _SubstitutionColumn;
		private BrightIdeasSoftware.OLVColumn _SequenceColumn;
		private System.Windows.Forms.Button _ListDocumentFontButton;
		private System.Windows.Forms.CheckBox _EmbedLegacyCjkFontsBox;
		private System.Windows.Forms.CheckBox _EnableFontSubstitutionsBox;
		private System.Windows.Forms.ContextMenuStrip _FontSubstitutionMenu;
		private System.Windows.Forms.ToolStripMenuItem _CopySubstitutionFont;
		private System.Windows.Forms.ToolStripMenuItem _PasteSubstitutionFont;
		private System.Windows.Forms.Panel panel1;
		private BrightIdeasSoftware.OLVColumn _CharSubstitutionColumn;
		private System.Windows.Forms.CheckBox _TrimTrailingWhiteSpaceBox;
	}
}
