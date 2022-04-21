namespace PDFPatcher.Functions
{
	partial class FontFilterForm
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
			this.components = new System.ComponentModel.Container ();
			this._OkButton = new System.Windows.Forms.Button ();
			this._CancelButton = new System.Windows.Forms.Button ();
			this._MessageLabel = new System.Windows.Forms.Label ();
			this._FontNameSizeColumn = new BrightIdeasSoftware.OLVColumn ();
			this._FirstPageColumn = new BrightIdeasSoftware.OLVColumn ();
			this._CountColumn = new BrightIdeasSoftware.OLVColumn ();
			this._FontInfoBox = new BrightIdeasSoftware.TreeListView ();
			this._AddFilterMenu = new System.Windows.Forms.ContextMenuStrip (this.components);
			this._FilterBox = new BrightIdeasSoftware.ObjectListView ();
			this._ConditionColumn = new BrightIdeasSoftware.OLVColumn ();
			this._AddConditionButton = new System.Windows.Forms.Button ();
			this._RemoveConditionButton = new System.Windows.Forms.Button ();
			((System.ComponentModel.ISupportInitialize)(this._FontInfoBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._FilterBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point (368, 376);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size (75, 23);
			this._OkButton.TabIndex = 5;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler (this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point (449, 376);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 6;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// _MessageLabel
			// 
			this._MessageLabel.AutoSize = true;
			this._MessageLabel.Location = new System.Drawing.Point (12, 9);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size (407, 12);
			this._MessageLabel.TabIndex = 0;
			this._MessageLabel.Text = "下表列出了 PDF 文档中所使用的字体。右键点击项目可添加字体筛选条件。";
			// 
			// _FontNameSizeColumn
			// 
			this._FontNameSizeColumn.Text = "字体名称/文本尺寸（首次出现文本）";
			this._FontNameSizeColumn.Width = 329;
			// 
			// _FirstPageColumn
			// 
			this._FirstPageColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._FirstPageColumn.Text = "首次出现页码";
			this._FirstPageColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._FirstPageColumn.Width = 51;
			// 
			// _CountColumn
			// 
			this._CountColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._CountColumn.Text = "出现次数";
			this._CountColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._CountColumn.Width = 52;
			// 
			// _FontInfoBox
			// 
			this._FontInfoBox.AllColumns.Add (this._FontNameSizeColumn);
			this._FontInfoBox.AllColumns.Add (this._FirstPageColumn);
			this._FontInfoBox.AllColumns.Add (this._CountColumn);
			this._FontInfoBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._FontInfoBox.CheckBoxes = false;
			this._FontInfoBox.Columns.AddRange (new System.Windows.Forms.ColumnHeader[] {
            this._FontNameSizeColumn,
            this._FirstPageColumn,
            this._CountColumn});
			this._FontInfoBox.ContextMenuStrip = this._AddFilterMenu;
			this._FontInfoBox.FullRowSelect = true;
			this._FontInfoBox.GridLines = true;
			this._FontInfoBox.HeaderWordWrap = true;
			this._FontInfoBox.Location = new System.Drawing.Point (14, 36);
			this._FontInfoBox.MultiSelect = false;
			this._FontInfoBox.Name = "_FontInfoBox";
			this._FontInfoBox.OwnerDraw = true;
			this._FontInfoBox.ShowGroups = false;
			this._FontInfoBox.Size = new System.Drawing.Size (510, 206);
			this._FontInfoBox.TabIndex = 1;
			this._FontInfoBox.UseCompatibleStateImageBehavior = false;
			this._FontInfoBox.View = System.Windows.Forms.View.Details;
			this._FontInfoBox.VirtualMode = true;
			// 
			// _AddFilterMenu
			// 
			this._AddFilterMenu.Name = "_AddFilterMenu";
			this._AddFilterMenu.ShowImageMargin = false;
			this._AddFilterMenu.Size = new System.Drawing.Size (36, 4);
			this._AddFilterMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler (this._AddFilterMenu_ItemClicked);
			this._AddFilterMenu.Opening += new System.ComponentModel.CancelEventHandler (this._AddFilterMenu_Opening);
			// 
			// _FilterBox
			// 
			this._FilterBox.AllColumns.Add (this._ConditionColumn);
			this._FilterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._FilterBox.Columns.AddRange (new System.Windows.Forms.ColumnHeader[] {
            this._ConditionColumn});
			this._FilterBox.FullRowSelect = true;
			this._FilterBox.GridLines = true;
			this._FilterBox.Location = new System.Drawing.Point (14, 248);
			this._FilterBox.Name = "_FilterBox";
			this._FilterBox.ShowGroups = false;
			this._FilterBox.Size = new System.Drawing.Size (415, 122);
			this._FilterBox.TabIndex = 2;
			this._FilterBox.UseCompatibleStateImageBehavior = false;
			this._FilterBox.View = System.Windows.Forms.View.Details;
			// 
			// _ConditionColumn
			// 
			this._ConditionColumn.Text = "筛选条件";
			this._ConditionColumn.Width = 330;
			// 
			// _AddConditionButton
			// 
			this._AddConditionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._AddConditionButton.Location = new System.Drawing.Point (435, 248);
			this._AddConditionButton.Name = "_AddConditionButton";
			this._AddConditionButton.Size = new System.Drawing.Size (89, 23);
			this._AddConditionButton.TabIndex = 3;
			this._AddConditionButton.Text = "添加筛选条件";
			this._AddConditionButton.UseVisualStyleBackColor = true;
			this._AddConditionButton.Click += new System.EventHandler (this.ControlEvent);
			// 
			// _RemoveConditionButton
			// 
			this._RemoveConditionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._RemoveConditionButton.Location = new System.Drawing.Point (435, 277);
			this._RemoveConditionButton.Name = "_RemoveConditionButton";
			this._RemoveConditionButton.Size = new System.Drawing.Size (89, 23);
			this._RemoveConditionButton.TabIndex = 4;
			this._RemoveConditionButton.Text = "删除筛选条件";
			this._RemoveConditionButton.UseVisualStyleBackColor = true;
			this._RemoveConditionButton.Click += new System.EventHandler (this.ControlEvent);
			// 
			// FontFilterForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (536, 411);
			this.Controls.Add (this._RemoveConditionButton);
			this.Controls.Add (this._AddConditionButton);
			this.Controls.Add (this._FontInfoBox);
			this.Controls.Add (this._MessageLabel);
			this.Controls.Add (this._FilterBox);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._OkButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FontFilterForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "添加字体筛选条件";
			((System.ComponentModel.ISupportInitialize)(this._FontInfoBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._FilterBox)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label _MessageLabel;
		private BrightIdeasSoftware.OLVColumn _FontNameSizeColumn;
		private BrightIdeasSoftware.OLVColumn _FirstPageColumn;
		private BrightIdeasSoftware.OLVColumn _CountColumn;
		private BrightIdeasSoftware.TreeListView _FontInfoBox;
		private System.Windows.Forms.ContextMenuStrip _AddFilterMenu;
		private BrightIdeasSoftware.ObjectListView _FilterBox;
		private BrightIdeasSoftware.OLVColumn _ConditionColumn;
		private System.Windows.Forms.Button _AddConditionButton;
		private System.Windows.Forms.Button _RemoveConditionButton;
	}
}

