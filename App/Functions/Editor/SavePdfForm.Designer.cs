namespace PDFPatcher.Functions
{
	partial class SavePdfForm
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
			this._OkButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this._MessageLabel = new System.Windows.Forms.Label();
			this._SourceFileBox = new PDFPatcher.SourceFileControl();
			this._TargetFileBox = new PDFPatcher.TargetFileControl();
			this._ConfigButton = new System.Windows.Forms.Button();
			this._OverwriteBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(322, 132);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 0;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler(this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(403, 132);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(75, 23);
			this._CancelButton.TabIndex = 1;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler(this._CancelButton_Click);
			// 
			// _MessageLabel
			// 
			this._MessageLabel.AutoSize = true;
			this._MessageLabel.Location = new System.Drawing.Point(113, 100);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size(347, 12);
			this._MessageLabel.TabIndex = 2;
			this._MessageLabel.Text = "点击“确定”按钮，将书签编辑器的书签写入到目标 PDF 文档。";
			// 
			// _SourceFileBox
			// 
			this._SourceFileBox.Location = new System.Drawing.Point(12, 12);
			this._SourceFileBox.Name = "_SourceFileBox";
			this._SourceFileBox.Size = new System.Drawing.Size(469, 26);
			this._SourceFileBox.TabIndex = 3;
			// 
			// _TargetFileBox
			// 
			this._TargetFileBox.Location = new System.Drawing.Point(12, 44);
			this._TargetFileBox.Name = "_TargetFileBox";
			this._TargetFileBox.Size = new System.Drawing.Size(469, 25);
			this._TargetFileBox.TabIndex = 4;
			// 
			// _ConfigButton
			// 
			this._ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._ConfigButton.Image = global::PDFPatcher.Properties.Resources.PdfOptions;
			this._ConfigButton.Location = new System.Drawing.Point(12, 132);
			this._ConfigButton.Name = "_ConfigButton";
			this._ConfigButton.Size = new System.Drawing.Size(181, 23);
			this._ConfigButton.TabIndex = 12;
			this._ConfigButton.Text = "设置 P&DF 文件的修改方式";
			this._ConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ConfigButton.UseVisualStyleBackColor = true;
			// 
			// _OverwriteBox
			// 
			this._OverwriteBox.AutoSize = true;
			this._OverwriteBox.Location = new System.Drawing.Point(115, 75);
			this._OverwriteBox.Name = "_OverwriteBox";
			this._OverwriteBox.Size = new System.Drawing.Size(126, 16);
			this._OverwriteBox.TabIndex = 13;
			this._OverwriteBox.Text = "覆盖原始 PDF 文件";
			this._OverwriteBox.UseVisualStyleBackColor = true;
			// 
			// SavePdfForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(490, 167);
			this.Controls.Add(this._OverwriteBox);
			this.Controls.Add(this._ConfigButton);
			this.Controls.Add(this._TargetFileBox);
			this.Controls.Add(this._SourceFileBox);
			this.Controls.Add(this._MessageLabel);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SavePdfForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "保存PDF文件";
			this.Load += new System.EventHandler(this.ImportBookmarkForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label _MessageLabel;
		private SourceFileControl _SourceFileBox;
		private TargetFileControl _TargetFileBox;
		private System.Windows.Forms.Button _ConfigButton;
		private System.Windows.Forms.CheckBox _OverwriteBox;
	}
}

