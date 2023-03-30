namespace PDFPatcher.Functions
{
	partial class AutoBookmarkControl
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripDropDownButton _AddAdjustmentButton;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this._AddFilterMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._TitleSizeThresholdBox = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._PageRangeBox = new System.Windows.Forms.TextBox();
			this._MergeAdjacentTitlesBox = new System.Windows.Forms.CheckBox();
			this._MergeDifferentSizeTitlesBox = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this._FirstLineAsTitleBox = new System.Windows.Forms.CheckBox();
			this._IgnoreOverlappedTextBox = new System.Windows.Forms.CheckBox();
			this._CreateBookmarkForFirstPageBox = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._WritingDirectionBox = new System.Windows.Forms.ComboBox();
			this._AutoHierarchicalArrangementBox = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this._MaxDistanceBetweenLinesBox = new System.Windows.Forms.NumericUpDown();
			this._GoToPageTopLevelBox = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this._YOffsetBox = new System.Windows.Forms.NumericUpDown();
			this._MergeDifferentFontTitlesBox = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this._IgnoreNumericTitleBox = new System.Windows.Forms.CheckBox();
			this._IgnoreSingleCharacterTitleBox = new System.Windows.Forms.CheckBox();
			this._ClearTextFiltersButton = new System.Windows.Forms.Button();
			this._IgnorePatternsBox = new System.Windows.Forms.DataGridView();
			this._PatternColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this._MatchCaseColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this._FullMatchColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this._PatternTypeColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this._RemovePatternColumn = new System.Windows.Forms.DataGridViewLinkColumn();
			this.label10 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this._FilterToolbar = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this._AddFilterFromInfoFileButton = new System.Windows.Forms.ToolStripButton();
			this._DeleteAdjustmentButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._CopyFilterButton = new System.Windows.Forms.ToolStripButton();
			this._PasteButton = new System.Windows.Forms.ToolStripButton();
			this._LevelAdjustmentBox = new BrightIdeasSoftware.ObjectListView();
			this._AdvancedFilterColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._AdjustmentLevelColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._RelativeAdjustmentColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._FilterBeforeMergeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.label12 = new System.Windows.Forms.Label();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this._ExportTextCoordinateBox = new System.Windows.Forms.CheckBox();
			this._ShowAllFontsBox = new System.Windows.Forms.CheckBox();
			this._DisplayFontStatisticsBox = new System.Windows.Forms.CheckBox();
			this._BookmarkControl = new PDFPatcher.BookmarkControl();
			this._SourceFileControl = new PDFPatcher.SourceFileControl();
			this._ExportBookmarkButton = new EnhancedGlassButton.GlassButton();
			_AddAdjustmentButton = new System.Windows.Forms.ToolStripDropDownButton();
			((System.ComponentModel.ISupportInitialize)(this._TitleSizeThresholdBox)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._MaxDistanceBetweenLinesBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._GoToPageTopLevelBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._YOffsetBox)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._IgnorePatternsBox)).BeginInit();
			this.tabPage3.SuspendLayout();
			this._FilterToolbar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._LevelAdjustmentBox)).BeginInit();
			this.tabPage5.SuspendLayout();
			this.SuspendLayout();
			// 
			// _AddAdjustmentButton
			// 
			_AddAdjustmentButton.DropDown = this._AddFilterMenu;
			_AddAdjustmentButton.Image = global::PDFPatcher.Properties.Resources.Add;
			_AddAdjustmentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			_AddAdjustmentButton.Name = "_AddAdjustmentButton";
			_AddAdjustmentButton.Size = new System.Drawing.Size(61, 22);
			_AddAdjustmentButton.Text = "添加";
			// 
			// _AddFilterMenu
			// 
			this._AddFilterMenu.Name = "_AddFilterMenu";
			this._AddFilterMenu.Size = new System.Drawing.Size(61, 4);
			this._AddFilterMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._AddFilterMenu_ItemClicked);
			// 
			// _TitleSizeThresholdBox
			// 
			this._TitleSizeThresholdBox.DecimalPlaces = 2;
			this._TitleSizeThresholdBox.Location = new System.Drawing.Point(98, 36);
			this._TitleSizeThresholdBox.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
			this._TitleSizeThresholdBox.Name = "_TitleSizeThresholdBox";
			this._TitleSizeThresholdBox.Size = new System.Drawing.Size(68, 21);
			this._TitleSizeThresholdBox.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "标题文本尺寸：";
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
			this._PageRangeBox.Size = new System.Drawing.Size(321, 21);
			this._PageRangeBox.TabIndex = 1;
			// 
			// _MergeAdjacentTitlesBox
			// 
			this._MergeAdjacentTitlesBox.AutoSize = true;
			this._MergeAdjacentTitlesBox.Location = new System.Drawing.Point(6, 84);
			this._MergeAdjacentTitlesBox.Name = "_MergeAdjacentTitlesBox";
			this._MergeAdjacentTitlesBox.Size = new System.Drawing.Size(132, 16);
			this._MergeAdjacentTitlesBox.TabIndex = 10;
			this._MergeAdjacentTitlesBox.Text = "合并连续出现的标题";
			this._MergeAdjacentTitlesBox.UseVisualStyleBackColor = true;
			// 
			// _MergeDifferentSizeTitlesBox
			// 
			this._MergeDifferentSizeTitlesBox.AutoSize = true;
			this._MergeDifferentSizeTitlesBox.Location = new System.Drawing.Point(6, 105);
			this._MergeDifferentSizeTitlesBox.Name = "_MergeDifferentSizeTitlesBox";
			this._MergeDifferentSizeTitlesBox.Size = new System.Drawing.Size(156, 16);
			this._MergeDifferentSizeTitlesBox.TabIndex = 14;
			this._MergeDifferentSizeTitlesBox.Text = "合并不同文本尺寸的标题";
			this._MergeDifferentSizeTitlesBox.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Location = new System.Drawing.Point(12, 93);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(463, 229);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this._FirstLineAsTitleBox);
			this.tabPage1.Controls.Add(this._IgnoreOverlappedTextBox);
			this.tabPage1.Controls.Add(this._CreateBookmarkForFirstPageBox);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this._WritingDirectionBox);
			this.tabPage1.Controls.Add(this._AutoHierarchicalArrangementBox);
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this._MaxDistanceBetweenLinesBox);
			this.tabPage1.Controls.Add(this._GoToPageTopLevelBox);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label9);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this._YOffsetBox);
			this.tabPage1.Controls.Add(this._MergeDifferentFontTitlesBox);
			this.tabPage1.Controls.Add(this._MergeDifferentSizeTitlesBox);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this._TitleSizeThresholdBox);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this._MergeAdjacentTitlesBox);
			this.tabPage1.Controls.Add(this._PageRangeBox);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage1.Size = new System.Drawing.Size(455, 203);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "标题识别";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _FirstLineAsTitleBox
			// 
			this._FirstLineAsTitleBox.AutoSize = true;
			this._FirstLineAsTitleBox.Location = new System.Drawing.Point(6, 170);
			this._FirstLineAsTitleBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._FirstLineAsTitleBox.Name = "_FirstLineAsTitleBox";
			this._FirstLineAsTitleBox.Size = new System.Drawing.Size(168, 16);
			this._FirstLineAsTitleBox.TabIndex = 20;
			this._FirstLineAsTitleBox.Text = "将每页第一行文本作为标题";
			this._FirstLineAsTitleBox.UseVisualStyleBackColor = true;
			// 
			// _IgnoreOverlappedTextBox
			// 
			this._IgnoreOverlappedTextBox.AutoSize = true;
			this._IgnoreOverlappedTextBox.Location = new System.Drawing.Point(6, 149);
			this._IgnoreOverlappedTextBox.Name = "_IgnoreOverlappedTextBox";
			this._IgnoreOverlappedTextBox.Size = new System.Drawing.Size(108, 16);
			this._IgnoreOverlappedTextBox.TabIndex = 18;
			this._IgnoreOverlappedTextBox.Text = "忽略重叠的文本";
			this._IgnoreOverlappedTextBox.UseVisualStyleBackColor = true;
			// 
			// _CreateBookmarkForFirstPageBox
			// 
			this._CreateBookmarkForFirstPageBox.AutoSize = true;
			this._CreateBookmarkForFirstPageBox.Location = new System.Drawing.Point(200, 143);
			this._CreateBookmarkForFirstPageBox.Name = "_CreateBookmarkForFirstPageBox";
			this._CreateBookmarkForFirstPageBox.Size = new System.Drawing.Size(132, 16);
			this._CreateBookmarkForFirstPageBox.TabIndex = 19;
			this._CreateBookmarkForFirstPageBox.Text = "文件名作为首页书签";
			this._CreateBookmarkForFirstPageBox.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(198, 91);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(113, 12);
			this.label6.TabIndex = 11;
			this.label6.Text = "合并连续标题不大于";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(198, 66);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 12);
			this.label5.TabIndex = 8;
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
			this._WritingDirectionBox.Location = new System.Drawing.Point(293, 63);
			this._WritingDirectionBox.Name = "_WritingDirectionBox";
			this._WritingDirectionBox.Size = new System.Drawing.Size(76, 20);
			this._WritingDirectionBox.TabIndex = 9;
			// 
			// _AutoHierarchicalArrangementBox
			// 
			this._AutoHierarchicalArrangementBox.AutoSize = true;
			this._AutoHierarchicalArrangementBox.Location = new System.Drawing.Point(6, 63);
			this._AutoHierarchicalArrangementBox.Name = "_AutoHierarchicalArrangementBox";
			this._AutoHierarchicalArrangementBox.Size = new System.Drawing.Size(120, 16);
			this._AutoHierarchicalArrangementBox.TabIndex = 7;
			this._AutoHierarchicalArrangementBox.Text = "自动组织标题层次";
			this._AutoHierarchicalArrangementBox.UseVisualStyleBackColor = true;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(250, 118);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(101, 12);
			this.label11.TabIndex = 17;
			this.label11.Text = "层标题定位到页首";
			// 
			// _MaxDistanceBetweenLinesBox
			// 
			this._MaxDistanceBetweenLinesBox.DecimalPlaces = 2;
			this._MaxDistanceBetweenLinesBox.Location = new System.Drawing.Point(317, 89);
			this._MaxDistanceBetweenLinesBox.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this._MaxDistanceBetweenLinesBox.Name = "_MaxDistanceBetweenLinesBox";
			this._MaxDistanceBetweenLinesBox.Size = new System.Drawing.Size(55, 21);
			this._MaxDistanceBetweenLinesBox.TabIndex = 12;
			// 
			// _GoToPageTopLevelBox
			// 
			this._GoToPageTopLevelBox.Location = new System.Drawing.Point(200, 116);
			this._GoToPageTopLevelBox.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this._GoToPageTopLevelBox.Name = "_GoToPageTopLevelBox";
			this._GoToPageTopLevelBox.Size = new System.Drawing.Size(41, 21);
			this._GoToPageTopLevelBox.TabIndex = 16;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(378, 91);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 12);
			this.label7.TabIndex = 13;
			this.label7.Text = "倍行距";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(359, 38);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(41, 12);
			this.label9.TabIndex = 6;
			this.label9.Text = "倍行距";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(174, 38);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(113, 12);
			this.label4.TabIndex = 4;
			this.label4.Text = "定位位置向上偏移：";
			// 
			// _YOffsetBox
			// 
			this._YOffsetBox.DecimalPlaces = 2;
			this._YOffsetBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
			this._YOffsetBox.Location = new System.Drawing.Point(293, 36);
			this._YOffsetBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._YOffsetBox.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this._YOffsetBox.Name = "_YOffsetBox";
			this._YOffsetBox.Size = new System.Drawing.Size(60, 21);
			this._YOffsetBox.TabIndex = 5;
			// 
			// _MergeDifferentFontTitlesBox
			// 
			this._MergeDifferentFontTitlesBox.AutoSize = true;
			this._MergeDifferentFontTitlesBox.Location = new System.Drawing.Point(6, 127);
			this._MergeDifferentFontTitlesBox.Name = "_MergeDifferentFontTitlesBox";
			this._MergeDifferentFontTitlesBox.Size = new System.Drawing.Size(132, 16);
			this._MergeDifferentFontTitlesBox.TabIndex = 15;
			this._MergeDifferentFontTitlesBox.Text = "合并不同字体的标题";
			this._MergeDifferentFontTitlesBox.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this._IgnoreNumericTitleBox);
			this.tabPage2.Controls.Add(this._IgnoreSingleCharacterTitleBox);
			this.tabPage2.Controls.Add(this._ClearTextFiltersButton);
			this.tabPage2.Controls.Add(this._IgnorePatternsBox);
			this.tabPage2.Controls.Add(this.label10);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage2.Size = new System.Drawing.Size(455, 203);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "文本过滤";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// _IgnoreNumericTitleBox
			// 
			this._IgnoreNumericTitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._IgnoreNumericTitleBox.AutoSize = true;
			this._IgnoreNumericTitleBox.Location = new System.Drawing.Point(170, 171);
			this._IgnoreNumericTitleBox.Name = "_IgnoreNumericTitleBox";
			this._IgnoreNumericTitleBox.Size = new System.Drawing.Size(132, 16);
			this._IgnoreNumericTitleBox.TabIndex = 22;
			this._IgnoreNumericTitleBox.Text = "忽略只有数字的标题";
			this._IgnoreNumericTitleBox.UseVisualStyleBackColor = true;
			// 
			// _IgnoreSingleCharacterTitleBox
			// 
			this._IgnoreSingleCharacterTitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._IgnoreSingleCharacterTitleBox.AutoSize = true;
			this._IgnoreSingleCharacterTitleBox.Location = new System.Drawing.Point(8, 171);
			this._IgnoreSingleCharacterTitleBox.Name = "_IgnoreSingleCharacterTitleBox";
			this._IgnoreSingleCharacterTitleBox.Size = new System.Drawing.Size(156, 16);
			this._IgnoreSingleCharacterTitleBox.TabIndex = 21;
			this._IgnoreSingleCharacterTitleBox.Text = "忽略只有一个字符的标题";
			this._IgnoreSingleCharacterTitleBox.UseVisualStyleBackColor = true;
			// 
			// _ClearTextFiltersButton
			// 
			this._ClearTextFiltersButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ClearTextFiltersButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._ClearTextFiltersButton.Location = new System.Drawing.Point(369, 6);
			this._ClearTextFiltersButton.Name = "_ClearTextFiltersButton";
			this._ClearTextFiltersButton.Size = new System.Drawing.Size(80, 23);
			this._ClearTextFiltersButton.TabIndex = 2;
			this._ClearTextFiltersButton.Text = "清空列表";
			this._ClearTextFiltersButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ClearTextFiltersButton.UseVisualStyleBackColor = true;
			this._ClearTextFiltersButton.Click += new System.EventHandler(this.ControlEvent);
			// 
			// _IgnorePatternsBox
			// 
			this._IgnorePatternsBox.AllowUserToResizeRows = false;
			this._IgnorePatternsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._IgnorePatternsBox.BackgroundColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this._IgnorePatternsBox.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this._IgnorePatternsBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._IgnorePatternsBox.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._PatternColumn,
            this._MatchCaseColumn,
            this._FullMatchColumn,
            this._PatternTypeColumn,
            this._RemovePatternColumn});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this._IgnorePatternsBox.DefaultCellStyle = dataGridViewCellStyle2;
			this._IgnorePatternsBox.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this._IgnorePatternsBox.Location = new System.Drawing.Point(8, 35);
			this._IgnorePatternsBox.Name = "_IgnorePatternsBox";
			this._IgnorePatternsBox.RowHeadersVisible = false;
			this._IgnorePatternsBox.RowTemplate.Height = 23;
			this._IgnorePatternsBox.Size = new System.Drawing.Size(441, 130);
			this._IgnorePatternsBox.TabIndex = 1;
			this._IgnorePatternsBox.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._IgnorePatternsBox_CellContentClick);
			// 
			// _PatternColumn
			// 
			this._PatternColumn.Frozen = true;
			this._PatternColumn.HeaderText = "忽略内容";
			this._PatternColumn.MinimumWidth = 50;
			this._PatternColumn.Name = "_PatternColumn";
			this._PatternColumn.ToolTipText = "忽略匹配此内容的标题";
			this._PatternColumn.Width = 150;
			// 
			// _MatchCaseColumn
			// 
			this._MatchCaseColumn.HeaderText = "区分大小写";
			this._MatchCaseColumn.MinimumWidth = 70;
			this._MatchCaseColumn.Name = "_MatchCaseColumn";
			this._MatchCaseColumn.ToolTipText = "是否区分大小写";
			this._MatchCaseColumn.Width = 70;
			// 
			// _FullMatchColumn
			// 
			this._FullMatchColumn.HeaderText = "匹配全标题";
			this._FullMatchColumn.MinimumWidth = 70;
			this._FullMatchColumn.Name = "_FullMatchColumn";
			this._FullMatchColumn.ToolTipText = "是否匹配整个标题";
			this._FullMatchColumn.Width = 70;
			// 
			// _PatternTypeColumn
			// 
			this._PatternTypeColumn.HeaderText = "正则表达式";
			this._PatternTypeColumn.MinimumWidth = 70;
			this._PatternTypeColumn.Name = "_PatternTypeColumn";
			this._PatternTypeColumn.ToolTipText = "是否使用正则表达式";
			this._PatternTypeColumn.Width = 70;
			// 
			// _RemovePatternColumn
			// 
			this._RemovePatternColumn.HeaderText = "删除";
			this._RemovePatternColumn.MinimumWidth = 35;
			this._RemovePatternColumn.Name = "_RemovePatternColumn";
			this._RemovePatternColumn.Text = "删除";
			this._RemovePatternColumn.ToolTipText = "删除此忽略模板";
			this._RemovePatternColumn.UseColumnTextForLinkValue = true;
			this._RemovePatternColumn.Width = 35;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(3, 11);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(149, 12);
			this.label10.TabIndex = 0;
			this.label10.Text = "忽略匹配以下内容的文本：";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this._FilterToolbar);
			this.tabPage3.Controls.Add(this._LevelAdjustmentBox);
			this.tabPage3.Controls.Add(this.label12);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage3.Size = new System.Drawing.Size(455, 203);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "高级筛选处理";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// _FilterToolbar
			// 
			this._FilterToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._FilterToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            _AddAdjustmentButton,
            this._AddFilterFromInfoFileButton,
            this._DeleteAdjustmentButton,
            this.toolStripSeparator1,
            this._CopyFilterButton,
            this._PasteButton});
			this._FilterToolbar.Location = new System.Drawing.Point(3, 3);
			this._FilterToolbar.Name = "_FilterToolbar";
			this._FilterToolbar.Size = new System.Drawing.Size(449, 25);
			this._FilterToolbar.TabIndex = 0;
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(68, 22);
			this.toolStripLabel1.Text = "筛选条件：";
			// 
			// _AddFilterFromInfoFileButton
			// 
			this._AddFilterFromInfoFileButton.Image = global::PDFPatcher.Properties.Resources.BookmarkFile;
			this._AddFilterFromInfoFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFilterFromInfoFileButton.Name = "_AddFilterFromInfoFileButton";
			this._AddFilterFromInfoFileButton.Size = new System.Drawing.Size(112, 22);
			this._AddFilterFromInfoFileButton.Text = "从信息文件添加";
			this._AddFilterFromInfoFileButton.Click += new System.EventHandler(this.ControlEvent);
			// 
			// _DeleteAdjustmentButton
			// 
			this._DeleteAdjustmentButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._DeleteAdjustmentButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._DeleteAdjustmentButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._DeleteAdjustmentButton.Name = "_DeleteAdjustmentButton";
			this._DeleteAdjustmentButton.Size = new System.Drawing.Size(23, 22);
			this._DeleteAdjustmentButton.Text = "删除";
			this._DeleteAdjustmentButton.Click += new System.EventHandler(this.ControlEvent);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// _CopyFilterButton
			// 
			this._CopyFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._CopyFilterButton.Image = global::PDFPatcher.Properties.Resources.Copy;
			this._CopyFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._CopyFilterButton.Name = "_CopyFilterButton";
			this._CopyFilterButton.Size = new System.Drawing.Size(23, 22);
			this._CopyFilterButton.Text = "复制";
			this._CopyFilterButton.Click += new System.EventHandler(this.ControlEvent);
			// 
			// _PasteButton
			// 
			this._PasteButton.Image = global::PDFPatcher.Properties.Resources.Paste;
			this._PasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._PasteButton.Name = "_PasteButton";
			this._PasteButton.Size = new System.Drawing.Size(52, 22);
			this._PasteButton.Text = "粘贴";
			this._PasteButton.Click += new System.EventHandler(this.ControlEvent);
			// 
			// _LevelAdjustmentBox
			// 
			this._LevelAdjustmentBox.AllColumns.Add(this._AdvancedFilterColumn);
			this._LevelAdjustmentBox.AllColumns.Add(this._AdjustmentLevelColumn);
			this._LevelAdjustmentBox.AllColumns.Add(this._RelativeAdjustmentColumn);
			this._LevelAdjustmentBox.AllColumns.Add(this._FilterBeforeMergeColumn);
			this._LevelAdjustmentBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._LevelAdjustmentBox.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._LevelAdjustmentBox.CellEditUseWholeCell = false;
			this._LevelAdjustmentBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._AdvancedFilterColumn,
            this._AdjustmentLevelColumn,
            this._RelativeAdjustmentColumn,
            this._FilterBeforeMergeColumn});
			this._LevelAdjustmentBox.Cursor = System.Windows.Forms.Cursors.Default;
			this._LevelAdjustmentBox.GridLines = true;
			this._LevelAdjustmentBox.HideSelection = false;
			this._LevelAdjustmentBox.IsSimpleDragSource = true;
			this._LevelAdjustmentBox.IsSimpleDropSink = true;
			this._LevelAdjustmentBox.Location = new System.Drawing.Point(6, 50);
			this._LevelAdjustmentBox.Name = "_LevelAdjustmentBox";
			this._LevelAdjustmentBox.ShowGroups = false;
			this._LevelAdjustmentBox.Size = new System.Drawing.Size(443, 137);
			this._LevelAdjustmentBox.TabIndex = 2;
			this._LevelAdjustmentBox.UseCompatibleStateImageBehavior = false;
			this._LevelAdjustmentBox.View = System.Windows.Forms.View.Details;
			this._LevelAdjustmentBox.ItemActivate += new System.EventHandler(this._LevelAdjustmentBox_ItemActivate);
			// 
			// _AdvancedFilterColumn
			// 
			this._AdvancedFilterColumn.IsEditable = false;
			this._AdvancedFilterColumn.Text = "筛选条件";
			this._AdvancedFilterColumn.Width = 273;
			// 
			// _AdjustmentLevelColumn
			// 
			this._AdjustmentLevelColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._AdjustmentLevelColumn.Text = "调整级别";
			// 
			// _RelativeAdjustmentColumn
			// 
			this._RelativeAdjustmentColumn.CheckBoxes = true;
			this._RelativeAdjustmentColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._RelativeAdjustmentColumn.Text = "相对调整";
			this._RelativeAdjustmentColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// _FilterBeforeMergeColumn
			// 
			this._FilterBeforeMergeColumn.CheckBoxes = true;
			this._FilterBeforeMergeColumn.Text = "合并文本前筛选";
			this._FilterBeforeMergeColumn.Width = 100;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(3, 35);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(251, 12);
			this.label12.TabIndex = 1;
			this.label12.Text = "调整匹配字体的尺寸级别（级别为0时忽略）：";
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this._ExportTextCoordinateBox);
			this.tabPage5.Controls.Add(this._ShowAllFontsBox);
			this.tabPage5.Controls.Add(this._DisplayFontStatisticsBox);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage5.Size = new System.Drawing.Size(455, 203);
			this.tabPage5.TabIndex = 4;
			this.tabPage5.Text = "其它选项";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// _ExportTextCoordinateBox
			// 
			this._ExportTextCoordinateBox.AutoSize = true;
			this._ExportTextCoordinateBox.Location = new System.Drawing.Point(6, 50);
			this._ExportTextCoordinateBox.Name = "_ExportTextCoordinateBox";
			this._ExportTextCoordinateBox.Size = new System.Drawing.Size(120, 16);
			this._ExportTextCoordinateBox.TabIndex = 2;
			this._ExportTextCoordinateBox.Text = "导出文本位置信息";
			this._ExportTextCoordinateBox.UseVisualStyleBackColor = true;
			// 
			// _ShowAllFontsBox
			// 
			this._ShowAllFontsBox.AutoSize = true;
			this._ShowAllFontsBox.Location = new System.Drawing.Point(6, 28);
			this._ShowAllFontsBox.Name = "_ShowAllFontsBox";
			this._ShowAllFontsBox.Size = new System.Drawing.Size(120, 16);
			this._ShowAllFontsBox.TabIndex = 1;
			this._ShowAllFontsBox.Text = "列出被忽略的字体";
			this._ShowAllFontsBox.UseVisualStyleBackColor = true;
			// 
			// _DisplayFontStatisticsBox
			// 
			this._DisplayFontStatisticsBox.AutoSize = true;
			this._DisplayFontStatisticsBox.Checked = true;
			this._DisplayFontStatisticsBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._DisplayFontStatisticsBox.Location = new System.Drawing.Point(6, 6);
			this._DisplayFontStatisticsBox.Name = "_DisplayFontStatisticsBox";
			this._DisplayFontStatisticsBox.Size = new System.Drawing.Size(192, 16);
			this._DisplayFontStatisticsBox.TabIndex = 0;
			this._DisplayFontStatisticsBox.Text = "完成识别后统计用于标题的字体";
			this._DisplayFontStatisticsBox.UseVisualStyleBackColor = true;
			// 
			// _BookmarkControl
			// 
			this._BookmarkControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._BookmarkControl.LabelText = "P&DF 信息文件：";
			this._BookmarkControl.Location = new System.Drawing.Point(12, 33);
			this._BookmarkControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._BookmarkControl.Name = "_BookmarkControl";
			this._BookmarkControl.Size = new System.Drawing.Size(463, 25);
			this._BookmarkControl.TabIndex = 2;
			this._BookmarkControl.UseForBookmarkExport = true;
			// 
			// _SourceFileControl
			// 
			this._SourceFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SourceFileControl.Location = new System.Drawing.Point(12, 3);
			this._SourceFileControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._SourceFileControl.Name = "_SourceFileControl";
			this._SourceFileControl.Size = new System.Drawing.Size(463, 24);
			this._SourceFileControl.TabIndex = 1;
			// 
			// _ExportBookmarkButton
			// 
			this._ExportBookmarkButton.AlternativeFocusBorderColor = System.Drawing.SystemColors.Highlight;
			this._ExportBookmarkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ExportBookmarkButton.AnimateGlow = true;
			this._ExportBookmarkButton.BackColor = System.Drawing.SystemColors.Highlight;
			this._ExportBookmarkButton.CornerRadius = 3;
			this._ExportBookmarkButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ExportBookmarkButton.GlowColor = System.Drawing.Color.White;
			this._ExportBookmarkButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._ExportBookmarkButton.InnerBorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this._ExportBookmarkButton.Location = new System.Drawing.Point(349, 63);
			this._ExportBookmarkButton.Name = "_ExportBookmarkButton";
			this._ExportBookmarkButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ExportBookmarkButton.ShowFocusBorder = true;
			this._ExportBookmarkButton.Size = new System.Drawing.Size(123, 29);
			this._ExportBookmarkButton.TabIndex = 15;
			this._ExportBookmarkButton.Text = " 生成书签(&S)";
			this._ExportBookmarkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExportBookmarkButton.Click += new System.EventHandler(this._ExportBookmarkButton_Click);
			// 
			// AutoBookmarkControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._ExportBookmarkButton);
			this.Controls.Add(this._SourceFileControl);
			this.Controls.Add(this._BookmarkControl);
			this.Controls.Add(this.tabControl1);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "AutoBookmarkControl";
			this.Size = new System.Drawing.Size(487, 333);
			((System.ComponentModel.ISupportInitialize)(this._TitleSizeThresholdBox)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._MaxDistanceBetweenLinesBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._GoToPageTopLevelBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._YOffsetBox)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._IgnorePatternsBox)).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this._FilterToolbar.ResumeLayout(false);
			this._FilterToolbar.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._LevelAdjustmentBox)).EndInit();
			this.tabPage5.ResumeLayout(false);
			this.tabPage5.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private SourceFileControl _SourceFileControl;
		private BookmarkControl _BookmarkControl;
		private System.Windows.Forms.NumericUpDown _TitleSizeThresholdBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _PageRangeBox;
		private System.Windows.Forms.CheckBox _MergeAdjacentTitlesBox;
		private System.Windows.Forms.CheckBox _MergeDifferentSizeTitlesBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown _GoToPageTopLevelBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown _YOffsetBox;
		private System.Windows.Forms.DataGridView _IgnorePatternsBox;
		private System.Windows.Forms.DataGridViewTextBoxColumn _PatternColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn _MatchCaseColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn _FullMatchColumn;
		private System.Windows.Forms.DataGridViewCheckBoxColumn _PatternTypeColumn;
		private System.Windows.Forms.DataGridViewLinkColumn _RemovePatternColumn;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Button _ClearTextFiltersButton;
		private System.Windows.Forms.CheckBox _AutoHierarchicalArrangementBox;
		private BrightIdeasSoftware.ObjectListView _LevelAdjustmentBox;
		private BrightIdeasSoftware.OLVColumn _AdvancedFilterColumn;
		private BrightIdeasSoftware.OLVColumn _AdjustmentLevelColumn;
		private BrightIdeasSoftware.OLVColumn _RelativeAdjustmentColumn;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox _WritingDirectionBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TabPage tabPage5;
		private System.Windows.Forms.CheckBox _ShowAllFontsBox;
		private System.Windows.Forms.CheckBox _DisplayFontStatisticsBox;
		private System.Windows.Forms.NumericUpDown _MaxDistanceBetweenLinesBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox _ExportTextCoordinateBox;
		private System.Windows.Forms.ContextMenuStrip _AddFilterMenu;
		private System.Windows.Forms.ToolStrip _FilterToolbar;
		private System.Windows.Forms.ToolStripButton _DeleteAdjustmentButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton _CopyFilterButton;
		private System.Windows.Forms.ToolStripButton _PasteButton;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripButton _AddFilterFromInfoFileButton;
		private System.Windows.Forms.CheckBox _CreateBookmarkForFirstPageBox;
		private System.Windows.Forms.CheckBox _MergeDifferentFontTitlesBox;
		private System.Windows.Forms.CheckBox _IgnoreOverlappedTextBox;
		private System.Windows.Forms.CheckBox _IgnoreNumericTitleBox;
		private System.Windows.Forms.CheckBox _IgnoreSingleCharacterTitleBox;
		private BrightIdeasSoftware.OLVColumn _FilterBeforeMergeColumn;
		private EnhancedGlassButton.GlassButton _ExportBookmarkButton;
        private System.Windows.Forms.CheckBox _FirstLineAsTitleBox;

	}
}
