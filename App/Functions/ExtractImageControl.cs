using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Processor;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public partial class ExtractImageControl : FunctionControl, IResettableControl
{
	public ExtractImageControl() {
		InitializeComponent();
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.ExtractImage);
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
			AppContext.ImageExtracter.AutoOutputFolder = _AutoOutputDirBox.Checked;
		};
	}

	public override string FunctionName => "提取图片";

	public override Bitmap IconImage => Resources.ExtractImage;

	#region IDefaultButtonControl 成员

	public override Button DefaultButton => _ExtractButton;

	#endregion

	public void Reset() {
		AppContext.ImageExtracter = new ImageExtracterOptions();
		Reload();
	}

	public void Reload() {
		ImageExtracterOptions o = AppContext.ImageExtracter;
		_AutoOutputDirBox.Checked = o.AutoOutputFolder;
		_FileNameMaskBox.Text = o.FileMask;
		_InvertBlackAndWhiteBox.Checked = o.InvertBlackAndWhiteImages;
		_MonoTiffBox.Checked = !o.MonoPng;
		_MergeImageBox.Checked = o.MergeImages;
		_MergeJpgToPngBox.Checked = o.MergeJpgToPng;
		_ExportAnnotImagesBox.Checked = o.ExtractAnnotationImages;
		_MinHeightBox.SetValue(o.MinHeight);
		_MinWidthBox.SetValue(o.MinWidth);
		_VerticalFlipImageBox.Checked = o.VerticalFlipImages;
		_ExportSoftMaskBox.Checked = o.ExtractSoftMask;
		_InvertSoftMaskBox.Checked = o.InvertSoftMask;
		_MonoPngBox.Checked = o.MonoPng;
		_SkipRedundantImagesBox.Checked = o.SkipRedundantImages;
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		ShowFileMaskPreview();
		AppContext.MainForm.SetTooltip(_SourceFileControl.FileList, "包含图片的 PDF 文件路径");
		AppContext.MainForm.SetTooltip(_TargetBox, "放置输出图片的文件夹路径");
		AppContext.MainForm.SetTooltip(_ExtractPageRangeBox, "在此指定需提取图片的页码范围，不指定页码范围时提取所有页面的图片");
		AppContext.MainForm.SetTooltip(_FileNameMaskBox,
			"提取的图片文件名按其所在页码数字命名，可在此修改命名规则\n“0000”：不足四位用0补足四位\n“0”：文件名按实际页码，不用0补位\n可用英文双引号将文本括起来（如“\"相约2000\"0”，前面的“2000”不会被解释为占位符）");
		AppContext.MainForm.SetTooltip(_MergeImageBox,
			"尝试将相同页面下的图片合并为同一个文件\n①合并图片的格式必须相同\n②宽度必须相同\n③仅限 PNG 和 TIFF 格式");
		AppContext.MainForm.SetTooltip(_VerticalFlipImageBox, "某些 PDF 文件导出的图片上下颠倒，可用此选项将其还原");
		AppContext.MainForm.SetTooltip(_InvertBlackAndWhiteBox, "翻转 PNG 和 TIFF 黑白图片的颜色");
		AppContext.MainForm.SetTooltip(_MinHeightBox, "忽略高度小于此处指定值的图片");
		AppContext.MainForm.SetTooltip(_MinWidthBox, "忽略宽度小于此处指定值的图片");
		AppContext.MainForm.SetTooltip(_MergeJpgToPngBox, "在合并图片时，将使用有损压缩的 JPEG 图片合并为无损压缩的 PNG 图片");
		AppContext.MainForm.SetTooltip(_ExtractButton, "点击此按钮，将 PDF 文件的图片提取到指定的目录");
		AppContext.MainForm.SetTooltip(_SkipRedundantImagesBox, "避免导出 PDF 内部引用值一致的图片");
		Reload();
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
			ImageExtracterOptions options = a[1] as ImageExtracterOptions;
			options.OutputPath = new FilePath(options.OutputPath).Normalize();
			if (files.Length > 1) {
				string ep = options.OutputPath;
				Tracker.SetTotalProgressGoal(files.Length);
				foreach (string file in files) {
					options.OutputPath = new FilePath(ep).Combine(new FilePath(file).FileNameWithoutExtension)
						.Normalize();
					Worker.ExtractImages(file, options);
					Tracker.IncrementTotalProgress();
					if (AppContext.Abort) {
						return;
					}
				}
			}
			else {
				Worker.ExtractImages(files[0], options);
			}
		};
		worker.RunWorkerCompleted += (dummy, arg) => {
			AppContext.ImageExtracter.OutputPath = _ExtractPageRangeBox.Text;
		};
		ImageExtracterOptions option = AppContext.ImageExtracter;
		option.ExtractAnnotationImages = _ExportAnnotImagesBox.Checked;
		option.PageRange = _ExtractPageRangeBox.Text;
		option.OutputPath = _TargetBox.Text;
		option.FileMask = _FileNameMaskBox.Text;
		option.MergeImages = _MergeImageBox.Checked;
		option.MergeJpgToPng = _MergeJpgToPngBox.Checked;
		option.VerticalFlipImages = _VerticalFlipImageBox.Checked;
		option.InvertBlackAndWhiteImages = _InvertBlackAndWhiteBox.Checked;
		option.MonoPng = _MonoPngBox.Checked;
		option.MinHeight = (int)_MinHeightBox.Value;
		option.MinWidth = (int)_MinWidthBox.Value;
		option.ExtractSoftMask = _ExportSoftMaskBox.Checked;
		option.InvertSoftMask = _InvertSoftMaskBox.Checked;
		option.SkipRedundantImages = _SkipRedundantImagesBox.Checked;
		worker.RunWorkerAsync(
			new object[] { AppContext.SourceFiles, option });
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