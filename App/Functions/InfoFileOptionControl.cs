using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public partial class InfoFileOptionControl : Form, IResettableControl
	{
		ExporterOptions _expOptions;
		ImporterOptions _impOptions;
		bool locked;

		public InfoFileOptionControl() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			this.SetIcon(Properties.Resources.InfoFileOptions);
			AppContext.MainForm.SetTooltip(_ExtractPageRangeBox, Messages.PageRanges);

			_UnitBox.Items.AddRange(Constants.Units.Names);

			Reload();

			_ConsolidateNamedDestBox.CheckedChanged += OptionChanged;
			_EncodingBox.Leave += _EncodingBox_Leave;
			_ExportBinaryStreamBox.ValueChanged += OptionChanged;
			_ExportBookmarksBox.CheckedChanged += OptionChanged;
			_ExportCatalogBox.CheckedChanged += OptionChanged;
			_ExportDocPropertiesBox.CheckedChanged += OptionChanged;
			_ExtractImagesBox.CheckedChanged += OptionChanged;
			_ExtractPageContentBox.CheckedChanged += OptionChanged;
			_ExtractPageDictionaryBox.CheckedChanged += OptionChanged;
			_ExtractPageLinksBox.CheckedChanged += OptionChanged;
			_ExtractPageRangeBox.Leave += OptionChanged;
			_ExtractPageSettingsBox.CheckedChanged += OptionChanged;
			_ExtractPageTextContentBox.CheckedChanged += OptionChanged;
			_ExportViewerPreferencesBox.CheckedChanged += OptionChanged;
			_ExportContentOperatorsBox.CheckedChanged += OptionChanged;
			_ImportBookmarksBox.CheckedChanged += OptionChanged;
			_ImportDocumentInfoBox.CheckedChanged += OptionChanged;
			_ImportPageLinksBox.CheckedChanged += OptionChanged;
			_ImportPageSettingsBox.CheckedChanged += OptionChanged;
			_ImportViewerPreferencesBox.CheckedChanged += OptionChanged;
			_KeepOriginalPageLinksBox.CheckedChanged += OptionChanged;
			_RemoveOriginalPageLinksBox.CheckedChanged += OptionChanged;
		}

		public void Reset() {
			locked = true;
			AppContext.Exporter = new ExporterOptions();
			AppContext.Importer = new ImporterOptions();
			Reload();
			locked = false;
		}

		public void Reload() {
			_expOptions = AppContext.Exporter;
			_impOptions = AppContext.Importer;

			_ConsolidateNamedDestBox.Checked = _expOptions.ConsolidateNamedDestinations;
			_EncodingBox.Text = _expOptions.Encoding;
			_ExportBinaryStreamBox.Value = _expOptions.ExportBinaryStream;
			_ExportBookmarksBox.Checked = _expOptions.ExportBookmarks;
			_ExportCatalogBox.Checked = _expOptions.ExportCatalog;
			_ExportContentOperatorsBox.Checked = _expOptions.ExportContentOperators;
			_ExportDocPropertiesBox.Checked = _expOptions.ExportDocProperties;
			_ExtractImagesBox.Checked = _expOptions.ExtractImages;
			_ExtractPageContentBox.Checked = _expOptions.ExtractPageContent;
			_ExtractPageDictionaryBox.Checked = _expOptions.ExtractPageDictionary;
			_ExtractPageLinksBox.Checked = _expOptions.ExtractPageLinks;
			_ExtractPageRangeBox.Text = _expOptions.ExtractPageRange;
			_ExtractPageSettingsBox.Checked = _expOptions.ExtractPageSettings;
			_ExtractPageTextContentBox.Checked = _expOptions.ExportDecodedText;
			_ExportViewerPreferencesBox.Checked = _expOptions.ExportViewerPreferences;
			_PageContentBox.Enabled = _ExtractPageContentBox.Checked = _expOptions.ExtractPageContent;

			_ImportDocumentInfoBox.Checked = _impOptions.ImportDocProperties;
			_ImportBookmarksBox.Checked = _impOptions.ImportBookmarks;
			_ImportPageLinksBox.Checked = _impOptions.ImportPageLinks;
			_ImportPageSettingsBox.Checked = _impOptions.ImportPageSettings;
			_ImportViewerPreferencesBox.Checked = _impOptions.ImportViewerPreferences;
			_KeepOriginalPageLinksBox.Checked = _impOptions.KeepPageLinks;

			var i = _UnitBox.Items.IndexOf(_expOptions.UnitConverter.Unit);
			_UnitBox.SelectedIndex = (i != -1) ? i : 0;
		}

		void OptionChanged(object sender, EventArgs e) {
			if (locked) {
				return;
			}
			if (sender == _ExtractPageDictionaryBox) {
				_expOptions.ExtractPageDictionary = _ExtractPageDictionaryBox.Checked;
			}
			else if (sender == _ExtractPageTextContentBox) {
				_expOptions.ExportDecodedText = _ExtractPageTextContentBox.Checked;
			}
			else if (sender == _ExportDocPropertiesBox) {
				_expOptions.ExportDocProperties = _ExportDocPropertiesBox.Checked;
			}
			else if (sender == _ConsolidateNamedDestBox) {
				_expOptions.ConsolidateNamedDestinations = _ConsolidateNamedDestBox.Checked;
			}
			else if (sender == _ExtractImagesBox) {
				_expOptions.ExtractImages = _ExtractImagesBox.Checked;
			}
			else if (sender == _ExtractPageSettingsBox) {
				_expOptions.ExtractPageSettings = _ExtractPageSettingsBox.Checked;
			}
			else if (sender == _ExportBookmarksBox) {
				_expOptions.ExportBookmarks = _ExportBookmarksBox.Checked;
			}
			else if (sender == _ExtractPageLinksBox) {
				_expOptions.ExtractPageLinks = _ExtractPageLinksBox.Checked;
			}
			else if (sender == _ExportViewerPreferencesBox) {
				_expOptions.ExportViewerPreferences = _ExportViewerPreferencesBox.Checked;
			}
			else if (sender == _ExtractPageContentBox) {
				_expOptions.ExtractPageContent = _PageContentBox.Enabled = _ExtractPageContentBox.Checked;
				_ExtractPageRangeBox.Focus();
			}
			else if (sender == _ExportContentOperatorsBox) {
				_expOptions.ExportContentOperators = _ExportContentOperatorsBox.Checked;
			}
			else if (sender == _ExportBinaryStreamBox) {
				AppContext.Exporter.ExportBinaryStream = (int)_ExportBinaryStreamBox.Value;
			}
			else if (sender == _ExportCatalogBox) {
				AppContext.Exporter.ExportCatalog = _ExportCatalogBox.Checked;
			}
			else if (sender == _ImportDocumentInfoBox) {
				_impOptions.ImportDocProperties = _ImportDocumentInfoBox.Checked;
			}
			else if (sender == _KeepOriginalPageLinksBox) {
				_impOptions.KeepPageLinks = _KeepOriginalPageLinksBox.Checked;
			}
			else if (sender == _ImportBookmarksBox) {
				_impOptions.ImportBookmarks = _ImportBookmarksBox.Checked;
			}
			else if (sender == _ImportPageLinksBox) {
				_impOptions.ImportPageLinks = _ImportPageLinksBox.Checked;
			}
			else if (sender == _ImportPageSettingsBox) {
				_impOptions.ImportPageSettings = _ImportPageSettingsBox.Checked;
			}
			else if (sender == _ImportViewerPreferencesBox) {
				_impOptions.ImportViewerPreferences = _ImportViewerPreferencesBox.Checked;
			}
		}

		void _EncodingBox_Leave(object sender, EventArgs e) {
			try {
				_expOptions.Encoding = _EncodingBox.Text;
				_EncodingBox.Text = _expOptions.Encoding;
			}
			catch (Exception) {
				FormHelper.ErrorBox("输入的编码无效。");
				_EncodingBox.Text = Constants.Encoding.SystemDefault;
			}
		}

		void _ExtractPageRangeBox_Leave(object sender, EventArgs e) {
			_expOptions.ExtractPageRange = _ExtractPageRangeBox.Text.Trim();
			if (_expOptions.ExtractPageRange.Length > 0) {
				_ExtractPageContentBox.Checked = true;
			}
		}

		private void ExportOptionControl_VisibleChanged(object sender, EventArgs e) {
			if (Visible) {
				_ExtractPageRangeBox.Text = AppContext.Exporter.ExtractPageRange;
			}
			else {
				AppContext.Exporter.UnitConverter.Unit = _UnitBox.SelectedItem.ToString();
			}
		}
	}
}
