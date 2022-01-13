using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	public partial class AddPdfObjectForm : Form
	{
		readonly Control[] _editBoxes;
		public string ObjectName => _ObjectNameBox.Text;
		private int _PdfObjectType;

		///<summary>获取或指定Description的值。</summary>
		public int PdfObjectType {
			get => _PdfObjectType;
			set {
				_PdfObjectType = value;
				FormHelper.ToggleVisibility(false, _editBoxes);
				switch (value) {
					case PdfObject.ARRAY: break;
					case PdfObject.BOOLEAN:
						_BooleanValueBox.Visible = true;
						break;
					case PdfObject.DICTIONARY: break;
					case PdfObject.NAME:
						_NameValueBox.Visible = true;
						break;
					case PdfObject.NUMBER:
						_NumericValueBox.Visible = true;
						break;
					case PdfObject.STRING:
						_TextValueBox.Visible = true;
						break;
				}
			}
		}

		public bool CreateAsIndirect => _CreateAsRefBox.Checked;

		public PdfObject PdfValue {
			get {
				PdfObject o;
				switch (_PdfObjectType) {
					case PdfObject.NAME:
						o = new PdfName(String.IsNullOrEmpty(_NameValueBox.Text) ? "name" : _NameValueBox.Text);
						break;
					case PdfObject.DICTIONARY:
						o = new PdfDictionary();
						break;
					case PdfObject.ARRAY:
						o = new PdfArray();
						break;
					case PdfObject.BOOLEAN:
						o = new PdfBoolean(_BooleanValueBox.Checked);
						break;
					case PdfObject.STRING:
						o = _TextValueBox.Text.ToPdfString();
						break;
					case PdfObject.NUMBER:
						o = new PdfNumber(_NumericValueBox.Text.ToDouble());
						break;
					default: return null;
				}

				return o;
			}
		}

		public AddPdfObjectForm() {
			InitializeComponent();
			_editBoxes = new Control[] {_NameValueBox, _NumericValueBox, _BooleanValueBox, _TextValueBox};
		}

		private void AddPdfObjectForm_Load(object sender, EventArgs e) {
			_NameValueBox.Location = _NumericValueBox.Location = _BooleanValueBox.Location = _TextValueBox.Location;
		}

		protected void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			Close();
		}

		protected void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}