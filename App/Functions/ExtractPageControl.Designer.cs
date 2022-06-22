namespace PDFPatcher.Functions
{
	partial class ExtractPageControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtractPageControl));
			this._ExtractPageRangeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._KeepBookmarkBox = new System.Windows.Forms.CheckBox();
			this._RemoveOrphanBoomarksBox = new System.Windows.Forms.CheckBox();
			this._KeepDocInfoPropertyBox = new System.Windows.Forms.CheckBox();
			this._SourceFileControl = new PDFPatcher.SourceFileControl();
			this._TargetFileControl = new PDFPatcher.TargetFileControl();
			this._RemoveRestrictionBox = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this._ExcludePageRangeBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._SeparatingModeBox = new System.Windows.Forms.ComboBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._NumberFileNamesBox = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this._SeperateByPageNumberBox = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this._ExtractButton = new EnhancedGlassButton.GlassButton();
			this._EnableFullCompression = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._SeperateByPageNumberBox)).BeginInit();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _ExtractPageRangeBox
			// 
			this._ExtractPageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ExtractPageRangeBox.Location = new System.Drawing.Point(112, 65);
			this._ExtractPageRangeBox.Name = "_ExtractPageRangeBox";
			this._ExtractPageRangeBox.Size = new System.Drawing.Size(454, 21);
			this._ExtractPageRangeBox.TabIndex = 3;
			this._ExtractPageRangeBox.TextChanged += new System.EventHandler(this._ExtractPageRangeBox_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "页码范围：";
			// 
			// _KeepBookmarkBox
			// 
			this._KeepBookmarkBox.AutoSize = true;
			this._KeepBookmarkBox.Location = new System.Drawing.Point(6, 41);
			this._KeepBookmarkBox.Name = "_KeepBookmarkBox";
			this._KeepBookmarkBox.Size = new System.Drawing.Size(120, 16);
			this._KeepBookmarkBox.TabIndex = 1;
			this._KeepBookmarkBox.Text = "保留原文档的书签";
			this._KeepBookmarkBox.UseVisualStyleBackColor = true;
			// 
			// _RemoveOrphanBoomarksBox
			// 
			this._RemoveOrphanBoomarksBox.AutoSize = true;
			this._RemoveOrphanBoomarksBox.Location = new System.Drawing.Point(6, 85);
			this._RemoveOrphanBoomarksBox.Name = "_RemoveOrphanBoomarksBox";
			this._RemoveOrphanBoomarksBox.Size = new System.Drawing.Size(168, 16);
			this._RemoveOrphanBoomarksBox.TabIndex = 3;
			this._RemoveOrphanBoomarksBox.Text = "删除连接到无效页面的书签";
			this._RemoveOrphanBoomarksBox.UseVisualStyleBackColor = true;
			// 
			// _KeepDocInfoPropertyBox
			// 
			this._KeepDocInfoPropertyBox.AutoSize = true;
			this._KeepDocInfoPropertyBox.Location = new System.Drawing.Point(6, 19);
			this._KeepDocInfoPropertyBox.Name = "_KeepDocInfoPropertyBox";
			this._KeepDocInfoPropertyBox.Size = new System.Drawing.Size(120, 16);
			this._KeepDocInfoPropertyBox.TabIndex = 0;
			this._KeepDocInfoPropertyBox.Text = "保留原文档的属性";
			this._KeepDocInfoPropertyBox.UseVisualStyleBackColor = true;
			// 
			// _SourceFileControl
			// 
			this._SourceFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SourceFileControl.Location = new System.Drawing.Point(12, 3);
			this._SourceFileControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._SourceFileControl.Name = "_SourceFileControl";
			this._SourceFileControl.Size = new System.Drawing.Size(559, 29);
			this._SourceFileControl.TabIndex = 0;
			// 
			// _TargetFileControl
			// 
			this._TargetFileControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TargetFileControl.Location = new System.Drawing.Point(12, 33);
			this._TargetFileControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TargetFileControl.Name = "_TargetFileControl";
			this._TargetFileControl.Size = new System.Drawing.Size(559, 29);
			this._TargetFileControl.TabIndex = 1;
			// 
			// _RemoveRestrictionBox
			// 
			this._RemoveRestrictionBox.AutoSize = true;
			this._RemoveRestrictionBox.Location = new System.Drawing.Point(6, 63);
			this._RemoveRestrictionBox.Name = "_RemoveRestrictionBox";
			this._RemoveRestrictionBox.Size = new System.Drawing.Size(120, 16);
			this._RemoveRestrictionBox.TabIndex = 2;
			this._RemoveRestrictionBox.Text = "解除原文档的限制";
			this._RemoveRestrictionBox.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(17, 97);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "排除页码范围：";
			// 
			// _ExcludePageRangeBox
			// 
			this._ExcludePageRangeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ExcludePageRangeBox.Location = new System.Drawing.Point(112, 94);
			this._ExcludePageRangeBox.Name = "_ExcludePageRangeBox";
			this._ExcludePageRangeBox.Size = new System.Drawing.Size(325, 21);
			this._ExcludePageRangeBox.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "拆分方式：";
			// 
			// _SeparatingModeBox
			// 
			this._SeparatingModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._SeparatingModeBox.FormattingEnabled = true;
			this._SeparatingModeBox.Items.AddRange(new object[] {
            "按页码范围的分号标记拆分",
            "按顶层书签拆分",
            "按页数拆分"});
			this._SeparatingModeBox.Location = new System.Drawing.Point(89, 20);
			this._SeparatingModeBox.Name = "_SeparatingModeBox";
			this._SeparatingModeBox.Size = new System.Drawing.Size(217, 20);
			this._SeparatingModeBox.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 121);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(559, 197);
			this.tabControl1.TabIndex = 7;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage1.Size = new System.Drawing.Size(551, 171);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "选项";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._KeepDocInfoPropertyBox);
			this.groupBox2.Controls.Add(this._EnableFullCompression);
			this.groupBox2.Controls.Add(this._RemoveOrphanBoomarksBox);
			this.groupBox2.Controls.Add(this._RemoveRestrictionBox);
			this.groupBox2.Controls.Add(this._KeepBookmarkBox);
			this.groupBox2.Location = new System.Drawing.Point(6, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 135);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "文档";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._NumberFileNamesBox);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this._SeperateByPageNumberBox);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._SeparatingModeBox);
			this.groupBox1.Location = new System.Drawing.Point(212, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(317, 111);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "拆分文档";
			// 
			// _NumberFileNamesBox
			// 
			this._NumberFileNamesBox.AutoSize = true;
			this._NumberFileNamesBox.Location = new System.Drawing.Point(24, 70);
			this._NumberFileNamesBox.Name = "_NumberFileNamesBox";
			this._NumberFileNamesBox.Size = new System.Drawing.Size(144, 16);
			this._NumberFileNamesBox.TabIndex = 6;
			this._NumberFileNamesBox.Text = "在文件名前面添加编号";
			this._NumberFileNamesBox.UseVisualStyleBackColor = true;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(234, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(17, 12);
			this.label5.TabIndex = 4;
			this.label5.Text = "页";
			// 
			// _SeperateByPageNumberBox
			// 
			this._SeperateByPageNumberBox.Location = new System.Drawing.Point(153, 46);
			this._SeperateByPageNumberBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this._SeperateByPageNumberBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._SeperateByPageNumberBox.Name = "_SeperateByPageNumberBox";
			this._SeperateByPageNumberBox.Size = new System.Drawing.Size(75, 21);
			this._SeperateByPageNumberBox.TabIndex = 3;
			this._SeperateByPageNumberBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(22, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(125, 12);
			this.label4.TabIndex = 2;
			this.label4.Text = "按页数拆分：每个文档";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.textBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPage2.Size = new System.Drawing.Size(551, 171);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "关于页码范围的说明";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(6, 6);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(532, 159);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// _ExtractButton
			// 
			this._ExtractButton.AlternativeFocusBorderColor = System.Drawing.SystemColors.Highlight;
			this._ExtractButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ExtractButton.AnimateGlow = true;
			this._ExtractButton.BackColor = System.Drawing.SystemColors.Highlight;
			this._ExtractButton.CornerRadius = 3;
			this._ExtractButton.ForeColor = System.Drawing.SystemColors.ControlText;
			this._ExtractButton.GlowColor = System.Drawing.Color.White;
			this._ExtractButton.Image = global::PDFPatcher.Properties.Resources.Save;
			this._ExtractButton.InnerBorderColor = System.Drawing.SystemColors.ControlDarkDark;
			this._ExtractButton.Location = new System.Drawing.Point(443, 92);
			this._ExtractButton.Name = "_ExtractButton";
			this._ExtractButton.OuterBorderColor = System.Drawing.SystemColors.ControlLightLight;
			this._ExtractButton.ShowFocusBorder = true;
			this._ExtractButton.Size = new System.Drawing.Size(123, 29);
			this._ExtractButton.TabIndex = 14;
			this._ExtractButton.Text = " 提取页面(&T)";
			this._ExtractButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._ExtractButton.Click += new System.EventHandler(this._ExtractButton_Click);
			// 
			// _EnableFullCompression
			// 
			this._EnableFullCompression.AutoSize = true;
			this._EnableFullCompression.Location = new System.Drawing.Point(6, 107);
			this._EnableFullCompression.Name = "_EnableFullCompression";
			this._EnableFullCompression.Size = new System.Drawing.Size(132, 16);
			this._EnableFullCompression.TabIndex = 3;
			this._EnableFullCompression.Text = "清理并压缩输出文档";
			this._EnableFullCompression.UseVisualStyleBackColor = true;
			// 
			// ExtractPageControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._ExtractButton);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this._ExcludePageRangeBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._SourceFileControl);
			this.Controls.Add(this._ExtractPageRangeBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._TargetFileControl);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Name = "ExtractPageControl";
			this.Size = new System.Drawing.Size(583, 333);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._SeperateByPageNumberBox)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _ExtractPageRangeBox;
		private System.Windows.Forms.Label label3;
		private SourceFileControl _SourceFileControl;
		private TargetFileControl _TargetFileControl;
		private System.Windows.Forms.CheckBox _KeepBookmarkBox;
		private System.Windows.Forms.CheckBox _RemoveOrphanBoomarksBox;
		private System.Windows.Forms.CheckBox _KeepDocInfoPropertyBox;
		private System.Windows.Forms.CheckBox _RemoveRestrictionBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _ExcludePageRangeBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _SeparatingModeBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown _SeperateByPageNumberBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox _NumberFileNamesBox;
		private EnhancedGlassButton.GlassButton _ExtractButton;
		private System.Windows.Forms.CheckBox _EnableFullCompression;
	}
}
