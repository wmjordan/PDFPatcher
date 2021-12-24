namespace PDFPatcher.Functions
{
	partial class ViewerPreferenceEditor
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
			this.groupBox2 = new System.Windows.Forms.GroupBox ();
			this._ForceInternalLinkBox = new System.Windows.Forms.CheckBox ();
			this._ForceRemoveZoomRateBox = new System.Windows.Forms.CheckBox ();
			this._ForceBookmarkOpenBox = new System.Windows.Forms.ComboBox ();
			this.label2 = new System.Windows.Forms.Label ();
			this.groupBox1 = new System.Windows.Forms.GroupBox ();
			this.label8 = new System.Windows.Forms.Label ();
			this._ForceInitialModeBox = new System.Windows.Forms.ComboBox ();
			this._ForceDirectionBox = new System.Windows.Forms.ComboBox ();
			this.label7 = new System.Windows.Forms.Label ();
			this._ForceInitialViewBox = new System.Windows.Forms.ComboBox ();
			this.label3 = new System.Windows.Forms.Label ();
			this.label5 = new System.Windows.Forms.Label ();
			this._HideMenuBox = new System.Windows.Forms.CheckBox ();
			this._HideToolbarBox = new System.Windows.Forms.CheckBox ();
			this._HideUIBox = new System.Windows.Forms.CheckBox ();
			this._FitWindowBox = new System.Windows.Forms.CheckBox ();
			this._CenterWindowBox = new System.Windows.Forms.CheckBox ();
			this._DisplayDocTitleBox = new System.Windows.Forms.CheckBox ();
			this._UISettingsPanel = new System.Windows.Forms.Panel ();
			this._OverrideUISettingsBox = new System.Windows.Forms.CheckBox ();
			this.groupBox4 = new System.Windows.Forms.GroupBox ();
			this.groupBox2.SuspendLayout ();
			this.groupBox1.SuspendLayout ();
			this._UISettingsPanel.SuspendLayout ();
			this.groupBox4.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add (this._ForceInternalLinkBox);
			this.groupBox2.Controls.Add (this._ForceRemoveZoomRateBox);
			this.groupBox2.Location = new System.Drawing.Point (6, 164);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size (426, 70);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "书签及链接动作";
			// 
			// _ForceInternalLinkBox
			// 
			this._ForceInternalLinkBox.AutoSize = true;
			this._ForceInternalLinkBox.Location = new System.Drawing.Point (6, 42);
			this._ForceInternalLinkBox.Name = "_ForceInternalLinkBox";
			this._ForceInternalLinkBox.Size = new System.Drawing.Size (234, 16);
			this._ForceInternalLinkBox.TabIndex = 3;
			this._ForceInternalLinkBox.Text = "强制将外部 PDF 文件链接改成内部链接";
			this._ForceInternalLinkBox.UseVisualStyleBackColor = true;
			this._ForceInternalLinkBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _ForceRemoveZoomRateBox
			// 
			this._ForceRemoveZoomRateBox.AutoSize = true;
			this._ForceRemoveZoomRateBox.Location = new System.Drawing.Point (6, 20);
			this._ForceRemoveZoomRateBox.Name = "_ForceRemoveZoomRateBox";
			this._ForceRemoveZoomRateBox.Size = new System.Drawing.Size (204, 16);
			this._ForceRemoveZoomRateBox.TabIndex = 0;
			this._ForceRemoveZoomRateBox.Text = "禁止书签及页面链接更改显示比例";
			this._ForceRemoveZoomRateBox.UseVisualStyleBackColor = true;
			this._ForceRemoveZoomRateBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _ForceBookmarkOpenBox
			// 
			this._ForceBookmarkOpenBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ForceBookmarkOpenBox.FormattingEnabled = true;
			this._ForceBookmarkOpenBox.Location = new System.Drawing.Point (77, 96);
			this._ForceBookmarkOpenBox.Name = "_ForceBookmarkOpenBox";
			this._ForceBookmarkOpenBox.Size = new System.Drawing.Size (127, 20);
			this._ForceBookmarkOpenBox.TabIndex = 2;
			this._ForceBookmarkOpenBox.SelectedIndexChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (6, 99);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (65, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "书签状态：";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add (this.label8);
			this.groupBox1.Controls.Add (this._ForceBookmarkOpenBox);
			this.groupBox1.Controls.Add (this._ForceInitialModeBox);
			this.groupBox1.Controls.Add (this._ForceDirectionBox);
			this.groupBox1.Controls.Add (this.label2);
			this.groupBox1.Controls.Add (this.label7);
			this.groupBox1.Controls.Add (this._ForceInitialViewBox);
			this.groupBox1.Controls.Add (this.label3);
			this.groupBox1.Location = new System.Drawing.Point (6, 23);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size (210, 135);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "阅读器设定";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point (6, 73);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size (65, 12);
			this.label8.TabIndex = 4;
			this.label8.Text = "初始模式：";
			// 
			// _ForceInitialModeBox
			// 
			this._ForceInitialModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ForceInitialModeBox.FormattingEnabled = true;
			this._ForceInitialModeBox.Location = new System.Drawing.Point (77, 70);
			this._ForceInitialModeBox.Name = "_ForceInitialModeBox";
			this._ForceInitialModeBox.Size = new System.Drawing.Size (127, 20);
			this._ForceInitialModeBox.TabIndex = 5;
			this._ForceInitialModeBox.SelectedIndexChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _ForceDirectionBox
			// 
			this._ForceDirectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ForceDirectionBox.FormattingEnabled = true;
			this._ForceDirectionBox.Location = new System.Drawing.Point (77, 44);
			this._ForceDirectionBox.Name = "_ForceDirectionBox";
			this._ForceDirectionBox.Size = new System.Drawing.Size (127, 20);
			this._ForceDirectionBox.TabIndex = 3;
			this._ForceDirectionBox.SelectedIndexChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point (6, 47);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size (65, 12);
			this.label7.TabIndex = 2;
			this.label7.Text = "阅读方向：";
			// 
			// _ForceInitialViewBox
			// 
			this._ForceInitialViewBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ForceInitialViewBox.Location = new System.Drawing.Point (77, 18);
			this._ForceInitialViewBox.Name = "_ForceInitialViewBox";
			this._ForceInitialViewBox.Size = new System.Drawing.Size (127, 20);
			this._ForceInitialViewBox.TabIndex = 1;
			this._ForceInitialViewBox.SelectedIndexChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point (6, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size (65, 12);
			this.label3.TabIndex = 0;
			this.label3.Text = "初始视图：";
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
			// _HideMenuBox
			// 
			this._HideMenuBox.AutoSize = true;
			this._HideMenuBox.Location = new System.Drawing.Point (3, 3);
			this._HideMenuBox.Name = "_HideMenuBox";
			this._HideMenuBox.Size = new System.Drawing.Size (72, 16);
			this._HideMenuBox.TabIndex = 0;
			this._HideMenuBox.Text = "隐藏菜单";
			this._HideMenuBox.UseVisualStyleBackColor = true;
			this._HideMenuBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _HideToolbarBox
			// 
			this._HideToolbarBox.AutoSize = true;
			this._HideToolbarBox.Location = new System.Drawing.Point (99, 3);
			this._HideToolbarBox.Name = "_HideToolbarBox";
			this._HideToolbarBox.Size = new System.Drawing.Size (84, 16);
			this._HideToolbarBox.TabIndex = 1;
			this._HideToolbarBox.Text = "隐藏工具栏";
			this._HideToolbarBox.UseVisualStyleBackColor = true;
			this._HideToolbarBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _HideUIBox
			// 
			this._HideUIBox.AutoSize = true;
			this._HideUIBox.Location = new System.Drawing.Point (3, 25);
			this._HideUIBox.Name = "_HideUIBox";
			this._HideUIBox.Size = new System.Drawing.Size (192, 16);
			this._HideUIBox.TabIndex = 2;
			this._HideUIBox.Text = "隐藏操作界面，仅显示文档内容";
			this._HideUIBox.UseVisualStyleBackColor = true;
			this._HideUIBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _FitWindowBox
			// 
			this._FitWindowBox.AutoSize = true;
			this._FitWindowBox.Location = new System.Drawing.Point (3, 47);
			this._FitWindowBox.Name = "_FitWindowBox";
			this._FitWindowBox.Size = new System.Drawing.Size (120, 16);
			this._FitWindowBox.TabIndex = 3;
			this._FitWindowBox.Text = "窗口适应文档首页";
			this._FitWindowBox.UseVisualStyleBackColor = true;
			this._FitWindowBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _CenterWindowBox
			// 
			this._CenterWindowBox.AutoSize = true;
			this._CenterWindowBox.Location = new System.Drawing.Point (3, 68);
			this._CenterWindowBox.Name = "_CenterWindowBox";
			this._CenterWindowBox.Size = new System.Drawing.Size (72, 16);
			this._CenterWindowBox.TabIndex = 4;
			this._CenterWindowBox.Text = "窗口居中";
			this._CenterWindowBox.UseVisualStyleBackColor = true;
			this._CenterWindowBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _DisplayDocTitleBox
			// 
			this._DisplayDocTitleBox.AutoSize = true;
			this._DisplayDocTitleBox.Location = new System.Drawing.Point (99, 68);
			this._DisplayDocTitleBox.Name = "_DisplayDocTitleBox";
			this._DisplayDocTitleBox.Size = new System.Drawing.Size (96, 16);
			this._DisplayDocTitleBox.TabIndex = 5;
			this._DisplayDocTitleBox.Text = "显示文档标题";
			this._DisplayDocTitleBox.UseVisualStyleBackColor = true;
			this._DisplayDocTitleBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// _UISettingsPanel
			// 
			this._UISettingsPanel.Controls.Add (this._DisplayDocTitleBox);
			this._UISettingsPanel.Controls.Add (this._HideMenuBox);
			this._UISettingsPanel.Controls.Add (this._CenterWindowBox);
			this._UISettingsPanel.Controls.Add (this._HideToolbarBox);
			this._UISettingsPanel.Controls.Add (this._FitWindowBox);
			this._UISettingsPanel.Controls.Add (this._HideUIBox);
			this._UISettingsPanel.Enabled = false;
			this._UISettingsPanel.Location = new System.Drawing.Point (2, 42);
			this._UISettingsPanel.Name = "_UISettingsPanel";
			this._UISettingsPanel.Size = new System.Drawing.Size (208, 87);
			this._UISettingsPanel.TabIndex = 1;
			// 
			// _OverrideUISettingsBox
			// 
			this._OverrideUISettingsBox.AutoSize = true;
			this._OverrideUISettingsBox.Location = new System.Drawing.Point (5, 20);
			this._OverrideUISettingsBox.Name = "_OverrideUISettingsBox";
			this._OverrideUISettingsBox.Size = new System.Drawing.Size (144, 16);
			this._OverrideUISettingsBox.TabIndex = 0;
			this._OverrideUISettingsBox.Text = "设置阅读器的操作界面";
			this._OverrideUISettingsBox.UseVisualStyleBackColor = true;
			this._OverrideUISettingsBox.CheckedChanged += new System.EventHandler (this.DocumentInfoChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add (this._OverrideUISettingsBox);
			this.groupBox4.Controls.Add (this._UISettingsPanel);
			this.groupBox4.Location = new System.Drawing.Point (222, 23);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size (210, 135);
			this.groupBox4.TabIndex = 3;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "阅读器操作界面设定";
			// 
			// ViewerPreferenceEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add (this.groupBox4);
			this.Controls.Add (this.groupBox2);
			this.Controls.Add (this.groupBox1);
			this.Controls.Add (this.label5);
			this.Name = "ViewerPreferenceEditor";
			this.Size = new System.Drawing.Size (438, 279);
			this.groupBox2.ResumeLayout (false);
			this.groupBox2.PerformLayout ();
			this.groupBox1.ResumeLayout (false);
			this.groupBox1.PerformLayout ();
			this._UISettingsPanel.ResumeLayout (false);
			this._UISettingsPanel.PerformLayout ();
			this.groupBox4.ResumeLayout (false);
			this.groupBox4.PerformLayout ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox _ForceBookmarkOpenBox;
		private System.Windows.Forms.CheckBox _ForceRemoveZoomRateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox _ForceInitialModeBox;
		private System.Windows.Forms.ComboBox _ForceDirectionBox;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox _ForceInitialViewBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox _HideUIBox;
		private System.Windows.Forms.CheckBox _HideToolbarBox;
		private System.Windows.Forms.CheckBox _HideMenuBox;
		private System.Windows.Forms.CheckBox _FitWindowBox;
		private System.Windows.Forms.CheckBox _DisplayDocTitleBox;
		private System.Windows.Forms.CheckBox _CenterWindowBox;
		private System.Windows.Forms.Panel _UISettingsPanel;
		private System.Windows.Forms.CheckBox _OverrideUISettingsBox;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox _ForceInternalLinkBox;
	}
}
