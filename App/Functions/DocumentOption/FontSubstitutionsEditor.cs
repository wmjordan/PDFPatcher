using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

public partial class FontSubstitutionsEditor : UserControl
{
	private readonly TypedObjectListView<FontSubstitution> _SubstitutionsBox;
	private string _copiedFont;

	private FontUtility.FriendlyFontName[] _Fonts;
	private List<FontSubstitution> _Substitutions;

	public FontSubstitutionsEditor() {
		InitializeComponent();
		_FontSubstitutionsBox.FormatRow += (s, args) => args.Item.SubItems[0].Text = (args.RowIndex + 1).ToText();

		_FontSubstitutionsBox.FullRowSelect = true;
		_FontSubstitutionsBox.HideSelection = false;
		_FontSubstitutionsBox.LabelEdit = false;
		_SubstitutionsBox = new TypedObjectListView<FontSubstitution>(_FontSubstitutionsBox);
		_FontSubstitutionsBox.CellEditStarting += (s, args) => {
			if (args.Column == _SubstitutionColumn) {
				EditSubstitutionItem(args);
			}
			else if (args.Column == _CharSubstitutionColumn) {
				using (FontCharSubstitutionForm f = new(args.RowObject as FontSubstitution)) {
					f.ShowDialog(this);
				}

				args.Cancel = true;
			}
		};
		_FontSubstitutionsBox.CellEditFinishing += (s, args) => {
			if (args.Column == _SubstitutionColumn) {
				ComboBox c = args.Control as ComboBox;
				if (c.FindString(c.Text) != -1) {
					args.NewValue = c.Text;
				}
			}
		};
		new TypedColumn<FontSubstitution>(_OriginalFontColumn) {
			AspectGetter = o => o.OriginalFont,
			AspectPutter = (o, v) => o.OriginalFont = v as string
		};
		new TypedColumn<FontSubstitution>(_SubstitutionColumn) {
			AspectGetter = o => o.Substitution,
			AspectPutter = (o, v) => o.Substitution = v as string
		};
		new TypedColumn<FontSubstitution>(_CharSubstitutionColumn) {
			AspectGetter = o => string.IsNullOrEmpty(o.OriginalCharacters) ? "添加" : "修改"
		};
	}

	[Browsable(false)]
	public List<FontSubstitution> Substitutions {
		get => _Substitutions;
		set {
			_Substitutions = value;
			_FontSubstitutionsBox.Objects = value;
		}
	}

	public PatcherOptions Options { get; set; }

	private void EditSubstitutionItem(CellEditEventArgs args) {
		ComboBox cb = new() {
			AutoCompleteSource = AutoCompleteSource.ListItems,
			AutoCompleteMode = AutoCompleteMode.SuggestAppend,
			Bounds = args.CellBounds
		};
		ComboBox.ObjectCollection b = cb.Items;
		b.Add(string.Empty);
		string sf = (args.RowObject as FontSubstitution).Substitution;
		bool cf = string.IsNullOrEmpty(sf) == false;
		if (cf) {
			sf = sf.ToUpperInvariant();
		}

		if (_Fonts.HasContent() == false) {
			_Fonts = FontUtility.InstalledFonts;
		}

		int l = _Fonts.Length;
		for (int i = 0; i < l; i++) {
			string fn = _Fonts[i].ToString();
			b.Add(fn);
			if (string.Equals(fn, sf, StringComparison.OrdinalIgnoreCase)) {
				cb.SelectedIndex = i + 1;
			}
		}

		if (cb.SelectedIndex == -1) {
			cb.SelectedIndex = 0;
		}

		args.Control = cb;
		cb.ParentChanged += (s1, a) => {
			ComboBox box = (ComboBox)s1;
			if (box.Parent != null) {
				box.DroppedDown = true;
			}
		};
	}

	private void FontSubstitutionsEditor_Load(object sender, EventArgs e) {
		if (DesignMode) {
			return;
		}

		_EmbedLegacyCjkFontsBox.Checked = Options.EmbedFonts;
		_EmbedLegacyCjkFontsBox.CheckedChanged += (s, args) => Options.EmbedFonts = _EmbedLegacyCjkFontsBox.Checked;
		_TrimTrailingWhiteSpaceBox.Checked = Options.TrimTrailingWhiteSpace;
		_TrimTrailingWhiteSpaceBox.CheckedChanged +=
			(s, args) => Options.TrimTrailingWhiteSpace = _TrimTrailingWhiteSpaceBox.Checked;
		_EnableFontSubstitutionsBox.CheckedChanged += (s, args) => {
			_ListDocumentFontButton.Enabled
				= _AddPageLabelButton.Enabled
					= _RemovePageLabelButton.Enabled
						= _FontSubstitutionsBox.Enabled
							= _TrimTrailingWhiteSpaceBox.Enabled
								= Options.EnableFontSubstitutions
									= _EnableFontSubstitutionsBox.Checked;
		};
		_EnableFontSubstitutionsBox.Checked = Options.EnableFontSubstitutions;
		_FontSubstitutionMenu.Invalidate();
	}

	private void _AddPageLabelButton_Click(object sender, EventArgs e) {
		FontSubstitution s = new() { OriginalFont = "请输入原字体名称" };
		_Substitutions.Add(s);
		_FontSubstitutionsBox.AddObject(s);
		_FontSubstitutionsBox.EditSubItem(_FontSubstitutionsBox.GetLastItemInDisplayOrder(), 1);
	}

	private void _RemovePageLabelButton_Click(object sender, EventArgs e) {
		_FontSubstitutionsBox.RemoveObjects(_FontSubstitutionsBox.SelectedObjects);
		_Substitutions.Clear();
		_Substitutions.AddRange(_SubstitutionsBox.Objects);
	}

	private void _ListDocumentFontButton_Click(object sender, EventArgs e) {
		using (DocumentFontListForm f = new()) {
			f.SubstitutionsEditor = this;
			f.ShowDialog();
		}
	}

	internal void AddFonts(IEnumerable<string> fonts) {
		HashSet<string> s = new(StringComparer.CurrentCultureIgnoreCase);
		foreach (FontSubstitution item in _Substitutions) {
			s.Add(item.OriginalFont);
		}

		foreach (string item in fonts) {
			if (s.Contains(item)) {
				continue;
			}

			_Substitutions.Add(new FontSubstitution { OriginalFont = item });
		}

		_SubstitutionsBox.Objects = _Substitutions;
	}

	private void _FontSubstitutionMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		if (e.ClickedItem == _CopySubstitutionFont) {
			_copiedFont = _FontSubstitutionsBox.GetFirstSelectedModel<FontSubstitution>().Substitution;
		}
		else if (e.ClickedItem == _PasteSubstitutionFont) {
			foreach (FontSubstitution item in _SubstitutionsBox.SelectedObjects) {
				item.Substitution = _copiedFont;
			}

			_FontSubstitutionsBox.RefreshSelectedObjects();
		}
	}

	private void _FontSubstitutionMenu_Opening(object sender, CancelEventArgs e) {
		_CopySubstitutionFont.Enabled = _FontSubstitutionsBox.SelectedIndex != -1;
		_PasteSubstitutionFont.Enabled = string.IsNullOrEmpty(_copiedFont) == false;
	}
}