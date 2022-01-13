using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Functions;

namespace PDFPatcher;

public partial class MainForm : Form
{
	private static readonly Dictionary<Function, FunctionControl> __FunctionControls = new();

	private ReportControl _LogControl;

	#region 公共功能

	private BackgroundWorker _Worker;
	private readonly FormState _formState = new();
	private bool _FullScreen;

	///<summary>获取或指定全屏显示的值。</summary>
	public bool FullScreen {
		get => _FullScreen;
		set {
			if (value == _FullScreen) {
				return;
			}

			_FullScreen = value;
			if (value) {
				_MainMenu.Visible = _GeneralToolbar.Visible = false;
				_formState.Maximize(this);
			}
			else {
				_MainMenu.Visible = true;
				_GeneralToolbar.Visible = AppContext.Toolbar.ShowGeneralToolbar;
				_formState.Restore(this);
			}
		}
	}

	/// <summary>
	/// 设置控件的提示信息。
	/// </summary>
	internal void SetTooltip(Control control, string text) {
		_ToolTip.SetToolTip(control, text);
	}

	/// <summary>
	/// 获取或设置状态栏文本。
	/// </summary>
	internal string StatusText {
		get => _MainStatusLabel.Text;
		set => _MainStatusLabel.Text = value;
	}

	#region Worker

	///<summary>获取或指定后台进程。</summary>
	internal BackgroundWorker GetWorker() {
		if (_Worker == null) {
			_Worker = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
			_Worker.DoWork += Worker_DoWork;
			_Worker.ProgressChanged += Worker_ProgressChanged;
			_Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			Tracker.SetWorker(_Worker);
		}

		return _Worker;
	}

	internal bool IsWorkerBusy => _Worker?.IsBusy == true;

	private void Worker_DoWork(object sender, DoWorkEventArgs e) {
		_Worker.ReportProgress(0);
		FileHelper.ResetOverwriteMode();
		AppContext.Abort = false;
	}

	private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
		_MainMenu.Enabled = _GeneralToolbar.Enabled = _FunctionContainer.Enabled = true;
		foreach (TabPage item in _FunctionContainer.TabPages) {
			item.Enabled = true;
		}

		if (e.Error == null || e.Cancelled == false) {
			_LogControl.Complete();
		}
	}

	public void ResetWorker() {
		if (_Worker != null) {
			if (_Worker.IsBusy) {
				throw new InvalidOperationException("Worker is busy. Can't be reset.");
			}

			_Worker.Dispose();
			_Worker = null;
		}
	}

	private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
		string s = e.UserState as string;
		if (s == "INC") {
			_LogControl.IncrementProgress(e.ProgressPercentage);
			return;
		}
		else if (s == "GOAL") {
			_LogControl.SetGoal(e.ProgressPercentage);
			return;
		}
		else if (s == "TINC") {
			_LogControl.IncrementTotalProgress();
		}
		else if (s == "TGOAL") {
			_LogControl.SetTotalGoal(e.ProgressPercentage);
		}

		if (e.ProgressPercentage > 0) {
			_LogControl.SetProgress(e.ProgressPercentage);
		}
		else if (e.ProgressPercentage == 0) {
			_MainMenu.Enabled = _GeneralToolbar.Enabled = _FunctionContainer.Enabled = false;
			foreach (TabPage item in _FunctionContainer.TabPages) {
				item.Enabled = false;
			}

			_LogControl.Reset();
			ShowLogControl();
		}
		else if (s != null) {
			_LogControl.PrintMessage(s, (Tracker.Category)e.ProgressPercentage);
		}
	}

	#endregion

	internal FunctionControl GetFunctionControl(Function functionName) {
		if (__FunctionControls.TryGetValue(functionName, out FunctionControl f) && f.IsDisposed == false) {
			return f;
		}

		switch (functionName) {
			case Function.FrontPage:
				__FunctionControls[functionName] = new FrontPageControl();
				break;
			case Function.Patcher:
				__FunctionControls[functionName] = new PatcherControl();
				break;
			case Function.Merger:
				__FunctionControls[functionName] = new MergerControl();
				break;
			case Function.BookmarkGenerator:
				__FunctionControls[functionName] = new AutoBookmarkControl();
				break;
			case Function.InfoExchanger:
				__FunctionControls[functionName] = new InfoExchangerControl();
				break;
			case Function.ExtractPages:
				__FunctionControls[functionName] = new ExtractPageControl();
				break;
			case Function.ExtractImages:
				__FunctionControls[functionName] = new ExtractImageControl();
				break;
			case Function.BookmarkEditor:
				//__FunctionControls[functionName] = new BookmarkEditorControl ();
				//break;
				EditorControl b = new();
				b.DocumentChanged += OnDocumentChanged;
				return b;
			//case FormHelper.Functions.InfoFileOptions:
			//    __FunctionControls[functionName] = new InfoFileOptionControl ();
			//    break;
			case Function.Ocr:
				__FunctionControls[functionName] = new OcrControl();
				break;
			case Function.RenderPages:
				__FunctionControls[functionName] = new RenderImageControl();
				break;
			//case Form.Functions.ImportOptions:
			//    __FunctionControls[functionName] = new ImportOptionControl ();
			//    break;
			//case FormHelper.Functions.Options:
			//    __FunctionControls[functionName] = new AppOptionControl ();
			//    break;
			case Function.About:
				__FunctionControls[functionName] = new AboutControl();
				break;
			//case FormHelper.Functions.Log:
			//    __FunctionControls[functionName] = new ReportControl ();
			//    break;
			case Function.Inspector:
				//__FunctionControls[functionName] = new DocumentInspectorControl ();
				//break;
				DocumentInspectorControl d = new();
				d.DocumentChanged += OnDocumentChanged;
				return d;
			case Function.Rename:
				__FunctionControls[functionName] = new RenameControl();
				break;
			default:
				return null;
				//__FunctionControls[Form.Functions.Default] = new Label ();
				//functionName = Form.Functions.Default;
				//break;
		}

		return __FunctionControls[functionName];
	}

	private void OnDocumentChanged(object sender, DocumentChangedEventArgs args) {
		string p = args.Path;
		_MainStatusLabel.Text = p ?? string.Empty;
		if (FileHelper.IsPathValid(p)) {
			p = System.IO.Path.GetFileNameWithoutExtension(p);
			if (p.Length > 20) {
				p = p.Substring(0, 17) + "...";
			}

			if (sender is Control f) {
				f = f.Parent;
				if (f == null) {
					return;
				}

				f.Text = p;
			}
		}
	}

	#endregion

	public MainForm() {
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		Processor.PdfHelper.ToggleReaderDebugMode(true); // 打开容错模式
		Processor.PdfHelper.ToggleUnethicalMode(true); // 打开强制读取加密文档模式

		try {
			AppContext.Load(null);
		}
		catch (Exception) {
			// ignore loading exception
		}

		Text = Constants.AppName + " [" + Application.ProductVersion + "]";
		MinimumSize = Size;
		StartPosition = FormStartPosition.CenterScreen;
		AllowDrop = true;
		DragEnter += (s, args) => args.FeedbackDragFileOver(Constants.FileExtensions.Pdf);
		DragDrop += (s, args) => OpenFiles(args.DropFileOver(Constants.FileExtensions.Pdf));

		SetupCustomizeToolbar();
		_GeneralToolbar.Visible = AppContext.Toolbar.ShowGeneralToolbar;

		_OpenPdfDialog.DefaultExt = Constants.FileExtensions.Pdf;
		_OpenPdfDialog.Filter = Constants.FileExtensions.PdfFilter;

		_LogControl = new ReportControl {
			Location = _FunctionContainer.Location,
			Size = _FunctionContainer.Size,
			Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
		};
		Controls.Add(_LogControl);
		HideLogControl();
		_LogControl.VisibleChanged += (s, args) => _FunctionContainer.Visible = !_LogControl.Visible;
		_OpenConfigDialog.FileName =
			_SaveConfigDialog.FileName = Constants.AppName + "配置文件" + Constants.FileExtensions.Json;
		_OpenConfigDialog.Filter = _SaveConfigDialog.Filter = Constants.FileExtensions.JsonFilter;
		_FunctionContainer.ImageList = new ImageList();
		_FunctionContainer.AllowDrop = true;
		_FunctionContainer.MouseClick += (s, args) => {
			if (args.Button == MouseButtons.Middle) { ClickCloseTab(args); }
		};
		_FunctionContainer.MouseDoubleClick += (s, args) => { ClickCloseTab(args); };
		_FunctionContainer.TabClosing += (s, args) => {
			TabPage t = _FunctionContainer.SelectedTab;
			Tracker.DebugMessage(args.Action.ToString());
			if (t.Tag.CastOrDefault<Function>() == Function.FrontPage) {
				args.Cancel = true;
				return;
			}

			int i = args.TabPageIndex;
			int c = _FunctionContainer.TabCount;
			if (i == 0 && c > 1) {
				_FunctionContainer.SelectedIndex = 1;
			}
			else if (i < c) {
				_FunctionContainer.SelectedIndex = i - 1;
			}

			_FunctionContainer.TabPages.RemoveAt(i);
			t.Dispose();
			_MainStatusLabel.Text = string.Empty;
		};
		_FunctionContainer.Selected += SelectedFunctionChanged;
		_FunctionContainer.Deselected += FunctionDeselected;

		// 关闭启动屏幕窗口
		using (EventWaitHandle closeSplashEvent = new(false,
				   EventResetMode.ManualReset, "CloseSplashScreenEvent" + Constants.AppEngName)) {
			closeSplashEvent.Set();
		}

		SelectFunctionList(Function.FrontPage);

		_GeneralToolbar.ItemClicked += MenuCommand;
		if (AppContext.CheckUpdateDate < DateTime.Today) {
			CheckUpdate();
			if (AppContext.CheckUpdateInterval != int.MaxValue) {
				AppContext.CheckUpdateDate = DateTime.Today + TimeSpan.FromDays(AppContext.CheckUpdateInterval);
			}
		}

		string[] ca = Environment.GetCommandLineArgs();
		if (ca.HasContent()) {
			OpenFiles(ca);
		}
#if DEBUG
		iTextSharp.text.io.StreamUtil.AddToResourceSearch("iTextAsian.dll");
#endif
	}

	private void OpenFiles(string[] files) {
		foreach (string item in files) {
			FilePath p = new(item);
			if (p.ExistsFile && p.HasExtension(Constants.FileExtensions.Pdf)) {
				OpenFileWithEditor(p.ToFullPath());
			}
		}
	}

	private void CheckUpdate() {
		WebClient client = new();
		client.DownloadDataCompleted += (s, args) => {
			if (args.Error != null) {
				goto Exit;
			}

			try {
				XmlDocument x = new();
				x.Load(new System.IO.MemoryStream(args.Result));
				XmlElement r = x.DocumentElement;
				string u = r.GetAttribute("url");
				if (string.IsNullOrEmpty(u) == false
					&& new Version(ProductVersion) < new Version(r.GetAttribute("version"))
					&& this.ConfirmOKBox("发现新版本，是否前往下载？")) {
					ShowDialogWindow(new UpdateForm());
				}
			}
			catch (Exception) {
				FormHelper.ErrorBox("版本信息文件格式错误，请稍候重试。");
			}

		Exit:
			client.Dispose();
			client = null;
		};
		client.DownloadDataAsync(new Uri(Constants.AppUpdateFile));
	}

	private void ClickCloseTab(MouseEventArgs args) {
		for (int i = _FunctionContainer.TabCount - 1; i >= 0; i--) {
			if (_FunctionContainer.GetTabRect(i).Contains(args.Location) == false) {
				continue;
			}

			if (_FunctionContainer.TabPages[i].Tag.CastOrDefault<Function>() != Function.FrontPage) {
				_FunctionContainer.TabPages[i].Dispose();
			}
		}
	}

	private void MenuCommand(object sender, ToolStripItemClickedEventArgs e) {
		ToolStripItem ci = e.ClickedItem;
		string t = ci.Tag as string;
		if (string.IsNullOrEmpty(t) == false) {
			Function func = (Function)Enum.Parse(typeof(Function), t);
			SelectFunctionList(func);
			return;
		}

		ci.HidePopupMenu();
		if (ci.OwnerItem == _RecentFiles) {
			FunctionControl f = GetActiveFunctionControl() as FunctionControl;
			f.RecentFileItemClicked?.Invoke(_MainMenu, e);
		}
		else {
			ExecuteCommand(ci.Name);
		}
	}

	internal void ExecuteCommand(string commandName) {
		if (commandName == Commands.ResetOptions) {
			if (GetActiveFunctionControl() is IResettableControl f
				&& FormHelper.YesNoBox("是否将当前功能恢复为默认设置？") == DialogResult.Yes) {
				f.Reset();
			}
		}
		else if (commandName == Commands.RestoreOptions && _OpenConfigDialog.ShowDialog() == DialogResult.OK) {
			if (AppContext.Load(_OpenConfigDialog.FileName) == false) {
				FormHelper.ErrorBox("无法加载指定的配置文件。");
				return;
			}

			foreach (Control item in __FunctionControls.Values) {
				(item as IResettableControl)?.Reload();
			}

			SetupCustomizeToolbar();
		}
		else if (commandName == Commands.SaveOptions && _SaveConfigDialog.ShowDialog() == DialogResult.OK) {
			AppContext.Save(_SaveConfigDialog.FileName, false);
		}
		else if (commandName == Commands.LogWindow) {
			ShowLogControl();
		}
		else if (commandName == Commands.CreateShortcut) {
			CommonCommands.CreateShortcut();
		}
		else if (commandName == Commands.VisitHomePage) {
			CommonCommands.VisitHomePage();
		}
		else if (commandName == Commands.CheckUpdate) {
			ShowDialogWindow(new UpdateForm());
		}
		else if (commandName == Commands.Close) {
			if (_FunctionContainer.SelectedTab.Tag.CastOrDefault<Function>() == Function.FrontPage) {
				return;
			}

			_FunctionContainer.SelectedTab.Dispose();
		}
		else if (commandName == Commands.CustomizeToolbar || commandName == "_CustomizeToolbarCommand") {
			ShowDialogWindow(new CustomizeToolbarForm());
			SetupCustomizeToolbar();
		}
		else if (commandName == Commands.ShowGeneralToolbar) {
			_FunctionContainer.SuspendLayout();
			_GeneralToolbar.Visible =
				AppContext.Toolbar.ShowGeneralToolbar = !AppContext.Toolbar.ShowGeneralToolbar;
			_FunctionContainer.PerformLayout();
		}
		else if (commandName == Commands.Exit) {
			Close();
		}
		else if (GetActiveFunctionControl() is FunctionControl f) {
			if (commandName == Commands.Action && f.DefaultButton != null) {
				f.DefaultButton.PerformClick();
			}
			else {
				f.ExecuteCommand(commandName);
			}
		}
	}

	private void SetupCustomizeToolbar() {
		AppContext.Toolbar.RemoveInvalidButtons();
		for (int i = _GeneralToolbar.Items.Count - 1; i > 0; i--) {
			_GeneralToolbar.Items[i].Dispose();
		}

		foreach (ToolbarOptions.ButtonOption item in AppContext.Toolbar.Buttons) {
			if (item.Visible == false) {
				continue;
			}

			_GeneralToolbar.Items.Add(item.CreateButton());
		}
	}

	private DialogResult ShowDialogWindow(Form window) {
		using (Form f = window) {
			f.StartPosition = FormStartPosition.CenterParent;
			return f.ShowDialog(this);
		}
	}

	private Control GetActiveFunctionControl() {
		TabPage t = _FunctionContainer.SelectedTab;
		if (t == null || t.HasChildren == false) {
			return null;
		}

		return t.Controls[0];
	}

	internal void OpenFileWithEditor(string path) {
		SelectFunctionList(Function.BookmarkEditor);
		EditorControl c = GetActiveFunctionControl() as EditorControl;
		if (string.IsNullOrEmpty(path)) {
			c.ExecuteCommand(Commands.Open);
		}
		else {
			c.ExecuteCommand(Commands.OpenFile, path);
		}
	}

	internal void SelectFunctionList(Function func) {
		if (func == Function.PatcherOptions) {
			ShowDialogWindow(new PatcherOptionForm(false) { Options = AppContext.Patcher });
		}
		else if (func == Function.MergerOptions) {
			ShowDialogWindow(new MergerOptionForm());
		}
		else if (func == Function.InfoFileOptions) {
			ShowDialogWindow(new InfoFileOptionControl());
		}
		else if (func == Function.EditorOptions) {
			ShowDialogWindow(new PatcherOptionForm(true) { Options = AppContext.Editor });
		}
		else if (func == Function.Options) {
			ShowDialogWindow(new AppOptionForm());
		}
		else {
			HideLogControl();
			string p = (GetActiveFunctionControl() as IDocumentEditor)?.DocumentPath;
			FunctionControl c = GetFunctionControl(func);
			foreach (TabPage item in _FunctionContainer.TabPages) {
				if (item.Controls.Count > 0 && item.Controls[0] == c) {
					_FunctionContainer.SelectedTab = item;
					if (string.IsNullOrEmpty(p) == false) {
						c.ExecuteCommand(Commands.OpenFile, p);
					}

					return;
				}
			}

			TabPage t = new(c.FunctionName) { Font = System.Drawing.SystemFonts.SmallCaptionFont };
			ImageList.ImageCollection im = _FunctionContainer.ImageList.Images;
			for (int i = im.Count - 1; i >= 0; i--) {
				if (im[i] == c.IconImage) {
					t.ImageIndex = i;
					break;
				}
			}

			if (t.ImageIndex < 0) {
				im.Add(c.IconImage);
				t.ImageIndex = im.Count - 1;
			}

			t.Tag = func;
			_FunctionContainer.TabPages.Add(t);
			c.Size = t.ClientSize;
			c.Dock = DockStyle.Fill;
			t.Controls.Add(c);
			_FunctionContainer.SelectedTab = t;
			AcceptButton = c.DefaultButton;

			if (string.IsNullOrEmpty(p) == false) {
				c.ExecuteCommand(Commands.OpenFile, p);
			}

			//c.HideOnClose = true;
			//c.Show (this._DockPanel);
		}
	}

	private void FunctionDeselected(object sender, TabControlEventArgs args) {
		if (GetActiveFunctionControl() is FunctionControl c) {
			c.OnDeselected();
		}
	}

	private void SelectedFunctionChanged(object sender, TabControlEventArgs args) {
		if (GetActiveFunctionControl() is FunctionControl c) {
			//foreach (ToolStripMenuItem item in _MainMenu.Items) {
			//	c.SetupMenu (item);
			//}
			c.OnSelected();
			_MainStatusLabel.Text = c is IDocumentEditor b ? b.DocumentPath : Messages.Welcome;
			AcceptButton = c.DefaultButton;
		}
	}

	internal string ShowPdfFileDialog() {
		return _OpenPdfDialog.ShowDialog() == DialogResult.OK ? _OpenPdfDialog.FileName : null;
	}

	private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
		try {
			AppContext.Save(null, true);
		}
		catch (Exception) {
			// ignore error
		}
	}

	private void HideLogControl() {
		_LogControl.Hide();
	}

	private void ShowLogControl() {
		_LogControl.Show();
	}

	private void MenuOpening(object sender, EventArgs e) {
		if (GetActiveFunctionControl() is FunctionControl f) {
			f.SetupMenu(sender as ToolStripMenuItem);
		}
	}

	private void RecentFileMenuOpening(object sender, EventArgs e) {
		if (GetActiveFunctionControl() is FunctionControl f && f.ListRecentFiles != null) {
			f.ListRecentFiles(sender, e);
		}
	}
}