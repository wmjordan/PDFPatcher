namespace PDFPatcher.Functions
{
	partial class ZoomRateEntryForm
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
			this._ZoomRateBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(97, 74);
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
			this._CancelButton.Location = new System.Drawing.Point(178, 74);
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
			this._MessageLabel.Location = new System.Drawing.Point(12, 33);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size(113, 12);
			this._MessageLabel.TabIndex = 2;
			this._MessageLabel.Text = "在此输入缩放比例：";
			// 
			// _ZoomRateBox
			// 
			this._ZoomRateBox.FormattingEnabled = true;
			this._ZoomRateBox.Items.AddRange(new object[] {
            "1",
            "保持不变",
            "——————",
            "4",
            "3",
            "2",
            "1.5",
            "1.3",
            "1.2",
            "1",
            "0.9",
            "0.8",
            "0.5",
            "0.3",
            "0.2"});
			this._ZoomRateBox.Location = new System.Drawing.Point(131, 30);
			this._ZoomRateBox.Name = "_ZoomRateBox";
			this._ZoomRateBox.Size = new System.Drawing.Size(121, 20);
			this._ZoomRateBox.TabIndex = 3;
			// 
			// ZoomRateEntryForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(265, 109);
			this.Controls.Add(this._ZoomRateBox);
			this.Controls.Add(this._MessageLabel);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ZoomRateEntryForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "输入缩放比例";
			this.Load += new System.EventHandler(this.ZoomRateEntryForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label _MessageLabel;
		private System.Windows.Forms.ComboBox _ZoomRateBox;
	}
}

