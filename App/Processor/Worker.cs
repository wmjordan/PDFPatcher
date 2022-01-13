using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using FreeImageAPI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MuPdfSharp;
using PDFPatcher.Common;
using PDFPatcher.Model;
using Ext = PDFPatcher.Constants.FileExtensions;

namespace PDFPatcher.Processor;

internal static class Worker
{
	private const string OperationCanceled = "已经取消操作。";

	private static PdfReader OpenPdf(string sourceFile, bool loadPartial, bool removeUnusedObjects) {
		try {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage,
				string.Concat("打开 PDF 文件：<<", sourceFile, ">>。"));
			return PdfHelper.OpenPdfFile(sourceFile, loadPartial, removeUnusedObjects);
		}
		catch (FileNotFoundException) {
			FormHelper.ErrorBox(string.Concat("找不到文件：“", sourceFile, "”。"));
			return null;
		}
		catch (iTextSharp.text.exceptions.BadPasswordException) {
			Tracker.TraceMessage(Tracker.Category.Error, Messages.PasswordInvalid);
			FormHelper.ErrorBox(Messages.PasswordInvalid);
			return null;
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在打开 PDF 文件时遇到错误：\n" + ex.Message);
#if DEBUG
			Tracker.TraceMessage(ex);
#endif
			return null;
		}
	}

	internal static void ExtractImages(string sourceFile, ImageExtracterOptions options) {
		const int loadDocProgressWeight = 10;
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		PdfReader pdf = OpenPdf(sourceFile, true, false); // 由于导出图片一般不需要往复访问文档，可用此选项降低内存占用及提高打开速度
		if (pdf == null) {
			return;
		}

		string targetPath = options.OutputPath;
		if (Directory.Exists(targetPath) == false) {
			Directory.CreateDirectory(targetPath);
		}

		PageRangeCollection ranges = PageRangeCollection.Parse(options.PageRange, 1, pdf.NumberOfPages, true);
		int loadCount = loadDocProgressWeight + ranges.TotalPages;
		Tracker.SetProgressGoal(loadCount);
		string op = targetPath;
		string om = options.FileMask;
		try {
			Tracker.TraceMessage("正在导出图片。");
			Tracker.TrackProgress(loadDocProgressWeight);
			if (FileHelper.HasFileNameMacro(op)) {
				options.OutputPath = ReplaceTargetFileNameMacros(sourceFile, op, pdf);
			}

			if (FileHelper.HasFileNameMacro(om)) {
				options.FileMask = ReplaceTargetFileNameMacros(sourceFile, om, pdf);
			}

			ImageExtractor exp = new(options, pdf);
			foreach (PageRange range in ranges) {
				foreach (int i in range) {
					exp.ExtractPageImages(pdf, i);
					if (exp.InfoList.Count > 0) {
						Tracker.TraceMessage(Tracker.Category.OutputFile,
							exp.InfoList[exp.InfoList.Count - 1].FileName);
					}

					Tracker.IncrementProgress(1);
				}
			}

			Tracker.TrackProgress(loadCount);
			Tracker.TraceMessage(Tracker.Category.Alert, "成功提取图片文件，存放目录为：<<" + targetPath + ">>。");
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出图片时遇到错误：\n" + ex.Message);
		}
		finally {
			options.OutputPath = op;
			options.FileMask = om;
			pdf?.Close();
		}
	}

	internal static void RenderPages(string sourceFile, ImageRendererOptions options) {
		const int loadDocProgressWeight = 10;
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		options.TintColor = Color.Transparent;
		if (Directory.Exists(options.ExtractImagePath) == false) {
			Directory.CreateDirectory(options.ExtractImagePath);
		}

		MuDocument mupdf = null;
		try {
			mupdf = PdfHelper.OpenMuDocument(sourceFile);
			PageRangeCollection ranges = PageRangeCollection.Parse(options.ExtractPageRange, 1, mupdf.PageCount, true);
			int loadCount = loadDocProgressWeight + ranges.TotalPages;
			Tracker.SetProgressGoal(loadCount);
			Tracker.TraceMessage("正在转换图片。");
			Tracker.TrackProgress(loadDocProgressWeight);
			foreach (PageRange range in ranges) {
				foreach (int i in range) {
					string fn = FileHelper.CombinePath(options.ExtractImagePath,
						i.ToString(options.FileMask) + options.FileFormatExtension);
					using (MuPage p = mupdf.LoadPage(i))
					using (Bitmap bmp = p.RenderBitmapPage(options.ImageWidth, 0, options)) {
						if (bmp == null) {
							Tracker.TraceMessage(Tracker.Category.Error, "页面" + i + "的尺寸为空。");
						}
						else if (options.FileFormat == ImageFormat.Tiff) {
							using (Bitmap b = Imaging.BitmapHelper.ToBitonal(bmp)) {
								Imaging.BitmapHelper.SaveAs(b, fn);
							}
						}
						else {
							Color[] uc = Imaging.BitmapHelper.GetPallete(bmp);
							if (uc.Length > 256 && options.Quantize) {
								using (Bitmap b = Imaging.WuQuantizer.QuantizeImage(bmp)) {
									Imaging.BitmapHelper.SaveAs(b, fn);
								}
							}
							else if (uc.Length <= 256 && Imaging.BitmapHelper.IsIndexed(bmp) == false) {
								using (Bitmap b = Imaging.BitmapHelper.ToIndexImage(bmp, uc)) {
									Imaging.BitmapHelper.SaveAs(b, fn);
								}
							}
							else if (options.FileFormat == ImageFormat.Jpeg) {
								Imaging.JpgHelper.Save(bmp, fn, options.JpegQuality);
							}
							else {
								Imaging.BitmapHelper.SaveAs(bmp, fn);
							}
						}
					}

					Tracker.TraceMessage(Tracker.Category.OutputFile, fn);
					Tracker.IncrementProgress(1);
				}
			}

			Tracker.TrackProgress(loadCount);
			Tracker.TraceMessage(Tracker.Category.Alert, "成功转换图片文件，存放目录为：<<" + options.ExtractImagePath + ">>。");
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在转换图片时遇到错误：\n" + ex.Message);
		}
		finally {
			if (mupdf != null) {
				mupdf.Dispose();
			}
		}
	}

	internal static void ExportInfo(string sourceFile, string targetFile) {
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		PdfReader r = OpenPdf(sourceFile, AppContext.LoadPartialPdfFile, false);
		if (r == null) {
			return;
		}

		if (FileHelper.IsPathValid(targetFile) == false || Path.GetFileName(targetFile).Length == 0) {
			Tracker.TraceMessage(Tracker.Category.Error, Messages.InfoFileNameInvalid);
			FormHelper.ErrorBox(Messages.InfoFileNameInvalid);
			return;
		}

		targetFile = FileHelper.MakePathRootedAndWithExtension(targetFile, sourceFile, Ext.Xml, false);
		DocInfoExporter export = new(r, AppContext.Exporter);
		if (AppContext.Exporter.ExtractImages) {
			AppContext.Exporter.Images.OutputPath = FileHelper.CombinePath(Path.GetDirectoryName(targetFile),
				Path.GetFileNameWithoutExtension(targetFile) + "图片文件\\");
		}

		try {
			Tracker.TraceMessage("正在导出信息文件。");
			if (targetFile.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) {
				Tracker.SetProgressGoal(50);
				using (TextWriter w = new StreamWriter(targetFile, false, AppContext.Exporter.GetEncoding())) {
					DocInfoExporter.WriteDocumentInfoAttributes(w, sourceFile, r.NumberOfPages);
					export.ExportDocument(w);
					w.WriteLine();
					Tracker.SetProgressGoal(10);
					r.ConsolidateNamedDestinations();
					export.ExportBookmarks(OutlineManager.GetBookmark(r, new UnitConverter()), w, 0, false);
				}
			}
			else {
				int workload = export.EstimateWorkload();
				Tracker.SetProgressGoal(workload);
				using (XmlWriter w = XmlWriter.Create(targetFile, DocInfoExporter.GetWriterSettings())) {
					w.WriteStartDocument();
					w.WriteStartElement(Constants.PdfInfo);
					DocInfoExporter.WriteDocumentInfoAttributes(w, sourceFile, r.NumberOfPages);
					export.ExportDocument(w);
					w.WriteEndElement();
				}
			}

			Tracker.TraceMessage(Tracker.Category.Alert, "成功导出信息文件到 <<" + targetFile + ">>。");
			//if (this._BookmarkBox.Text.Length == 0) {
			//    this._BookmarkBox.Text = targetFile;
			//}
			//if (Common.Form.YesNoBox ("已完成导出信息文件到 " + targetFile + "，是否使用内置的编辑器编辑文件？") == DialogResult.Yes) {
			//    ShowInfoFileEditorForm (targetFile);
			//}
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (System.Text.EncoderFallbackException ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message + "\n\n请选择在导出信息选项中选择其它编码方式。");
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message);
		}
		finally {
			r?.Close();
		}
	}

	internal static void ConvertBookmark(string sourceFile, string targetFile) {
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		try {
			PdfInfoXmlDocument infoDoc = new();
			OutlineManager.ImportSimpleBookmarks(sourceFile, infoDoc);
			using (XmlWriter w = XmlWriter.Create(targetFile, DocInfoExporter.GetWriterSettings())) {
				infoDoc.Save(w);
			}

			Tracker.TraceMessage(Tracker.Category.Alert, "成功转换信息文件到 <<" + targetFile + ">>。");
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在转换书签文件时发生错误：\n" + ex.Message);
		}
	}

	internal static void PatchDocument(SourceItem.Pdf sourceFile, FilePath targetFile, object infoDoc,
		ImporterOptions options, PatcherOptions pdfSettings) {
		string sourcePath = sourceFile.FilePath.ToString();
		if (sourceFile.FilePath.IsEmpty) {
			Tracker.TraceMessage(Tracker.Category.Error, "输入文件名为空。");
			return;
		}

		if (string.IsNullOrEmpty(targetFile) || targetFile.IsEmpty) {
			Tracker.TraceMessage(Tracker.Category.Error, "输出文件名为空。");
			return;
		}

		Tracker.TraceMessage(Tracker.Category.InputFile, sourcePath);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		bool overwriteSource = false;
		PdfReader pdf = OpenPdf(sourcePath, AppContext.LoadPartialPdfFile, true);
		if (pdf == null) {
			return;
		}

		try {
			DocInfoImporter import;
			PdfInfoXmlDocument xInfoDoc = infoDoc as PdfInfoXmlDocument;
			string docPath = infoDoc as string;
			if (xInfoDoc != null) {
				import = new DocInfoImporter(options, pdf, pdfSettings, xInfoDoc.BookmarkRoot);
			}
			else if (string.IsNullOrEmpty(docPath)) {
				Tracker.TraceMessage("没有指定信息文件，将按程序界面的设置执行补丁。");
				import = new DocInfoImporter(options, pdf, pdfSettings, null);
				Tracker.TraceMessage("加载源 PDF 文件信息完毕，准备执行补丁操作。");
			}
			else {
				Tracker.TraceMessage(Tracker.Category.ImportantMessage, string.Concat("加载信息文件：<<", docPath, ">>。"));
				import = new DocInfoImporter(options, docPath);
				if (import.InfoDoc != null && VerifyInfoDocument(import.InfoDoc) == false) {
					return;
				}
			}

			PdfProcessingEngine pdfEngine = new(pdf);
			pdfEngine.CreateProcessors(pdfSettings);
			int workload = 110;
			workload += pdfEngine.EstimateWorkload();
			Tracker.SetProgressGoal(workload);
			Tracker.TrackProgress(10);

			if (pdf.ConfirmUnethicalMode() == false) {
				return;
			}

			Tracker.TraceMessage("导入文档属性。");
			//var pdfInfo = DocInfoExporter.RewriteDocInfoWithEncoding (pdf.Trailer.GetAsDict (PdfName.INFO), AppContext.Encodings.DocInfoEncoding);
			GeneralInfo info = pdfSettings.MetaData.SpecifyMetaData
				? pdfSettings.MetaData
				//: sourceFile.DocInfo;
				// 不再使用书签文件的文件属性
				: import.ImportDocumentInfomation()
				  ?? sourceFile.DocInfo;
			DocInfoImporter.ImportDocumentInfomation(info, pdf, sourcePath);
			Tracker.TrackProgress(20);

			if (FileHelper.HasFileNameMacro(targetFile)) {
				targetFile = ReplaceTargetFileNameMacros(sourcePath, targetFile, pdf);
			}

			targetFile = FileHelper.MakePathRootedAndWithExtension(targetFile, sourcePath, Ext.Pdf, true);
			targetFile = targetFile.Normalize();

			Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
			if (FileHelper.ComparePath(sourcePath, targetFile)) {
				if (FormHelper.YesNoBox("是否覆盖原始 PDF 文件？\n如处理过程出现错误，覆盖操作将导致原始数据无法恢复！") == DialogResult.No) {
					Tracker.TraceMessage(Tracker.Category.Error, Messages.SourceFileEqualsTargetFile);
					return;
				}

				overwriteSource = true;
				targetFile = sourceFile.FilePath.ChangeExtension(Ext.Tmp).ToString();
			}
			else if (FileHelper.CheckOverwrite(targetFile) == false) {
				return;
			}

			targetFile.CreateContainingDirectory();

			using (Stream s = new FileStream(targetFile, FileMode.Create)) {
				PdfStamper st = new(pdf, s);
				pdfEngine.ProcessDocument(st.Writer);

				#region 处理信息文件

				List<IInfoDocProcessor> processors = new();
				if (pdfSettings.ViewerPreferences.RemoveZoomRate) {
					processors.Add(new RemoveZoomRateProcessor());
				}

				if (pdfEngine.ExtraData.ContainsKey(DocProcessorContext.CoordinateTransition) &&
				    pdfSettings.UnifiedPageSettings.ScaleContent) {
					processors.Add(new GotoDestinationProcessor() {
						TransitionMapper =
							pdfEngine.ExtraData[DocProcessorContext.CoordinateTransition] as
								CoordinateTranslationSettings[]
					});
				}

				if (pdfSettings.ViewerPreferences.ForceInternalLink) {
					processors.Add(new ForceInternalDestinationProcessor());
				}

				//var cts = new CoordinateTranslationSettings[pdf.NumberOfPages + 1]; // 页面的位置偏移量
				//var sc = false;
				//if (pdfSettings.PageSettings.Count > 0) {
				//    Tracker.TraceMessage ("重设页面尺寸。");
				//    pdf.ResetReleasePage ();
				//    foreach (var item in pdfSettings.PageSettings) {
				//        var ranges = PageRangeCollection.Parse (item.PageRanges, 1, pdf.NumberOfPages, true);
				//        foreach (var range in ranges) {
				//            foreach (var i in range) {
				//                var s = PageDimensionProcessor.ResizePage (pdf.GetPageN (i), item.PaperSize, item.HorizontalAlign, item.VerticalAlign, -1, item.ScaleContent);
				//                if (item.ScaleContent && s.XScale != 1 && s.YScale != 1) {
				//                    PageDimensionProcessor.ScaleContent (pdf, i, s);
				//                    cts[i] = s; // TODO: 需要解决重复指定相同页面的问题
				//                    sc = true;
				//                }
				//            }
				//        }
				//    }
				//    pdf.ResetReleasePage ();
				//}
				if (pdfSettings.UnifiedPageSettings.PaperSize.PaperName == PaperSize.AsPageSize) {
					import.ImportPageSettings(pdf);
				}

				//if (sc == false) {
				//    cts = null;
				//}

				#endregion

				if (pdfSettings.FullCompression) {
					st.SetFullCompression();
				}

				//st.Writer.CompressionLevel = ContextData.CreateDocumentOptions.CompressionLevel;
				PdfPageLabels labels = DocInfoImporter.ImportPageLabels(pdfSettings.PageLabels) ??
				                       import.ImportPageLabels();
				if (labels != null) {
					st.Writer.PageLabels = labels;
				}

				if (options.ImportPageLinks ||
				    pdfSettings.UnifiedPageSettings.PaperSize.PaperName != PaperSize.AsPageSize /* sc*/) {
					import.ImportPageLinks(pdf, st);
				}

				PdfDocumentCreator.ProcessInfoItem(
					import.InfoDoc.DocumentElement.SelectSingleNode(Constants.PageLink) as XmlElement, processors);
				PdfDocumentCreator.ProcessInfoItem(
					import.InfoDoc.DocumentElement.SelectSingleNode(Constants.NamedDestination) as XmlElement,
					processors);
				XmlElement bookmarks = null;
				if ((options.ImportBookmarks && pdfSettings.RemoveBookmarks == false) || xInfoDoc != null) {
					Tracker.TraceMessage("导入书签。");
					bookmarks = import.GetBookmarks() ??
					            OutlineManager.GetBookmark(pdf, new UnitConverter() {Unit = Constants.Units.Point});
				}

				if (bookmarks != null) {
					// 预处理书签
					processors.Add(new CollapseBookmarkProcessor() {
						BookmarkStatus = pdfSettings.ViewerPreferences.CollapseBookmark
					});
					PdfDocumentCreator.ProcessInfoItem(bookmarks, processors);
					if (bookmarks.ChildNodes.Count > 0 || xInfoDoc != null) {
						import.ImportNamedDestinations(pdf, st.Writer);
						OutlineManager.KillOutline(pdf);
						PdfIndirectReference bm = OutlineManager.WriteOutline(st.Writer, bookmarks, pdf.NumberOfPages);
						if (bm != null) {
							pdf.Catalog.Put(PdfName.OUTLINES, bm);
						}

						if (pdf.Catalog.Contains(PdfName.PAGEMODE) == false) {
							pdf.Catalog.Put(PdfName.PAGEMODE, PdfName.USEOUTLINES);
						}
					}
					else if (string.IsNullOrEmpty(docPath) == false) {
						OutlineManager.KillOutline(pdf);
					}
				}

				Tracker.IncrementProgress(10);
				Tracker.TraceMessage("导入文档设置。");
				import.ImportViewerPreferences(pdf);
				DocInfoImporter.OverrideViewerPreferences(pdfSettings.ViewerPreferences, pdf, st.Writer);
				//import.OverrideDocumentSettings (pdf);
				Tracker.IncrementProgress(5);
				Tracker.TraceMessage("清理输出文件。");
				pdf.RemoveUnusedObjects();
				if (pdf.AcroForm == null) {
					pdf.Trailer.Locate<PdfDictionary>(PdfName.ROOT).Remove(PdfName.ACROFORM);
				}

				Tracker.IncrementProgress(10);
				Tracker.TraceMessage("保存文件：" + targetFile);
				st.Close();
				Tracker.TrackProgress(workload);
			}
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
			return;
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导入信息时发生错误：\n" + ex.Message);
			return;
		}
		finally {
			if (pdf != null) {
				try {
					pdf.Close();
				}
				catch (Exception ex) {
					// ignore exception
					Tracker.TraceMessage(ex);
				}
			}
		}

		if (overwriteSource) {
			try {
				Tracker.TraceMessage("覆盖输入文件。");
				File.Delete(sourcePath);
				File.Move(targetFile, sourcePath);
				targetFile = sourcePath;
			}
			catch (Exception) {
				FormHelper.ErrorBox("无法覆盖输入文件。\n请手工将“" + Ext.Tmp + "”后缀的临时文件更名为输入文件。");
			}
		}

		Tracker.TraceMessage(Tracker.Category.Alert, "成功导入信息到 <<" + targetFile + ">>。");
	}

	internal static string ReplaceTargetFileNameMacros(string sourceFile, string targetFile, PdfReader pdf) {
		try {
			return ReplaceTargetFileNameMacros(sourceFile, targetFile, pdf.Trailer.GetAsDict(PdfName.INFO));
		}
		catch (IOException) {
			return ReplaceTargetFileNameMacros(sourceFile, targetFile, null as PdfDictionary);
		}
	}

	/// <summary>
	/// 替换目标文件名的替代符。
	/// </summary>
	/// <param name="sourceFile">用于替换的源文件名。</param>
	/// <param name="targetFile">包含替代符的目标文件名。</param>
	/// <param name="info">源文件的元数据属性。</param>
	/// <returns>替换目标文件名后的文件名。</returns>
	internal static string ReplaceTargetFileNameMacros(string sourceFile, string targetFile, PdfDictionary info) {
		string p = null; // 文档属性
		if (info != null) {
			if (info.Contains(PdfName.TITLE)) {
				p = FileHelper.GetValidFileName(info.GetAsString(PdfName.TITLE).ToUnicodeString());
			}

			targetFile = targetFile.Replace(Constants.FileNameMacros.TitleProperty,
				p ?? Path.GetFileNameWithoutExtension(sourceFile));
			p = string.Empty;
			if (info.Contains(PdfName.SUBJECT)) {
				p = FileHelper.GetValidFileName(info.GetAsString(PdfName.SUBJECT).ToUnicodeString());
			}

			targetFile = targetFile.Replace(Constants.FileNameMacros.SubjectProperty, p);
			p = string.Empty;
			if (info.Contains(PdfName.AUTHOR)) {
				p = FileHelper.GetValidFileName(info.GetAsString(PdfName.AUTHOR).ToUnicodeString());
			}

			targetFile = targetFile.Replace(Constants.FileNameMacros.AuthorProperty, p);
			p = string.Empty;
			if (info.Contains(PdfName.KEYWORDS)) {
				p = FileHelper.GetValidFileName(info.GetAsString(PdfName.KEYWORDS).ToUnicodeString());
			}

			targetFile = targetFile.Replace(Constants.FileNameMacros.KeywordsProperty, p);
		}

		return targetFile
			.Replace(Constants.FileNameMacros.FileName, Path.GetFileNameWithoutExtension(sourceFile))
			.Replace(Constants.FileNameMacros.FolderName, Path.GetFileName(Path.GetDirectoryName(sourceFile)))
			.Replace(Constants.FileNameMacros.PathName,
				FileHelper.CombinePath(sourceFile.Substring(0, sourceFile.LastIndexOf(Path.DirectorySeparatorChar)),
					string.Empty));
	}

	private static bool VerifyInfoDocument(XmlDocument infoDoc) {
		XmlElement root = infoDoc.DocumentElement;
		switch (root.Name) {
			case Constants.PdfInfo:
				// 使用中文的书签
				string v = root.GetAttribute(Constants.Info.ProductVersion);
				if (v != Constants.InfoDocVersion
				    && FormHelper.YesNoBox(string.Concat("信息文件不是用这个版本的程序生成的，可能会导入不成功，是否继续？\n当前程序的版本是：",
					    Application.ProductVersion, "\n信息文件的导出程序版本是：", v)) == DialogResult.No) {
					return false;
				}

				break;
			default:
				FormHelper.ErrorBox("信息文件格式有误，根元素不是“" + Constants.PdfInfo + "”。");
				return false;
		}

		return true;
	}

	internal static void ExtractPages(ExtractPageOptions options, string sourceFile, string targetFile) {
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		PdfReader r = OpenPdf(sourceFile, AppContext.LoadPartialPdfFile, false);
		if (r == null) {
			return;
		}

		try {
			if (r.ConfirmUnethicalMode() == false) {
				return;
			}

			PdfPageExtractor.ExtractPages(options, sourceFile, targetFile, r);
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出页面内容时出错：\n" + ex.Message);
		}
		finally {
			if (r != null) {
				try {
					r.Close();
				}
				catch (Exception ex) {
					// ignore exception
					Tracker.TraceMessage(ex);
				}
			}
		}
	}

	internal static void MergeDocuments(ICollection<SourceItem> sources, FilePath targetFile, string infoFile) {
		targetFile = targetFile.EnsureExtension(Ext.Pdf);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile.ToString());
		MergerOptions option = AppContext.Merger;
		ImporterOptions impOptions = AppContext.Importer;
		Document doc = null;
		DocumentSink sink = new(sources, true);
		if (sink.Workload == 0) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, "合并文件列表不包含图片或 PDF 文件。");
			return;
		}

		Tracker.SetProgressGoal(10 + sink.Workload);
		Tracker.TrackProgress(1);
		try {
			GeneralInfo info = null;
			PdfPageLabels labels = null;
			BookmarkContainer bookmarks = null;
			if (string.IsNullOrEmpty(infoFile) == false) {
				Tracker.TraceMessage(Tracker.Category.ImportantMessage,
					string.Concat("加载信息文件：<<", infoFile, ">>。"));
				DocInfoImporter import = new(impOptions, infoFile);
				info = import.ImportDocumentInfomation();
				labels = import.ImportPageLabels();
				bookmarks = import.GetBookmarks();
			}

			if (labels == null && option.PageLabels.Count > 0) {
				labels = DocInfoImporter.ImportPageLabels(option.PageLabels);
			}

			FilePath f = targetFile.EnsureExtension(Ext.Pdf);
			Tracker.TraceMessage(Tracker.Category.OutputFile, f.ToString());
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, string.Concat("输出到文件：", f, "。"));
			if (f.IsValidPath == false) {
				Tracker.TraceMessage(Tracker.Category.Error, "输出文件路径无效。");
				return;
			}

			f.CreateContainingDirectory();
			using (Stream s = new FileStream(f, FileMode.Create)) {
				PageBoxSettings ps = option.PageSettings;
				doc = new Document(
					new iTextSharp.text.Rectangle(ps.PaperSize.Width, ps.PaperSize.Height),
					ps.Margins.Left, ps.Margins.Right, ps.Margins.Top, ps.Margins.Bottom
				);
				PdfSmartCopy w = new(doc, s);
				if (option.FullCompression) {
					w.SetFullCompression();
				}

				//w.CompressionLevel = ContextData.CreateDocumentOptions.CompressionLevel;
				doc.Open();
				doc.AddCreator(Application.ProductName + " " + Application.ProductVersion);
				if (labels != null) {
					w.PageLabels = labels;
				}

				Tracker.IncrementProgress(10);
				PdfDocumentCreator creator = new(sink, option, impOptions, doc, w);
				foreach (SourceItem item in sources) {
					creator.ProcessFile(item, creator.PdfBookmarks.BookmarkRoot);
				}

				Tracker.TraceMessage("设置文档选项。");
				DocInfoImporter.ImportDocumentInfomation(option.MetaData.SpecifyMetaData
					? option.MetaData
					: info, doc);
				DocInfoImporter.OverrideViewerPreferences(option.ViewerPreferences, null, w);
				if ((bookmarks == null || bookmarks.HasChildNodes == false) &&
				    creator.PdfBookmarks.DocumentElement.HasChildNodes) {
					bookmarks = creator.PdfBookmarks.BookmarkRoot;
				}

				if (bookmarks != null && bookmarks.HasChildNodes) {
					Tracker.TraceMessage("写入文档书签。");
					OutlineManager.WriteOutline(w, bookmarks,
						w.PageEmpty ? w.CurrentPageNumber - 1 : w.CurrentPageNumber);
					w.ViewerPreferences = PdfWriter.PageModeUseOutlines;
				}

				Tracker.TraceMessage("写入文件索引。");
				Tracker.TraceMessage(Tracker.Category.Alert, "生成文件：<<" + targetFile.ToString() + ">>。");
				w.Close();
			}
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在生成文档时出错：\n" + ex.Message);
		}
		finally {
			if (doc != null) {
				try {
					doc.Close();
				}
				catch (Exception ex) {
					// ignore exception
					Tracker.TraceMessage(ex);
				}
			}
		}
	}

	internal static void CreateBookmark(string sourceFile, string bookmarkFile, AutoBookmarkOptions options) {
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);
		PdfReader r = OpenPdf(sourceFile, AppContext.LoadPartialPdfFile, false);
		if (r == null) {
			return;
		}

		if (FileHelper.IsPathValid(bookmarkFile) == false || Path.GetFileName(bookmarkFile).Length == 0) {
			Tracker.TraceMessage(Tracker.Category.Error, Messages.InfoFileNameInvalid);
			FormHelper.ErrorBox(Messages.InfoFileNameInvalid);
			return;
		}

		bookmarkFile = FileHelper.MakePathRootedAndWithExtension(bookmarkFile, sourceFile, Ext.Xml, true);
		Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);

		if (options.CreateBookmarkForFirstPage) {
			options.FirstPageTitle = Path.GetFileNameWithoutExtension(sourceFile);
		}

		AutoBookmarkCreator creator = new(r, options);

		try {
			Tracker.TraceMessage("正在分析 PDF 文件。");
			int workload = creator.EstimateWorkload();
			Tracker.SetProgressGoal(workload);
			using (XmlWriter w = XmlWriter.Create(bookmarkFile, DocInfoExporter.GetWriterSettings())) {
				w.WriteStartDocument();
				w.WriteStartElement(Constants.PdfInfo);
				w.WriteAttributeString(Constants.Info.ProductName, Application.ProductName);
				w.WriteAttributeString(Constants.Info.ProductVersion, Constants.InfoDocVersion);
				w.WriteAttributeString(Constants.Info.ExportDate, DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));
				//w.WriteAttributeString (Constants.Info.DocumentName, Path.GetFileNameWithoutExtension (sourceFile));
				w.WriteAttributeString(Constants.Info.DocumentPath, sourceFile);
				w.WriteAttributeString(Constants.Info.PageNumber, r.NumberOfPages.ToText());
				creator.ExportAutoBookmarks(w, options);
				w.WriteEndElement();
			}

			Tracker.TraceMessage(Tracker.Category.Alert, "成功导出信息文件到 <<" + bookmarkFile + ">>。");
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (System.Text.EncoderFallbackException ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message + "\n\n请选择在导出信息选项中选择其它编码方式。");
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message);
		}
		finally {
			if (r != null) {
				r.Close();
			}
		}
	}

	internal static void Ocr(string sourceFile, string bookmarkFile, OcrOptions options) {
		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);
		PdfReader r = OpenPdf(sourceFile, AppContext.LoadPartialPdfFile, false);
		if (r == null) {
			return;
		}

		bool noOutputFile = false;
		if (string.IsNullOrEmpty(bookmarkFile)) {
			noOutputFile = true;
		}
		else if (FileHelper.IsPathValid(bookmarkFile) == false || Path.GetFileName(bookmarkFile).Length == 0) {
			Tracker.TraceMessage(Tracker.Category.Error, Messages.InfoFileNameInvalid);
			FormHelper.ErrorBox(Messages.InfoFileNameInvalid);
			return;
		}

		try {
			if (FileHelper.CheckOverwrite(bookmarkFile) == false) {
				return;
			}
		}
		catch (OperationCanceledException) {
			return;
		}

		bookmarkFile = FileHelper.MakePathRootedAndWithExtension(bookmarkFile, sourceFile, Ext.Xml, false);
		Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);

		OcrProcessor ocr;
		try {
			ocr = new OcrProcessor(r, options);
		}
		catch (FileNotFoundException ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox(ex.Message);
			return;
		}

		try {
			Tracker.TraceMessage("正在分析 PDF 文件。");
			int workload = ocr.EstimateWorkload();
			Tracker.SetProgressGoal(workload);
			if (noOutputFile) {
				ocr.PerformOcr();
			}
			else if (new FilePath(bookmarkFile).HasExtension(Ext.Txt)) {
				Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);
				Tracker.TraceMessage("输出简易信息文件：" + bookmarkFile);
				using (TextWriter w = new StreamWriter(bookmarkFile, false, AppContext.Exporter.GetEncoding())) {
					DocInfoExporter.WriteDocumentInfoAttributes(w, sourceFile, r.NumberOfPages);
					ocr.SetWriter(w);
					ocr.PerformOcr();
				}
			}
			else {
				Tracker.TraceMessage(Tracker.Category.OutputFile, bookmarkFile);
				Tracker.TraceMessage("输出信息文件：" + bookmarkFile);
				using (XmlWriter w = XmlWriter.Create(bookmarkFile, DocInfoExporter.GetWriterSettings())) {
					w.WriteStartDocument();
					w.WriteStartElement(Constants.PdfInfo);
					DocInfoExporter.WriteDocumentInfoAttributes(w, sourceFile, r.NumberOfPages);
					ocr.SetWriter(w);
					ocr.PerformOcr();
					w.WriteEndElement();
				}
			}

			if (noOutputFile == false) {
				Tracker.TraceMessage(Tracker.Category.Alert, "已完成导出信息文件到 <<" + bookmarkFile + ">>。");
			}
			else {
				Tracker.TraceMessage(Tracker.Category.Alert, "分析文档完毕。");
			}
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (System.Text.EncoderFallbackException ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message + "\n\n请选择在导出信息选项中选择其它编码方式。");
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导出信息文件时遇到错误：\n" + ex.Message);
		}
		finally {
			if (r != null) {
				r.Close();
			}
		}
	}

	internal static string RenameFile(string template, SourceItem.Pdf item) {
		string t;
		FilePath s = item.FilePath;
		PdfDictionary d = new();
		DocInfoImporter.UpdateInfoValue(d, PdfName.TITLE, item.DocInfo.Title, s);
		DocInfoImporter.UpdateInfoValue(d, PdfName.AUTHOR, item.DocInfo.Author, s);
		DocInfoImporter.UpdateInfoValue(d, PdfName.SUBJECT, item.DocInfo.Subject, s);
		DocInfoImporter.UpdateInfoValue(d, PdfName.KEYWORDS, item.DocInfo.Keywords, s);
		t = ReplaceTargetFileNameMacros(s.ToString(), template, d);
		if (FileHelper.IsPathValid(t) == false) {
			return t;
		}
		else if (Path.GetFileName(t).Length > 0) {
			t = new FilePath(t).EnsureExtension(Ext.Pdf).ToString();
		}

		return Path.GetFullPath(t);
	}

	internal static void RenameFiles(List<SourceItem.Pdf> items, string template, bool keepSourceFile) {
		Tracker.SetTotalProgressGoal(items.Count);
		Tracker.TraceMessage(string.Concat("使用“", template, "”模板重命名。"));
		foreach (SourceItem.Pdf item in items) {
			try {
				FilePath s = item.FilePath.ToFullPath();
				if (s.ExistsFile == false) {
					Tracker.TraceMessage(Tracker.Category.Error, string.Concat("找不到 PDF 文件：", s));
					continue;
				}

				string t = RenameFile(template, item);
				Tracker.TraceMessage(Tracker.Category.InputFile, s.ToString());
				Tracker.TraceMessage(Tracker.Category.OutputFile, t);
				Tracker.TraceMessage(Tracker.Category.ImportantMessage, string.Concat("重命名 PDF 文件：", s));
				Tracker.TraceMessage(Tracker.Category.ImportantMessage, string.Concat("到目标 PDF 文件：<<", t, ">>。"));
				if (FileHelper.IsPathValid(t) == false) {
					Tracker.TraceMessage(Tracker.Category.Error, string.Concat("输出文件名 ", t, " 无效。"));
					goto Exit;
				}

				if (s.Equals(t)) {
					Tracker.TraceMessage("源文件与目标文件名称相同。不需重命名。");
					goto Exit;
				}
				else if (Path.GetFileName(t).Trim().Length == 0) {
					Tracker.TraceMessage("输出文件名为空，无法重命名。");
					goto Exit;
				}
				else if (File.Exists(t)) {
					DialogResult r = FormHelper.YesNoCancelBox("是否覆盖已存在的 PDF 文件：" + t);
					if (r == DialogResult.No) {
						goto Exit;
					}
					else if (r == DialogResult.Cancel) {
						throw new OperationCanceledException();
					}

					// r == DialogResult.Yes
					File.Delete(t);
				}

				if (Directory.Exists(Path.GetDirectoryName(t)) == false) {
					Directory.CreateDirectory(Path.GetDirectoryName(t));
				}

				if (keepSourceFile) {
					File.Copy(s.ToString(), t);
				}
				else {
					File.Move(s.ToString(), t);
				}
			}
			catch (OperationCanceledException) {
				Tracker.TraceMessage(Tracker.Category.Alert, "已取消重命名操作。");
				return;
			}
			catch (Exception ex) {
				Tracker.TraceMessage(ex);
			}

			Exit:
			Tracker.IncrementTotalProgress();
		}

		Tracker.TraceMessage("重命名操作已完成。");
	}

	internal static void ImportOcr(string sourceFile, string infoFile, string targetFile) {
		if (FileHelper.IsPathValid(sourceFile) == false) {
			Tracker.TraceMessage(Tracker.Category.Error, "输入 PDF 文件路径无效。");
			return;
		}

		if (FileHelper.IsPathValid(targetFile) == false) {
			Tracker.TraceMessage(Tracker.Category.Error, "输出 PDF 文件路径无效。");
			return;
		}

		if (FileHelper.IsPathValid(infoFile) == false) {
			Tracker.TraceMessage(Tracker.Category.Error, "信息文件路径无效。");
			return;
		}

		Tracker.TraceMessage(Tracker.Category.InputFile, sourceFile);
		Tracker.TraceMessage(Tracker.Category.OutputFile, targetFile);
		if (FileHelper.ComparePath(sourceFile, targetFile)) {
			Tracker.TraceMessage(Tracker.Category.Error, "输入文件和输出文件不能相同。");
			return;
		}

		if (FileHelper.CheckOverwrite(targetFile) == false) {
			return;
		}

		PdfReader pdf = OpenPdf(sourceFile, AppContext.LoadPartialPdfFile, false);
		if (pdf == null) {
			return;
		}

		try {
			using (XmlReader infoReader = XmlReader.Create(infoFile,
				       new XmlReaderSettings() {IgnoreComments = true, IgnoreProcessingInstructions = true})) {
				infoReader.MoveToContent(); // 移到根元素
				using (Stream s = new FileStream(targetFile, FileMode.Create)) {
					PdfStamper st = new(pdf, s);
					PdfProcessingEngine en = new(pdf);
					en.ExtraData[DocProcessorContext.OcrData] = infoReader;
					en.DocumentProcessors.Add(new ImportOcrResultProcessor());
					Tracker.SetProgressGoal(en.EstimateWorkload());
					en.ProcessDocument(st.Writer);
					st.Close();
				}
			}

			Tracker.TraceMessage(Tracker.Category.ImportantMessage, "成功写入识别结果到文件：<<" + targetFile + ">>。");
		}
		catch (OperationCanceledException) {
			Tracker.TraceMessage(Tracker.Category.ImportantMessage, OperationCanceled);
		}
		catch (Exception ex) {
			Tracker.TraceMessage(ex);
			FormHelper.ErrorBox("在导入信息时发生错误：\n" + ex.Message);
		}
		finally {
			if (pdf != null) {
				try {
					pdf.Close();
				}
				catch (Exception ex) {
					// ignore exception
					Tracker.TraceMessage(ex);
				}
			}
		}
	}
}