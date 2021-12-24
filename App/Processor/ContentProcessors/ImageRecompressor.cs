using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Processor.Imaging;
using PDFPatcher.Model;
using System.Linq;

namespace PDFPatcher.Processor
{
	sealed class ImageRecompressor : IPageProcessor
	{
		static readonly PdfName[] __IgnoreFilters = new PdfName[] { PdfName.DCTDECODE, PdfName.JBIG2DECODE };
		static readonly ImageExtracterOptions _imgExpOption = new ImageExtracterOptions ()
		{
			OutputPath = System.IO.Path.GetTempPath (),
			MergeImages = false
		};

		int _processedImageCount;
		int _optimizedImageCount;

		#region IPageProcessor 成员
		public string Name { get { return "优化压缩黑白图片"; } }

		public void BeginProcess (DocProcessorContext context) {
			_processedImageCount = 0;
			_optimizedImageCount = 0;
		}
		public bool EndProcess (PdfReader pdf) {
			Tracker.TraceMessage (Tracker.Category.Notice, this.Name + "功能：");
			Tracker.TraceMessage ("　　处理了 " + _processedImageCount + " 幅图片。");
			Tracker.TraceMessage ("　　优化了 " + _optimizedImageCount + " 幅图片的压缩率。");
			return false;
		}

		public int EstimateWorkload (PdfReader pdf) {
			return pdf.NumberOfPages * 10;
		}

		public bool Process (PageProcessorContext context) {
			Tracker.IncrementProgress (10);
			var _imgExp = new ImageExtractor (_imgExpOption, context.Pdf);
			var images = context.Page.Locate<PdfDictionary> (PdfName.RESOURCES, PdfName.XOBJECT);
			if (images == null) {
				return false;
			}
			foreach (var item in images) {
				var im = PdfReader.GetPdfObject (item.Value) as PRStream;
				if (im == null
					|| PdfName.IMAGE.Equals (im.GetAsName (PdfName.SUBTYPE)) == false) {
					continue;
				}
				_processedImageCount++;
				var l = im.GetAsNumber (PdfName.LENGTH);
				if (l == null || l.IntValue < 400 /*忽略小图片*/) {
					continue;
				}
				var f = im.Get (PdfName.FILTER);
				PdfName fn = null;
				if (f.Type == PdfObject.ARRAY) {
					var fl = f as PdfArray;
					fn = fl.GetAsName (fl.Size - 1);
				}
				else if (f.Type == PdfObject.NAME) {
					fn = f as PdfName;
				}
				if (fn != null && __IgnoreFilters.Contains (fn)) {
					continue;
				}

				if (OptimizeBinaryImage (item.Value as PdfIndirectReference, im, l.IntValue)
					|| ReplaceJ2kImage (item.Value as PdfIndirectReference, im, fn)) {
				}
			}
			return true;
		}

		private bool OptimizeBinaryImage (PdfIndirectReference imgRef, PRStream imgStream, int length) {
			var bpc = imgStream.GetAsNumber (PdfName.BITSPERCOMPONENT);
			var mask = imgStream.GetAsBoolean (PdfName.IMAGEMASK);
			if (bpc == null && (mask == null || mask.BooleanValue == false)
				|| bpc != null && bpc.IntValue != 1) {
				return false;
			}

			var info = new ImageInfo (imgRef);
			var bytes = info.DecodeImage (_imgExpOption);
			using (var fi = ImageExtractor.CreateFreeImageBitmap (info, ref bytes, false, false)) {
				var sb = JBig2Encoder.Encode (fi);
				if (sb.Length > length) {
					return false;
				}
				imgStream.SetData (sb, false);
				imgStream.Put (PdfName.FILTER, PdfName.JBIG2DECODE);
				if (imgStream.GetAsArray (PdfName.COLORSPACE) == null) {
					imgStream.Put (PdfName.COLORSPACE, PdfName.DEVICEGRAY);
				}
				imgStream.Put (PdfName.BITSPERCOMPONENT, new PdfNumber (1));
				imgStream.Put (PdfName.LENGTH, new PdfNumber (sb.Length));
				imgStream.Remove (PdfName.K);
				imgStream.Remove (PdfName.ENDOFLINE);
				imgStream.Remove (PdfName.ENCODEDBYTEALIGN);
				imgStream.Remove (PdfName.COLUMNS);
				imgStream.Remove (PdfName.ROWS);
				imgStream.Remove (PdfName.ENDOFBLOCK);
				imgStream.Remove (PdfName.BLACKIS1);
				imgStream.Remove (PdfName.PREDICTOR);
				imgStream.Remove (PdfName.COLORS);
				imgStream.Remove (PdfName.COLUMNS);
				imgStream.Remove (PdfName.EARLYCHANGE);
				imgStream.Remove (PdfName.DECODEPARMS);
				imgStream.Remove (PdfName.DECODE);
				_optimizedImageCount++;
			}
			return true;
		}

		private bool ReplaceJ2kImage (PdfIndirectReference imgRef, PRStream imgStream, PdfName filter) {
			if (PdfName.JPXDECODE.Equals (filter) == false) {
				return false;
			}

			var info = new ImageInfo (imgRef);
			byte[] jpg;
			using (var ms = new System.IO.MemoryStream (info.DecodeImage (_imgExpOption)))
			using (var js = new System.IO.MemoryStream ())
			using (var fi = new FreeImageAPI.FreeImageBitmap (ms)) {
				fi.Save (js, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG, FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_BASELINE | FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_QUALITYNORMAL | FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.JPEG_PROGRESSIVE);
				jpg = js.ToArray ();
			}
			imgStream.SetData (jpg, false);
			imgStream.Put (PdfName.FILTER, PdfName.DCTDECODE);
			imgStream.Put (PdfName.LENGTH, new PdfNumber (jpg.Length));
			_optimizedImageCount++;
			return true;
		}

		#endregion
	}
}
