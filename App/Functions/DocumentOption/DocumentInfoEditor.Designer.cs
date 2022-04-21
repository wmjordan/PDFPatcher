namespace PDFPatcher.Functions
{
	partial class DocumentInfoEditor
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
			this.components = new System.ComponentModel.Container ();
			this.groupBox4 = new System.Windows.Forms.GroupBox ();
			this._MetadataPanel = new System.Windows.Forms.Panel ();
			this.label16 = new System.Windows.Forms.Label ();
			this._KeywordsBox = new System.Windows.Forms.TextBox ();
			this.label17 = new System.Windows.Forms.Label ();
			this._SubjectBox = new System.Windows.Forms.TextBox ();
			this.label18 = new System.Windows.Forms.Label ();
			this._AuthorBox = new System.Windows.Forms.TextBox ();
			this.label19 = new System.Windows.Forms.Label ();
			this._TitleBox = new System.Windows.Forms.TextBox ();
			this._ForceMetadataBox = new System.Windows.Forms.CheckBox ();
			this.label5 = new System.Windows.Forms.Label ();
			this._RewriteXmpBox = new System.Windows.Forms.CheckBox ();
			this._PropertyMacroMenu = new PDFPatcher.Functions.MacroMenu (this.components);
			this.groupBox4.SuspendLayout ();
			this._MetadataPanel.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add (this._MetadataPanel);
			this.groupBox4.Controls.Add (this._ForceMetadataBox);
			this.groupBox4.Location = new System.Drawing.Point (6, 23);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size (426, 232);
			this.groupBox4.TabIndex = 2;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "文档信息";
			// 
			// _MetadataPanel
			// 
			this._MetadataPanel.Controls.Add (this._RewriteXmpBox);
			this._MetadataPanel.Controls.Add (this.label16);
			this._MetadataPanel.Controls.Add (this._KeywordsBox);
			this._MetadataPanel.Controls.Add (this.label17);
			this._MetadataPanel.Controls.Add (this._SubjectBox);
			this._MetadataPanel.Controls.Add (this.label18);
			this._MetadataPanel.Controls.Add (this._AuthorBox);
			this._MetadataPanel.Controls.Add (this.label19);
			this._MetadataPanel.Controls.Add (this._TitleBox);
			this._MetadataPanel.Enabled = false;
			this._MetadataPanel.Location = new System.Drawing.Point (6, 41);
			this._MetadataPanel.Name = "_MetadataPanel";
			this._MetadataPanel.Size = new System.Drawing.Size (414, 185);
			this._MetadataPanel.TabIndex = 1;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point (3, 0);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size (41, 12);
			this.label16.TabIndex = 0;
			this.label16.Text = "标题：";
			// 
			// _KeywordsBox
			// 
			this._KeywordsBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._KeywordsBox.Location = new System.Drawing.Point (5, 133);
			this._KeywordsBox.Name = "_KeywordsBox";
			this._KeywordsBox.Size = new System.Drawing.Size (406, 21);
			this._KeywordsBox.TabIndex = 7;
			this._KeywordsBox.TextChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point (3, 40);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size (41, 12);
			this.label17.TabIndex = 2;
			this.label17.Text = "作者：";
			// 
			// _SubjectBox
			// 
			this._SubjectBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._SubjectBox.Location = new System.Drawing.Point (5, 94);
			this._SubjectBox.Name = "_SubjectBox";
			this._SubjectBox.Size = new System.Drawing.Size (406, 21);
			this._SubjectBox.TabIndex = 5;
			this._SubjectBox.TextChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point (3, 79);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size (41, 12);
			this.label18.TabIndex = 4;
			this.label18.Text = "主题：";
			// 
			// _AuthorBox
			// 
			this._AuthorBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._AuthorBox.Location = new System.Drawing.Point (5, 55);
			this._AuthorBox.Name = "_AuthorBox";
			this._AuthorBox.Size = new System.Drawing.Size (406, 21);
			this._AuthorBox.TabIndex = 3;
			this._AuthorBox.TextChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point (3, 118);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size (53, 12);
			this.label19.TabIndex = 6;
			this.label19.Text = "关键词：";
			// 
			// _TitleBox
			// 
			this._TitleBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._TitleBox.Location = new System.Drawing.Point (5, 15);
			this._TitleBox.Name = "_TitleBox";
			this._TitleBox.Size = new System.Drawing.Size (406, 21);
			this._TitleBox.TabIndex = 1;
			this._TitleBox.TextChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _ForceMetadataBox
			// 
			this._ForceMetadataBox.AutoSize = true;
			this._ForceMetadataBox.Location = new System.Drawing.Point (8, 20);
			this._ForceMetadataBox.Name = "_ForceMetadataBox";
			this._ForceMetadataBox.Size = new System.Drawing.Size (180, 16);
			this._ForceMetadataBox.TabIndex = 0;
			this._ForceMetadataBox.Text = "使用此处设定的文档属性信息";
			this._ForceMetadataBox.UseVisualStyleBackColor = true;
			this._ForceMetadataBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point (6, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size (221, 12);
			this.label5.TabIndex = 0;
			this.label5.Text = "说明：以下设定优先于信息文件的内容。";
			// 
			// _RewriteXmpBox
			// 
			this._RewriteXmpBox.AutoSize = true;
			this._RewriteXmpBox.Location = new System.Drawing.Point (5, 160);
			this._RewriteXmpBox.Name = "_RewriteXmpBox";
			this._RewriteXmpBox.Size = new System.Drawing.Size (198, 16);
			this._RewriteXmpBox.TabIndex = 8;
			this._RewriteXmpBox.Text = "重写扩展标记（XML）元数据属性";
			this._RewriteXmpBox.UseVisualStyleBackColor = true;
			this._RewriteXmpBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _PropertyMacroMenu
			// 
			this._PropertyMacroMenu.Name = "_PropertyMacroMenu";
			this._PropertyMacroMenu.Size = new System.Drawing.Size (61, 4);
			// 
			// DocumentInfoEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add (this.groupBox4);
			this.Controls.Add (this.label5);
			this.Name = "DocumentInfoEditor";
			this.Size = new System.Drawing.Size (438, 270);
			this.groupBox4.ResumeLayout (false);
			this.groupBox4.PerformLayout ();
			this._MetadataPanel.ResumeLayout (false);
			this._MetadataPanel.PerformLayout ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Panel _MetadataPanel;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox _KeywordsBox;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox _SubjectBox;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox _AuthorBox;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox _TitleBox;
		private System.Windows.Forms.CheckBox _ForceMetadataBox;
		private System.Windows.Forms.Label label5;
		private Functions.MacroMenu _PropertyMacroMenu;
		private System.Windows.Forms.CheckBox _RewriteXmpBox;
	}
}
