using System;
using System.Windows.Forms;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	internal sealed partial class SourceImageOptionForm : Form
	{
		readonly SourceItem.Image _image;


		internal SourceImageOptionForm(SourceItem.Image image) {
			InitializeComponent();
			_SourceFileBox.Text = image.FilePath.ToString();

			_TopMarginBox.Value = image.Cropping.Top;
			_LeftMarginBox.Value = image.Cropping.Left;
			_BottomMarginBox.Value = image.Cropping.Bottom;
			_RightMarginBox.Value = image.Cropping.Right;
			_MinCropHeightBox.Value = image.Cropping.MinHeight;
			_MinCropWidthBox.Value = image.Cropping.MinWidth;

			_image = image;
		}

		private void _OkButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.OK;
			_image.Cropping.Top = (int)_TopMarginBox.Value;
			_image.Cropping.Left = (int)_LeftMarginBox.Value;
			_image.Cropping.Right = (int)_RightMarginBox.Value;
			_image.Cropping.Bottom = (int)_BottomMarginBox.Value;
			_image.Cropping.MinHeight = (int)_MinCropHeightBox.Value;
			_image.Cropping.MinWidth = (int)_MinCropWidthBox.Value;

			Close();
		}

		private void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

	}
}
