namespace PDFPatcher.Functions
{
	partial class ExtractImageControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractImageControl));
			this._ExtractPageRangeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._BrowseTargetPdfButton = new System.Windows.Forms.Button();
			this._SaveImageBox = new System.Windows.Forms.FolderBrowserDialog();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._FileMaskPreviewBox = new System.Windows.Forms.Label();
			this._FileNameMaskBox = new System.Windows.Forms.ComboBox();
			this._MergeImageBox = new System.Windows.Forms.CheckBox();
			this._VerticalFlipImageBox = new System.Windows.Forms.CheckBox();
			this._InvertBlackAndWhiteBox = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this._MinWidthBox = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this._MinHeightBox = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._SkipRedundantImagesBox = new System.Windows.Forms.CheckBox();
			this._MonoPngBox = new System.Windows.Forms.RadioButton();
			this._MonoTiffBox = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this._InvertSoftMaskBox = new System.Windows.Forms.CheckBox();
			this._ExportSoftMaskBox = new System.Windows.Forms.CheckBox();
			this._ExportAnnotImagesBox = new System.Windows.Forms.CheckBox();
			this._MergeJpgToPngBox = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this._AutoOutputDirBox = new System.Windows.Forms.CheckBox();
			this._TargetBox = new PDFPatcher.HistoryComboBox();
			this._SourceFileControl = new PDFPatcher.SourceFileControl();
			this._ExtractButton = new EnhancedGlassButton.GlassButton();
			this._ExtractOnlyInPageBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this._MinWidthBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._MinHeightBox)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _ExtractPageRangeBox
			// 
			this._ExtractPageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ExtractPageRangeBox.Location = new System.Drawing.Point(93, 6);
			this._ExtractPageRangeBox.Name = "_ExtractPageRangeBox";
			this._ExtractPageRangeBox.Size = new System.Drawing.Size(356, 21);
			this._ExtractPageRangeBox.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "页码范围：";
			// 
			// _BrowseTargetPdfButton
			// 
			this._BrowseTargetPdfButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._BrowseTargetPdfButton.Image = global::PDFPatcher.Properties.Resources.ImageFolder;
			this._BrowseTargetPdfButton.Location = new System.Drawing.Point(397, 34);
			this._BrowseTargetPdfButton.Name = "_BrowseTargetPdfButton";
			this._BrowseTargetPdfButton.Size = new System.Drawing.Size(75, 23);
			this._BrowseTargetPdfButton.TabIndex = 4;
			this._BrowseTargetPdfButton.Text = "浏览...";
			this._BrowseTargetPdfButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._BrowseTargetPdfButton.UseVisualStyleBackColor = true;
			this._BrowseTargetPdfButton.Click += new System.EventHandler(this._BrowseTargetPdfButton_Click);
			// 
			// _SaveImageBox
			// 
			this._SaveImageBox.Description = "请选择保存图片的文件夹";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 39);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "输出图片位置：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "文件名掩码：";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 95);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "文件名示例：";
			// 
			// _FileMaskPreviewBox
			// 
			this._FileMaskPreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._FileMaskPreviewBox.Location = new System.Drawing.Point(91, 95);
			this._FileMaskPreviewBox.Name = "_FileMaskPreviewBox";
			this._FileMaskPreviewBox.Size = new System.Drawing.Size(330, 31);
			this._FileMaskPreviewBox.TabIndex = 3;
			// 
			// _FileNameMaskBox
			// 
			this._FileNameMaskBox.FormattingEnabled = true;
			this._FileNameMaskBox.Items.AddRange(new object[] {
            "0000",
            "000",
            "0",
            "图片0000"});
			this._FileNameMaskBox.Location = new System.Drawing.Point(93, 6);
			this._FileNameMaskBox.Name = "_FileNameMaskBox";
			this._FileNameMaskBox.Size = new System.Drawing.Size(318, 20);
			this._FileNameMaskBox.TabIndex = 1;
			this._FileNameMaskBox.TextChanged += new System.EventHandler(this._FileNameMaskBox_TextChanged);
			// 
			// _MergeImageBox
			// 
			this._MergeImageBox.AutoSize = true;
			this._MergeImageBox.Location = new System.Drawing.Point(8, 33);
			this._MergeImageBox.Name = "_MergeImageBox";
			this._MergeImageBox.Size = new System.Drawing.Size(156, 16);
			this._MergeImageBox.TabIndex = 3;
			this._MergeImageBox.Text = "尝试合并相同页面的图片";
			this._MergeImageBox.UseVisualStyleBackColor = true;
			// 
			// _VerticalFlipImageBox
			// 
			this._VerticalFlipImageBox.AutoSize = true;
			this._VerticalFlipImageBox.Location = new System.Drawing.Point(208, 55);
			this._VerticalFlipImageBox.Name = "_VerticalFlipImageBox";
			this._VerticalFlipImageBox.Size = new System.Drawing.Size(96, 16);
			this._VerticalFlipImageBox.TabIndex = 6;
			this._VerticalFlipImageBox.Text = "垂直翻转图片";
			this._VerticalFlipImageBox.UseVisualStyleBackColor = true;
			// 
			// _InvertBlackAndWhiteBox
			// 
			this._InvertBlackAndWhiteBox.AutoSize = true;
			this._InvertBlackAndWhiteBox.Location = new System.Drawing.Point(8, 77);
			this._InvertBlackAndWhiteBox.Name = "_InvertBlackAndWhiteBox";
			this._InvertBlackAndWhiteBox.Size = new System.Drawing.Size(132, 16);
			this._InvertBlackAndWhiteBox.TabIndex = 7;
			this._InvertBlackAndWhiteBox.Text = "反转黑白图片的颜色";
			this._InvertBlackAndWhiteBox.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 124);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 12);
			this.label7.TabIndex = 13;
			this.label7.Text = "忽略宽度小于";
			// 
			// _MinWidthBox
			// 
			this._MinWidthBox.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._MinWidthBox.Location = new System.Drawing.Point(89, 122);
			this._MinWidthBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._MinWidthBox.Name = "_MinWidthBox";
			this._MinWidthBox.Size = new System.Drawing.Size(43, 21);
			this._MinWidthBox.TabIndex = 14;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(138, 124);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 12);
			this.label8.TabIndex = 15;
			this.label8.Text = "或高度小于";
			// 
			// _MinHeightBox
			// 
			this._MinHeightBox.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._MinHeightBox.Location = new System.Drawing.Point(209, 122);
			this._MinHeightBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._MinHeightBox.Name = "_MinHeightBox";
			this._MinHeightBox.Size = new System.Drawing.Size(43, 21);
			this._MinHeightBox.TabIndex = 16;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(258, 124);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 12);
			this.label9.TabIndex = 17;
			this.label9.Text = "像素的图片";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(12, 92);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(463, 229);
			this.tabControl1.TabIndex = 7;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._ExtractOnlyInPageBox);
			this.tabPage1.Controls.Add(this._SkipRedundantImagesBox);
			this.tabPage1.Controls.Add(this._MonoPngBox);
			this.tabPage1.Controls.Add(this._MonoTiffBox);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this._InvertSoftMaskBox);
			this.tabPage1.Controls.Add(this._ExportSoftMaskBox);
			this.tabPage1.Controls.Add(this._ExportAnnotImagesBox);
			this.tabPage1.Controls.Add(this._MergeJpgToPngBox);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this._InvertBlackAndWhiteBox);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label8);
			this.tabPage1.Controls.Add(this._VerticalFlipImageBox);
			this.tabPage1.Controls.Add(this._ExtractPageRangeBox);
			this.tabPage1.Controls.Add(this._MinWidthBox);
			this.tabPage1.Controls.Add(this._MergeImageBox);
			this.tabPage1.Controls.Add(this._MinHeightBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage1.Size = new System.Drawing.Size(455, 203);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "选项";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _SkipRedundantImagesBox
			// 
			this._SkipRedundantImagesBox.AutoSize = true;
			this._SkipRedundantImagesBox.Location = new System.Drawing.Point(8, 146);
			this._SkipRedundantImagesBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._SkipRedundantImagesBox.Name = "_SkipRedundantImagesBox";
			this._SkipRedundantImagesBox.Size = new System.Drawing.Size(132, 16);
			this._SkipRedundantImagesBox.TabIndex = 18;
			this._SkipRedundantImagesBox.Text = "避免导出相同的图片";
			this._SkipRedundantImagesBox.UseVisualStyleBackColor = true;
			// 
			// _MonoPngBox
			// 
			this._MonoPngBox.AutoSize = true;
			this._MonoPngBox.Location = new System.Drawing.Point(208, 99);
			this._MonoPngBox.Name = "_MonoPngBox";
			this._MonoPngBox.Size = new System.Drawing.Size(71, 16);
			this._MonoPngBox.TabIndex = 12;
			this._MonoPngBox.TabStop = true;
			this._MonoPngBox.Text = "PNG 文件";
			this._MonoPngBox.UseVisualStyleBackColor = true;
			// 
			// _MonoTiffBox
			// 
			this._MonoTiffBox.AutoSize = true;
			this._MonoTiffBox.Location = new System.Drawing.Point(125, 99);
			this._MonoTiffBox.Name = "_MonoTiffBox";
			this._MonoTiffBox.Size = new System.Drawing.Size(77, 16);
			this._MonoTiffBox.TabIndex = 11;
			this._MonoTiffBox.TabStop = true;
			this._MonoTiffBox.Text = "TIFF 文件";
			this._MonoTiffBox.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 101);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 12);
			this.label1.TabIndex = 10;
			this.label1.Text = "将黑白图片保存为：";
			// 
			// _InvertSoftMaskBox
			// 
			this._InvertSoftMaskBox.AutoSize = true;
			this._InvertSoftMaskBox.Location = new System.Drawing.Point(310, 77);
			this._InvertSoftMaskBox.Name = "_InvertSoftMaskBox";
			this._InvertSoftMaskBox.Size = new System.Drawing.Size(96, 16);
			this._InvertSoftMaskBox.TabIndex = 9;
			this._InvertSoftMaskBox.Text = "反转掩模颜色";
			this._InvertSoftMaskBox.UseVisualStyleBackColor = true;
			// 
			// _ExportSoftMaskBox
			// 
			this._ExportSoftMaskBox.AutoSize = true;
			this._ExportSoftMaskBox.Location = new System.Drawing.Point(208, 77);
			this._ExportSoftMaskBox.Name = "_ExportSoftMaskBox";
			this._ExportSoftMaskBox.Size = new System.Drawing.Size(96, 16);
			this._ExportSoftMaskBox.TabIndex = 8;
			this._ExportSoftMaskBox.Text = "导出图像掩模";
			this._ExportSoftMaskBox.UseVisualStyleBackColor = true;
			// 
			// _ExportAnnotImagesBox
			// 
			this._ExportAnnotImagesBox.AutoSize = true;
			this._ExportAnnotImagesBox.Location = new System.Drawing.Point(8, 55);
			this._ExportAnnotImagesBox.Name = "_ExportAnnotImagesBox";
			this._ExportAnnotImagesBox.Size = new System.Drawing.Size(120, 16);
			this._ExportAnnotImagesBox.TabIndex = 5;
			this._ExportAnnotImagesBox.Text = "导出批注内的图片";
			this._ExportAnnotImagesBox.UseVisualStyleBackColor = true;
			// 
			// _MergeJpgToPngBox
			// 
			this._MergeJpgToPngBox.AutoSize = true;
			this._MergeJpgToPngBox.Location = new System.Drawing.Point(208, 33);
			this._MergeJpgToPngBox.Name = "_MergeJpgToPngBox";
			this._MergeJpgToPngBox.Size = new System.Drawing.Size(192, 16);
			this._MergeJpgToPngBox.TabIndex = 4;
			this._MergeJpgToPngBox.Text = "JPEG 图片无损合并为 PNG 图片";
			this._MergeJpgToPngBox.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label6);
			this.tabPage2.Controls.Add(this._FileNameMaskBox);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this._FileMaskPreviewBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage2.Size = new System.Drawing.Size(455, 203);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "文件命名";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(91, 30);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(330, 65);
			this.label6.TabIndex = 4;
			this.label6.Text = "文件名掩码说明：\r\n“0000”：不足四位用0补足四位。\r\n“0”：文件名按实际页码，不用0补位。\r\n可用英文双引号将文本括起来（如“\"相约2046\"0”，前面的" +
    "“2046”不会被解释为占位符）。";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.textBox1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage3.Size = new System.Drawing.Size(455, 203);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "页码范围说明";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(6, 6);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(437, 180);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// _AutoOutputDirBox
			// 
			this._AutoOutputDirBox.AutoSize = true;
			this._AutoOutputDirBox.Location = new System.Drawing.Point(19, 67);
			this._AutoOutputDirBox.Name = "_AutoOutputDirBox";
			this._AutoOutputDirBox.Size = new System.Drawing.Size(156, 16);
			this._AutoOutputDirBox.TabIndex = 5;
			this._AutoOutputDirBox.Text = "自动指定输出图片的位置";
			this._AutoOutputDirBox.UseVisualStyleBackColor = true;
			// 
			// _TargetBox
			// 
			this._TargetBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetBox.Contents = null;
			this._TargetBox.FormattingEnabled = true;
			this._TargetBox.Location = new System.Drawing.Point(112, 36);
			this._TargetBox.MaxItemCount = 16;
			this._TargetBox.Name = "_TargetBox";
			this._TargetBox.Size = new System.Drawing.Size(279, 20);
			this._TargetBox.TabIndex = 3;
			// 
			// _SourceFileControl
			// 
			this._SourceFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SourceFileControl.Location = new System.Drawing.Point(12, 3);
			this._SourceFileControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._SourceFileControl.Name = "_SourceFileControl";
			this._SourceFileControl.Size = new System.Drawing.Size(463, 29);
			this._SourceFileControl.TabIndex = 1;
			// 
			// _ExtractButton
			// 
			this._ExtractButton.AlternativeFocusBorderColor = System.Drawing.SystemColors.Highlight;
			this._ExtractButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ExtractButton.AnimateGlow = true;
			this._ExtractButton.BackColor = System.Drawing.SystemColors.Highlight;
			this._ExtractButton.CornerRadius = 3;
			this._ExtractButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ExtractButton.GlowColor = System.Drawing.Color.White;
			this._ExtractButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._ExtractButton.InnerBorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this._ExtractButton.Location = new System.Drawing.Point(349, 63);
			this._ExtractButton.Name = "_ExtractButton";
			this._ExtractButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ExtractButton.ShowFocusBorder = true;
			this._ExtractButton.Size = new System.Drawing.Size(123, 29);
			this._ExtractButton.TabIndex = 13;
			this._ExtractButton.Text = " 提取图片(&T)";
			this._ExtractButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExtractButton.Click += new System.EventHandler(this._ExtractButton_Click);
			// 
			// _ExtractOnlyInPageBox
			// 
			this._ExtractOnlyInPageBox.AutoSize = true;
			this._ExtractOnlyInPageBox.Location = new System.Drawing.Point(8, 167);
			this._ExtractOnlyInPageBox.Name = "_ExtractOnlyInPageBox";
			this._ExtractOnlyInPageBox.Size = new System.Drawing.Size(120, 16);
			this._ExtractOnlyInPageBox.TabIndex = 19;
			this._ExtractOnlyInPageBox.Text = "仅导出可见的图片";
			this._ExtractOnlyInPageBox.UseVisualStyleBackColor = true;
			// 
			// ExtractImageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._ExtractButton);
			this.Controls.Add(this._AutoOutputDirBox);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._TargetBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._SourceFileControl);
			this.Controls.Add(this._BrowseTargetPdfButton);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "ExtractImageControl";
			this.Size = new System.Drawing.Size(487, 333);
			this.Load += new System.EventHandler(this.Control_Show);
			this.VisibleChanged += new System.EventHandler(this.Control_Show);
			this.Click += new System.EventHandler(this._ExtractButton_Click);
			((System.ComponentModel.ISupportInitialize)(this._MinWidthBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._MinHeightBox)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _ExtractPageRangeBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button _BrowseTargetPdfButton;
		private SourceFileControl _SourceFileControl;
		private System.Windows.Forms.FolderBrowserDialog _SaveImageBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label _FileMaskPreviewBox;
		private System.Windows.Forms.ComboBox _FileNameMaskBox;
		private HistoryComboBox _TargetBox;
		private System.Windows.Forms.CheckBox _MergeImageBox;
		private System.Windows.Forms.CheckBox _VerticalFlipImageBox;
		private System.Windows.Forms.CheckBox _InvertBlackAndWhiteBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown _MinHeightBox;
		private System.Windows.Forms.NumericUpDown _MinWidthBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.CheckBox _AutoOutputDirBox;
		private System.Windows.Forms.CheckBox _MergeJpgToPngBox;
		private System.Windows.Forms.CheckBox _ExportAnnotImagesBox;
		private System.Windows.Forms.CheckBox _ExportSoftMaskBox;
		private System.Windows.Forms.CheckBox _InvertSoftMaskBox;
		private System.Windows.Forms.RadioButton _MonoTiffBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton _MonoPngBox;
		private EnhancedGlassButton.GlassButton _ExtractButton;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox _SkipRedundantImagesBox;
		private System.Windows.Forms.CheckBox _ExtractOnlyInPageBox;
	}
}
