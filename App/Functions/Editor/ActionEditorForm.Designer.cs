namespace PDFPatcher.Functions
{
	partial class ActionEditorForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (ActionEditorForm));
			this._OkButton = new System.Windows.Forms.Button ();
			this._CancelButton = new System.Windows.Forms.Button ();
			this._DestinationPanel = new System.Windows.Forms.GroupBox ();
			this._PathPanel = new System.Windows.Forms.Panel ();
			this._PathBox = new System.Windows.Forms.TextBox ();
			this.label4 = new System.Windows.Forms.Label ();
			this._NewWindowBox = new System.Windows.Forms.CheckBox ();
			this._NamedBox = new System.Windows.Forms.TextBox ();
			this._GotoNamedDestBox = new System.Windows.Forms.RadioButton ();
			this._GotoLocationBox = new System.Windows.Forms.RadioButton ();
			this._LocationPanel = new System.Windows.Forms.Panel ();
			this.label10 = new System.Windows.Forms.Label ();
			this.label3 = new System.Windows.Forms.Label ();
			this._KeepYBox = new System.Windows.Forms.CheckBox ();
			this._PageBox = new System.Windows.Forms.NumericUpDown ();
			this._KeepXBox = new System.Windows.Forms.CheckBox ();
			this.label5 = new System.Windows.Forms.Label ();
			this._ZoomRateBox = new System.Windows.Forms.ComboBox ();
			this._LeftBox = new System.Windows.Forms.NumericUpDown ();
			this.label7 = new System.Windows.Forms.Label ();
			this._TopBox = new System.Windows.Forms.NumericUpDown ();
			this.label6 = new System.Windows.Forms.Label ();
			this._RectanglePanel = new System.Windows.Forms.Panel ();
			this.label8 = new System.Windows.Forms.Label ();
			this._WidthBox = new System.Windows.Forms.NumericUpDown ();
			this.label9 = new System.Windows.Forms.Label ();
			this._HeightBox = new System.Windows.Forms.NumericUpDown ();
			this._ActionBox = new System.Windows.Forms.ComboBox ();
			this.label2 = new System.Windows.Forms.Label ();
			this._TitleBox = new System.Windows.Forms.TextBox ();
			this.label1 = new System.Windows.Forms.Label ();
			this.tabControl1 = new System.Windows.Forms.TabControl ();
			this.tabPage1 = new System.Windows.Forms.TabPage ();
			this._DefaultOpenBox = new System.Windows.Forms.CheckBox ();
			this._ScriptBox = new System.Windows.Forms.GroupBox ();
			this._ScriptContentBox = new System.Windows.Forms.TextBox ();
			this.tabPage2 = new System.Windows.Forms.TabPage ();
			this._AttributesBox = new BrightIdeasSoftware.ObjectListView ();
			this._AttrNameColumn = new BrightIdeasSoftware.OLVColumn ();
			this._AttrValueColumn = new BrightIdeasSoftware.OLVColumn ();
			this._DestinationPanel.SuspendLayout ();
			this._PathPanel.SuspendLayout ();
			this._LocationPanel.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._PageBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._LeftBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._TopBox)).BeginInit ();
			this._RectanglePanel.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).BeginInit ();
			this.tabControl1.SuspendLayout ();
			this.tabPage1.SuspendLayout ();
			this._ScriptBox.SuspendLayout ();
			this.tabPage2.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._AttributesBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point (293, 348);
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
			this._CancelButton.Location = new System.Drawing.Point (374, 348);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 1;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// _DestinationPanel
			// 
			this._DestinationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._DestinationPanel.Controls.Add (this._PathPanel);
			this._DestinationPanel.Controls.Add (this._NamedBox);
			this._DestinationPanel.Controls.Add (this._GotoNamedDestBox);
			this._DestinationPanel.Controls.Add (this._GotoLocationBox);
			this._DestinationPanel.Controls.Add (this._LocationPanel);
			this._DestinationPanel.Location = new System.Drawing.Point (6, 61);
			this._DestinationPanel.Name = "_DestinationPanel";
			this._DestinationPanel.Size = new System.Drawing.Size (417, 238);
			this._DestinationPanel.TabIndex = 7;
			this._DestinationPanel.TabStop = false;
			this._DestinationPanel.Text = "目标";
			// 
			// _PathPanel
			// 
			this._PathPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._PathPanel.Controls.Add (this._PathBox);
			this._PathPanel.Controls.Add (this.label4);
			this._PathPanel.Controls.Add (this._NewWindowBox);
			this._PathPanel.Enabled = false;
			this._PathPanel.Location = new System.Drawing.Point (5, 185);
			this._PathPanel.Name = "_PathPanel";
			this._PathPanel.Size = new System.Drawing.Size (406, 47);
			this._PathPanel.TabIndex = 15;
			// 
			// _PathBox
			// 
			this._PathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._PathBox.Location = new System.Drawing.Point (105, 3);
			this._PathBox.Name = "_PathBox";
			this._PathBox.Size = new System.Drawing.Size (298, 21);
			this._PathBox.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point (13, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size (89, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "外部文档路径：";
			// 
			// _NewWindowBox
			// 
			this._NewWindowBox.AutoSize = true;
			this._NewWindowBox.Location = new System.Drawing.Point (105, 30);
			this._NewWindowBox.Name = "_NewWindowBox";
			this._NewWindowBox.Size = new System.Drawing.Size (96, 16);
			this._NewWindowBox.TabIndex = 4;
			this._NewWindowBox.Text = "在新窗口打开";
			this._NewWindowBox.UseVisualStyleBackColor = true;
			// 
			// _NamedBox
			// 
			this._NamedBox.Enabled = false;
			this._NamedBox.Location = new System.Drawing.Point (110, 158);
			this._NamedBox.Name = "_NamedBox";
			this._NamedBox.Size = new System.Drawing.Size (215, 21);
			this._NamedBox.TabIndex = 14;
			// 
			// _GotoNamedDestBox
			// 
			this._GotoNamedDestBox.AutoSize = true;
			this._GotoNamedDestBox.Location = new System.Drawing.Point (9, 159);
			this._GotoNamedDestBox.Name = "_GotoNamedDestBox";
			this._GotoNamedDestBox.Size = new System.Drawing.Size (95, 16);
			this._GotoNamedDestBox.TabIndex = 13;
			this._GotoNamedDestBox.TabStop = true;
			this._GotoNamedDestBox.Text = "转到命名位置";
			this._GotoNamedDestBox.UseVisualStyleBackColor = true;
			this._GotoNamedDestBox.CheckedChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// _GotoLocationBox
			// 
			this._GotoLocationBox.AutoSize = true;
			this._GotoLocationBox.Location = new System.Drawing.Point (6, 20);
			this._GotoLocationBox.Name = "_GotoLocationBox";
			this._GotoLocationBox.Size = new System.Drawing.Size (95, 16);
			this._GotoLocationBox.TabIndex = 12;
			this._GotoLocationBox.TabStop = true;
			this._GotoLocationBox.Text = "转到指定位置";
			this._GotoLocationBox.UseVisualStyleBackColor = true;
			this._GotoLocationBox.CheckedChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// _LocationPanel
			// 
			this._LocationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._LocationPanel.Controls.Add (this.label10);
			this._LocationPanel.Controls.Add (this.label3);
			this._LocationPanel.Controls.Add (this._KeepYBox);
			this._LocationPanel.Controls.Add (this._PageBox);
			this._LocationPanel.Controls.Add (this._KeepXBox);
			this._LocationPanel.Controls.Add (this.label5);
			this._LocationPanel.Controls.Add (this._ZoomRateBox);
			this._LocationPanel.Controls.Add (this._LeftBox);
			this._LocationPanel.Controls.Add (this.label7);
			this._LocationPanel.Controls.Add (this._TopBox);
			this._LocationPanel.Controls.Add (this.label6);
			this._LocationPanel.Controls.Add (this._RectanglePanel);
			this._LocationPanel.Enabled = false;
			this._LocationPanel.Location = new System.Drawing.Point (39, 42);
			this._LocationPanel.Name = "_LocationPanel";
			this._LocationPanel.Size = new System.Drawing.Size (372, 110);
			this._LocationPanel.TabIndex = 11;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point (173, 86);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size (95, 12);
			this.label10.TabIndex = 11;
			this.label10.Text = "（0：保持不变）";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (0, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (65, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "目标页面：";
			// 
			// _KeepYBox
			// 
			this._KeepYBox.AutoSize = true;
			this._KeepYBox.Location = new System.Drawing.Point (145, 57);
			this._KeepYBox.Name = "_KeepYBox";
			this._KeepYBox.Size = new System.Drawing.Size (48, 16);
			this._KeepYBox.TabIndex = 7;
			this._KeepYBox.Text = "默认";
			this._KeepYBox.UseVisualStyleBackColor = true;
			this._KeepYBox.CheckedChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// _PageBox
			// 
			this._PageBox.Location = new System.Drawing.Point (71, 3);
			this._PageBox.Maximum = new decimal (new int[] {
            9999999,
            0,
            0,
            0});
			this._PageBox.Minimum = new decimal (new int[] {
            1,
            0,
            0,
            0});
			this._PageBox.Name = "_PageBox";
			this._PageBox.Size = new System.Drawing.Size (68, 21);
			this._PageBox.TabIndex = 1;
			this._PageBox.Value = new decimal (new int[] {
            1,
            0,
            0,
            0});
			// 
			// _KeepXBox
			// 
			this._KeepXBox.AutoSize = true;
			this._KeepXBox.Location = new System.Drawing.Point (145, 30);
			this._KeepXBox.Name = "_KeepXBox";
			this._KeepXBox.Size = new System.Drawing.Size (48, 16);
			this._KeepXBox.TabIndex = 4;
			this._KeepXBox.Text = "默认";
			this._KeepXBox.UseVisualStyleBackColor = true;
			this._KeepXBox.CheckedChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point (12, 31);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (53, 12);
			this.label5.TabIndex = 2;
			this.label5.Text = "横坐标：";
			// 
			// _ZoomRateBox
			// 
			this._ZoomRateBox.FormattingEnabled = true;
			this._ZoomRateBox.Location = new System.Drawing.Point (71, 83);
			this._ZoomRateBox.Name = "_ZoomRateBox";
			this._ZoomRateBox.Size = new System.Drawing.Size (96, 20);
			this._ZoomRateBox.TabIndex = 10;
			this._ZoomRateBox.SelectedIndexChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// _LeftBox
			// 
			this._LeftBox.DecimalPlaces = 2;
			this._LeftBox.Location = new System.Drawing.Point (71, 29);
			this._LeftBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._LeftBox.Minimum = new decimal (new int[] {
            10000,
            0,
            0,
            -2147483648});
			this._LeftBox.Name = "_LeftBox";
			this._LeftBox.Size = new System.Drawing.Size (68, 21);
			this._LeftBox.TabIndex = 3;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point (2, 86);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (65, 12);
			this.label7.TabIndex = 9;
			this.label7.Text = "缩放比例：";
			// 
			// _TopBox
			// 
			this._TopBox.DecimalPlaces = 2;
			this._TopBox.Location = new System.Drawing.Point (71, 56);
			this._TopBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._TopBox.Minimum = new decimal (new int[] {
            10000,
            0,
            0,
            -2147483648});
			this._TopBox.Name = "_TopBox";
			this._TopBox.Size = new System.Drawing.Size (68, 21);
			this._TopBox.TabIndex = 6;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point (12, 58);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size (53, 12);
			this.label6.TabIndex = 5;
			this.label6.Text = "纵坐标：";
			// 
			// _RectanglePanel
			// 
			this._RectanglePanel.Controls.Add (this.label8);
			this._RectanglePanel.Controls.Add (this._WidthBox);
			this._RectanglePanel.Controls.Add (this.label9);
			this._RectanglePanel.Controls.Add (this._HeightBox);
			this._RectanglePanel.Enabled = false;
			this._RectanglePanel.Location = new System.Drawing.Point (229, 27);
			this._RectanglePanel.Name = "_RectanglePanel";
			this._RectanglePanel.Size = new System.Drawing.Size (128, 56);
			this._RectanglePanel.TabIndex = 8;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point (3, 4);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size (41, 12);
			this.label8.TabIndex = 0;
			this.label8.Text = "宽度：";
			// 
			// _WidthBox
			// 
			this._WidthBox.DecimalPlaces = 2;
			this._WidthBox.Location = new System.Drawing.Point (54, 2);
			this._WidthBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._WidthBox.Name = "_WidthBox";
			this._WidthBox.Size = new System.Drawing.Size (68, 21);
			this._WidthBox.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point (3, 31);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size (41, 12);
			this.label9.TabIndex = 2;
			this.label9.Text = "高度：";
			// 
			// _HeightBox
			// 
			this._HeightBox.DecimalPlaces = 2;
			this._HeightBox.Location = new System.Drawing.Point (54, 29);
			this._HeightBox.Maximum = new decimal (new int[] {
            10000,
            0,
            0,
            0});
			this._HeightBox.Name = "_HeightBox";
			this._HeightBox.Size = new System.Drawing.Size (68, 21);
			this._HeightBox.TabIndex = 3;
			// 
			// _ActionBox
			// 
			this._ActionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ActionBox.FormattingEnabled = true;
			this._ActionBox.Location = new System.Drawing.Point (49, 35);
			this._ActionBox.Name = "_ActionBox";
			this._ActionBox.Size = new System.Drawing.Size (156, 20);
			this._ActionBox.TabIndex = 6;
			this._ActionBox.SelectedIndexChanged += new System.EventHandler (this.Control_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (6, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (41, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "动作：";
			// 
			// _TitleBox
			// 
			this._TitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._TitleBox.Location = new System.Drawing.Point (49, 10);
			this._TitleBox.Name = "_TitleBox";
			this._TitleBox.Size = new System.Drawing.Size (374, 21);
			this._TitleBox.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (6, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (41, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "名称：";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add (this.tabPage1);
			this.tabControl1.Controls.Add (this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point (12, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size (437, 330);
			this.tabControl1.TabIndex = 8;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add (this._DefaultOpenBox);
			this.tabPage1.Controls.Add (this._TitleBox);
			this.tabPage1.Controls.Add (this.label2);
			this.tabPage1.Controls.Add (this.label1);
			this.tabPage1.Controls.Add (this._ActionBox);
			this.tabPage1.Controls.Add (this._DestinationPanel);
			this.tabPage1.Controls.Add (this._ScriptBox);
			this.tabPage1.Location = new System.Drawing.Point (4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding (3);
			this.tabPage1.Size = new System.Drawing.Size (429, 304);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "常规";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// _DefaultOpenBox
			// 
			this._DefaultOpenBox.AutoSize = true;
			this._DefaultOpenBox.Location = new System.Drawing.Point (211, 37);
			this._DefaultOpenBox.Name = "_DefaultOpenBox";
			this._DefaultOpenBox.Size = new System.Drawing.Size (96, 16);
			this._DefaultOpenBox.TabIndex = 9;
			this._DefaultOpenBox.Text = "默认打开书签";
			this._DefaultOpenBox.UseVisualStyleBackColor = true;
			// 
			// _ScriptBox
			// 
			this._ScriptBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._ScriptBox.Controls.Add (this._ScriptContentBox);
			this._ScriptBox.Location = new System.Drawing.Point (348, 37);
			this._ScriptBox.Name = "_ScriptBox";
			this._ScriptBox.Size = new System.Drawing.Size (75, 49);
			this._ScriptBox.TabIndex = 8;
			this._ScriptBox.TabStop = false;
			this._ScriptBox.Text = "脚本内容";
			this._ScriptBox.Visible = false;
			// 
			// _ScriptContentBox
			// 
			this._ScriptContentBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._ScriptContentBox.Location = new System.Drawing.Point (6, 20);
			this._ScriptContentBox.Multiline = true;
			this._ScriptContentBox.Name = "_ScriptContentBox";
			this._ScriptContentBox.Size = new System.Drawing.Size (63, 23);
			this._ScriptContentBox.TabIndex = 16;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add (this._AttributesBox);
			this.tabPage2.Location = new System.Drawing.Point (4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding (3);
			this.tabPage2.Size = new System.Drawing.Size (429, 304);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "属性";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// _AttributesBox
			// 
			this._AttributesBox.AllColumns.Add (this._AttrNameColumn);
			this._AttributesBox.AllColumns.Add (this._AttrValueColumn);
			this._AttributesBox.Columns.AddRange (new System.Windows.Forms.ColumnHeader[] {
            this._AttrNameColumn,
            this._AttrValueColumn});
			this._AttributesBox.GridLines = true;
			this._AttributesBox.Location = new System.Drawing.Point (6, 6);
			this._AttributesBox.Name = "_AttributesBox";
			this._AttributesBox.ShowGroups = false;
			this._AttributesBox.Size = new System.Drawing.Size (417, 293);
			this._AttributesBox.TabIndex = 0;
			this._AttributesBox.UseCompatibleStateImageBehavior = false;
			this._AttributesBox.View = System.Windows.Forms.View.Details;
			// 
			// _AttrNameColumn
			// 
			this._AttrNameColumn.Text = "属性名称";
			// 
			// _AttrValueColumn
			// 
			this._AttrValueColumn.FillsFreeSpace = true;
			this._AttrValueColumn.Text = "属性值";
			// 
			// ActionEditorForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (461, 383);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._OkButton);
			this.Controls.Add (this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ActionEditorForm";
			this.ShowInTaskbar = false;
			this.Text = "链接属性编辑器";
			this._DestinationPanel.ResumeLayout (false);
			this._DestinationPanel.PerformLayout ();
			this._PathPanel.ResumeLayout (false);
			this._PathPanel.PerformLayout ();
			this._LocationPanel.ResumeLayout (false);
			this._LocationPanel.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this._PageBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._LeftBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._TopBox)).EndInit ();
			this._RectanglePanel.ResumeLayout (false);
			this._RectanglePanel.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this._WidthBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._HeightBox)).EndInit ();
			this.tabControl1.ResumeLayout (false);
			this.tabPage1.ResumeLayout (false);
			this.tabPage1.PerformLayout ();
			this._ScriptBox.ResumeLayout (false);
			this._ScriptBox.PerformLayout ();
			this.tabPage2.ResumeLayout (false);
			((System.ComponentModel.ISupportInitialize)(this._AttributesBox)).EndInit ();
			this.ResumeLayout (false);

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.GroupBox _DestinationPanel;
		private System.Windows.Forms.ComboBox _ActionBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _TitleBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown _PageBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox _NewWindowBox;
		private System.Windows.Forms.TextBox _PathBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox _KeepYBox;
		private System.Windows.Forms.CheckBox _KeepXBox;
		private System.Windows.Forms.ComboBox _ZoomRateBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown _TopBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown _LeftBox;
		private System.Windows.Forms.NumericUpDown _HeightBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown _WidthBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox _NamedBox;
		private System.Windows.Forms.RadioButton _GotoNamedDestBox;
		private System.Windows.Forms.RadioButton _GotoLocationBox;
		private System.Windows.Forms.Panel _LocationPanel;
		private System.Windows.Forms.Panel _PathPanel;
		private System.Windows.Forms.Panel _RectanglePanel;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private BrightIdeasSoftware.ObjectListView _AttributesBox;
		private BrightIdeasSoftware.OLVColumn _AttrNameColumn;
		private BrightIdeasSoftware.OLVColumn _AttrValueColumn;
		private System.Windows.Forms.TextBox _ScriptContentBox;
		private System.Windows.Forms.GroupBox _ScriptBox;
		private System.Windows.Forms.CheckBox _DefaultOpenBox;
	}
}

