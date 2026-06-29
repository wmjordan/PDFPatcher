using System.Windows.Forms;
using FreeImageAPI;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;
using PDFPatcher.Processor;
using PDFPatcher.Processor.Imaging;

namespace PDFPatcher.Functions;

sealed partial class ImageViewerForm : Form
{
	public ImageViewerForm() {
		InitializeComponent();
	}
	internal ImageViewerForm(Pixmap pixmap) : this() {
		_ImageBox.Image = pixmap.ToBitmap();
	}
	internal ImageViewerForm(ImageInfo image, byte[] bytes) : this() {
		this.SetIcon(Properties.Resources.ViewContent);
		if (image.ExtName == Constants.FileExtensions.Png || image.ExtName == Constants.FileExtensions.Tif) {
			using FreeImageBitmap bmp = ImageExtractor.CreateFreeImageBitmap(image, ref bytes, false, true);
			_ImageBox.Image = bmp.ToBitmap();
		}
		else {
			FIBITMAP b = default;
			try {
				var isCmyk = iTextSharp.text.pdf.PdfName.DEVICECMYK.Equals(image.ColorSpace);
				using var ms = new System.IO.MemoryStream(bytes);
				b = FreeImage.LoadFromStream(ms, isCmyk ? FREE_IMAGE_LOAD_FLAGS.TIFF_CMYK | FREE_IMAGE_LOAD_FLAGS.JPEG_CMYK : FREE_IMAGE_LOAD_FLAGS.DEFAULT);
				if (isCmyk) {
					b = ConvertCmykToRgb(b, image.InvertCmyk);
				}
				_ImageBox.Image = FreeImage.GetBitmap(b);
			}
			catch (System.Exception ex) {
				this.ErrorBox("无法加载图片", ex);
			}
			finally {
				if (!b.IsNull) {
					FreeImage.Unload(b);
				}
			}
		}
	}

	protected override void OnClosed(System.EventArgs e) {
		_ImageBox.Image.TryDispose();
		base.OnClosed(e);
	}

	void _MainToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		var n = e.ClickedItem.Name;
		switch (n) {
			case "_Save":
				using (var f = new SaveFileDialog {
					Title = "保存图片文件",
					DefaultExt = Constants.FileExtensions.Png,
					FileName = "导出图片.png",
					Filter = Constants.FileExtensions.ImageFilter
				}) {
					if (f.ShowDialog() == DialogResult.OK) {
						try {
							using var fi = new FreeImageAPI.FreeImageBitmap(_ImageBox.Image);
							fi.Save(f.FileName);
						}
						catch (System.Exception ex) {
							FormHelper.ErrorBox(ex.Message);
						}
					}
				}
				break;
			case "_ZoomReset":
				_ImageBox.ActualSize(); break;
			case "_FitWindow":
				_ImageBox.ZoomToFit(); break;
			default:
				break;
		}
	}

	static FIBITMAP ConvertCmykToRgb(FIBITMAP cmykDib, bool invert) {
		int width = (int)FreeImage.GetWidth(cmykDib);
		int height = (int)FreeImage.GetHeight(cmykDib);

		FIBITMAP rgbDib = FreeImage.Allocate(width, height, 24, 0, 0, 0);

		FIBITMAP cChannel = FreeImage.GetChannel(cmykDib, FREE_IMAGE_COLOR_CHANNEL.FICC_BLUE);
		FIBITMAP mChannel = FreeImage.GetChannel(cmykDib, FREE_IMAGE_COLOR_CHANNEL.FICC_GREEN);
		FIBITMAP yChannel = FreeImage.GetChannel(cmykDib, FREE_IMAGE_COLOR_CHANNEL.FICC_RED);
		FIBITMAP kChannel = FreeImage.GetChannel(cmykDib, FREE_IMAGE_COLOR_CHANNEL.FICC_ALPHA);

		unsafe {
			var cPtr = (byte*)FreeImage.GetBits(cChannel);
			var mPtr = (byte*)FreeImage.GetBits(mChannel);
			var yPtr = (byte*)FreeImage.GetBits(yChannel);
			var kPtr = (byte*)FreeImage.GetBits(kChannel);
			var rgbPtr = (byte*)FreeImage.GetBits(rgbDib);

			var pitch = (int)FreeImage.GetPitch(rgbDib);

			for (int y = 0; y < height; y++) {
				int rowOffset = y * pitch;
				int srcBaseIdx = y * width; // 假设通道也是连续的，无行填充（通常 FreeImage 通道如此）

				for (int x = 0; x < width; x++) {
					int srcIdx = srcBaseIdx + x;
					int dstIdx = rowOffset + x * 3;

					// 读取原始值
					int c = cPtr[srcIdx];
					int m = mPtr[srcIdx];
					int yVal = yPtr[srcIdx];
					int k = kPtr[srcIdx];

					// 如果需要取反（之前的逻辑）
					if (invert) {
						c = 255 - c;
						m = 255 - m;
						yVal = 255 - yVal;
						k = 255 - k;
					}

					// 计算红色: R = (255 - C) * (255 - K) / 255
					int r_val = ((255 - c) * (255 - k)) / 255;

					// 计算绿色: G = (255 - M) * (255 - K) / 255
					int g_val = ((255 - m) * (255 - k)) / 255;

					// 计算蓝色: B = (255 - Y) * (255 - K) / 255
					int b_val = ((255 - yVal) * (255 - k)) / 255;

					// 写入 RGB (BGR 顺序)
					rgbPtr[dstIdx + 0] = (byte)b_val;
					rgbPtr[dstIdx + 1] = (byte)g_val;
					rgbPtr[dstIdx + 2] = (byte)r_val;
				}
			}
		}

		FreeImage.Unload(cChannel);
		FreeImage.Unload(mChannel);
		FreeImage.Unload(yChannel);
		FreeImage.Unload(kChannel);
		FreeImage.Unload(cmykDib);

		return rgbDib;
	}
}
