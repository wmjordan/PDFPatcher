namespace PDFPatcher.Functions.Editor
{
	partial class PagePropertyForm
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
			this._CloseButton = new System.Windows.Forms.Button();
			this._PageDimensionBox = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._TopBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._RightBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._BottomBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._LeftBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this._RotationBox = new System.Windows.Forms.TextBox();
			this._MainTab = new System.Windows.Forms.TabControl();
			this._DimensionPage = new System.Windows.Forms.TabPage();
			this._TextStylePage = new System.Windows.Forms.TabPage();
			this._TextStyleBox = new BrightIdeasSoftware.ObjectListView();
			this._FontNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this._SizeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this._WidthBox = new System.Windows.Forms.TextBox();
			this._HeightBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this._MainTab.SuspendLayout();
			this._DimensionPage.SuspendLayout();
			this._TextStylePage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._TextStyleBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _CloseButton
			// 
			this._CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CloseButton.Location = new System.Drawing.Point(368, 282);
			this._CloseButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._CloseButton.Name = "_CloseButton";
			this._CloseButton.Size = new System.Drawing.Size(100, 29);
			this._CloseButton.TabIndex = 1;
			this._CloseButton.Text = "关闭(&G)";
			this._CloseButton.UseVisualStyleBackColor = true;
			// 
			// _PageDimensionBox
			// 
			this._PageDimensionBox.FormattingEnabled = true;
			this._PageDimensionBox.ItemHeight = 15;
			this._PageDimensionBox.Location = new System.Drawing.Point(133, 20);
			this._PageDimensionBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._PageDimensionBox.Name = "_PageDimensionBox";
			this._PageDimensionBox.Size = new System.Drawing.Size(159, 109);
			this._PageDimensionBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(39, 20);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "页面边框：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(301, 20);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 15);
			this.label2.TabIndex = 6;
			this.label2.Text = "上：";
			// 
			// _TopBox
			// 
			this._TopBox.Location = new System.Drawing.Point(348, 16);
			this._TopBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TopBox.Name = "_TopBox";
			this._TopBox.ReadOnly = true;
			this._TopBox.Size = new System.Drawing.Size(69, 25);
			this._TopBox.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(301, 54);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "右：";
			// 
			// _RightBox
			// 
			this._RightBox.Location = new System.Drawing.Point(348, 50);
			this._RightBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._RightBox.Name = "_RightBox";
			this._RightBox.ReadOnly = true;
			this._RightBox.Size = new System.Drawing.Size(69, 25);
			this._RightBox.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 74);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(37, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "下：";
			// 
			// _BottomBox
			// 
			this._BottomBox.Location = new System.Drawing.Point(55, 70);
			this._BottomBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._BottomBox.Name = "_BottomBox";
			this._BottomBox.ReadOnly = true;
			this._BottomBox.Size = new System.Drawing.Size(69, 25);
			this._BottomBox.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(8, 108);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 15);
			this.label5.TabIndex = 4;
			this.label5.Text = "左：";
			// 
			// _LeftBox
			// 
			this._LeftBox.Location = new System.Drawing.Point(55, 104);
			this._LeftBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._LeftBox.Name = "_LeftBox";
			this._LeftBox.ReadOnly = true;
			this._LeftBox.Size = new System.Drawing.Size(69, 25);
			this._LeftBox.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(71, 158);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(52, 15);
			this.label6.TabIndex = 14;
			this.label6.Text = "旋转：";
			// 
			// _RotationBox
			// 
			this._RotationBox.Location = new System.Drawing.Point(133, 154);
			this._RotationBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._RotationBox.Name = "_RotationBox";
			this._RotationBox.ReadOnly = true;
			this._RotationBox.Size = new System.Drawing.Size(53, 25);
			this._RotationBox.TabIndex = 15;
			// 
			// _MainTab
			// 
			this._MainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._MainTab.Controls.Add(this._DimensionPage);
			this._MainTab.Controls.Add(this._TextStylePage);
			this._MainTab.Location = new System.Drawing.Point(16, 15);
			this._MainTab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._MainTab.Name = "_MainTab";
			this._MainTab.SelectedIndex = 0;
			this._MainTab.Size = new System.Drawing.Size(452, 260);
			this._MainTab.TabIndex = 0;
			// 
			// _DimensionPage
			// 
			this._DimensionPage.Controls.Add(this._PageDimensionBox);
			this._DimensionPage.Controls.Add(this._RotationBox);
			this._DimensionPage.Controls.Add(this.label1);
			this._DimensionPage.Controls.Add(this.label6);
			this._DimensionPage.Controls.Add(this.label9);
			this._DimensionPage.Controls.Add(this.label2);
			this._DimensionPage.Controls.Add(this._LeftBox);
			this._DimensionPage.Controls.Add(this._HeightBox);
			this._DimensionPage.Controls.Add(this._TopBox);
			this._DimensionPage.Controls.Add(this.label5);
			this._DimensionPage.Controls.Add(this._WidthBox);
			this._DimensionPage.Controls.Add(this.label4);
			this._DimensionPage.Controls.Add(this.label8);
			this._DimensionPage.Controls.Add(this._RightBox);
			this._DimensionPage.Controls.Add(this.label3);
			this._DimensionPage.Controls.Add(this._BottomBox);
			this._DimensionPage.Location = new System.Drawing.Point(4, 25);
			this._DimensionPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._DimensionPage.Name = "_DimensionPage";
			this._DimensionPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._DimensionPage.Size = new System.Drawing.Size(444, 231);
			this._DimensionPage.TabIndex = 1;
			this._DimensionPage.Text = "页面尺寸";
			this._DimensionPage.UseVisualStyleBackColor = true;
			// 
			// _TextStylePage
			// 
			this._TextStylePage.Controls.Add(this._TextStyleBox);
			this._TextStylePage.Controls.Add(this.label7);
			this._TextStylePage.Location = new System.Drawing.Point(4, 25);
			this._TextStylePage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TextStylePage.Name = "_TextStylePage";
			this._TextStylePage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TextStylePage.Size = new System.Drawing.Size(444, 231);
			this._TextStylePage.TabIndex = 2;
			this._TextStylePage.Text = "文本样式";
			this._TextStylePage.UseVisualStyleBackColor = true;
			// 
			// _TextStyleBox
			// 
			this._TextStyleBox.AllColumns.Add(this._FontNameColumn);
			this._TextStyleBox.AllColumns.Add(this._SizeColumn);
			this._TextStyleBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._FontNameColumn,
            this._SizeColumn});
			this._TextStyleBox.Cursor = System.Windows.Forms.Cursors.Default;
			this._TextStyleBox.Location = new System.Drawing.Point(11, 22);
			this._TextStyleBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TextStyleBox.Name = "_TextStyleBox";
			this._TextStyleBox.ShowGroups = false;
			this._TextStyleBox.Size = new System.Drawing.Size(421, 196);
			this._TextStyleBox.TabIndex = 1;
			this._TextStyleBox.UseCompatibleStateImageBehavior = false;
			this._TextStyleBox.View = System.Windows.Forms.View.Details;
			// 
			// _FontNameColumn
			// 
			this._FontNameColumn.Text = "字体名称";
			this._FontNameColumn.Width = 219;
			// 
			// _SizeColumn
			// 
			this._SizeColumn.Text = "字体尺寸";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 4);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(202, 15);
			this.label7.TabIndex = 0;
			this.label7.Text = "本页面包含如下样式的文本：";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(301, 121);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 15);
			this.label8.TabIndex = 12;
			this.label8.Text = "宽：";
			// 
			// _WidthBox
			// 
			this._WidthBox.Location = new System.Drawing.Point(348, 117);
			this._WidthBox.Margin = new System.Windows.Forms.Padding(4);
			this._WidthBox.Name = "_WidthBox";
			this._WidthBox.ReadOnly = true;
			this._WidthBox.Size = new System.Drawing.Size(69, 25);
			this._WidthBox.TabIndex = 13;
			// 
			// _HeightBox
			// 
			this._HeightBox.Location = new System.Drawing.Point(348, 83);
			this._HeightBox.Margin = new System.Windows.Forms.Padding(4);
			this._HeightBox.Name = "_HeightBox";
			this._HeightBox.ReadOnly = true;
			this._HeightBox.Size = new System.Drawing.Size(69, 25);
			this._HeightBox.TabIndex = 11;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(301, 87);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(37, 15);
			this.label9.TabIndex = 10;
			this.label9.Text = "高：";
			// 
			// PagePropertyForm
			// 
			this.AcceptButton = this._CloseButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 326);
			this.Controls.Add(this._MainTab);
			this.Controls.Add(this._CloseButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PagePropertyForm";
			this.ShowInTaskbar = false;
			this.Text = "页面属性";
			this._MainTab.ResumeLayout(false);
			this._DimensionPage.ResumeLayout(false);
			this._DimensionPage.PerformLayout();
			this._TextStylePage.ResumeLayout(false);
			this._TextStylePage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._TextStyleBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _CloseButton;
		private System.Windows.Forms.ListBox _PageDimensionBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _TopBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _RightBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _BottomBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _LeftBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _RotationBox;
		private System.Windows.Forms.TabControl _MainTab;
		private System.Windows.Forms.TabPage _DimensionPage;
		private System.Windows.Forms.TabPage _TextStylePage;
		private BrightIdeasSoftware.ObjectListView _TextStyleBox;
		private System.Windows.Forms.Label label7;
		private BrightIdeasSoftware.OLVColumn _FontNameColumn;
		private BrightIdeasSoftware.OLVColumn _SizeColumn;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox _HeightBox;
		private System.Windows.Forms.TextBox _WidthBox;
		private System.Windows.Forms.Label label8;
	}
}