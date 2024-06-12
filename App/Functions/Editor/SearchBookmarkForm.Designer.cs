namespace PDFPatcher.Functions
{
	partial class SearchBookmarkForm
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
			this._SearchButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._MatchCaseBox = new System.Windows.Forms.CheckBox();
			this._FullMatchBox = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this._ReplaceButton = new System.Windows.Forms.Button();
			this._ResultLabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._NormalSearchBox = new System.Windows.Forms.RadioButton();
			this._RegexSearchBox = new System.Windows.Forms.RadioButton();
			this._XPathSearchBox = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this._ReplaceInSelectionBox = new System.Windows.Forms.RadioButton();
			this._ReplaceInAllBox = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this._SearchNextButton = new System.Windows.Forms.Button();
			this._ReplaceTextBox = new PDFPatcher.HistoryComboBox();
			this._SearchTextBox = new PDFPatcher.HistoryComboBox();
			this._CloseButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _SearchButton
			// 
			this._SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._SearchButton.Location = new System.Drawing.Point(357, 10);
			this._SearchButton.Name = "_SearchButton";
			this._SearchButton.Size = new System.Drawing.Size(99, 23);
			this._SearchButton.TabIndex = 11;
			this._SearchButton.Text = "搜索全部(&S)";
			this._SearchButton.UseVisualStyleBackColor = true;
			this._SearchButton.Click += new System.EventHandler(this._SearchButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "搜索文本：";
			// 
			// _MatchCaseBox
			// 
			this._MatchCaseBox.AutoSize = true;
			this._MatchCaseBox.Location = new System.Drawing.Point(83, 38);
			this._MatchCaseBox.Name = "_MatchCaseBox";
			this._MatchCaseBox.Size = new System.Drawing.Size(84, 16);
			this._MatchCaseBox.TabIndex = 2;
			this._MatchCaseBox.Text = "区分大小写";
			this._MatchCaseBox.UseVisualStyleBackColor = true;
			// 
			// _FullMatchBox
			// 
			this._FullMatchBox.AutoSize = true;
			this._FullMatchBox.Location = new System.Drawing.Point(183, 38);
			this._FullMatchBox.Name = "_FullMatchBox";
			this._FullMatchBox.Size = new System.Drawing.Size(132, 16);
			this._FullMatchBox.TabIndex = 3;
			this._FullMatchBox.Text = "匹配整个书签的文本";
			this._FullMatchBox.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 85);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "替换文本：";
			// 
			// _ReplaceButton
			// 
			this._ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._ReplaceButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._ReplaceButton.Location = new System.Drawing.Point(357, 80);
			this._ReplaceButton.Name = "_ReplaceButton";
			this._ReplaceButton.Size = new System.Drawing.Size(99, 23);
			this._ReplaceButton.TabIndex = 13;
			this._ReplaceButton.Text = "替换(&T)";
			this._ReplaceButton.UseVisualStyleBackColor = true;
			this._ReplaceButton.Click += new System.EventHandler(this._ReplaceButton_Click);
			// 
			// _ResultLabel
			// 
			this._ResultLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ResultLabel.Location = new System.Drawing.Point(12, 127);
			this._ResultLabel.Name = "_ResultLabel";
			this._ResultLabel.Size = new System.Drawing.Size(443, 33);
			this._ResultLabel.TabIndex = 10;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "搜索模式：";
			// 
			// _NormalSearchBox
			// 
			this._NormalSearchBox.AutoSize = true;
			this._NormalSearchBox.Location = new System.Drawing.Point(0, 0);
			this._NormalSearchBox.Name = "_NormalSearchBox";
			this._NormalSearchBox.Size = new System.Drawing.Size(47, 16);
			this._NormalSearchBox.TabIndex = 0;
			this._NormalSearchBox.TabStop = true;
			this._NormalSearchBox.Text = "普通";
			this._NormalSearchBox.UseVisualStyleBackColor = true;
			this._NormalSearchBox.CheckedChanged += new System.EventHandler(this.MatchModeChanged);
			// 
			// _RegexSearchBox
			// 
			this._RegexSearchBox.AutoSize = true;
			this._RegexSearchBox.Location = new System.Drawing.Point(53, 0);
			this._RegexSearchBox.Name = "_RegexSearchBox";
			this._RegexSearchBox.Size = new System.Drawing.Size(83, 16);
			this._RegexSearchBox.TabIndex = 1;
			this._RegexSearchBox.TabStop = true;
			this._RegexSearchBox.Text = "正则表达式";
			this._RegexSearchBox.UseVisualStyleBackColor = true;
			this._RegexSearchBox.CheckedChanged += new System.EventHandler(this.MatchModeChanged);
			// 
			// _XPathSearchBox
			// 
			this._XPathSearchBox.AutoSize = true;
			this._XPathSearchBox.Location = new System.Drawing.Point(144, 0);
			this._XPathSearchBox.Name = "_XPathSearchBox";
			this._XPathSearchBox.Size = new System.Drawing.Size(53, 16);
			this._XPathSearchBox.TabIndex = 2;
			this._XPathSearchBox.TabStop = true;
			this._XPathSearchBox.Text = "XPath";
			this._XPathSearchBox.UseVisualStyleBackColor = true;
			this._XPathSearchBox.CheckedChanged += new System.EventHandler(this.MatchModeChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 110);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 12);
			this.label4.TabIndex = 8;
			this.label4.Text = "替换范围：";
			// 
			// _ReplaceInSelectionBox
			// 
			this._ReplaceInSelectionBox.AutoSize = true;
			this._ReplaceInSelectionBox.Location = new System.Drawing.Point(0, 0);
			this._ReplaceInSelectionBox.Name = "_ReplaceInSelectionBox";
			this._ReplaceInSelectionBox.Size = new System.Drawing.Size(83, 16);
			this._ReplaceInSelectionBox.TabIndex = 0;
			this._ReplaceInSelectionBox.TabStop = true;
			this._ReplaceInSelectionBox.Text = "选中的书签";
			this._ReplaceInSelectionBox.UseVisualStyleBackColor = true;
			this._ReplaceInSelectionBox.CheckedChanged += new System.EventHandler(this.ReplaceModeChanged);
			// 
			// _ReplaceInAllBox
			// 
			this._ReplaceInAllBox.AutoSize = true;
			this._ReplaceInAllBox.Location = new System.Drawing.Point(100, 0);
			this._ReplaceInAllBox.Name = "_ReplaceInAllBox";
			this._ReplaceInAllBox.Size = new System.Drawing.Size(71, 16);
			this._ReplaceInAllBox.TabIndex = 1;
			this._ReplaceInAllBox.TabStop = true;
			this._ReplaceInAllBox.Text = "所有书签";
			this._ReplaceInAllBox.UseVisualStyleBackColor = true;
			this._ReplaceInAllBox.CheckedChanged += new System.EventHandler(this.ReplaceModeChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._ReplaceInAllBox);
			this.panel1.Controls.Add(this._ReplaceInSelectionBox);
			this.panel1.Location = new System.Drawing.Point(83, 108);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(268, 16);
			this.panel1.TabIndex = 9;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this._NormalSearchBox);
			this.panel2.Controls.Add(this._XPathSearchBox);
			this.panel2.Controls.Add(this._RegexSearchBox);
			this.panel2.Location = new System.Drawing.Point(83, 60);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(268, 16);
			this.panel2.TabIndex = 5;
			// 
			// _SearchNextButton
			// 
			this._SearchNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._SearchNextButton.Location = new System.Drawing.Point(357, 39);
			this._SearchNextButton.Name = "_SearchNextButton";
			this._SearchNextButton.Size = new System.Drawing.Size(99, 23);
			this._SearchNextButton.TabIndex = 12;
			this._SearchNextButton.Text = "搜索下一个(&X)";
			this._SearchNextButton.UseVisualStyleBackColor = true;
			this._SearchNextButton.Click += new System.EventHandler(this._SearchButton_Click);
			// 
			// _ReplaceTextBox
			// 
			this._ReplaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ReplaceTextBox.Contents = null;
			this._ReplaceTextBox.FormattingEnabled = true;
			this._ReplaceTextBox.Location = new System.Drawing.Point(83, 82);
			this._ReplaceTextBox.MaxItemCount = 16;
			this._ReplaceTextBox.Name = "_ReplaceTextBox";
			this._ReplaceTextBox.Size = new System.Drawing.Size(268, 20);
			this._ReplaceTextBox.TabIndex = 7;
			// 
			// _SearchTextBox
			// 
			this._SearchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SearchTextBox.Contents = null;
			this._SearchTextBox.FormattingEnabled = true;
			this._SearchTextBox.Location = new System.Drawing.Point(83, 12);
			this._SearchTextBox.MaxItemCount = 16;
			this._SearchTextBox.Name = "_SearchTextBox";
			this._SearchTextBox.Size = new System.Drawing.Size(268, 20);
			this._SearchTextBox.TabIndex = 1;
			// 
			// _CloseButton
			// 
			this._CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CloseButton.Location = new System.Drawing.Point(357, 105);
			this._CloseButton.Name = "_CloseButton";
			this._CloseButton.Size = new System.Drawing.Size(99, 23);
			this._CloseButton.TabIndex = 14;
			this._CloseButton.Text = "关闭";
			this._CloseButton.UseVisualStyleBackColor = true;
			this._CloseButton.Click += new System.EventHandler(this._CloseButton_Click);
			// 
			// SearchBookmarkForm
			// 
			this.AcceptButton = this._SearchButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CloseButton;
			this.ClientSize = new System.Drawing.Size(467, 161);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._ReplaceTextBox);
			this.Controls.Add(this._SearchTextBox);
			this.Controls.Add(this._ResultLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._FullMatchBox);
			this.Controls.Add(this._MatchCaseBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._CloseButton);
			this.Controls.Add(this._ReplaceButton);
			this.Controls.Add(this._SearchNextButton);
			this.Controls.Add(this._SearchButton);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(999, 200);
			this.MinimizeBox = false;
			this.Name = "SearchBookmarkForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "查找、替换书签项";
			this.Load += new System.EventHandler(this.SearchBookmarkForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _SearchButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox _MatchCaseBox;
		private System.Windows.Forms.CheckBox _FullMatchBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button _ReplaceButton;
		private System.Windows.Forms.Label _ResultLabel;
		private HistoryComboBox _SearchTextBox;
		private HistoryComboBox _ReplaceTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.RadioButton _NormalSearchBox;
		private System.Windows.Forms.RadioButton _RegexSearchBox;
		private System.Windows.Forms.RadioButton _XPathSearchBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton _ReplaceInSelectionBox;
		private System.Windows.Forms.RadioButton _ReplaceInAllBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button _SearchNextButton;
		private System.Windows.Forms.Button _CloseButton;
	}
}

