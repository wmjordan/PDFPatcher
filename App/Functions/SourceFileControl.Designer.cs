namespace PDFPatcher
{
	partial class SourceFileControl
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
            this._BrowseSourcePdfButton = new System.Windows.Forms.Button();
            this._OpenPdfBox = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this._SourcePdfBox = new PDFPatcher.HistoryComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "原始 &PDF 文件：";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // _BrowseSourcePdfButton
            // 
            this._BrowseSourcePdfButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._BrowseSourcePdfButton.Image = global::PDFPatcher.Properties.Resources.OriginalPdfFile;
            this._BrowseSourcePdfButton.Location = new System.Drawing.Point(391, 1);
            this._BrowseSourcePdfButton.Name = "_BrowseSourcePdfButton";
            this._BrowseSourcePdfButton.Size = new System.Drawing.Size(75, 23);
            this._BrowseSourcePdfButton.TabIndex = 2;
            this._BrowseSourcePdfButton.Text = "浏览(&L)";
            this._BrowseSourcePdfButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this._BrowseSourcePdfButton.UseVisualStyleBackColor = true;
            this._BrowseSourcePdfButton.Click += new System.EventHandler(this._BrowseSourcePdfButton_Click);
            // 
            // _OpenPdfBox
            // 
            this._OpenPdfBox.DefaultExt = "pdf";
            this._OpenPdfBox.Filter = "PDF 文件(*.pdf)|*.pdf";
            this._OpenPdfBox.Multiselect = true;
            this._OpenPdfBox.Title = "打开 PDF 源文件";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._SourcePdfBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this._BrowseSourcePdfButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(469, 26);
            this.panel1.TabIndex = 4;
            // 
            // _SourcePdfBox
            // 
            this._SourcePdfBox.AllowDrop = true;
            this._SourcePdfBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._SourcePdfBox.Contents = null;
            this._SourcePdfBox.FormattingEnabled = true;
            this._SourcePdfBox.Location = new System.Drawing.Point(104, 3);
            this._SourcePdfBox.MaxItemCount = 16;
            this._SourcePdfBox.Name = "_SourcePdfBox";
            this._SourcePdfBox.Size = new System.Drawing.Size(281, 20);
            this._SourcePdfBox.TabIndex = 3;
            this._SourcePdfBox.SelectedIndexChanged += new System.EventHandler(this._SourcePdfBox_SelectedIndexChanged);
            this._SourcePdfBox.DragDrop += new System.Windows.Forms.DragEventHandler(this._SourcePdfBox_DragDrop);
            this._SourcePdfBox.DragEnter += new System.Windows.Forms.DragEventHandler(this._SourcePdfBox_DragEnter);
            this._SourcePdfBox.TextChanged += new System.EventHandler(this._SourcePdfBox_TextChanged);
            // 
            // SourceFileControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Name = "SourceFileControl";
            this.Size = new System.Drawing.Size(469, 26);
            this.Load += new System.EventHandler(this.SourceFileControl_Show);
            this.VisibleChanged += new System.EventHandler(this.SourceFileControl_Show);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _BrowseSourcePdfButton;
		private System.Windows.Forms.OpenFileDialog _OpenPdfBox;
		private HistoryComboBox _SourcePdfBox;
        private System.Windows.Forms.Panel panel1;
	}
}
