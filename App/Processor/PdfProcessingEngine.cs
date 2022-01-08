using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFPatcher.Processor
{
	sealed class PdfProcessingEngine
	{
		///<summary>获取文档处理器列表。</summary>
		internal List<IDocProcessor> DocumentProcessors { get; private set; }

		///<summary>获取页面处理器列表。</summary>
		internal List<IPageProcessor> PageProcessors { get; private set; }

		public PdfReader Pdf { get; private set; }
		public Dictionary<int, object> ExtraData { get; private set; }

		public PdfProcessingEngine(PdfReader pdf) {
			DocumentProcessors = new List<IDocProcessor>();
			PageProcessors = new List<IPageProcessor>();
			Pdf = pdf;
			ExtraData = new Dictionary<int, object>();
		}

		public void CreateProcessors(PatcherOptions settings) {
			if (settings.RemoveBookmarks) {
				DocumentProcessors.Add(new RemoveBookmarkProcessor());
			}
			if (settings.FixContents) {
				PageProcessors.Add(new FixContentProcessor());
			}
			if (settings.EmbedFonts || settings.EnableFontSubstitutions && settings.FontSubstitutions.Count > 0) {
				var d = new Dictionary<string, FontSubstitution>(settings.FontSubstitutions.Count, StringComparer.CurrentCultureIgnoreCase);
				if (settings.EnableFontSubstitutions) {
					foreach (var item in settings.FontSubstitutions) {
						if (String.IsNullOrEmpty(item.OriginalFont) || String.IsNullOrEmpty(item.Substitution)) {
							continue;
						}
						d[item.OriginalFont] = item;
					}
				}
				if (settings.EmbedFonts || d.Count > 0) {
					PageProcessors.Add(new ReplaceFontProcessor(settings.EmbedFonts, settings.TrimTrailingWhiteSpace, d));
				}
			}
			if (settings.RemovePageForms) {
				PageProcessors.Add(new RemoveFormProcessor());
			}
			if (settings.RemovePageLinks) {
				PageProcessors.Add(new RemoveAnnotationProcessor(PdfName.LINK));
			}
			if (settings.RecompressWithJbig2) {
				PageProcessors.Add(new ImageRecompressor());
				//this.PageProcessors.Add (new ColorizeBinaryImageProcessor ());
			}
			if (settings.RemovePageTextBlocks) {
				PageProcessors.Add(new RemoveTextBlockProcessor());
			}
			if (settings.RemovePageThumbnails) {
				PageProcessors.Add(new RemoveThumbnailProcessor());
			}
			if (settings.UnifiedPageSettings.NeedAdjustMargins || settings.UnifiedPageSettings.NeedResize) {
				PageProcessors.Add(new PageDimensionProcessor { Settings = settings.UnifiedPageSettings });
			}
			if (settings.RemoveLeadingCommandCount > 0 || settings.RemoveTrailingCommandCount > 0) {
				PageProcessors.Add(new RemoveWrappedCommandProcessor(settings.RemoveLeadingCommandCount, settings.RemoveTrailingCommandCount));
			}
			if (settings.PageSettings.Count > 0) {
				foreach (var item in settings.PageSettings) {
					PageProcessors.Add(new PageDimensionProcessor { Settings = item });
				}
			}
			//if (settings.DeskewImages) {
			//    this.PageProcessors.Add (new ImageDeskewProcessor ());
			//}
			PageProcessors.Add(new CommonProcessor(settings));
		}

		internal int EstimateWorkload() {
			int workload = 0;
			foreach (var p in DocumentProcessors) {
				workload += p.EstimateWorkload(Pdf);
			}
			foreach (var p in PageProcessors) {
				workload += p.EstimateWorkload(Pdf);
			}
			return workload;
		}

		internal void ProcessDocument(PdfWriter writer) {
			ProcessDocument(writer, null);
		}
		internal void ProcessDocument(PdfWriter writer, Document document) {
			var pp = PageProcessors.ToArray();
			var dc = new DocProcessorContext(this, writer, document);
			foreach (var p in DocumentProcessors) {
				Tracker.TraceMessage(p.Name);
				p.BeginProcess(dc);
			}
			foreach (var p in pp) {
				Tracker.TraceMessage(p.Name);
				p.BeginProcess(dc);
			}
			foreach (var p in DocumentProcessors) {
				p.Process(dc);
			}
			var pn = Pdf.NumberOfPages;
			var i = 0;
			while ((++i <= pn)) {
				var pc = new PageProcessorContext(Pdf, i);
				try {
					foreach (var p in pp) {
						p.Process(pc);
					}
					if (pc.IsPageContentModified) {
						pc.WritePageCommands();
					}
				}
				catch (Exception ex) {
					Tracker.TraceMessage("在处理文档第 " + i + " 页时出错。");
					throw;
				}
			}
			foreach (var p in DocumentProcessors) {
				p.EndProcess(dc);
			}
			foreach (var p in pp) {
				p.EndProcess(Pdf);
			}
		}
	}
}
