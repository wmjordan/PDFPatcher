using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Processor
{
	class ImageDeskewProcessor : IPageProcessor
	{
		static readonly ExportImageOptions _imgExpOption = new ExportImageOptions () {
			ExportImagePath = System.IO.Path.GetTempPath (),
			MergeImages = false
		};
		int _processedImageCount;
		int _deskewedImageCount;

		#region IPageProcessor 成员
		public string Name { get { return "校正倾斜图片"; } }

		public void BeginProcess (PdfReader pdf) {
			_processedImageCount = 0;
			_deskewedImageCount = 0;
		}
		public bool EndProcess (PdfReader pdf) {
			Tracker.TraceMessage (Tracker.Category.Notice, this.Name + "功能：");
			Tracker.TraceMessage ("　　处理了 " + _processedImageCount + " 幅图片。");
			Tracker.TraceMessage ("　　校正了 " + _deskewedImageCount + " 幅图片的角度。");
			return false;
		}

		public int EstimateWorkload (PdfReader pdf) {
			return pdf.NumberOfPages * 10;
		}

		public bool Process (Model.PageProcessorContext context) {
			Tracker.IncrementProgress (10);
			Processor.PdfImageExporter ie = new PdfImageExporter (_imgExpOption, context.Pdf);
			var images = PdfHelper.Locate<PdfDictionary> (context.Page, true, PdfName.RESOURCES, PdfName.XOBJECT);
			if (images == null) {
				return false;
			}
			Dictionary<PdfName, double> angles = new Dictionary<PdfName, double> ();
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
				var inf = new ImageInfo (item.Value as PdfIndirectReference);
				var b = inf.DecodeImage (_imgExpOption);
				using (var fi = PdfImageExporter.CreateFreeImageBitmap (inf, ref b, false, false)) {
					//if (fi.Height > 500 && fi.Width > 500) {
					//    var zy = (float)fi.Height / 500f;
					//    var zx = (float)fi.Width / 500f;
					//    if (zy > zx) {
					//        fi.Rescale (500, (int)((float)fi.Height / zx), FreeImageAPI.FREE_IMAGE_FILTER.FILTER_BILINEAR);
					//    }
					//    else {
					//        fi.Rescale ((int)((float)fi.Width / zy), 500, FreeImageAPI.FREE_IMAGE_FILTER.FILTER_BILINEAR);
					//    }
					//}
					angles.Add (item.Key, new ImageDeskew ().GetSkewAngle (fi)); // 获取图片倾斜角度
				}
			}
			if (angles.Count == 0) {
				return false;
			}
			var p = new PdfPageCommandProcessor (context);
			var r = false;
			foreach (var item in p.Commands) {
				r |= ProcessSubCommand (item, angles);
			}
			if (r == true) {
				p.WritePdfCommands (context);
				_deskewedImageCount++;
			}
			return r;
		}

		#endregion

		private bool ProcessSubCommand (Model.PdfPageCommand item, Dictionary<PdfName, double> angles) {
			if (item.Type != Model.PdfPageCommandType.Enclosure) {
				return false;
			}
			var ec = (item as Model.EnclosingCommand);
			if (ec.Name.ToString () == "q") {
				if (ec.SubCommands.Count != 2
					|| ec.SubCommands[0].Name.ToString () != "cm"
					|| ec.SubCommands[1].Name.ToString () != "Do") {
					return false;
				}
				var m = ec.SubCommands[0] as Model.MatrixCommand;
				var d = ec.SubCommands[1];
				if (d.HasOperand == false) {
					return false;
				}
				double a;
				if (angles.TryGetValue (d.Operands[0] as PdfName, out a) == false || a < 0.0001 && a > -0.0001) {
					return false;
				}
				Tracker.TraceMessage (d.Operands[0].ToString () + " rotate " + a.ToString ());
				a *= Math.PI / 180f;
				ec.SubCommands.Insert (0, new Model.MatrixCommand (new PdfLiteral ("cm"), new List<PdfObject> () {
					new PdfNumber(Math.Cos (a)),
					new PdfNumber(Math.Sin (a)),
					new PdfNumber(-Math.Sin (a)),
					new PdfNumber(Math.Cos (a)),
					new PdfNumber (0), new PdfNumber(0)
				}));
				return true;
			}
			var r = false;
			foreach (var sub in ec.SubCommands) {
				r |= ProcessSubCommand (sub, angles);
			}
			return r;
		}
	}
}
