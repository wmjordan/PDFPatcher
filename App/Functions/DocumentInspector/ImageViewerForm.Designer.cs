namespace PDFPatcher.Functions
{
	partial class ImageViewerForm
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
			System.Windows.Forms.ToolStripButton _Save;
			System.Windows.Forms.ToolStripButton _ZoomReset;
			this._MainToolbar = new System.Windows.Forms.ToolStrip ();
			this._FitWindow = new System.Windows.Forms.ToolStripButton ();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator ();
			this._ImageBox = new Cyotek.Windows.Forms.ImageBox ();
			_Save = new System.Windows.Forms.ToolStripButton ();
			_ZoomReset = new System.Windows.Forms.ToolStripButton ();
			this._MainToolbar.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// _MainToolbar
			// 
			this._MainToolbar.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
            _Save,
            this.toolStripSeparator1,
            _ZoomReset,
            this._FitWindow});
			this._MainToolbar.Location = new System.Drawing.Point (0, 0);
			this._MainToolbar.Name = "_MainToolbar";
			this._MainToolbar.Size = new System.Drawing.Size (539, 25);
			this._MainToolbar.TabIndex = 1;
			this._MainToolbar.Text = "toolStrip1";
			this._MainToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler (this._MainToolbar_ItemClicked);
			// 
			// _Save
			// 
			_Save.Image = global::PDFPatcher.Properties.Resources.Save;
			_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Save.Name = "_Save";
			_Save.Size = new System.Drawing.Size (90, 22);
			_Save.Text = "保存图片(&B)";
			_Save.ToolTipText = "将显示的图片保存为文件";
			// 
			// _ZoomReset
			// 
			_ZoomReset.Image = global::PDFPatcher.Properties.Resources.Zoom;
			_ZoomReset.ImageTransparentColor = System.Drawing.Color.Magenta;
			_ZoomReset.Name = "_ZoomReset";
			_ZoomReset.Size = new System.Drawing.Size (75, 22);
			_ZoomReset.Text = "原图比例";
			// 
			// _FitWindow
			// 
			this._FitWindow.Image = global::PDFPatcher.Properties.Resources.Image;
			this._FitWindow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._FitWindow.Name = "_FitWindow";
			this._FitWindow.Size = new System.Drawing.Size (75, 22);
			this._FitWindow.Text = "适合窗口";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size (6, 25);
			// 
			// _ImageBox
			// 
			this._ImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._ImageBox.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ImageBox.Location = new System.Drawing.Point (12, 28);
			this._ImageBox.MinimumSize = new System.Drawing.Size (454, 145);
			this._ImageBox.Name = "_ImageBox";
			this._ImageBox.Size = new System.Drawing.Size (515, 380);
			this._ImageBox.TabIndex = 0;
			this._ImageBox.TabStop = false;
			// 
			// ImageViewerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (539, 420);
			this.Controls.Add (this._MainToolbar);
			this.Controls.Add (this._ImageBox);
			this.Name = "ImageViewerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "查看图片";
			this._MainToolbar.ResumeLayout (false);
			this._MainToolbar.PerformLayout ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private Cyotek.Windows.Forms.ImageBox _ImageBox;
		private System.Windows.Forms.ToolStrip _MainToolbar;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton _FitWindow;
	}
}