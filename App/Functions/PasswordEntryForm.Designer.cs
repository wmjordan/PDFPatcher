namespace PDFPatcher
{
	partial class PasswordEntryForm
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
			this.label1 = new System.Windows.Forms.Label ();
			this._PasswordBox = new System.Windows.Forms.TextBox ();
			this._OkButton = new System.Windows.Forms.Button ();
			this._CancelButton = new System.Windows.Forms.Button ();
			this._MessageLabel = new System.Windows.Forms.Label ();
			this.SuspendLayout ();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (12, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (191, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "请输入 PDF 文件的编辑权限密码：";
			// 
			// _PasswordBox
			// 
			this._PasswordBox.Location = new System.Drawing.Point (14, 61);
			this._PasswordBox.Name = "_PasswordBox";
			this._PasswordBox.PasswordChar = '★';
			this._PasswordBox.Size = new System.Drawing.Size (274, 21);
			this._PasswordBox.TabIndex = 1;
			// 
			// _OkButton
			// 
			this._OkButton.Location = new System.Drawing.Point (132, 88);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size (75, 23);
			this._OkButton.TabIndex = 2;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler (this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point (213, 88);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size (75, 23);
			this._CancelButton.TabIndex = 3;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler (this._CancelButton_Click);
			// 
			// _MessageLabel
			// 
			this._MessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._MessageLabel.Location = new System.Drawing.Point (12, 9);
			this._MessageLabel.Name = "_MessageLabel";
			this._MessageLabel.Size = new System.Drawing.Size (275, 37);
			this._MessageLabel.TabIndex = 4;
			this._MessageLabel.Text = "PDF 文件已被加密，需要编辑权限密码才能打开。";
			// 
			// PasswordEntryForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size (300, 123);
			this.Controls.Add (this._MessageLabel);
			this.Controls.Add (this._CancelButton);
			this.Controls.Add (this._OkButton);
			this.Controls.Add (this._PasswordBox);
			this.Controls.Add (this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PasswordEntryForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "输入密码";
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _PasswordBox;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label _MessageLabel;
	}
}