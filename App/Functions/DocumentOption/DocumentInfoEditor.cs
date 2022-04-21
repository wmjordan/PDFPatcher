using System;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	sealed partial class DocumentInfoEditor : UserControl
	{
		bool _settingsLockdown;
		GeneralInfo _Options;
		internal GeneralInfo Options {
			get => _Options;
			set {
				_Options = value;
				_settingsLockdown = true;
				_AuthorBox.Text = _Options.Author;
				_MetadataPanel.Enabled = _ForceMetadataBox.Checked = _Options.SpecifyMetaData;
				_KeywordsBox.Text = _Options.Keywords;
				_SubjectBox.Text = _Options.Subject;
				_TitleBox.Text = _Options.Title;
				_RewriteXmpBox.Checked = _Options.RewriteXmp;
				_settingsLockdown = false;
			}
		}

		public DocumentInfoEditor() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			_settingsLockdown = true;
			_TitleBox.ContextMenuStrip = _SubjectBox.ContextMenuStrip = _AuthorBox.ContextMenuStrip = _KeywordsBox.ContextMenuStrip = _PropertyMacroMenu;
			_PropertyMacroMenu.AddInsertMacroMenuItem(Constants.FileNameMacros.FileName);
			_PropertyMacroMenu.AddInsertMacroMenuItem(Constants.FileNameMacros.FolderName);
			_PropertyMacroMenu.ItemClicked += _PropertyMacroMenu.ProcessInsertMacroCommand;
			_settingsLockdown = false;
		}

		void DocumentInfoChanged(object sender, EventArgs e) {
			if (_settingsLockdown) {
				return;
			}

			if (sender == _AuthorBox) {
				Options.Author = _AuthorBox.Text;
			}
			else if (sender == _ForceMetadataBox) {
				_MetadataPanel.Enabled = Options.SpecifyMetaData = _ForceMetadataBox.Checked;
			}
			else if (sender == _KeywordsBox) {
				Options.Keywords = _KeywordsBox.Text;
			}
			else if (sender == _SubjectBox) {
				Options.Subject = _SubjectBox.Text;
			}
			else if (sender == _TitleBox) {
				Options.Title = _TitleBox.Text;
			}
			else if (sender == _RewriteXmpBox) {
				Options.RewriteXmp = _RewriteXmpBox.Checked;
			}
		}
	}
}
