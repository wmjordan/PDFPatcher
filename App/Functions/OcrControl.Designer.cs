namespace PDFPatcher.Functions
{
	partial class OcrControl
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
			this.label3 = new System.Windows.Forms.Label();
			this._PageRangeBox = new System.Windows.Forms.TextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._PrintOcrResultBox = new System.Windows.Forms.CheckBox();
			this._OutputOriginalOcrResultBox = new System.Windows.Forms.CheckBox();
			this._ConvertToMonoColorBox = new System.Windows.Forms.CheckBox();
			this._RemoveSpaceBetweenChineseBox = new System.Windows.Forms.CheckBox();
			this._SaveOcredImageBox = new System.Windows.Forms.CheckBox();
			this._CompressWhiteSpaceBox = new System.Windows.Forms.CheckBox();
			this._DetectContentPunctuationsBox = new System.Windows.Forms.CheckBox();
			this._DetectColumnsBox = new System.Windows.Forms.CheckBox();
			this._StretchBox = new System.Windows.Forms.CheckBox();
			this._OrientBox = new System.Windows.Forms.CheckBox();
			this._OcrLangBox = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._WritingDirectionBox = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this._QuantitiveFactorBox = new System.Windows.Forms.NumericUpDown();
			this._SourceFileControl = new PDFPatcher.SourceFileControl();
			this._BookmarkControl = new PDFPatcher.BookmarkControl();
			this._TargetFileControl = new PDFPatcher.TargetFileControl();
			this._ExportBookmarkButton = new System.Windows.Forms.Button();
			this._ImportOcrResultButton = new EnhancedGlassButton.GlassButton();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._QuantitiveFactorBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(89, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "识别页码范围：";
			// 
			// _PageRangeBox
			// 
			this._PageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._PageRangeBox.Location = new System.Drawing.Point(98, 9);
			this._PageRangeBox.Name = "_PageRangeBox";
			this._PageRangeBox.Size = new System.Drawing.Size(335, 21);
			this._PageRangeBox.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Location = new System.Drawing.Point(13, 124);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(447, 194);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._PrintOcrResultBox);
			this.tabPage1.Controls.Add(this._OutputOriginalOcrResultBox);
			this.tabPage1.Controls.Add(this._ConvertToMonoColorBox);
			this.tabPage1.Controls.Add(this._RemoveSpaceBetweenChineseBox);
			this.tabPage1.Controls.Add(this._SaveOcredImageBox);
			this.tabPage1.Controls.Add(this._CompressWhiteSpaceBox);
			this.tabPage1.Controls.Add(this._DetectContentPunctuationsBox);
			this.tabPage1.Controls.Add(this._DetectColumnsBox);
			this.tabPage1.Controls.Add(this._StretchBox);
			this.tabPage1.Controls.Add(this._OrientBox);
			this.tabPage1.Controls.Add(this._OcrLangBox);
			this.tabPage1.Controls.Add(this.label13);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this._WritingDirectionBox);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this._PageRangeBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(439, 168);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "识别选项";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _PrintOcrResultBox
			// 
			this._PrintOcrResultBox.AutoSize = true;
			this._PrintOcrResultBox.Location = new System.Drawing.Point(6, 150);
			this._PrintOcrResultBox.Name = "_PrintOcrResultBox";
			this._PrintOcrResultBox.Size = new System.Drawing.Size(180, 16);
			this._PrintOcrResultBox.TabIndex = 17;
			this._PrintOcrResultBox.Text = "在日志窗口输出识别后的文本";
			this._PrintOcrResultBox.UseVisualStyleBackColor = true;
			// 
			// _OutputOriginalOcrResultBox
			// 
			this._OutputOriginalOcrResultBox.AutoSize = true;
			this._OutputOriginalOcrResultBox.Location = new System.Drawing.Point(202, 128);
			this._OutputOriginalOcrResultBox.Name = "_OutputOriginalOcrResultBox";
			this._OutputOriginalOcrResultBox.Size = new System.Drawing.Size(132, 16);
			this._OutputOriginalOcrResultBox.TabIndex = 16;
			this._OutputOriginalOcrResultBox.Text = "保存原始的识别结果";
			this._OutputOriginalOcrResultBox.UseVisualStyleBackColor = true;
			this._OutputOriginalOcrResultBox.CheckedChanged += new System.EventHandler(this.ControlEvent);
			// 
			// _ConvertToMonoColorBox
			// 
			this._ConvertToMonoColorBox.AutoSize = true;
			this._ConvertToMonoColorBox.Location = new System.Drawing.Point(6, 128);
			this._ConvertToMonoColorBox.Name = "_ConvertToMonoColorBox";
			this._ConvertToMonoColorBox.Size = new System.Drawing.Size(144, 16);
			this._ConvertToMonoColorBox.TabIndex = 12;
			this._ConvertToMonoColorBox.Text = "转换为黑白图片再识别";
			this._ConvertToMonoColorBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveSpaceBetweenChineseBox
			// 
			this._RemoveSpaceBetweenChineseBox.AutoSize = true;
			this._RemoveSpaceBetweenChineseBox.Location = new System.Drawing.Point(202, 106);
			this._RemoveSpaceBetweenChineseBox.Name = "_RemoveSpaceBetweenChineseBox";
			this._RemoveSpaceBetweenChineseBox.Size = new System.Drawing.Size(120, 16);
			this._RemoveSpaceBetweenChineseBox.TabIndex = 11;
			this._RemoveSpaceBetweenChineseBox.Text = "删除汉字间的空格";
			this._RemoveSpaceBetweenChineseBox.UseVisualStyleBackColor = true;
			// 
			// _SaveOcredImageBox
			// 
			this._SaveOcredImageBox.AutoSize = true;
			this._SaveOcredImageBox.Location = new System.Drawing.Point(202, 150);
			this._SaveOcredImageBox.Name = "_SaveOcredImageBox";
			this._SaveOcredImageBox.Size = new System.Drawing.Size(156, 16);
			this._SaveOcredImageBox.TabIndex = 13;
			this._SaveOcredImageBox.Text = "保存识别引擎处理的图片";
			this._SaveOcredImageBox.UseVisualStyleBackColor = true;
			this._SaveOcredImageBox.Visible = false;
			// 
			// _CompressWhiteSpaceBox
			// 
			this._CompressWhiteSpaceBox.AutoSize = true;
			this._CompressWhiteSpaceBox.Location = new System.Drawing.Point(6, 106);
			this._CompressWhiteSpaceBox.Name = "_CompressWhiteSpaceBox";
			this._CompressWhiteSpaceBox.Size = new System.Drawing.Size(132, 16);
			this._CompressWhiteSpaceBox.TabIndex = 10;
			this._CompressWhiteSpaceBox.Text = "压缩连续出现的空格";
			this._CompressWhiteSpaceBox.UseVisualStyleBackColor = true;
			// 
			// _DetectContentPunctuationsBox
			// 
			this._DetectContentPunctuationsBox.AutoSize = true;
			this._DetectContentPunctuationsBox.Location = new System.Drawing.Point(202, 84);
			this._DetectContentPunctuationsBox.Name = "_DetectContentPunctuationsBox";
			this._DetectContentPunctuationsBox.Size = new System.Drawing.Size(192, 16);
			this._DetectContentPunctuationsBox.TabIndex = 9;
			this._DetectContentPunctuationsBox.Text = "识别目录页的点（……）分隔符";
			this._DetectContentPunctuationsBox.UseVisualStyleBackColor = true;
			// 
			// _DetectColumnsBox
			// 
			this._DetectColumnsBox.AutoSize = true;
			this._DetectColumnsBox.Location = new System.Drawing.Point(6, 84);
			this._DetectColumnsBox.Name = "_DetectColumnsBox";
			this._DetectColumnsBox.Size = new System.Drawing.Size(96, 16);
			this._DetectColumnsBox.TabIndex = 8;
			this._DetectColumnsBox.Text = "识别分栏排版";
			this._DetectColumnsBox.UseVisualStyleBackColor = true;
			// 
			// _StretchBox
			// 
			this._StretchBox.AutoSize = true;
			this._StretchBox.Location = new System.Drawing.Point(202, 62);
			this._StretchBox.Name = "_StretchBox";
			this._StretchBox.Size = new System.Drawing.Size(96, 16);
			this._StretchBox.TabIndex = 7;
			this._StretchBox.Text = "纠正倾斜页面";
			this._StretchBox.UseVisualStyleBackColor = true;
			// 
			// _OrientBox
			// 
			this._OrientBox.AutoSize = true;
			this._OrientBox.Location = new System.Drawing.Point(6, 62);
			this._OrientBox.Name = "_OrientBox";
			this._OrientBox.Size = new System.Drawing.Size(96, 16);
			this._OrientBox.TabIndex = 6;
			this._OrientBox.Text = "检测页面方向";
			this._OrientBox.UseVisualStyleBackColor = true;
			// 
			// _OcrLangBox
			// 
			this._OcrLangBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._OcrLangBox.FormattingEnabled = true;
			this._OcrLangBox.Location = new System.Drawing.Point(295, 36);
			this._OcrLangBox.Name = "_OcrLangBox";
			this._OcrLangBox.Size = new System.Drawing.Size(76, 20);
			this._OcrLangBox.TabIndex = 5;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(200, 39);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(89, 12);
			this.label13.TabIndex = 4;
			this.label13.Text = "文字识别语言：";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 39);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "文字排版方向：";
			// 
			// _WritingDirectionBox
			// 
			this._WritingDirectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._WritingDirectionBox.FormattingEnabled = true;
			this._WritingDirectionBox.Items.AddRange(new object[] {
            "自动检测",
            "横向",
            "纵向"});
			this._WritingDirectionBox.Location = new System.Drawing.Point(98, 36);
			this._WritingDirectionBox.Name = "_WritingDirectionBox";
			this._WritingDirectionBox.Size = new System.Drawing.Size(76, 20);
			this._WritingDirectionBox.TabIndex = 3;
			this._WritingDirectionBox.SelectedIndexChanged += new System.EventHandler(this.ControlEvent);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(20, 96);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(89, 12);
			this.label14.TabIndex = 14;
			this.label14.Text = "尺寸量化因数：";
			this.label14.Visible = false;
			// 
			// _QuantitiveFactorBox
			// 
			this._QuantitiveFactorBox.DecimalPlaces = 2;
			this._QuantitiveFactorBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._QuantitiveFactorBox.Location = new System.Drawing.Point(102, 94);
			this._QuantitiveFactorBox.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this._QuantitiveFactorBox.Name = "_QuantitiveFactorBox";
			this._QuantitiveFactorBox.Size = new System.Drawing.Size(53, 21);
			this._QuantitiveFactorBox.TabIndex = 15;
			this._QuantitiveFactorBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._QuantitiveFactorBox.Visible = false;
			// 
			// _SourceFileControl
			// 
			this._SourceFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SourceFileControl.Location = new System.Drawing.Point(9, 3);
			this._SourceFileControl.Name = "_SourceFileControl";
			this._SourceFileControl.Size = new System.Drawing.Size(454, 24);
			this._SourceFileControl.TabIndex = 1;
			// 
			// _BookmarkControl
			// 
			this._BookmarkControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._BookmarkControl.LabelText = "识别结果文件：";
			this._BookmarkControl.Location = new System.Drawing.Point(9, 33);
			this._BookmarkControl.Name = "_BookmarkControl";
			this._BookmarkControl.Size = new System.Drawing.Size(454, 25);
			this._BookmarkControl.TabIndex = 2;
			this._BookmarkControl.UseForBookmarkExport = true;
			// 
			// _TargetFileControl
			// 
			this._TargetFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetFileControl.Location = new System.Drawing.Point(9, 64);
			this._TargetFileControl.Name = "_TargetFileControl";
			this._TargetFileControl.Size = new System.Drawing.Size(454, 25);
			this._TargetFileControl.TabIndex = 16;
			// 
			// _ExportBookmarkButton
			// 
			this._ExportBookmarkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ExportBookmarkButton.Image = global::PDFPatcher.Properties.Resources.Ocr;
			this._ExportBookmarkButton.Location = new System.Drawing.Point(211, 96);
			this._ExportBookmarkButton.Name = "_ExportBookmarkButton";
			this._ExportBookmarkButton.Size = new System.Drawing.Size(120, 23);
			this._ExportBookmarkButton.TabIndex = 3;
			this._ExportBookmarkButton.Text = "识别图像文本(&S)";
			this._ExportBookmarkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExportBookmarkButton.UseVisualStyleBackColor = true;
			this._ExportBookmarkButton.Click += new System.EventHandler(this.Button_Click);
			// 
			// _ImportOcrResultButton
			// 
			this._ImportOcrResultButton.AlternativeFocusBorderColor = System.Drawing.SystemColors.Highlight;
			this._ImportOcrResultButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ImportOcrResultButton.AnimateGlow = true;
			this._ImportOcrResultButton.BackColor = System.Drawing.SystemColors.Highlight;
			this._ImportOcrResultButton.CornerRadius = 3;
			this._ImportOcrResultButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ImportOcrResultButton.GlowColor = System.Drawing.Color.White;
			this._ImportOcrResultButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._ImportOcrResultButton.InnerBorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this._ImportOcrResultButton.Location = new System.Drawing.Point(337, 95);
			this._ImportOcrResultButton.Name = "_ImportOcrResultButton";
			this._ImportOcrResultButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ImportOcrResultButton.ShowFocusBorder = true;
			this._ImportOcrResultButton.Size = new System.Drawing.Size(123, 29);
			this._ImportOcrResultButton.TabIndex = 17;
			this._ImportOcrResultButton.Text = "写入PDF文档(&X)";
			this._ImportOcrResultButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ImportOcrResultButton.Click += new System.EventHandler(this.Button_Click);
			// 
			// OcrControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._TargetFileControl);
			this.Controls.Add(this._ExportBookmarkButton);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._SourceFileControl);
			this.Controls.Add(this.label14);
			this.Controls.Add(this._BookmarkControl);
			this.Controls.Add(this._QuantitiveFactorBox);
			this.Controls.Add(this._ImportOcrResultButton);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "OcrControl";
			this.Size = new System.Drawing.Size(475, 333);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._QuantitiveFactorBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SourceFileControl _SourceFileControl;
		private BookmarkControl _BookmarkControl;
		private System.Windows.Forms.Button _ExportBookmarkButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _PageRangeBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox _WritingDirectionBox;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.NumericUpDown _QuantitiveFactorBox;
		private System.Windows.Forms.CheckBox _StretchBox;
		private System.Windows.Forms.CheckBox _OrientBox;
		private System.Windows.Forms.ComboBox _OcrLangBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckBox _DetectColumnsBox;
		private System.Windows.Forms.CheckBox _DetectContentPunctuationsBox;
		private System.Windows.Forms.CheckBox _CompressWhiteSpaceBox;
		private System.Windows.Forms.CheckBox _RemoveSpaceBetweenChineseBox;
		private System.Windows.Forms.CheckBox _SaveOcredImageBox;
		private System.Windows.Forms.CheckBox _ConvertToMonoColorBox;
		private System.Windows.Forms.CheckBox _OutputOriginalOcrResultBox;
		private TargetFileControl _TargetFileControl;
		private EnhancedGlassButton.GlassButton _ImportOcrResultButton;
		private System.Windows.Forms.CheckBox _PrintOcrResultBox;

	}
}
