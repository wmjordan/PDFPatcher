namespace PDFPatcher.Functions
{
	partial class SourcePdfOptionForm
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
			this._MessageLabel = new System.Windows.Forms.Label ();
			this._PageRangeBox = new System.Windows.Forms.TextBox ();
			this.label2 = new System.Windows.Forms.Label ();
			this._SourceFileBox = new System.Windows.Forms.TextBox ();
			this._ImportImagesOnlyBox = new System.Windows.Forms.CheckBox ();
			this._ExtractImageOptionBox = new System.Windows.Forms.GroupBox ();
			this.label13 = new System.Windows.Forms.Label ();
			this.label12 = new System.Windows.Forms.Label ();
			this.label11 = new System.Windows.Forms.Label ();
			this.label8 = new System.Windows.Forms.Label ();
			this.label9 = new System.Windows.Forms.Label ();
			this.label10 = new System.Windows.Forms.Label ();
			this._MinCropWidthBox = new System.Windows.Forms.NumericUpDown ();
			this._MinCropHeightBox = new System.Windows.Forms.NumericUpDown ();
			this._MinWidthBox = new System.Windows.Forms.NumericUpDown ();
			this._MinHeightBox = new System.Windows.Forms.NumericUpDown ();
			this._VerticalFlipImagesBox = new System.Windows.Forms.CheckBox ();
			this._InvertBlackAndWhiteImageBox = new System.Windows.Forms.CheckBox ();
			this._MergeImagesBox = new System.Windows.Forms.CheckBox ();
			this.label5 = new System.Windows.Forms.Label ();
			this._RightMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._LeftMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._BottomMarginBox = new System.Windows.Forms.NumericUpDown ();
			this._TopMarginBox = new System.Windows.Forms.NumericUpDown ();
			this.label7 = new System.Windows.Forms.Label ();
			this.label3 = new System.Windows.Forms.Label ();
			this.label6 = new System.Windows.Forms.Label ();
			this.label4 = new System.Windows.Forms.Label ();
			this._ExtractImageOptionBox.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropWidthBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropHeightBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinWidthBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinHeightBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point (288, 335);
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
			this._CancelButton.Location = new System.Drawing.Point (369, 335);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 6;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// _MessageLabel
			// 
			this._MessageLabel.AutoSize = true;
			this._MessageLabel.Location = new System.Drawing.Point (12, 54);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size (89, 12);
			this._MessageLabel.TabIndex = 0;
			this._MessageLabel.Text = "导入页码范围：";
			// 
			// _PageRangeBox
			// 
			this._PageRangeBox.Location = new System.Drawing.Point (107, 51);
			this._PageRangeBox.Name = "_PageRangeBox";
			this._PageRangeBox.Size = new System.Drawing.Size (337, 21);
			this._PageRangeBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (10, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (53, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "文件名：";
			// 
			// _SourceFileBox
			// 
			this._SourceFileBox.Location = new System.Drawing.Point (71, 12);
			this._SourceFileBox.Name = "_SourceFileBox";
			this._SourceFileBox.ReadOnly = true;
			this._SourceFileBox.Size = new System.Drawing.Size (373, 21);
			this._SourceFileBox.TabIndex = 8;
			// 
			// _ImportImagesOnlyBox
			// 
			this._ImportImagesOnlyBox.AutoSize = true;
			this._ImportImagesOnlyBox.Location = new System.Drawing.Point (14, 78);
			this._ImportImagesOnlyBox.Name = "_ImportImagesOnlyBox";
			this._ImportImagesOnlyBox.Size = new System.Drawing.Size (162, 16);
			this._ImportImagesOnlyBox.TabIndex = 3;
			this._ImportImagesOnlyBox.Text = "仅导入源 PDF 文件的图片";
			this._ImportImagesOnlyBox.UseVisualStyleBackColor = true;
			this._ImportImagesOnlyBox.CheckedChanged += new System.EventHandler (this._ImportImagesOnlyBox_CheckedChanged);
			// 
			// _ExtractImageOptionBox
			// 
			this._ExtractImageOptionBox.Controls.Add (this.label13);
			this._ExtractImageOptionBox.Controls.Add (this.label12);
			this._ExtractImageOptionBox.Controls.Add (this.label11);
			this._ExtractImageOptionBox.Controls.Add (this.label8);
			this._ExtractImageOptionBox.Controls.Add (this.label9);
			this._ExtractImageOptionBox.Controls.Add (this.label10);
			this._ExtractImageOptionBox.Controls.Add (this._MinCropWidthBox);
			this._ExtractImageOptionBox.Controls.Add (this._MinCropHeightBox);
			this._ExtractImageOptionBox.Controls.Add (this._MinWidthBox);
			this._ExtractImageOptionBox.Controls.Add (this._MinHeightBox);
			this._ExtractImageOptionBox.Controls.Add (this._VerticalFlipImagesBox);
			this._ExtractImageOptionBox.Controls.Add (this._InvertBlackAndWhiteImageBox);
			this._ExtractImageOptionBox.Controls.Add (this._MergeImagesBox);
			this._ExtractImageOptionBox.Controls.Add (this.label5);
			this._ExtractImageOptionBox.Controls.Add (this._RightMarginBox);
			this._ExtractImageOptionBox.Controls.Add (this._LeftMarginBox);
			this._ExtractImageOptionBox.Controls.Add (this._BottomMarginBox);
			this._ExtractImageOptionBox.Controls.Add (this._TopMarginBox);
			this._ExtractImageOptionBox.Controls.Add (this.label7);
			this._ExtractImageOptionBox.Controls.Add (this.label3);
			this._ExtractImageOptionBox.Controls.Add (this.label6);
			this._ExtractImageOptionBox.Controls.Add (this.label4);
			this._ExtractImageOptionBox.Enabled = false;
			this._ExtractImageOptionBox.Location = new System.Drawing.Point (14, 100);
			this._ExtractImageOptionBox.Name = "_ExtractImageOptionBox";
			this._ExtractImageOptionBox.Size = new System.Drawing.Size (430, 175);
			this._ExtractImageOptionBox.TabIndex = 4;
			this._ExtractImageOptionBox.TabStop = false;
			this._ExtractImageOptionBox.Text = "导入及处理源 PDF 文件图片的方式（尺寸单位：像素）";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point (220, 21);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size (89, 12);
			this.label13.TabIndex = 3;
			this.label13.Text = "导入图片条件：";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point (232, 116);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size (65, 12);
			this.label12.TabIndex = 18;
			this.label12.Text = "宽度不小于";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point (220, 92);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size (65, 12);
			this.label11.TabIndex = 17;
			this.label11.Text = "裁剪条件：";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point (232, 38);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size (65, 12);
			this.label8.TabIndex = 4;
			this.label8.Text = "宽度不小于";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point (232, 143);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size (65, 12);
			this.label9.TabIndex = 20;
			this.label9.Text = "高度不小于";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point (232, 65);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size (65, 12);
			this.label10.TabIndex = 6;
			this.label10.Text = "高度不小于";
			// 
			// _MinCropWidthBox
			// 
			this._MinCropWidthBox.Increment = new decimal (new int[] {
            100,
            0,
            0,
            0});
			this._MinCropWidthBox.Location = new System.Drawing.Point (308, 113);
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
			this._MinCropHeightBox.Location = new System.Drawing.Point (308, 140);
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
			// _MinWidthBox
			// 
			this._MinWidthBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._MinWidthBox.Location = new System.Drawing.Point (308, 36);
			this._MinWidthBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._MinWidthBox.Name = "_MinWidthBox";
			this._MinWidthBox.Size = new System.Drawing.Size (53, 21);
			this._MinWidthBox.TabIndex = 5;
			this._MinWidthBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _MinHeightBox
			// 
			this._MinHeightBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._MinHeightBox.Location = new System.Drawing.Point (308, 63);
			this._MinHeightBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._MinHeightBox.Name = "_MinHeightBox";
			this._MinHeightBox.Size = new System.Drawing.Size (53, 21);
			this._MinHeightBox.TabIndex = 7;
			this._MinHeightBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _VerticalFlipImagesBox
			// 
			this._VerticalFlipImagesBox.AutoSize = true;
			this._VerticalFlipImagesBox.Location = new System.Drawing.Point (6, 42);
			this._VerticalFlipImagesBox.Name = "_VerticalFlipImagesBox";
			this._VerticalFlipImagesBox.Size = new System.Drawing.Size (96, 16);
			this._VerticalFlipImagesBox.TabIndex = 1;
			this._VerticalFlipImagesBox.Text = "垂直翻转图片";
			this._VerticalFlipImagesBox.UseVisualStyleBackColor = true;
			// 
			// _InvertBlackAndWhiteImageBox
			// 
			this._InvertBlackAndWhiteImageBox.AutoSize = true;
			this._InvertBlackAndWhiteImageBox.Location = new System.Drawing.Point (6, 64);
			this._InvertBlackAndWhiteImageBox.Name = "_InvertBlackAndWhiteImageBox";
			this._InvertBlackAndWhiteImageBox.Size = new System.Drawing.Size (132, 16);
			this._InvertBlackAndWhiteImageBox.TabIndex = 2;
			this._InvertBlackAndWhiteImageBox.Text = "反转黑白图片的颜色";
			this._InvertBlackAndWhiteImageBox.UseVisualStyleBackColor = true;
			// 
			// _MergeImagesBox
			// 
			this._MergeImagesBox.AutoSize = true;
			this._MergeImagesBox.Checked = true;
			this._MergeImagesBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._MergeImagesBox.Location = new System.Drawing.Point (6, 20);
			this._MergeImagesBox.Name = "_MergeImagesBox";
			this._MergeImagesBox.Size = new System.Drawing.Size (120, 16);
			this._MergeImagesBox.TabIndex = 0;
			this._MergeImagesBox.Text = "尝试合并同页图片";
			this._MergeImagesBox.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point (6, 91);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (65, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "裁剪图片：";
			// 
			// _RightMarginBox
			// 
			this._RightMarginBox.Increment = new decimal (new int[] {
            10,
            0,
            0,
            0});
			this._RightMarginBox.Location = new System.Drawing.Point (137, 139);
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
			this._LeftMarginBox.Location = new System.Drawing.Point (47, 139);
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
			this._BottomMarginBox.Location = new System.Drawing.Point (137, 112);
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
			this._TopMarginBox.Location = new System.Drawing.Point (47, 112);
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
			this.label7.Location = new System.Drawing.Point (102, 142);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (29, 12);
			this.label7.TabIndex = 15;
			this.label7.Text = "右：";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (102, 114);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (29, 12);
			this.label3.TabIndex = 11;
			this.label3.Text = "下：";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point (12, 142);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size (29, 12);
			this.label6.TabIndex = 13;
			this.label6.Text = "左：";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point (12, 115);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size (29, 12);
			this.label4.TabIndex = 9;
			this.label4.Text = "上：";
			// 
			// SourcePdfOptionForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (456, 370);
			this.Controls.Add (this._PageRangeBox);
			this.Controls.Add (this._ImportImagesOnlyBox);
			this.Controls.Add (this._MessageLabel);
			this.Controls.Add (this._ExtractImageOptionBox);
			this.Controls.Add (this._SourceFileBox);
			this.Controls.Add (this.label2);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SourcePdfOptionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "源 PDF 文件选项";
			this._ExtractImageOptionBox.ResumeLayout (false);
			this._ExtractImageOptionBox.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropWidthBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinCropHeightBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinWidthBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinHeightBox)).EndInit ();
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
		private System.Windows.Forms.Label _MessageLabel;
		private System.Windows.Forms.TextBox _PageRangeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _SourceFileBox;
		private System.Windows.Forms.CheckBox _ImportImagesOnlyBox;
		private System.Windows.Forms.GroupBox _ExtractImageOptionBox;
		private System.Windows.Forms.NumericUpDown _RightMarginBox;
		private System.Windows.Forms.NumericUpDown _LeftMarginBox;
		private System.Windows.Forms.NumericUpDown _BottomMarginBox;
		private System.Windows.Forms.NumericUpDown _TopMarginBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox _MergeImagesBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox _InvertBlackAndWhiteImageBox;
		private System.Windows.Forms.CheckBox _VerticalFlipImagesBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown _MinWidthBox;
		private System.Windows.Forms.NumericUpDown _MinHeightBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown _MinCropWidthBox;
		private System.Windows.Forms.NumericUpDown _MinCropHeightBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
	}
}

