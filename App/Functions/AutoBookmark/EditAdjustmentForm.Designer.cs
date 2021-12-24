namespace PDFPatcher.Functions
{
	partial class EditAdjustmentForm
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
			this._AddFilterMenuItem = new System.Windows.Forms.ToolStripDropDownButton ();
			this._OkButton = new System.Windows.Forms.Button ();
			this._CancelButton = new System.Windows.Forms.Button ();
			this._FilterBox = new BrightIdeasSoftware.ObjectListView ();
			this._TypeColumn = new BrightIdeasSoftware.OLVColumn ();
			this._ConditionColumn = new BrightIdeasSoftware.OLVColumn ();
			this._IsInclusiveColumn = new BrightIdeasSoftware.OLVColumn ();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip ();
			this._RemoveButton = new System.Windows.Forms.ToolStripButton ();
			this._EditFilterBox = new System.Windows.Forms.GroupBox ();
			this._EditFilterPanel = new System.Windows.Forms.Panel ();
			((System.ComponentModel.ISupportInitialize)(this._FilterBox)).BeginInit ();
			this.toolStrip1.SuspendLayout ();
			this._EditFilterBox.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// _AddFilterMenuItem
			// 
			this._AddFilterMenuItem.Image = global::PDFPatcher.Properties.Resources.Add;
			this._AddFilterMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._AddFilterMenuItem.Name = "_AddFilterMenuItem";
			this._AddFilterMenuItem.Size = new System.Drawing.Size (108, 22);
			this._AddFilterMenuItem.Text = "添加匹配条件";
			this._AddFilterMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler (this._AddFilterMenuItem_DropDownItemClicked);
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point (267, 285);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size (75, 23);
			this._OkButton.TabIndex = 0;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler (this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point (348, 285);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 1;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// _FilterBox
			// 
			this._FilterBox.AllColumns.Add (this._TypeColumn);
			this._FilterBox.AllColumns.Add (this._ConditionColumn);
			this._FilterBox.AllColumns.Add (this._IsInclusiveColumn);
			this._FilterBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._FilterBox.Columns.AddRange (new System.Windows.Forms.ColumnHeader[] {
            this._TypeColumn,
            this._ConditionColumn,
            this._IsInclusiveColumn});
			this._FilterBox.FullRowSelect = true;
			this._FilterBox.GridLines = true;
			this._FilterBox.HideSelection = false;
			this._FilterBox.IsSimpleDragSource = true;
			this._FilterBox.IsSimpleDropSink = true;
			this._FilterBox.Location = new System.Drawing.Point (12, 28);
			this._FilterBox.MultiSelect = false;
			this._FilterBox.Name = "_FilterBox";
			this._FilterBox.ShowGroups = false;
			this._FilterBox.Size = new System.Drawing.Size (411, 141);
			this._FilterBox.TabIndex = 3;
			this._FilterBox.UseCompatibleStateImageBehavior = false;
			this._FilterBox.View = System.Windows.Forms.View.Details;
			this._FilterBox.SelectedIndexChanged += new System.EventHandler (this._FilterBox_SelectedIndexChanged);
			// 
			// _TypeColumn
			// 
			this._TypeColumn.IsEditable = false;
			this._TypeColumn.Text = "筛选条件";
			this._TypeColumn.Width = 69;
			// 
			// _ConditionColumn
			// 
			this._ConditionColumn.FillsFreeSpace = true;
			this._ConditionColumn.Text = "匹配条件";
			this._ConditionColumn.Width = 241;
			// 
			// _IsInclusiveColumn
			// 
			this._IsInclusiveColumn.IsEditable = false;
			this._IsInclusiveColumn.Text = "包含筛选";
			this._IsInclusiveColumn.Width = 78;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this._AddFilterMenuItem,
            this._RemoveButton});
			this.toolStrip1.Location = new System.Drawing.Point (0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size (435, 25);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// _RemoveButton
			// 
			this._RemoveButton.Image = global::PDFPatcher.Properties.Resources.Delete;
			this._RemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._RemoveButton.Name = "_RemoveButton";
			this._RemoveButton.Size = new System.Drawing.Size (51, 22);
			this._RemoveButton.Text = "删除";
			this._RemoveButton.Click += new System.EventHandler (this._RemoveButton_Click);
			// 
			// _EditFilterBox
			// 
			this._EditFilterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._EditFilterBox.Controls.Add (this._EditFilterPanel);
			this._EditFilterBox.Location = new System.Drawing.Point (12, 175);
			this._EditFilterBox.Name = "_EditFilterBox";
			this._EditFilterBox.Size = new System.Drawing.Size (411, 104);
			this._EditFilterBox.TabIndex = 5;
			this._EditFilterBox.TabStop = false;
			this._EditFilterBox.Text = "更改匹配条件";
			// 
			// _EditFilterPanel
			// 
			this._EditFilterPanel.Location = new System.Drawing.Point (6, 20);
			this._EditFilterPanel.Name = "_EditFilterPanel";
			this._EditFilterPanel.Size = new System.Drawing.Size (399, 78);
			this._EditFilterPanel.TabIndex = 0;
			// 
			// EditAdjustmentForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (435, 320);
			this.ControlBox = false;
			this.Controls.Add (this._EditFilterBox);
			this.Controls.Add (this.toolStrip1);
			this.Controls.Add (this._FilterBox);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._OkButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size (443, 318);
			this.Name = "EditAdjustmentForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "编辑标题文本筛选条件";
			this.Load += new System.EventHandler (this.EditAdjustmentForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._FilterBox)).EndInit ();
			this.toolStrip1.ResumeLayout (false);
			this.toolStrip1.PerformLayout ();
			this._EditFilterBox.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private BrightIdeasSoftware.ObjectListView _FilterBox;
		private BrightIdeasSoftware.OLVColumn _ConditionColumn;
		private BrightIdeasSoftware.OLVColumn _TypeColumn;
		private BrightIdeasSoftware.OLVColumn _IsInclusiveColumn;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton _RemoveButton;
		private System.Windows.Forms.GroupBox _EditFilterBox;
		private System.Windows.Forms.Panel _EditFilterPanel;
		private System.Windows.Forms.ToolStripDropDownButton _AddFilterMenuItem;
	}
}

