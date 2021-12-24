namespace PDFPatcher.Functions
{
	partial class ReportControl
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
			this._ProgressBar = new System.Windows.Forms.ProgressBar();
			this.label2 = new System.Windows.Forms.Label();
			this._CancelButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._InputFileBox = new System.Windows.Forms.TextBox();
			this._OutputFileBox = new System.Windows.Forms.TextBox();
			this._TotalProgressBar = new System.Windows.Forms.ProgressBar();
			this.label6 = new System.Windows.Forms.Label();
			this._LogBox = new RichTextBoxLinks.RichTextBoxEx();
			this.SuspendLayout();
			// 
			// _ProgressBar
			// 
			this._ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._ProgressBar.Location = new System.Drawing.Point(111, 302);
			this._ProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._ProgressBar.Name = "_ProgressBar";
			this._ProgressBar.Size = new System.Drawing.Size(447, 21);
			this._ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._ProgressBar.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 65);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "日志内容：";
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.Image = global::PDFPatcher.Properties.Resources.Return;
			this._CancelButton.Location = new System.Drawing.Point(565, 302);
			this._CancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(100, 42);
			this._CancelButton.TabIndex = 9;
			this._CancelButton.Text = "返回(&F)";
			this._CancelButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler(this._CancelButton_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 302);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 15);
			this.label3.TabIndex = 7;
			this.label3.Text = "执行进度：";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 15);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 15);
			this.label4.TabIndex = 1;
			this.label4.Text = "输入文件：";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(16, 42);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(82, 15);
			this.label5.TabIndex = 3;
			this.label5.Text = "输出文件：";
			// 
			// _InputFileBox
			// 
			this._InputFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._InputFileBox.BackColor = System.Drawing.SystemColors.Control;
			this._InputFileBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._InputFileBox.Location = new System.Drawing.Point(111, 13);
			this._InputFileBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._InputFileBox.Name = "_InputFileBox";
			this._InputFileBox.ReadOnly = true;
			this._InputFileBox.Size = new System.Drawing.Size(551, 18);
			this._InputFileBox.TabIndex = 2;
			// 
			// _OutputFileBox
			// 
			this._OutputFileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._OutputFileBox.BackColor = System.Drawing.SystemColors.Control;
			this._OutputFileBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._OutputFileBox.Location = new System.Drawing.Point(111, 40);
			this._OutputFileBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._OutputFileBox.Name = "_OutputFileBox";
			this._OutputFileBox.ReadOnly = true;
			this._OutputFileBox.Size = new System.Drawing.Size(551, 18);
			this._OutputFileBox.TabIndex = 4;
			// 
			// _TotalProgressBar
			// 
			this._TotalProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._TotalProgressBar.Location = new System.Drawing.Point(111, 324);
			this._TotalProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._TotalProgressBar.Name = "_TotalProgressBar";
			this._TotalProgressBar.Size = new System.Drawing.Size(447, 21);
			this._TotalProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._TotalProgressBar.TabIndex = 8;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 324);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 15);
			this.label6.TabIndex = 7;
			this.label6.Text = "总体进度：";
			// 
			// _LogBox
			// 
			this._LogBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._LogBox.BackColor = System.Drawing.SystemColors.Window;
			this._LogBox.Location = new System.Drawing.Point(17, 84);
			this._LogBox.Margin = new System.Windows.Forms.Padding(4);
			this._LogBox.Name = "_LogBox";
			this._LogBox.ReadOnly = true;
			this._LogBox.Size = new System.Drawing.Size(645, 210);
			this._LogBox.TabIndex = 6;
			this._LogBox.Text = "";
			this._LogBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this._LogBox_LinkClicked);
			// 
			// ReportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._OutputFileBox);
			this.Controls.Add(this._InputFileBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._TotalProgressBar);
			this.Controls.Add(this._ProgressBar);
			this.Controls.Add(this._LogBox);
			this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ReportControl";
			this.Size = new System.Drawing.Size(680, 359);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.ProgressBar _ProgressBar;
		private System.Windows.Forms.Label label2;
		internal RichTextBoxLinks.RichTextBoxEx _LogBox;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		internal System.Windows.Forms.TextBox _InputFileBox;
		internal System.Windows.Forms.TextBox _OutputFileBox;
		internal System.Windows.Forms.ProgressBar _TotalProgressBar;
		private System.Windows.Forms.Label label6;

	}
}
