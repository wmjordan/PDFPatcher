namespace PDFPatcher
{
	partial class AboutControl
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
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
			this._FrontPageBox = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
			this.SuspendLayout();
			// 
			// _FrontPageBox
			// 
			this._FrontPageBox.AutoScroll = true;
			this._FrontPageBox.BackColor = System.Drawing.SystemColors.Window;
			this._FrontPageBox.BaseStylesheet = "";
			this._FrontPageBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._FrontPageBox.IsContextMenuEnabled = false;
			this._FrontPageBox.IsSelectionEnabled = false;
			this._FrontPageBox.Location = new System.Drawing.Point(9, 8);
			this._FrontPageBox.Name = "_FrontPageBox";
			this._FrontPageBox.Size = new System.Drawing.Size(433, 328);
			this._FrontPageBox.TabIndex = 1;
			this._FrontPageBox.Text = null;
			this._FrontPageBox.LinkClicked += new System.EventHandler<TheArtOfDev.HtmlRenderer.Core.Entities.HtmlLinkClickedEventArgs>(this._FrontPageBox_LinkClicked);
			this._FrontPageBox.ImageLoad += new System.EventHandler<TheArtOfDev.HtmlRenderer.Core.Entities.HtmlImageLoadEventArgs>(this._FrontPageBox_ImageLoad);
			// 
			// AboutControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._FrontPageBox);
			this.Name = "AboutControl";
			this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
			this.Size = new System.Drawing.Size(451, 344);
			this.ResumeLayout(false);

		}

		#endregion

		private TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel _FrontPageBox;

	}
}
