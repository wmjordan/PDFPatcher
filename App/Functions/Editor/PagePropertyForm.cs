using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrightIdeasSoftware;
using MuPDF;
using PDFPatcher.Common;
using MuRectangle = MuPDF.Box;

namespace PDFPatcher.Functions.Editor
{
	sealed partial class PagePropertyForm : DraggableForm
	{
		public int PageNumber { get; set; }

		public PagePropertyForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}
		void OnLoad() {
			_PageDimensionBox.SelectedIndexChanged += _PageDimensionBox_SelectedIndexChanged;
			_CloseButton.Click += (s, args) => Hide();
			_FontNameColumn.AsTyped<MuFontAndSize>(f => f.AspectGetter = o => o.FontName);
			_SizeColumn.AsTyped<MuFontAndSize>(f => f.AspectGetter = o => o.Size);
			_TextStyleBox.ScaleColumnWidths();
		}

		public void LoadPage(Page page) {
			_PageDimensionBox.Items.Clear();
			AddBox(page, page.CropBox, Constants.Content.PageSettings.CropBox);
			AddBox(page, page.MediaBox, Constants.Content.PageSettings.MediaBox);
			AddBox(page, page.TrimBox, Constants.Content.PageSettings.TrimBox);
			AddBox(page, page.ArtBox, Constants.Content.PageSettings.ArtBox);
			AddBox(page, page.BleedBox, Constants.Content.PageSettings.BleedBox);
			_RotationBox.Text = page.Rotation.ToString();
			if (_PageDimensionBox.Items.Count > 0) {
				_PageDimensionBox.SelectedIndex = 0;
			}
			var ts = new HashSet<MuFontAndSize>(new FontAndSizeComparer());
			foreach (var block in page.TextPage) {
				foreach (var line in block) {
					var c = line.FirstCharacter;
					ts.Add(new MuFontAndSize(Model.PdfDocumentFont.RemoveSubsetPrefix(c.Font.Name), c.Size));
				}
			}
			_TextStyleBox.Objects = ts;
			_TextStyleBox.Sort(_SizeColumn, SortOrder.Descending);
			PageNumber = page.PageNumber + 1;
		}

		void AddBox(Page page, MuRectangle rect, string title) {
			if (rect.IsValid && rect.IsEmpty == false) {
				_PageDimensionBox.Items.Add(new Box(rect, title));
			}
		}

		void _PageDimensionBox_SelectedIndexChanged(object sender, EventArgs args) {
			var v = _PageDimensionBox.SelectedItem as Box;
			if (v == null) {
				return;
			}
			var r = v.Rect;
			_TopBox.Text = r.Bottom.ToText();
			_RightBox.Text = r.Right.ToText();
			_BottomBox.Text = r.Top.ToText();
			_LeftBox.Text = r.Left.ToText();
			_WidthBox.Text = r.Width.ToText();
			_HeightBox.Text = r.Height.ToText();
		}

		protected override void OnDeactivate(EventArgs e) {
			Hide();
			base.OnDeactivate(e);
		}

		sealed class Box
		{
			public readonly MuRectangle Rect;
			public readonly string Title;
			public Box(MuRectangle rect, string title) {
				Rect = rect;
				Title = title;
			}
			public override string ToString() {
				return Title;
			}
		}

		sealed class MuFontAndSize
		{
			public readonly string FontName;
			public readonly float Size;

			public MuFontAndSize(string fontName, float size) {
				FontName = fontName;
				Size = size;
			}
		}
		sealed class FontAndSizeComparer : IEqualityComparer<MuFontAndSize>
		{
			public bool Equals(MuFontAndSize x, MuFontAndSize y) {
				return x.FontName == y.FontName && x.Size == y.Size;
			}

			public int GetHashCode(MuFontAndSize obj) {
				return obj.FontName.GetHashCode() ^ obj.Size.GetHashCode();
			}
		}
	}
}
