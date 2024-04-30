namespace PDFPatcher.Functions
{
	partial class AppOptionForm
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
			this._DocInfoEncodingBox = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this._BookmarkEncodingBox = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._LoadPartialFileBox = new System.Windows.Forms.RadioButton();
			this._LoadEntireFileBox = new System.Windows.Forms.RadioButton();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._FontNameEncodingBox = new System.Windows.Forms.ComboBox();
			this._TextEncodingBox = new System.Windows.Forms.ComboBox();
			this._SaveAppSettingsBox = new System.Windows.Forms.CheckBox();
			this._CreateShortcutButton = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this._DefaultOverwritePdfFileBox = new System.Windows.Forms.CheckBox();
			this._AddContextMenuBox = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// _DocInfoEncodingBox
			// 
			this._DocInfoEncodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._DocInfoEncodingBox.FormattingEnabled = true;
			this._DocInfoEncodingBox.Location = new System.Drawing.Point(77, 37);
			this._DocInfoEncodingBox.Name = "_DocInfoEncodingBox";
			this._DocInfoEncodingBox.Size = new System.Drawing.Size(134, 20);
			this._DocInfoEncodingBox.TabIndex = 3;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 40);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(65, 12);
			this.label11.TabIndex = 2;
			this.label11.Text = "文档属性：";
			// 
			// _BookmarkEncodingBox
			// 
			this._BookmarkEncodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._BookmarkEncodingBox.FormattingEnabled = true;
			this._BookmarkEncodingBox.Location = new System.Drawing.Point(292, 37);
			this._BookmarkEncodingBox.Name = "_BookmarkEncodingBox";
			this._BookmarkEncodingBox.Size = new System.Drawing.Size(134, 20);
			this._BookmarkEncodingBox.TabIndex = 5;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(221, 40);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(65, 12);
			this.label10.TabIndex = 4;
			this.label10.Text = "书签文本：";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._LoadPartialFileBox);
			this.groupBox1.Controls.Add(this._LoadEntireFileBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 61);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(449, 46);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "访问 PDF 文档";
			// 
			// _LoadPartialFileBox
			// 
			this._LoadPartialFileBox.AutoSize = true;
			this._LoadPartialFileBox.Location = new System.Drawing.Point(223, 20);
			this._LoadPartialFileBox.Name = "_LoadPartialFileBox";
			this._LoadPartialFileBox.Size = new System.Drawing.Size(215, 16);
			this._LoadPartialFileBox.TabIndex = 1;
			this._LoadPartialFileBox.TabStop = true;
			this._LoadPartialFileBox.Text = "减少占用内存（仅加载需处理部分）";
			this._LoadPartialFileBox.UseVisualStyleBackColor = true;
			// 
			// _LoadEntireFileBox
			// 
			this._LoadEntireFileBox.AutoSize = true;
			this._LoadEntireFileBox.Location = new System.Drawing.Point(8, 20);
			this._LoadEntireFileBox.Name = "_LoadEntireFileBox";
			this._LoadEntireFileBox.Size = new System.Drawing.Size(191, 16);
			this._LoadEntireFileBox.TabIndex = 0;
			this._LoadEntireFileBox.TabStop = true;
			this._LoadEntireFileBox.Text = "优化处理效率（加载整个文件）";
			this._LoadEntireFileBox.UseVisualStyleBackColor = true;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label12.Location = new System.Drawing.Point(6, 17);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(437, 17);
			this.label12.TabIndex = 6;
			this.label12.Text = "说明：当遇到 PDF 文档的文本为乱码时，可尝试使用此选项强制设定编码。";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this._FontNameEncodingBox);
			this.groupBox2.Controls.Add(this._TextEncodingBox);
			this.groupBox2.Controls.Add(this._DocInfoEncodingBox);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this._BookmarkEncodingBox);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Location = new System.Drawing.Point(12, 113);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(449, 91);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "读取文档所用的编码";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(220, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 7;
			this.label1.Text = "字体名称：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 66);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "正文文本：";
			// 
			// _FontNameEncodingBox
			// 
			this._FontNameEncodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._FontNameEncodingBox.FormattingEnabled = true;
			this._FontNameEncodingBox.Location = new System.Drawing.Point(292, 63);
			this._FontNameEncodingBox.Name = "_FontNameEncodingBox";
			this._FontNameEncodingBox.Size = new System.Drawing.Size(134, 20);
			this._FontNameEncodingBox.TabIndex = 3;
			// 
			// _TextEncodingBox
			// 
			this._TextEncodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._TextEncodingBox.FormattingEnabled = true;
			this._TextEncodingBox.Location = new System.Drawing.Point(77, 63);
			this._TextEncodingBox.Name = "_TextEncodingBox";
			this._TextEncodingBox.Size = new System.Drawing.Size(134, 20);
			this._TextEncodingBox.TabIndex = 3;
			// 
			// _SaveAppSettingsBox
			// 
			this._SaveAppSettingsBox.AutoSize = true;
			this._SaveAppSettingsBox.Location = new System.Drawing.Point(12, 12);
			this._SaveAppSettingsBox.Name = "_SaveAppSettingsBox";
			this._SaveAppSettingsBox.Size = new System.Drawing.Size(144, 16);
			this._SaveAppSettingsBox.TabIndex = 11;
			this._SaveAppSettingsBox.Text = "自动保存应用程序设置";
			this._SaveAppSettingsBox.UseVisualStyleBackColor = true;
			// 
			// _CreateShortcutButton
			// 
			this._CreateShortcutButton.Location = new System.Drawing.Point(299, 8);
			this._CreateShortcutButton.Name = "_CreateShortcutButton";
			this._CreateShortcutButton.Size = new System.Drawing.Size(162, 23);
			this._CreateShortcutButton.TabIndex = 12;
			this._CreateShortcutButton.Text = "在桌面创建程序快捷方式";
			this._CreateShortcutButton.UseVisualStyleBackColor = true;
			this._CreateShortcutButton.Click += new System.EventHandler(this._CreateShortcutButton_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this._DefaultOverwritePdfFileBox);
			this.groupBox3.Location = new System.Drawing.Point(12, 210);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(449, 47);
			this.groupBox3.TabIndex = 13;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "编辑器";
			// 
			// _DefaultOverwritePdfFileBox
			// 
			this._DefaultOverwritePdfFileBox.AutoSize = true;
			this._DefaultOverwritePdfFileBox.Location = new System.Drawing.Point(8, 20);
			this._DefaultOverwritePdfFileBox.Name = "_DefaultOverwritePdfFileBox";
			this._DefaultOverwritePdfFileBox.Size = new System.Drawing.Size(186, 16);
			this._DefaultOverwritePdfFileBox.TabIndex = 0;
			this._DefaultOverwritePdfFileBox.Text = "默认覆盖正在编辑的 PDF 文件";
			this._DefaultOverwritePdfFileBox.UseVisualStyleBackColor = true;
			// 
			// _AddContextMenuBox
			// 
			this._AddContextMenuBox.AutoSize = true;
			this._AddContextMenuBox.Location = new System.Drawing.Point(20, 36);
			this._AddContextMenuBox.Name = "_AddContextMenuBox";
			this._AddContextMenuBox.Size = new System.Drawing.Size(264, 16);
			this._AddContextMenuBox.TabIndex = 14;
			this._AddContextMenuBox.Text = "PDF 文件右键菜单的“打开方式”添加“PDF补丁丁”";
			this._AddContextMenuBox.UseVisualStyleBackColor = true;
			// 
			// AppOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 267);
			this.Controls.Add(this._AddContextMenuBox);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this._CreateShortcutButton);
			this.Controls.Add(this._SaveAppSettingsBox);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AppOptionForm";
			this.Text = "程序工作选项";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox _DocInfoEncodingBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.ComboBox _BookmarkEncodingBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.RadioButton _LoadEntireFileBox;
		private System.Windows.Forms.RadioButton _LoadPartialFileBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _TextEncodingBox;
		private System.Windows.Forms.CheckBox _SaveAppSettingsBox;
		private System.Windows.Forms.Button _CreateShortcutButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _FontNameEncodingBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox _DefaultOverwritePdfFileBox;
		private System.Windows.Forms.CheckBox _AddContextMenuBox;
	}
}
