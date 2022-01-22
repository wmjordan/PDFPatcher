using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions;

public partial class PageSettingsEditor : UserControl
{
	private readonly TypedObjectListView<PageBoxSettings> _SettingsBox;
	private List<PageBoxSettings> _Settings;

	public PageSettingsEditor() {
		InitializeComponent();
		_PageSettingsBox.FormatRow += (s, args) => {
			args.Item.SubItems[0].Text = (args.RowIndex + 1).ToText();
		};
		_PageSettingsBox.FixEditControlWidth();
		_PageSettingsBox.FullRowSelect = true;
		_PageSettingsBox.LabelEdit = false;
		_PageSettingsBox.CellClick += (s, args) => {
			if (args.Column == _PageFilterColumn) {
				ShowMenuForClickedCell(args, _PageRangeFilterTypeMenu);
			}
			else if (args.Column == _SettingsColumn) {
				ShowMenuForClickedCell(args, _PageSettingsMenu);
			}
		};
		_SettingsBox = new TypedObjectListView<PageBoxSettings>(_PageSettingsBox);
		_PageRangeFilterTypeMenu.Opening += (s, args) => {
			PageFilterFlag f = _SettingsBox.SelectedObject.Filter;
			_AllPagesMenu.Checked = f == PageFilterFlag.All || f == PageFilterFlag.NotSpecified;
			_OddPagesMenu.Checked = (f & PageFilterFlag.Odd) == PageFilterFlag.Odd;
			_EvenPagesMenu.Checked = (f & PageFilterFlag.Even) == PageFilterFlag.Even;
			_PortraitPagesMenu.Checked = (f & PageFilterFlag.Portrait) == PageFilterFlag.Portrait;
			_LandscapePagesMenu.Checked = (f & PageFilterFlag.Landscape) == PageFilterFlag.Landscape;
		};
		_PageRangeFilterTypeMenu.ItemClicked += (s, args) => {
			PageBoxSettings o = _SettingsBox.SelectedObject;
			ToolStripItem i = args.ClickedItem;
			if (_AllPagesMenu == i) {
				o.Filter = PageFilterFlag.NotSpecified;
			}
			else if (_OddPagesMenu == i) {
				o.Filter &= ~PageFilterFlag.Even;
				o.Filter ^= PageFilterFlag.Odd;
			}
			else if (_EvenPagesMenu == i) {
				o.Filter &= ~PageFilterFlag.Odd;
				o.Filter ^= PageFilterFlag.Even;
			}
			else if (_LandscapePagesMenu == i) {
				o.Filter &= ~PageFilterFlag.Portrait;
				o.Filter ^= PageFilterFlag.Landscape;
			}
			else if (_PortraitPagesMenu == i) {
				o.Filter &= ~PageFilterFlag.Landscape;
				o.Filter ^= PageFilterFlag.Portrait;
			}

			if (o.Filter == PageFilterFlag.All) {
				o.Filter = PageFilterFlag.NotSpecified;
			}

			_PageSettingsBox.RefreshObject(_PageSettingsBox.SelectedObject);
		};
		new TypedColumn<PageBoxSettings>(_PageFilterColumn) {
			AspectGetter = o => {
				PageFilterFlag f = o.Filter;
				PageFilterFlag eo = f & (PageFilterFlag.Even | PageFilterFlag.Odd);
				PageFilterFlag pl = f & (PageFilterFlag.Landscape | PageFilterFlag.Portrait);
				return f == PageFilterFlag.NotSpecified
					? "所有页面"
					: string.Concat(
						eo == PageFilterFlag.Odd ? "单数"
						: eo == PageFilterFlag.Even ? "双数"
						: string.Empty,
						pl == PageFilterFlag.Landscape ? "横向"
						: pl == PageFilterFlag.Portrait ? "纵向"
						: string.Empty,
						"页");
			}
		};
		_RotateMenu.DropDownOpening += (s, args) => {
			int r = _SettingsBox.SelectedObject.Rotation;
			foreach (ToolStripMenuItem item in _RotateMenu.DropDownItems) {
				item.Checked = false;
			}

			switch (r) {
				case 0:
					_RotateZeroMenuItem.Checked = true;
					break;
				case 90:
					_RotateRightMenuItem.Checked = true;
					break;
				case 180:
					_Rotate180MenuItem.Checked = true;
					break;
				case 270:
					_RotateLeftMenuItem.Checked = true;
					break;
				default:
					_RotateZeroMenuItem.Checked = true;
					break;
			}
		};
		_RotateMenu.DropDownItemClicked += (s, args) => {
			PageBoxSettings o = _SettingsBox.SelectedObject;
			ToolStripItem i = args.ClickedItem;
			if (_RotateZeroMenuItem == i) {
				o.Rotation = 0;
			}
			else if (_RotateRightMenuItem == i) {
				o.Rotation = 90;
			}
			else if (_RotateLeftMenuItem == i) {
				o.Rotation = 270;
			}
			else if (_Rotate180MenuItem == i) {
				o.Rotation = 180;
			}

			_PageSettingsBox.RefreshObject(o);
		};
		new TypedColumn<PageBoxSettings>(_SettingsColumn) {
			AspectGetter = o => {
				int r = o.Rotation;
				return string.Concat(
					r == 0 ? Constants.Content.RotationDirections.Zero
					: r == 90 ? Constants.Content.RotationDirections.Right
					: r == 180 ? Constants.Content.RotationDirections.HalfClock
					: r == 270 ? Constants.Content.RotationDirections.Left
					: Constants.Content.RotationDirections.Zero
				);
			}
		};
		new TypedColumn<PageBoxSettings>(_PageRangeColumn) {
			AspectGetter = o => {
				return string.IsNullOrEmpty(o.PageRanges) ? Constants.PageFilterTypes.AllPages : o.PageRanges;
			},
			AspectPutter = (o, v) => {
				string s = v as string;
				o.PageRanges = s != Constants.PageFilterTypes.AllPages ? s : null;
			}
		};
	}

	[Browsable(false)]
	public List<PageBoxSettings> Settings {
		get => _Settings;
		set {
			_Settings = value;
			_SettingsBox.Objects = value;
		}
	}

	private void ShowMenuForClickedCell(CellEventArgs args, ToolStripDropDown menu) {
		Rectangle b = _PageSettingsBox.GetSubItem(args.RowIndex, args.ColumnIndex).Bounds;
		menu.Show(_PageSettingsBox, b.Left, b.Bottom);
	}

	private void _AddPageSettingsButton_Click(object sender, EventArgs e) {
		_Settings.Add(new PageBoxSettings());
		_SettingsBox.Objects = _Settings;
	}

	private void _RemovePageSettingsButton_Click(object sender, EventArgs e) {
		_PageSettingsBox.RemoveObjects(_PageSettingsBox.SelectedObjects);
		_Settings.Clear();
		_Settings.AddRange(_SettingsBox.Objects);
	}
}