using System;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions.Editor.Parts
{
	sealed class MousePositionInfoHandler
	{
		readonly Label _Label;
		readonly ViewerControl _ViewerBox;

		public MousePositionInfoHandler(Label label, ViewerControl viewerBox) {
			_Label = label;
			_ViewerBox = viewerBox;
			viewerBox.MouseMove += HandleViewerBoxMouseMove;
		}

		void HandleViewerBoxMouseMove(object sender, MouseEventArgs e) {
			if (_ViewerBox.FirstPage == 0) {
				return;
			}
			var l = e.Location;
			var p = _ViewerBox.TransposeClientToPagePosition(l.X, l.Y);
			if (p.Page == 0) {
				return;
			}
			var ti = _ViewerBox.FindTextLines(p);
			var t = ti.ToString();
			_Label.Text = string.Concat("页面：",
				p.Page,
				"/",
				_ViewerBox.TotalPageCount.ToText(),
				"; 位置：",
				Math.Round(p.PageX, 2),
				" * ",
				Math.Round(p.PageY, 2),
				ti.Spans.HasContent() ? String.Concat("; 字体：", String.Join(";", ti.GetFontNames()), " ", ti.Spans[0].Size) : null,
				t != null ? "; 文本：" : null,
				t);
		}
	}
}
