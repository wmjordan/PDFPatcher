namespace PDFPatcher.Functions
{
	partial class RenderImageControl
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
			this._ExtractPageRangeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._SaveImageBox = new System.Windows.Forms.FolderBrowserDialog();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._FileMaskPreviewBox = new System.Windows.Forms.Label();
			this._FileNameMaskBox = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this._InvertColorBox = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._RotationBox = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this._HorizontalFlipImageBox = new System.Windows.Forms.CheckBox();
			this._HideAnnotationsBox = new System.Windows.Forms.CheckBox();
			this._VerticalFlipImageBox = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._ResolutionBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this._ExtractPageImageWidthBox = new System.Windows.Forms.NumericUpDown();
			this._ExtractPageRatioBox = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this._SpecificRatioBox = new System.Windows.Forms.RadioButton();
			this._SpecificWidthBox = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._ColorSpaceRgbBox = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this._QuantizeBox = new System.Windows.Forms.CheckBox();
			this._ColorSpaceGrayBox = new System.Windows.Forms.RadioButton();
			this.label11 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this._JpegQualityBox = new System.Windows.Forms.ComboBox();
			this._ImageFormatBox = new System.Windows.Forms.ComboBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._AutoOutputDirBox = new System.Windows.Forms.CheckBox();
			this._BrowseTargetPdfButton = new System.Windows.Forms.Button();
			this._TargetBox = new PDFPatcher.HistoryComboBox();
			this._SourceFileControl = new PDFPatcher.SourceFileControl();
			this._ExtractButton = new EnhancedGlassButton.GlassButton();
			this._RenderToPdfBox = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._ExtractPageImageWidthBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._ExtractPageRatioBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _ExtractPageRangeBox
			// 
			this._ExtractPageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ExtractPageRangeBox.Location = new System.Drawing.Point(93, 6);
			this._ExtractPageRangeBox.Name = "_ExtractPageRangeBox";
			this._ExtractPageRangeBox.Size = new System.Drawing.Size(478, 21);
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
			this.label4.TabIndex = 1;
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
			this.label5.Location = new System.Drawing.Point(10, 30);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "文件名示例：";
			// 
			// _FileMaskPreviewBox
			// 
			this._FileMaskPreviewBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._FileMaskPreviewBox.Location = new System.Drawing.Point(93, 30);
			this._FileMaskPreviewBox.Name = "_FileMaskPreviewBox";
			this._FileMaskPreviewBox.Size = new System.Drawing.Size(330, 31);
			this._FileMaskPreviewBox.TabIndex = 3;
			// 
			// _FileNameMaskBox
			// 
			this._FileNameMaskBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._FileNameMaskBox.FormattingEnabled = true;
			this._FileNameMaskBox.Items.AddRange(new object[] {
            "0000",
            "000",
            "0",
            "图片0000"});
			this._FileNameMaskBox.Location = new System.Drawing.Point(93, 6);
			this._FileNameMaskBox.Name = "_FileNameMaskBox";
			this._FileNameMaskBox.Size = new System.Drawing.Size(244, 20);
			this._FileNameMaskBox.TabIndex = 1;
			this._FileNameMaskBox.TextChanged += new System.EventHandler(this._FileNameMaskBox_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(46, 30);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(522, 38);
			this.label6.TabIndex = 2;
			this.label6.Text = "用“-”表示起止页码。多个页码可用“;”、“,”或“ ”（空格）隔开，如“1;4-15;2 56”，表示依次提取第1页、第4至15页、第2页和第56页的内容。不指" +
    "定页码时提取源文件所有页面的内容。";
			// 
			// _InvertColorBox
			// 
			this._InvertColorBox.AutoSize = true;
			this._InvertColorBox.Location = new System.Drawing.Point(10, 48);
			this._InvertColorBox.Name = "_InvertColorBox";
			this._InvertColorBox.Size = new System.Drawing.Size(108, 16);
			this._InvertColorBox.TabIndex = 3;
			this._InvertColorBox.Text = "反转图片的颜色";
			this._InvertColorBox.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 92);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(585, 229);
			this.tabControl1.TabIndex = 5;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this._JpegQualityBox);
			this.tabPage1.Controls.Add(this._ImageFormatBox);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this._ExtractPageRangeBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(577, 203);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "选项";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this._RotationBox);
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this._HorizontalFlipImageBox);
			this.groupBox3.Controls.Add(this._HideAnnotationsBox);
			this.groupBox3.Controls.Add(this._VerticalFlipImageBox);
			this.groupBox3.Location = new System.Drawing.Point(378, 97);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(179, 100);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "其它";
			// 
			// _RotationBox
			// 
			this._RotationBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._RotationBox.FormattingEnabled = true;
			this._RotationBox.Items.AddRange(new object[] {
            "不旋转",
            "顺时针90度",
            "180度",
            "逆时针90度"});
			this._RotationBox.Location = new System.Drawing.Point(76, 20);
			this._RotationBox.Name = "_RotationBox";
			this._RotationBox.Size = new System.Drawing.Size(86, 20);
			this._RotationBox.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(5, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 12);
			this.label9.TabIndex = 0;
			this.label9.Text = "旋转角度：";
			// 
			// _HorizontalFlipImageBox
			// 
			this._HorizontalFlipImageBox.AutoSize = true;
			this._HorizontalFlipImageBox.Location = new System.Drawing.Point(7, 48);
			this._HorizontalFlipImageBox.Name = "_HorizontalFlipImageBox";
			this._HorizontalFlipImageBox.Size = new System.Drawing.Size(72, 16);
			this._HorizontalFlipImageBox.TabIndex = 2;
			this._HorizontalFlipImageBox.Text = "水平翻转";
			this._HorizontalFlipImageBox.UseVisualStyleBackColor = true;
			// 
			// _HideAnnotationsBox
			// 
			this._HideAnnotationsBox.AutoSize = true;
			this._HideAnnotationsBox.Location = new System.Drawing.Point(7, 70);
			this._HideAnnotationsBox.Name = "_HideAnnotationsBox";
			this._HideAnnotationsBox.Size = new System.Drawing.Size(96, 16);
			this._HideAnnotationsBox.TabIndex = 4;
			this._HideAnnotationsBox.Text = "隐藏批注内容";
			this._HideAnnotationsBox.UseVisualStyleBackColor = true;
			// 
			// _VerticalFlipImageBox
			// 
			this._VerticalFlipImageBox.AutoSize = true;
			this._VerticalFlipImageBox.Location = new System.Drawing.Point(85, 48);
			this._VerticalFlipImageBox.Name = "_VerticalFlipImageBox";
			this._VerticalFlipImageBox.Size = new System.Drawing.Size(72, 16);
			this._VerticalFlipImageBox.TabIndex = 3;
			this._VerticalFlipImageBox.Text = "垂直翻转";
			this._VerticalFlipImageBox.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._ResolutionBox);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this._ExtractPageImageWidthBox);
			this.groupBox2.Controls.Add(this._ExtractPageRatioBox);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this._SpecificRatioBox);
			this.groupBox2.Controls.Add(this._SpecificWidthBox);
			this.groupBox2.Location = new System.Drawing.Point(193, 97);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(179, 100);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "输出图片尺寸";
			// 
			// _ResolutionBox
			// 
			this._ResolutionBox.FormattingEnabled = true;
			this._ResolutionBox.Items.AddRange(new object[] {
            "72",
            "96",
            "100",
            "150",
            "200",
            "300",
            "400",
            "600",
            "1200",
            "2400"});
			this._ResolutionBox.Location = new System.Drawing.Point(86, 74);
			this._ResolutionBox.Name = "_ResolutionBox";
			this._ResolutionBox.Size = new System.Drawing.Size(55, 20);
			this._ResolutionBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 77);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "分辨率：";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(147, 23);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(17, 12);
			this.label12.TabIndex = 5;
			this.label12.Text = "倍";
			// 
			// _ExtractPageImageWidthBox
			// 
			this._ExtractPageImageWidthBox.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this._ExtractPageImageWidthBox.Location = new System.Drawing.Point(86, 48);
			this._ExtractPageImageWidthBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._ExtractPageImageWidthBox.Name = "_ExtractPageImageWidthBox";
			this._ExtractPageImageWidthBox.Size = new System.Drawing.Size(55, 21);
			this._ExtractPageImageWidthBox.TabIndex = 1;
			// 
			// _ExtractPageRatioBox
			// 
			this._ExtractPageRatioBox.DecimalPlaces = 1;
			this._ExtractPageRatioBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._ExtractPageRatioBox.Location = new System.Drawing.Point(86, 21);
			this._ExtractPageRatioBox.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this._ExtractPageRatioBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._ExtractPageRatioBox.Name = "_ExtractPageRatioBox";
			this._ExtractPageRatioBox.Size = new System.Drawing.Size(55, 21);
			this._ExtractPageRatioBox.TabIndex = 4;
			this._ExtractPageRatioBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(147, 77);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(23, 12);
			this.label13.TabIndex = 2;
			this.label13.Text = "DPI";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(147, 50);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(29, 12);
			this.label10.TabIndex = 2;
			this.label10.Text = "像素";
			// 
			// _SpecificRatioBox
			// 
			this._SpecificRatioBox.AutoSize = true;
			this._SpecificRatioBox.Location = new System.Drawing.Point(9, 21);
			this._SpecificRatioBox.Name = "_SpecificRatioBox";
			this._SpecificRatioBox.Size = new System.Drawing.Size(71, 16);
			this._SpecificRatioBox.TabIndex = 3;
			this._SpecificRatioBox.TabStop = true;
			this._SpecificRatioBox.Text = "指定比例";
			this._SpecificRatioBox.UseVisualStyleBackColor = true;
			// 
			// _SpecificWidthBox
			// 
			this._SpecificWidthBox.AutoSize = true;
			this._SpecificWidthBox.Location = new System.Drawing.Point(9, 49);
			this._SpecificWidthBox.Name = "_SpecificWidthBox";
			this._SpecificWidthBox.Size = new System.Drawing.Size(71, 16);
			this._SpecificWidthBox.TabIndex = 0;
			this._SpecificWidthBox.TabStop = true;
			this._SpecificWidthBox.Text = "指定宽度";
			this._SpecificWidthBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._ColorSpaceRgbBox);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this._QuantizeBox);
			this.groupBox1.Controls.Add(this._ColorSpaceGrayBox);
			this.groupBox1.Controls.Add(this._InvertColorBox);
			this.groupBox1.Location = new System.Drawing.Point(8, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(179, 100);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "颜色";
			// 
			// _ColorSpaceRgbBox
			// 
			this._ColorSpaceRgbBox.AutoSize = true;
			this._ColorSpaceRgbBox.Location = new System.Drawing.Point(55, 22);
			this._ColorSpaceRgbBox.Name = "_ColorSpaceRgbBox";
			this._ColorSpaceRgbBox.Size = new System.Drawing.Size(47, 16);
			this._ColorSpaceRgbBox.TabIndex = 1;
			this._ColorSpaceRgbBox.TabStop = true;
			this._ColorSpaceRgbBox.Text = "彩色";
			this._ColorSpaceRgbBox.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 23);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(41, 12);
			this.label8.TabIndex = 0;
			this.label8.Text = "颜色：";
			// 
			// _QuantizeBox
			// 
			this._QuantizeBox.AutoSize = true;
			this._QuantizeBox.Location = new System.Drawing.Point(10, 70);
			this._QuantizeBox.Name = "_QuantizeBox";
			this._QuantizeBox.Size = new System.Drawing.Size(108, 16);
			this._QuantizeBox.TabIndex = 4;
			this._QuantizeBox.Text = "减少图片的颜色";
			this._QuantizeBox.UseVisualStyleBackColor = true;
			// 
			// _ColorSpaceGrayBox
			// 
			this._ColorSpaceGrayBox.AutoSize = true;
			this._ColorSpaceGrayBox.Location = new System.Drawing.Point(108, 22);
			this._ColorSpaceGrayBox.Name = "_ColorSpaceGrayBox";
			this._ColorSpaceGrayBox.Size = new System.Drawing.Size(47, 16);
			this._ColorSpaceGrayBox.TabIndex = 2;
			this._ColorSpaceGrayBox.TabStop = true;
			this._ColorSpaceGrayBox.Text = "灰度";
			this._ColorSpaceGrayBox.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(193, 74);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(89, 12);
			this.label11.TabIndex = 5;
			this.label11.Text = "JPEG图片质量：";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 74);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 12);
			this.label7.TabIndex = 3;
			this.label7.Text = "输出图片格式：";
			// 
			// _JpegQualityBox
			// 
			this._JpegQualityBox.FormattingEnabled = true;
			this._JpegQualityBox.Items.AddRange(new object[] {
            "95",
            "85",
            "75",
            "50",
            "30"});
			this._JpegQualityBox.Location = new System.Drawing.Point(288, 71);
			this._JpegQualityBox.Name = "_JpegQualityBox";
			this._JpegQualityBox.Size = new System.Drawing.Size(86, 20);
			this._JpegQualityBox.TabIndex = 6;
			// 
			// _ImageFormatBox
			// 
			this._ImageFormatBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ImageFormatBox.FormattingEnabled = true;
			this._ImageFormatBox.Items.AddRange(new object[] {
            "PNG",
            "JPEG",
            "黑白TIFF"});
			this._ImageFormatBox.Location = new System.Drawing.Point(101, 71);
			this._ImageFormatBox.Name = "_ImageFormatBox";
			this._ImageFormatBox.Size = new System.Drawing.Size(78, 20);
			this._ImageFormatBox.TabIndex = 4;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this._FileNameMaskBox);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this._AutoOutputDirBox);
			this.tabPage2.Controls.Add(this.label5);
			this.tabPage2.Controls.Add(this._FileMaskPreviewBox);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(577, 203);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "文件命名";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// _AutoOutputDirBox
			// 
			this._AutoOutputDirBox.AutoSize = true;
			this._AutoOutputDirBox.Location = new System.Drawing.Point(344, 8);
			this._AutoOutputDirBox.Name = "_AutoOutputDirBox";
			this._AutoOutputDirBox.Size = new System.Drawing.Size(156, 16);
			this._AutoOutputDirBox.TabIndex = 5;
			this._AutoOutputDirBox.Text = "自动指定输出图片的位置";
			this._AutoOutputDirBox.UseVisualStyleBackColor = true;
			// 
			// _BrowseTargetPdfButton
			// 
			this._BrowseTargetPdfButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._BrowseTargetPdfButton.Image = global::PDFPatcher.Properties.Resources.ImageFolder;
			this._BrowseTargetPdfButton.Location = new System.Drawing.Point(522, 34);
			this._BrowseTargetPdfButton.Name = "_BrowseTargetPdfButton";
			this._BrowseTargetPdfButton.Size = new System.Drawing.Size(75, 23);
			this._BrowseTargetPdfButton.TabIndex = 3;
			this._BrowseTargetPdfButton.Text = "浏览...";
			this._BrowseTargetPdfButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._BrowseTargetPdfButton.UseVisualStyleBackColor = true;
			this._BrowseTargetPdfButton.Click += new System.EventHandler(this._BrowseTargetPdfButton_Click);
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
			this._TargetBox.Size = new System.Drawing.Size(404, 20);
			this._TargetBox.TabIndex = 2;
			// 
			// _SourceFileControl
			// 
			this._SourceFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SourceFileControl.Location = new System.Drawing.Point(12, 3);
			this._SourceFileControl.Name = "_SourceFileControl";
			this._SourceFileControl.Size = new System.Drawing.Size(588, 29);
			this._SourceFileControl.TabIndex = 0;
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
			this._ExtractButton.Location = new System.Drawing.Point(474, 63);
			this._ExtractButton.Name = "_ExtractButton";
			this._ExtractButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ExtractButton.ShowFocusBorder = true;
			this._ExtractButton.Size = new System.Drawing.Size(123, 29);
			this._ExtractButton.TabIndex = 6;
			this._ExtractButton.Text = " 转换图片(&T)";
			this._ExtractButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExtractButton.Click += new System.EventHandler(this._ExtractButton_Click);
			// 
			// _RenderToPdfBox
			// 
			this._RenderToPdfBox.AutoSize = true;
			this._RenderToPdfBox.Location = new System.Drawing.Point(19, 70);
			this._RenderToPdfBox.Name = "_RenderToPdfBox";
			this._RenderToPdfBox.Size = new System.Drawing.Size(162, 16);
			this._RenderToPdfBox.TabIndex = 4;
			this._RenderToPdfBox.Text = "合并输出到图片 PDF 文件";
			this._RenderToPdfBox.UseVisualStyleBackColor = true;
			// 
			// RenderImageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._RenderToPdfBox);
			this.Controls.Add(this._ExtractButton);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._TargetBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._SourceFileControl);
			this.Controls.Add(this._BrowseTargetPdfButton);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "RenderImageControl";
			this.Size = new System.Drawing.Size(612, 333);
			this.Load += new System.EventHandler(this.Control_Show);
			this.VisibleChanged += new System.EventHandler(this.Control_Show);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._ExtractPageImageWidthBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._ExtractPageRatioBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
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
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox _InvertColorBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.CheckBox _AutoOutputDirBox;
		private System.Windows.Forms.NumericUpDown _ExtractPageImageWidthBox;
		private System.Windows.Forms.RadioButton _ColorSpaceGrayBox;
		private System.Windows.Forms.RadioButton _ColorSpaceRgbBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox _ImageFormatBox;
		private System.Windows.Forms.CheckBox _VerticalFlipImageBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox _RotationBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox _JpegQualityBox;
		private System.Windows.Forms.CheckBox _HorizontalFlipImageBox;
		private System.Windows.Forms.CheckBox _HideAnnotationsBox;
		private System.Windows.Forms.CheckBox _QuantizeBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown _ExtractPageRatioBox;
		private System.Windows.Forms.RadioButton _SpecificRatioBox;
		private System.Windows.Forms.RadioButton _SpecificWidthBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox3;
		private EnhancedGlassButton.GlassButton _ExtractButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox _ResolutionBox;
		private System.Windows.Forms.CheckBox _RenderToPdfBox;
	}
}
