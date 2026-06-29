using System;
using System.Windows.Forms;
using MuPDF;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class AddPdfObjectForm : Form
	{
		readonly Control[] _editBoxes;
		public string ObjectName => _ObjectNameBox.Text;
		Kind _PdfObjectType;
		///<summary>获取或指定Description的值。</summary>
		public Kind PdfObjectType {
			get => _PdfObjectType;
			set {
				_PdfObjectType = value;
				FormHelper.ToggleVisibility(false, _editBoxes);
				switch (value) {
					case Kind.Array: break;
					case Kind.Boolean: _BooleanValueBox.Visible = true; break;
					case Kind.Dictionary: break;
					case Kind.Name: _NameValueBox.Visible = true; break;
					case Kind.Integer: _NumericValueBox.Visible = true; break;
					case Kind.String: _TextValueBox.Visible = true; break;
				}
			}
		}
		public bool CreateAsIndirect => _CreateAsRefBox.Checked;

		public AddPdfObjectForm() {
			InitializeComponent();
			_editBoxes = [_NameValueBox, _NumericValueBox, _BooleanValueBox, _TextValueBox];
		}

		public PdfObject CreatePdfObject(Document document) {
			return _PdfObjectType switch {
				Kind.Name => new PdfName(String.IsNullOrEmpty(_NameValueBox.Text) ? "name" : _NameValueBox.Text),
				Kind.Dictionary => document.NewDictionary(0),
				Kind.Array => document.NewArray(0),
				Kind.Boolean => _BooleanValueBox.Checked ? PdfBoolean.True : PdfBoolean.False,
				Kind.String => new PdfString(_TextValueBox.Text),
				Kind.Integer => new PdfFloat(_NumericValueBox.Text.ToSingle()),
				Kind.Float => new PdfFloat(_NumericValueBox.Text.ToSingle()),
				_ => PdfNull.Instance,
			};
		}

		void AddPdfObjectForm_Load(object sender, EventArgs e) {
			_NameValueBox.Location = _NumericValueBox.Location = _BooleanValueBox.Location = _TextValueBox.Location;
		}

		void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

	}
}
