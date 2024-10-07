using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	internal sealed partial class SourcePdfOptionForm : Form
	{
		readonly SourceItem.Pdf _pdf;

		SourcePdfOptionForm() {
			InitializeComponent();
		}
		internal SourcePdfOptionForm(SourceItem.Pdf pdf) : this() {
			_SourceFileBox.Text = pdf.FilePath.ToString();
			_PageRangeBox.Text = pdf.PageRanges;
			_ImportImagesOnlyBox.Checked = pdf.ImportImagesOnly;

			_MergeImagesBox.Checked = pdf.ExtractImageOptions.MergeImages;
			_InvertBlackAndWhiteImageBox.Checked = pdf.ExtractImageOptions.InvertBlackAndWhiteImages;
			_VerticalFlipImagesBox.Checked = pdf.ExtractImageOptions.VerticalFlipImages;
			_MinHeightBox.Value = pdf.ExtractImageOptions.MinHeight;
			_MinWidthBox.Value = pdf.ExtractImageOptions.MinWidth;
			_pdf = pdf;
		}

		void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			_pdf.PageRanges = PageRangeCollection.Parse(_PageRangeBox.Text, 1, _pdf.PageCount, true).ToString();
			_pdf.ImportImagesOnly = _ImportImagesOnlyBox.Checked;
			_pdf.ExtractImageOptions.MergeImages = _MergeImagesBox.Checked;
			_pdf.ExtractImageOptions.InvertBlackAndWhiteImages = _InvertBlackAndWhiteImageBox.Checked;
			_pdf.ExtractImageOptions.VerticalFlipImages = _VerticalFlipImagesBox.Checked;
			_pdf.ExtractImageOptions.MinHeight = (int)_MinHeightBox.Value;
			_pdf.ExtractImageOptions.MinWidth = (int)_MinWidthBox.Value;
			Close();
		}

		void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		void _ImportImagesOnlyBox_CheckedChanged(object sender, EventArgs e) {
			_ExtractImageOptionBox.Enabled = _ImportImagesOnlyBox.Checked;
		}
	}
}
