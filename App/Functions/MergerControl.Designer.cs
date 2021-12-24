namespace PDFPatcher.Functions
{
	partial class MergerControl
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
			System.Windows.Forms.ToolStripDropDownButton _File;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
			System.Windows.Forms.ToolStripMenuItem _LoadList;
			System.Windows.Forms.ToolStripMenuItem _SaveList;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripButton _EditItemProperty;
			System.Windows.Forms.ToolStripButton _Refresh;
			System.Windows.Forms.ToolStripMenuItem _SetPdfOptions;
			System.Windows.Forms.ToolStripMenuItem _SetCroppingOptions;
			System.Windows.Forms.ToolStripMenuItem _Copy;
			System.Windows.Forms.ToolStripMenuItem _RefreshFolder;
			System.Windows.Forms.ToolStripMenuItem _ClearBookmarkTitle;
			System.Windows.Forms.ToolStripMenuItem _SetBookmarkTitle;
			System.Windows.Forms.ToolStripMenuItem _PasteBookmarkText;
			System.Windows.Forms.ToolStripMenuItem _CopyBookmarkText;
			System.Windows.Forms.ToolStripMenuItem _CopyFileName;
			System.Windows.Forms.ToolStripButton _Delete;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergerControl));
			this._FileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._SortByNaturalNumberItem = new System.Windows.Forms.ToolStripMenuItem();
			this._SortByAlphaItem = new System.Windows.Forms.ToolStripMenuItem();
			this._MainToolbar = new System.Windows.Forms.ToolStrip();
			this._AddFilesButton = new System.Windows.Forms.ToolStripSplitButton();
			this._RecentFileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._AddFolderButton = new System.Windows.Forms.ToolStripSplitButton();
			this._RecentFolderMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._InsertEmptyPage = new System.Windows.Forms.ToolStripButton();
			this._BoldStyleButton = new System.Windows.Forms.ToolStripButton();
			this._ItalicStyleButton = new System.Windows.Forms.ToolStripButton();
			this._BookmarkColorButton = new ColorPicker.ToolStripColorPicker();
			this._BookmarkTextMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this._ItemList = new BrightIdeasSoftware.TreeListView();
			this._NameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._BookmarkColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._PageRangeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._FolderColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ItemListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this._FileTypeList = new System.Windows.Forms.ImageList(this.components);
			this._OpenImageBox = new System.Windows.Forms.OpenFileDialog();
			this._OpenPdfBox = new System.Windows.Forms.OpenFileDialog();
			this._AddDocumentWorker = new System.ComponentModel.BackgroundWorker();
			this._BookmarkControl = new PDFPatcher.BookmarkControl();
			this._TargetPdfFile = new PDFPatcher.TargetFileControl();
			this._ImportButton = new EnhancedGlassButton.GlassButton();
			this._IndividualMergerModeBox = new System.Windows.Forms.CheckBox();
			this._ConfigButton = new System.Windows.Forms.Button();
			_File = new System.Windows.Forms.ToolStripDropDownButton();
			toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			_LoadList = new System.Windows.Forms.ToolStripMenuItem();
			_SaveList = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			_EditItemProperty = new System.Windows.Forms.ToolStripButton();
			_Refresh = new System.Windows.Forms.ToolStripButton();
			_SetPdfOptions = new System.Windows.Forms.ToolStripMenuItem();
			_SetCroppingOptions = new System.Windows.Forms.ToolStripMenuItem();
			_Copy = new System.Windows.Forms.ToolStripMenuItem();
			_RefreshFolder = new System.Windows.Forms.ToolStripMenuItem();
			_ClearBookmarkTitle = new System.Windows.Forms.ToolStripMenuItem();
			_SetBookmarkTitle = new System.Windows.Forms.ToolStripMenuItem();
			_PasteBookmarkText = new System.Windows.Forms.ToolStripMenuItem();
			_CopyBookmarkText = new System.Windows.Forms.ToolStripMenuItem();
			_CopyFileName = new System.Windows.Forms.ToolStripMenuItem();
			_Delete = new System.Windows.Forms.ToolStripButton();
			this._FileMenu.SuspendLayout();
			this._MainToolbar.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._ItemList)).BeginInit();
			this._ItemListMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// _File
			// 
			_File.AutoToolTip = false;
			_File.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			_File.DropDown = this._FileMenu;
			_File.Image = global::PDFPatcher.Properties.Resources.Sort;
			_File.ImageTransparentColor = System.Drawing.Color.Magenta;
			_File.Name = "_File";
			_File.Size = new System.Drawing.Size(69, 28);
			_File.Text = "文件(&J)";
			_File.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _FileMenu
			// 
			this._FileMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._FileMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._SortByNaturalNumberItem,
            this._SortByAlphaItem,
            toolStripSeparator4,
            _LoadList,
            _SaveList});
			this._FileMenu.Name = "_SortMenu";
			this._FileMenu.OwnerItem = _File;
			this._FileMenu.Size = new System.Drawing.Size(254, 114);
			this._FileMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._SortMenu_ItemClicked);
			// 
			// _SortByNaturalNumberItem
			// 
			this._SortByNaturalNumberItem.Image = global::PDFPatcher.Properties.Resources.NaturalSort;
			this._SortByNaturalNumberItem.Name = "_SortByNaturalNumberItem";
			this._SortByNaturalNumberItem.Size = new System.Drawing.Size(253, 26);
			this._SortByNaturalNumberItem.Text = "按数值和字母顺序排序(&S)";
			this._SortByNaturalNumberItem.Visible = false;
			// 
			// _SortByAlphaItem
			// 
			this._SortByAlphaItem.Image = global::PDFPatcher.Properties.Resources.AlphabeticSort;
			this._SortByAlphaItem.Name = "_SortByAlphaItem";
			this._SortByAlphaItem.Size = new System.Drawing.Size(253, 26);
			this._SortByAlphaItem.Text = "按字母顺序排序(&Z)";
			this._SortByAlphaItem.Visible = false;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
			toolStripSeparator4.Visible = false;
			// 
			// _LoadList
			// 
			_LoadList.Image = global::PDFPatcher.Properties.Resources.Open;
			_LoadList.Name = "_LoadList";
			_LoadList.Size = new System.Drawing.Size(253, 26);
			_LoadList.Text = "加载文件列表(&J)...";
			_LoadList.ToolTipText = "加载上次保存的文件列表，供继续编辑";
			// 
			// _SaveList
			// 
			_SaveList.Image = global::PDFPatcher.Properties.Resources.Save;
			_SaveList.Name = "_SaveList";
			_SaveList.Size = new System.Drawing.Size(253, 26);
			_SaveList.Text = "保存文件列表(&B)...";
			_SaveList.ToolTipText = "保存文件列表到文件，供以后处理";
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
			// 
			// _EditItemProperty
			// 
			_EditItemProperty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			_EditItemProperty.Image = global::PDFPatcher.Properties.Resources.PdfPageRange;
			_EditItemProperty.ImageTransparentColor = System.Drawing.Color.Magenta;
			_EditItemProperty.Name = "_EditItemProperty";
			_EditItemProperty.Size = new System.Drawing.Size(24, 28);
			_EditItemProperty.Text = "编辑源文件的处理方式";
			// 
			// _Refresh
			// 
			_Refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			_Refresh.Image = global::PDFPatcher.Properties.Resources.Refresh;
			_Refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Refresh.Name = "_Refresh";
			_Refresh.Size = new System.Drawing.Size(24, 28);
			_Refresh.Text = "toolStripButton1";
			_Refresh.Visible = false;
			// 
			// _SetPdfOptions
			// 
			_SetPdfOptions.Name = "_SetPdfOptions";
			_SetPdfOptions.Size = new System.Drawing.Size(267, 26);
			_SetPdfOptions.Text = "设置源 &PDF 文件处理方式...";
			_SetPdfOptions.ToolTipText = "更改源 PDF 文件的页码范围";
			// 
			// _SetCroppingOptions
			// 
			_SetCroppingOptions.Name = "_SetCroppingOptions";
			_SetCroppingOptions.Size = new System.Drawing.Size(267, 26);
			_SetCroppingOptions.Text = "设置裁剪图片选项(&C)...";
			// 
			// _Copy
			// 
			_Copy.Name = "_Copy";
			_Copy.Size = new System.Drawing.Size(267, 26);
			_Copy.Text = "复制列表内容(&F)";
			// 
			// _RefreshFolder
			// 
			_RefreshFolder.Image = global::PDFPatcher.Properties.Resources.Refresh;
			_RefreshFolder.Name = "_RefreshFolder";
			_RefreshFolder.Size = new System.Drawing.Size(267, 26);
			_RefreshFolder.Text = "刷新文件夹(&W)";
			_RefreshFolder.ToolTipText = "刷新文件夹的内容";
			// 
			// _ClearBookmarkTitle
			// 
			_ClearBookmarkTitle.Name = "_ClearBookmarkTitle";
			_ClearBookmarkTitle.Size = new System.Drawing.Size(249, 26);
			_ClearBookmarkTitle.Text = "清空书签文本";
			_ClearBookmarkTitle.ToolTipText = "清空选中项目对应的书签文本及书签设置";
			// 
			// _SetBookmarkTitle
			// 
			_SetBookmarkTitle.Name = "_SetBookmarkTitle";
			_SetBookmarkTitle.Size = new System.Drawing.Size(249, 26);
			_SetBookmarkTitle.Text = "设置书签文本为源文件名";
			_SetBookmarkTitle.ToolTipText = "将选中项目对应的书签文本设置为文件名";
			// 
			// _PasteBookmarkText
			// 
			_PasteBookmarkText.Image = global::PDFPatcher.Properties.Resources.Paste;
			_PasteBookmarkText.Name = "_PasteBookmarkText";
			_PasteBookmarkText.Size = new System.Drawing.Size(267, 26);
			_PasteBookmarkText.Text = "粘贴书签文本(Z)";
			// 
			// _CopyBookmarkText
			// 
			_CopyBookmarkText.Image = global::PDFPatcher.Properties.Resources.Copy;
			_CopyBookmarkText.Name = "_CopyBookmarkText";
			_CopyBookmarkText.Size = new System.Drawing.Size(267, 26);
			_CopyBookmarkText.Text = "复制书签文本(&S)";
			// 
			// _CopyFileName
			// 
			_CopyFileName.Name = "_CopyFileName";
			_CopyFileName.Size = new System.Drawing.Size(267, 26);
			_CopyFileName.Text = "复制文件名(M)";
			// 
			// _MainToolbar
			// 
			this._MainToolbar.AutoSize = false;
			this._MainToolbar.GripMargin = new System.Windows.Forms.Padding(0);
			this._MainToolbar.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._MainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _File,
            this._AddFilesButton,
            this._AddFolderButton,
            this._InsertEmptyPage,
            _Delete,
            toolStripSeparator1,
            this._BoldStyleButton,
            this._ItalicStyleButton,
            this._BookmarkColorButton,
            this._BookmarkTextMenu,
            toolStripSeparator2,
            _EditItemProperty,
            _Refresh});
			this._MainToolbar.Location = new System.Drawing.Point(0, 0);
			this._MainToolbar.Name = "_MainToolbar";
			this._MainToolbar.Size = new System.Drawing.Size(767, 31);
			this._MainToolbar.TabIndex = 1;
			this._MainToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _AddFilesButton
			// 
			this._AddFilesButton.DropDown = this._RecentFileMenu;
			this._AddFilesButton.Image = global::PDFPatcher.Properties.Resources.Add;
			this._AddFilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFilesButton.Name = "_AddFilesButton";
			this._AddFilesButton.Size = new System.Drawing.Size(127, 28);
			this._AddFilesButton.Text = "添加文件(&T)";
			this._AddFilesButton.ToolTipText = "添加需要合并的文件到处理列表";
			this._AddFilesButton.ButtonClick += new System.EventHandler(this._MainToolbar_ButtonClick);
			// 
			// _RecentFileMenu
			// 
			this._RecentFileMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._RecentFileMenu.Name = "_RecentFileMenu";
			this._RecentFileMenu.OwnerItem = this._AddFilesButton;
			this._RecentFileMenu.ShowImageMargin = false;
			this._RecentFileMenu.Size = new System.Drawing.Size(36, 4);
			// 
			// _AddFolderButton
			// 
			this._AddFolderButton.DropDown = this._RecentFolderMenu;
			this._AddFolderButton.Image = global::PDFPatcher.Properties.Resources.ImageFolder;
			this._AddFolderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFolderButton.Name = "_AddFolderButton";
			this._AddFolderButton.Size = new System.Drawing.Size(123, 28);
			this._AddFolderButton.Text = "添加文件夹";
			this._AddFolderButton.ToolTipText = "添加文件夹及其包含的文件到处理列表";
			this._AddFolderButton.ButtonClick += new System.EventHandler(this._MainToolbar_ButtonClick);
			this._AddFolderButton.DropDownOpening += new System.EventHandler(this._AddFolder_DropDownOpening);
			this._AddFolderButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._AddFolderButton_DropDownItemClicked);
			// 
			// _RecentFolderMenu
			// 
			this._RecentFolderMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._RecentFolderMenu.Name = "_RecentFolderMenu";
			this._RecentFolderMenu.OwnerItem = this._AddFolderButton;
			this._RecentFolderMenu.ShowImageMargin = false;
			this._RecentFolderMenu.Size = new System.Drawing.Size(36, 4);
			// 
			// _InsertEmptyPage
			// 
			this._InsertEmptyPage.Image = global::PDFPatcher.Properties.Resources.EmptyPage;
			this._InsertEmptyPage.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._InsertEmptyPage.Name = "_InsertEmptyPage";
			this._InsertEmptyPage.Size = new System.Drawing.Size(108, 28);
			this._InsertEmptyPage.Text = "插入空白页";
			// 
			// _Delete
			// 
			_Delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			_Delete.Image = global::PDFPatcher.Properties.Resources.Delete;
			_Delete.ImageTransparentColor = System.Drawing.Color.Magenta;
			_Delete.Name = "_Delete";
			_Delete.Size = new System.Drawing.Size(24, 28);
			_Delete.Text = "删除选中项";
			// 
			// _BoldStyleButton
			// 
			this._BoldStyleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._BoldStyleButton.Image = global::PDFPatcher.Properties.Resources.Bold;
			this._BoldStyleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._BoldStyleButton.Name = "_BoldStyleButton";
			this._BoldStyleButton.Size = new System.Drawing.Size(24, 28);
			this._BoldStyleButton.Text = "切换书签文本的粗体样式";
			// 
			// _ItalicStyleButton
			// 
			this._ItalicStyleButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._ItalicStyleButton.Image = global::PDFPatcher.Properties.Resources.Italic;
			this._ItalicStyleButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._ItalicStyleButton.Name = "_ItalicStyleButton";
			this._ItalicStyleButton.Size = new System.Drawing.Size(24, 28);
			this._ItalicStyleButton.Text = "切换书签文本的斜体样式";
			// 
			// _BookmarkColorButton
			// 
			this._BookmarkColorButton.AutoSize = false;
			this._BookmarkColorButton.ButtonDisplayStyle = ColorPicker.ToolStripColorPickerDisplayType.UnderLineAndImage;
			this._BookmarkColorButton.Color = System.Drawing.Color.Black;
			this._BookmarkColorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._BookmarkColorButton.Image = ((System.Drawing.Image)(resources.GetObject("_BookmarkColorButton.Image")));
			this._BookmarkColorButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._BookmarkColorButton.Name = "_BookmarkColorButton";
			this._BookmarkColorButton.Size = new System.Drawing.Size(30, 23);
			this._BookmarkColorButton.Text = "颜色";
			this._BookmarkColorButton.ToolTipText = "设置书签文本的颜色";
			// 
			// _BookmarkTextMenu
			// 
			this._BookmarkTextMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._BookmarkTextMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ClearBookmarkTitle,
            _SetBookmarkTitle});
			this._BookmarkTextMenu.Image = global::PDFPatcher.Properties.Resources.Mark;
			this._BookmarkTextMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._BookmarkTextMenu.Name = "_BookmarkTextMenu";
			this._BookmarkTextMenu.Size = new System.Drawing.Size(34, 28);
			this._BookmarkTextMenu.Text = "设置选中项目的书签文本";
			this._BookmarkTextMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// _ItemList
			// 
			this._ItemList.AllColumns.Add(this._NameColumn);
			this._ItemList.AllColumns.Add(this._BookmarkColumn);
			this._ItemList.AllColumns.Add(this._PageRangeColumn);
			this._ItemList.AllColumns.Add(this._FolderColumn);
			this._ItemList.AllowDrop = true;
			this._ItemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ItemList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._ItemList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._NameColumn,
            this._BookmarkColumn,
            this._PageRangeColumn,
            this._FolderColumn});
			this._ItemList.ContextMenuStrip = this._ItemListMenu;
			this._ItemList.Cursor = System.Windows.Forms.Cursors.Default;
			this._ItemList.GridLines = true;
			this._ItemList.HideSelection = false;
			this._ItemList.IsSimpleDragSource = true;
			this._ItemList.IsSimpleDropSink = true;
			this._ItemList.Location = new System.Drawing.Point(17, 35);
			this._ItemList.Margin = new System.Windows.Forms.Padding(4);
			this._ItemList.Name = "_ItemList";
			this._ItemList.ShowGroups = false;
			this._ItemList.Size = new System.Drawing.Size(732, 246);
			this._ItemList.SmallImageList = this._FileTypeList;
			this._ItemList.TabIndex = 0;
			this._ItemList.UseCellFormatEvents = true;
			this._ItemList.UseCompatibleStateImageBehavior = false;
			this._ItemList.View = System.Windows.Forms.View.Details;
			this._ItemList.VirtualMode = true;
			this._ItemList.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this._ItemList_FormatRow);
			this._ItemList.ItemActivate += new System.EventHandler(this._ItemList_ItemActivate);
			// 
			// _NameColumn
			// 
			this._NameColumn.IsEditable = false;
			this._NameColumn.Text = "源文件名";
			this._NameColumn.Width = 178;
			// 
			// _BookmarkColumn
			// 
			this._BookmarkColumn.Text = "书签文本";
			this._BookmarkColumn.Width = 150;
			// 
			// _PageRangeColumn
			// 
			this._PageRangeColumn.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._PageRangeColumn.Text = "页码范围";
			// 
			// _FolderColumn
			// 
			this._FolderColumn.IsEditable = false;
			this._FolderColumn.Text = "文件夹";
			this._FolderColumn.Width = 114;
			// 
			// _ItemListMenu
			// 
			this._ItemListMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._ItemListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _CopyBookmarkText,
            _PasteBookmarkText,
            _CopyFileName,
            _Copy,
            this.toolStripSeparator3,
            _SetCroppingOptions,
            _SetPdfOptions,
            _RefreshFolder});
			this._ItemListMenu.Name = "_ItemListMenu";
			this._ItemListMenu.Size = new System.Drawing.Size(268, 192);
			this._ItemListMenu.Opening += new System.ComponentModel.CancelEventHandler(this._ItemListMenu_Opening);
			this._ItemListMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._MainToolbar_ItemClicked);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(264, 6);
			// 
			// _FileTypeList
			// 
			this._FileTypeList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._FileTypeList.ImageSize = new System.Drawing.Size(16, 16);
			this._FileTypeList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _OpenImageBox
			// 
			this._OpenImageBox.Filter = resources.GetString("_OpenImageBox.Filter");
			this._OpenImageBox.Multiselect = true;
			this._OpenImageBox.Title = "选择需要导入的图片文件或PDF文件";
			// 
			// _OpenPdfBox
			// 
			this._OpenPdfBox.DefaultExt = "pdf";
			this._OpenPdfBox.Filter = "PDF 文件（*.pdf）|*.pdf";
			this._OpenPdfBox.Multiselect = true;
			this._OpenPdfBox.Title = "选择需要处理的 PDF 文件";
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
			this._BookmarkControl.Location = new System.Drawing.Point(17, 294);
			this._BookmarkControl.Margin = new System.Windows.Forms.Padding(5);
			this._BookmarkControl.Name = "_BookmarkControl";
			this._BookmarkControl.Size = new System.Drawing.Size(737, 30);
			this._BookmarkControl.TabIndex = 1;
			// 
			// _TargetPdfFile
			// 
			this._TargetPdfFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetPdfFile.Location = new System.Drawing.Point(17, 331);
			this._TargetPdfFile.Margin = new System.Windows.Forms.Padding(5);
			this._TargetPdfFile.Name = "_TargetPdfFile";
			this._TargetPdfFile.Size = new System.Drawing.Size(737, 32);
			this._TargetPdfFile.TabIndex = 2;
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
			this._ImportButton.Location = new System.Drawing.Point(587, 371);
			this._ImportButton.Margin = new System.Windows.Forms.Padding(4);
			this._ImportButton.Name = "_ImportButton";
			this._ImportButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ImportButton.ShowFocusBorder = true;
			this._ImportButton.Size = new System.Drawing.Size(164, 36);
			this._ImportButton.TabIndex = 5;
			this._ImportButton.Text = "生成合并文件(&S)";
			this._ImportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ImportButton.Click += new System.EventHandler(this._ImportButton_Click);
			// 
			// _IndividualMergerModeBox
			// 
			this._IndividualMergerModeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._IndividualMergerModeBox.AutoSize = true;
			this._IndividualMergerModeBox.Location = new System.Drawing.Point(17, 377);
			this._IndividualMergerModeBox.Margin = new System.Windows.Forms.Padding(4);
			this._IndividualMergerModeBox.Name = "_IndividualMergerModeBox";
			this._IndividualMergerModeBox.Size = new System.Drawing.Size(233, 19);
			this._IndividualMergerModeBox.TabIndex = 3;
			this._IndividualMergerModeBox.Text = "顶层项目合并为单独的PDF文件";
			this._IndividualMergerModeBox.UseVisualStyleBackColor = true;
			// 
			// _ConfigButton
			// 
			this._ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._ConfigButton.Image = global::PDFPatcher.Properties.Resources.PdfOptions;
			this._ConfigButton.Location = new System.Drawing.Point(337, 371);
			this._ConfigButton.Margin = new System.Windows.Forms.Padding(4);
			this._ConfigButton.Name = "_ConfigButton";
			this._ConfigButton.Size = new System.Drawing.Size(241, 29);
			this._ConfigButton.TabIndex = 12;
			this._ConfigButton.Text = "设置合并后的 P&DF 文件选项";
			this._ConfigButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ConfigButton.UseVisualStyleBackColor = true;
			this._ConfigButton.Click += new System.EventHandler(this._MainToolbar_ButtonClick);
			// 
			// MergerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._ConfigButton);
			this.Controls.Add(this._IndividualMergerModeBox);
			this.Controls.Add(this._MainToolbar);
			this.Controls.Add(this._TargetPdfFile);
			this.Controls.Add(this._ImportButton);
			this.Controls.Add(this._BookmarkControl);
			this.Controls.Add(this._ItemList);
			this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MergerControl";
			this.Size = new System.Drawing.Size(767, 428);
			this.Load += new System.EventHandler(this.MergerControl_Load);
			this._FileMenu.ResumeLayout(false);
			this._MainToolbar.ResumeLayout(false);
			this._MainToolbar.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._ItemList)).EndInit();
			this._ItemListMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BrightIdeasSoftware.TreeListView _ItemList;
		private BrightIdeasSoftware.OLVColumn _NameColumn;
		private EnhancedGlassButton.GlassButton _ImportButton;
		private System.Windows.Forms.OpenFileDialog _OpenImageBox;
		private BrightIdeasSoftware.OLVColumn _FolderColumn;
		private TargetFileControl _TargetPdfFile;
		private System.Windows.Forms.ContextMenuStrip _FileMenu;
		private System.Windows.Forms.ToolStripMenuItem _SortByAlphaItem;
		private BookmarkControl _BookmarkControl;
		private System.Windows.Forms.ContextMenuStrip _ItemListMenu;
		private System.Windows.Forms.ToolStripSplitButton _AddFilesButton;
		private System.Windows.Forms.ContextMenuStrip _RecentFileMenu;
		private System.Windows.Forms.OpenFileDialog _OpenPdfBox;
		private System.Windows.Forms.ToolStripButton _InsertEmptyPage;
		private System.ComponentModel.BackgroundWorker _AddDocumentWorker;
		private System.Windows.Forms.ImageList _FileTypeList;
		private BrightIdeasSoftware.OLVColumn _BookmarkColumn;
		private System.Windows.Forms.ToolStripButton _BoldStyleButton;
		private System.Windows.Forms.ToolStripButton _ItalicStyleButton;
		private ColorPicker.ToolStripColorPicker _BookmarkColorButton;
		private System.Windows.Forms.ToolStrip _MainToolbar;
		private System.Windows.Forms.ToolStripSplitButton _AddFolderButton;
		private System.Windows.Forms.ContextMenuStrip _RecentFolderMenu;
		private System.Windows.Forms.ToolStripDropDownButton _BookmarkTextMenu;
		private System.Windows.Forms.CheckBox _IndividualMergerModeBox;
		private BrightIdeasSoftware.OLVColumn _PageRangeColumn;
		private System.Windows.Forms.Button _ConfigButton;
		private System.Windows.Forms.ToolStripMenuItem _SortByNaturalNumberItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
	}
}
