using System.Collections.Generic;
using System.IO;
using System.Linq;
using FreeImageAPI;
using iTextSharp.text.pdf;
using PDFPatcher.Model;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Processor;

internal sealed class ImageRecompressor : IPageProcessor
{
	private static readonly PdfName[] __IgnoreFilters = { PdfName.DCTDECODE, PdfName.JBIG2DECODE };

	private static readonly ImageExtracterOptions _imgExpOption = new() {
		OutputPath = Path.GetTempPath(),
		MergeImages = false
	};

	private int _optimizedImageCount;

	private int _processedImageCount;

	#region IPageProcessor 成员

	public string Name => "优化压缩黑白图片";

	public void BeginProcess(DocProcessorContext context) {
		_processedImageCount = 0;
		_optimizedImageCount = 0;
	}

	public bool EndProcess(PdfReader pdf) {
		Tracker.TraceMessage(Tracker.Category.Notice, Name + "功能：");
		Tracker.TraceMessage("　　处理了 " + _processedImageCount + " 幅图片。");
		Tracker.TraceMessage("　　优化了 " + _optimizedImageCount + " 幅图片的压缩率。");
		return false;
	}

	public int EstimateWorkload(PdfReader pdf) {
		return pdf.NumberOfPages * 10;
	}

	public bool Process(PageProcessorContext context) {
		Tracker.IncrementProgress(10);
		ImageExtractor _imgExp = new(_imgExpOption, context.Pdf);
		PdfDictionary images = context.Page.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
		if (images == null) {
			return false;
		}

		foreach (KeyValuePair<PdfName, PdfObject> item in images) {
			if (PdfReader.GetPdfObject(item.Value) is not PRStream im
				|| PdfName.IMAGE.Equals(im.GetAsName(PdfName.SUBTYPE)) == false) {
				continue;
			}

			_processedImageCount++;
			PdfNumber l = im.GetAsNumber(PdfName.LENGTH);
			if (l == null || l.IntValue < 400 /*忽略小图片*/) {
				continue;
			}

			PdfObject f = im.Get(PdfName.FILTER);
			PdfName fn = null;
			if (f.Type == PdfObject.ARRAY) {
				PdfArray fl = f as PdfArray;
				fn = fl.GetAsName(fl.Size - 1);
			}
			else if (f.Type == PdfObject.NAME) {
				fn = f as PdfName;
			}

			if (fn != null && __IgnoreFilters.Contains(fn)) {
				continue;
			}

			if (OptimizeBinaryImage(item.Value as PdfIndirectReference, im, l.IntValue)
				|| ReplaceJ2kImage(item.Value as PdfIndirectReference, im, fn)) {
			}
		}

		return true;
	}

	private bool OptimizeBinaryImage(PdfIndirectReference imgRef, PRStream imgStream, int length) {
		PdfNumber bpc = imgStream.GetAsNumber(PdfName.BITSPERCOMPONENT);
		PdfBoolean mask = imgStream.GetAsBoolean(PdfName.IMAGEMASK);
		if ((bpc == null && (mask == null || mask.BooleanValue == false))
			|| (bpc != null && bpc.IntValue != 1)) {
			return false;
		}

		ImageInfo info = new(imgRef);
		byte[] bytes = info.DecodeImage(_imgExpOption);
		using FreeImageBitmap fi = ImageExtractor.CreateFreeImageBitmap(info, ref bytes, false, false);
		byte[] sb = JBig2Encoder.Encode(fi);
		if (sb.Length > length) {
			return false;
		}

		imgStream.SetData(sb, false);
		imgStream.Put(PdfName.FILTER, PdfName.JBIG2DECODE);
		if (imgStream.GetAsArray(PdfName.COLORSPACE) == null) {
			imgStream.Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
		}

		imgStream.Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
		imgStream.Put(PdfName.LENGTH, new PdfNumber(sb.Length));
		imgStream.Remove(PdfName.K);
		imgStream.Remove(PdfName.ENDOFLINE);
		imgStream.Remove(PdfName.ENCODEDBYTEALIGN);
		imgStream.Remove(PdfName.COLUMNS);
		imgStream.Remove(PdfName.ROWS);
		imgStream.Remove(PdfName.ENDOFBLOCK);
		imgStream.Remove(PdfName.BLACKIS1);
		imgStream.Remove(PdfName.PREDICTOR);
		imgStream.Remove(PdfName.COLORS);
		imgStream.Remove(PdfName.COLUMNS);
		imgStream.Remove(PdfName.EARLYCHANGE);
		imgStream.Remove(PdfName.DECODEPARMS);
		imgStream.Remove(PdfName.DECODE);
		_optimizedImageCount++;

		return true;
	}

	private bool ReplaceJ2kImage(PdfIndirectReference imgRef, PRStream imgStream, PdfName filter) {
		if (PdfName.JPXDECODE.Equals(filter) == false) {
			return false;
		}

		ImageInfo info = new(imgRef);
		byte[] jpg;
		using (MemoryStream ms = new(info.DecodeImage(_imgExpOption)))
		using (MemoryStream js = new())
		using (FreeImageBitmap fi = new(ms)) {
			fi.Save(js, FREE_IMAGE_FORMAT.FIF_JPEG,
				FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE | FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL |
				FREE_IMAGE_SAVE_FLAGS.JPEG_PROGRESSIVE);
			jpg = js.ToArray();
		}

		imgStream.SetData(jpg, false);
		imgStream.Put(PdfName.FILTER, PdfName.DCTDECODE);
		imgStream.Put(PdfName.LENGTH, new PdfNumber(jpg.Length));
		_optimizedImageCount++;
		return true;
	}

	#endregion
}