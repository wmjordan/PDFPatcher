namespace PDFPatcher.Functions.Editor
{
	partial class CustomPatternForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this._PatternBox = new System.Windows.Forms.TextBox();
			this._MatchCaseBox = new System.Windows.Forms.CheckBox();
			this._FullMatchBox = new System.Windows.Forms.CheckBox();
			this._OkButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "正则表达式：";
			// 
			// _PatternBox
			// 
			this._PatternBox.AutoSize = true;
			this._PatternBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this._PatternBox.Multiline = true;
			this._PatternBox.Location = new System.Drawing.Point(134, 12);
			this._PatternBox.Name = "_PatternBox";
			this._PatternBox.Size = new System.Drawing.Size(364, 28);
			this._PatternBox.TabIndex = 1;
			// 
			// _MatchCaseBox
			// 
			this._MatchCaseBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this._MatchCaseBox.AutoSize = true;
			this._MatchCaseBox.Location = new System.Drawing.Point(134, 46);
			this._MatchCaseBox.Name = "_MatchCaseBox";
			this._MatchCaseBox.Size = new System.Drawing.Size(160, 22);
			this._MatchCaseBox.TabIndex = 2;
			this._MatchCaseBox.Text = "区分英文大小写";
			this._MatchCaseBox.UseVisualStyleBackColor = true;
			// 
			// _FullMatchBox
			// 
			this._FullMatchBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this._FullMatchBox.AutoSize = true;
			this._FullMatchBox.Location = new System.Drawing.Point(374, 46);
			this._FullMatchBox.Name = "_FullMatchBox";
			this._FullMatchBox.Size = new System.Drawing.Size(124, 22);
			this._FullMatchBox.TabIndex = 3;
			this._FullMatchBox.Text = "匹配全标题";
			this._FullMatchBox.UseVisualStyleBackColor = true;
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this._OkButton.Location = new System.Drawing.Point(134, 74);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(124, 30);
			this._OkButton.TabIndex = 4;
			this._OkButton.Text = "确定";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler(this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			this._CancelButton.Location = new System.Drawing.Point(264, 74);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(124, 30);
			this._CancelButton.TabIndex = 5;
			this._CancelButton.Text = "取消";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler(this._CancelButton_Click);
			// 
			// CustomPatternForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(525, 129);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.Controls.Add(this._FullMatchBox);
			this.Controls.Add(this._MatchCaseBox);
			this.Controls.Add(this._PatternBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "CustomPatternForm";
			this.Text = "自定义书签文本匹配模式";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _PatternBox;
		private System.Windows.Forms.CheckBox _MatchCaseBox;
		private System.Windows.Forms.CheckBox _FullMatchBox;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
	}
}