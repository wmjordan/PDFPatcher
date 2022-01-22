using System;
using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

public partial class FontCharSubstitutionForm : Form
{
	private readonly FontSubstitution _Substitution;

	public FontCharSubstitutionForm(FontSubstitution substitution) {
		InitializeComponent();
		this.SetIcon(Resources.Replace);
		MinimumSize = Size;
		MaximumSize = new Size(999, Size.Height);
		_Substitution = substitution;
		_OriginalCharactersBox.Text = substitution.OriginalCharacters;
		_SubstituteCharactersBox.Text = substitution.SubstituteCharacters;
		_ChineseCaseBox.Select(substitution.TraditionalChineseConversion == -1
			? 2
			: substitution.TraditionalChineseConversion);
		_LetterWidthBox.Select(
			substitution.AlphabeticWidthConversion == -1 ? 2 : substitution.AlphabeticWidthConversion);
		_NumericWidthBox.Select(substitution.NumericWidthConversion == -1 ? 2 : substitution.NumericWidthConversion);
		_PunctuationWidthBox.Select(substitution.PunctuationWidthConversion == -1
			? 2
			: substitution.PunctuationWidthConversion);
	}

	private void _OriginalCharactersBox_TextChanged(object sender, EventArgs e) {
		_Substitution.OriginalCharacters = _OriginalCharactersBox.Text;
	}

	private void _SubstituteCharactersBox_TextChanged(object sender, EventArgs e) {
		_Substitution.SubstituteCharacters = _SubstituteCharactersBox.Text;
	}

	private void _SubstituteCharactersBox_Enter(object sender, EventArgs e) {
		_SubstituteCharactersBox.Select(
			_OriginalCharactersBox.SelectionStart,
			_OriginalCharactersBox.SelectionLength
		);
	}

	private void _OriginalCharactersBox_SelectionChanged(object sender, EventArgs e) {
		_SubstituteCharactersBox.Select(
			_OriginalCharactersBox.SelectionStart,
			_OriginalCharactersBox.SelectionLength
		);
	}

	private void _ChineseCaseBox_SelectedIndexChanged(object sender, EventArgs e) {
		_Substitution.TraditionalChineseConversion =
			_ChineseCaseBox.SelectedIndex == 2 ? -1 : _ChineseCaseBox.SelectedIndex;
	}

	private void _NumericWidthBox_SelectedIndexChanged(object sender, EventArgs e) {
		_Substitution.NumericWidthConversion =
			_NumericWidthBox.SelectedIndex == 2 ? -1 : _NumericWidthBox.SelectedIndex;
	}

	private void _LetterWidthBox_SelectedIndexChanged(object sender, EventArgs e) {
		_Substitution.AlphabeticWidthConversion =
			_LetterWidthBox.SelectedIndex == 2 ? -1 : _LetterWidthBox.SelectedIndex;
	}

	private void _PunctuationWidthBox_SelectedIndexChanged(object sender, EventArgs e) {
		_Substitution.PunctuationWidthConversion =
			_PunctuationWidthBox.SelectedIndex == 2 ? -1 : _PunctuationWidthBox.SelectedIndex;
	}
}