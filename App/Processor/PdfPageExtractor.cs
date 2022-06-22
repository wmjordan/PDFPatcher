using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Ext = PDFPatcher.Constants.FileExtensions;

namespace PDFPatcher.Processor
{
	static class PdfPageExtractor
	{
		internal static void ExtractPages(ExtractPageOptions options, string sourceFile, string targetFile, PdfReader pdf) {
			var pn = pdf.NumberOfPages;
			if (pn == 1) {
				Tracker.TraceMessage("文档只有一页，无法拆分。");
				return;
			}
			switch (options.SeparatingMode) {
				case 0: goto default;
				case 1: // 按顶层书签拆分
					SeparateByBookmarks(options, sourceFile, targetFile, ref pdf);
					break;
				case 2: // 按页数拆分
					SeparateByPageNumber(options, sourceFile, targetFile, ref pdf);
					break;
				default:
					SeparateByPageRanges(options, sourceFile, targetFile, ref pdf);
					break;
			}
		}

		static void SeparateByPageNumber(ExtractPageOptions options, string sourceFile, string targetFile, ref PdfReader pdf) {
			var c = 1;
			var pn = pdf.NumberOfPages;
			if (pn <= options.SeparateByPage) {
				Tracker.TraceMessage("拆分的页数超过文档页数，无法拆分。");
				return;
			}
			for (int i = 1; i <= pn; i += options.SeparateByPage) {
				if (pdf == null) {
					pdf = PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);
				}
				var tf = RewriteTargetFileName(sourceFile, targetFile, pdf);
				var e = i + options.SeparateByPage - 1;
				if (e > pn) {
					e = pn;
				}
				//var r = String.Concat (i.ToText (), "-", e.ToText ());
				var s = options.NumberFileNames || c > 1 ? String.Concat("[", c.ToText(), "]") : null;
				if (s != null) {
					tf = string.Concat(tf.Substring(0, tf.LastIndexOf('.')), s, Ext.Pdf);
				}
				ExtractPages(options, sourceFile, tf, pdf, PageRangeCollection.CreateSingle(i, e));
				pdf.Close();
				pdf = null;
				++c;
				Tracker.IncrementProgress(e - i + 1);
			}
		}

		static void SeparateByPageRanges(ExtractPageOptions options, string sourceFile, string targetFile, ref PdfReader pdf) {
			var rl = options.PageRanges.Split(';', '；');
			var pn = pdf.NumberOfPages;
			var i = 1;
			var c = 0;
			foreach (var range in rl) {
				c += PageRangeCollection.Parse(range, 1, pn, true).TotalPages;
			}
			Tracker.SetProgressGoal(c);
			foreach (var range in rl) {
				if (pdf == null) {
					pdf = PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);
				}
				var tf = RewriteTargetFileName(sourceFile, targetFile, pdf);
				var s = options.NumberFileNames || i > 1 ? String.Concat("[", i.ToText(), "]") : null;
				if (s != null) {
					tf = string.Concat(tf.Substring(0, tf.LastIndexOf('.')), s, Ext.Pdf);
				}
				var ranges = PageRangeCollection.Parse(range, 1, pn, true);
				ExtractPages(options, sourceFile, tf, pdf, ranges);
				pdf.Close();
				pdf = null;
				++i;
				Tracker.IncrementProgress(ranges.TotalPages);
			}
		}

		static void SeparateByBookmarks(ExtractPageOptions options, string sourceFile, string targetFile, ref PdfReader pdf) {
			var n = pdf.NumberOfPages;
			Tracker.TraceMessage("导出文档书签。");
			pdf.ConsolidateNamedDestinations();
			var b = OutlineManager.GetBookmark(pdf, new UnitConverter());
			pdf.Close();
			if (b == null) {
				Tracker.TraceMessage("文档没有书签，无法拆分。");
				return;
			}
			Tracker.SetProgressGoal(n);
			var l = new List<KeyValuePair<int, string>>();
			var pp = 0;
			foreach (XmlElement item in b.ChildNodes) {
				if (item == null) {
					continue;
				}
				var p = item.GetAttribute(Constants.DestinationAttributes.Page).ToInt32();
				var t = FileHelper.GetValidFileName(item.GetAttribute(Constants.BookmarkAttributes.Title));
				var a = item.GetAttribute(Constants.DestinationAttributes.Action);
				if (a.Length > 0 && a != Constants.ActionType.Goto) {
					continue;
				}
				if (p == 0 || p > n || p <= pp || t.Length == 0) {
					continue;
				}
				l.Add(new KeyValuePair<int, string>(p, t));
				pp = p;
			}
			if (l.Count == 1 && l[0].Key == 1) {
				Tracker.TraceMessage("文档只有一个有效书签，无法拆分。");
				return;
			}
			if (l[0].Key > 1) {
				l.Insert(0, new KeyValuePair<int, string>(1, Path.GetFileNameWithoutExtension(sourceFile)));
			}
			targetFile = RewriteTargetFileName(sourceFile, targetFile, pdf);
			var dn = Path.GetDirectoryName(targetFile);
			var fn = Path.GetFileNameWithoutExtension(targetFile);
			for (int i = 0; i < l.Count; i++) {
				int s = l[i].Key, e = i < l.Count - 1 ? l[i + 1].Key - 1 : n;
				pdf = PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);
				var tf = FileHelper.CombinePath(dn, String.Concat(
					fn, Path.DirectorySeparatorChar,
					options.NumberFileNames ? (i + 1).ToText() + " - " : String.Empty,
					l[i].Value,
					Ext.Pdf));
				ExtractPages(options, sourceFile, tf, pdf, PageRangeCollection.CreateSingle(s, e));
				pdf.Close();
				Tracker.IncrementProgress(e - s + 1);
			}

		}

		static void ExtractPages(ExtractPageOptions options, string sourceFile, FilePath targetFile, PdfReader pdf, PageRangeCollection ranges) {
			Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
			if (FileHelper.ComparePath(sourceFile, targetFile)) {
				Tracker.TraceMessage(Tracker.Category.Error, "输入文件和输出文件不能相同。");
				return;
			}
			if (FileHelper.CheckOverwrite(targetFile) == false) {
				return;
			}

			var excludeRanges = PageRangeCollection.Parse(options.ExcludePageRanges, 1, pdf.NumberOfPages, false);
			targetFile.CreateContainingDirectory();

			using (var s = new FileStream(targetFile, FileMode.Create)) {
				var pages = new int[ranges.TotalPages];
				var remapper = new int[pdf.NumberOfPages + 1];
				int i = 0;
				foreach (var range in excludeRanges) {
					foreach (var item in range) {
						remapper[item] = -1;
					}
				}
				foreach (var range in ranges) {
					foreach (var item in range) {
						if (remapper[item] < 0) {
							continue;
						}
						pages[i++] = item;
						if (remapper[item] == 0) {
							remapper[item] = i;
						}
					}
				}
				var w = new PdfStamper(pdf, s);
				XmlElement bm = null;
				if (options.KeepBookmarks) {
					Tracker.TraceMessage("导出原文档书签。");
					pdf.ConsolidateNamedDestinations();
					bm = OutlineManager.GetBookmark(pdf, new UnitConverter() { Unit = Constants.Units.Point });
					if (bm?.HasChildNodes == true) {
						var processors = new IInfoDocProcessor[]
							{
								new GotoDestinationProcessor () {
									RemoveOrphanDestination = options.RemoveOrphanBookmarks,
									PageRemapper = remapper,
									TransitionMapper = null
								}
							};
						PdfDocumentCreator.ProcessInfoItem(bm, processors);
					}
				}
				else {
					OutlineManager.KillOutline(pdf);
				}
				pdf.SelectPages(pages);
				if (options.KeepDocumentProperties == false) {
					pdf.Trailer.Remove(PdfName.INFO);
					pdf.Catalog.Remove(PdfName.METADATA);
				}
				if (bm != null) {
					pdf.Catalog.Put(PdfName.OUTLINES, OutlineManager.WriteOutline(w.Writer, bm, ranges.TotalPages));
					w.ViewerPreferences = PdfWriter.PageModeUseOutlines;
				}
				w.Writer.Info.Put(PdfName.PRODUCER, new PdfString(String.Concat(Application.ProductName, " ", Application.ProductVersion)));
				Tracker.TraceMessage("保存文件：" + targetFile);
				if (options.EnableFullCompression) {
					pdf.RemoveUnusedObjects();
					w.SetFullCompression();
				}
				w.Close();
				//Document doc = new Document ();
				//PdfSmartCopy w = new PdfSmartCopy (doc, s);
				//if (impOptions.ImportDocProperties) {
				//    w.Info.Merge (info ?? new PdfDictionary ());
				//}
				//w.Info.Put (PdfName.CREATOR, new PdfString (String.Concat (Application.ProductName, " ", Application.ProductVersion)));
				//doc.Open ();
				//var creator = new PdfDocumentCreator (options, impOptions, doc, w);
				//creator.ProcessFile (options.SourceItems[0]);
				//Tracker.TraceMessage ("保存文件：" + targetFile);
				//var bm = creator.PdfBookmarks;
				//if (bm != null && bm.DocumentElement != null && bm.DocumentElement.HasChildNodes) {
				//    Tracker.TraceMessage ("自动生成文档书签。");
				//    PdfBookmarkUtility.WriteOutline (w, bm.DocumentElement, w.PageEmpty ? w.CurrentPageNumber - 1 : w.CurrentPageNumber);
				//    w.ViewerPreferences = PdfWriter.PageModeUseOutlines;
				//}
				//doc.Close ();
				//w.Close ();
			}
			Tracker.TraceMessage(Tracker.Category.Alert, "成功提取文件内容到 <<" + targetFile + ">>。");
		}

		static string RewriteTargetFileName(string sourceFile, string targetFile, PdfReader pdf) {
			DocInfoExporter.RewriteDocInfoWithEncoding(pdf, AppContext.Encodings.DocInfoEncoding);
			targetFile = Worker.ReplaceTargetFileNameMacros(sourceFile, targetFile, pdf);
			targetFile = FileHelper.MakePathRootedAndWithExtension(targetFile, sourceFile, Ext.Pdf, true);
			return targetFile;
		}

	}
}
