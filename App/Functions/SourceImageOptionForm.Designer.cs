namespace PDFPatcher.Functions
{
	partial class SourceImageOptionForm
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
			this._OkButton = new System.Windows.Forms.Button ();
			this._CancelButton = new System.Windows.Forms.Button ();
			this.label2 = new System.Windows.Forms.Label ();
			this._SourceFileBox = new System.Windows.Forms.TextBox ();
			this.label12 = new System.Windows.Forms.Label ();
			this.label11 = new System.Windows.Forms.Label ();
			this.label9 = new System.Windows.Forms.Label ();
			this._MinCropWidthBox = new System.Windows.Forms.NumericUpDown ();
			this._MinCropHeightBox = new System.Windows.Forms.NumericUpDown ();
			this.label5 = new System.Windows.Forms.Label ();
			this._RightMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._LeftMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._BottomMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._TopMarginBox = new System.Windows.Forms.NumericUpDown ();
			this.label7 = new System.Windows.Forms.Label ();
			this.label3 = new System.Windows.Forms.Label ();
			this.label6 = new System.Windows.Forms.Label ();
			this.label4 = new System.Windows.Forms.Label ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropWidthBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropHeightBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point (226, 137);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size (75, 23);
			this._OkButton.TabIndex = 5;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler (this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point (307, 137);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 6;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (12, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (53, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "文件名：";
			// 
			// _SourceFileBox
			// 
			this._SourceFileBox.Enabled = false;
			this._SourceFileBox.Location = new System.Drawing.Point (14, 24);
			this._SourceFileBox.Name = "_SourceFileBox";
			this._SourceFileBox.Size = new System.Drawing.Size (368, 21);
			this._SourceFileBox.TabIndex = 8;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point (223, 71);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size (65, 12);
			this.label12.TabIndex = 18;
			this.label12.Text = "宽度不小于";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point (211, 48);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size (149, 12);
			this.label11.TabIndex = 17;
			this.label11.Text = "裁剪条件（单位：像素）：";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point (224, 98);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size (65, 12);
			this.label9.TabIndex = 20;
			this.label9.Text = "高度不小于";
			// 
			// _MinCropWidthBox
			// 
			this._MinCropWidthBox.Increment = new decimal (new int[] {
            100,
            0,
            0,
            0});
			this._MinCropWidthBox.Location = new System.Drawing.Point (299, 69);
			this._MinCropWidthBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._MinCropWidthBox.Name = "_MinCropWidthBox";
			this._MinCropWidthBox.Size = new System.Drawing.Size (53, 21);
			this._MinCropWidthBox.TabIndex = 19;
			this._MinCropWidthBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _MinCropHeightBox
			// 
			this._MinCropHeightBox.Increment = new decimal (new int[] {
            100,
            0,
            0,
            0});
			this._MinCropHeightBox.Location = new System.Drawing.Point (299, 96);
			this._MinCropHeightBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._MinCropHeightBox.Name = "_MinCropHeightBox";
			this._MinCropHeightBox.Size = new System.Drawing.Size (53, 21);
			this._MinCropHeightBox.TabIndex = 21;
			this._MinCropHeightBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point (12, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (149, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "裁剪图片（单位：像素）：";
			// 
			// _RightMarginBox
			// 
			this._RightMarginBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._RightMarginBox.Location = new System.Drawing.Point (143, 96);
			this._RightMarginBox.Maximum = new decimal (new int[] {
            1000,
            0,
            0,
            0});
			this._RightMarginBox.Name = "_RightMarginBox";
			this._RightMarginBox.Size = new System.Drawing.Size (49, 21);
			this._RightMarginBox.TabIndex = 16;
			this._RightMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _LeftMarginBox
			// 
			this._LeftMarginBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._LeftMarginBox.Location = new System.Drawing.Point (53, 96);
			this._LeftMarginBox.Maximum = new decimal (new int[] {
            1000,
            0,
            0,
            0});
			this._LeftMarginBox.Name = "_LeftMarginBox";
			this._LeftMarginBox.Size = new System.Drawing.Size (49, 21);
			this._LeftMarginBox.TabIndex = 14;
			this._LeftMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _BottomMarginBox
			// 
			this._BottomMarginBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._BottomMarginBox.Location = new System.Drawing.Point (143, 69);
			this._BottomMarginBox.Maximum = new decimal (new int[] {
            1000,
            0,
            0,
            0});
			this._BottomMarginBox.Name = "_BottomMarginBox";
			this._BottomMarginBox.Size = new System.Drawing.Size (49, 21);
			this._BottomMarginBox.TabIndex = 12;
			this._BottomMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _TopMarginBox
			// 
			this._TopMarginBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._TopMarginBox.Location = new System.Drawing.Point (53, 69);
			this._TopMarginBox.Maximum = new decimal (new int[] {
            1000,
            0,
            0,
            0});
			this._TopMarginBox.Name = "_TopMarginBox";
			this._TopMarginBox.Size = new System.Drawing.Size (49, 21);
			this._TopMarginBox.TabIndex = 10;
			this._TopMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point (108, 99);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (29, 12);
			this.label7.TabIndex = 15;
			this.label7.Text = "右：";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (108, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (29, 12);
			this.label3.TabIndex = 11;
			this.label3.Text = "下：";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point (18, 99);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size (29, 12);
			this.label6.TabIndex = 13;
			this.label6.Text = "左：";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point (18, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size (29, 12);
			this.label4.TabIndex = 9;
			this.label4.Text = "上：";
			// 
			// SourceImageOptionForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (394, 172);
			this.Controls.Add (this.label12);
			this.Controls.Add (this.label11);
			this.Controls.Add (this._SourceFileBox);
			this.Controls.Add (this.label9);
			this.Controls.Add (this.label2);
			this.Controls.Add (this._MinCropWidthBox);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._MinCropHeightBox);
			this.Controls.Add (this._OkButton);
			this.Controls.Add (this.label5);
			this.Controls.Add (this._RightMarginBox);
			this.Controls.Add (this.label4);
			this.Controls.Add (this._LeftMarginBox);
			this.Controls.Add (this.label6);
			this.Controls.Add (this._BottomMarginBox);
			this.Controls.Add (this.label3);
			this.Controls.Add (this._TopMarginBox);
			this.Controls.Add (this.label7);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SourceImageOptionForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "源图片处理选项";
			((System.ComponentModel.ISupportInitialize)(this._MinCropWidthBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropHeightBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _SourceFileBox;
		private System.Windows.Forms.NumericUpDown _RightMarginBox;
		private System.Windows.Forms.NumericUpDown _LeftMarginBox;
		private System.Windows.Forms.NumericUpDown _BottomMarginBox;
		private System.Windows.Forms.NumericUpDown _TopMarginBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown _MinCropWidthBox;
		private System.Windows.Forms.NumericUpDown _MinCropHeightBox;
		private System.Windows.Forms.Label label12;
	}
}

