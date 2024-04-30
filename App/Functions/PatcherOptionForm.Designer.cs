namespace PDFPatcher.Functions
{
	partial class PatcherOptionForm
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
			this._OptionsBox = new PDFPatcher.Functions.PatcherOptionsControl();
			this.SuspendLayout();
			// 
			// _OptionsBox
			// 
			this._OptionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._OptionsBox.ForEditor = false;
			this._OptionsBox.Location = new System.Drawing.Point(12, 15);
			this._OptionsBox.Name = "_OptionsBox";
			this._OptionsBox.Options = null;
			this._OptionsBox.Size = new System.Drawing.Size(452, 321);
			this._OptionsBox.TabIndex = 0;
			// 
			// PatcherOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(476, 348);
			this.Controls.Add(this._OptionsBox);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(482, 374);
			this.Name = "PatcherOptionForm";
			this.ShowInTaskbar = false;
			this.Text = "PDF 文档选项";
			this.ResumeLayout(false);

		}

		#endregion

		private PatcherOptionsControl _OptionsBox;
	}
}
