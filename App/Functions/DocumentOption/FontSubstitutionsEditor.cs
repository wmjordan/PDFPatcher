using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	sealed partial class FontSubstitutionsEditor : UserControl
	{
		string _copiedFont;

		FontUtility.FriendlyFontName[] _Fonts;
		readonly TypedObjectListView<FontSubstitution> _SubstitutionsBox;
		List<FontSubstitution> _Substitutions;
		[Browsable(false)]
		public List<FontSubstitution> Substitutions {
			get => _Substitutions;
			set { _Substitutions = value; _FontSubstitutionsBox.Objects = value; }
		}
		public PatcherOptions Options { get; set; }

		public FontSubstitutionsEditor() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
			_SubstitutionsBox = new TypedObjectListView<FontSubstitution>(_FontSubstitutionsBox);
			_FontSubstitutionsBox.FormatRow += (s, args) => args.Item.SubItems[0].Text = ValueHelper.ToText(args.RowIndex + 1);
			new TypedColumn<FontSubstitution>(_OriginalFontColumn) {
				AspectGetter = (o) => o.OriginalFont,
				AspectPutter = (o, v) => o.OriginalFont = v as string
			};
			new TypedColumn<FontSubstitution>(_SubstitutionColumn) {
				AspectGetter = (o) => o.Substitution,
				AspectPutter = (o, v) => o.Substitution = v as string
			};
			new TypedColumn<FontSubstitution>(_CharSubstitutionColumn) {
				AspectGetter = (o) => String.IsNullOrEmpty(o.OriginalCharacters) ? "添加" : "修改"
			};
		}

		void OnLoad() {
			_FontSubstitutionsBox.FixEditControlWidth();
			_FontSubstitutionsBox.ScaleColumnWidths();
			_FontSubstitutionsBox.FullRowSelect = true;
			_FontSubstitutionsBox.HideSelection = false;
			_FontSubstitutionsBox.LabelEdit = false;
			_FontSubstitutionsBox.CellEditStarting += (s, args) => {
				if (args.Column == _SubstitutionColumn) {
					EditSubstitutionItem(args);
				}
				else if (args.Column == _CharSubstitutionColumn) {
					using (var f = new FontCharSubstitutionForm(args.RowObject as FontSubstitution)) {
						f.ShowDialog(this);
					}
					args.Cancel = true;
				}
			};
			_FontSubstitutionsBox.CellEditFinishing += (s, args) => {
				if (args.Column == _SubstitutionColumn) {
					var c = args.Control as ComboBox;
					if (c.FindString(c.Text) != -1) {
						args.NewValue = c.Text;
					}
				}
			};
		}

		void EditSubstitutionItem(CellEditEventArgs args) {
			var cb = new ComboBox {
				AutoCompleteSource = AutoCompleteSource.ListItems,
				AutoCompleteMode = AutoCompleteMode.SuggestAppend,
				Bounds = args.CellBounds
			};
			var b = cb.Items;
			b.Add(String.Empty);
			var sf = (args.RowObject as FontSubstitution).Substitution;
			bool cf = String.IsNullOrEmpty(sf) == false;
			if (cf) {
				sf = sf.ToUpperInvariant();
			}
			if (_Fonts.HasContent() == false) {
				_Fonts = FontUtility.InstalledFonts;
			}
			var l = _Fonts.Length;
			string fn;
			for (int i = 0; i < l; i++) {
				fn = _Fonts[i].ToString();
				b.Add(fn);
				if (String.Equals(fn, sf, StringComparison.OrdinalIgnoreCase)) {
					cb.SelectedIndex = i + 1;
				}
			}
			if (cb.SelectedIndex == -1) {
				cb.SelectedIndex = 0;
			}
			args.Control = cb;
			cb.ParentChanged += (s1, a) => {
				var box = ((ComboBox)s1);
				if (box.Parent != null) {
					box.DroppedDown = true;
				}
			};
		}

		void FontSubstitutionsEditor_Load(object sender, EventArgs e) {
			if (DesignMode) {
				return;
			}
			_EmbedLegacyCjkFontsBox.Checked = Options.EmbedFonts;
			_EmbedLegacyCjkFontsBox.CheckedChanged += (s, args) => Options.EmbedFonts = _EmbedLegacyCjkFontsBox.Checked;
			_TrimTrailingWhiteSpaceBox.Checked = Options.TrimTrailingWhiteSpace;
			_TrimTrailingWhiteSpaceBox.CheckedChanged += (s, args) => Options.TrimTrailingWhiteSpace = _TrimTrailingWhiteSpaceBox.Checked;
			_EnableFontSubstitutionsBox.CheckedChanged += (s, args) => {
				_AddSubstitutionButton.Enabled
					= _RemoveSubstitutionButton.Enabled
					= _FontSubstitutionsBox.Enabled
					= _TrimTrailingWhiteSpaceBox.Enabled
					= Options.EnableFontSubstitutions
					= _EnableFontSubstitutionsBox.Checked;
			};
			_EnableFontSubstitutionsBox.Checked = Options.EnableFontSubstitutions;
			_FontSubstitutionMenu.Invalidate();
		}

		void _AddSubstitutionButton_Click(object sender, EventArgs e) {
			var s = new FontSubstitution { OriginalFont = "请输入原字体名称" };
			_Substitutions.Add(s);
			_FontSubstitutionsBox.AddObject(s);
			_FontSubstitutionsBox.EditSubItem(_FontSubstitutionsBox.GetLastItemInDisplayOrder(), 1);
		}

		void _RemoveSubstitutionButton_Click(object sender, EventArgs e) {
			_FontSubstitutionsBox.RemoveObjects(_FontSubstitutionsBox.SelectedObjects);
			_Substitutions.Clear();
			_Substitutions.AddRange(_SubstitutionsBox.Objects);
		}

		void _ListDocumentFontButton_Click(object sender, EventArgs e) {
			using (var f = new DocumentFontListForm()) {
				f.SubstitutionsEditor = this;
				f.ShowDialog();
			}
		}

		internal void AddFonts(IEnumerable<string> fonts) {
			var s = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
			foreach (var item in _Substitutions) {
				s.Add(item.OriginalFont);
			}
			foreach (var item in fonts) {
				if (s.Contains(item)) {
					continue;
				}
				_Substitutions.Add(new FontSubstitution() { OriginalFont = item });
			}
			_SubstitutionsBox.Objects = _Substitutions;
			_EnableFontSubstitutionsBox.Checked = true;
		}

		void _FontSubstitutionMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			if (e.ClickedItem == _CopySubstitutionFont) {
				_copiedFont = _FontSubstitutionsBox.GetFirstSelectedModel<FontSubstitution>().Substitution;
			}
			else if (e.ClickedItem == _PasteSubstitutionFont) {
				foreach (var item in _SubstitutionsBox.SelectedObjects) {
					item.Substitution = _copiedFont;
				}
				_FontSubstitutionsBox.RefreshSelectedObjects();
			}
		}

		void _FontSubstitutionMenu_Opening(object sender, CancelEventArgs e) {
			_CopySubstitutionFont.Enabled = (_FontSubstitutionsBox.SelectedIndex != -1);
			_PasteSubstitutionFont.Enabled = String.IsNullOrEmpty(_copiedFont) == false;
		}

	}
}
