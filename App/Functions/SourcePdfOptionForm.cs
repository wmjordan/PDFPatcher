using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	internal sealed partial class SourcePdfOptionForm : Form
	{
		SourceItem.Pdf _pdf;


		internal SourcePdfOptionForm (SourceItem.Pdf pdf) {
			InitializeComponent ();
			_SourceFileBox.Text = pdf.FilePath.ToString();
			_PageRangeBox.Text = pdf.PageRanges;
			_ImportImagesOnlyBox.Checked = pdf.ImportImagesOnly;

			//_TopMarginBox.Value = pdf.Cropping.Top;
			//_LeftMarginBox.Value = pdf.Cropping.Left;
			//_BottomMarginBox.Value = pdf.Cropping.Bottom;
			//_RightMarginBox.Value = pdf.Cropping.Right;
			//_MinCropHeightBox.Value = pdf.Cropping.MinHeight;
			//_MinCropWidthBox.Value = pdf.Cropping.MinWidth;

			_MergeImagesBox.Checked = pdf.ExtractImageOptions.MergeImages;
			_InvertBlackAndWhiteImageBox.Checked = pdf.ExtractImageOptions.InvertBlackAndWhiteImages;
			_VerticalFlipImagesBox.Checked = pdf.ExtractImageOptions.VerticalFlipImages;
			_MinHeightBox.Value = pdf.ExtractImageOptions.MinHeight;
			_MinWidthBox.Value = pdf.ExtractImageOptions.MinWidth;
			_pdf = pdf;
		}

		private void _OkButton_Click (Object source, EventArgs args) {
			this.DialogResult = DialogResult.OK;
			_pdf.PageRanges = Model.PageRangeCollection.Parse (_PageRangeBox.Text, 1, _pdf.PageCount, true).ToString ();
			_pdf.ImportImagesOnly = _ImportImagesOnlyBox.Checked;
			//_pdf.Cropping.Top = (int)_TopMarginBox.Value;
			//_pdf.Cropping.Left = (int)_LeftMarginBox.Value;
			//_pdf.Cropping.Right = (int)_RightMarginBox.Value;
			//_pdf.Cropping.Bottom = (int)_BottomMarginBox.Value;
			//_pdf.Cropping.MinHeight = (int)_MinCropHeightBox.Value;
			//_pdf.Cropping.MinWidth = (int)_MinCropWidthBox.Value;

			_pdf.ExtractImageOptions.MergeImages = _MergeImagesBox.Checked;
			_pdf.ExtractImageOptions.InvertBlackAndWhiteImages = _InvertBlackAndWhiteImageBox.Checked;
			_pdf.ExtractImageOptions.VerticalFlipImages = _VerticalFlipImagesBox.Checked;
			_pdf.ExtractImageOptions.MinHeight = (int)_MinHeightBox.Value;
			_pdf.ExtractImageOptions.MinWidth = (int)_MinWidthBox.Value;
			this.Close ();
		}

		private void _CancelButton_Click (Object source, EventArgs args) {
			this.DialogResult = DialogResult.Cancel;
			this.Close ();
		}

		private void _ImportImagesOnlyBox_CheckedChanged (object sender, EventArgs e) {
			_ExtractImageOptionBox.Enabled = _ImportImagesOnlyBox.Checked;
		}
	}
}
