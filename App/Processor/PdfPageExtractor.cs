using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Ext = PDFPatcher.Constants.FileExtensions;

namespace PDFPatcher.Processor;

internal class PdfPageExtractor
{
	internal static void ExtractPages(ExtractPageOptions options, string sourceFile, string targetFile, PdfReader pdf) {
		int pn = pdf.NumberOfPages;
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

	private static void SeparateByPageNumber(ExtractPageOptions options, string sourceFile, string targetFile,
		ref PdfReader pdf) {
		int c = 1;
		int pn = pdf.NumberOfPages;
		if (pn <= options.SeparateByPage) {
			Tracker.TraceMessage("拆分的页数超过文档页数，无法拆分。");
			return;
		}

		for (int i = 1; i <= pn; i += options.SeparateByPage) {
			pdf ??= PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);

			string tf = RewriteTargetFileName(sourceFile, targetFile, pdf);
			int e = i + options.SeparateByPage - 1;
			if (e > pn) {
				e = pn;
			}

			//var r = String.Concat (i.ToText (), "-", e.ToText ());
			string s = options.NumberFileNames || c > 1 ? string.Concat("[", c.ToText(), "]") : null;
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

	private static void SeparateByPageRanges(ExtractPageOptions options, string sourceFile, string targetFile,
		ref PdfReader pdf) {
		string[] rl = options.PageRanges.Split(';', '；');
		int pn = pdf.NumberOfPages;
		int i = 1;
		int c = rl.Sum(range => PageRangeCollection.Parse(range, 1, pn, true).TotalPages);

		Tracker.SetProgressGoal(c);
		foreach (string range in rl) {
			pdf ??= PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);

			string tf = RewriteTargetFileName(sourceFile, targetFile, pdf);
			string s = options.NumberFileNames || i > 1 ? string.Concat("[", i.ToText(), "]") : null;
			if (s != null) {
				tf = string.Concat(tf.Substring(0, tf.LastIndexOf('.')), s, Ext.Pdf);
			}

			PageRangeCollection ranges = PageRangeCollection.Parse(range, 1, pn, true);
			ExtractPages(options, sourceFile, tf, pdf, ranges);
			pdf.Close();
			pdf = null;
			++i;
			Tracker.IncrementProgress(ranges.TotalPages);
		}
	}

	private static void SeparateByBookmarks(ExtractPageOptions options, string sourceFile, string targetFile,
		ref PdfReader pdf) {
		int n = pdf.NumberOfPages;
		Tracker.TraceMessage("导出文档书签。");
		pdf.ConsolidateNamedDestinations();
		XmlElement b = OutlineManager.GetBookmark(pdf, new UnitConverter());
		pdf.Close();
		if (b == null) {
			Tracker.TraceMessage("文档没有书签，无法拆分。");
			return;
		}

		Tracker.SetProgressGoal(n);
		List<KeyValuePair<int, string>> l = new();
		int pp = 0;
		foreach (XmlElement item in b.ChildNodes) {
			if (item == null) {
				continue;
			}

			int p = item.GetAttribute(Constants.DestinationAttributes.Page).ToInt32();
			string t = FileHelper.GetValidFileName(item.GetAttribute(Constants.BookmarkAttributes.Title));
			string a = item.GetAttribute(Constants.DestinationAttributes.Action);
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
		string dn = Path.GetDirectoryName(targetFile);
		string fn = Path.GetFileNameWithoutExtension(targetFile);
		for (int i = 0; i < l.Count; i++) {
			int s = l[i].Key, e = i < l.Count - 1 ? l[i + 1].Key - 1 : n;
			//var pr = String.Concat (s.ToText (), "-", e.ToText ());
			pdf = PdfHelper.OpenPdfFile(sourceFile, AppContext.LoadPartialPdfFile, false);
			string tf = FileHelper.CombinePath(dn, string.Concat(
				fn, Path.DirectorySeparatorChar,
				options.NumberFileNames ? (i + 1).ToText() + " - " : string.Empty,
				l[i].Value,
				Ext.Pdf));
			ExtractPages(options, sourceFile, tf, pdf, PageRangeCollection.CreateSingle(s, e));
			pdf.Close();
			Tracker.IncrementProgress(e - s + 1);
		}
	}

	private static void ExtractPages(ExtractPageOptions options, string sourceFile, FilePath targetFile, PdfReader pdf,
		PageRangeCollection ranges) {
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		if (FileHelper.ComparePath(sourceFile, targetFile)) {
			Tracker.TraceMessage(Tracker.Category.Error, "输入文件和输出文件不能相同。");
			return;
		}

		if (FileHelper.CheckOverwrite(targetFile) == false) {
			return;
		}

		PageRangeCollection excludeRanges =
			PageRangeCollection.Parse(options.ExcludePageRanges, 1, pdf.NumberOfPages, false);
		targetFile.CreateContainingDirectory();

		using Stream s = new FileStream(targetFile, FileMode.Create);
		int[] pages = new int[ranges.TotalPages];
		int[] remapper = new int[pdf.NumberOfPages + 1];
		int i = 0;
		foreach (int item in excludeRanges.SelectMany(range => range)) {
			remapper[item] = -1;
		}

		foreach (int item in from range in ranges from item in range where remapper[item] >= 0 select item) {
			pages[i++] = item;
			if (remapper[item] == 0) {
				remapper[item] = i;
			}
		}

		PdfStamper w = new(pdf, s);
		XmlElement bm = null;
		if (options.KeepBookmarks) {
			Tracker.TraceMessage("导出原文档书签。");
			pdf.ConsolidateNamedDestinations();
			bm = OutlineManager.GetBookmark(pdf, new UnitConverter { Unit = Constants.Units.Point });
			if (bm is { HasChildNodes: true }) {
				IInfoDocProcessor[] processors = {
					new GotoDestinationProcessor {
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

		w.Writer.Info.Put(PdfName.PRODUCER,
			new PdfString(string.Concat(Application.ProductName, " ", Application.ProductVersion)));
		Tracker.TraceMessage("保存文件：" + targetFile);
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
		Tracker.TraceMessage(Tracker.Category.Alert, "成功提取文件内容到 <<" + targetFile + ">>。");
	}

	private static string RewriteTargetFileName(string sourceFile, string targetFile, PdfReader pdf) {
		DocInfoExporter.RewriteDocInfoWithEncoding(pdf, AppContext.Encodings.DocInfoEncoding);
		targetFile = Worker.ReplaceTargetFileNameMacros(sourceFile, targetFile, pdf);
		targetFile = FileHelper.MakePathRootedAndWithExtension(targetFile, sourceFile, Ext.Pdf, true);
		return targetFile;
	}
}