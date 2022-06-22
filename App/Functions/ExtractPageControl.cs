using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	sealed partial class ExtractPageControl : FunctionControl, IResettableControl
	{
		public override string FunctionName => "提取页面";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.ExtractPages;

		public ExtractPageControl() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		void OnLoad() {
			AppContext.MainForm.SetTooltip(_SourceFileControl.FileList, "需要提取页面的 PDF 文件路径，可选择多个文件");
			AppContext.MainForm.SetTooltip(_ExtractPageRangeBox, "提取页面的页码范围，不指定页码范围时提取源文件的所有页");
			AppContext.MainForm.SetTooltip(_TargetFileControl.FileList, "输出 PDF 文件的路径，右键点击插入文件名替代符");
			AppContext.MainForm.SetTooltip(_ExtractButton, "点击此按钮，提取源 PDF 文件指定范围的页面，生成新的文件");
			AppContext.MainForm.SetTooltip(_SeparatingModeBox, "选择拆分源 PDF 文档的方式");
			AppContext.MainForm.SetTooltip(_SeperateByPageNumberBox, "将源 PDF 文档按页数拆分");
			AppContext.MainForm.SetTooltip(_NumberFileNamesBox, "按书签拆分：在拆分所得的文件名前面添加“1 - ”、“2 - ”等顺序编号；其它拆分：第 1 个文件名也添加编号");
			AppContext.MainForm.SetTooltip(_ExcludePageRangeBox, "不提取此范围内的页面");
			AppContext.MainForm.SetTooltip(_EnableFullCompression, "去除文档中未被使用的对象，尽可能压缩输出文档");

			_TargetFileControl.FileMacroMenu.LoadStandardInfoMacros();
			_TargetFileControl.FileMacroMenu.LoadStandardSourceFileMacros();
			_SeparatingModeBox.SelectedIndexChanged += (s, args) => {
				_NumberFileNamesBox.Text = _SeparatingModeBox.SelectedIndex == 1 ? "在文件名前面添加编号" : "第一个文件名也添加编号";
				_SeperateByPageNumberBox.Enabled = _SeparatingModeBox.SelectedIndex == 2;
			};
			((IResettableControl)this).Reload();
		}

		void _ExtractButton_Click(object sender, EventArgs e) {
			if (File.Exists(_SourceFileControl.FirstFile) == false) {
				FormHelper.ErrorBox(Messages.SourceFileNotFound);
				return;
			}
			if (_TargetFileControl.Text.IsNullOrWhiteSpace()) {
				FormHelper.ErrorBox(Messages.TargetFileNotSpecified);
				return;
			}
			AppContext.SourceFiles = _SourceFileControl.Files;
			if (AppContext.SourceFiles.Length == 1) {
				_SourceFileControl.FileList.AddHistoryItem();
				_TargetFileControl.FileList.AddHistoryItem();
			}
			var o = AppContext.ExtractPage;
			o.EnableFullCompression = _EnableFullCompression.Checked;
			o.KeepBookmarks = _KeepBookmarkBox.Checked;
			o.KeepDocumentProperties = _KeepDocInfoPropertyBox.Checked;
			o.RemoveOrphanBookmarks = _RemoveOrphanBoomarksBox.Checked;
			o.PageRanges = _ExtractPageRangeBox.Text;
			o.RemoveDocumentRestrictions = _RemoveRestrictionBox.Checked;
			o.ExcludePageRanges = _ExcludePageRangeBox.Text;
			o.SeparatingMode = _SeparatingModeBox.SelectedIndex;
			o.SeparateByPage = (int)_SeperateByPageNumberBox.Value;
			o.NumberFileNames = _NumberFileNamesBox.Checked;

			AppContext.MainForm.ResetWorker();
			var worker = AppContext.MainForm.GetWorker();
			worker.DoWork += (dummy, arg) => {
				var a = arg.Argument as object[];
				var files = a[0] as string[];
				var t = a[1] as string;
				var options = a[2] as ExtractPageOptions;
				if (files.Length > 1) {
					var m = FileHelper.HasFileNameMacro(t); // 包含替换符
					var p = m ? null : Path.GetDirectoryName(t);
					Tracker.SetTotalProgressGoal(files.Length);
					foreach (var file in files) {
						Processor.Worker.ExtractPages(options,
							file,
							m ? t : FileHelper.CombinePath(p, Path.GetFileNameWithoutExtension(file) + Constants.FileExtensions.Pdf));
						Tracker.IncrementTotalProgress();
						if (AppContext.Abort) {
							return;
						}
					}
				}
				else {
					Processor.Worker.ExtractPages(options, files[0], t);
				}
			};
			worker.RunWorkerAsync(new object[] {
				AppContext.SourceFiles,
				_TargetFileControl.Text,
				AppContext.ExtractPage
			});
		}

		void _ExtractPageRangeBox_TextChanged(object sender, EventArgs e) {
			AppContext.Exporter.ExtractPageRange = _ExtractPageRangeBox.Text;
		}

		#region IDefaultButtonControl 成员

		public override Button DefaultButton => _ExtractButton;

		#endregion


		#region IResettableControl 成员

		void IResettableControl.Reset() {
			AppContext.ExtractPage = new ExtractPageOptions();
			((IResettableControl)this).Reload();
		}

		void IResettableControl.Reload() {
			var options = AppContext.ExtractPage;
			_EnableFullCompression.Checked = options.EnableFullCompression;
			_KeepBookmarkBox.Checked = options.KeepBookmarks;
			_KeepDocInfoPropertyBox.Checked = options.KeepDocumentProperties;
			_RemoveRestrictionBox.Checked = options.RemoveDocumentRestrictions;
			_RemoveOrphanBoomarksBox.Checked = options.RemoveOrphanBookmarks;
			_SeparatingModeBox.Select(options.SeparatingMode);
			_NumberFileNamesBox.Checked = options.NumberFileNames;
			_SeperateByPageNumberBox.SetValue(options.SeparateByPage);
		}

		#endregion


	}
}
