using System;
using System.Windows.Forms;
using MuPdfSharp;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor;

public partial class DocumentInfoForm : Form
{
	public DocumentInfoForm() {
		InitializeComponent();
	}

	internal MuDocument Document { get; set; }
	internal PdfInfoXmlDocument InfoDucument { get; set; }

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		if (Document != null) {
			MuDocumentInfo info = Document.Info;
			_AuthorBox.Text = info.Author;
			_CreationDateBox.Text = PdfHelper.ParseDateTime(info.CreationDate).ToString();
			_CreatorBox.Text = info.Creator;
			_FilePathBox.Text = Document.FilePath;
			_KeywordsBox.Text = info.Keywords;
			_ModDateBox.Text = PdfHelper.ParseDateTime(info.ModificationDate).ToString();
			_PageCountBox.Text = Document.PageCount.ToString();
			_ProducerBox.Text = info.Producer;
			_SubjectBox.Text = info.Subject;
			_TitleBox.Text = info.Title;
		}

		if (InfoDucument != null) {
			DocumentInfoElement info = InfoDucument.InfoNode;
			SetText(_AuthorBox, info.Author);
			SetText(_CreatorBox, info.Creator);
			SetText(_KeywordsBox, info.Keywords);
			SetText(_ProducerBox, info.Producer);
			SetText(_SubjectBox, info.Subject);
			SetText(_TitleBox, info.Title);
		}
		else {
			_OkButton.Enabled = false;
		}

		_OkButton.Click += (s, args) => {
			DialogResult = DialogResult.OK;
			DocumentInfoElement info = InfoDucument.InfoNode;
			info.Author = _AuthorBox.Text;
			info.Creator = _CreatorBox.Text;
			info.Keywords = _KeywordsBox.Text;
			info.Producer = _ProducerBox.Text;
			info.Subject = _SubjectBox.Text;
			info.Title = _TitleBox.Text;
			Close();
		};
		_CancelButton.Click += (s, args) => {
			DialogResult = DialogResult.Cancel;
			Close();
		};
		_ConfigButton.Click += (s, args) => {
			AppContext.MainForm.SelectFunctionList(Function.EditorOptions);
		};
	}

	private static void SetText(Control control, string value) {
		if (value == null) {
			return;
		}

		control.Text = value;
	}
}