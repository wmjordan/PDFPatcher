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
			this._PageSettingsPage = new System.Windows.Forms.TabPage();
			this._PageSettingsEditor = new PDFPatcher.Functions.PageSettingsEditor();
			this._MainTab = new System.Windows.Forms.TabControl();
			this._PageLayoutPage = new System.Windows.Forms.TabPage();
			this._MarginGroupBox = new System.Windows.Forms.GroupBox();
			this._MarginUnitBox = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this._SyncMarginsBox = new System.Windows.Forms.CheckBox();
			this._RightMarginBox = new System.Windows.Forms.NumericUpDown();
			this._LeftMarginBox = new System.Windows.Forms.NumericUpDown();
			this._BottomMarginBox = new System.Windows.Forms.NumericUpDown();
			this._TopMarginBox = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._LayoutGroupBox = new System.Windows.Forms.GroupBox();
			this._ImageHAlignBox = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this._ImageVAlignBox = new System.Windows.Forms.ComboBox();
			this._ScalePdfPagesBox = new System.Windows.Forms.RadioButton();
			this._ResizePdfPagesBox = new System.Windows.Forms.RadioButton();
			this._HeightBox = new System.Windows.Forms.NumericUpDown();
			this._WidthBox = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._PageSizeBox = new System.Windows.Forms.ComboBox();
			this._AutoRotateBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._ViewerSettingsPage = new System.Windows.Forms.TabPage();
			this._ViewerSettingsEditor = new PDFPatcher.Functions.ViewerPreferenceEditor();
			this._CleanerPage = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this._RemoveTrailingCommandCountBox = new System.Windows.Forms.NumericUpDown();
			this._RemoveLeadingCommandCountBox = new System.Windows.Forms.NumericUpDown();
			this.label10 = new System.Windows.Forms.Label();
			this._RemovePageFormsBox = new System.Windows.Forms.CheckBox();
			this._RemovePageThumbnailsBox = new System.Windows.Forms.CheckBox();
			this._RemovePageTextBlocksBox = new System.Windows.Forms.CheckBox();
			this._RemovePageLinksBox = new System.Windows.Forms.CheckBox();
			this._RemoveAnnotationsBox = new System.Windows.Forms.CheckBox();
			this._RemovePageAutoActionsBox = new System.Windows.Forms.CheckBox();
			this._RemovePageMetaDataBox = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._RemoveBookmarksBox = new System.Windows.Forms.CheckBox();
			this._FixContentBox = new System.Windows.Forms.CheckBox();
			this._RemoveXmlMetaDataBox = new System.Windows.Forms.CheckBox();
			this._RemoveDocAutoActionsBox = new System.Windows.Forms.CheckBox();
			this._RemoveUsageRightsBox = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._RecompressWithJbig2Box = new System.Windows.Forms.CheckBox();
			this._FullCompressionBox = new System.Windows.Forms.CheckBox();
			this._DocumentInfoPage = new System.Windows.Forms.TabPage();
			this._DocumentInfoEditor = new PDFPatcher.Functions.DocumentInfoEditor();
			this._PageLabelsPage = new System.Windows.Forms.TabPage();
			this._PageLabelEditor = new PDFPatcher.Functions.PageLabelEditor();
			this._FontSubstitutionsPage = new System.Windows.Forms.TabPage();
			this._FontSubstitutionsEditor = new PDFPatcher.Functions.FontSubstitutionsEditor();
			this._ConfigPage = new System.Windows.Forms.TabPage();
			this._ExportButton = new System.Windows.Forms.Button();
			this._ImportButton = new System.Windows.Forms.Button();
			this._ResetButton = new System.Windows.Forms.Button();
			this._PageSettingsPage.SuspendLayout();
			this._MainTab.SuspendLayout();
			this._PageLayoutPage.SuspendLayout();
			this._MarginGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).BeginInit();
			this._LayoutGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).BeginInit();
			this._ViewerSettingsPage.SuspendLayout();
			this._CleanerPage.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._RemoveTrailingCommandCountBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._RemoveLeadingCommandCountBox)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this._DocumentInfoPage.SuspendLayout();
			this._PageLabelsPage.SuspendLayout();
			this._FontSubstitutionsPage.SuspendLayout();
			this._ConfigPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// _PageSettingsPage
			// 
			this._PageSettingsPage.Controls.Add(this._PageSettingsEditor);
			this._PageSettingsPage.Location = new System.Drawing.Point(4, 25);
			this._PageSettingsPage.Margin = new System.Windows.Forms.Padding(4);
			this._PageSettingsPage.Name = "_PageSettingsPage";
			this._PageSettingsPage.Padding = new System.Windows.Forms.Padding(4);
			this._PageSettingsPage.Size = new System.Drawing.Size(595, 372);
			this._PageSettingsPage.TabIndex = 6;
			this._PageSettingsPage.Text = "页面设置";
			this._PageSettingsPage.UseVisualStyleBackColor = true;
			// 
			// _PageSettingsEditor
			// 
			this._PageSettingsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._PageSettingsEditor.Location = new System.Drawing.Point(4, 4);
			this._PageSettingsEditor.Margin = new System.Windows.Forms.Padding(5);
			this._PageSettingsEditor.Name = "_PageSettingsEditor";
			this._PageSettingsEditor.Settings = null;
			this._PageSettingsEditor.Size = new System.Drawing.Size(587, 364);
			this._PageSettingsEditor.TabIndex = 0;
			// 
			// _MainTab
			// 
			this._MainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._MainTab.Controls.Add(this._PageLayoutPage);
			this._MainTab.Controls.Add(this._ViewerSettingsPage);
			this._MainTab.Controls.Add(this._CleanerPage);
			this._MainTab.Controls.Add(this._DocumentInfoPage);
			this._MainTab.Controls.Add(this._PageLabelsPage);
			this._MainTab.Controls.Add(this._PageSettingsPage);
			this._MainTab.Controls.Add(this._FontSubstitutionsPage);
			this._MainTab.Controls.Add(this._ConfigPage);
			this._MainTab.Location = new System.Drawing.Point(16, 15);
			this._MainTab.Margin = new System.Windows.Forms.Padding(4);
			this._MainTab.Name = "_MainTab";
			this._MainTab.SelectedIndex = 0;
			this._MainTab.Size = new System.Drawing.Size(603, 401);
			this._MainTab.TabIndex = 1;
			// 
			// _PageLayoutPage
			// 
			this._PageLayoutPage.Controls.Add(this._MarginGroupBox);
			this._PageLayoutPage.Controls.Add(this._LayoutGroupBox);
			this._PageLayoutPage.Location = new System.Drawing.Point(4, 25);
			this._PageLayoutPage.Margin = new System.Windows.Forms.Padding(4);
			this._PageLayoutPage.Name = "_PageLayoutPage";
			this._PageLayoutPage.Padding = new System.Windows.Forms.Padding(4);
			this._PageLayoutPage.Size = new System.Drawing.Size(595, 372);
			this._PageLayoutPage.TabIndex = 8;
			this._PageLayoutPage.Text = "页面尺寸";
			this._PageLayoutPage.UseVisualStyleBackColor = true;
			// 
			// _MarginGroupBox
			// 
			this._MarginGroupBox.Controls.Add(this._MarginUnitBox);
			this._MarginGroupBox.Controls.Add(this.label13);
			this._MarginGroupBox.Controls.Add(this._SyncMarginsBox);
			this._MarginGroupBox.Controls.Add(this._RightMarginBox);
			this._MarginGroupBox.Controls.Add(this._LeftMarginBox);
			this._MarginGroupBox.Controls.Add(this._BottomMarginBox);
			this._MarginGroupBox.Controls.Add(this._TopMarginBox);
			this._MarginGroupBox.Controls.Add(this.label7);
			this._MarginGroupBox.Controls.Add(this.label3);
			this._MarginGroupBox.Controls.Add(this.label6);
			this._MarginGroupBox.Controls.Add(this.label1);
			this._MarginGroupBox.Location = new System.Drawing.Point(8, 171);
			this._MarginGroupBox.Margin = new System.Windows.Forms.Padding(4);
			this._MarginGroupBox.Name = "_MarginGroupBox";
			this._MarginGroupBox.Padding = new System.Windows.Forms.Padding(4);
			this._MarginGroupBox.Size = new System.Drawing.Size(576, 90);
			this._MarginGroupBox.TabIndex = 7;
			this._MarginGroupBox.TabStop = false;
			this._MarginGroupBox.Text = "调整页边留白";
			// 
			// _MarginUnitBox
			// 
			this._MarginUnitBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._MarginUnitBox.FormattingEnabled = true;
			this._MarginUnitBox.Items.AddRange(new object[] {
            "厘米",
            "相对原页面尺寸比例"});
			this._MarginUnitBox.Location = new System.Drawing.Point(71, 57);
			this._MarginUnitBox.Name = "_MarginUnitBox";
			this._MarginUnitBox.Size = new System.Drawing.Size(215, 23);
			this._MarginUnitBox.TabIndex = 12;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(9, 60);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(52, 15);
			this.label13.TabIndex = 11;
			this.label13.Text = "单位：";
			// 
			// _SyncMarginsBox
			// 
			this._SyncMarginsBox.AutoSize = true;
			this._SyncMarginsBox.Location = new System.Drawing.Point(303, 59);
			this._SyncMarginsBox.Margin = new System.Windows.Forms.Padding(4);
			this._SyncMarginsBox.Name = "_SyncMarginsBox";
			this._SyncMarginsBox.Size = new System.Drawing.Size(149, 19);
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
            65536});
			this._RightMarginBox.Location = new System.Drawing.Point(481, 25);
			this._RightMarginBox.Margin = new System.Windows.Forms.Padding(4);
			this._RightMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._RightMarginBox.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
			this._RightMarginBox.Name = "_RightMarginBox";
			this._RightMarginBox.Size = new System.Drawing.Size(81, 25);
			this._RightMarginBox.TabIndex = 8;
			this._RightMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _LeftMarginBox
			// 
			this._LeftMarginBox.DecimalPlaces = 2;
			this._LeftMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._LeftMarginBox.Location = new System.Drawing.Point(347, 25);
			this._LeftMarginBox.Margin = new System.Windows.Forms.Padding(4);
			this._LeftMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._LeftMarginBox.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
			this._LeftMarginBox.Name = "_LeftMarginBox";
			this._LeftMarginBox.Size = new System.Drawing.Size(81, 25);
			this._LeftMarginBox.TabIndex = 6;
			this._LeftMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _BottomMarginBox
			// 
			this._BottomMarginBox.DecimalPlaces = 2;
			this._BottomMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._BottomMarginBox.Location = new System.Drawing.Point(205, 25);
			this._BottomMarginBox.Margin = new System.Windows.Forms.Padding(4);
			this._BottomMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._BottomMarginBox.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
			this._BottomMarginBox.Name = "_BottomMarginBox";
			this._BottomMarginBox.Size = new System.Drawing.Size(81, 25);
			this._BottomMarginBox.TabIndex = 4;
			this._BottomMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// _TopMarginBox
			// 
			this._TopMarginBox.DecimalPlaces = 2;
			this._TopMarginBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._TopMarginBox.Location = new System.Drawing.Point(71, 25);
			this._TopMarginBox.Margin = new System.Windows.Forms.Padding(4);
			this._TopMarginBox.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this._TopMarginBox.Minimum = new decimal(new int[] {
            99,
            0,
            0,
            -2147483648});
			this._TopMarginBox.Name = "_TopMarginBox";
			this._TopMarginBox.Size = new System.Drawing.Size(81, 25);
			this._TopMarginBox.TabIndex = 2;
			this._TopMarginBox.ValueChanged += new System.EventHandler(this.MarginBox_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(435, 29);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(37, 15);
			this.label7.TabIndex = 7;
			this.label7.Text = "右：";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(159, 28);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 15);
			this.label3.TabIndex = 3;
			this.label3.Text = "下：";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(300, 29);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 15);
			this.label6.TabIndex = 5;
			this.label6.Text = "左：";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 29);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "上：";
			// 
			// _LayoutGroupBox
			// 
			this._LayoutGroupBox.Controls.Add(this._ImageHAlignBox);
			this._LayoutGroupBox.Controls.Add(this.label8);
			this._LayoutGroupBox.Controls.Add(this._ImageVAlignBox);
			this._LayoutGroupBox.Controls.Add(this._ScalePdfPagesBox);
			this._LayoutGroupBox.Controls.Add(this._ResizePdfPagesBox);
			this._LayoutGroupBox.Controls.Add(this._HeightBox);
			this._LayoutGroupBox.Controls.Add(this._WidthBox);
			this._LayoutGroupBox.Controls.Add(this.label9);
			this._LayoutGroupBox.Controls.Add(this.label2);
			this._LayoutGroupBox.Controls.Add(this._PageSizeBox);
			this._LayoutGroupBox.Controls.Add(this._AutoRotateBox);
			this._LayoutGroupBox.Controls.Add(this.label5);
			this._LayoutGroupBox.Controls.Add(this.label4);
			this._LayoutGroupBox.Location = new System.Drawing.Point(8, 8);
			this._LayoutGroupBox.Margin = new System.Windows.Forms.Padding(4);
			this._LayoutGroupBox.Name = "_LayoutGroupBox";
			this._LayoutGroupBox.Padding = new System.Windows.Forms.Padding(4);
			this._LayoutGroupBox.Size = new System.Drawing.Size(576, 156);
			this._LayoutGroupBox.TabIndex = 6;
			this._LayoutGroupBox.TabStop = false;
			this._LayoutGroupBox.Text = "指定页面布局及尺寸（单位：厘米）";
			// 
			// _ImageHAlignBox
			// 
			this._ImageHAlignBox.DisplayMember = "Key";
			this._ImageHAlignBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ImageHAlignBox.Location = new System.Drawing.Point(71, 116);
			this._ImageHAlignBox.Margin = new System.Windows.Forms.Padding(4);
			this._ImageHAlignBox.Name = "_ImageHAlignBox";
			this._ImageHAlignBox.Size = new System.Drawing.Size(100, 23);
			this._ImageHAlignBox.TabIndex = 13;
			this._ImageHAlignBox.ValueMember = "Value";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 120);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(52, 15);
			this.label8.TabIndex = 12;
			this.label8.Text = "方位：";
			// 
			// _ImageVAlignBox
			// 
			this._ImageVAlignBox.DisplayMember = "Key";
			this._ImageVAlignBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ImageVAlignBox.Location = new System.Drawing.Point(185, 116);
			this._ImageVAlignBox.Margin = new System.Windows.Forms.Padding(4);
			this._ImageVAlignBox.Name = "_ImageVAlignBox";
			this._ImageVAlignBox.Size = new System.Drawing.Size(100, 23);
			this._ImageVAlignBox.TabIndex = 14;
			this._ImageVAlignBox.ValueMember = "Value";
			// 
			// _ScalePdfPagesBox
			// 
			this._ScalePdfPagesBox.AutoSize = true;
			this._ScalePdfPagesBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this._ScalePdfPagesBox.Location = new System.Drawing.Point(185, 89);
			this._ScalePdfPagesBox.Margin = new System.Windows.Forms.Padding(4);
			this._ScalePdfPagesBox.Name = "_ScalePdfPagesBox";
			this._ScalePdfPagesBox.Size = new System.Drawing.Size(178, 19);
			this._ScalePdfPagesBox.TabIndex = 11;
			this._ScalePdfPagesBox.TabStop = true;
			this._ScalePdfPagesBox.Text = "按比例缩放至页面边缘";
			this._ScalePdfPagesBox.UseVisualStyleBackColor = true;
			// 
			// _ResizePdfPagesBox
			// 
			this._ResizePdfPagesBox.AutoSize = true;
			this._ResizePdfPagesBox.Location = new System.Drawing.Point(71, 89);
			this._ResizePdfPagesBox.Margin = new System.Windows.Forms.Padding(4);
			this._ResizePdfPagesBox.Name = "_ResizePdfPagesBox";
			this._ResizePdfPagesBox.Size = new System.Drawing.Size(88, 19);
			this._ResizePdfPagesBox.TabIndex = 10;
			this._ResizePdfPagesBox.Text = "保持不变";
			this._ResizePdfPagesBox.UseVisualStyleBackColor = true;
			// 
			// _HeightBox
			// 
			this._HeightBox.DecimalPlaces = 2;
			this._HeightBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._HeightBox.Location = new System.Drawing.Point(481, 21);
			this._HeightBox.Margin = new System.Windows.Forms.Padding(4);
			this._HeightBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this._HeightBox.Name = "_HeightBox";
			this._HeightBox.Size = new System.Drawing.Size(81, 25);
			this._HeightBox.TabIndex = 5;
			// 
			// _WidthBox
			// 
			this._WidthBox.DecimalPlaces = 2;
			this._WidthBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._WidthBox.Location = new System.Drawing.Point(347, 21);
			this._WidthBox.Margin = new System.Windows.Forms.Padding(4);
			this._WidthBox.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
			this._WidthBox.Name = "_WidthBox";
			this._WidthBox.Size = new System.Drawing.Size(81, 25);
			this._WidthBox.TabIndex = 3;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 91);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(52, 15);
			this.label9.TabIndex = 0;
			this.label9.Text = "内容：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 26);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "尺寸：";
			// 
			// _PageSizeBox
			// 
			this._PageSizeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._PageSizeBox.Location = new System.Drawing.Point(71, 22);
			this._PageSizeBox.Margin = new System.Windows.Forms.Padding(4);
			this._PageSizeBox.Name = "_PageSizeBox";
			this._PageSizeBox.Size = new System.Drawing.Size(215, 23);
			this._PageSizeBox.TabIndex = 1;
			this._PageSizeBox.SelectedIndexChanged += new System.EventHandler(this._PageSizeBox_SelectedIndexChanged);
			// 
			// _AutoRotateBox
			// 
			this._AutoRotateBox.AutoSize = true;
			this._AutoRotateBox.Checked = true;
			this._AutoRotateBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._AutoRotateBox.Location = new System.Drawing.Point(71, 55);
			this._AutoRotateBox.Margin = new System.Windows.Forms.Padding(4);
			this._AutoRotateBox.Name = "_AutoRotateBox";
			this._AutoRotateBox.Size = new System.Drawing.Size(179, 19);
			this._AutoRotateBox.TabIndex = 6;
			this._AutoRotateBox.Text = "适应原始内容纵横方向";
			this._AutoRotateBox.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(436, 26);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 15);
			this.label5.TabIndex = 4;
			this.label5.Text = "高：";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(300, 26);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "宽：";
			// 
			// _ViewerSettingsPage
			// 
			this._ViewerSettingsPage.Controls.Add(this._ViewerSettingsEditor);
			this._ViewerSettingsPage.Location = new System.Drawing.Point(4, 25);
			this._ViewerSettingsPage.Margin = new System.Windows.Forms.Padding(4);
			this._ViewerSettingsPage.Name = "_ViewerSettingsPage";
			this._ViewerSettingsPage.Padding = new System.Windows.Forms.Padding(4);
			this._ViewerSettingsPage.Size = new System.Drawing.Size(595, 372);
			this._ViewerSettingsPage.TabIndex = 4;
			this._ViewerSettingsPage.Text = "阅读方式";
			this._ViewerSettingsPage.UseVisualStyleBackColor = true;
			// 
			// _ViewerSettingsEditor
			// 
			this._ViewerSettingsEditor.Location = new System.Drawing.Point(0, 0);
			this._ViewerSettingsEditor.Margin = new System.Windows.Forms.Padding(5);
			this._ViewerSettingsEditor.Name = "_ViewerSettingsEditor";
			this._ViewerSettingsEditor.Size = new System.Drawing.Size(584, 349);
			this._ViewerSettingsEditor.TabIndex = 0;
			// 
			// _CleanerPage
			// 
			this._CleanerPage.Controls.Add(this.groupBox4);
			this._CleanerPage.Controls.Add(this.groupBox3);
			this._CleanerPage.Controls.Add(this.groupBox1);
			this._CleanerPage.Location = new System.Drawing.Point(4, 25);
			this._CleanerPage.Margin = new System.Windows.Forms.Padding(4);
			this._CleanerPage.Name = "_CleanerPage";
			this._CleanerPage.Padding = new System.Windows.Forms.Padding(4);
			this._CleanerPage.Size = new System.Drawing.Size(595, 372);
			this._CleanerPage.TabIndex = 5;
			this._CleanerPage.Text = "压缩清理";
			this._CleanerPage.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this._RemoveTrailingCommandCountBox);
			this.groupBox4.Controls.Add(this._RemoveLeadingCommandCountBox);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this._RemovePageFormsBox);
			this.groupBox4.Controls.Add(this._RemovePageThumbnailsBox);
			this.groupBox4.Controls.Add(this._RemovePageTextBlocksBox);
			this.groupBox4.Controls.Add(this._RemovePageLinksBox);
			this.groupBox4.Controls.Add(this._RemoveAnnotationsBox);
			this.groupBox4.Controls.Add(this._RemovePageAutoActionsBox);
			this.groupBox4.Controls.Add(this._RemovePageMetaDataBox);
			this.groupBox4.Location = new System.Drawing.Point(8, 131);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox4.Size = new System.Drawing.Size(576, 166);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "源文档页面";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(296, 134);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(52, 15);
			this.label12.TabIndex = 10;
			this.label12.Text = "条指令";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(190, 134);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(37, 15);
			this.label11.TabIndex = 9;
			this.label11.Text = "结尾";
			// 
			// _RemoveTrailingCommandCountBox
			// 
			this._RemoveTrailingCommandCountBox.Location = new System.Drawing.Point(233, 132);
			this._RemoveTrailingCommandCountBox.Name = "_RemoveTrailingCommandCountBox";
			this._RemoveTrailingCommandCountBox.Size = new System.Drawing.Size(51, 25);
			this._RemoveTrailingCommandCountBox.TabIndex = 8;
			// 
			// _RemoveLeadingCommandCountBox
			// 
			this._RemoveLeadingCommandCountBox.Location = new System.Drawing.Point(118, 132);
			this._RemoveLeadingCommandCountBox.Name = "_RemoveLeadingCommandCountBox";
			this._RemoveLeadingCommandCountBox.Size = new System.Drawing.Size(51, 25);
			this._RemoveLeadingCommandCountBox.TabIndex = 8;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(7, 134);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(105, 15);
			this.label10.TabIndex = 7;
			this.label10.Text = "清除页面开头 ";
			// 
			// _RemovePageFormsBox
			// 
			this._RemovePageFormsBox.AutoSize = true;
			this._RemovePageFormsBox.Location = new System.Drawing.Point(299, 52);
			this._RemovePageFormsBox.Name = "_RemovePageFormsBox";
			this._RemovePageFormsBox.Size = new System.Drawing.Size(149, 19);
			this._RemovePageFormsBox.TabIndex = 3;
			this._RemovePageFormsBox.Text = "清除页面所有表单";
			this._RemovePageFormsBox.UseVisualStyleBackColor = true;
			// 
			// _RemovePageThumbnailsBox
			// 
			this._RemovePageThumbnailsBox.AutoSize = true;
			this._RemovePageThumbnailsBox.Location = new System.Drawing.Point(8, 52);
			this._RemovePageThumbnailsBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemovePageThumbnailsBox.Name = "_RemovePageThumbnailsBox";
			this._RemovePageThumbnailsBox.Size = new System.Drawing.Size(134, 19);
			this._RemovePageThumbnailsBox.TabIndex = 2;
			this._RemovePageThumbnailsBox.Text = "清除页面缩略图";
			this._RemovePageThumbnailsBox.UseVisualStyleBackColor = true;
			// 
			// _RemovePageTextBlocksBox
			// 
			this._RemovePageTextBlocksBox.AutoSize = true;
			this._RemovePageTextBlocksBox.Location = new System.Drawing.Point(8, 106);
			this._RemovePageTextBlocksBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemovePageTextBlocksBox.Name = "_RemovePageTextBlocksBox";
			this._RemovePageTextBlocksBox.Size = new System.Drawing.Size(149, 19);
			this._RemovePageTextBlocksBox.TabIndex = 6;
			this._RemovePageTextBlocksBox.Text = "清除页面所有文本";
			this._RemovePageTextBlocksBox.UseVisualStyleBackColor = true;
			// 
			// _RemovePageLinksBox
			// 
			this._RemovePageLinksBox.AutoSize = true;
			this._RemovePageLinksBox.Location = new System.Drawing.Point(299, 77);
			this._RemovePageLinksBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemovePageLinksBox.Name = "_RemovePageLinksBox";
			this._RemovePageLinksBox.Size = new System.Drawing.Size(179, 19);
			this._RemovePageLinksBox.TabIndex = 5;
			this._RemovePageLinksBox.Text = "清除页面所有链接批注";
			this._RemovePageLinksBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveAnnotationsBox
			// 
			this._RemoveAnnotationsBox.AutoSize = true;
			this._RemoveAnnotationsBox.Location = new System.Drawing.Point(8, 79);
			this._RemoveAnnotationsBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveAnnotationsBox.Name = "_RemoveAnnotationsBox";
			this._RemoveAnnotationsBox.Size = new System.Drawing.Size(149, 19);
			this._RemoveAnnotationsBox.TabIndex = 4;
			this._RemoveAnnotationsBox.Text = "清除页面所有批注";
			this._RemoveAnnotationsBox.UseVisualStyleBackColor = true;
			// 
			// _RemovePageAutoActionsBox
			// 
			this._RemovePageAutoActionsBox.AutoSize = true;
			this._RemovePageAutoActionsBox.Location = new System.Drawing.Point(8, 25);
			this._RemovePageAutoActionsBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemovePageAutoActionsBox.Name = "_RemovePageAutoActionsBox";
			this._RemovePageAutoActionsBox.Size = new System.Drawing.Size(179, 19);
			this._RemovePageAutoActionsBox.TabIndex = 0;
			this._RemovePageAutoActionsBox.Text = "禁止页面自动执行动作";
			this._RemovePageAutoActionsBox.UseVisualStyleBackColor = true;
			// 
			// _RemovePageMetaDataBox
			// 
			this._RemovePageMetaDataBox.AutoSize = true;
			this._RemovePageMetaDataBox.Location = new System.Drawing.Point(299, 25);
			this._RemovePageMetaDataBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemovePageMetaDataBox.Name = "_RemovePageMetaDataBox";
			this._RemovePageMetaDataBox.Size = new System.Drawing.Size(248, 19);
			this._RemovePageMetaDataBox.TabIndex = 1;
			this._RemovePageMetaDataBox.Text = "删除页面扩展标记（XML）元数据";
			this._RemovePageMetaDataBox.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this._RemoveBookmarksBox);
			this.groupBox3.Controls.Add(this._FixContentBox);
			this.groupBox3.Controls.Add(this._RemoveXmlMetaDataBox);
			this.groupBox3.Controls.Add(this._RemoveDocAutoActionsBox);
			this.groupBox3.Controls.Add(this._RemoveUsageRightsBox);
			this.groupBox3.Location = new System.Drawing.Point(8, 8);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox3.Size = new System.Drawing.Size(576, 116);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "源文档";
			// 
			// _RemoveBookmarksBox
			// 
			this._RemoveBookmarksBox.AutoSize = true;
			this._RemoveBookmarksBox.Location = new System.Drawing.Point(8, 80);
			this._RemoveBookmarksBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveBookmarksBox.Name = "_RemoveBookmarksBox";
			this._RemoveBookmarksBox.Size = new System.Drawing.Size(119, 19);
			this._RemoveBookmarksBox.TabIndex = 4;
			this._RemoveBookmarksBox.Text = "删除导航书签";
			this._RemoveBookmarksBox.UseVisualStyleBackColor = true;
			// 
			// _FixContentBox
			// 
			this._FixContentBox.AutoSize = true;
			this._FixContentBox.Location = new System.Drawing.Point(299, 52);
			this._FixContentBox.Margin = new System.Windows.Forms.Padding(4);
			this._FixContentBox.Name = "_FixContentBox";
			this._FixContentBox.Size = new System.Drawing.Size(149, 19);
			this._FixContentBox.TabIndex = 3;
			this._FixContentBox.Text = "尝试修复文档错误";
			this._FixContentBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveXmlMetaDataBox
			// 
			this._RemoveXmlMetaDataBox.AutoSize = true;
			this._RemoveXmlMetaDataBox.Location = new System.Drawing.Point(299, 25);
			this._RemoveXmlMetaDataBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveXmlMetaDataBox.Name = "_RemoveXmlMetaDataBox";
			this._RemoveXmlMetaDataBox.Size = new System.Drawing.Size(248, 19);
			this._RemoveXmlMetaDataBox.TabIndex = 1;
			this._RemoveXmlMetaDataBox.Text = "删除扩展标记（XML）元数据属性";
			this._RemoveXmlMetaDataBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveDocAutoActionsBox
			// 
			this._RemoveDocAutoActionsBox.AutoSize = true;
			this._RemoveDocAutoActionsBox.Location = new System.Drawing.Point(8, 52);
			this._RemoveDocAutoActionsBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveDocAutoActionsBox.Name = "_RemoveDocAutoActionsBox";
			this._RemoveDocAutoActionsBox.Size = new System.Drawing.Size(224, 19);
			this._RemoveDocAutoActionsBox.TabIndex = 2;
			this._RemoveDocAutoActionsBox.Text = "禁止打开文档时自动执行动作";
			this._RemoveDocAutoActionsBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveUsageRightsBox
			// 
			this._RemoveUsageRightsBox.AutoSize = true;
			this._RemoveUsageRightsBox.Location = new System.Drawing.Point(8, 25);
			this._RemoveUsageRightsBox.Margin = new System.Windows.Forms.Padding(4);
			this._RemoveUsageRightsBox.Name = "_RemoveUsageRightsBox";
			this._RemoveUsageRightsBox.Size = new System.Drawing.Size(179, 19);
			this._RemoveUsageRightsBox.TabIndex = 0;
			this._RemoveUsageRightsBox.Text = "清除复制、打印等限制";
			this._RemoveUsageRightsBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._RecompressWithJbig2Box);
			this.groupBox1.Controls.Add(this._FullCompressionBox);
			this.groupBox1.Location = new System.Drawing.Point(8, 305);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(576, 54);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "内容压缩";
			// 
			// _RecompressWithJbig2Box
			// 
			this._RecompressWithJbig2Box.AutoSize = true;
			this._RecompressWithJbig2Box.Location = new System.Drawing.Point(299, 25);
			this._RecompressWithJbig2Box.Margin = new System.Windows.Forms.Padding(4);
			this._RecompressWithJbig2Box.Name = "_RecompressWithJbig2Box";
			this._RecompressWithJbig2Box.Size = new System.Drawing.Size(164, 19);
			this._RecompressWithJbig2Box.TabIndex = 1;
			this._RecompressWithJbig2Box.Text = "优化黑白图片压缩率";
			this._RecompressWithJbig2Box.UseVisualStyleBackColor = true;
			// 
			// _FullCompressionBox
			// 
			this._FullCompressionBox.AutoSize = true;
			this._FullCompressionBox.Location = new System.Drawing.Point(8, 25);
			this._FullCompressionBox.Margin = new System.Windows.Forms.Padding(4);
			this._FullCompressionBox.Name = "_FullCompressionBox";
			this._FullCompressionBox.Size = new System.Drawing.Size(149, 19);
			this._FullCompressionBox.TabIndex = 0;
			this._FullCompressionBox.Text = "压缩索引表和书签";
			this._FullCompressionBox.UseVisualStyleBackColor = true;
			// 
			// _DocumentInfoPage
			// 
			this._DocumentInfoPage.Controls.Add(this._DocumentInfoEditor);
			this._DocumentInfoPage.Location = new System.Drawing.Point(4, 25);
			this._DocumentInfoPage.Margin = new System.Windows.Forms.Padding(4);
			this._DocumentInfoPage.Name = "_DocumentInfoPage";
			this._DocumentInfoPage.Padding = new System.Windows.Forms.Padding(4);
			this._DocumentInfoPage.Size = new System.Drawing.Size(595, 372);
			this._DocumentInfoPage.TabIndex = 2;
			this._DocumentInfoPage.Text = "文档属性";
			this._DocumentInfoPage.UseVisualStyleBackColor = true;
			// 
			// _DocumentInfoEditor
			// 
			this._DocumentInfoEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._DocumentInfoEditor.Location = new System.Drawing.Point(0, 0);
			this._DocumentInfoEditor.Margin = new System.Windows.Forms.Padding(5);
			this._DocumentInfoEditor.Name = "_DocumentInfoEditor";
			this._DocumentInfoEditor.Size = new System.Drawing.Size(585, 368);
			this._DocumentInfoEditor.TabIndex = 0;
			// 
			// _PageLabelsPage
			// 
			this._PageLabelsPage.Controls.Add(this._PageLabelEditor);
			this._PageLabelsPage.Location = new System.Drawing.Point(4, 25);
			this._PageLabelsPage.Margin = new System.Windows.Forms.Padding(4);
			this._PageLabelsPage.Name = "_PageLabelsPage";
			this._PageLabelsPage.Padding = new System.Windows.Forms.Padding(4);
			this._PageLabelsPage.Size = new System.Drawing.Size(595, 372);
			this._PageLabelsPage.TabIndex = 3;
			this._PageLabelsPage.Text = "页码标签";
			this._PageLabelsPage.UseVisualStyleBackColor = true;
			// 
			// _PageLabelEditor
			// 
			this._PageLabelEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._PageLabelEditor.Labels = null;
			this._PageLabelEditor.Location = new System.Drawing.Point(4, 4);
			this._PageLabelEditor.Margin = new System.Windows.Forms.Padding(5);
			this._PageLabelEditor.Name = "_PageLabelEditor";
			this._PageLabelEditor.Size = new System.Drawing.Size(587, 364);
			this._PageLabelEditor.TabIndex = 0;
			// 
			// _FontSubstitutionsPage
			// 
			this._FontSubstitutionsPage.Controls.Add(this._FontSubstitutionsEditor);
			this._FontSubstitutionsPage.Location = new System.Drawing.Point(4, 25);
			this._FontSubstitutionsPage.Margin = new System.Windows.Forms.Padding(4);
			this._FontSubstitutionsPage.Name = "_FontSubstitutionsPage";
			this._FontSubstitutionsPage.Padding = new System.Windows.Forms.Padding(4);
			this._FontSubstitutionsPage.Size = new System.Drawing.Size(595, 372);
			this._FontSubstitutionsPage.TabIndex = 7;
			this._FontSubstitutionsPage.Text = "替换字体";
			this._FontSubstitutionsPage.UseVisualStyleBackColor = true;
			// 
			// _FontSubstitutionsEditor
			// 
			this._FontSubstitutionsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._FontSubstitutionsEditor.Location = new System.Drawing.Point(4, 4);
			this._FontSubstitutionsEditor.Margin = new System.Windows.Forms.Padding(5);
			this._FontSubstitutionsEditor.Name = "_FontSubstitutionsEditor";
			this._FontSubstitutionsEditor.Options = null;
			this._FontSubstitutionsEditor.Size = new System.Drawing.Size(587, 364);
			this._FontSubstitutionsEditor.Substitutions = null;
			this._FontSubstitutionsEditor.TabIndex = 0;
			// 
			// _ConfigPage
			// 
			this._ConfigPage.Controls.Add(this._ResetButton);
			this._ConfigPage.Controls.Add(this._ImportButton);
			this._ConfigPage.Controls.Add(this._ExportButton);
			this._ConfigPage.Location = new System.Drawing.Point(4, 25);
			this._ConfigPage.Name = "_ConfigPage";
			this._ConfigPage.Padding = new System.Windows.Forms.Padding(3);
			this._ConfigPage.Size = new System.Drawing.Size(595, 372);
			this._ConfigPage.TabIndex = 9;
			this._ConfigPage.Text = "其它";
			this._ConfigPage.UseVisualStyleBackColor = true;
			// 
			// _ExportButton
			// 
			this._ExportButton.Location = new System.Drawing.Point(30, 22);
			this._ExportButton.Name = "_ExportButton";
			this._ExportButton.Size = new System.Drawing.Size(185, 23);
			this._ExportButton.TabIndex = 0;
			this._ExportButton.Text = "导出选项配置文件...";
			this._ExportButton.UseVisualStyleBackColor = true;
			// 
			// _ImportButton
			// 
			this._ImportButton.Location = new System.Drawing.Point(30, 51);
			this._ImportButton.Name = "_ImportButton";
			this._ImportButton.Size = new System.Drawing.Size(185, 23);
			this._ImportButton.TabIndex = 0;
			this._ImportButton.Text = "导入选项配置文件...";
			this._ImportButton.UseVisualStyleBackColor = true;
			// 
			// _ResetButton
			// 
			this._ResetButton.Location = new System.Drawing.Point(30, 80);
			this._ResetButton.Name = "_ResetButton";
			this._ResetButton.Size = new System.Drawing.Size(185, 23);
			this._ResetButton.TabIndex = 0;
			this._ResetButton.Text = "还原选项为默认值";
			this._ResetButton.UseVisualStyleBackColor = true;
			// 
			// PatcherOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(635, 435);
			this.Controls.Add(this._MainTab);
			this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(637, 458);
			this.Name = "PatcherOptionForm";
			this.ShowInTaskbar = false;
			this.Text = "PDF 文档选项";
			this.Load += new System.EventHandler(this.PatcherOptionForm_Load);
			this._PageSettingsPage.ResumeLayout(false);
			this._MainTab.ResumeLayout(false);
			this._PageLayoutPage.ResumeLayout(false);
			this._MarginGroupBox.ResumeLayout(false);
			this._MarginGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._RightMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._LeftMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._BottomMarginBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._TopMarginBox)).EndInit();
			this._LayoutGroupBox.ResumeLayout(false);
			this._LayoutGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).EndInit();
			this._ViewerSettingsPage.ResumeLayout(false);
			this._CleanerPage.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._RemoveTrailingCommandCountBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._RemoveLeadingCommandCountBox)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this._DocumentInfoPage.ResumeLayout(false);
			this._PageLabelsPage.ResumeLayout(false);
			this._FontSubstitutionsPage.ResumeLayout(false);
			this._ConfigPage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl _MainTab;
		private System.Windows.Forms.TabPage _DocumentInfoPage;
		private PDFPatcher.Functions.DocumentInfoEditor _DocumentInfoEditor;
		private System.Windows.Forms.TabPage _PageLabelsPage;
		private PDFPatcher.Functions.PageLabelEditor _PageLabelEditor;
		private System.Windows.Forms.TabPage _ViewerSettingsPage;
		private PDFPatcher.Functions.ViewerPreferenceEditor _ViewerSettingsEditor;
		private System.Windows.Forms.TabPage _CleanerPage;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox _FixContentBox;
		private System.Windows.Forms.CheckBox _RemoveXmlMetaDataBox;
		private System.Windows.Forms.CheckBox _RemoveDocAutoActionsBox;
		private System.Windows.Forms.CheckBox _RemoveUsageRightsBox;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox _RemoveAnnotationsBox;
		private System.Windows.Forms.CheckBox _RemovePageAutoActionsBox;
		private System.Windows.Forms.CheckBox _RemovePageMetaDataBox;
		private System.Windows.Forms.CheckBox _RemovePageTextBlocksBox;
		private System.Windows.Forms.CheckBox _RemoveBookmarksBox;
		private PageSettingsEditor _PageSettingsEditor;
		private System.Windows.Forms.CheckBox _RemovePageThumbnailsBox;
		private System.Windows.Forms.TabPage _FontSubstitutionsPage;
		private FontSubstitutionsEditor _FontSubstitutionsEditor;
		private System.Windows.Forms.TabPage _PageSettingsPage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox _RecompressWithJbig2Box;
		private System.Windows.Forms.CheckBox _FullCompressionBox;
		private System.Windows.Forms.TabPage _PageLayoutPage;
		private System.Windows.Forms.GroupBox _LayoutGroupBox;
		private System.Windows.Forms.NumericUpDown _HeightBox;
		private System.Windows.Forms.NumericUpDown _WidthBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _PageSizeBox;
		private System.Windows.Forms.CheckBox _AutoRotateBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton _ResizePdfPagesBox;
		private System.Windows.Forms.RadioButton _ScalePdfPagesBox;
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
		private System.Windows.Forms.ComboBox _ImageHAlignBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox _ImageVAlignBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox _RemovePageFormsBox;
		private System.Windows.Forms.CheckBox _RemovePageLinksBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown _RemoveTrailingCommandCountBox;
		private System.Windows.Forms.NumericUpDown _RemoveLeadingCommandCountBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox _MarginUnitBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TabPage _ConfigPage;
		private System.Windows.Forms.Button _ResetButton;
		private System.Windows.Forms.Button _ImportButton;
		private System.Windows.Forms.Button _ExportButton;
	}
}
