namespace PDFPatcher.Functions
{
	partial class DocumentFontListForm
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

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent () {
            this.label1 = new System.Windows.Forms.Label();
            this._PageRangeBox = new System.Windows.Forms.TextBox();
            this._FontListBox = new BrightIdeasSoftware.FastObjectListView();
            this._NameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this._EmbeddedColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this._FirstPageColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this._ReferenceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this._Worker = new System.ComponentModel.BackgroundWorker();
            this._ProgressBar = new System.Windows.Forms.ProgressBar();
            this._AddSelectedFontsButton = new System.Windows.Forms.Button();
            this._SelectAllButton = new System.Windows.Forms.Button();
            this._ListFontsButton = new System.Windows.Forms.Button();
            this._SourceFileBox = new PDFPatcher.SourceFileControl();
            this._AppConfigButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._FontListBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "页码范围：";
            // 
            // _PageRangeBox
            // 
            this._PageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._PageRangeBox.Location = new System.Drawing.Point(146, 55);
            this._PageRangeBox.Margin = new System.Windows.Forms.Padding(4);
            this._PageRangeBox.Name = "_PageRangeBox";
            this._PageRangeBox.Size = new System.Drawing.Size(470, 25);
            this._PageRangeBox.TabIndex = 2;
            // 
            // _FontListBox
            // 
            this._FontListBox.AllColumns.Add(this._NameColumn);
            this._FontListBox.AllColumns.Add(this._EmbeddedColumn);
            this._FontListBox.AllColumns.Add(this._FirstPageColumn);
            this._FontListBox.AllColumns.Add(this._ReferenceColumn);
            this._FontListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._FontListBox.CheckBoxes = true;
            this._FontListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._NameColumn,
            this._EmbeddedColumn,
            this._FirstPageColumn,
            this._ReferenceColumn});
            this._FontListBox.GridLines = true;
            this._FontListBox.Location = new System.Drawing.Point(24, 128);
            this._FontListBox.Margin = new System.Windows.Forms.Padding(4);
            this._FontListBox.MultiSelect = false;
            this._FontListBox.Name = "_FontListBox";
            this._FontListBox.OwnerDraw = true;
            this._FontListBox.ShowGroups = false;
            this._FontListBox.ShowImagesOnSubItems = true;
            this._FontListBox.Size = new System.Drawing.Size(699, 309);
            this._FontListBox.TabIndex = 8;
            this._FontListBox.UseCompatibleStateImageBehavior = false;
            this._FontListBox.View = System.Windows.Forms.View.Details;
            this._FontListBox.VirtualMode = true;
            // 
            // _NameColumn
            // 
            this._NameColumn.AspectName = "";
            this._NameColumn.Text = "字体名称";
            this._NameColumn.Width = 273;
            // 
            // _EmbeddedColumn
            // 
            this._EmbeddedColumn.AspectName = "";
            this._EmbeddedColumn.CheckBoxes = true;
            this._EmbeddedColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._EmbeddedColumn.IsEditable = false;
            this._EmbeddedColumn.Text = "已嵌入";
            this._EmbeddedColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _FirstPageColumn
            // 
            this._FirstPageColumn.AspectName = "";
            this._FirstPageColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._FirstPageColumn.Text = "首次出现页码";
            this._FirstPageColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._FirstPageColumn.Width = 96;
            // 
            // _ReferenceColumn
            // 
            this._ReferenceColumn.AspectName = "";
            this._ReferenceColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._ReferenceColumn.Text = "出现页数";
            this._ReferenceColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _Worker
            // 
            this._Worker.WorkerReportsProgress = true;
            this._Worker.WorkerSupportsCancellation = true;
            // 
            // _ProgressBar
            // 
            this._ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ProgressBar.Location = new System.Drawing.Point(24, 446);
            this._ProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this._ProgressBar.Name = "_ProgressBar";
            this._ProgressBar.Size = new System.Drawing.Size(700, 29);
            this._ProgressBar.TabIndex = 9;
            // 
            // _AddSelectedFontsButton
            // 
            this._AddSelectedFontsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._AddSelectedFontsButton.Location = new System.Drawing.Point(516, 91);
            this._AddSelectedFontsButton.Margin = new System.Windows.Forms.Padding(4);
            this._AddSelectedFontsButton.Name = "_AddSelectedFontsButton";
            this._AddSelectedFontsButton.Size = new System.Drawing.Size(208, 29);
            this._AddSelectedFontsButton.TabIndex = 7;
            this._AddSelectedFontsButton.Text = "添加选中项至替换列表";
            this._AddSelectedFontsButton.UseVisualStyleBackColor = true;
            this._AddSelectedFontsButton.Click += new System.EventHandler(this._AddSelectedFontsButton_Click);
            // 
            // _SelectAllButton
            // 
            this._SelectAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._SelectAllButton.Location = new System.Drawing.Point(381, 91);
            this._SelectAllButton.Margin = new System.Windows.Forms.Padding(4);
            this._SelectAllButton.Name = "_SelectAllButton";
            this._SelectAllButton.Size = new System.Drawing.Size(127, 29);
            this._SelectAllButton.TabIndex = 6;
            this._SelectAllButton.Text = "全部选中(&Q)";
            this._SelectAllButton.UseVisualStyleBackColor = true;
            this._SelectAllButton.Click += new System.EventHandler(this._SelectAllButton_Click);
            // 
            // _ListFontsButton
            // 
            this._ListFontsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._ListFontsButton.Location = new System.Drawing.Point(624, 55);
            this._ListFontsButton.Margin = new System.Windows.Forms.Padding(4);
            this._ListFontsButton.Name = "_ListFontsButton";
            this._ListFontsButton.Size = new System.Drawing.Size(100, 29);
            this._ListFontsButton.TabIndex = 3;
            this._ListFontsButton.Text = "列出字体";
            this._ListFontsButton.UseVisualStyleBackColor = true;
            this._ListFontsButton.Click += new System.EventHandler(this._ListFontsButton_Click);
            // 
            // _SourceFileBox
            // 
            this._SourceFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._SourceFileBox.Location = new System.Drawing.Point(16, 15);
            this._SourceFileBox.Margin = new System.Windows.Forms.Padding(5);
            this._SourceFileBox.Name = "_SourceFileBox";
            this._SourceFileBox.Size = new System.Drawing.Size(712, 32);
            this._SourceFileBox.TabIndex = 0;
            // 
            // _AppConfigButton
            // 
            this._AppConfigButton.Location = new System.Drawing.Point(26, 91);
            this._AppConfigButton.Name = "_AppConfigButton";
            this._AppConfigButton.Size = new System.Drawing.Size(127, 29);
            this._AppConfigButton.TabIndex = 10;
            this._AppConfigButton.Text = "程序配置...";
            this._AppConfigButton.UseVisualStyleBackColor = true;
            this._AppConfigButton.Click += new System.EventHandler(this._AppConfigButton_Click);
            // 
            // DocumentFontListForm
            // 
            this.AcceptButton = this._ListFontsButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 490);
            this.Controls.Add(this._AppConfigButton);
            this.Controls.Add(this._SelectAllButton);
            this.Controls.Add(this._AddSelectedFontsButton);
            this.Controls.Add(this._ProgressBar);
            this.Controls.Add(this._FontListBox);
            this.Controls.Add(this._ListFontsButton);
            this.Controls.Add(this._PageRangeBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._SourceFileBox);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocumentFontListForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "PDF 文档使用的字体列表";
            ((System.ComponentModel.ISupportInitialize)(this._FontListBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private SourceFileControl _SourceFileBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _PageRangeBox;
		private System.Windows.Forms.Button _ListFontsButton;
		private BrightIdeasSoftware.FastObjectListView _FontListBox;
		private BrightIdeasSoftware.OLVColumn _NameColumn;
		private BrightIdeasSoftware.OLVColumn _FirstPageColumn;
		private BrightIdeasSoftware.OLVColumn _EmbeddedColumn;
		private System.ComponentModel.BackgroundWorker _Worker;
		private System.Windows.Forms.ProgressBar _ProgressBar;
		private BrightIdeasSoftware.OLVColumn _ReferenceColumn;
		private System.Windows.Forms.Button _AddSelectedFontsButton;
		private System.Windows.Forms.Button _SelectAllButton;
        private System.Windows.Forms.Button _AppConfigButton;
    }
}