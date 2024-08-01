using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Model;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Processor
{
	sealed class ImageRecompressor : IPageProcessor
	{
		static readonly PdfName[] __IgnoreFilters = new PdfName[] { PdfName.JBIG2DECODE };
		static readonly ImageExtracterOptions _imgExpOption = new ImageExtracterOptions() {
			OutputPath = System.IO.Path.GetTempPath(),
			MergeImages = false
		};

		int _processedImageCount;
		int _optimizedImageCount;

		public bool ConvertToBinary { get; set; }

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
			return IterateObjects(context.Page);
		}

		bool IterateObjects(PdfDictionary container) {
			var items = container.Locate<PdfDictionary>(PdfName.RESOURCES, PdfName.XOBJECT);
			if (items == null) {
				return false;
			}
			foreach (var item in items) {
				if (item.Value is PdfIndirectReference pdfRef
					&& PdfReader.GetPdfObject(pdfRef) is PRStream im) {
					var subType = im.GetAsName(PdfName.SUBTYPE);
					if (PdfName.IMAGE.Equals(subType)) {
						CompressImage(pdfRef, im);
					}
					else if (PdfName.FORM.Equals(subType)) {
						IterateObjects(im);
					}
				}
			}
			return true;
		}

		void CompressImage(PdfIndirectReference pdfRef, PRStream img) {
			_processedImageCount++;
			var l = img.GetAsNumber(PdfName.LENGTH);
			if (l == null || l.IntValue < 400 /*忽略小图片*/) {
				return;
			}
			var f = img.Get(PdfName.FILTER);
			PdfName fn = null;
			if (f.Type == PdfObject.ARRAY) {
				var fl = f as PdfArray;
				fn = fl.GetAsName(fl.Size - 1);
			}
			else if (f.Type == PdfObject.NAME) {
				fn = f as PdfName;
			}
			if (fn == null || !__IgnoreFilters.Contains(fn)) {
				if (OptimizeBinaryImage(pdfRef, img, l.IntValue, ConvertToBinary)
					/*|| ReplaceJ2kImage(pdfRef, im, fn)*/) {
					_optimizedImageCount++;
				}
			}
		}

		static bool OptimizeBinaryImage(PdfIndirectReference imgRef, PRStream imgStream, int length, bool convertToBinary) {
			var oneBitPerComponent = imgStream.GetAsNumber(PdfName.BITSPERCOMPONENT)?.IntValue == 1;
			var isMask = imgStream.GetAsBoolean(PdfName.IMAGEMASK)?.BooleanValue == true;
			if (oneBitPerComponent == false && isMask == false && convertToBinary == false) {
				return false;
			}

			var info = new ImageInfo(imgRef);
			var bytes = info.DecodeImage(_imgExpOption);
			using (var fi = ImageExtractor.CreateFreeImageBitmap(info, ref bytes, false, info.ICCProfile != null)) {
				if (convertToBinary
					&& (fi.HasPalette == false
					|| fi.UniqueColors > 256
					|| fi.ConvertColorDepth(FreeImageAPI.FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_THRESHOLD) == false)) {
					return false;
				}
				var sb = JBig2Encoder.Encode(fi);
				if (sb.Length > length) {
					return false;
				}
				imgStream.SetData(sb, false);
				imgStream.Put(PdfName.FILTER, PdfName.JBIG2DECODE);
				imgStream.Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
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
			}
			return true;
		}

		static bool ReplaceJ2kImage(PdfIndirectReference imgRef, PRStream imgStream, PdfName filter) {
			if (PdfName.JPXDECODE.Equals(filter) == false) {
				return false;
			}

			var info = new ImageInfo(imgRef);
			byte[] jpg;
			using (var ms = new System.IO.MemoryStream(info.DecodeImage(_imgExpOption)))
			using (var js = new System.IO.MemoryStream())
			using (var fi = new FreeImageAPI.FreeImageBitmap(ms)) {
				fi.Save(js, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG, FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE | FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL | FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_PROGRESSIVE);
				jpg = js.ToArray();
			}
			imgStream.SetData(jpg, false);
			imgStream.Put(PdfName.FILTER, PdfName.DCTDECODE);
			imgStream.Put(PdfName.LENGTH, new PdfNumber(jpg.Length));
			return true;
		}

		#endregion
	}
}
