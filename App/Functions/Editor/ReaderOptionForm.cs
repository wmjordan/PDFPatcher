using System;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor;

public partial class ReaderOptionForm : Form
{
	bool _uiLockDown;
	public ReaderOptionForm() {
		InitializeComponent();
		this.OnFirstLoad(OnLoad);
	}

	void OnLoad() {
		this.SetIcon(Properties.Resources.PdfOptions);
		MinimumSize = Size;
		var fonts = Array.ConvertAll(new InstalledFontCollection().Families, i => i.Name);
		Array.Sort(fonts, StringComparer.OrdinalIgnoreCase);
		_BookmarkFontBox.Items.AddRange(fonts);
		_ZoomRateBox.Items.AddRange(ReaderOptions.ZoomModes);

		Reload();
	}

	public void Reset() {
		AppContext.Reader = new ReaderOptions();
		Reload();
	}

	void Reload() {
		_uiLockDown = true;
		var options = AppContext.Reader;
		_ShowTextBorderBox.Checked = options.ShowTextBoder;
		_ShowAnnotationBox.Checked = options.ShowAnnotation;
		_GrayScaleBox.Checked = options.GrayScale;
		_FullPageScrollBox.Checked = options.FullPageScroll;
		_ZoomRateBox.Text = options.Zoom.SubstituteDefault("自动");
		_DirectionBox.Select((int)options.ContentDirection);

		_ShowBookmarkBox.Select((int)options.BookmarkState);
		_BookmarkFontBox.Text = options.BookmarkFont;
		_AutoEditNextBookmarkBox.Checked = options.ContinuousBookmarkEdit;
		_LocateOnBookmarkEditBox.Checked = options.LocateBookmarkOnEdit;

		_uiLockDown = false;
	}

	protected override void OnClosed(EventArgs e) {
		base.OnClosed(e);
		var options = AppContext.Reader;
		options.ShowTextBoder = _ShowTextBorderBox.Checked;
		options.ShowAnnotation = _ShowAnnotationBox.Checked;
		options.GrayScale = _GrayScaleBox.Checked;
		options.FullPageScroll = _FullPageScrollBox.Checked;
		options.ContentDirection = (Editor.ContentDirection)_DirectionBox.SelectedIndex;
		options.Zoom = _ZoomRateBox.Text;

		options.BookmarkState = (BookmarkState)_ShowBookmarkBox.SelectedIndex;
		options.BookmarkFont = _BookmarkFontBox.Text;
		options.ContinuousBookmarkEdit = _AutoEditNextBookmarkBox.Checked;
		options.LocateBookmarkOnEdit = _LocateOnBookmarkEditBox.Checked;
	}
}
