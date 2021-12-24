namespace PDFPatcher
{
	partial class BookmarkControl
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
            this.label1 = new System.Windows.Forms.Label();
            this._BrowseBookmarkButton = new System.Windows.Forms.Button();
            this._OpenBookmarkBox = new System.Windows.Forms.OpenFileDialog();
            this._SaveBookmarkBox = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this._BookmarkBox = new PDFPatcher.HistoryComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "P&DF 信息文件：";
            // 
            // _BrowseBookmarkButton
            // 
            this._BrowseBookmarkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._BrowseBookmarkButton.Image = global::PDFPatcher.Properties.Resources.BookmarkFile;
            this._BrowseBookmarkButton.Location = new System.Drawing.Point(391, 1);
            this._BrowseBookmarkButton.Name = "_BrowseBookmarkButton";
            this._BrowseBookmarkButton.Size = new System.Drawing.Size(75, 23);
            this._BrowseBookmarkButton.TabIndex = 2;
            this._BrowseBookmarkButton.Text = "浏览...";
            this._BrowseBookmarkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._BrowseBookmarkButton.UseVisualStyleBackColor = true;
            this._BrowseBookmarkButton.Click += new System.EventHandler(this._BrowseSourcePdfButton_Click);
            // 
            // _OpenBookmarkBox
            // 
            this._OpenBookmarkBox.DefaultExt = "xml";
            this._OpenBookmarkBox.Filter = "支持的信息文件 (*.xml,*.txt)|*.xml;*.txt|XML 信息文件 (*.xml)|*.xml|简易文本书签文件(*.txt)|*.txt";
            this._OpenBookmarkBox.Title = "指定需要导入的信息文件的路径";
            // 
            // _SaveBookmarkBox
            // 
            this._SaveBookmarkBox.DefaultExt = "xml";
            this._SaveBookmarkBox.Filter = "支持的信息文件 (*.xml,*.txt)|*.xml;*.txt|XML 信息文件 (*.xml)|*.xml|简易文本书签文件(*.txt)|*.txt";
            this._SaveBookmarkBox.Title = "指定导出的信息文件路径";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._BookmarkBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this._BrowseBookmarkButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(469, 26);
            this.panel1.TabIndex = 3;
            // 
            // _BookmarkBox
            // 
            this._BookmarkBox.AllowDrop = true;
            this._BookmarkBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._BookmarkBox.Contents = null;
            this._BookmarkBox.FormattingEnabled = true;
            this._BookmarkBox.Location = new System.Drawing.Point(104, 3);
            this._BookmarkBox.MaxItemCount = 16;
            this._BookmarkBox.Name = "_BookmarkBox";
            this._BookmarkBox.Size = new System.Drawing.Size(281, 20);
            this._BookmarkBox.TabIndex = 1;
            this._BookmarkBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._BookmarkBox_DragDrop);
            this._BookmarkBox.DragEnter += new System.Windows.Forms.DragEventHandler(this._BookmarkBox_DragEnter);
            this._BookmarkBox.TextChanged += new System.EventHandler(this._BookmarkBox_TextChanged);
            // 
            // BookmarkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Name = "BookmarkControl";
            this.Size = new System.Drawing.Size(469, 26);
            this.Load += new System.EventHandler(this.BookmarkControl_Show);
            this.VisibleChanged += new System.EventHandler(this.BookmarkControl_Show);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _BrowseBookmarkButton;
		private System.Windows.Forms.OpenFileDialog _OpenBookmarkBox;
		private System.Windows.Forms.SaveFileDialog _SaveBookmarkBox;
		private PDFPatcher.HistoryComboBox _BookmarkBox;
        private System.Windows.Forms.Panel panel1;
	}
}
