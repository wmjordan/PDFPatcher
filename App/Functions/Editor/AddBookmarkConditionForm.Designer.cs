namespace PDFPatcher.Functions.Editor
{
	partial class AddBookmarkConditionForm
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
			this._MatchFontBox = new System.Windows.Forms.CheckBox ();
			this._MatchTextSizeBox = new System.Windows.Forms.CheckBox ();
			this._MinSizeBox = new System.Windows.Forms.NumericUpDown ();
			this._MaxSizeBox = new System.Windows.Forms.NumericUpDown ();
			this.label1 = new System.Windows.Forms.Label ();
			this._OkButton = new System.Windows.Forms.Button ();
			this._MatchTitlePatternBox = new System.Windows.Forms.CheckBox ();
			this._FontListBox = new System.Windows.Forms.ListBox ();
			this.label2 = new System.Windows.Forms.Label ();
			this._TitleLevelBox = new System.Windows.Forms.NumericUpDown ();
			((System.ComponentModel.ISupportInitialize)(this._MinSizeBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MaxSizeBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._TitleLevelBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// _MatchFontBox
			// 
			this._MatchFontBox.AutoSize = true;
			this._MatchFontBox.Location = new System.Drawing.Point (12, 12);
			this._MatchFontBox.Name = "_MatchFontBox";
			this._MatchFontBox.Size = new System.Drawing.Size (72, 16);
			this._MatchFontBox.TabIndex = 0;
			this._MatchFontBox.Text = "匹配字体";
			this._MatchFontBox.UseVisualStyleBackColor = true;
			// 
			// _MatchTextSizeBox
			// 
			this._MatchTextSizeBox.AutoSize = true;
			this._MatchTextSizeBox.Location = new System.Drawing.Point (174, 12);
			this._MatchTextSizeBox.Name = "_MatchTextSizeBox";
			this._MatchTextSizeBox.Size = new System.Drawing.Size (108, 16);
			this._MatchTextSizeBox.TabIndex = 2;
			this._MatchTextSizeBox.Text = "匹配文本尺寸：";
			this._MatchTextSizeBox.UseVisualStyleBackColor = true;
			// 
			// _MinSizeBox
			// 
			this._MinSizeBox.DecimalPlaces = 2;
			this._MinSizeBox.Location = new System.Drawing.Point (192, 34);
			this._MinSizeBox.Name = "_MinSizeBox";
			this._MinSizeBox.Size = new System.Drawing.Size (64, 21);
			this._MinSizeBox.TabIndex = 3;
			this._MinSizeBox.Value = new decimal (new int[] {
            2,
            0,
            0,
            0});
			// 
			// _MaxSizeBox
			// 
			this._MaxSizeBox.DecimalPlaces = 2;
			this._MaxSizeBox.Location = new System.Drawing.Point (285, 34);
			this._MaxSizeBox.Minimum = new decimal (new int[] {
            1,
            0,
            0,
            0});
			this._MaxSizeBox.Name = "_MaxSizeBox";
			this._MaxSizeBox.Size = new System.Drawing.Size (64, 21);
			this._MaxSizeBox.TabIndex = 5;
			this._MaxSizeBox.Value = new decimal (new int[] {
            1,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (262, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (17, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "到";
			// 
			// _OkButton
			// 
			this._OkButton.Location = new System.Drawing.Point (274, 120);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size (75, 23);
			this._OkButton.TabIndex = 9;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			// 
			// _MatchTitlePatternBox
			// 
			this._MatchTitlePatternBox.AutoSize = true;
			this._MatchTitlePatternBox.Location = new System.Drawing.Point (174, 61);
			this._MatchTitlePatternBox.Name = "_MatchTitlePatternBox";
			this._MatchTitlePatternBox.Size = new System.Drawing.Size (120, 16);
			this._MatchTitlePatternBox.TabIndex = 6;
			this._MatchTitlePatternBox.Text = "匹配标题编号模式";
			this._MatchTitlePatternBox.UseVisualStyleBackColor = true;
			// 
			// _FontListBox
			// 
			this._FontListBox.FormattingEnabled = true;
			this._FontListBox.ItemHeight = 12;
			this._FontListBox.Location = new System.Drawing.Point (29, 34);
			this._FontListBox.Name = "_FontListBox";
			this._FontListBox.Size = new System.Drawing.Size (139, 76);
			this._FontListBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (12, 125);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (65, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "标题级别：";
			// 
			// _TitleLevelBox
			// 
			this._TitleLevelBox.Location = new System.Drawing.Point (83, 123);
			this._TitleLevelBox.Minimum = new decimal (new int[] {
            1,
            0,
            0,
            0});
			this._TitleLevelBox.Name = "_TitleLevelBox";
			this._TitleLevelBox.Size = new System.Drawing.Size (41, 21);
			this._TitleLevelBox.TabIndex = 8;
			this._TitleLevelBox.Value = new decimal (new int[] {
            1,
            0,
            0,
            0});
			// 
			// AddBookmarkConditionForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (368, 155);
			this.Controls.Add (this._TitleLevelBox);
			this.Controls.Add (this.label2);
			this.Controls.Add (this._FontListBox);
			this.Controls.Add (this._MatchTitlePatternBox);
			this.Controls.Add (this._OkButton);
			this.Controls.Add (this.label1);
			this.Controls.Add (this._MaxSizeBox);
			this.Controls.Add (this._MinSizeBox);
			this.Controls.Add (this._MatchTextSizeBox);
			this.Controls.Add (this._MatchFontBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "AddBookmarkConditionForm";
			this.Text = "添加自动标记书签设置";
			((System.ComponentModel.ISupportInitialize)(this._MinSizeBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MaxSizeBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._TitleLevelBox)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.CheckBox _MatchFontBox;
		private System.Windows.Forms.CheckBox _MatchTextSizeBox;
		private System.Windows.Forms.NumericUpDown _MinSizeBox;
		private System.Windows.Forms.NumericUpDown _MaxSizeBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.CheckBox _MatchTitlePatternBox;
		private System.Windows.Forms.ListBox _FontListBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown _TitleLevelBox;
	}
}