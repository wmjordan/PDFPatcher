namespace PDFPatcher.Functions
{
	partial class PatcherControl
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
			System.Windows.Forms.ToolStrip _MainToolbar;
			System.Windows.Forms.ToolStripDropDownButton _Sort;
			System.Windows.Forms.ToolStripButton _Delete;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripMenuItem _SelectAll;
			System.Windows.Forms.ToolStripMenuItem _InvertSelect;
			System.Windows.Forms.ToolStripMenuItem _SelectNone;
			System.Windows.Forms.ToolStripMenuItem _Copy;
			System.Windows.Forms.ToolStripMenuItem _RefreshInfo;
			this._AddFilesButton = new System.Windows.Forms.ToolStripSplitButton();
			this._RecentFileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._SortMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._SortByNaturalNumberItem = new System.Windows.Forms.ToolStripMenuItem();
			this._SortByAlphaItem = new System.Windows.Forms.ToolStripMenuItem();
			this._RefreshInfoButton = new System.Windows.Forms.ToolStripSplitButton();
			this._RefreshInfoMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._SelectionMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._SelectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._ItemList = new BrightIdeasSoftware.ObjectListView();
			this._NameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._PageCountColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._TitleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._AuthorColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._SubjectColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._KeywordsColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._FolderColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItemListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._FileTypeList = new System.Windows.Forms.ImageList(this.components);
			this._OpenPdfBox = new System.Windows.Forms.OpenFileDialog();
			this._AutoClearListBox = new System.Windows.Forms.CheckBox();
			this._AddDocumentWorker = new System.ComponentModel.BackgroundWorker();
			this._TargetPdfFile = new PDFPatcher.TargetFileControl();
			this._ActionsBox = new BrightIdeasSoftware.ObjectListView();
			this._ActionNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItemActionsContainerBox = new System.Windows.Forms.SplitContainer();
			this._ConfigButton = new System.Windows.Forms.Button();
			this._ImportButton = new EnhancedGlassButton.GlassButton();
			this._FileTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			_MainToolbar = new System.Windows.Forms.ToolStrip();
			_Sort = new System.Windows.Forms.ToolStripDropDownButton();
			_Delete = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
			_InvertSelect = new System.Windows.Forms.ToolStripMenuItem();
			_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
			_Copy = new System.Windows.Forms.ToolStripMenuItem();
			_RefreshInfo = new System.Windows.Forms.ToolStripMenuItem();
			_MainToolbar.SuspendLayout();
			this._SortMenu.SuspendLayout();
			this._SelectionMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._ItemList)).BeginInit();
			this._ItemListMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._ActionsBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._ItemActionsContainerBox)).BeginInit();
			this._ItemActionsContainerBox.Panel1.SuspendLayout();
			this._ItemActionsContainerBox.Panel2.SuspendLayout();
			this._ItemActionsContainerBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _MainToolbar
			// 
			_MainToolbar.AutoSize = false;
			_MainToolbar.Dock = System.Windows.Forms.DockStyle.None;
			_MainToolbar.GripMargin = new System.Windows.Forms.Padding(0);
			_MainToolbar.ImageScalingSize = new System.Drawing.Size(24, 24);
			_MainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._AddFilesButton,
            _Sort,
            _Delete,
            toolStripSeparator2,
            this._RefreshInfoButton});
			_MainToolbar.Location = new System.Drawing.Point(0, 0);
			_MainToolbar.Name = "_MainToolbar";
			_MainToolbar.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			_MainToolbar.Size = new System.Drawing.Size(567, 38);
			_MainToolbar.TabIndex = 0;
			_MainToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _AddFilesButton
			// 
			this._AddFilesButton.DropDown = this._RecentFileMenu;
			this._AddFilesButton.Image = global::PDFPatcher.Properties.Resources.Add;
			this._AddFilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFilesButton.Name = "_AddFilesButton";
			this._AddFilesButton.Size = new System.Drawing.Size(149, 33);
			this._AddFilesButton.Text = "添加文件(&T)";
			this._AddFilesButton.ToolTipText = "添加需要合并的文件到处理列表";
			// 
			// _RecentFileMenu
			// 
			this._RecentFileMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._RecentFileMenu.Name = "_RecentFileMenu";
			this._RecentFileMenu.OwnerItem = this._AddFilesButton;
			this._RecentFileMenu.ShowImageMargin = false;
			this._RecentFileMenu.Size = new System.Drawing.Size(36, 4);
			// 
			// _Sort
			// 
			_Sort.DropDown = this._SortMenu;
			_Sort.Image = global::PDFPatcher.Properties.Resources.Sort;
			_Sort.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Sort.Name = "_Sort";
			_Sort.Size = new System.Drawing.Size(88, 33);
			_Sort.Text = "排序";
			_Sort.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._SortMenu_ItemClicked);
			// 
			// _SortMenu
			// 
			this._SortMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._SortMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._SortByNaturalNumberItem,
            this._SortByAlphaItem});
			this._SortMenu.Name = "_SortMenu";
			this._SortMenu.OwnerItem = _Sort;
			this._SortMenu.Size = new System.Drawing.Size(299, 68);
			this._SortMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._SortMenu_ItemClicked);
			// 
			// _SortByNaturalNumberItem
			// 
			this._SortByNaturalNumberItem.Image = global::PDFPatcher.Properties.Resources.NaturalSort;
			this._SortByNaturalNumberItem.Name = "_SortByNaturalNumberItem";
			this._SortByNaturalNumberItem.Size = new System.Drawing.Size(298, 32);
			this._SortByNaturalNumberItem.Text = "按数值和字母顺序排序(&M)";
			// 
			// _SortByAlphaItem
			// 
			this._SortByAlphaItem.Image = global::PDFPatcher.Properties.Resources.AlphabeticSort;
			this._SortByAlphaItem.Name = "_SortByAlphaItem";
			this._SortByAlphaItem.Size = new System.Drawing.Size(298, 32);
			this._SortByAlphaItem.Text = "按字母顺序排序(&Z)";
			// 
			// _Delete
			// 
			_Delete.Image = global::PDFPatcher.Properties.Resources.Delete;
			_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Delete.Name = "_Delete";
			_Delete.Size = new System.Drawing.Size(110, 33);
			_Delete.Text = "删除文件";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 38);
			// 
			// _RefreshInfoButton
			// 
			this._RefreshInfoButton.Image = global::PDFPatcher.Properties.Resources.Refresh;
			this._RefreshInfoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._RefreshInfoButton.Name = "_RefreshInfoButton";
			this._RefreshInfoButton.Size = new System.Drawing.Size(163, 33);
			this._RefreshInfoButton.Text = "刷新文档属性";
			// 
			// _SelectAll
			// 
			_SelectAll.Image = global::PDFPatcher.Properties.Resources.SelectAll;
			_SelectAll.Name = "_SelectAll";
			_SelectAll.Size = new System.Drawing.Size(160, 32);
			_SelectAll.Text = "全部选中";
			// 
			// _InvertSelect
			// 
			_InvertSelect.Name = "_InvertSelect";
			_InvertSelect.Size = new System.Drawing.Size(160, 32);
			_InvertSelect.Text = "反转选择";
			// 
			// _SelectNone
			// 
			_SelectNone.Name = "_SelectNone";
			_SelectNone.Size = new System.Drawing.Size(160, 32);
			_SelectNone.Text = "取消选择";
			// 
			// _Copy
			// 
			_Copy.Image = global::PDFPatcher.Properties.Resources.Copy;
			_Copy.Name = "_Copy";
			_Copy.Size = new System.Drawing.Size(196, 32);
			_Copy.Text = "复制列表内容";
			// 
			// _RefreshInfo
			// 
			_RefreshInfo.DropDown = this._RefreshInfoMenu;
			_RefreshInfo.Image = global::PDFPatcher.Properties.Resources.Refresh;
			_RefreshInfo.Name = "_RefreshInfo";
			_RefreshInfo.Size = new System.Drawing.Size(196, 32);
			_RefreshInfo.Text = "刷新文档属性";
			// 
			// _RefreshInfoMenu
			// 
			this._RefreshInfoMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._RefreshInfoMenu.Name = "_RefreshInfoMenu";
			this._RefreshInfoMenu.OwnerItem = _RefreshInfo;
			this._RefreshInfoMenu.Size = new System.Drawing.Size(61, 4);
			// 
			// _SelectionMenu
			// 
			this._SelectionMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._SelectionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _SelectAll,
            _InvertSelect,
            _SelectNone});
			this._SelectionMenu.Name = "_SelectionMenu";
			this._SelectionMenu.OwnerItem = this._SelectionMenuItem;
			this._SelectionMenu.Size = new System.Drawing.Size(161, 100);
			this._SelectionMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _SelectionMenuItem
			// 
			this._SelectionMenuItem.DropDown = this._SelectionMenu;
			this._SelectionMenuItem.Image = global::PDFPatcher.Properties.Resources.SelectItem;
			this._SelectionMenuItem.Name = "_SelectionMenuItem";
			this._SelectionMenuItem.Size = new System.Drawing.Size(196, 32);
			this._SelectionMenuItem.Text = "选择文件";
			// 
			// _ItemList
			// 
			this._ItemList.AllColumns.Add(this._NameColumn);
			this._ItemList.AllColumns.Add(this._PageCountColumn);
			this._ItemList.AllColumns.Add(this._TitleColumn);
			this._ItemList.AllColumns.Add(this._AuthorColumn);
			this._ItemList.AllColumns.Add(this._SubjectColumn);
			this._ItemList.AllColumns.Add(this._KeywordsColumn);
			this._ItemList.AllColumns.Add(this._FolderColumn);
			this._ItemList.AllColumns.Add(this._FileTimeColumn);
			this._ItemList.AllowDrop = true;
			this._ItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ItemList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._ItemList.CellEditUseWholeCell = false;
			this._ItemList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._NameColumn,
            this._PageCountColumn,
            this._TitleColumn,
            this._AuthorColumn,
            this._SubjectColumn,
            this._KeywordsColumn,
            this._FolderColumn,
            this._FileTimeColumn});
			this._ItemList.ContextMenuStrip = this._ItemListMenu;
			this._ItemList.Cursor = System.Windows.Forms.Cursors.Default;
			this._ItemList.GridLines = true;
			this._ItemList.HideSelection = false;
			this._ItemList.Location = new System.Drawing.Point(4, 4);
			this._ItemList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ItemList.Name = "_ItemList";
			this._ItemList.ShowGroups = false;
			this._ItemList.Size = new System.Drawing.Size(814, 336);
			this._ItemList.SmallImageList = this._FileTypeList;
			this._ItemList.TabIndex = 0;
			this._ItemList.UseCompatibleStateImageBehavior = false;
			this._ItemList.View = System.Windows.Forms.View.Details;
			this._ItemList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this._ImageList_ColumnClick);
			// 
			// _NameColumn
			// 
			this._NameColumn.Text = "源文件名";
			this._NameColumn.Width = 149;
			// 
			// _PageCountColumn
			// 
			this._PageCountColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._PageCountColumn.IsEditable = false;
			this._PageCountColumn.Text = "页数";
			this._PageCountColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._PageCountColumn.Width = 50;
			// 
			// _TitleColumn
			// 
			this._TitleColumn.Text = "标题";
			// 
			// _AuthorColumn
			// 
			this._AuthorColumn.Text = "作者";
			// 
			// _SubjectColumn
			// 
			this._SubjectColumn.Text = "主题";
			// 
			// _KeywordsColumn
			// 
			this._KeywordsColumn.Text = "关键词";
			this._KeywordsColumn.Width = 73;
			// 
			// _FolderColumn
			// 
			this._FolderColumn.IsEditable = false;
			this._FolderColumn.Text = "文件夹";
			this._FolderColumn.Width = 96;
			// 
			// _ItemListMenu
			// 
			this._ItemListMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._ItemListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _Copy,
            _RefreshInfo,
            this._SelectionMenuItem});
			this._ItemListMenu.Name = "_ItemListMenu";
			this._ItemListMenu.Size = new System.Drawing.Size(197, 100);
			this._ItemListMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _FileTypeList
			// 
			this._FileTypeList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._FileTypeList.ImageSize = new System.Drawing.Size(16, 16);
			this._FileTypeList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _OpenPdfBox
			// 
			this._OpenPdfBox.DefaultExt = "pdf";
			this._OpenPdfBox.Filter = "PDF 文件（*.pdf）|*.pdf";
			this._OpenPdfBox.Multiselect = true;
			this._OpenPdfBox.Title = "选择需要处理的 PDF 文件";
			// 
			// _AutoClearListBox
			// 
			this._AutoClearListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._AutoClearListBox.AutoSize = true;
			this._AutoClearListBox.Checked = true;
			this._AutoClearListBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this._AutoClearListBox.Location = new System.Drawing.Point(653, 14);
			this._AutoClearListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._AutoClearListBox.Name = "_AutoClearListBox";
			this._AutoClearListBox.Size = new System.Drawing.Size(196, 22);
			this._AutoClearListBox.TabIndex = 4;
			this._AutoClearListBox.Text = "添加文件前清空列表";
			this._AutoClearListBox.UseVisualStyleBackColor = true;
			// 
			// _AddDocumentWorker
			// 
			this._AddDocumentWorker.WorkerReportsProgress = true;
			this._AddDocumentWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._AddDocumentWorker_DoWork);
			this._AddDocumentWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this._AddDocumentWorker_ProgressChanged);
			this._AddDocumentWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._AddDocumentWorker_RunWorkerCompleted);
			// 
			// _TargetPdfFile
			// 
			this._TargetPdfFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetPdfFile.Location = new System.Drawing.Point(20, 398);
			this._TargetPdfFile.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._TargetPdfFile.Name = "_TargetPdfFile";
			this._TargetPdfFile.Size = new System.Drawing.Size(830, 39);
			this._TargetPdfFile.TabIndex = 7;
			// 
			// _ActionsBox
			// 
			this._ActionsBox.AllColumns.Add(this._ActionNameColumn);
			this._ActionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ActionsBox.CellEditUseWholeCell = false;
			this._ActionsBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._ActionNameColumn});
			this._ActionsBox.Cursor = System.Windows.Forms.Cursors.Default;
			this._ActionsBox.FullRowSelect = true;
			this._ActionsBox.GridLines = true;
			this._ActionsBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._ActionsBox.HideSelection = false;
			this._ActionsBox.Location = new System.Drawing.Point(8, 4);
			this._ActionsBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ActionsBox.Name = "_ActionsBox";
			this._ActionsBox.RowHeight = 18;
			this._ActionsBox.ShowGroups = false;
			this._ActionsBox.Size = new System.Drawing.Size(187, 262);
			this._ActionsBox.TabIndex = 18;
			this._ActionsBox.UseCompatibleStateImageBehavior = false;
			this._ActionsBox.View = System.Windows.Forms.View.Details;
			// 
			// _ActionNameColumn
			// 
			this._ActionNameColumn.Text = "补丁操作";
			this._ActionNameColumn.Width = 120;
			// 
			// _ItemActionsContainerBox
			// 
			this._ItemActionsContainerBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ItemActionsContainerBox.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._ItemActionsContainerBox.Location = new System.Drawing.Point(20, 42);
			this._ItemActionsContainerBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ItemActionsContainerBox.Name = "_ItemActionsContainerBox";
			// 
			// _ItemActionsContainerBox.Panel1
			// 
			this._ItemActionsContainerBox.Panel1.Controls.Add(this._ItemList);
			// 
			// _ItemActionsContainerBox.Panel2
			// 
			this._ItemActionsContainerBox.Panel2.Controls.Add(this._ActionsBox);
			this._ItemActionsContainerBox.Panel2Collapsed = true;
			this._ItemActionsContainerBox.Size = new System.Drawing.Size(825, 346);
			this._ItemActionsContainerBox.SplitterDistance = 412;
			this._ItemActionsContainerBox.SplitterWidth = 6;
			this._ItemActionsContainerBox.TabIndex = 5;
			// 
			// _ConfigButton
			// 
			this._ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ConfigButton.Image = global::PDFPatcher.Properties.Resources.PdfOptions;
			this._ConfigButton.Location = new System.Drawing.Point(380, 446);
			this._ConfigButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ConfigButton.Name = "_ConfigButton";
			this._ConfigButton.Size = new System.Drawing.Size(272, 34);
			this._ConfigButton.TabIndex = 11;
			this._ConfigButton.Text = "设置 P&DF 文件的修改方式";
			this._ConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ConfigButton.UseVisualStyleBackColor = true;
			// 
			// _ImportButton
			// 
			this._ImportButton.AlternativeFocusBorderColor = System.Drawing.SystemColors.Highlight;
			this._ImportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ImportButton.AnimateGlow = true;
			this._ImportButton.BackColor = System.Drawing.SystemColors.Highlight;
			this._ImportButton.CornerRadius = 3;
			this._ImportButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ImportButton.GlowColor = System.Drawing.Color.White;
			this._ImportButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._ImportButton.InnerBorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this._ImportButton.Location = new System.Drawing.Point(660, 446);
			this._ImportButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ImportButton.Name = "_ImportButton";
			this._ImportButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ImportButton.ShowFocusBorder = true;
			this._ImportButton.Size = new System.Drawing.Size(184, 44);
			this._ImportButton.TabIndex = 12;
			this._ImportButton.Text = "生成目标文件(&S)";
			this._ImportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ImportButton.Click += new System.EventHandler(this._ImportButton_Click);
			// 
			// _FileTimeColumn
			// 
			this._FileTimeColumn.IsEditable = false;
			this._FileTimeColumn.Text = "修改时间";
			this._FileTimeColumn.Width = 145;
			// 
			// PatcherControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._ImportButton);
			this.Controls.Add(this._ConfigButton);
			this.Controls.Add(_MainToolbar);
			this.Controls.Add(this._TargetPdfFile);
			this.Controls.Add(this._ItemActionsContainerBox);
			this.Controls.Add(this._AutoClearListBox);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "PatcherControl";
			this.Size = new System.Drawing.Size(862, 513);
			this.Load += new System.EventHandler(this.PatcherControl_OnLoad);
			_MainToolbar.ResumeLayout(false);
			_MainToolbar.PerformLayout();
			this._SortMenu.ResumeLayout(false);
			this._SelectionMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._ItemList)).EndInit();
			this._ItemListMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._ActionsBox)).EndInit();
			this._ItemActionsContainerBox.Panel1.ResumeLayout(false);
			this._ItemActionsContainerBox.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._ItemActionsContainerBox)).EndInit();
			this._ItemActionsContainerBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView _ItemList;
		private BrightIdeasSoftware.OLVColumn _NameColumn;
		private BrightIdeasSoftware.OLVColumn _FolderColumn;
		private TargetFileControl _TargetPdfFile;
		private System.Windows.Forms.ContextMenuStrip _SortMenu;
		private System.Windows.Forms.ToolStripMenuItem _SortByNaturalNumberItem;
		private System.Windows.Forms.ToolStripMenuItem _SortByAlphaItem;
		private System.Windows.Forms.ContextMenuStrip _SelectionMenu;
		private BrightIdeasSoftware.OLVColumn _PageCountColumn;
		private System.Windows.Forms.ToolStripMenuItem _SelectionMenuItem;
		private System.Windows.Forms.ContextMenuStrip _ItemListMenu;
		private System.Windows.Forms.ContextMenuStrip _RecentFileMenu;
		private System.Windows.Forms.OpenFileDialog _OpenPdfBox;
		private System.Windows.Forms.CheckBox _AutoClearListBox;
		private System.ComponentModel.BackgroundWorker _AddDocumentWorker;
		private BrightIdeasSoftware.OLVColumn _TitleColumn;
		private BrightIdeasSoftware.OLVColumn _AuthorColumn;
		private BrightIdeasSoftware.OLVColumn _SubjectColumn;
		private BrightIdeasSoftware.OLVColumn _KeywordsColumn;
		private BrightIdeasSoftware.ObjectListView _ActionsBox;
		private System.Windows.Forms.SplitContainer _ItemActionsContainerBox;
		private BrightIdeasSoftware.OLVColumn _ActionNameColumn;
		private System.Windows.Forms.ImageList _FileTypeList;
		private System.Windows.Forms.ToolStripSplitButton _RefreshInfoButton;
		private System.Windows.Forms.ContextMenuStrip _RefreshInfoMenu;
		private System.Windows.Forms.Button _ConfigButton;
		private EnhancedGlassButton.GlassButton _ImportButton;
		private System.Windows.Forms.ToolStripSplitButton _AddFilesButton;
		private BrightIdeasSoftware.OLVColumn _FileTimeColumn;
	}
}
