namespace PDFPatcher.Functions
{
	partial class CustomizeToolbarForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			this.components = new System.ComponentModel.Container();
			this._ItemListBox = new BrightIdeasSoftware.ObjectListView();
			this._NameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._VisibleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ShowTextColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._DisplayTextColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._ButtonImageList = new System.Windows.Forms.ImageList(this.components);
			this._OkButton = new System.Windows.Forms.Button();
			this._ResetButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._ItemListBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _ItemListBox
			// 
			this._ItemListBox.AllColumns.Add(this._NameColumn);
			this._ItemListBox.AllColumns.Add(this._VisibleColumn);
			this._ItemListBox.AllColumns.Add(this._ShowTextColumn);
			this._ItemListBox.AllColumns.Add(this._DisplayTextColumn);
			this._ItemListBox.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClickAlways;
			this._ItemListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._NameColumn,
            this._VisibleColumn,
            this._ShowTextColumn,
            this._DisplayTextColumn});
			this._ItemListBox.Location = new System.Drawing.Point(12, 46);
			this._ItemListBox.Name = "_ItemListBox";
			this._ItemListBox.OwnerDraw = true;
			this._ItemListBox.ShowGroups = false;
			this._ItemListBox.Size = new System.Drawing.Size(388, 263);
			this._ItemListBox.SmallImageList = this._ButtonImageList;
			this._ItemListBox.TabIndex = 0;
			this._ItemListBox.UseCompatibleStateImageBehavior = false;
			this._ItemListBox.View = System.Windows.Forms.View.Details;
			// 
			// _NameColumn
			// 
			this._NameColumn.IsEditable = false;
			this._NameColumn.Text = "工具栏按钮";
			this._NameColumn.Width = 145;
			// 
			// _VisibleColumn
			// 
			this._VisibleColumn.CheckBoxes = true;
			this._VisibleColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._VisibleColumn.Text = "显示";
			this._VisibleColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._VisibleColumn.Width = 54;
			// 
			// _ShowTextColumn
			// 
			this._ShowTextColumn.CheckBoxes = true;
			this._ShowTextColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._ShowTextColumn.Text = "显示文本";
			this._ShowTextColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this._ShowTextColumn.Width = 63;
			// 
			// _DisplayTextColumn
			// 
			this._DisplayTextColumn.AutoCompleteEditor = false;
			this._DisplayTextColumn.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
			this._DisplayTextColumn.Text = "按钮文本内容";
			this._DisplayTextColumn.Width = 120;
			// 
			// _ButtonImageList
			// 
			this._ButtonImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._ButtonImageList.ImageSize = new System.Drawing.Size(16, 16);
			this._ButtonImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _OkButton
			// 
			this._OkButton.Location = new System.Drawing.Point(325, 315);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 1;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler(this._OkButton_Click);
			// 
			// _ResetButton
			// 
			this._ResetButton.Location = new System.Drawing.Point(12, 315);
			this._ResetButton.Name = "_ResetButton";
			this._ResetButton.Size = new System.Drawing.Size(127, 23);
			this._ResetButton.TabIndex = 2;
			this._ResetButton.Text = "重置常用工具栏";
			this._ResetButton.UseVisualStyleBackColor = true;
			this._ResetButton.Click += new System.EventHandler(this._ResetButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(305, 34);
			this.label1.TabIndex = 3;
			this.label1.Text = "使用鼠标上下拖动项目可调整工具按钮的显示顺序。\r\n要隐藏按钮，请取消“是否显示”的选中状态。";
			// 
			// CustomizeToolbarForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(412, 350);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._ResetButton);
			this.Controls.Add(this._OkButton);
			this.Controls.Add(this._ItemListBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CustomizeToolbarForm";
			this.ShowInTaskbar = false;
			this.Text = "自定义常用工具栏项目";
			this.Load += new System.EventHandler(this.CustomizeToolbarForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._ItemListBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BrightIdeasSoftware.ObjectListView _ItemListBox;
		private System.Windows.Forms.Button _OkButton;
		private BrightIdeasSoftware.OLVColumn _NameColumn;
		private BrightIdeasSoftware.OLVColumn _VisibleColumn;
		private BrightIdeasSoftware.OLVColumn _ShowTextColumn;
		private System.Windows.Forms.Button _ResetButton;
		private System.Windows.Forms.ImageList _ButtonImageList;
		private System.Windows.Forms.Label label1;
		private BrightIdeasSoftware.OLVColumn _DisplayTextColumn;
	}
}