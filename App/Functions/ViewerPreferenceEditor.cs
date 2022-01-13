using System;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

public partial class ViewerPreferenceEditor : UserControl
{
	private readonly string[] __bookmarkStatus = new string[] {"保持不变", "全部关闭", "全部打开", "打开首层"};
	private bool _settingsLockdown;
	private ViewerOptions _Options;

	internal ViewerOptions Options {
		get => _Options;
		set {
			_Options = value;
			_settingsLockdown = true;
			_ForceRemoveZoomRateBox.Checked = _Options.RemoveZoomRate;
			_FitWindowBox.Checked = _Options.FitWindow;
			_DisplayDocTitleBox.Checked = _Options.DisplayDocTitle;
			_CenterWindowBox.Checked = _Options.CenterWindow;
			_HideMenuBox.Checked = _Options.HideMenu;
			_HideToolbarBox.Checked = _Options.HideToolbar;
			_HideUIBox.Checked = _Options.HideUI;
			_OverrideUISettingsBox.Checked = _Options.SpecifyViewerPreferences;
			_ForceInternalLinkBox.Checked = _Options.ForceInternalLink;

			_ForceBookmarkOpenBox.SelectedIndex = (int)_Options.CollapseBookmark;
			int i = Array.IndexOf(Constants.PageLayoutType.Names, _Options.InitialView);
			_ForceInitialViewBox.SelectedIndex = i != -1 ? i : 0;
			i = Array.IndexOf(Constants.ViewerPreferencesType.DirectionType.Names, _Options.Direction);
			_ForceDirectionBox.SelectedIndex = i != -1 ? i : 0;
			i = Array.IndexOf(Constants.PageModes.Names, _Options.InitialMode);
			_ForceInitialModeBox.SelectedIndex = i != -1 ? i : 0;
			_settingsLockdown = false;
		}
	}

	public ViewerPreferenceEditor() {
		InitializeComponent();

		_settingsLockdown = true;
		_ForceBookmarkOpenBox.FormattingEnabled
			= _ForceDirectionBox.FormattingEnabled
				= _ForceInitialModeBox.FormattingEnabled
					= _ForceInitialViewBox.FormattingEnabled
						= false;
		_ForceBookmarkOpenBox.AddRange(__bookmarkStatus).Select(0);
		_ForceInitialViewBox.AddRange(Constants.PageLayoutType.Names).Select(0);
		_ForceDirectionBox.AddRange(Constants.ViewerPreferencesType.DirectionType.Names).Select(0);
		_ForceInitialModeBox.AddRange(Constants.PageModes.Names).Select(0);
		_settingsLockdown = false;
	}


	private void DocumentInfoChanged(object sender, EventArgs e) {
		if (_settingsLockdown) {
			return;
		}

		if (sender == _ForceBookmarkOpenBox) {
			Options.CollapseBookmark = (BookmarkStatus)_ForceBookmarkOpenBox.SelectedIndex;
		}
		else if (sender == _ForceDirectionBox) {
			Options.Direction = (string)_ForceDirectionBox.SelectedItem;
		}
		else if (sender == _ForceInitialModeBox) {
			Options.InitialMode = (string)_ForceInitialModeBox.SelectedItem;
		}
		else if (sender == _ForceInitialViewBox) {
			Options.InitialView = (string)_ForceInitialViewBox.SelectedItem;
		}
		else if (sender == _ForceRemoveZoomRateBox) {
			Options.RemoveZoomRate = _ForceRemoveZoomRateBox.Checked;
		}
		else if (sender == _OverrideUISettingsBox) {
			Options.SpecifyViewerPreferences
				= _UISettingsPanel.Enabled
					= _OverrideUISettingsBox.Checked;
		}
		else if (sender == _HideMenuBox) {
			Options.HideMenu = _HideMenuBox.Checked;
		}
		else if (sender == _HideToolbarBox) {
			Options.HideToolbar = _HideToolbarBox.Checked;
		}
		else if (sender == _HideUIBox) {
			Options.HideUI = _HideUIBox.Checked;
		}
		else if (sender == _CenterWindowBox) {
			Options.CenterWindow = _CenterWindowBox.Checked;
		}
		else if (sender == _FitWindowBox) {
			Options.FitWindow = _FitWindowBox.Checked;
		}
		else if (sender == _DisplayDocTitleBox) {
			Options.DisplayDocTitle = _DisplayDocTitleBox.Checked;
		}
		else if (sender == _ForceInternalLinkBox) {
			Options.ForceInternalLink = _ForceInternalLinkBox.Checked;
		}
	}
}