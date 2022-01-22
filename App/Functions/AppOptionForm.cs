using System;
using System.ComponentModel;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public partial class AppOptionForm : Form, IResettableControl
{
	private bool locked;

	public AppOptionForm() {
		InitializeComponent();
		Reload();
		this.SetIcon(Resources.AppOptions);
		_BookmarkEncodingBox.SelectedIndexChanged += ControlChanged;
		_DocInfoEncodingBox.SelectedIndexChanged += ControlChanged;
		_TextEncodingBox.SelectedIndexChanged += ControlChanged;
		_FontNameEncodingBox.SelectedIndexChanged += ControlChanged;
		_SaveAppSettingsBox.CheckedChanged += ControlChanged;
		_LoadEntireFileBox.CheckedChanged += ControlChanged;
	}

	public void Reset() {
		AppContext.SaveAppSettings = true;
		AppContext.LoadPartialPdfFile = false;
		//ContextData.PdfReaderPath = String.Empty;
		AppContext.Encodings = new EncodingOptions();
		Reload();
	}

	public void Reload() {
		locked = true;
		_SaveAppSettingsBox.Checked = AppContext.SaveAppSettings;
		_LoadPartialFileBox.Checked = AppContext.LoadPartialPdfFile;
		_LoadEntireFileBox.Checked = !AppContext.LoadPartialPdfFile;
		//_PdfReaderPathBox.Text = ContextData.PdfReaderPath;

		InitEncodingList(_BookmarkEncodingBox, AppContext.Encodings.BookmarkEncodingName);
		InitEncodingList(_DocInfoEncodingBox, AppContext.Encodings.DocInfoEncodingName);
		InitEncodingList(_TextEncodingBox, AppContext.Encodings.TextEncodingName);
		InitEncodingList(_FontNameEncodingBox, AppContext.Encodings.FontNameEncodingName);

		locked = false;
	}

	private static void InitEncodingList(ComboBox list, string encodingName) {
		list.Items.Clear();
		foreach (string item in Constants.Encoding.EncodingNames) {
			list.Items.Add(item);
			if (encodingName == item) {
				list.SelectedIndex = list.Items.Count - 1;
			}
		}

		if (list.SelectedIndex == -1) {
			list.SelectedIndex = 0;
		}
	}

	private void ControlChanged(object sender, EventArgs e) {
		if (locked) {
			return;
		}

		if (sender == _DocInfoEncodingBox) {
			AppContext.Encodings.DocInfoEncodingName = _DocInfoEncodingBox.SelectedItem.ToString();
		}
		else if (sender == _BookmarkEncodingBox) {
			AppContext.Encodings.BookmarkEncodingName = _BookmarkEncodingBox.SelectedItem.ToString();
		}
		else if (sender == _TextEncodingBox) {
			AppContext.Encodings.TextEncodingName = _TextEncodingBox.SelectedItem.ToString();
		}
		else if (sender == _FontNameEncodingBox) {
			AppContext.Encodings.FontNameEncodingName = _FontNameEncodingBox.SelectedItem.ToString();
		}
		else if (sender == _SaveAppSettingsBox) {
			AppContext.SaveAppSettings = _SaveAppSettingsBox.Checked;
		}
		else if (sender == _LoadEntireFileBox) {
			AppContext.LoadPartialPdfFile = _LoadPartialFileBox.Checked;
		}
	}

	private void _CreateShortcutButton_Click(object sender, EventArgs e) {
		CommonCommands.CreateShortcut();
	}
}