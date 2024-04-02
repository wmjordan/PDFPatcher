namespace PDFPatcher.Functions
{
	partial class MergerOptionForm
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
			this._MainTab = new System.Windows.Forms.TabControl();
			this._FilePage = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._DeduplicateBox = new System.Windows.Forms.CheckBox();
			this._KeepSourcePdfBookmarkBox = new System.Windows.Forms.CheckBox();
			this._RemoveOrphanBoomarksBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._SubFolderWithFilesBox = new System.Windows.Forms.RadioButton();
			this._ExcludeSubFoldersBox = new System.Windows.Forms.RadioButton();
			this.label9 = new System.Windows.Forms.Label();
			this._AutoBookmarkTitleBox = new System.Windows.Forms.CheckBox();
			this._CajSortBox = new System.Windows.Forms.CheckBox();
			this._IgnoreLeadingNumbersBox = new System.Windows.Forms.CheckBox();
			this._NumericAwareSortBox = new System.Windows.Forms.CheckBox();
			this._SubFoldersBeforeFilesBox = new System.Windows.Forms.RadioButton();
			this._LayoutPage = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._DpiYBox = new System.Windows.Forms.NumericUpDown();
			this._DpiXBox = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this._RecompressImageBox = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this._AutoMaskBWImageBox = new System.Windows.Forms.CheckBox();
			this._ImageGroupBox = new System.Windows.Forms.GroupBox();
			this._AutoScaleDownBox = new System.Windows.Forms.CheckBox();
			this._AutoScaleUpBox = new System.Windows.Forms.CheckBox();
			this._LayoutGroupBox = new System.Windows.Forms.GroupBox();
			this._RotationBox = new System.Windows.Forms.ComboBox();
			this._SourceOrientationBox = new System.Windows.Forms.ComboBox();
			this._UnifyOrientationBox = new System.Windows.Forms.CheckBox();
			this._HeightBox = new System.Windows.Forms.NumericUpDown();
			this._WidthBox = new System.Windows.Forms.NumericUpDown();
			this._ImageVAlignBox = new System.Windows.Forms.ComboBox();
			this._ImageHAlignBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this._PageSizeBox = new System.Windows.Forms.ComboBox();
			this._AutoRotateBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._MarginGroupBox = new System.Windows.Forms.GroupBox();
			this._SyncMarginsBox = new System.Windows.Forms.CheckBox();
			this._RightMarginBox = new System.Windows.Forms.NumericUpDown();
			this._LeftMarginBox = new System.Windows.Forms.NumericUpDown();
			this._BottomMarginBox = new System.Windows.Forms.NumericUpDown();
			this._TopMarginBox = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._PdfGroupBox = new System.Windows.Forms.GroupBox();
			this._ResizePdfPagesBox = new System.Windows.Forms.RadioButton();
			this._ScalePdfPagesBox = new System.Windows.Forms.RadioButton();
			this._ViewerSettingsPage = new System.Windows.Forms.TabPage();
			this._ViewerSettingsEditor = new PDFPatcher.Functions.ViewerPreferenceEditor();
			this._DocumentInfoPage = new System.Windows.Forms.TabPage();
			this._FullCompressionBox = new System.Windows.Forms.CheckBox();
			this._DocumentInfoEditor = new PDFPatcher.Functions.DocumentInfoEditor();
			this._PageLabelsPage = new System.Windows.Forms.TabPage();
			this._PageLabelEditor = new PDFPatcher.Functions.PageLabelEditor();
			this._ExtraEmptyPageBox = new System.Windows.Forms.CheckBox();
			this._MainTab.SuspendLayout();
			this._FilePage.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this._LayoutPage.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._DpiYBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._DpiXBox)).BeginInit();
			this._ImageGroupBox.SuspendLayout();
			this._LayoutGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).BeginInit();
			this._MarginGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).BeginInit();
			this._PdfGroupBox.SuspendLayout();
			this._ViewerSettingsPage.SuspendLayout();
			this._DocumentInfoPage.SuspendLayout();
			this._PageLabelsPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// _MainTab
			// 
			this._MainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._MainTab.Controls.Add(this._FilePage);
			this._MainTab.Controls.Add(this._LayoutPage);
			this._MainTab.Controls.Add(this._ViewerSettingsPage);
			this._MainTab.Controls.Add(this._DocumentInfoPage);
			this._MainTab.Controls.Add(this._PageLabelsPage);
			this._MainTab.Location = new System.Drawing.Point(12, 12);
			this._MainTab.Name = "_MainTab";
			this._MainTab.SelectedIndex = 0;
			this._MainTab.Size = new System.Drawing.Size(448, 319);
			this._MainTab.TabIndex = 0;
			// 
			// _FilePage
			// 
			this._FilePage.Controls.Add(this.groupBox3);
			this._FilePage.Controls.Add(this.groupBox1);
			this._FilePage.Location = new System.Drawing.Point(4, 22);
			this._FilePage.Name = "_FilePage";
			this._FilePage.Padding = new System.Windows.Forms.Padding(3);
			this._FilePage.Size = new System.Drawing.Size(440, 293);
			this._FilePage.TabIndex = 1;
			this._FilePage.Text = "文件";
			this._FilePage.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this._ExtraEmptyPageBox);
			this.groupBox3.Controls.Add(this._DeduplicateBox);
			this.groupBox3.Controls.Add(this._KeepSourcePdfBookmarkBox);
			this.groupBox3.Controls.Add(this._RemoveOrphanBoomarksBox);
			this.groupBox3.Location = new System.Drawing.Point(6, 141);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox3.Size = new System.Drawing.Size(431, 133);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "合并文件行为";
			// 
			// _DeduplicateBox
			// 
			this._DeduplicateBox.AutoSize = true;
			this._DeduplicateBox.Location = new System.Drawing.Point(10, 73);
			this._DeduplicateBox.Margin = new System.Windows.Forms.Padding(2);
			this._DeduplicateBox.Name = "_DeduplicateBox";
			this._DeduplicateBox.Size = new System.Drawing.Size(258, 16);
			this._DeduplicateBox.TabIndex = 2;
			this._DeduplicateBox.Text = "尝试合并 PDF 文档重复数据以缩小结果文件";
			this._DeduplicateBox.UseVisualStyleBackColor = true;
			// 
			// _KeepSourcePdfBookmarkBox
			// 
			this._KeepSourcePdfBookmarkBox.AutoSize = true;
			this._KeepSourcePdfBookmarkBox.Location = new System.Drawing.Point(10, 19);
			this._KeepSourcePdfBookmarkBox.Name = "_KeepSourcePdfBookmarkBox";
			this._KeepSourcePdfBookmarkBox.Size = new System.Drawing.Size(150, 16);
			this._KeepSourcePdfBookmarkBox.TabIndex = 0;
			this._KeepSourcePdfBookmarkBox.Text = "保留源 PDF 文档的书签";
			this._KeepSourcePdfBookmarkBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveOrphanBoomarksBox
			// 
			this._RemoveOrphanBoomarksBox.AutoSize = true;
			this._RemoveOrphanBoomarksBox.Location = new System.Drawing.Point(33, 41);
			this._RemoveOrphanBoomarksBox.Name = "_RemoveOrphanBoomarksBox";
			this._RemoveOrphanBoomarksBox.Size = new System.Drawing.Size(168, 16);
			this._RemoveOrphanBoomarksBox.TabIndex = 1;
			this._RemoveOrphanBoomarksBox.Text = "删除连接到无效页面的书签";
			this._RemoveOrphanBoomarksBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._SubFolderWithFilesBox);
			this.groupBox1.Controls.Add(this._ExcludeSubFoldersBox);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this._AutoBookmarkTitleBox);
			this.groupBox1.Controls.Add(this._CajSortBox);
			this.groupBox1.Controls.Add(this._IgnoreLeadingNumbersBox);
			this.groupBox1.Controls.Add(this._NumericAwareSortBox);
			this.groupBox1.Controls.Add(this._SubFoldersBeforeFilesBox);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(431, 131);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "向文件列表添加项目时的行为";
			// 
			// _SubFolderWithFilesBox
			// 
			this._SubFolderWithFilesBox.AutoSize = true;
			this._SubFolderWithFilesBox.Location = new System.Drawing.Point(177, 63);
			this._SubFolderWithFilesBox.Name = "_SubFolderWithFilesBox";
			this._SubFolderWithFilesBox.Size = new System.Drawing.Size(107, 16);
			this._SubFolderWithFilesBox.TabIndex = 4;
			this._SubFolderWithFilesBox.TabStop = true;
			this._SubFolderWithFilesBox.Text = "和文件一起排序";
			this._SubFolderWithFilesBox.UseVisualStyleBackColor = true;
			// 
			// _ExcludeSubFoldersBox
			// 
			this._ExcludeSubFoldersBox.AutoSize = true;
			this._ExcludeSubFoldersBox.Location = new System.Drawing.Point(290, 63);
			this._ExcludeSubFoldersBox.Name = "_ExcludeSubFoldersBox";
			this._ExcludeSubFoldersBox.Size = new System.Drawing.Size(71, 16);
			this._ExcludeSubFoldersBox.TabIndex = 5;
			this._ExcludeSubFoldersBox.TabStop = true;
			this._ExcludeSubFoldersBox.Text = "不要导入";
			this._ExcludeSubFoldersBox.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 65);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 12);
			this.label9.TabIndex = 2;
			this.label9.Text = "子文件夹：";
			// 
			// _AutoBookmarkTitleBox
			// 
			this._AutoBookmarkTitleBox.AutoSize = true;
			this._AutoBookmarkTitleBox.Location = new System.Drawing.Point(10, 19);
			this._AutoBookmarkTitleBox.Name = "_AutoBookmarkTitleBox";
			this._AutoBookmarkTitleBox.Size = new System.Drawing.Size(168, 16);
			this._AutoBookmarkTitleBox.TabIndex = 0;
			this._AutoBookmarkTitleBox.Text = "添加项时自动生成书签文本";
			this._AutoBookmarkTitleBox.UseVisualStyleBackColor = true;
			// 
			// _CajSortBox
			// 
			this._CajSortBox.AutoSize = true;
			this._CajSortBox.Location = new System.Drawing.Point(10, 107);
			this._CajSortBox.Name = "_CajSortBox";
			this._CajSortBox.Size = new System.Drawing.Size(144, 16);
			this._CajSortBox.TabIndex = 7;
			this._CajSortBox.Text = "使用超星命名规则排序";
			this._CajSortBox.UseVisualStyleBackColor = true;
			// 
			// _IgnoreLeadingNumbersBox
			// 
			this._IgnoreLeadingNumbersBox.AutoSize = true;
			this._IgnoreLeadingNumbersBox.Location = new System.Drawing.Point(33, 41);
			this._IgnoreLeadingNumbersBox.Name = "_IgnoreLeadingNumbersBox";
			this._IgnoreLeadingNumbersBox.Size = new System.Drawing.Size(156, 16);
			this._IgnoreLeadingNumbersBox.TabIndex = 1;
			this._IgnoreLeadingNumbersBox.Text = "删除书签文本的前导数字";
			this._IgnoreLeadingNumbersBox.UseVisualStyleBackColor = true;
			// 
			// _NumericAwareSortBox
			// 
			this._NumericAwareSortBox.AutoSize = true;
			this._NumericAwareSortBox.Location = new System.Drawing.Point(10, 85);
			this._NumericAwareSortBox.Name = "_NumericAwareSortBox";
			this._NumericAwareSortBox.Size = new System.Drawing.Size(180, 16);
			this._NumericAwareSortBox.TabIndex = 6;
			this._NumericAwareSortBox.Text = "文件名分别按数值和文本排序";
			this._NumericAwareSortBox.UseVisualStyleBackColor = true;
			// 
			// _SubFoldersBeforeFilesBox
			// 
			this._SubFoldersBeforeFilesBox.AutoSize = true;
			this._SubFoldersBeforeFilesBox.Location = new System.Drawing.Point(76, 63);
			this._SubFoldersBeforeFilesBox.Name = "_SubFoldersBeforeFilesBox";
			this._SubFoldersBeforeFilesBox.Size = new System.Drawing.Size(95, 16);
			this._SubFoldersBeforeFilesBox.TabIndex = 3;
			this._SubFoldersBeforeFilesBox.Text = "排在文件前面";
			this._SubFoldersBeforeFilesBox.UseVisualStyleBackColor = true;
			// 
			// _LayoutPage
			// 
			this._LayoutPage.Controls.Add(this.groupBox2);
			this._LayoutPage.Controls.Add(this._ImageGroupBox);
			this._LayoutPage.Controls.Add(this._LayoutGroupBox);
			this._LayoutPage.Controls.Add(this._MarginGroupBox);
			this._LayoutPage.Controls.Add(this._PdfGroupBox);
			this._LayoutPage.Location = new System.Drawing.Point(4, 22);
			this._LayoutPage.Name = "_LayoutPage";
			this._LayoutPage.Padding = new System.Windows.Forms.Padding(3);
			this._LayoutPage.Size = new System.Drawing.Size(440, 293);
			this._LayoutPage.TabIndex = 0;
			this._LayoutPage.Text = "页面布局";
			this._LayoutPage.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._DpiYBox);
			this.groupBox2.Controls.Add(this._DpiXBox);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this._RecompressImageBox);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this._AutoMaskBWImageBox);
			this.groupBox2.Location = new System.Drawing.Point(237, 139);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(195, 138);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "图片";
			// 
			// _DpiYBox
			// 
			this._DpiYBox.Location = new System.Drawing.Point(55, 111);
			this._DpiYBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._DpiYBox.Name = "_DpiYBox";
			this._DpiYBox.Size = new System.Drawing.Size(61, 21);
			this._DpiYBox.TabIndex = 6;
			this._DpiYBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _DpiXBox
			// 
			this._DpiXBox.Location = new System.Drawing.Point(55, 84);
			this._DpiXBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._DpiXBox.Name = "_DpiXBox";
			this._DpiXBox.Size = new System.Drawing.Size(61, 21);
			this._DpiXBox.TabIndex = 4;
			this._DpiXBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 66);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(179, 12);
			this.label10.TabIndex = 2;
			this.label10.Text = "指定导入分辨率（0：保持原图）";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(8, 113);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(41, 12);
			this.label11.TabIndex = 5;
			this.label11.Text = "垂直：";
			// 
			// _RecompressImageBox
			// 
			this._RecompressImageBox.AutoSize = true;
			this._RecompressImageBox.Location = new System.Drawing.Point(7, 43);
			this._RecompressImageBox.Name = "_RecompressImageBox";
			this._RecompressImageBox.Size = new System.Drawing.Size(120, 16);
			this._RecompressImageBox.TabIndex = 1;
			this._RecompressImageBox.Text = "优化压缩黑白图片";
			this._RecompressImageBox.UseVisualStyleBackColor = true;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(8, 87);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(41, 12);
			this.label12.TabIndex = 3;
			this.label12.Text = "水平：";
			// 
			// _AutoMaskBWImageBox
			// 
			this._AutoMaskBWImageBox.AutoSize = true;
			this._AutoMaskBWImageBox.Location = new System.Drawing.Point(7, 21);
			this._AutoMaskBWImageBox.Name = "_AutoMaskBWImageBox";
			this._AutoMaskBWImageBox.Size = new System.Drawing.Size(120, 16);
			this._AutoMaskBWImageBox.TabIndex = 0;
			this._AutoMaskBWImageBox.Text = "黑白图片设为透明";
			this._AutoMaskBWImageBox.UseVisualStyleBackColor = true;
			// 
			// _ImageGroupBox
			// 
			this._ImageGroupBox.Controls.Add(this._AutoScaleDownBox);
			this._ImageGroupBox.Controls.Add(this._AutoScaleUpBox);
			this._ImageGroupBox.Location = new System.Drawing.Point(237, 6);
			this._ImageGroupBox.Name = "_ImageGroupBox";
			this._ImageGroupBox.Size = new System.Drawing.Size(195, 48);
			this._ImageGroupBox.TabIndex = 2;
			this._ImageGroupBox.TabStop = false;
			this._ImageGroupBox.Text = "缩放原始内容适应页面";
			// 
			// _AutoScaleDownBox
			// 
			this._AutoScaleDownBox.AutoSize = true;
			this._AutoScaleDownBox.Checked = true;
			this._AutoScaleDownBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._AutoScaleDownBox.Location = new System.Drawing.Point(7, 20);
			this._AutoScaleDownBox.Name = "_AutoScaleDownBox";
			this._AutoScaleDownBox.Size = new System.Drawing.Size(72, 16);
			this._AutoScaleDownBox.TabIndex = 0;
			this._AutoScaleDownBox.Text = "无损缩小";
			this._AutoScaleDownBox.UseVisualStyleBackColor = true;
			// 
			// _AutoScaleUpBox
			// 
			this._AutoScaleUpBox.AutoSize = true;
			this._AutoScaleUpBox.Location = new System.Drawing.Point(94, 20);
			this._AutoScaleUpBox.Name = "_AutoScaleUpBox";
			this._AutoScaleUpBox.Size = new System.Drawing.Size(72, 16);
			this._AutoScaleUpBox.TabIndex = 1;
			this._AutoScaleUpBox.Text = "无损放大";
			this._AutoScaleUpBox.UseVisualStyleBackColor = true;
			// 
			// _LayoutGroupBox
			// 
			this._LayoutGroupBox.Controls.Add(this._RotationBox);
			this._LayoutGroupBox.Controls.Add(this._SourceOrientationBox);
			this._LayoutGroupBox.Controls.Add(this._UnifyOrientationBox);
			this._LayoutGroupBox.Controls.Add(this._HeightBox);
			this._LayoutGroupBox.Controls.Add(this._WidthBox);
			this._LayoutGroupBox.Controls.Add(this._ImageVAlignBox);
			this._LayoutGroupBox.Controls.Add(this._ImageHAlignBox);
			this._LayoutGroupBox.Controls.Add(this.label2);
			this._LayoutGroupBox.Controls.Add(this.label8);
			this._LayoutGroupBox.Controls.Add(this._PageSizeBox);
			this._LayoutGroupBox.Controls.Add(this._AutoRotateBox);
			this._LayoutGroupBox.Controls.Add(this.label5);
			this._LayoutGroupBox.Controls.Add(this.label4);
			this._LayoutGroupBox.Location = new System.Drawing.Point(6, 6);
			this._LayoutGroupBox.Name = "_LayoutGroupBox";
			this._LayoutGroupBox.Size = new System.Drawing.Size(225, 171);
			this._LayoutGroupBox.TabIndex = 0;
			this._LayoutGroupBox.TabStop = false;
			this._LayoutGroupBox.Text = "默认页面布局及尺寸（单位：厘米）";
			// 
			// _RotationBox
			// 
			this._RotationBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._RotationBox.FormattingEnabled = true;
			this._RotationBox.Items.AddRange(new object[] {
            "顺时针旋转90度",
            "逆时针旋转90度"});
			this._RotationBox.Location = new System.Drawing.Point(103, 140);
			this._RotationBox.Name = "_RotationBox";
			this._RotationBox.Size = new System.Drawing.Size(112, 20);
			this._RotationBox.TabIndex = 12;
			// 
			// _SourceOrientationBox
			// 
			this._SourceOrientationBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._SourceOrientationBox.FormattingEnabled = true;
			this._SourceOrientationBox.Items.AddRange(new object[] {
            "横向页面",
            "纵向页面"});
			this._SourceOrientationBox.Location = new System.Drawing.Point(20, 140);
			this._SourceOrientationBox.Name = "_SourceOrientationBox";
			this._SourceOrientationBox.Size = new System.Drawing.Size(77, 20);
			this._SourceOrientationBox.TabIndex = 11;
			// 
			// _UnifyOrientationBox
			// 
			this._UnifyOrientationBox.AutoSize = true;
			this._UnifyOrientationBox.Location = new System.Drawing.Point(8, 118);
			this._UnifyOrientationBox.Name = "_UnifyOrientationBox";
			this._UnifyOrientationBox.Size = new System.Drawing.Size(156, 16);
			this._UnifyOrientationBox.TabIndex = 10;
			this._UnifyOrientationBox.Text = "修改所有页面的纵横方向";
			this._UnifyOrientationBox.UseVisualStyleBackColor = true;
			// 
			// _HeightBox
			// 
			this._HeightBox.DecimalPlaces = 2;
			this._HeightBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._HeightBox.Location = new System.Drawing.Point(154, 45);
			this._HeightBox.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this._HeightBox.Name = "_HeightBox";
			this._HeightBox.Size = new System.Drawing.Size(61, 21);
			this._HeightBox.TabIndex = 5;
			this._HeightBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _WidthBox
			// 
			this._WidthBox.DecimalPlaces = 2;
			this._WidthBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._WidthBox.Location = new System.Drawing.Point(53, 44);
			this._WidthBox.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this._WidthBox.Name = "_WidthBox";
			this._WidthBox.Size = new System.Drawing.Size(61, 21);
			this._WidthBox.TabIndex = 3;
			this._WidthBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _ImageVAlignBox
			// 
			this._ImageVAlignBox.DisplayMember = "Key";
			this._ImageVAlignBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ImageVAlignBox.Location = new System.Drawing.Point(139, 93);
			this._ImageVAlignBox.Name = "_ImageVAlignBox";
			this._ImageVAlignBox.Size = new System.Drawing.Size(76, 20);
			this._ImageVAlignBox.TabIndex = 9;
			this._ImageVAlignBox.ValueMember = "Value";
			// 
			// _ImageHAlignBox
			// 
			this._ImageHAlignBox.DisplayMember = "Key";
			this._ImageHAlignBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ImageHAlignBox.Location = new System.Drawing.Point(53, 93);
			this._ImageHAlignBox.Name = "_ImageHAlignBox";
			this._ImageHAlignBox.Size = new System.Drawing.Size(76, 20);
			this._ImageHAlignBox.TabIndex = 8;
			this._ImageHAlignBox.ValueMember = "Value";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "尺寸：";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(41, 12);
			this.label8.TabIndex = 7;
			this.label8.Text = "位置：";
			// 
			// _PageSizeBox
			// 
			this._PageSizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._PageSizeBox.Location = new System.Drawing.Point(53, 18);
			this._PageSizeBox.Name = "_PageSizeBox";
			this._PageSizeBox.Size = new System.Drawing.Size(162, 20);
			this._PageSizeBox.TabIndex = 1;
			this._PageSizeBox.SelectedIndexChanged += new System.EventHandler(this._PageSizeBox_SelectedIndexChanged);
			// 
			// _AutoRotateBox
			// 
			this._AutoRotateBox.AutoSize = true;
			this._AutoRotateBox.Checked = true;
			this._AutoRotateBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._AutoRotateBox.Location = new System.Drawing.Point(20, 72);
			this._AutoRotateBox.Name = "_AutoRotateBox";
			this._AutoRotateBox.Size = new System.Drawing.Size(192, 16);
			this._AutoRotateBox.TabIndex = 6;
			this._AutoRotateBox.Text = "旋转页面适应原始内容纵横方向";
			this._AutoRotateBox.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(120, 47);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "高：";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 47);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "宽：";
			// 
			// _MarginGroupBox
			// 
			this._MarginGroupBox.Controls.Add(this._SyncMarginsBox);
			this._MarginGroupBox.Controls.Add(this._RightMarginBox);
			this._MarginGroupBox.Controls.Add(this._LeftMarginBox);
			this._MarginGroupBox.Controls.Add(this._BottomMarginBox);
			this._MarginGroupBox.Controls.Add(this._TopMarginBox);
			this._MarginGroupBox.Controls.Add(this.label7);
			this._MarginGroupBox.Controls.Add(this.label3);
			this._MarginGroupBox.Controls.Add(this.label6);
			this._MarginGroupBox.Controls.Add(this.label1);
			this._MarginGroupBox.Location = new System.Drawing.Point(6, 183);
			this._MarginGroupBox.Name = "_MarginGroupBox";
			this._MarginGroupBox.Size = new System.Drawing.Size(225, 94);
			this._MarginGroupBox.TabIndex = 1;
			this._MarginGroupBox.TabStop = false;
			this._MarginGroupBox.Text = "页边留白（单位：厘米）";
			// 
			// _SyncMarginsBox
			// 
			this._SyncMarginsBox.AutoSize = true;
			this._SyncMarginsBox.Checked = true;
			this._SyncMarginsBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._SyncMarginsBox.Location = new System.Drawing.Point(53, 74);
			this._SyncMarginsBox.Name = "_SyncMarginsBox";
			this._SyncMarginsBox.Size = new System.Drawing.Size(120, 16);
			this._SyncMarginsBox.TabIndex = 9;
			this._SyncMarginsBox.Text = "同步调整四边留白";
			this._SyncMarginsBox.UseVisualStyleBackColor = true;
			// 
			// _RightMarginBox
			// 
			this._RightMarginBox.DecimalPlaces = 2;
			this._RightMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._RightMarginBox.Location = new System.Drawing.Point(154, 47);
			this._RightMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._RightMarginBox.Name = "_RightMarginBox";
			this._RightMarginBox.Size = new System.Drawing.Size(61, 21);
			this._RightMarginBox.TabIndex = 8;
			this._RightMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._RightMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _LeftMarginBox
			// 
			this._LeftMarginBox.DecimalPlaces = 2;
			this._LeftMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._LeftMarginBox.Location = new System.Drawing.Point(53, 47);
			this._LeftMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._LeftMarginBox.Name = "_LeftMarginBox";
			this._LeftMarginBox.Size = new System.Drawing.Size(61, 21);
			this._LeftMarginBox.TabIndex = 6;
			this._LeftMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._LeftMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _BottomMarginBox
			// 
			this._BottomMarginBox.DecimalPlaces = 2;
			this._BottomMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._BottomMarginBox.Location = new System.Drawing.Point(154, 20);
			this._BottomMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._BottomMarginBox.Name = "_BottomMarginBox";
			this._BottomMarginBox.Size = new System.Drawing.Size(61, 21);
			this._BottomMarginBox.TabIndex = 4;
			this._BottomMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._BottomMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _TopMarginBox
			// 
			this._TopMarginBox.DecimalPlaces = 2;
			this._TopMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this._TopMarginBox.Location = new System.Drawing.Point(53, 20);
			this._TopMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._TopMarginBox.Name = "_TopMarginBox";
			this._TopMarginBox.Size = new System.Drawing.Size(61, 21);
			this._TopMarginBox.TabIndex = 1;
			this._TopMarginBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._TopMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(119, 50);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(29, 12);
			this.label7.TabIndex = 7;
			this.label7.Text = "右：";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(119, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "下：";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(18, 50);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29, 12);
			this.label6.TabIndex = 5;
			this.label6.Text = "左：";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "上：";
			// 
			// _PdfGroupBox
			// 
			this._PdfGroupBox.Controls.Add(this._ResizePdfPagesBox);
			this._PdfGroupBox.Controls.Add(this._ScalePdfPagesBox);
			this._PdfGroupBox.Location = new System.Drawing.Point(237, 60);
			this._PdfGroupBox.Name = "_PdfGroupBox";
			this._PdfGroupBox.Size = new System.Drawing.Size(195, 73);
			this._PdfGroupBox.TabIndex = 3;
			this._PdfGroupBox.TabStop = false;
			this._PdfGroupBox.Text = "源 PDF 页面尺寸";
			// 
			// _ResizePdfPagesBox
			// 
			this._ResizePdfPagesBox.AutoSize = true;
			this._ResizePdfPagesBox.Location = new System.Drawing.Point(7, 20);
			this._ResizePdfPagesBox.Name = "_ResizePdfPagesBox";
			this._ResizePdfPagesBox.Size = new System.Drawing.Size(107, 16);
			this._ResizePdfPagesBox.TabIndex = 1;
			this._ResizePdfPagesBox.Text = "调整为页面尺寸";
			this._ResizePdfPagesBox.UseVisualStyleBackColor = true;
			// 
			// _ScalePdfPagesBox
			// 
			this._ScalePdfPagesBox.AutoSize = true;
			this._ScalePdfPagesBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this._ScalePdfPagesBox.Location = new System.Drawing.Point(7, 42);
			this._ScalePdfPagesBox.Name = "_ScalePdfPagesBox";
			this._ScalePdfPagesBox.Size = new System.Drawing.Size(119, 16);
			this._ScalePdfPagesBox.TabIndex = 2;
			this._ScalePdfPagesBox.TabStop = true;
			this._ScalePdfPagesBox.Text = "缩放内容适应页面";
			this._ScalePdfPagesBox.UseVisualStyleBackColor = true;
			// 
			// _ViewerSettingsPage
			// 
			this._ViewerSettingsPage.Controls.Add(this._ViewerSettingsEditor);
			this._ViewerSettingsPage.Location = new System.Drawing.Point(4, 22);
			this._ViewerSettingsPage.Name = "_ViewerSettingsPage";
			this._ViewerSettingsPage.Padding = new System.Windows.Forms.Padding(3);
			this._ViewerSettingsPage.Size = new System.Drawing.Size(440, 293);
			this._ViewerSettingsPage.TabIndex = 2;
			this._ViewerSettingsPage.Text = "阅读方式";
			this._ViewerSettingsPage.UseVisualStyleBackColor = true;
			// 
			// _ViewerSettingsEditor
			// 
			this._ViewerSettingsEditor.Location = new System.Drawing.Point(0, 0);
			this._ViewerSettingsEditor.Margin = new System.Windows.Forms.Padding(4);
			this._ViewerSettingsEditor.Name = "_ViewerSettingsEditor";
			this._ViewerSettingsEditor.Size = new System.Drawing.Size(438, 279);
			this._ViewerSettingsEditor.TabIndex = 1;
			// 
			// _DocumentInfoPage
			// 
			this._DocumentInfoPage.Controls.Add(this._FullCompressionBox);
			this._DocumentInfoPage.Controls.Add(this._DocumentInfoEditor);
			this._DocumentInfoPage.Location = new System.Drawing.Point(4, 22);
			this._DocumentInfoPage.Name = "_DocumentInfoPage";
			this._DocumentInfoPage.Padding = new System.Windows.Forms.Padding(3);
			this._DocumentInfoPage.Size = new System.Drawing.Size(440, 293);
			this._DocumentInfoPage.TabIndex = 3;
			this._DocumentInfoPage.Text = "文档杂项";
			this._DocumentInfoPage.UseVisualStyleBackColor = true;
			// 
			// _FullCompressionBox
			// 
			this._FullCompressionBox.AutoSize = true;
			this._FullCompressionBox.Location = new System.Drawing.Point(15, 260);
			this._FullCompressionBox.Name = "_FullCompressionBox";
			this._FullCompressionBox.Size = new System.Drawing.Size(120, 16);
			this._FullCompressionBox.TabIndex = 2;
			this._FullCompressionBox.Text = "压缩索引表和书签";
			this._FullCompressionBox.UseVisualStyleBackColor = true;
			// 
			// _DocumentInfoEditor
			// 
			this._DocumentInfoEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._DocumentInfoEditor.Location = new System.Drawing.Point(0, 0);
			this._DocumentInfoEditor.Margin = new System.Windows.Forms.Padding(4);
			this._DocumentInfoEditor.Name = "_DocumentInfoEditor";
			this._DocumentInfoEditor.Size = new System.Drawing.Size(439, 294);
			this._DocumentInfoEditor.TabIndex = 1;
			// 
			// _PageLabelsPage
			// 
			this._PageLabelsPage.Controls.Add(this._PageLabelEditor);
			this._PageLabelsPage.Location = new System.Drawing.Point(4, 22);
			this._PageLabelsPage.Name = "_PageLabelsPage";
			this._PageLabelsPage.Padding = new System.Windows.Forms.Padding(3);
			this._PageLabelsPage.Size = new System.Drawing.Size(440, 293);
			this._PageLabelsPage.TabIndex = 4;
			this._PageLabelsPage.Text = "页码标签";
			this._PageLabelsPage.UseVisualStyleBackColor = true;
			// 
			// _PageLabelEditor
			// 
			this._PageLabelEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._PageLabelEditor.Labels = null;
			this._PageLabelEditor.Location = new System.Drawing.Point(-2, 0);
			this._PageLabelEditor.Margin = new System.Windows.Forms.Padding(4);
			this._PageLabelEditor.Name = "_PageLabelEditor";
			this._PageLabelEditor.Size = new System.Drawing.Size(439, 282);
			this._PageLabelEditor.TabIndex = 1;
			// 
			// _ExtraEmptyPageBox
			// 
			this._ExtraEmptyPageBox.AutoSize = true;
			this._ExtraEmptyPageBox.Location = new System.Drawing.Point(10, 93);
			this._ExtraEmptyPageBox.Margin = new System.Windows.Forms.Padding(2);
			this._ExtraEmptyPageBox.Name = "_ExtraEmptyPageBox";
			this._ExtraEmptyPageBox.Size = new System.Drawing.Size(222, 16);
			this._ExtraEmptyPageBox.TabIndex = 3;
			this._ExtraEmptyPageBox.Text = "在单数页的 PDF 文档后附加一空白页";
			this._ExtraEmptyPageBox.UseVisualStyleBackColor = true;
			// 
			// MergerOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(474, 343);
			this.Controls.Add(this._MainTab);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MergerOptionForm";
			this.ShowInTaskbar = false;
			this.Text = "合并 PDF 文档选项";
			this._MainTab.ResumeLayout(false);
			this._FilePage.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this._LayoutPage.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._DpiYBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._DpiXBox)).EndInit();
			this._ImageGroupBox.ResumeLayout(false);
			this._ImageGroupBox.PerformLayout();
			this._LayoutGroupBox.ResumeLayout(false);
			this._LayoutGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).EndInit();
			this._MarginGroupBox.ResumeLayout(false);
			this._MarginGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).EndInit();
			this._PdfGroupBox.ResumeLayout(false);
			this._PdfGroupBox.PerformLayout();
			this._ViewerSettingsPage.ResumeLayout(false);
			this._DocumentInfoPage.ResumeLayout(false);
			this._DocumentInfoPage.PerformLayout();
			this._PageLabelsPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl _MainTab;
		private System.Windows.Forms.TabPage _LayoutPage;
		private System.Windows.Forms.TabPage _FilePage;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox _AutoMaskBWImageBox;
		private System.Windows.Forms.GroupBox _ImageGroupBox;
		private System.Windows.Forms.CheckBox _AutoScaleDownBox;
		private System.Windows.Forms.CheckBox _AutoScaleUpBox;
		private System.Windows.Forms.GroupBox _LayoutGroupBox;
		private System.Windows.Forms.NumericUpDown _HeightBox;
		private System.Windows.Forms.NumericUpDown _WidthBox;
		private System.Windows.Forms.ComboBox _ImageVAlignBox;
		private System.Windows.Forms.ComboBox _ImageHAlignBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox _PageSizeBox;
		private System.Windows.Forms.CheckBox _AutoRotateBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox _MarginGroupBox;
		private System.Windows.Forms.CheckBox _SyncMarginsBox;
		private System.Windows.Forms.NumericUpDown _RightMarginBox;
		private System.Windows.Forms.NumericUpDown _LeftMarginBox;
		private System.Windows.Forms.NumericUpDown _BottomMarginBox;
		private System.Windows.Forms.NumericUpDown _TopMarginBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox _PdfGroupBox;
		private System.Windows.Forms.RadioButton _ResizePdfPagesBox;
		private System.Windows.Forms.RadioButton _ScalePdfPagesBox;
		private System.Windows.Forms.CheckBox _IgnoreLeadingNumbersBox;
		private System.Windows.Forms.TabPage _ViewerSettingsPage;
		private ViewerPreferenceEditor _ViewerSettingsEditor;
		private System.Windows.Forms.TabPage _DocumentInfoPage;
		private DocumentInfoEditor _DocumentInfoEditor;
		private System.Windows.Forms.TabPage _PageLabelsPage;
		private PageLabelEditor _PageLabelEditor;
		private System.Windows.Forms.CheckBox _RecompressImageBox;
		private System.Windows.Forms.CheckBox _FullCompressionBox;
		private System.Windows.Forms.CheckBox _AutoBookmarkTitleBox;
		private System.Windows.Forms.RadioButton _SubFoldersBeforeFilesBox;
		private System.Windows.Forms.CheckBox _NumericAwareSortBox;
		private System.Windows.Forms.CheckBox _RemoveOrphanBoomarksBox;
		private System.Windows.Forms.CheckBox _KeepSourcePdfBookmarkBox;
		private System.Windows.Forms.CheckBox _CajSortBox;
		private System.Windows.Forms.CheckBox _UnifyOrientationBox;
		private System.Windows.Forms.ComboBox _RotationBox;
		private System.Windows.Forms.ComboBox _SourceOrientationBox;
		private System.Windows.Forms.CheckBox _DeduplicateBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton _ExcludeSubFoldersBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.RadioButton _SubFolderWithFilesBox;
		private System.Windows.Forms.NumericUpDown _DpiYBox;
		private System.Windows.Forms.NumericUpDown _DpiXBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox _ExtraEmptyPageBox;
	}
}