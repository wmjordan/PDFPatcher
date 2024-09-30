namespace PDFPatcher.Functions
{
	partial class DocumentInspectorControl
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
			System.Windows.Forms.ToolStripMenuItem _ExportBinary;
			System.Windows.Forms.ToolStripMenuItem _ExportHexText;
			System.Windows.Forms.ToolStripMenuItem _ExportXml;
			System.Windows.Forms.ToolStripMenuItem _ExportUncompressedBinary;
			System.Windows.Forms.ToolStripMenuItem _ExportUncompressedHexText;
			System.Windows.Forms.ToolStripMenuItem _ExportToUnicode;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentInspectorControl));
			this._Container = new System.Windows.Forms.SplitContainer();
			this._ObjectDetailBox = new BrightIdeasSoftware.TreeListView();
			this._NameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ValueColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._DescriptionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ObjectTypeIcons = new System.Windows.Forms.ImageList(this.components);
			this._DescriptionBox = new RichTextBoxLinks.RichTextBoxEx();
			this._RecentFileMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._OpenButton = new System.Windows.Forms.ToolStripSplitButton();
			this._LoadDocumentWorker = new System.ComponentModel.BackgroundWorker();
			this._MainToolbar = new System.Windows.Forms.ToolStrip();
			this._SaveButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._ExportButton = new System.Windows.Forms.ToolStripDropDownButton();
			this._ViewButton = new System.Windows.Forms.ToolStripButton();
			this._AddObjectMenu = new System.Windows.Forms.ToolStripDropDownButton();
			this._AddArrayNode = new System.Windows.Forms.ToolStripMenuItem();
			this._AddDictNode = new System.Windows.Forms.ToolStripMenuItem();
			this._AddBooleanNode = new System.Windows.Forms.ToolStripMenuItem();
			this._AddStringNode = new System.Windows.Forms.ToolStripMenuItem();
			this._AddNumberNode = new System.Windows.Forms.ToolStripMenuItem();
			this._AddNameNode = new System.Windows.Forms.ToolStripMenuItem();
			this._DeleteButton = new System.Windows.Forms.ToolStripButton();
			this._ExpandButton = new System.Windows.Forms.ToolStripButton();
			this._CollapseButton = new System.Windows.Forms.ToolStripButton();
			_ExportBinary = new System.Windows.Forms.ToolStripMenuItem();
			_ExportHexText = new System.Windows.Forms.ToolStripMenuItem();
			_ExportXml = new System.Windows.Forms.ToolStripMenuItem();
			_ExportUncompressedBinary = new System.Windows.Forms.ToolStripMenuItem();
			_ExportUncompressedHexText = new System.Windows.Forms.ToolStripMenuItem();
			_ExportToUnicode = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this._Container)).BeginInit();
			this._Container.Panel1.SuspendLayout();
			this._Container.Panel2.SuspendLayout();
			this._Container.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._ObjectDetailBox)).BeginInit();
			this._MainToolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// _ExportBinary
			// 
			_ExportBinary.Name = "_ExportBinary";
			_ExportBinary.Size = new System.Drawing.Size(244, 22);
			_ExportBinary.Text = "二进制文件(&E)...";
			// 
			// _ExportHexText
			// 
			_ExportHexText.Name = "_ExportHexText";
			_ExportHexText.Size = new System.Drawing.Size(244, 22);
			_ExportHexText.Text = "二进制文本文件(&W)...";
			// 
			// _ExportXml
			// 
			_ExportXml.Name = "_ExportXml";
			_ExportXml.Size = new System.Drawing.Size(244, 22);
			_ExportXml.Text = "&XML信息文件...";
			// 
			// _ExportUncompressedBinary
			// 
			_ExportUncompressedBinary.Name = "_ExportUncompressedBinary";
			_ExportUncompressedBinary.Size = new System.Drawing.Size(244, 22);
			_ExportUncompressedBinary.Text = "原始流对象二进制文件(&Y)...";
			// 
			// _ExportUncompressedHexText
			// 
			_ExportUncompressedHexText.Name = "_ExportUncompressedHexText";
			_ExportUncompressedHexText.Size = new System.Drawing.Size(244, 22);
			_ExportUncompressedHexText.Text = "原始流对象二进制文本文件(&Y)...";
			// 
			// _ExportToUnicode
			// 
			_ExportToUnicode.Name = "_ExportToUnicode";
			_ExportToUnicode.Size = new System.Drawing.Size(244, 22);
			_ExportToUnicode.Text = "&ToUnicode 映射表";
			// 
			// _Container
			// 
			this._Container.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._Container.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._Container.Location = new System.Drawing.Point(3, 28);
			this._Container.Name = "_Container";
			this._Container.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _Container.Panel1
			// 
			this._Container.Panel1.Controls.Add(this._ObjectDetailBox);
			// 
			// _Container.Panel2
			// 
			this._Container.Panel2.Controls.Add(this._DescriptionBox);
			this._Container.Size = new System.Drawing.Size(487, 310);
			this._Container.SplitterDistance = 229;
			this._Container.TabIndex = 1;
			// 
			// _ObjectDetailBox
			// 
			this._ObjectDetailBox.AllColumns.Add(this._NameColumn);
			this._ObjectDetailBox.AllColumns.Add(this._ValueColumn);
			this._ObjectDetailBox.AllColumns.Add(this._DescriptionColumn);
			this._ObjectDetailBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ObjectDetailBox.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
			this._ObjectDetailBox.CellEditUseWholeCell = false;
			this._ObjectDetailBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._NameColumn,
            this._ValueColumn,
            this._DescriptionColumn});
			this._ObjectDetailBox.GridLines = true;
			this._ObjectDetailBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._ObjectDetailBox.HideSelection = false;
			this._ObjectDetailBox.Location = new System.Drawing.Point(3, 3);
			this._ObjectDetailBox.Name = "_ObjectDetailBox";
			this._ObjectDetailBox.RevealAfterExpand = false;
			this._ObjectDetailBox.ShowGroups = false;
			this._ObjectDetailBox.Size = new System.Drawing.Size(481, 223);
			this._ObjectDetailBox.SmallImageList = this._ObjectTypeIcons;
			this._ObjectDetailBox.TabIndex = 0;
			this._ObjectDetailBox.UseCompatibleStateImageBehavior = false;
			this._ObjectDetailBox.View = System.Windows.Forms.View.Details;
			this._ObjectDetailBox.VirtualMode = true;
			this._ObjectDetailBox.ItemActivate += new System.EventHandler(this.ControlEvent);
			// 
			// _NameColumn
			// 
			this._NameColumn.IsEditable = false;
			this._NameColumn.Text = "名称";
			this._NameColumn.Width = 184;
			// 
			// _ValueColumn
			// 
			this._ValueColumn.Text = "值";
			this._ValueColumn.Width = 187;
			// 
			// _DescriptionColumn
			// 
			this._DescriptionColumn.IsEditable = true;
			this._DescriptionColumn.Text = "说明";
			this._DescriptionColumn.Width = 93;
			// 
			// _ObjectTypeIcons
			// 
			this._ObjectTypeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_ObjectTypeIcons.ImageStream")));
			this._ObjectTypeIcons.TransparentColor = System.Drawing.Color.Transparent;
			this._ObjectTypeIcons.Images.SetKeyName(0, "Current");
			this._ObjectTypeIcons.Images.SetKeyName(1, "Page");
			this._ObjectTypeIcons.Images.SetKeyName(2, "Dictionary");
			this._ObjectTypeIcons.Images.SetKeyName(3, "Array");
			this._ObjectTypeIcons.Images.SetKeyName(4, "Name");
			this._ObjectTypeIcons.Images.SetKeyName(5, "String");
			this._ObjectTypeIcons.Images.SetKeyName(6, "Number");
			this._ObjectTypeIcons.Images.SetKeyName(7, "Stream");
			this._ObjectTypeIcons.Images.SetKeyName(8, "Bool");
			this._ObjectTypeIcons.Images.SetKeyName(9, "Reference");
			this._ObjectTypeIcons.Images.SetKeyName(10, "Document");
			this._ObjectTypeIcons.Images.SetKeyName(11, "Pages");
			this._ObjectTypeIcons.Images.SetKeyName(12, "PageCommands");
			this._ObjectTypeIcons.Images.SetKeyName(13, "Outlines");
			this._ObjectTypeIcons.Images.SetKeyName(14, "Outline");
			this._ObjectTypeIcons.Images.SetKeyName(15, "Trailer");
			this._ObjectTypeIcons.Images.SetKeyName(16, "GoToPage");
			this._ObjectTypeIcons.Images.SetKeyName(17, "Image");
			this._ObjectTypeIcons.Images.SetKeyName(18, "Form");
			this._ObjectTypeIcons.Images.SetKeyName(19, "Info");
			this._ObjectTypeIcons.Images.SetKeyName(20, "Font");
			this._ObjectTypeIcons.Images.SetKeyName(21, "Resources");
			this._ObjectTypeIcons.Images.SetKeyName(22, "Null");
			this._ObjectTypeIcons.Images.SetKeyName(23, "Hidden");
			this._ObjectTypeIcons.Images.SetKeyName(24, "op_q");
			this._ObjectTypeIcons.Images.SetKeyName(25, "op_cm");
			this._ObjectTypeIcons.Images.SetKeyName(26, "op_tm");
			this._ObjectTypeIcons.Images.SetKeyName(27, "op_cs");
			this._ObjectTypeIcons.Images.SetKeyName(28, "op_sc");
			this._ObjectTypeIcons.Images.SetKeyName(29, "op_g");
			this._ObjectTypeIcons.Images.SetKeyName(30, "op_s");
			this._ObjectTypeIcons.Images.SetKeyName(31, "op_TJ");
			this._ObjectTypeIcons.Images.SetKeyName(32, "op_tj_");
			this._ObjectTypeIcons.Images.SetKeyName(33, "op_f");
			this._ObjectTypeIcons.Images.SetKeyName(34, "op_Ts");
			this._ObjectTypeIcons.Images.SetKeyName(35, "op_BT");
			this._ObjectTypeIcons.Images.SetKeyName(36, "op_Td");
			this._ObjectTypeIcons.Images.SetKeyName(37, "op_Tr");
			this._ObjectTypeIcons.Images.SetKeyName(38, "op_BDC");
			this._ObjectTypeIcons.Images.SetKeyName(39, "op_re");
			this._ObjectTypeIcons.Images.SetKeyName(40, "op_W*");
			this._ObjectTypeIcons.Images.SetKeyName(41, "op_c");
			this._ObjectTypeIcons.Images.SetKeyName(42, "op_l");
			this._ObjectTypeIcons.Images.SetKeyName(43, "op_tc");
			this._ObjectTypeIcons.Images.SetKeyName(44, "op_Tz");
			this._ObjectTypeIcons.Images.SetKeyName(45, "op_Tl");
			this._ObjectTypeIcons.Images.SetKeyName(46, "op_gs");
			this._ObjectTypeIcons.Images.SetKeyName(47, "op_w");
			this._ObjectTypeIcons.Images.SetKeyName(48, "op_M_");
			this._ObjectTypeIcons.Images.SetKeyName(49, "op_d");
			this._ObjectTypeIcons.Images.SetKeyName(50, "op_b");
			this._ObjectTypeIcons.Images.SetKeyName(51, "op_m");
			this._ObjectTypeIcons.Images.SetKeyName(52, "op_h");
			// 
			// _DescriptionBox
			// 
			this._DescriptionBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._DescriptionBox.Location = new System.Drawing.Point(3, 3);
			this._DescriptionBox.Name = "_DescriptionBox";
			this._DescriptionBox.ReadOnly = true;
			this._DescriptionBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this._DescriptionBox.Size = new System.Drawing.Size(481, 71);
			this._DescriptionBox.TabIndex = 1;
			this._DescriptionBox.Text = "";
			// 
			// _RecentFileMenu
			// 
			this._RecentFileMenu.Name = "_RecentFileMenu";
			this._RecentFileMenu.OwnerItem = this._OpenButton;
			this._RecentFileMenu.Size = new System.Drawing.Size(61, 4);
			// 
			// _OpenButton
			// 
			this._OpenButton.DropDown = this._RecentFileMenu;
			this._OpenButton.Image = global::PDFPatcher.Properties.Resources.OpenFile;
			this._OpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._OpenButton.Name = "_OpenButton";
			this._OpenButton.Size = new System.Drawing.Size(81, 22);
			this._OpenButton.Text = "打开(&D)";
			this._OpenButton.ToolTipText = "打开 PDF 文档";
			this._OpenButton.ButtonClick += new System.EventHandler(this.ControlEvent);
			// 
			// _LoadDocumentWorker
			// 
			this._LoadDocumentWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._LoadDocumentWorker_DoWork);
			this._LoadDocumentWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._LoadDocumentWorker_RunWorkerCompleted);
			// 
			// _MainToolbar
			// 
			this._MainToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._OpenButton,
            this._SaveButton,
            this.toolStripSeparator1,
            this._ExportButton,
            this._ViewButton,
            this._AddObjectMenu,
            this._DeleteButton,
            this._ExpandButton,
            this._CollapseButton});
			this._MainToolbar.Location = new System.Drawing.Point(0, 0);
			this._MainToolbar.Name = "_MainToolbar";
			this._MainToolbar.Size = new System.Drawing.Size(495, 25);
			this._MainToolbar.TabIndex = 0;
			this._MainToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolbarItemClicked);
			// 
			// _SaveButton
			// 
			this._SaveButton.Enabled = false;
			this._SaveButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._SaveButton.Name = "_SaveButton";
			this._SaveButton.Size = new System.Drawing.Size(68, 22);
			this._SaveButton.Text = "保存(&B)";
			this._SaveButton.ToolTipText = "保存修改后的 PDF 文档";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// _ExportButton
			// 
			this._ExportButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ExportBinary,
            _ExportHexText,
            _ExportXml,
            _ExportUncompressedBinary,
            _ExportUncompressedHexText,
            _ExportToUnicode});
			this._ExportButton.Enabled = false;
			this._ExportButton.Image = global::PDFPatcher.Properties.Resources.ExportFile;
			this._ExportButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._ExportButton.Name = "_ExportButton";
			this._ExportButton.Size = new System.Drawing.Size(61, 22);
			this._ExportButton.Text = "导出";
			this._ExportButton.ToolTipText = "导出流对象的内容";
			this._ExportButton.DropDownOpening += new System.EventHandler(this._ExportButton_DropDownOpening);
			this._ExportButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolbarItemClicked);
			// 
			// _ViewButton
			// 
			this._ViewButton.Enabled = false;
			this._ViewButton.Image = global::PDFPatcher.Properties.Resources.ViewContent;
			this._ViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._ViewButton.Name = "_ViewButton";
			this._ViewButton.Size = new System.Drawing.Size(52, 22);
			this._ViewButton.Text = "查看";
			this._ViewButton.ToolTipText = "查看流对象";
			// 
			// _AddObjectMenu
			// 
			this._AddObjectMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._AddArrayNode,
            this._AddDictNode,
            this._AddBooleanNode,
            this._AddStringNode,
            this._AddNumberNode,
            this._AddNameNode});
			this._AddObjectMenu.Enabled = false;
			this._AddObjectMenu.Image = global::PDFPatcher.Properties.Resources.AddChildNode;
			this._AddObjectMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddObjectMenu.Name = "_AddObjectMenu";
			this._AddObjectMenu.Size = new System.Drawing.Size(97, 22);
			this._AddObjectMenu.Text = "插入子节点";
			this._AddObjectMenu.ToolTipText = "添加子节点对象";
			this._AddObjectMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._AddObjectMenu_DropDownItemClicked);
			// 
			// _AddArrayNode
			// 
			this._AddArrayNode.Name = "_AddArrayNode";
			this._AddArrayNode.Size = new System.Drawing.Size(136, 22);
			this._AddArrayNode.Text = "列表节点";
			// 
			// _AddDictNode
			// 
			this._AddDictNode.Name = "_AddDictNode";
			this._AddDictNode.Size = new System.Drawing.Size(136, 22);
			this._AddDictNode.Text = "字典节点";
			// 
			// _AddBooleanNode
			// 
			this._AddBooleanNode.Name = "_AddBooleanNode";
			this._AddBooleanNode.Size = new System.Drawing.Size(136, 22);
			this._AddBooleanNode.Text = "真假值节点";
			// 
			// _AddStringNode
			// 
			this._AddStringNode.Name = "_AddStringNode";
			this._AddStringNode.Size = new System.Drawing.Size(136, 22);
			this._AddStringNode.Text = "字符串节点";
			// 
			// _AddNumberNode
			// 
			this._AddNumberNode.Name = "_AddNumberNode";
			this._AddNumberNode.Size = new System.Drawing.Size(136, 22);
			this._AddNumberNode.Text = "数值节点";
			// 
			// _AddNameNode
			// 
			this._AddNameNode.Name = "_AddNameNode";
			this._AddNameNode.Size = new System.Drawing.Size(136, 22);
			this._AddNameNode.Text = "名称节点";
			// 
			// _DeleteButton
			// 
			this._DeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._DeleteButton.Enabled = false;
			this._DeleteButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._DeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._DeleteButton.Name = "_DeleteButton";
			this._DeleteButton.Size = new System.Drawing.Size(23, 22);
			this._DeleteButton.Text = "删除";
			this._DeleteButton.ToolTipText = "删除选中的对象";
			// 
			// _ExpandButton
			// 
			this._ExpandButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._ExpandButton.Enabled = false;
			this._ExpandButton.Image = global::PDFPatcher.Properties.Resources.Expand;
			this._ExpandButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._ExpandButton.Name = "_ExpandButton";
			this._ExpandButton.Size = new System.Drawing.Size(23, 22);
			this._ExpandButton.Text = "展开";
			this._ExpandButton.ToolTipText = "展开选中的项目";
			// 
			// _CollapseButton
			// 
			this._CollapseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._CollapseButton.Enabled = false;
			this._CollapseButton.Image = global::PDFPatcher.Properties.Resources.Collapse;
			this._CollapseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._CollapseButton.Name = "_CollapseButton";
			this._CollapseButton.Size = new System.Drawing.Size(23, 22);
			this._CollapseButton.Text = "收拢";
			this._CollapseButton.ToolTipText = "收拢选中的项目";
			// 
			// DocumentInspectorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._MainToolbar);
			this.Controls.Add(this._Container);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "DocumentInspectorControl";
			this.Size = new System.Drawing.Size(495, 341);
			this._Container.Panel1.ResumeLayout(false);
			this._Container.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._Container)).EndInit();
			this._Container.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._ObjectDetailBox)).EndInit();
			this._MainToolbar.ResumeLayout(false);
			this._MainToolbar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer _Container;
		private BrightIdeasSoftware.TreeListView _ObjectDetailBox;
		private BrightIdeasSoftware.OLVColumn _NameColumn;
		private BrightIdeasSoftware.OLVColumn _ValueColumn;
		private RichTextBoxLinks.RichTextBoxEx _DescriptionBox;
		private System.Windows.Forms.ContextMenuStrip _RecentFileMenu;
		private System.Windows.Forms.ImageList _ObjectTypeIcons;
		private BrightIdeasSoftware.OLVColumn _DescriptionColumn;
		private System.ComponentModel.BackgroundWorker _LoadDocumentWorker;
		private System.Windows.Forms.ToolStripSplitButton _OpenButton;
		private System.Windows.Forms.ToolStrip _MainToolbar;
		private System.Windows.Forms.ToolStripButton _SaveButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripDropDownButton _ExportButton;
		private System.Windows.Forms.ToolStripButton _ViewButton;
		private System.Windows.Forms.ToolStripButton _DeleteButton;
		private System.Windows.Forms.ToolStripButton _ExpandButton;
		private System.Windows.Forms.ToolStripButton _CollapseButton;
		private System.Windows.Forms.ToolStripDropDownButton _AddObjectMenu;
		private System.Windows.Forms.ToolStripMenuItem _AddNameNode;
		private System.Windows.Forms.ToolStripMenuItem _AddNumberNode;
		private System.Windows.Forms.ToolStripMenuItem _AddStringNode;
		private System.Windows.Forms.ToolStripMenuItem _AddBooleanNode;
		private System.Windows.Forms.ToolStripMenuItem _AddDictNode;
		private System.Windows.Forms.ToolStripMenuItem _AddArrayNode;
	}
}
