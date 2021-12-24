namespace PDFPatcher.Functions
{
	partial class RenamePreviewForm
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
			this._RenamePreviewBox = new System.Windows.Forms.ListView ();
			this._OriginalNameColumn = new System.Windows.Forms.ColumnHeader ();
			this._OutputNameColumn = new System.Windows.Forms.ColumnHeader ();
			this._OriginalFolderColumn = new System.Windows.Forms.ColumnHeader ();
			this._OutputFolderColumn = new System.Windows.Forms.ColumnHeader ();
			this.label1 = new System.Windows.Forms.Label ();
			this._OKButton = new System.Windows.Forms.Button ();
			this.SuspendLayout ();
			// 
			// _RenamePreviewBox
			// 
			this._RenamePreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._RenamePreviewBox.Columns.AddRange (new System.Windows.Forms.ColumnHeader[] {
            this._OriginalNameColumn,
            this._OutputNameColumn,
            this._OriginalFolderColumn,
            this._OutputFolderColumn});
			this._RenamePreviewBox.GridLines = true;
			this._RenamePreviewBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._RenamePreviewBox.Location = new System.Drawing.Point (12, 24);
			this._RenamePreviewBox.Name = "_RenamePreviewBox";
			this._RenamePreviewBox.Size = new System.Drawing.Size (456, 197);
			this._RenamePreviewBox.TabIndex = 0;
			this._RenamePreviewBox.UseCompatibleStateImageBehavior = false;
			this._RenamePreviewBox.View = System.Windows.Forms.View.Details;
			// 
			// _OriginalNameColumn
			// 
			this._OriginalNameColumn.Text = "原始文件名";
			this._OriginalNameColumn.Width = 84;
			// 
			// _OutputNameColumn
			// 
			this._OutputNameColumn.Text = "输出文件名";
			this._OutputNameColumn.Width = 83;
			// 
			// _OriginalFolderColumn
			// 
			this._OriginalFolderColumn.Text = "原始文件夹";
			this._OriginalFolderColumn.Width = 85;
			// 
			// _OutputFolderColumn
			// 
			this._OutputFolderColumn.Text = "输出文件夹";
			this._OutputFolderColumn.Width = 88;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (149, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "PDF 文件重命名结果预览：";
			// 
			// _OKButton
			// 
			this._OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._OKButton.Location = new System.Drawing.Point (393, 232);
			this._OKButton.Name = "_OKButton";
			this._OKButton.Size = new System.Drawing.Size (75, 23);
			this._OKButton.TabIndex = 2;
			this._OKButton.Text = "确定(&Q)";
			this._OKButton.UseVisualStyleBackColor = true;
			this._OKButton.Click += new System.EventHandler (this._OKButton_Click);
			// 
			// RenamePreviewForm
			// 
			this.AcceptButton = this._OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._OKButton;
			this.ClientSize = new System.Drawing.Size (480, 267);
			this.Controls.Add (this._OKButton);
			this.Controls.Add (this.label1);
			this.Controls.Add (this._RenamePreviewBox);
			this.MinimumSize = new System.Drawing.Size (300, 200);
			this.Name = "RenamePreviewForm";
			this.Text = "重命名结果预览";
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.ListView _RenamePreviewBox;
		private System.Windows.Forms.ColumnHeader _OriginalNameColumn;
		private System.Windows.Forms.ColumnHeader _OutputNameColumn;
		private System.Windows.Forms.ColumnHeader _OriginalFolderColumn;
		private System.Windows.Forms.ColumnHeader _OutputFolderColumn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _OKButton;
	}
}