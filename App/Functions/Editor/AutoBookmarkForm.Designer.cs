namespace PDFPatcher.Functions
{
	partial class AutoBookmarkForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			this.label1 = new System.Windows.Forms.Label();
			this._BookmarkConditionBox = new BrightIdeasSoftware.ObjectListView();
			this._ConditionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._LevelColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._BoldColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItalicColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ColorColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._OpenColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._RemoveButton = new System.Windows.Forms.Button();
			this._AutoBookmarkButton = new System.Windows.Forms.Button();
			this._MergeAdjacentTitleBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._BookmarkConditionBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 11);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "书签识别条件列表：";
			// 
			// _BookmarkConditionBox
			// 
			this._BookmarkConditionBox.AllColumns.Add(this._ConditionColumn);
			this._BookmarkConditionBox.AllColumns.Add(this._LevelColumn);
			this._BookmarkConditionBox.AllColumns.Add(this._BoldColumn);
			this._BookmarkConditionBox.AllColumns.Add(this._ItalicColumn);
			this._BookmarkConditionBox.AllColumns.Add(this._ColorColumn);
			this._BookmarkConditionBox.AllColumns.Add(this._OpenColumn);
			this._BookmarkConditionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._BookmarkConditionBox.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._BookmarkConditionBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._ConditionColumn,
            this._LevelColumn,
            this._BoldColumn,
            this._ItalicColumn,
            this._ColorColumn,
            this._OpenColumn});
			this._BookmarkConditionBox.Cursor = System.Windows.Forms.Cursors.Default;
			this._BookmarkConditionBox.GridLines = true;
			this._BookmarkConditionBox.HideSelection = false;
			this._BookmarkConditionBox.Location = new System.Drawing.Point(19, 30);
			this._BookmarkConditionBox.Margin = new System.Windows.Forms.Padding(4);
			this._BookmarkConditionBox.Name = "_BookmarkConditionBox";
			this._BookmarkConditionBox.ShowGroups = false;
			this._BookmarkConditionBox.Size = new System.Drawing.Size(580, 206);
			this._BookmarkConditionBox.TabIndex = 1;
			this._BookmarkConditionBox.UseCompatibleStateImageBehavior = false;
			this._BookmarkConditionBox.View = System.Windows.Forms.View.Details;
			// 
			// _ConditionColumn
			// 
			this._ConditionColumn.IsEditable = false;
			this._ConditionColumn.Text = "识别条件";
			this._ConditionColumn.Width = 244;
			// 
			// _LevelColumn
			// 
			this._LevelColumn.Text = "书签级别";
			this._LevelColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._LevelColumn.Width = 80;
			// 
			// _BoldColumn
			// 
			this._BoldColumn.CheckBoxes = true;
			this._BoldColumn.Text = "粗体";
			this._BoldColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// _ItalicColumn
			// 
			this._ItalicColumn.CheckBoxes = true;
			this._ItalicColumn.Text = "斜体";
			this._ItalicColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// _ColorColumn
			// 
			this._ColorColumn.IsEditable = false;
			this._ColorColumn.Text = "颜色";
			// 
			// _OpenColumn
			// 
			this._OpenColumn.CheckBoxes = true;
			this._OpenColumn.Text = "默认展开";
			this._OpenColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._OpenColumn.Width = 73;
			// 
			// _RemoveButton
			// 
			this._RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._RemoveButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._RemoveButton.Location = new System.Drawing.Point(358, 5);
			this._RemoveButton.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveButton.Name = "_RemoveButton";
			this._RemoveButton.Size = new System.Drawing.Size(100, 29);
			this._RemoveButton.TabIndex = 2;
			this._RemoveButton.Text = "删除(&S)";
			this._RemoveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._RemoveButton.UseVisualStyleBackColor = true;
			this._RemoveButton.Click += new System.EventHandler(this._RemoveButton_Click);
			// 
			// _AutoBookmarkButton
			// 
			this._AutoBookmarkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._AutoBookmarkButton.Image = global::PDFPatcher.Properties.Resources.AutoBookmark;
			this._AutoBookmarkButton.Location = new System.Drawing.Point(466, 5);
			this._AutoBookmarkButton.Margin = new System.Windows.Forms.Padding(4);
			this._AutoBookmarkButton.Name = "_AutoBookmarkButton";
			this._AutoBookmarkButton.Size = new System.Drawing.Size(135, 29);
			this._AutoBookmarkButton.TabIndex = 4;
			this._AutoBookmarkButton.Text = "生成书签(&K)";
			this._AutoBookmarkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._AutoBookmarkButton.UseVisualStyleBackColor = true;
			this._AutoBookmarkButton.Click += new System.EventHandler(this._AutoBookmarkButton_Click);
			// 
			// _MergeAdjacentTitleBox
			// 
			this._MergeAdjacentTitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._MergeAdjacentTitleBox.AutoSize = true;
			this._MergeAdjacentTitleBox.Checked = true;
			this._MergeAdjacentTitleBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._MergeAdjacentTitleBox.Location = new System.Drawing.Point(19, 251);
			this._MergeAdjacentTitleBox.Name = "_MergeAdjacentTitleBox";
			this._MergeAdjacentTitleBox.Size = new System.Drawing.Size(209, 19);
			this._MergeAdjacentTitleBox.TabIndex = 5;
			this._MergeAdjacentTitleBox.Text = "合并同字体尺寸的相邻标题";
			this._MergeAdjacentTitleBox.UseVisualStyleBackColor = true;
			// 
			// AutoBookmarkForm
			// 
			this.AcceptButton = this._AutoBookmarkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(624, 282);
			this.Controls.Add(this._MergeAdjacentTitleBox);
			this.Controls.Add(this._AutoBookmarkButton);
			this.Controls.Add(this._RemoveButton);
			this.Controls.Add(this._BookmarkConditionBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AutoBookmarkForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "自动生成书签";
			this.Load += new System.EventHandler(this.AutoBookmarkForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._BookmarkConditionBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private BrightIdeasSoftware.ObjectListView _BookmarkConditionBox;
		private BrightIdeasSoftware.OLVColumn _ConditionColumn;
		private System.Windows.Forms.Button _RemoveButton;
		private System.Windows.Forms.Button _AutoBookmarkButton;
		private BrightIdeasSoftware.OLVColumn _LevelColumn;
		private BrightIdeasSoftware.OLVColumn _BoldColumn;
		private BrightIdeasSoftware.OLVColumn _ItalicColumn;
		private BrightIdeasSoftware.OLVColumn _ColorColumn;
		private BrightIdeasSoftware.OLVColumn _OpenColumn;
		private System.Windows.Forms.CheckBox _MergeAdjacentTitleBox;
	}
}