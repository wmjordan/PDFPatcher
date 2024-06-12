namespace PDFPatcher.Functions
{
	partial class ShiftPageNumberEntryForm
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
			this._ShiftNumberBox = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this._ShiftNumberBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(128, 71);
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
			this._CancelButton.Location = new System.Drawing.Point(209, 71);
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
			this._MessageLabel.Location = new System.Drawing.Point(12, 27);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size(197, 24);
			this._MessageLabel.TabIndex = 2;
			this._MessageLabel.Text = "在此输入需要增加或减少的页数\r\n（正数增加页码，负数减少页码）：";
			// 
			// _ShiftNumberBox
			// 
			this._ShiftNumberBox.Location = new System.Drawing.Point(215, 27);
			this._ShiftNumberBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this._ShiftNumberBox.Minimum = new decimal(new int[] {
            999,
            0,
            0,
            -2147483648});
			this._ShiftNumberBox.Name = "_ShiftNumberBox";
			this._ShiftNumberBox.Size = new System.Drawing.Size(68, 21);
			this._ShiftNumberBox.TabIndex = 3;
			this._ShiftNumberBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// ShiftPageNumberEntryForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(296, 106);
			this.Controls.Add(this._ShiftNumberBox);
			this.Controls.Add(this._MessageLabel);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShiftPageNumberEntryForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "增加或减少页码";
			((System.ComponentModel.ISupportInitialize)(this._ShiftNumberBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label _MessageLabel;
		private System.Windows.Forms.NumericUpDown _ShiftNumberBox;
	}
}

