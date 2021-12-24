namespace PDFPatcher.Functions
{
	partial class InfoFileOptionControl
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
			this._EncodingBox = new System.Windows.Forms.ComboBox ();
			this.label1 = new System.Windows.Forms.Label ();
			this._ExtractPageLinksBox = new System.Windows.Forms.CheckBox ();
			this._ExportViewerPreferencesBox = new System.Windows.Forms.CheckBox ();
			this._ExportBookmarksBox = new System.Windows.Forms.CheckBox ();
			this._ExportOptionsTab = new System.Windows.Forms.TabControl ();
			this.tabPage1 = new System.Windows.Forms.TabPage ();
			this._UnitBox = new System.Windows.Forms.ComboBox ();
			this.label7 = new System.Windows.Forms.Label ();
			this.groupBox1 = new System.Windows.Forms.GroupBox ();
			this._ExportDocPropertiesBox = new System.Windows.Forms.CheckBox ();
			this._ExtractPageSettingsBox = new System.Windows.Forms.CheckBox ();
			this._ConsolidateNamedDestBox = new System.Windows.Forms.CheckBox ();
			this.tabPage2 = new System.Windows.Forms.TabPage ();
			this._ExportCatalogBox = new System.Windows.Forms.CheckBox ();
			this.label2 = new System.Windows.Forms.Label ();
			this.label5 = new System.Windows.Forms.Label ();
			this._ExtractPageContentBox = new System.Windows.Forms.CheckBox ();
			this._PageContentBox = new System.Windows.Forms.Panel ();
			this._ExportContentOperatorsBox = new System.Windows.Forms.CheckBox ();
			this._ExtractPageDictionaryBox = new System.Windows.Forms.CheckBox ();
			this._ExtractPageRangeBox = new System.Windows.Forms.TextBox ();
			this.label9 = new System.Windows.Forms.Label ();
			this.label3 = new System.Windows.Forms.Label ();
			this.label8 = new System.Windows.Forms.Label ();
			this._ExportBinaryStreamBox = new System.Windows.Forms.NumericUpDown ();
			this._ExtractImagesBox = new System.Windows.Forms.CheckBox ();
			this._ExtractPageTextContentBox = new System.Windows.Forms.CheckBox ();
			this.tabPage3 = new System.Windows.Forms.TabPage ();
			this.groupBox2 = new System.Windows.Forms.GroupBox ();
			this._ImportPageSettingsBox = new System.Windows.Forms.CheckBox ();
			this._ImportDocumentInfoBox = new System.Windows.Forms.CheckBox ();
			this.panel1 = new System.Windows.Forms.Panel ();
			this.label10 = new System.Windows.Forms.Label ();
			this._RemoveOriginalPageLinksBox = new System.Windows.Forms.RadioButton ();
			this._KeepOriginalPageLinksBox = new System.Windows.Forms.RadioButton ();
			this._ImportBookmarksBox = new System.Windows.Forms.CheckBox ();
			this._ImportPageLinksBox = new System.Windows.Forms.CheckBox ();
			this._ImportViewerPreferencesBox = new System.Windows.Forms.CheckBox ();
			this._ExportOptionsTab.SuspendLayout ();
			this.tabPage1.SuspendLayout ();
			this.groupBox1.SuspendLayout ();
			this.tabPage2.SuspendLayout ();
			this._PageContentBox.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._ExportBinaryStreamBox)).BeginInit ();
			this.tabPage3.SuspendLayout ();
			this.groupBox2.SuspendLayout ();
			this.panel1.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// _EncodingBox
			// 
			this._EncodingBox.FormattingEnabled = true;
			this._EncodingBox.Items.AddRange (new object[] {
            "系统默认",
            "GB18030",
            "UTF-8",
            "UTF-16",
            "Big5"});
			this._EncodingBox.Location = new System.Drawing.Point (77, 6);
			this._EncodingBox.Name = "_EncodingBox";
			this._EncodingBox.Size = new System.Drawing.Size (121, 20);
			this._EncodingBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (6, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (65, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "字符编码：";
			// 
			// _ExtractPageLinksBox
			// 
			this._ExtractPageLinksBox.AutoSize = true;
			this._ExtractPageLinksBox.Location = new System.Drawing.Point (114, 42);
			this._ExtractPageLinksBox.Name = "_ExtractPageLinksBox";
			this._ExtractPageLinksBox.Size = new System.Drawing.Size (96, 16);
			this._ExtractPageLinksBox.TabIndex = 4;
			this._ExtractPageLinksBox.Text = "页面内的链接";
			this._ExtractPageLinksBox.UseVisualStyleBackColor = true;
			// 
			// _ExportViewerPreferencesBox
			// 
			this._ExportViewerPreferencesBox.AutoSize = true;
			this._ExportViewerPreferencesBox.Location = new System.Drawing.Point (6, 86);
			this._ExportViewerPreferencesBox.Name = "_ExportViewerPreferencesBox";
			this._ExportViewerPreferencesBox.Size = new System.Drawing.Size (240, 16);
			this._ExportViewerPreferencesBox.TabIndex = 6;
			this._ExportViewerPreferencesBox.Text = "阅读器设置（如排版布局、页码样式等）";
			this._ExportViewerPreferencesBox.UseVisualStyleBackColor = true;
			// 
			// _ExportBookmarksBox
			// 
			this._ExportBookmarksBox.AutoSize = true;
			this._ExportBookmarksBox.Location = new System.Drawing.Point (6, 42);
			this._ExportBookmarksBox.Name = "_ExportBookmarksBox";
			this._ExportBookmarksBox.Size = new System.Drawing.Size (72, 16);
			this._ExportBookmarksBox.TabIndex = 3;
			this._ExportBookmarksBox.Text = "文档书签";
			this._ExportBookmarksBox.UseVisualStyleBackColor = true;
			// 
			// _ExportOptionsTab
			// 
			this._ExportOptionsTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._ExportOptionsTab.Controls.Add (this.tabPage1);
			this._ExportOptionsTab.Controls.Add (this.tabPage2);
			this._ExportOptionsTab.Controls.Add (this.tabPage3);
			this._ExportOptionsTab.Location = new System.Drawing.Point (12, 12);
			this._ExportOptionsTab.Name = "_ExportOptionsTab";
			this._ExportOptionsTab.SelectedIndex = 0;
			this._ExportOptionsTab.Size = new System.Drawing.Size (424, 245);
			this._ExportOptionsTab.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add (this._UnitBox);
			this.tabPage1.Controls.Add (this.label7);
			this.tabPage1.Controls.Add (this.label1);
			this.tabPage1.Controls.Add (this._EncodingBox);
			this.tabPage1.Controls.Add (this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point (4, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding (3);
			this.tabPage1.Size = new System.Drawing.Size (416, 220);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "常规导出选项";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _UnitBox
			// 
			this._UnitBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._UnitBox.FormattingEnabled = true;
			this._UnitBox.Location = new System.Drawing.Point (299, 6);
			this._UnitBox.Name = "_UnitBox";
			this._UnitBox.Size = new System.Drawing.Size (54, 20);
			this._UnitBox.TabIndex = 9;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point (204, 9);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (89, 12);
			this.label7.TabIndex = 8;
			this.label7.Text = "尺寸度量单位：";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add (this._ExportDocPropertiesBox);
			this.groupBox1.Controls.Add (this._ExportBookmarksBox);
			this.groupBox1.Controls.Add (this._ExtractPageSettingsBox);
			this.groupBox1.Controls.Add (this._ConsolidateNamedDestBox);
			this.groupBox1.Controls.Add (this._ExtractPageLinksBox);
			this.groupBox1.Controls.Add (this._ExportViewerPreferencesBox);
			this.groupBox1.Location = new System.Drawing.Point (6, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size (404, 185);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "导出如下内容到信息文件";
			// 
			// _ExportDocPropertiesBox
			// 
			this._ExportDocPropertiesBox.AutoSize = true;
			this._ExportDocPropertiesBox.Location = new System.Drawing.Point (6, 20);
			this._ExportDocPropertiesBox.Name = "_ExportDocPropertiesBox";
			this._ExportDocPropertiesBox.Size = new System.Drawing.Size (228, 16);
			this._ExportDocPropertiesBox.TabIndex = 2;
			this._ExportDocPropertiesBox.Text = "文档属性（如标题、作者、关键字等）";
			this._ExportDocPropertiesBox.UseVisualStyleBackColor = true;
			// 
			// _ExtractPageSettingsBox
			// 
			this._ExtractPageSettingsBox.AutoSize = true;
			this._ExtractPageSettingsBox.Location = new System.Drawing.Point (6, 108);
			this._ExtractPageSettingsBox.Name = "_ExtractPageSettingsBox";
			this._ExtractPageSettingsBox.Size = new System.Drawing.Size (264, 16);
			this._ExtractPageSettingsBox.TabIndex = 7;
			this._ExtractPageSettingsBox.Text = "页面设置（如页面尺寸、裁剪、旋转角度等）";
			this._ExtractPageSettingsBox.UseVisualStyleBackColor = true;
			// 
			// _ConsolidateNamedDestBox
			// 
			this._ConsolidateNamedDestBox.AutoSize = true;
			this._ConsolidateNamedDestBox.Location = new System.Drawing.Point (18, 64);
			this._ConsolidateNamedDestBox.Name = "_ConsolidateNamedDestBox";
			this._ConsolidateNamedDestBox.Size = new System.Drawing.Size (192, 16);
			this._ConsolidateNamedDestBox.TabIndex = 5;
			this._ConsolidateNamedDestBox.Text = "解析书签和页面链接的命名位置";
			this._ConsolidateNamedDestBox.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add (this._ExportCatalogBox);
			this.tabPage2.Controls.Add (this.label2);
			this.tabPage2.Controls.Add (this.label5);
			this.tabPage2.Controls.Add (this._ExtractPageContentBox);
			this.tabPage2.Controls.Add (this._PageContentBox);
			this.tabPage2.Location = new System.Drawing.Point (4, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding (3);
			this.tabPage2.Size = new System.Drawing.Size (416, 220);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "高级导出选项";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// _ExportCatalogBox
			// 
			this._ExportCatalogBox.AutoSize = true;
			this._ExportCatalogBox.Location = new System.Drawing.Point (6, 38);
			this._ExportCatalogBox.Name = "_ExportCatalogBox";
			this._ExportCatalogBox.Size = new System.Drawing.Size (96, 16);
			this._ExportCatalogBox.TabIndex = 1;
			this._ExportCatalogBox.Text = "导出编录信息";
			this._ExportCatalogBox.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point (4, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (432, 31);
			this.label2.TabIndex = 0;
			this.label2.Text = "说明：高级导出选项导出的内容仅供研究 PDF 文件结构之用，导入信息文件时不会导入这些内容。";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point (107, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (307, 28);
			this.label5.TabIndex = 3;
			this.label5.Text = "此选项将生成很大的信息文件，包含页面大部分内容（如字体、图片信息等）。";
			// 
			// _ExtractPageContentBox
			// 
			this._ExtractPageContentBox.AutoSize = true;
			this._ExtractPageContentBox.Location = new System.Drawing.Point (6, 60);
			this._ExtractPageContentBox.Name = "_ExtractPageContentBox";
			this._ExtractPageContentBox.Size = new System.Drawing.Size (96, 16);
			this._ExtractPageContentBox.TabIndex = 2;
			this._ExtractPageContentBox.Text = "导出页面内容";
			this._ExtractPageContentBox.UseVisualStyleBackColor = true;
			// 
			// _PageContentBox
			// 
			this._PageContentBox.Controls.Add (this._ExportContentOperatorsBox);
			this._PageContentBox.Controls.Add (this._ExtractPageDictionaryBox);
			this._PageContentBox.Controls.Add (this._ExtractPageRangeBox);
			this._PageContentBox.Controls.Add (this.label9);
			this._PageContentBox.Controls.Add (this.label3);
			this._PageContentBox.Controls.Add (this.label8);
			this._PageContentBox.Controls.Add (this._ExportBinaryStreamBox);
			this._PageContentBox.Controls.Add (this._ExtractImagesBox);
			this._PageContentBox.Controls.Add (this._ExtractPageTextContentBox);
			this._PageContentBox.Enabled = false;
			this._PageContentBox.Location = new System.Drawing.Point (17, 92);
			this._PageContentBox.Name = "_PageContentBox";
			this._PageContentBox.Size = new System.Drawing.Size (419, 125);
			this._PageContentBox.TabIndex = 4;
			// 
			// _ExportContentOperatorsBox
			// 
			this._ExportContentOperatorsBox.AutoSize = true;
			this._ExportContentOperatorsBox.Location = new System.Drawing.Point (224, 36);
			this._ExportContentOperatorsBox.Name = "_ExportContentOperatorsBox";
			this._ExportContentOperatorsBox.Size = new System.Drawing.Size (120, 16);
			this._ExportContentOperatorsBox.TabIndex = 9;
			this._ExportContentOperatorsBox.Text = "导出绘制页面命令";
			this._ExportContentOperatorsBox.UseVisualStyleBackColor = true;
			// 
			// _ExtractPageDictionaryBox
			// 
			this._ExtractPageDictionaryBox.AutoSize = true;
			this._ExtractPageDictionaryBox.Location = new System.Drawing.Point (10, 36);
			this._ExtractPageDictionaryBox.Name = "_ExtractPageDictionaryBox";
			this._ExtractPageDictionaryBox.Size = new System.Drawing.Size (120, 16);
			this._ExtractPageDictionaryBox.TabIndex = 8;
			this._ExtractPageDictionaryBox.Text = "导出页面字典信息";
			this._ExtractPageDictionaryBox.UseVisualStyleBackColor = true;
			// 
			// _ExtractPageRangeBox
			// 
			this._ExtractPageRangeBox.Location = new System.Drawing.Point (98, 3);
			this._ExtractPageRangeBox.Name = "_ExtractPageRangeBox";
			this._ExtractPageRangeBox.Size = new System.Drawing.Size (231, 21);
			this._ExtractPageRangeBox.TabIndex = 1;
			this._ExtractPageRangeBox.Leave += new System.EventHandler (this._ExtractPageRangeBox_Leave);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point (226, 77);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size (83, 12);
			this.label9.TabIndex = 7;
			this.label9.Text = "（0：不限制）";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (3, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (89, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "导出页码范围：";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point (5, 77);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size (149, 12);
			this.label8.TabIndex = 5;
			this.label8.Text = "限制导出二进制流字节数：";
			// 
			// _ExportBinaryStreamBox
			// 
			this._ExportBinaryStreamBox.Increment = new decimal (new int[] {
            100,
            0,
            0,
            0});
			this._ExportBinaryStreamBox.Location = new System.Drawing.Point (157, 75);
			this._ExportBinaryStreamBox.Maximum = new decimal (new int[] {
            99999999,
            0,
            0,
            0});
			this._ExportBinaryStreamBox.Name = "_ExportBinaryStreamBox";
			this._ExportBinaryStreamBox.Size = new System.Drawing.Size (63, 21);
			this._ExportBinaryStreamBox.TabIndex = 6;
			this._ExportBinaryStreamBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _ExtractImagesBox
			// 
			this._ExtractImagesBox.AutoSize = true;
			this._ExtractImagesBox.Location = new System.Drawing.Point (10, 58);
			this._ExtractImagesBox.Name = "_ExtractImagesBox";
			this._ExtractImagesBox.Size = new System.Drawing.Size (144, 16);
			this._ExtractImagesBox.TabIndex = 3;
			this._ExtractImagesBox.Text = "将图片导出为独立文件";
			this._ExtractImagesBox.UseVisualStyleBackColor = true;
			// 
			// _ExtractPageTextContentBox
			// 
			this._ExtractPageTextContentBox.AutoSize = true;
			this._ExtractPageTextContentBox.Location = new System.Drawing.Point (224, 58);
			this._ExtractPageTextContentBox.Name = "_ExtractPageTextContentBox";
			this._ExtractPageTextContentBox.Size = new System.Drawing.Size (144, 16);
			this._ExtractPageTextContentBox.TabIndex = 4;
			this._ExtractPageTextContentBox.Text = "解码导出页面内的文本";
			this._ExtractPageTextContentBox.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add (this.groupBox2);
			this.tabPage3.Location = new System.Drawing.Point (4, 21);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding (3);
			this.tabPage3.Size = new System.Drawing.Size (416, 220);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "导入选项";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add (this._ImportPageSettingsBox);
			this.groupBox2.Controls.Add (this._ImportDocumentInfoBox);
			this.groupBox2.Controls.Add (this.panel1);
			this.groupBox2.Controls.Add (this._ImportBookmarksBox);
			this.groupBox2.Controls.Add (this._ImportPageLinksBox);
			this.groupBox2.Controls.Add (this._ImportViewerPreferencesBox);
			this.groupBox2.Location = new System.Drawing.Point (6, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size (430, 211);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "导入信息文件如下项目";
			// 
			// _ImportPageSettingsBox
			// 
			this._ImportPageSettingsBox.AutoSize = true;
			this._ImportPageSettingsBox.Location = new System.Drawing.Point (6, 108);
			this._ImportPageSettingsBox.Name = "_ImportPageSettingsBox";
			this._ImportPageSettingsBox.Size = new System.Drawing.Size (264, 16);
			this._ImportPageSettingsBox.TabIndex = 12;
			this._ImportPageSettingsBox.Text = "页面设置（如页面尺寸、裁剪、旋转角度等）";
			this._ImportPageSettingsBox.UseVisualStyleBackColor = true;
			// 
			// _ImportDocumentInfoBox
			// 
			this._ImportDocumentInfoBox.AutoSize = true;
			this._ImportDocumentInfoBox.Checked = true;
			this._ImportDocumentInfoBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._ImportDocumentInfoBox.Location = new System.Drawing.Point (6, 20);
			this._ImportDocumentInfoBox.Name = "_ImportDocumentInfoBox";
			this._ImportDocumentInfoBox.Size = new System.Drawing.Size (252, 16);
			this._ImportDocumentInfoBox.TabIndex = 7;
			this._ImportDocumentInfoBox.Text = "文档属性信息（如作者、主题、关键字等）";
			this._ImportDocumentInfoBox.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add (this.label10);
			this.panel1.Controls.Add (this._RemoveOriginalPageLinksBox);
			this.panel1.Controls.Add (this._KeepOriginalPageLinksBox);
			this.panel1.Location = new System.Drawing.Point (213, 64);
			this.panel1.Margin = new System.Windows.Forms.Padding (0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size (191, 16);
			this.panel1.TabIndex = 10;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point (3, 1);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size (77, 12);
			this.label10.TabIndex = 0;
			this.label10.Text = "原页面链接：";
			// 
			// _RemoveOriginalPageLinksBox
			// 
			this._RemoveOriginalPageLinksBox.AutoSize = true;
			this._RemoveOriginalPageLinksBox.Checked = true;
			this._RemoveOriginalPageLinksBox.Location = new System.Drawing.Point (86, -1);
			this._RemoveOriginalPageLinksBox.Name = "_RemoveOriginalPageLinksBox";
			this._RemoveOriginalPageLinksBox.Size = new System.Drawing.Size (46, 16);
			this._RemoveOriginalPageLinksBox.TabIndex = 1;
			this._RemoveOriginalPageLinksBox.TabStop = true;
			this._RemoveOriginalPageLinksBox.Text = "替换";
			this._RemoveOriginalPageLinksBox.UseVisualStyleBackColor = true;
			// 
			// _KeepOriginalPageLinksBox
			// 
			this._KeepOriginalPageLinksBox.AutoSize = true;
			this._KeepOriginalPageLinksBox.Location = new System.Drawing.Point (139, -1);
			this._KeepOriginalPageLinksBox.Name = "_KeepOriginalPageLinksBox";
			this._KeepOriginalPageLinksBox.Size = new System.Drawing.Size (46, 16);
			this._KeepOriginalPageLinksBox.TabIndex = 2;
			this._KeepOriginalPageLinksBox.Text = "保留";
			this._KeepOriginalPageLinksBox.UseVisualStyleBackColor = true;
			// 
			// _ImportBookmarksBox
			// 
			this._ImportBookmarksBox.AutoSize = true;
			this._ImportBookmarksBox.Checked = true;
			this._ImportBookmarksBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._ImportBookmarksBox.Location = new System.Drawing.Point (6, 42);
			this._ImportBookmarksBox.Name = "_ImportBookmarksBox";
			this._ImportBookmarksBox.Size = new System.Drawing.Size (72, 16);
			this._ImportBookmarksBox.TabIndex = 8;
			this._ImportBookmarksBox.Text = "文档书签";
			this._ImportBookmarksBox.UseVisualStyleBackColor = true;
			// 
			// _ImportPageLinksBox
			// 
			this._ImportPageLinksBox.AutoSize = true;
			this._ImportPageLinksBox.Checked = true;
			this._ImportPageLinksBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._ImportPageLinksBox.Location = new System.Drawing.Point (6, 64);
			this._ImportPageLinksBox.Name = "_ImportPageLinksBox";
			this._ImportPageLinksBox.Size = new System.Drawing.Size (204, 16);
			this._ImportPageLinksBox.TabIndex = 9;
			this._ImportPageLinksBox.Text = "页面内的链接（合并模式下无效）";
			this._ImportPageLinksBox.UseVisualStyleBackColor = true;
			// 
			// _ImportViewerPreferencesBox
			// 
			this._ImportViewerPreferencesBox.AutoSize = true;
			this._ImportViewerPreferencesBox.Checked = true;
			this._ImportViewerPreferencesBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._ImportViewerPreferencesBox.Location = new System.Drawing.Point (6, 86);
			this._ImportViewerPreferencesBox.Name = "_ImportViewerPreferencesBox";
			this._ImportViewerPreferencesBox.Size = new System.Drawing.Size (240, 16);
			this._ImportViewerPreferencesBox.TabIndex = 11;
			this._ImportViewerPreferencesBox.Text = "阅读器设置（如排版布局、页码样式等）";
			this._ImportViewerPreferencesBox.UseVisualStyleBackColor = true;
			// 
			// ExportOptionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (448, 269);
			this.Controls.Add (this._ExportOptionsTab);
			this.Font = new System.Drawing.Font ("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExportOptionControl";
			this.Text = "信息文件选项";
			this.VisibleChanged += new System.EventHandler (this.ExportOptionControl_VisibleChanged);
			this._ExportOptionsTab.ResumeLayout (false);
			this.tabPage1.ResumeLayout (false);
			this.tabPage1.PerformLayout ();
			this.groupBox1.ResumeLayout (false);
			this.groupBox1.PerformLayout ();
			this.tabPage2.ResumeLayout (false);
			this.tabPage2.PerformLayout ();
			this._PageContentBox.ResumeLayout (false);
			this._PageContentBox.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this._ExportBinaryStreamBox)).EndInit ();
			this.tabPage3.ResumeLayout (false);
			this.groupBox2.ResumeLayout (false);
			this.groupBox2.PerformLayout ();
			this.panel1.ResumeLayout (false);
			this.panel1.PerformLayout ();
			this.ResumeLayout (false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _ExtractPageLinksBox;
		private System.Windows.Forms.CheckBox _ExportViewerPreferencesBox;
		private System.Windows.Forms.CheckBox _ExportBookmarksBox;
		private System.Windows.Forms.ComboBox _EncodingBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl _ExportOptionsTab;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _ExtractPageRangeBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox _ExtractPageContentBox;
		private System.Windows.Forms.CheckBox _ExtractPageSettingsBox;
		private System.Windows.Forms.CheckBox _ExtractImagesBox;
		private System.Windows.Forms.CheckBox _ConsolidateNamedDestBox;
		private System.Windows.Forms.CheckBox _ExportDocPropertiesBox;
		private System.Windows.Forms.ComboBox _UnitBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown _ExportBinaryStreamBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.CheckBox _ExportCatalogBox;
		private System.Windows.Forms.CheckBox _ExtractPageTextContentBox;
		private System.Windows.Forms.Panel _PageContentBox;
		private System.Windows.Forms.CheckBox _ExtractPageDictionaryBox;
		private System.Windows.Forms.CheckBox _ExportContentOperatorsBox;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox _ImportDocumentInfoBox;
		private System.Windows.Forms.CheckBox _ImportPageLinksBox;
		private System.Windows.Forms.CheckBox _ImportViewerPreferencesBox;
		private System.Windows.Forms.CheckBox _ImportBookmarksBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.RadioButton _RemoveOriginalPageLinksBox;
		private System.Windows.Forms.RadioButton _KeepOriginalPageLinksBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox _ImportPageSettingsBox;
	}
}
