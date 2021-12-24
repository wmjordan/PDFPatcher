namespace PDFPatcher.Functions
{
	partial class TextPositionConditionEditor
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
			this.panel2 = new System.Windows.Forms.Panel ();
			this.label2 = new System.Windows.Forms.Label ();
			this._PositionBox = new System.Windows.Forms.ComboBox ();
			this._RangeBox = new System.Windows.Forms.RadioButton ();
			this._SpecificValueBox = new System.Windows.Forms.NumericUpDown ();
			this._SpecificBox = new System.Windows.Forms.RadioButton ();
			this._MinBox = new System.Windows.Forms.NumericUpDown ();
			this.label1 = new System.Windows.Forms.Label ();
			this._MaxBox = new System.Windows.Forms.NumericUpDown ();
			this.panel2.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this._SpecificValueBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinBox)).BeginInit ();
			((System.ComponentModel.ISupportInitialize)(this._MaxBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// panel2
			// 
			this.panel2.Controls.Add (this.label2);
			this.panel2.Controls.Add (this._PositionBox);
			this.panel2.Controls.Add (this._RangeBox);
			this.panel2.Controls.Add (this._SpecificValueBox);
			this.panel2.Controls.Add (this._SpecificBox);
			this.panel2.Controls.Add (this._MinBox);
			this.panel2.Controls.Add (this.label1);
			this.panel2.Controls.Add (this._MaxBox);
			this.panel2.Location = new System.Drawing.Point (3, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size (377, 88);
			this.panel2.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point (21, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size (101, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "匹配文本块的坐标";
			// 
			// _PositionBox
			// 
			this._PositionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._PositionBox.FormattingEnabled = true;
			this._PositionBox.Items.AddRange (new object[] {
            "上坐标",
            "下坐标",
            "左坐标",
            "右坐标"});
			this._PositionBox.Location = new System.Drawing.Point (135, 3);
			this._PositionBox.Name = "_PositionBox";
			this._PositionBox.Size = new System.Drawing.Size (121, 20);
			this._PositionBox.TabIndex = 4;
			this._PositionBox.SelectedIndexChanged += new System.EventHandler (this.ControlChanged);
			// 
			// _RangeBox
			// 
			this._RangeBox.AutoSize = true;
			this._RangeBox.Location = new System.Drawing.Point (23, 56);
			this._RangeBox.Name = "_RangeBox";
			this._RangeBox.Size = new System.Drawing.Size (95, 16);
			this._RangeBox.TabIndex = 3;
			this._RangeBox.TabStop = true;
			this._RangeBox.Text = "匹配坐标范围";
			this._RangeBox.UseVisualStyleBackColor = true;
			this._RangeBox.CheckedChanged += new System.EventHandler (this.ControlChanged);
			// 
			// _SpecificValueBox
			// 
			this._SpecificValueBox.DecimalPlaces = 2;
			this._SpecificValueBox.Location = new System.Drawing.Point (135, 29);
			this._SpecificValueBox.Maximum = new decimal (new int[] {
            9999,
            0,
            0,
            0});
			this._SpecificValueBox.Minimum = new decimal (new int[] {
            9999,
            0,
            0,
            -2147483648});
			this._SpecificValueBox.Name = "_SpecificValueBox";
			this._SpecificValueBox.Size = new System.Drawing.Size (67, 21);
			this._SpecificValueBox.TabIndex = 1;
			this._SpecificValueBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._SpecificValueBox.ValueChanged += new System.EventHandler (this.ControlChanged);
			// 
			// _SpecificBox
			// 
			this._SpecificBox.AutoSize = true;
			this._SpecificBox.Location = new System.Drawing.Point (23, 29);
			this._SpecificBox.Name = "_SpecificBox";
			this._SpecificBox.Size = new System.Drawing.Size (107, 16);
			this._SpecificBox.TabIndex = 3;
			this._SpecificBox.TabStop = true;
			this._SpecificBox.Text = "匹配特定坐标值";
			this._SpecificBox.UseVisualStyleBackColor = true;
			this._SpecificBox.CheckedChanged += new System.EventHandler (this.ControlChanged);
			// 
			// _MinBox
			// 
			this._MinBox.DecimalPlaces = 2;
			this._MinBox.Location = new System.Drawing.Point (135, 56);
			this._MinBox.Maximum = new decimal (new int[] {
            9999,
            0,
            0,
            0});
			this._MinBox.Minimum = new decimal (new int[] {
            9999,
            0,
            0,
            -2147483648});
			this._MinBox.Name = "_MinBox";
			this._MinBox.Size = new System.Drawing.Size (67, 21);
			this._MinBox.TabIndex = 1;
			this._MinBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._MinBox.ValueChanged += new System.EventHandler (this.ControlChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (208, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (17, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "到";
			// 
			// _MaxBox
			// 
			this._MaxBox.DecimalPlaces = 2;
			this._MaxBox.Location = new System.Drawing.Point (231, 56);
			this._MaxBox.Maximum = new decimal (new int[] {
            9999,
            0,
            0,
            0});
			this._MaxBox.Minimum = new decimal (new int[] {
            9999,
            0,
            0,
            -2147483648});
			this._MaxBox.Name = "_MaxBox";
			this._MaxBox.Size = new System.Drawing.Size (67, 21);
			this._MaxBox.TabIndex = 1;
			this._MaxBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._MaxBox.ValueChanged += new System.EventHandler (this.ControlChanged);
			// 
			// TextPositionConditionEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add (this.panel2);
			this.Name = "TextPositionConditionEditor";
			this.Size = new System.Drawing.Size (383, 88);
			this.panel2.ResumeLayout (false);
			this.panel2.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this._SpecificValueBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MinBox)).EndInit ();
			((System.ComponentModel.ISupportInitialize)(this._MaxBox)).EndInit ();
			this.ResumeLayout (false);

		}

		#endregion

		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RadioButton _RangeBox;
		private System.Windows.Forms.NumericUpDown _SpecificValueBox;
		private System.Windows.Forms.RadioButton _SpecificBox;
		private System.Windows.Forms.NumericUpDown _MinBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown _MaxBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _PositionBox;

	}
}
