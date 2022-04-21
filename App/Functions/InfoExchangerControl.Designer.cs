namespace PDFPatcher.Functions
{
	partial class InfoExchangerControl
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
			System.Windows.Forms.ToolStripDropDownButton _Actions;
			System.Windows.Forms.ToolStripMenuItem _DocumentActions;
			System.Windows.Forms.ToolStripMenuItem _RemoveUsageRightsAction;
			System.Windows.Forms.ToolStripMenuItem _ModifyMetaDataAction;
			System.Windows.Forms.ToolStripMenuItem _PageActions;
			System.Windows.Forms.ToolStripMenuItem _ImageRecompressionAction;
			System.Windows.Forms.ToolStripMenuItem _RemoveAnnotationAction;
			System.Windows.Forms.ToolStripMenuItem _RemoveThumbnailAction;
			System.Windows.Forms.ToolStripMenuItem _RemoveTextAction;
			System.Windows.Forms.ToolStripMenuItem _RemoveImageAction;
			System.Windows.Forms.ToolStripMenuItem _RemoveActions;
			System.Windows.Forms.ToolStripDropDownButton _Sort;
			System.Windows.Forms.ToolStripButton _Delete;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripMenuItem _SelectAllItem;
			System.Windows.Forms.ToolStripMenuItem _InvertSelectItem;
			System.Windows.Forms.ToolStripMenuItem _SelectNoneItem;
			System.Windows.Forms.ToolStripMenuItem _Copy;
			System.Windows.Forms.ToolStripMenuItem _RefreshInfo;
			this._MainToolbar = new System.Windows.Forms.ToolStrip();
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
			this._FileTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItemListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._FileTypeList = new System.Windows.Forms.ImageList(this.components);
			this._OpenPdfBox = new System.Windows.Forms.OpenFileDialog();
			this._AutoClearListBox = new System.Windows.Forms.CheckBox();
			this._AddDocumentWorker = new System.ComponentModel.BackgroundWorker();
			this._BookmarkControl = new PDFPatcher.BookmarkControl();
			this._TargetPdfFile = new PDFPatcher.TargetFileControl();
			this._ActionsBox = new BrightIdeasSoftware.ObjectListView();
			this._ActionNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItemActionsContainerBox = new System.Windows.Forms.SplitContainer();
			this._ExportBookmarkButton = new System.Windows.Forms.Button();
			this._ImportButton = new EnhancedGlassButton.GlassButton();
			this._ConfigButton = new System.Windows.Forms.Button();
			this._InfoConfigButton = new System.Windows.Forms.Button();
			_Actions = new System.Windows.Forms.ToolStripDropDownButton();
			_DocumentActions = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveUsageRightsAction = new System.Windows.Forms.ToolStripMenuItem();
			_ModifyMetaDataAction = new System.Windows.Forms.ToolStripMenuItem();
			_PageActions = new System.Windows.Forms.ToolStripMenuItem();
			_ImageRecompressionAction = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveAnnotationAction = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveThumbnailAction = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveTextAction = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveImageAction = new System.Windows.Forms.ToolStripMenuItem();
			_RemoveActions = new System.Windows.Forms.ToolStripMenuItem();
			_Sort = new System.Windows.Forms.ToolStripDropDownButton();
			_Delete = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			_SelectAllItem = new System.Windows.Forms.ToolStripMenuItem();
			_InvertSelectItem = new System.Windows.Forms.ToolStripMenuItem();
			_SelectNoneItem = new System.Windows.Forms.ToolStripMenuItem();
			_Copy = new System.Windows.Forms.ToolStripMenuItem();
			_RefreshInfo = new System.Windows.Forms.ToolStripMenuItem();
			this._MainToolbar.SuspendLayout();
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
			this._MainToolbar.Dock = System.Windows.Forms.DockStyle.None;
			this._MainToolbar.GripMargin = new System.Windows.Forms.Padding(0);
			this._MainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._AddFilesButton,
            _Actions,
            _Sort,
            _Delete,
            toolStripSeparator2,
            this._RefreshInfoButton});
			this._MainToolbar.Location = new System.Drawing.Point(0, 0);
			this._MainToolbar.Name = "_MainToolbar";
			this._MainToolbar.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this._MainToolbar.Size = new System.Drawing.Size(483, 25);
			this._MainToolbar.TabIndex = 0;
			this._MainToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _AddFilesButton
			// 
			this._AddFilesButton.DropDown = this._RecentFileMenu;
			this._AddFilesButton.Image = global::PDFPatcher.Properties.Resources.Add;
			this._AddFilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFilesButton.Name = "_AddFilesButton";
			this._AddFilesButton.Size = new System.Drawing.Size(103, 22);
			this._AddFilesButton.Text = "添加文件(&T)";
			this._AddFilesButton.ToolTipText = "添加文件";
			this._AddFilesButton.ButtonClick += new System.EventHandler(this._MainToolbar_ButtonClick);
			// 
			// _RecentFileMenu
			// 
			this._RecentFileMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._RecentFileMenu.Name = "_RecentFileMenu";
			this._RecentFileMenu.OwnerItem = this._AddFilesButton;
			this._RecentFileMenu.ShowImageMargin = false;
			this._RecentFileMenu.Size = new System.Drawing.Size(36, 4);
			// 
			// _Actions
			// 
			_Actions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _DocumentActions,
            _PageActions,
            _RemoveActions});
			_Actions.Image = global::PDFPatcher.Properties.Resources.Actions;
			_Actions.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Actions.Name = "_Actions";
			_Actions.Size = new System.Drawing.Size(85, 22);
			_Actions.Text = "补丁操作";
			_Actions.Visible = false;
			_Actions.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _DocumentActions
			// 
			_DocumentActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _RemoveUsageRightsAction,
            _ModifyMetaDataAction});
			_DocumentActions.Image = global::PDFPatcher.Properties.Resources.DocumentProcessor;
			_DocumentActions.Name = "_DocumentActions";
			_DocumentActions.Size = new System.Drawing.Size(196, 22);
			_DocumentActions.Text = "添加文档处理操作";
			_DocumentActions.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _RemoveUsageRightsAction
			// 
			_RemoveUsageRightsAction.Name = "_RemoveUsageRightsAction";
			_RemoveUsageRightsAction.Size = new System.Drawing.Size(184, 22);
			_RemoveUsageRightsAction.Text = "移除复制、打印限制";
			// 
			// _ModifyMetaDataAction
			// 
			_ModifyMetaDataAction.Name = "_ModifyMetaDataAction";
			_ModifyMetaDataAction.Size = new System.Drawing.Size(184, 22);
			_ModifyMetaDataAction.Text = "修改元数据";
			// 
			// _PageActions
			// 
			_PageActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ImageRecompressionAction,
            _RemoveAnnotationAction,
            _RemoveThumbnailAction,
            _RemoveTextAction,
            _RemoveImageAction});
			_PageActions.Image = global::PDFPatcher.Properties.Resources.PageProcessor;
			_PageActions.Name = "_PageActions";
			_PageActions.Size = new System.Drawing.Size(196, 22);
			_PageActions.Text = "添加页面内容处理操作";
			_PageActions.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _ImageRecompressionAction
			// 
			_ImageRecompressionAction.Name = "_ImageRecompressionAction";
			_ImageRecompressionAction.Size = new System.Drawing.Size(196, 22);
			_ImageRecompressionAction.Text = "优化纯黑白图像压缩率";
			// 
			// _RemoveAnnotationAction
			// 
			_RemoveAnnotationAction.Name = "_RemoveAnnotationAction";
			_RemoveAnnotationAction.Size = new System.Drawing.Size(196, 22);
			_RemoveAnnotationAction.Text = "删除批注";
			// 
			// _RemoveThumbnailAction
			// 
			_RemoveThumbnailAction.Name = "_RemoveThumbnailAction";
			_RemoveThumbnailAction.Size = new System.Drawing.Size(196, 22);
			_RemoveThumbnailAction.Text = "删除缩略图";
			// 
			// _RemoveTextAction
			// 
			_RemoveTextAction.Name = "_RemoveTextAction";
			_RemoveTextAction.Size = new System.Drawing.Size(196, 22);
			_RemoveTextAction.Text = "删除文本内容";
			// 
			// _RemoveImageAction
			// 
			_RemoveImageAction.Name = "_RemoveImageAction";
			_RemoveImageAction.Size = new System.Drawing.Size(196, 22);
			_RemoveImageAction.Text = "删除图片";
			// 
			// _RemoveActions
			// 
			_RemoveActions.Image = global::PDFPatcher.Properties.Resources.Delete;
			_RemoveActions.Name = "_RemoveActions";
			_RemoveActions.Size = new System.Drawing.Size(196, 22);
			_RemoveActions.Text = "删除选中的操作";
			// 
			// _Sort
			// 
			_Sort.DropDown = this._SortMenu;
			_Sort.Image = global::PDFPatcher.Properties.Resources.Sort;
			_Sort.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Sort.Name = "_Sort";
			_Sort.Size = new System.Drawing.Size(61, 22);
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
			this._SortMenu.Size = new System.Drawing.Size(225, 64);
			this._SortMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._SortMenu_ItemClicked);
			// 
			// _SortByNaturalNumberItem
			// 
			this._SortByNaturalNumberItem.Image = global::PDFPatcher.Properties.Resources.NaturalSort;
			this._SortByNaturalNumberItem.Name = "_SortByNaturalNumberItem";
			this._SortByNaturalNumberItem.Size = new System.Drawing.Size(224, 30);
			this._SortByNaturalNumberItem.Text = "按数值和字母顺序排序(&M)";
			// 
			// _SortByAlphaItem
			// 
			this._SortByAlphaItem.Image = global::PDFPatcher.Properties.Resources.AlphabeticSort;
			this._SortByAlphaItem.Name = "_SortByAlphaItem";
			this._SortByAlphaItem.Size = new System.Drawing.Size(224, 30);
			this._SortByAlphaItem.Text = "按字母顺序排序(&Z)";
			// 
			// _Delete
			// 
			_Delete.Image = global::PDFPatcher.Properties.Resources.Delete;
			_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Delete.Name = "_Delete";
			_Delete.Size = new System.Drawing.Size(76, 22);
			_Delete.Text = "删除文件";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// _RefreshInfoButton
			// 
			this._RefreshInfoButton.Image = global::PDFPatcher.Properties.Resources.Refresh;
			this._RefreshInfoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._RefreshInfoButton.Name = "_RefreshInfoButton";
			this._RefreshInfoButton.Size = new System.Drawing.Size(112, 22);
			this._RefreshInfoButton.Text = "刷新文档属性";
			// 
			// _SelectAllItem
			// 
			_SelectAllItem.Image = global::PDFPatcher.Properties.Resources.SelectAll;
			_SelectAllItem.Name = "_SelectAllItem";
			_SelectAllItem.Size = new System.Drawing.Size(132, 30);
			_SelectAllItem.Text = "全部选中";
			// 
			// _InvertSelectItem
			// 
			_InvertSelectItem.Name = "_InvertSelectItem";
			_InvertSelectItem.Size = new System.Drawing.Size(132, 30);
			_InvertSelectItem.Text = "反转选择";
			// 
			// _SelectNoneItem
			// 
			_SelectNoneItem.Name = "_SelectNoneItem";
			_SelectNoneItem.Size = new System.Drawing.Size(132, 30);
			_SelectNoneItem.Text = "取消选择";
			// 
			// _Copy
			// 
			_Copy.Image = global::PDFPatcher.Properties.Resources.Copy;
			_Copy.Name = "_Copy";
			_Copy.Size = new System.Drawing.Size(156, 30);
			_Copy.Text = "复制列表内容";
			// 
			// _RefreshInfo
			// 
			_RefreshInfo.DropDown = this._RefreshInfoMenu;
			_RefreshInfo.Image = global::PDFPatcher.Properties.Resources.Refresh;
			_RefreshInfo.Name = "_RefreshInfo";
			_RefreshInfo.Size = new System.Drawing.Size(156, 30);
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
            _SelectAllItem,
            _InvertSelectItem,
            _SelectNoneItem});
			this._SelectionMenu.Name = "_SelectionMenu";
			this._SelectionMenu.OwnerItem = this._SelectionMenuItem;
			this._SelectionMenu.Size = new System.Drawing.Size(133, 94);
			this._SelectionMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _SelectionMenuItem
			// 
			this._SelectionMenuItem.DropDown = this._SelectionMenu;
			this._SelectionMenuItem.Image = global::PDFPatcher.Properties.Resources.SelectItem;
			this._SelectionMenuItem.Name = "_SelectionMenuItem";
			this._SelectionMenuItem.Size = new System.Drawing.Size(156, 30);
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
			this._ItemList.Location = new System.Drawing.Point(3, 3);
			this._ItemList.Name = "_ItemList";
			this._ItemList.ShowGroups = false;
			this._ItemList.Size = new System.Drawing.Size(544, 199);
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
			// _FileTimeColumn
			// 
			this._FileTimeColumn.IsEditable = false;
			this._FileTimeColumn.Text = "修改时间";
			this._FileTimeColumn.Width = 145;
			// 
			// _ItemListMenu
			// 
			this._ItemListMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._ItemListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _Copy,
            _RefreshInfo,
            this._SelectionMenuItem});
			this._ItemListMenu.Name = "_ItemListMenu";
			this._ItemListMenu.Size = new System.Drawing.Size(157, 94);
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
			this._AutoClearListBox.Location = new System.Drawing.Point(434, 9);
			this._AutoClearListBox.Name = "_AutoClearListBox";
			this._AutoClearListBox.Size = new System.Drawing.Size(132, 16);
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
			// _BookmarkControl
			// 
			this._BookmarkControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._BookmarkControl.LabelText = "P&DF 信息文件：";
			this._BookmarkControl.Location = new System.Drawing.Point(13, 235);
			this._BookmarkControl.Margin = new System.Windows.Forms.Padding(4);
			this._BookmarkControl.Name = "_BookmarkControl";
			this._BookmarkControl.Size = new System.Drawing.Size(553, 24);
			this._BookmarkControl.TabIndex = 6;
			// 
			// _TargetPdfFile
			// 
			this._TargetPdfFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetPdfFile.Location = new System.Drawing.Point(13, 265);
			this._TargetPdfFile.Margin = new System.Windows.Forms.Padding(4);
			this._TargetPdfFile.Name = "_TargetPdfFile";
			this._TargetPdfFile.Size = new System.Drawing.Size(553, 26);
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
			this._ActionsBox.Location = new System.Drawing.Point(5, 3);
			this._ActionsBox.Name = "_ActionsBox";
			this._ActionsBox.RowHeight = 18;
			this._ActionsBox.ShowGroups = false;
			this._ActionsBox.Size = new System.Drawing.Size(126, 176);
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
			this._ItemActionsContainerBox.Location = new System.Drawing.Point(13, 28);
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
			this._ItemActionsContainerBox.Size = new System.Drawing.Size(550, 205);
			this._ItemActionsContainerBox.SplitterDistance = 412;
			this._ItemActionsContainerBox.TabIndex = 5;
			// 
			// _ExportBookmarkButton
			// 
			this._ExportBookmarkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._ExportBookmarkButton.Image = global::PDFPatcher.Properties.Resources.ExportInfoFile;
			this._ExportBookmarkButton.Location = new System.Drawing.Point(18, 297);
			this._ExportBookmarkButton.Name = "_ExportBookmarkButton";
			this._ExportBookmarkButton.Size = new System.Drawing.Size(120, 23);
			this._ExportBookmarkButton.TabIndex = 8;
			this._ExportBookmarkButton.Text = "导出信息文件(&C)";
			this._ExportBookmarkButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExportBookmarkButton.UseVisualStyleBackColor = true;
			this._ExportBookmarkButton.Click += new System.EventHandler(this._ExportBookmarkButton_Click);
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
			this._ImportButton.Location = new System.Drawing.Point(440, 297);
			this._ImportButton.Name = "_ImportButton";
			this._ImportButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ImportButton.ShowFocusBorder = true;
			this._ImportButton.Size = new System.Drawing.Size(123, 29);
			this._ImportButton.TabIndex = 13;
			this._ImportButton.Text = "生成目标文件(&S)";
			this._ImportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ImportButton.Click += new System.EventHandler(this._ImportButton_Click);
			// 
			// _ConfigButton
			// 
			this._ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ConfigButton.Image = global::PDFPatcher.Properties.Resources.PdfOptions;
			this._ConfigButton.Location = new System.Drawing.Point(253, 297);
			this._ConfigButton.Name = "_ConfigButton";
			this._ConfigButton.Size = new System.Drawing.Size(181, 23);
			this._ConfigButton.TabIndex = 14;
			this._ConfigButton.Text = "设置 P&DF 文件的修改方式";
			this._ConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ConfigButton.UseVisualStyleBackColor = true;
			// 
			// _InfoConfigButton
			// 
			this._InfoConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._InfoConfigButton.Image = global::PDFPatcher.Properties.Resources.InfoFileOptions;
			this._InfoConfigButton.Location = new System.Drawing.Point(144, 297);
			this._InfoConfigButton.Name = "_InfoConfigButton";
			this._InfoConfigButton.Size = new System.Drawing.Size(103, 23);
			this._InfoConfigButton.TabIndex = 14;
			this._InfoConfigButton.Text = "信息文件配置";
			this._InfoConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._InfoConfigButton.UseVisualStyleBackColor = true;
			this._InfoConfigButton.Click += new System.EventHandler(this._MainToolbar_ButtonClick);
			// 
			// InfoExchangerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._InfoConfigButton);
			this.Controls.Add(this._ConfigButton);
			this.Controls.Add(this._ImportButton);
			this.Controls.Add(this._MainToolbar);
			this.Controls.Add(this._TargetPdfFile);
			this.Controls.Add(this._ExportBookmarkButton);
			this.Controls.Add(this._ItemActionsContainerBox);
			this.Controls.Add(this._AutoClearListBox);
			this.Controls.Add(this._BookmarkControl);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "InfoExchangerControl";
			this.Size = new System.Drawing.Size(575, 342);
			this._MainToolbar.ResumeLayout(false);
			this._MainToolbar.PerformLayout();
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
		private BookmarkControl _BookmarkControl;
		private BrightIdeasSoftware.OLVColumn _PageCountColumn;
		private System.Windows.Forms.ToolStripMenuItem _SelectionMenuItem;
		private System.Windows.Forms.ContextMenuStrip _ItemListMenu;
		private System.Windows.Forms.Button _ExportBookmarkButton;
		private System.Windows.Forms.ToolStripSplitButton _AddFilesButton;
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
		private EnhancedGlassButton.GlassButton _ImportButton;
		private System.Windows.Forms.Button _ConfigButton;
		private System.Windows.Forms.Button _InfoConfigButton;
		private BrightIdeasSoftware.OLVColumn _FileTimeColumn;
		private System.Windows.Forms.ToolStrip _MainToolbar;
	}
}
