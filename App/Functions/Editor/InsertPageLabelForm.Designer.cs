namespace PDFPatcher.Functions
{
	partial class InsertPageLabelForm
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
			this._PrefixBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._NumericStyleBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this._StartAtBox = new System.Windows.Forms.NumericUpDown();
			this._OkButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this._PageNumberBox = new System.Windows.Forms.Label();
			this._RemoveLabelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._StartAtBox)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(65, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "前缀文本：";
			// 
			// _PrefixBox
			// 
			this._PrefixBox.Location = new System.Drawing.Point(83, 38);
			this._PrefixBox.Name = "_PrefixBox";
			this._PrefixBox.Size = new System.Drawing.Size(63, 21);
			this._PrefixBox.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(152, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "编号格式：";
			// 
			// _NumericStyleBox
			// 
			this._NumericStyleBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._NumericStyleBox.FormattingEnabled = true;
			this._NumericStyleBox.Location = new System.Drawing.Point(223, 11);
			this._NumericStyleBox.Name = "_NumericStyleBox";
			this._NumericStyleBox.Size = new System.Drawing.Size(121, 20);
			this._NumericStyleBox.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(152, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 12);
			this.label3.TabIndex = 6;
			this.label3.Text = "起始页号：";
			// 
			// _StartAtBox
			// 
			this._StartAtBox.Location = new System.Drawing.Point(223, 38);
			this._StartAtBox.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this._StartAtBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._StartAtBox.Name = "_StartAtBox";
			this._StartAtBox.Size = new System.Drawing.Size(63, 21);
			this._StartAtBox.TabIndex = 7;
			this._StartAtBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _OkButton
			// 
			this._OkButton.Location = new System.Drawing.Point(202, 79);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 8;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			// 
			// _CancelButton
			// 
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(286, 79);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(75, 23);
			this._CancelButton.TabIndex = 9;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 12);
			this.label4.TabIndex = 0;
			this.label4.Text = "目标页面";
			// 
			// _PageNumberBox
			// 
			this._PageNumberBox.Location = new System.Drawing.Point(81, 14);
			this._PageNumberBox.Name = "_PageNumberBox";
			this._PageNumberBox.Size = new System.Drawing.Size(65, 19);
			this._PageNumberBox.TabIndex = 1;
			// 
			// _RemoveLabelButton
			// 
			this._RemoveLabelButton.Location = new System.Drawing.Point(14, 79);
			this._RemoveLabelButton.Name = "_RemoveLabelButton";
			this._RemoveLabelButton.Size = new System.Drawing.Size(117, 23);
			this._RemoveLabelButton.TabIndex = 10;
			this._RemoveLabelButton.Text = "删除本页码标签(&S)";
			this._RemoveLabelButton.UseVisualStyleBackColor = true;
			// 
			// InsertPageLabelForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(373, 114);
			this.Controls.Add(this._RemoveLabelButton);
			this.Controls.Add(this._PageNumberBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.Controls.Add(this._StartAtBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._NumericStyleBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._PrefixBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "InsertPageLabelForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "插入页码标签";
			this.Load += new System.EventHandler(this.InsertPageLabelForm_Load);
			((System.ComponentModel.ISupportInitialize)(this._StartAtBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _PrefixBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _NumericStyleBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown _StartAtBox;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label _PageNumberBox;
		private System.Windows.Forms.Button _RemoveLabelButton;
	}
}