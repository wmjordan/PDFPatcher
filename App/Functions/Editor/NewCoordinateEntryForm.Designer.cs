namespace PDFPatcher.Functions
{
	partial class NewCoordinateEntryForm
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
			this._CancelButton = new System.Windows.Forms.Button();
			this._OkButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._CoordinateBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this._AdjustmentAmountBox = new System.Windows.Forms.NumericUpDown();
			this._RelativeBox = new System.Windows.Forms.RadioButton();
			this._AbsoluteBox = new System.Windows.Forms.RadioButton();
			this._ProportionBox = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this._AdjustmentAmountBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _CancelButton
			// 
			this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._CancelButton.Location = new System.Drawing.Point(142, 158);
			this._CancelButton.Name = "_CancelButton";
			this._CancelButton.Size = new System.Drawing.Size(75, 23);
			this._CancelButton.TabIndex = 5;
			this._CancelButton.Text = "取消(&X)";
			this._CancelButton.UseVisualStyleBackColor = true;
			this._CancelButton.Click += new System.EventHandler(this._CancelButton_Click);
			// 
			// _OkButton
			// 
			this._OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._OkButton.Location = new System.Drawing.Point(61, 158);
			this._OkButton.Name = "_OkButton";
			this._OkButton.Size = new System.Drawing.Size(75, 23);
			this._OkButton.TabIndex = 4;
			this._OkButton.Text = "确定(&Q)";
			this._OkButton.UseVisualStyleBackColor = true;
			this._OkButton.Click += new System.EventHandler(this._OkButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "需要调整的坐标：";
			// 
			// _CoordinateBox
			// 
			this._CoordinateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._CoordinateBox.FormattingEnabled = true;
			this._CoordinateBox.Items.AddRange(new object[] {
            "上",
            "下",
            "左",
            "右"});
			this._CoordinateBox.Location = new System.Drawing.Point(119, 17);
			this._CoordinateBox.Name = "_CoordinateBox";
			this._CoordinateBox.Size = new System.Drawing.Size(87, 20);
			this._CoordinateBox.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "坐标调整量：";
			// 
			// _AdjustmentAmountBox
			// 
			this._AdjustmentAmountBox.DecimalPlaces = 2;
			this._AdjustmentAmountBox.Location = new System.Drawing.Point(119, 43);
			this._AdjustmentAmountBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this._AdjustmentAmountBox.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
			this._AdjustmentAmountBox.Name = "_AdjustmentAmountBox";
			this._AdjustmentAmountBox.Size = new System.Drawing.Size(87, 21);
			this._AdjustmentAmountBox.TabIndex = 9;
			this._AdjustmentAmountBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// _RelativeBox
			// 
			this._RelativeBox.AutoSize = true;
			this._RelativeBox.Checked = true;
			this._RelativeBox.Location = new System.Drawing.Point(14, 71);
			this._RelativeBox.Name = "_RelativeBox";
			this._RelativeBox.Size = new System.Drawing.Size(179, 16);
			this._RelativeBox.TabIndex = 10;
			this._RelativeBox.TabStop = true;
			this._RelativeBox.Text = "相对调整（原坐标加调整量）";
			this._RelativeBox.UseVisualStyleBackColor = true;
			// 
			// _AbsoluteBox
			// 
			this._AbsoluteBox.AutoSize = true;
			this._AbsoluteBox.Location = new System.Drawing.Point(14, 93);
			this._AbsoluteBox.Name = "_AbsoluteBox";
			this._AbsoluteBox.Size = new System.Drawing.Size(191, 16);
			this._AbsoluteBox.TabIndex = 11;
			this._AbsoluteBox.Text = "绝对调整（原坐标设为调整值）";
			this._AbsoluteBox.UseVisualStyleBackColor = true;
			// 
			// _ProportionBox
			// 
			this._ProportionBox.AutoSize = true;
			this._ProportionBox.Location = new System.Drawing.Point(14, 114);
			this._ProportionBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._ProportionBox.Name = "_ProportionBox";
			this._ProportionBox.Size = new System.Drawing.Size(191, 16);
			this._ProportionBox.TabIndex = 12;
			this._ProportionBox.TabStop = true;
			this._ProportionBox.Text = "比例调整（原坐标乘以调整量）";
			this._ProportionBox.UseVisualStyleBackColor = true;
			// 
			// NewCoordinateEntryForm
			// 
			this.AcceptButton = this._OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._CancelButton;
			this.ClientSize = new System.Drawing.Size(229, 194);
			this.Controls.Add(this._ProportionBox);
			this.Controls.Add(this._AbsoluteBox);
			this.Controls.Add(this._RelativeBox);
			this.Controls.Add(this._AdjustmentAmountBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._CoordinateBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._CancelButton);
			this.Controls.Add(this._OkButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewCoordinateEntryForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "输入坐标调整值";
			((System.ComponentModel.ISupportInitialize)(this._AdjustmentAmountBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _CancelButton;
		private System.Windows.Forms.Button _OkButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _CoordinateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown _AdjustmentAmountBox;
		private System.Windows.Forms.RadioButton _RelativeBox;
		private System.Windows.Forms.RadioButton _AbsoluteBox;
		private System.Windows.Forms.RadioButton _ProportionBox;
	}
}