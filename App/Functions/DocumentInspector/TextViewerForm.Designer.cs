namespace PDFPatcher.Functions
{
	partial class TextViewerForm
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
			this._TextBox = new System.Windows.Forms.RichTextBox();
			this._OkButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._EncodingBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// _TextBox
			// 
			this._TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TextBox.Location = new System.Drawing.Point(12, 12);
			this._TextBox.Name = "_TextBox";
			this._TextBox.Size = new System.Drawing.Size(472, 219);
			this._TextBox.TabIndex = 0;
			this._TextBox.Text = "";
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(328, 240);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 1;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(409, 240);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(75, 23);
			this._CancelButton.TabIndex = 2;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 245);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "解码方式：";
			// 
			// _EncodingBox
			// 
			this._EncodingBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._EncodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._EncodingBox.FormattingEnabled = true;
			this._EncodingBox.Items.AddRange(new object[] {
            "国标GBK（cp936）",
            "英文（cp1252）",
            "UTF-8",
            "十六进制表示形式"});
			this._EncodingBox.Location = new System.Drawing.Point(81, 240);
			this._EncodingBox.Name = "_EncodingBox";
			this._EncodingBox.Size = new System.Drawing.Size(121, 20);
			this._EncodingBox.TabIndex = 4;
			this._EncodingBox.SelectedIndexChanged += new System.EventHandler(this._EncodingBox_SelectedIndexChanged);
			// 
			// TextViewerForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(496, 275);
			this.Controls.Add(this._EncodingBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.Controls.Add(this._TextBox);
			this.Name = "TextViewerForm";
			this.Text = "文本内容";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox _TextBox;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _EncodingBox;
	}
}