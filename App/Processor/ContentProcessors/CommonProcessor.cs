using System;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class CommonProcessor : IPageProcessor
	{
		readonly PatcherOptions _options;
		public CommonProcessor(PatcherOptions options) {
			_options = options;
		}

		#region IPageProcessor 成员
		public string Name => "PDF 常规处理";

		int IPageProcessor.EstimateWorkload(PdfReader pdf) {
			return pdf.NumberOfPages;
		}

		void IPageProcessor.BeginProcess(DocProcessorContext context) {
			if (context.OutputDocument == null) {
				var pdf = context.Pdf;
				var c = pdf.Catalog;
				if (_options.RemoveUsageRights) {
					Tracker.TraceMessage("删除权限控制。");
					pdf.RemoveUsageRights();
					var form = pdf.Catalog.GetAsDict(PdfName.ACROFORM);
					if (form != null) {
						form.Remove(PdfName.SIGFLAGS);
						var fields = form.GetAsArray(PdfName.FIELDS);
						if (fields != null) {
							for (int i = fields.Size - 1; i >= 0; i--) {
								var f = fields.GetAsDict(i);
								if (f != null && PdfName.SIG.Equals(f.GetAsName(PdfName.FT))) {
									fields.Remove(i);
								}
							}
						}
					}
				}
				if (_options.RemoveXmlMetadata) {
					Tracker.TraceMessage("删除 XML 元数据。");
					PdfReader.KillIndirect(c.Get(PdfName.METADATA));
					c.Remove(PdfName.METADATA);
				}

				if (_options.RemoveDocAutoActions) {
					Tracker.TraceMessage("删除打开文档时的自动动作。");
					c.Remove(PdfName.OPENACTION);
					c.Remove(PdfName.AA);
				}
				if (_options.RemoveAnnotations) {
					Tracker.TraceMessage("删除文档批注。");
					pdf.Catalog.Remove(PdfName.ACROFORM);
				}
			}
			if (_options.RemovePageAutoActions) {
				Tracker.TraceMessage("删除页面自动动作。");
			}
			if (_options.RemovePageMetaData) {
				Tracker.TraceMessage("删除页面扩展标记元数据属性。");
			}
		}

		bool IPageProcessor.Process(PageProcessorContext context) {
			bool isTouched = false;
			var page = context.Page;
			if (_options.RemoveAnnotations && page.Contains(PdfName.ANNOTS)) {
				page.Remove(PdfName.ANNOTS);
				isTouched = true;
			}
			if (_options.RemovePageAutoActions && page.Contains(PdfName.AA)) {
				page.Remove(PdfName.AA);
				isTouched = true;
			}
			if (_options.RemovePageMetaData && page.Contains(PdfName.METADATA)) {
				page.Remove(PdfName.METADATA);
				isTouched = true;
			}
			Tracker.IncrementProgress(1);
			return isTouched;
		}

		bool IPageProcessor.EndProcess(PdfReader pdf) {
			return false;
		}

		#endregion

	}
}
