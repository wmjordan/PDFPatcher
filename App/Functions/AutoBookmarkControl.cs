using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public partial class AutoBookmarkControl : FunctionControl, IResettableControl, IDocumentSource
	{
		AutoBookmarkOptions _options;
		static AutoBookmarkOptions.LevelAdjustmentOption[] _copiedLevelAdjustments;

		public AutoBookmarkControl() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}

		public override string FunctionName => "自动生成书签";

		public override System.Drawing.Bitmap IconImage => Properties.Resources.AutoBookmark;

		public override Button DefaultButton => _ExportBookmarkButton;

		public string DocumentPath => _SourceFileControl.Text;

		void OnLoad() {
			_FilterToolbar.ScaleIcons(16);
			_LevelAdjustmentBox.ScaleColumnWidths();
			AppContext.MainForm.SetTooltip(_SourceFileControl.FileList, "需要识别标题为书签的 PDF 源文件路径");
			AppContext.MainForm.SetTooltip(_BookmarkControl.FileList, "指定识别书签后生成的信息文件或简易文本书签文件路径");
			AppContext.MainForm.SetTooltip(_ExportBookmarkButton, "点击此按钮识别 PDF 文件的标题为信息文件");
			AppContext.MainForm.SetTooltip(_TitleSizeThresholdBox, "指定标题文本的最小尺寸，小于此尺寸的文本将被忽略");
			AppContext.MainForm.SetTooltip(_AutoHierarchicalArrangementBox, "根据标题文本的尺寸级别生成多层次的书签");
			AppContext.MainForm.SetTooltip(_YOffsetBox, "将标题的定位位置向上偏移的行距");
			AppContext.MainForm.SetTooltip(_MergeAdjacentTitlesBox, "将连续出现的标题合并为一个标题");
			AppContext.MainForm.SetTooltip(_MergeDifferentSizeTitlesBox, "合并不同尺寸的相邻标题");
			AppContext.MainForm.SetTooltip(_GoToPageTopLevelBox, "小于指定层数的标题定位到页首，而非所在精确位置");
			AppContext.MainForm.SetTooltip(_IgnoreOverlappedTextBox, "忽略用于制作粗体、阴影等效果的重叠文本");
			AppContext.MainForm.SetTooltip(_CreateBookmarkForFirstPageBox, "生成一个书签指向文档的第一页，书签文本为 PDF 文件的名称");
			AppContext.MainForm.SetTooltip(_PageRangeBox, Messages.PageRanges);

			int i = 0;
			foreach (var item in EditAdjustmentForm.FilterNames) {
				_AddFilterMenu.Items.Add(item).Name = EditAdjustmentForm.FilterIDs[i++];
			}
			_LevelAdjustmentBox.CellEditUseWholeCell = true;
			_LevelAdjustmentBox.BeforeSorting += (object sender, BrightIdeasSoftware.BeforeSortingEventArgs e) => {
				e.Canceled = true;
			};
			_LevelAdjustmentBox.DropSink = new BrightIdeasSoftware.RearrangingDropSink(false);
			_AdvancedFilterColumn.AspectGetter = (object x) => {
				if (x is not AutoBookmarkOptions.LevelAdjustmentOption f) {
					return null;
				}
				return f.Condition.Description;
			};
			_AdjustmentLevelColumn.AspectGetter = (object x) => {
				var f = x as AutoBookmarkOptions.LevelAdjustmentOption;
				return f?.AdjustmentLevel ?? 0;
			};
			_AdjustmentLevelColumn.AspectPutter = (object x, object value) => {
				if (x is not AutoBookmarkOptions.LevelAdjustmentOption f) {
					return;
				}
				if ((value ?? "0").ToString().TryParse(out float a)) {
					f.AdjustmentLevel = a;
				}
			};
			_RelativeAdjustmentColumn.AspectGetter = (object x) => (x as AutoBookmarkOptions.LevelAdjustmentOption)?.RelativeAdjustment == true;
			_RelativeAdjustmentColumn.AspectPutter = (object x, object value) => {
				if (x is not AutoBookmarkOptions.LevelAdjustmentOption f) {
					return;
				}
				f.RelativeAdjustment = value is bool b && b;
			};
			_FilterBeforeMergeColumn.AspectGetter = (object x) => ((x as AutoBookmarkOptions.LevelAdjustmentOption)?.FilterBeforeMergeTitle) ?? false;
			_FilterBeforeMergeColumn.AspectPutter = (object x, object value) => {
				if (x is AutoBookmarkOptions.LevelAdjustmentOption f) {
					f.FilterBeforeMergeTitle = value is bool b && b;
				}
			};
			_IgnorePatternsBox.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
			Reload();

			var d = _BookmarkControl.FileDialog;
			d.CheckFileExists = false;
			d.CheckPathExists = false;

			if (d is SaveFileDialog sd) {
				sd.OverwritePrompt = false;
			}
		}

		public void Reset() {
			AppContext.AutoBookmarker = new AutoBookmarkOptions();
			Reload();
		}

		public void Reload() {
			_options = AppContext.AutoBookmarker;
			_CreateBookmarkForFirstPageBox.Checked = _options.CreateBookmarkForFirstPage;
			_MergeAdjacentTitlesBox.Checked = _options.MergeAdjacentTitles;
			_MergeDifferentSizeTitlesBox.Checked = _options.MergeDifferentSizeTitles;
			_AutoHierarchicalArrangementBox.Checked = _options.AutoHierarchicalArrangement;
			_IgnoreNumericTitleBox.Checked = _options.IgnoreNumericTitle;
			_IgnoreOverlappedTextBox.Checked = _options.IgnoreOverlappedText;
			_IgnoreSingleCharacterTitleBox.Checked = _options.IgnoreSingleCharacterTitle;
			_ShowAllFontsBox.Checked = _options.DisplayAllFonts;
			_DisplayFontStatisticsBox.Checked = _options.DisplayFontStatistics;
			_WritingDirectionBox.SelectedIndex = (int)_options.WritingDirection;
			_MergeDifferentFontTitlesBox.Checked = _options.MergeDifferentFontTitles;
			_TitleSizeThresholdBox.SetValue(_options.TitleThreshold);
			_YOffsetBox.SetValue(_options.YOffset);
			_MaxDistanceBetweenLinesBox.SetValue(_options.MaxDistanceBetweenLines);
			_FirstLineAsTitleBox.Checked = _options.FirstLineAsTitle;

			for (int i = _options.LevelAdjustment.Count - 1; i >= 0; i--) {
				if (_options.LevelAdjustment[i].Condition == null) {
					_options.LevelAdjustment.RemoveAt(i);
				}
			}
			_LevelAdjustmentBox.SetObjects(_options.LevelAdjustment);
			_IgnorePatternsBox.Rows.Clear();
			foreach (var item in _options.IgnorePatterns) {
				if (String.IsNullOrEmpty(item.Text)) {
					continue;
				}
				_IgnorePatternsBox.Rows.Add(item.Text, item.MatchCase, item.FullMatch, item.UseRegularExpression);
			}
		}

		private void _ExportBookmarkButton_Click(object sender, EventArgs e) {
			if (File.Exists(_SourceFileControl.FirstFile) == false) {
				FormHelper.ErrorBox(Messages.SourceFileNotFound);
				return;
			}
			else if (String.IsNullOrEmpty(_BookmarkControl.Text)) {
				FormHelper.ErrorBox(Messages.InfoDocNotSpecified);
				return;
			}

			AppContext.SourceFiles = _SourceFileControl.Files;
			AppContext.BookmarkFile = _BookmarkControl.Text;
			if (_SourceFileControl.Files.Length == 1) {
				_SourceFileControl.FileList.AddHistoryItem();
				_BookmarkControl.FileList.AddHistoryItem();
			}

			AppContext.MainForm.ResetWorker();
			AppContext.MainForm.GetWorker().DoWork += ExportControl_DoWork;
			SyncOptions();
			AppContext.MainForm.GetWorker().RunWorkerAsync(new object[] {
				AppContext.SourceFiles,
				AppContext.BookmarkFile,
				_options
			});
		}

		private void SyncOptions() {
			_options.CreateBookmarkForFirstPage = _CreateBookmarkForFirstPageBox.Checked;
			_options.PageRanges = _PageRangeBox.Text;
			_options.TitleThreshold = (float)_TitleSizeThresholdBox.Value;
			_options.MergeAdjacentTitles = _MergeAdjacentTitlesBox.Checked;
			_options.IgnoreNumericTitle = _IgnoreNumericTitleBox.Checked;
			_options.IgnoreOverlappedText = _IgnoreOverlappedTextBox.Checked;
			_options.IgnoreSingleCharacterTitle = _IgnoreSingleCharacterTitleBox.Checked;
			_options.MergeDifferentSizeTitles = _MergeDifferentSizeTitlesBox.Checked;
			_options.MergeDifferentFontTitles = _MergeDifferentFontTitlesBox.Checked;
			_options.YOffset = (float)_YOffsetBox.Value;
			_options.ExportTextCoordinates = _ExportTextCoordinateBox.Checked;
			_options.PageTopForLevel = (int)_GoToPageTopLevelBox.Value;
			_options.AutoHierarchicalArrangement = _AutoHierarchicalArrangementBox.Checked;
			_options.DisplayFontStatistics = _DisplayFontStatisticsBox.Checked;
			_options.DisplayAllFonts = _ShowAllFontsBox.Checked;
			_options.WritingDirection = (WritingDirection)_WritingDirectionBox.SelectedIndex;
			_options.MaxDistanceBetweenLines = (float)_MaxDistanceBetweenLinesBox.Value;
			_options.FirstLineAsTitle = _FirstLineAsTitleBox.Checked;
			_options.IgnorePatterns.Clear();
			foreach (DataGridViewRow item in _IgnorePatternsBox.Rows) {
				if (item.IsNewRow) {
					continue;
				}
				var cells = item.Cells;
				if (cells[0].Value == null) {
					continue;
				}
				_options.IgnorePatterns.Add(new PDFPatcher.Model.MatchPattern(
					cells[0].Value.ToString(),
					(bool)(cells[_MatchCaseColumn.Index].Value ?? false),
					(bool)(cells[_FullMatchColumn.Index].Value ?? false),
					(bool)(cells[_PatternTypeColumn.Index].Value ?? false)));
			}
			_options.LevelAdjustment.Clear();
			if (_LevelAdjustmentBox.Items.Count > 0) {
				foreach (ListViewItem item in _LevelAdjustmentBox.Items) {
					_options.LevelAdjustment.Add(_LevelAdjustmentBox.GetModelObject(item.Index) as AutoBookmarkOptions.LevelAdjustmentOption);
				}
			}
		}

		void ExportControl_DoWork(object sender, DoWorkEventArgs e) {
			var a = e.Argument as object[];
			var files = a[0] as string[];
			var b = a[1] as string;
			var options = a[2] as AutoBookmarkOptions;
			if (files.Length > 1) {
				var p = Path.GetDirectoryName(b);
				var ext = Path.GetExtension(b);
				foreach (var file in files) {
					Processor.Worker.CreateBookmark(file, FileHelper.CombinePath(p, Path.GetFileNameWithoutExtension(file) + ext), options);
					if (AppContext.Abort) {
						return;
					}
				}
			}
			else {
				Processor.Worker.CreateBookmark(files[0], b, options);
			}
		}

		private void _IgnorePatternsBox_CellContentClick(object sender, DataGridViewCellEventArgs e) {
			if (e.ColumnIndex == _RemovePatternColumn.Index && e.RowIndex >= 0) {
				_IgnorePatternsBox.Rows.RemoveAt(e.RowIndex);
			}
		}

		private void _ImportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			AppContext.MainForm.SelectFunctionList(Function.Patcher);
		}

		private void ControlEvent(object sender, EventArgs e) {
			if (sender == _DeleteAdjustmentButton && _LevelAdjustmentBox.Items.Count > 0 && FormHelper.YesNoBox("是否删除选中的项？") == DialogResult.Yes) {
				_LevelAdjustmentBox.RemoveObjects(_LevelAdjustmentBox.SelectedObjects);
			}
			else if (sender == _ClearTextFiltersButton && _IgnorePatternsBox.Rows.Count > 0 && FormHelper.YesNoBox("是否清空文本过滤列表？") == DialogResult.Yes) {
				_IgnorePatternsBox.Rows.Clear();
			}
			else if (sender == _CopyFilterButton) {
				var si = _LevelAdjustmentBox.SelectedObjects;
				if (si.Count == 0) {
					return;
				}
				_copiedLevelAdjustments = new AutoBookmarkOptions.LevelAdjustmentOption[si.Count];
				for (int i = 0; i < _copiedLevelAdjustments.Length; i++) {
					var item = si[i] as AutoBookmarkOptions.LevelAdjustmentOption;
					_copiedLevelAdjustments[i] = item;
				}
			}
			else if (sender == _PasteButton) {
				if (_copiedLevelAdjustments.HasContent() == false) {
					return;
				}
				foreach (var item in _copiedLevelAdjustments) {
					_LevelAdjustmentBox.AddObject(item.Clone());
				}
			}
			else if (sender == _AddFilterFromInfoFileButton) {
				if (String.IsNullOrEmpty(_BookmarkControl.Text)) {
					if (_BookmarkControl.FileDialog.ShowDialog() != DialogResult.OK) {
						FormHelper.InfoBox("请先指定信息文件的路径。");
						return;
					}
					_BookmarkControl.Text = _BookmarkControl.FileDialog.FileName;
				}
				var doc = new System.Xml.XmlDocument();
				System.Xml.XmlNode fontInfo;
				try {
					doc.Load(_BookmarkControl.Text);
					fontInfo = doc.SelectSingleNode(Constants.PdfInfo + "/" + Constants.Font.DocumentFont);
				}
				catch (Exception ex) {
					AppContext.MainForm.ErrorBox("无法从信息文件加载字体信息", ex);
					return;
				}
				if (fontInfo == null) {
					FormHelper.ErrorBox("无法从信息文件加载字体信息。");
					return;
				}
				using (var f = new FontFilterForm(fontInfo)) {
					if (f.ShowDialog() == DialogResult.OK && f.FilterConditions != null) {
						foreach (var item in f.FilterConditions) {
							_LevelAdjustmentBox.AddObject(new AutoBookmarkOptions.LevelAdjustmentOption() {
								Condition = item,
								AdjustmentLevel = 0,
								RelativeAdjustment = false
							});
						}
					}
				}
			}
		}

		private void _LevelAdjustmentBox_ItemActivate(object sender, EventArgs e) {
			if (_LevelAdjustmentBox.FocusedItem == null) {
				return;
			}
			var fi = _LevelAdjustmentBox.FocusedItem;
			var i = fi.Index;
			var o = _LevelAdjustmentBox.GetModelObject(i) as AutoBookmarkOptions.LevelAdjustmentOption;
			using (var dialog = new EditAdjustmentForm(o)) {
				if (dialog.ShowDialog() == DialogResult.OK) {
					if (dialog.Filter.Condition != null) {
						_LevelAdjustmentBox.InsertObjects(i, new AutoBookmarkOptions.LevelAdjustmentOption[] { dialog.Filter });
						_LevelAdjustmentBox.SelectedIndex = i;
					}
					_LevelAdjustmentBox.RemoveObject(o);
				}
			}
		}

		private void _AddFilterMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			var c = EditAdjustmentForm.CreateCondition(e.ClickedItem.Name);
			if (c == null) {
				return;
			}

			using (var dialog = new EditAdjustmentForm(new AutoBookmarkOptions.LevelAdjustmentOption { Condition = c })) {
				if (dialog.ShowDialog() == DialogResult.OK && dialog.Filter.Condition != null) {
					_LevelAdjustmentBox.AddObject(dialog.Filter);
				}
			}
		}
	}
}
