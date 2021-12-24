namespace PDFPatcher.Functions
{
	partial class AddPdfObjectForm
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
			this._OkButton = new System.Windows.Forms.Button();
			this._CancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._ObjectNameBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this._TextValueBox = new System.Windows.Forms.TextBox();
			this._NumericValueBox = new System.Windows.Forms.TextBox();
			this._BooleanValueBox = new System.Windows.Forms.CheckBox();
			this._NameValueBox = new System.Windows.Forms.TextBox();
			this._CreateAsRefBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(111, 141);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 0;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler(this._OkButton_Click);
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(192, 141);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(75, 23);
			this._CancelButton.TabIndex = 1;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler(this._CancelButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "名称：";
			// 
			// _ObjectNameBox
			// 
			this._ObjectNameBox.Location = new System.Drawing.Point(59, 12);
			this._ObjectNameBox.Name = "_ObjectNameBox";
			this._ObjectNameBox.Size = new System.Drawing.Size(208, 21);
			this._ObjectNameBox.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "取值：";
			// 
			// _TextValueBox
			// 
			this._TextValueBox.AcceptsReturn = true;
			this._TextValueBox.AcceptsTab = true;
			this._TextValueBox.Location = new System.Drawing.Point(59, 39);
			this._TextValueBox.Multiline = true;
			this._TextValueBox.Name = "_TextValueBox";
			this._TextValueBox.Size = new System.Drawing.Size(208, 56);
			this._TextValueBox.TabIndex = 7;
			this._TextValueBox.Visible = false;
			// 
			// _NumericValueBox
			// 
			this._NumericValueBox.Location = new System.Drawing.Point(7, 123);
			this._NumericValueBox.Name = "_NumericValueBox";
			this._NumericValueBox.Size = new System.Drawing.Size(100, 21);
			this._NumericValueBox.TabIndex = 8;
			this._NumericValueBox.Visible = false;
			// 
			// _BooleanValueBox
			// 
			this._BooleanValueBox.AutoSize = true;
			this._BooleanValueBox.Location = new System.Drawing.Point(7, 79);
			this._BooleanValueBox.Name = "_BooleanValueBox";
			this._BooleanValueBox.Size = new System.Drawing.Size(48, 16);
			this._BooleanValueBox.TabIndex = 9;
			this._BooleanValueBox.Text = "True";
			this._BooleanValueBox.UseVisualStyleBackColor = true;
			this._BooleanValueBox.Visible = false;
			// 
			// _NameValueBox
			// 
			this._NameValueBox.Location = new System.Drawing.Point(7, 138);
			this._NameValueBox.Name = "_NameValueBox";
			this._NameValueBox.Size = new System.Drawing.Size(100, 21);
			this._NameValueBox.TabIndex = 10;
			this._NameValueBox.Visible = false;
			// 
			// _CreateAsRefBox
			// 
			this._CreateAsRefBox.AutoSize = true;
			this._CreateAsRefBox.Location = new System.Drawing.Point(59, 101);
			this._CreateAsRefBox.Name = "_CreateAsRefBox";
			this._CreateAsRefBox.Size = new System.Drawing.Size(132, 16);
			this._CreateAsRefBox.TabIndex = 11;
			this._CreateAsRefBox.Text = "创建为间接引用节点";
			this._CreateAsRefBox.UseVisualStyleBackColor = true;
			// 
			// AddPdfObjectForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(279, 176);
			this.Controls.Add(this._CreateAsRefBox);
			this.Controls.Add(this._NameValueBox);
			this.Controls.Add(this._BooleanValueBox);
			this.Controls.Add(this._NumericValueBox);
			this.Controls.Add(this._TextValueBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._ObjectNameBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddPdfObjectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "添加PDF对象";
			this.Load += new System.EventHandler(this.AddPdfObjectForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _ObjectNameBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _TextValueBox;
		private System.Windows.Forms.TextBox _NumericValueBox;
		private System.Windows.Forms.CheckBox _BooleanValueBox;
		private System.Windows.Forms.TextBox _NameValueBox;
		private System.Windows.Forms.CheckBox _CreateAsRefBox;
	}
}

