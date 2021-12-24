namespace PDFPatcher.Functions
{
	partial class UpdateForm
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
			this._InfoBox = new System.Windows.Forms.RichTextBox();
			this._HomePageButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this._DownloadButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._CheckUpdateIntervalBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// _InfoBox
			// 
			this._InfoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._InfoBox.Location = new System.Drawing.Point(21, 18);
			this._InfoBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._InfoBox.Name = "_InfoBox";
			this._InfoBox.ReadOnly = true;
			this._InfoBox.Size = new System.Drawing.Size(777, 446);
			this._InfoBox.TabIndex = 0;
			this._InfoBox.Text = "";
			// 
			// _HomePageButton
			// 
			this._HomePageButton.Image = global::PDFPatcher.Properties.Resources.HomePage;
			this._HomePageButton.Location = new System.Drawing.Point(199, 474);
			this._HomePageButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._HomePageButton.Name = "_HomePageButton";
			this._HomePageButton.Size = new System.Drawing.Size(162, 35);
			this._HomePageButton.TabIndex = 2;
			this._HomePageButton.Text = "转到主页(&Z)";
			this._HomePageButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._HomePageButton.UseVisualStyleBackColor = true;
			// 
			// _CancelButton
			// 
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(370, 474);
			this._CancelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(112, 35);
			this._CancelButton.TabIndex = 3;
			this._CancelButton.Text = "取消(&Q)";
			this._CancelButton.UseVisualStyleBackColor = true;
			// 
			// _DownloadButton
			// 
			this._DownloadButton.Enabled = false;
			this._DownloadButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._DownloadButton.Location = new System.Drawing.Point(21, 474);
			this._DownloadButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this._DownloadButton.Name = "_DownloadButton";
			this._DownloadButton.Size = new System.Drawing.Size(170, 35);
			this._DownloadButton.TabIndex = 1;
			this._DownloadButton.Text = "下载新版本(&X)";
			this._DownloadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._DownloadButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(490, 482);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170, 18);
			this.label1.TabIndex = 4;
			this.label1.Text = "自动检查更新间隔：";
			// 
			// _CheckUpdateIntervalBox
			// 
			this._CheckUpdateIntervalBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._CheckUpdateIntervalBox.FormattingEnabled = true;
			this._CheckUpdateIntervalBox.Items.AddRange(new object[] {
            "7天",
            "14天",
            "30天",
            "从不检查"});
			this._CheckUpdateIntervalBox.Location = new System.Drawing.Point(657, 479);
			this._CheckUpdateIntervalBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this._CheckUpdateIntervalBox.Name = "_CheckUpdateIntervalBox";
			this._CheckUpdateIntervalBox.Size = new System.Drawing.Size(136, 26);
			this._CheckUpdateIntervalBox.TabIndex = 5;
			// 
			// UpdateForm
			// 
			this.AcceptButton = this._HomePageButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(818, 534);
			this.Controls.Add(this._CheckUpdateIntervalBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._DownloadButton);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._HomePageButton);
			this.Controls.Add(this._InfoBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateForm";
			this.Text = "检查更新";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox _InfoBox;
		private System.Windows.Forms.Button _HomePageButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Button _DownloadButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _CheckUpdateIntervalBox;
	}
}