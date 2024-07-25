using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	public partial class PatcherOptionsControl : UserControl
	{
		const float cm2point = (72f / 2.54f);
		string paperName;
		bool _uiLockDown;

		public PatcherOptionsControl() {
			InitializeComponent();
		}

		public bool ForEditor { get; set; }
		public PatcherOptions Options { get; set; }

		public PatcherOptions Apply() {
			var settings = Options;
			var ps = settings.UnifiedPageSettings;
			ps.AutoRotation = _AutoRotateBox.Checked;
			ps.Margins.IsRelative = _MarginUnitBox.SelectedIndex == 1;
			var marginScale = ps.Margins.IsRelative ? 1 : Constants.Units.CmToPoint;
			ps.Margins.Top = (float)_TopMarginBox.Value * marginScale;
			ps.Margins.Bottom = (float)_BottomMarginBox.Value * marginScale;
			ps.Margins.Left = (float)_LeftMarginBox.Value * marginScale;
			ps.Margins.Right = (float)_RightMarginBox.Value * marginScale;
			ps.PaperSize.PaperName = paperName;
			ps.PaperSize.Width = CmToPoint(_WidthBox);
			ps.PaperSize.Height = CmToPoint(_HeightBox);
			ps.HorizontalAlign = (Model.HorizontalAlignment)_ImageHAlignBox.SelectedIndex;
			ps.VerticalAlign = (Model.VerticalAlignment)_ImageVAlignBox.SelectedIndex;
			ps.ScaleContent = _ScalePdfPagesBox.Checked;
			settings.RecompressWithJbig2 = _RecompressWithJbig2Box.Checked;
			settings.RemoveAnnotations = _RemoveAnnotationsBox.Checked;
			settings.RemoveAttachments = _RemoveAttachmentsBox.Checked;
			settings.RemoveBookmarks = _RemoveBookmarksBox.Checked;
			settings.RemoveLeadingCommandCount = (int)_RemoveLeadingCommandCountBox.Value;
			settings.RemoveUsageRights = _RemoveUsageRightsBox.Checked;
			settings.RemoveXmlMetadata = _RemoveXmlMetaDataBox.Checked;
			settings.RemoveDocAutoActions = _RemoveDocAutoActionsBox.Checked;
			settings.RemovePageAutoActions = _RemovePageAutoActionsBox.Checked;
			settings.RemovePageForms = _RemovePageFormsBox.Checked;
			settings.RemovePageLinks = _RemovePageLinksBox.Checked;
			settings.RemovePageMetaData = _RemovePageMetaDataBox.Checked;
			settings.RemovePageTextBlocks = _RemovePageTextBlocksBox.Checked;
			settings.RemovePageThumbnails = _RemovePageThumbnailsBox.Checked;
			settings.RemoveTrailingCommandCount = (int)_RemoveTrailingCommandCountBox.Value;
			settings.FixContents = _FixContentBox.Checked;
			settings.FullCompression = _FullCompressionBox.Checked;
			return Options;
		}

		public void Reset() {
			if (ForEditor) {
				Options = AppContext.Editor = new PatcherOptions();
			}
			else {
				Options = AppContext.Patcher = new PatcherOptions();
			}
			Reload();
		}

		public void Reload() {
			_uiLockDown = true;
			var settings = Options;
			_DocumentInfoEditor.Options = settings.MetaData;
			_FontSubstitutionsEditor.Options = settings;
			_FontSubstitutionsEditor.Substitutions = settings.FontSubstitutions;
			_ViewerSettingsEditor.Options = settings.ViewerPreferences;
			_PageLabelEditor.Labels = settings.PageLabels;
			_PageSettingsEditor.Settings = settings.PageSettings;

			_FixContentBox.Checked = settings.FixContents;
			_RecompressWithJbig2Box.Checked = settings.RecompressWithJbig2;
			_RemoveAnnotationsBox.Checked = settings.RemoveAnnotations;
			_RemoveAttachmentsBox.Checked = settings.RemoveAttachments;
			_RemoveBookmarksBox.Checked = settings.RemoveBookmarks;
			_RemoveDocAutoActionsBox.Checked = settings.RemoveDocAutoActions;
			_RemoveLeadingCommandCountBox.SetValue(settings.RemoveLeadingCommandCount);
			_RemovePageAutoActionsBox.Checked = settings.RemovePageAutoActions;
			_RemovePageFormsBox.Checked = settings.RemovePageForms;
			_RemovePageLinksBox.Checked = settings.RemovePageLinks;
			_RemovePageMetaDataBox.Checked = settings.RemovePageMetaData;
			_RemovePageTextBlocksBox.Checked = settings.RemovePageTextBlocks;
			_RemovePageThumbnailsBox.Checked = settings.RemovePageThumbnails;
			_RemoveTrailingCommandCountBox.SetValue(settings.RemoveTrailingCommandCount);
			_RemoveUsageRightsBox.Checked = settings.RemoveUsageRights;
			_RemoveXmlMetaDataBox.Checked = settings.RemoveXmlMetadata;
			_FullCompressionBox.Checked = settings.FullCompression;

			var ps = settings.UnifiedPageSettings;
			_AutoRotateBox.Checked = ps.AutoRotation;
			_MarginUnitBox.SelectedIndex = ps.Margins.IsRelative ? 1 : 0;
			var marginScale = ps.Margins.IsRelative ? 1 : Constants.Units.CmToPoint;
			_BottomMarginBox.SetValue(ps.Margins.Bottom / marginScale);
			_LeftMarginBox.SetValue(ps.Margins.Left / marginScale);
			_RightMarginBox.SetValue(ps.Margins.Right / marginScale);
			_TopMarginBox.SetValue(ps.Margins.Top / marginScale);
			_HeightBox.SetValue(ps.PaperSize.Height / Constants.Units.CmToPoint);
			_WidthBox.SetValue(ps.PaperSize.Width / Constants.Units.CmToPoint);
			_ImageHAlignBox.SelectedIndex = (int)ps.HorizontalAlign;
			_ImageVAlignBox.SelectedIndex = (int)ps.VerticalAlign;
			for (int i = 0; i < _PageSizeBox.Items.Count; i++) {
				var p = _PageSizeBox.Items[i] as Model.PaperSize;
				if (p.PaperName == ps.PaperSize.PaperName) {
					_PageSizeBox.SelectedIndex = i;
				}
			}
			if (_PageSizeBox.SelectedIndex == -1) {
				_PageSizeBox.SelectedIndex = 0;
			}
			_ResizePdfPagesBox.Checked = ps.ScaleContent == false;
			_ScalePdfPagesBox.Checked = ps.ScaleContent;
			if (_PageSizeBox.SelectedIndex == 0) {
				_HeightBox.Value = 26.01M;
				_WidthBox.Value = 18M;
			}
			_uiLockDown = false;
		}

		internal void OnLoad() {
			_PageSizeBox.Items.AddRange(Processor.PdfDocumentCreator.PaperSizes);
			_ImageHAlignBox.Items.Add("水平居中");
			_ImageHAlignBox.Items.Add("左对齐");
			_ImageHAlignBox.Items.Add("右对齐");
			_ImageVAlignBox.Items.Add("垂直居中");
			_ImageVAlignBox.Items.Add("置顶");
			_ImageVAlignBox.Items.Add("置底");
			_ResetButton.Click += (s, args) => {
				if (this.ConfirmYesBox("是否将选项配置还原为默认值？")) {
					Reset();
				}
			};
			Reload();
			if (ForEditor) {
				_MainTab.TabPages.Remove(_DocumentInfoPage);
				Options.MetaData.SpecifyMetaData = false;
			}
		}

		static float CmToPoint(NumericUpDown box) {
			return (float)box.Value * Constants.Units.CmToPoint;
		}

		void _PageSizeBox_SelectedIndexChanged(object sender, EventArgs e) {
			if (_PageSizeBox.SelectedIndex == -1) {
				return;
			}
			var p = _PageSizeBox.SelectedItem as PaperSize;
			if (p.Width > 0 && p.Height > 0) {
				_WidthBox.SetValue((decimal)p.Width / 100);
				_HeightBox.SetValue((decimal)p.Height / 100);
			}
			paperName = p.PaperName;
			switch (paperName) {
				case PaperSize.FixedWidthAutoHeight:
					_AutoRotateBox.Enabled =
					_HeightBox.Enabled =
					_ImageVAlignBox.Enabled =
					false;
					_ScalePdfPagesBox.Enabled =
					_ResizePdfPagesBox.Enabled =
					_ImageHAlignBox.Enabled =
					_WidthBox.Enabled = true;
					break;
				case PaperSize.AsNarrowestPage:
				case PaperSize.AsWidestPage:
					_AutoRotateBox.Enabled =
					_HeightBox.Enabled =
					_ImageVAlignBox.Enabled =
					_WidthBox.Enabled =
					false;
					_ScalePdfPagesBox.Enabled =
					_ResizePdfPagesBox.Enabled =
					_ImageHAlignBox.Enabled = true;
					break;
				case PaperSize.AsPageSize:
					_AutoRotateBox.Enabled =
					_WidthBox.Enabled =
					_ImageHAlignBox.Enabled =
					_ImageVAlignBox.Enabled =
					_ScalePdfPagesBox.Enabled =
					_ResizePdfPagesBox.Enabled =
					_HeightBox.Enabled = false;
					break;
				case PaperSize.AsLargestPage:
				case PaperSize.AsSmallestPage:
					_AutoRotateBox.Enabled =
					_HeightBox.Enabled =
					_WidthBox.Enabled =
					false;
					_ImageVAlignBox.Enabled =
					_ImageHAlignBox.Enabled =
					_ScalePdfPagesBox.Enabled =
					_ResizePdfPagesBox.Enabled = true;
					break;
				default:
					_AutoRotateBox.Enabled =
					_WidthBox.Enabled =
					_HeightBox.Enabled =
					_ImageHAlignBox.Enabled =
					_ImageVAlignBox.Enabled =
					_ScalePdfPagesBox.Enabled =
					_ResizePdfPagesBox.Enabled =
					true;
					break;
			}
		}

		void MarginBox_ValueChanged(object sender, EventArgs e) {
			if (_SyncMarginsBox.Checked == false || _uiLockDown) {
				return;
			}
			var c = sender as NumericUpDown;
			var d = c.Value;
			_TopMarginBox.Value = _BottomMarginBox.Value = _LeftMarginBox.Value = _RightMarginBox.Value = d;
		}

	}
}
