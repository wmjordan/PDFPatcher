﻿using System.Collections.Generic;
using iTextSharp.text.pdf;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

/// <summary>
///     设置黑白图片的颜色。
/// </summary>
internal sealed class ColorizeBinaryImageProcessor : IPageProcessor
{
	private int _processedPageCount;

	private bool ProcessCommands(IList<PdfPageCommand> parent, IList<PdfName> bwImages) {
		bool r = false;
		PdfPageCommand cmd;
		EnclosingCommand ec;
		for (int i = 0; i < parent.Count; i++) {
			cmd = parent[i];
			ec = cmd as EnclosingCommand;
			if (ec != null) {
				r |= ProcessCommands(ec.Commands, bwImages);
				continue;
			}

			if (cmd.Name.ToString() == "Do") {
				foreach (PdfName item in bwImages) {
					if (item.Equals(cmd.Operands[0])) {
						parent.Insert(i,
							PdfPageCommand.Create("RG", new PdfNumber(1), new PdfNumber(0),
								new PdfNumber(0)));
						parent.Insert(i,
							PdfPageCommand.Create("rg", new PdfNumber(0), new PdfNumber(1),
								new PdfNumber(0)));
						return true;
					}
				}
			}
		}

		return r;
	}

	#region IPageProcessor 成员

	public string Name => "设置黑白图片颜色";

	public void BeginProcess(DocProcessorContext context) {
		_processedPageCount = 0;
	}

	public bool EndProcess(PdfReader pdf) {
		Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
		Tracker.TraceMessage("　　修改了 " + _processedPageCount + " 个黑白图片的颜色。");
		return false;
	}

	public int EstimateWorkload(PdfReader pdf) {
		return pdf.NumberOfPages;
	}

	public bool Process(PageProcessorContext context) {
		Tracker.IncrementProgress(1);
		PdfDictionary images = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
		if (images == null) {
			return false;
		}

		List<PdfName> bw = new();
		foreach (KeyValuePair<PdfName, PdfObject> item in images) {
			PRStream im = PdfReader.GetPdfObject(item.Value) as PRStream;
			if (im == null
				|| PdfName.IMAGE.Equals(im.GetAsName(PdfName.SUBTYPE)) == false
				|| im.TryGetInt32(PdfName.BITSPERCOMPONENT, 0) != 1
			   ) {
				continue;
			}

			bw.Add(item.Key);
		}

		if (ProcessCommands(context.PageCommands.Commands, bw)) {
			context.IsPageContentModified = true;
			_processedPageCount++;
			return true;
		}

		return false;
	}

	#endregion
}