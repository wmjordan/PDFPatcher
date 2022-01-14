namespace PDFPatcher.Functions
{
	partial class FontCharSubstitutionForm
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
			this.label1 = new System.Windows.Forms.Label();
			this._OriginalCharactersBox = new System.Windows.Forms.RichTextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._SubstituteCharactersBox = new System.Windows.Forms.RichTextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._ChineseCaseBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 34);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "原字符：";
			// 
			// _OriginalCharactersBox
			// 
			this._OriginalCharactersBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._OriginalCharactersBox.HideSelection = false;
			this._OriginalCharactersBox.Location = new System.Drawing.Point(112, 30);
			this._OriginalCharactersBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this._OriginalCharactersBox.Multiline = false;
			this._OriginalCharactersBox.Name = "_OriginalCharactersBox";
			this._OriginalCharactersBox.Size = new System.Drawing.Size(354, 29);
			this._OriginalCharactersBox.TabIndex = 1;
			this._OriginalCharactersBox.Text = "";
			this._OriginalCharactersBox.SelectionChanged += new System.EventHandler(this._OriginalCharactersBox_SelectionChanged);
			this._OriginalCharactersBox.TextChanged += new System.EventHandler(this._OriginalCharactersBox_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "替换字符：";
			// 
			// _SubstituteCharactersBox
			// 
			this._SubstituteCharactersBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._SubstituteCharactersBox.HideSelection = false;
			this._SubstituteCharactersBox.Location = new System.Drawing.Point(112, 88);
			this._SubstituteCharactersBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this._SubstituteCharactersBox.Multiline = false;
			this._SubstituteCharactersBox.Name = "_SubstituteCharactersBox";
			this._SubstituteCharactersBox.Size = new System.Drawing.Size(354, 29);
			this._SubstituteCharactersBox.TabIndex = 1;
			this._SubstituteCharactersBox.Text = "";
			this._SubstituteCharactersBox.TextChanged += new System.EventHandler(this._SubstituteCharactersBox_TextChanged);
			this._SubstituteCharactersBox.Enter += new System.EventHandler(this._SubstituteCharactersBox_Enter);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(134, 18);
			this.label3.TabIndex = 3;
			this.label3.Text = "简繁汉字替换：";
			// 
			// _ChineseCaseBox
			// 
			this._ChineseCaseBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._ChineseCaseBox.FormattingEnabled = true;
			this._ChineseCaseBox.Items.AddRange(new object[] {
            "不改变",
            "简体转繁体",
            "繁体转简体"});
			this._ChineseCaseBox.Location = new System.Drawing.Point(154, 149);
			this._ChineseCaseBox.Name = "_ChineseCaseBox";
			this._ChineseCaseBox.Size = new System.Drawing.Size(161, 26);
			this._ChineseCaseBox.TabIndex = 4;
			this._ChineseCaseBox.SelectedIndexChanged += new System.EventHandler(this._ChineseCaseBox_SelectedIndexChanged);
			// 
			// FontCharSubstitutionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(480, 198);
			this.Controls.Add(this._ChineseCaseBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._SubstituteCharactersBox);
			this.Controls.Add(this._OriginalCharactersBox);
			this.Controls.Add(this.label1);
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FontCharSubstitutionForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "替换字符";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox _OriginalCharactersBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RichTextBox _SubstituteCharactersBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox _ChineseCaseBox;
	}
}