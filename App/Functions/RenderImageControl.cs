using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MuPdfSharp;
using PDFPatcher.Common;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public partial class RenderImageControl : FunctionControl, IResettableControl
{
	public override string FunctionName => "转换页面为图片";

	public override System.Drawing.Bitmap IconImage => Properties.Resources.RenderDocument;

	public RenderImageControl() {
		InitializeComponent();
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.RenderImage);
		_SourceFileControl.BrowseSelectedFiles += (object sender, EventArgs e) => {
			if (_AutoOutputDirBox.Checked == false) {
				return;
			}

			string sourceFile = _SourceFileControl.FirstFile;
			if (sourceFile.Length > 0) {
				_TargetBox.Text = FileHelper.CombinePath(Path.GetDirectoryName(sourceFile),
					Path.GetFileNameWithoutExtension(sourceFile));
			}
		};
		_AutoOutputDirBox.CheckedChanged += (object sender, EventArgs e) => {
			AppContext.ImageRenderer.AutoOutputFolder = _AutoOutputDirBox.Checked;
		};
		_ResolutionBox.TextChanged += (s, args) => {
			float v = _ResolutionBox.Text.ToSingle();
			if (v <= 0) {
				_ResolutionBox.Text = "72";
			}
			else if (v > 3000) {
				_ResolutionBox.Text = "3000";
			}
		};
		_ExtractPageImageWidthBox.GotFocus += (s, args) => { _SpecificWidthBox.Checked = true; };
		_ExtractPageRatioBox.GotFocus += (s, args) => { _SpecificRatioBox.Checked = true; };
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		ShowFileMaskPreview();
		AppContext.MainForm.SetTooltip(_SourceFileControl.FileList, "包含图片的 PDF 文件路径");
		AppContext.MainForm.SetTooltip(_TargetBox, "放置输出图片的文件夹路径");
		AppContext.MainForm.SetTooltip(_ExtractPageRangeBox, "需要提取图片的页码范围，不指定页码范围时提取所有页面的图片");
		AppContext.MainForm.SetTooltip(_FileNameMaskBox,
			"提取的图片文件名按其所在页码数字命名，可在此修改命名规则\n“0000”：不足四位用0补足四位\n“0”：文件名按实际页码，不用0补位\n可用英文双引号将文本括起来（如“\"相约2000\"0”，前面的“2000”不会被解释为占位符）");
		AppContext.MainForm.SetTooltip(_VerticalFlipImageBox, "某些 PDF 文件导出的图片上下颠倒，可用此选项将其还原");
		AppContext.MainForm.SetTooltip(_InvertColorBox, "翻转 PNG 和 TIFF 黑白图片的颜色");
		AppContext.MainForm.SetTooltip(_QuantizeBox, "尽量减少导出图片所用的颜色，从而减小图片占用的磁盘空间");
		AppContext.MainForm.SetTooltip(_SpecificWidthBox, "指定输出图片的宽度（单位为像素，图片的高度将按比例缩放）");
		AppContext.MainForm.SetTooltip(_SpecificRatioBox, "指定输出图片的放大倍数");
		AppContext.MainForm.SetTooltip(_ExtractPageImageWidthBox,
			"指定输出图片的宽度（单位为像素，图片的高度将按比例缩放），宽度为 0 时相当于按 1：1 比例输出");
		Reload();
	}

	public void Reset() {
		AppContext.ImageRenderer = new ImageRendererOptions();
		Reload();
	}

	public void Reload() {
		ImageRendererOptions o = AppContext.ImageRenderer;
		_AutoOutputDirBox.Checked = o.AutoOutputFolder;
		_ColorSpaceRgbBox.Checked = !(_ColorSpaceGrayBox.Checked = o.ColorSpace == ColorSpace.Gray);
		_FileNameMaskBox.Text = o.FileMask;
		_HorizontalFlipImageBox.Checked = o.HorizontalFlipImages;
		_HideAnnotationsBox.Checked = o.HideAnnotations;
		_ImageFormatBox.SelectedIndex = ValueHelper.MapValue(o.FileFormat,
			new ImageFormat[] {ImageFormat.Png, ImageFormat.Jpeg, ImageFormat.Tiff}, new int[] {0, 1, 2}, 0);
		_InvertColorBox.Checked = o.InvertColor;
		if (o.JpegQuality > 0 && o.JpegQuality <= 100) {
			_JpegQualityBox.Text = ValueHelper.ToText(o.JpegQuality);
		}
		else {
			o.JpegQuality = 75;
			_JpegQualityBox.Text = "75";
		}

		_QuantizeBox.Checked = o.Quantize;
		_ResolutionBox.Text = o.Dpi.ToText();
		_RotationBox.SelectedIndex =
			ValueHelper.MapValue(o.Rotation, new int[] {0, 90, 180, 270}, new int[] {0, 1, 2, 3}, 0);
		_SpecificRatioBox.Checked = !o.UseSpecificWidth;
		_SpecificWidthBox.Checked = o.UseSpecificWidth;
		_VerticalFlipImageBox.Checked = o.VerticalFlipImages;
		_ExtractPageImageWidthBox.SetValue(o.ImageWidth);
		_ExtractPageRatioBox.SetValue(o.ScaleRatio);
	}

	private void _BrowseTargetPdfButton_Click(object sender, EventArgs e) {
		string sourceFile = _SourceFileControl.Text;
		if (_TargetBox.Text.Length > 0) {
			_SaveImageBox.SelectedPath = Path.GetDirectoryName(_TargetBox.Text);
		}
		else if (sourceFile.Length > 0) {
			_SaveImageBox.SelectedPath = Path.GetDirectoryName(sourceFile);
		}

		if (_SaveImageBox.ShowDialog() == DialogResult.OK) {
			_TargetBox.Text =
				_SaveImageBox.SelectedPath
				//+ (_SaveImageBox.SelectedPath.EndsWith ("\\") ? String.Empty : "\\")
				//+ Path.GetFileNameWithoutExtension (sourceFile)
				;
		}
	}

	private void _ExtractButton_Click(object sender, EventArgs e) {
		if (File.Exists(_SourceFileControl.FirstFile) == false) {
			FormHelper.ErrorBox(Messages.SourceFileNotFound);
			return;
		}

		if (_TargetBox.Text.IsNullOrWhiteSpace()) {
			_BrowseTargetPdfButton_Click(_BrowseTargetPdfButton, e);
			if (_TargetBox.Text.IsNullOrWhiteSpace()) {
				return;
			}
		}

		AppContext.SourceFiles = _SourceFileControl.Files;
		if (_SourceFileControl.Files.Length == 1) {
			_SourceFileControl.FileList.AddHistoryItem();
			_TargetBox.AddHistoryItem();
		}

		AppContext.MainForm.ResetWorker();
		BackgroundWorker worker = AppContext.MainForm.GetWorker();
		worker.DoWork += (dummy, arg) => {
			object[] a = arg.Argument as object[];
			string[] files = a[0] as string[];
			ImageRendererOptions options = a[1] as ImageRendererOptions;
			options.ExtractImagePath = new FilePath(options.ExtractImagePath).Normalize();
			if (files.Length > 1) {
				string ep = options.ExtractImagePath;
				foreach (string file in files) {
					options.ExtractImagePath = new FilePath(ep).Combine(new FilePath(file).FileNameWithoutExtension)
						.Normalize();
					Processor.Worker.RenderPages(file, options);
					Tracker.IncrementTotalProgress();
					if (AppContext.Abort) {
						return;
					}
				}
			}
			else {
				Processor.Worker.RenderPages(files[0], options);
			}
		};
		worker.RunWorkerCompleted += (dummy, arg) => {
			AppContext.ImageExtracter.OutputPath = _ExtractPageRangeBox.Text;
		};
		ImageRendererOptions option = AppContext.ImageRenderer;
		option.ColorSpace = _ColorSpaceRgbBox.Checked ? ColorSpace.Rgb : ColorSpace.Gray;
		option.ExtractPageRange = _ExtractPageRangeBox.Text;
		option.ExtractImagePath = _TargetBox.Text;
		option.FileMask = _FileNameMaskBox.Text;
		option.HideAnnotations = _HideAnnotationsBox.Checked;
		option.HorizontalFlipImages = _HorizontalFlipImageBox.Checked;
		option.InvertColor = _InvertColorBox.Checked;
		option.FileFormat = ValueHelper.MapValue(_ImageFormatBox.SelectedIndex, new int[] {0, 1, 2},
			new ImageFormat[] {ImageFormat.Png, ImageFormat.Jpeg, ImageFormat.Tiff}, ImageFormat.Png);
		option.ImageWidth = (int)_ExtractPageImageWidthBox.Value;
		option.JpegQuality = _JpegQualityBox.Text.TryParse(out int j)
			? j > 0 && j <= 100 ? j : 75
			: 75;
		option.Quantize = _QuantizeBox.Checked;
		option.Dpi = _ResolutionBox.Text.ToSingle();
		option.Rotation = _RotationBox.SelectedIndex * 90;
		option.ScaleRatio = (float)_ExtractPageRatioBox.Value;
		option.UseSpecificWidth = _SpecificWidthBox.Checked;
		option.VerticalFlipImages = _VerticalFlipImageBox.Checked;
		worker.RunWorkerAsync(
			new object[] {AppContext.SourceFiles, option});
		option = null;
	}

	#region IDefaultButtonControl 成员

	public override Button DefaultButton => _ExtractButton;

	#endregion

	private void _GoToImportImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
		AppContext.MainForm.SelectFunctionList(Function.Patcher);
	}

	private void _FileNameMaskBox_TextChanged(object sender, EventArgs e) {
		ShowFileMaskPreview();
	}

	private void ShowFileMaskPreview() {
		try {
			string[] previews = new string[7];
			string f = _FileNameMaskBox.Text;
			previews[0] = 1.ToString(f) + ".jpg";
			previews[1] = 2.ToString(f) + ".jpg";
			previews[2] = 3.ToString(f) + ".jpg ...";
			previews[3] = "\n" + 11.ToString(f) + ".jpg";
			previews[4] = 12.ToString(f) + ".jpg";
			previews[5] = 13.ToString(f) + ".jpg ...";
			previews[6] = 100.ToString(f) + ".jpg";
			_FileMaskPreviewBox.Text = string.Join(" ", previews);
		}
		catch (Exception) {
			_FileMaskPreviewBox.Text = "文件名掩码无效。";
		}
	}

	private void Control_Show(object sender, EventArgs e) {
		if (Visible && AppContext.MainForm != null) {
			_TargetBox.Contents = AppContext.Recent.Folders;
		}
		//else if (this.Visible == false) {
		//    this._TargetBox.DataSource = null;
		//}
	}
}