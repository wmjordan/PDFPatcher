using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using CLR;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Processor.ContentParser;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public sealed partial class DocumentInspectorControl : FunctionControl, IDocumentEditor
{
	static readonly PdfObjectType[] __XmlExportableTypes = [PdfObjectType.Page, PdfObjectType.Pages, PdfObjectType.Trailer];
	static Dictionary<string, int> __OpNameIcons;
	static int[] __PdfObjectIcons;

	PdfPathDocument _pdf;
	ImageExtractor _imgExp;
	string _fileName;
	ToolStripItem[] _addPdfObjectMenuItems;
	Kind[] _pdfTypeForAddObjectMenuItems;

	static readonly ImageExtracterOptions _imgExpOption = new ImageExtracterOptions() {
		OutputPath = Path.GetTempPath(),
		MergeImages = false
	};

	public override string FunctionName => "结构探查器";

	public override Bitmap IconImage => Properties.Resources.DocumentInspector;

	public event EventHandler<DocumentChangedEventArgs> DocumentChanged;
	public string DocumentPath {
		get => _fileName;
		set {
			if (_fileName != value) {
				_fileName = value;
				DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(value));
			}
		}
	}
	public bool IsBusy => _LoadDocumentWorker.IsBusy;
	public bool IsDirty => false;

	public DocumentInspectorControl() {
		InitializeComponent();
		this.OnFirstLoad(OnLoad);
	}

	void OnLoad() {
		_MainToolbar.ScaleElements();
		_ObjectDetailBox.ScaleColumnWidths();

		_ObjectDetailBox.EmptyListMsg = "请使用“打开”按钮加载需要检查结构的 PDF 文件，或从资源管理器拖放文件到本列表框";

		if (__OpNameIcons == null || __OpNameIcons.Count == 0) {
			__OpNameIcons = InitOpNameIcons();
		}
		if (__PdfObjectIcons == null || __PdfObjectIcons.Length == 0) {
			__PdfObjectIcons = InitPdfObjectIcons();
		}
		#region TreeListView init
		_ObjectDetailBox.SetTreeViewLine();
		_ObjectDetailBox.FixEditControlWidth();
		new TypedColumn<DocumentObject>(_NameColumn) {
			AspectGetter = (DocumentObject d) => d.FriendlyName ?? d.Name,
			ImageGetter = (DocumentObject d) => d.ImageKey ?? d.Type switch {
				PdfObjectType.Normal => GetImageKey(d),
				PdfObjectType.Trailer => __OpNameIcons["Document"],
				PdfObjectType.Root => GetImageKey(d),
				PdfObjectType.Pages => __OpNameIcons["Pages"],
				PdfObjectType.Page => __OpNameIcons["Page"],
				PdfObjectType.Image => __OpNameIcons["Image"],
				PdfObjectType.Form => __OpNameIcons["Form"],
				PdfObjectType.Resources => __OpNameIcons["Resources"],
				PdfObjectType.Outline => __OpNameIcons["Outline"],
				PdfObjectType.PageCommands => __OpNameIcons["PageCommands"],
				PdfObjectType.PageCommand => MakeImageKeyForPageCommand(d),
				PdfObjectType.GoToPage => __OpNameIcons["GoToPage"],
				PdfObjectType.Hidden => __OpNameIcons["Hidden"],
				_ => GetImageKey(d),
			}
		};
		new TypedColumn<DocumentObject>(_ValueColumn) {
			AspectGetter = (DocumentObject d) => d.FriendlyValue ?? d.LiteralValue,
			AspectPutter = (DocumentObject d, object value) => {
				if (!d.UpdateDocumentObject(value)) {
					return;
				}
				var r = d.FindReferenceAncestor();
				if (r != null) {
					RefreshReferences(r);
				}
				if (d.Parent?.Type == PdfObjectType.Outline && d.Name == "Title") {
					d.Parent.Description = (string)value;
					_ObjectDetailBox.RefreshObject(d.Parent);
				}
			}
		};
		_DescriptionColumn.AspectGetter = (object o) => ((DocumentObject)o).Description;
		_ObjectDetailBox.PrimarySortColumn = null;
		_ObjectDetailBox.CopySelectionOnControlC = true;
		_ObjectDetailBox.CellEditStarting += (s, args) => {
			var o = (DocumentObject)args.RowObject;
			string t;
			bool readOnly = true;
			if (args.Column.Index == 2) {
				if (!String.IsNullOrEmpty(o.Description)) {
					t = o.Description;
					goto MAKE_CONTROL;
				}
				goto EXIT;
			}
			var po = o.Value;
			if (po != null) {
				switch (po.TypeKind) {
					case Kind.Boolean:
						args.Control = new CheckBox() { Checked = ((PdfBoolean)po).Value, Bounds = args.CellBounds };
						return;
					case Kind.Integer:
						t = po.IntegerValue.ToText();
						readOnly = false;
						goto MAKE_CONTROL;
					case Kind.Float:
						t = po.FloatValue.ToText();
						readOnly = false;
						goto MAKE_CONTROL;
					case Kind.Name:
						t = ((PdfName)po).Name;
						readOnly = false;
						goto MAKE_CONTROL;
					case Kind.String:
						t = ((PdfString)po).Value;
						readOnly = false;
						goto MAKE_CONTROL;
					case Kind.Dictionary:
					case Kind.Stream:
					case Kind.Null:
						args.Cancel = true;
						return;
				}
			}
			if (args.Value != null) {
				t = args.Value.ToString();
				goto MAKE_CONTROL;
			}
			EXIT:
			args.Cancel = true;
			return;
			MAKE_CONTROL:
			args.Control = new AutoResizingTextBox(args.CellBounds, t, (Control)s) { ReadOnly = readOnly };
		};
		_ObjectDetailBox.CanExpandGetter = (object o) => o is DocumentObject d && d.HasChildren;
		_ObjectDetailBox.ChildrenGetter = (object o) => o is DocumentObject d
			? (System.Collections.IEnumerable)d.Children
			: null;
		_ObjectDetailBox.RowFormatter = (OLVListItem olvItem) => {
			if (olvItem.RowObject is not DocumentObject o) {
				return;
			}
			if (o.Type == PdfObjectType.Normal) {
				var po = o.Value;
				if (po == null) {
					return;
				}
				if (po.TypeKind == Kind.Reference) {
					olvItem.UseItemStyleForSubItems = false;
					olvItem.SubItems[_ValueColumn.Index].ForeColor = SystemColors.HotTrack;
				}
				else if (po.IsContainer()) {
					olvItem.UseItemStyleForSubItems = false;
					olvItem.SubItems[_ValueColumn.Index].ForeColor = SystemColors.GrayText;
				}
			}
			else if (o.Type == PdfObjectType.Page) {
				olvItem.ForeColor = Color.DarkRed;
			}
			else if (o.Type == PdfObjectType.Pages) {
				olvItem.Font = new Font(olvItem.Font, FontStyle.Bold);
				olvItem.ForeColor = Color.DarkRed;
				olvItem.BackColor = Color.LightYellow;
			}
			else if (o.Type == PdfObjectType.Trailer) {
				olvItem.Font = new Font(olvItem.Font, FontStyle.Bold);
				olvItem.BackColor = Color.LightYellow;
			}
			else if (o.Type == PdfObjectType.Outline) {
				olvItem.UseItemStyleForSubItems = false;
				olvItem.SubItems[0].ForeColor = SystemColors.HotTrack;
				olvItem.SubItems[_ValueColumn.Index].ForeColor = SystemColors.HotTrack;
			}
			else if (o.Type == PdfObjectType.PageCommand && (o.Name == "字符串" || o.Name == "换行字符串")) {
				olvItem.UseItemStyleForSubItems = false;
				olvItem.SubItems[_DescriptionColumn.Index].Font = new Font(olvItem.Font, FontStyle.Underline);
			}
		};
		_ObjectDetailBox.SelectionChanged += _ObjectDetailBox_SelectionChanged;
		_ObjectDetailBox.IsSimpleDropSink = true;
		_ObjectDetailBox.CanDrop += _ObjectDetailBox_CanDrop;
		_ObjectDetailBox.Dropped += _ObjectDetailBox_Dropped;
		#endregion
		_AddNameNode.Image = _ObjectTypeIcons.Images["Name"];
		_AddStringNode.Image = _ObjectTypeIcons.Images["String"];
		_AddDictNode.Image = _ObjectTypeIcons.Images["Dictionary"];
		_AddArrayNode.Image = _ObjectTypeIcons.Images["Array"];
		_AddNumberNode.Image = _ObjectTypeIcons.Images["Number"];
		_AddBooleanNode.Image = _ObjectTypeIcons.Images["Bool"];

		_addPdfObjectMenuItems = [_AddNameNode, _AddStringNode, _AddDictNode, _AddArrayNode, _AddNumberNode, _AddBooleanNode];
		_pdfTypeForAddObjectMenuItems = [Kind.Name, Kind.String, Kind.Dictionary, Kind.Array, Kind.Float, Kind.Boolean];

		_OpenButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
		_OpenButton.DropDownItemClicked += (s, args) => {
			args.ClickedItem.Owner.Hide();
			LoadDocument(args.ClickedItem.ToolTipText);
		};
		Disposed += (s, args) => _pdf?.Document.Dispose();
	}

	static object MakeImageKeyForPageCommand(DocumentObject d) {
		return d.ImageKey ??= (d.ExtensiveObject is Operation n && __OpNameIcons.TryGetValue(n.Operator, out int ic))
					|| (d.Name.StartsWith(Constants.ContentPrefix + ":") && __OpNameIcons.TryGetValue(d.Name, out ic))
				? ic
				: __OpNameIcons["?"];
	}

	public override void SetupCommand(ToolStripItem item) {
		var n = item.Name;
		switch (n) {
			case Commands.Action:
				item.Text = _SaveButton.Text;
				item.Image = _SaveButton.Image;
				item.ToolTipText = _SaveButton.ToolTipText;
				return;
			case Commands.Delete:
				EnableCommand(item, _DeleteButton.Enabled, true);
				return;
		}
		if (Commands.CommonSelectionCommands.Contains(n)
			|| Commands.RecentFiles == n
			) {
			EnableCommand(item, _ObjectDetailBox.GetItemCount() > 0, true);
		}
		else {
			base.SetupCommand(item);
		}
	}

	public override void ExecuteCommand(string commandName, params string[] parameters) {
		switch (commandName) {
			case Commands.Open:
				var p = AppContext.MainForm.ShowPdfFileDialog();
				if (p != null) {
					LoadDocument(p);
				}
				break;
			case Commands.OpenFile:
				LoadDocument(parameters[0]);
				break;
			case Commands.Action:
				SaveDocument();
				break;
			case Commands.SelectAllItems:
				_ObjectDetailBox.SelectAll();
				break;
			case Commands.SelectNone:
				_ObjectDetailBox.SelectedObjects = null;
				break;
			case Commands.InvertSelection:
				_ObjectDetailBox.InvertSelect();
				break;
			default:
				base.ExecuteCommand(commandName, parameters);
				break;
		}
	}

	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
		if (_ObjectDetailBox.IsCellEditing) {
			return base.ProcessCmdKey(ref msg, keyData);
		}
		switch (keyData ^ Keys.Control) {
			case Keys.O: ExecuteCommand(Commands.Open); return true;
			case Keys.C: ExecuteCommand(Commands.Copy); return true;
			case Keys.S: ExecuteCommand(Commands.Action); return true;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	void RefreshReferences(DocumentObject r) {
		if (r.Value == null || r.Value.TypeKind != Kind.Reference) {
			return;
		}
		var v = r.Value as PdfReference;
		var l = _ObjectDetailBox.VirtualListSize;
		for (int i = 0; i < l; i++) {
			if (_ObjectDetailBox.GetModelObject(i) is not DocumentObject m) {
				continue;
			}
			if (m.Type == PdfObjectType.PageCommands) {
				i += (_ObjectDetailBox.VirtualListDataSource as TreeListView.Tree).GetVisibleDescendentCount(m);
			}
			if (m.ExtensiveObject != null && m.Value != null && m.Value.TypeKind == Kind.Reference) {
				var mv = m.Value as PdfReference;
				if (mv.Number == v.Number && mv.Generation == v.Generation && m != r) {
					_ObjectDetailBox.RefreshObject(m);
				}
			}
		}
	}

	public void CloseDocument() {
		_pdf.Document?.CloseFile();
	}

	public void Reopen() {
		_pdf.Document?.Reopen();
	}

	void _ObjectDetailBox_CanDrop(object sender, OlvDropEventArgs e) {
		if (e.DataObject is not DataObject o) {
			return;
		}
		foreach (var item in o.GetFileDropList()) {
			if (FileHelper.HasExtension(item, Constants.FileExtensions.Xml)
				|| FileHelper.HasExtension(item, Constants.FileExtensions.Pdf)) {
				e.Handled = true;
				e.DropTargetLocation = DropTargetLocation.Background;
				e.Effect = DragDropEffects.Move;
				e.InfoMessage = "打开文件" + item;
				return;
			}
		}
		e.Effect = DragDropEffects.None;
		e.DropTargetLocation = DropTargetLocation.None;
	}

	void _ObjectDetailBox_Dropped(object sender, OlvDropEventArgs e) {
		if (e.DataObject is not DataObject o) {
			return;
		}
		var f = o.GetFileDropList();
		if (f.Count == 0) {
			return;
		}
		LoadDocument(f[0]);
	}

	void _ObjectDetailBox_SelectionChanged(object sender, EventArgs e) {
		var si = _ObjectDetailBox.SelectedItem;
		if (si == null) {
			return;
		}
		_ExpandButton.Enabled = _CollapseButton.Enabled = true;
		_ViewButton.Enabled = false;
		_DeleteButton.Enabled = false;
		_ExportButton.Enabled = false;
		_AddObjectMenu.Enabled = false;
		if (_ObjectDetailBox.GetModelObject(si.Index) is not DocumentObject d) {
			return;
		}
		if (d.Value != null && (d.Value.TypeKind == Kind.Reference || d.Value.TypeKind == Kind.Stream)) {
			var s = d.Value as PdfStream ?? d.ExtensiveObject as PdfStream;
			if (s is not null) {
				_ViewButton.Enabled = !d.Name.HasPrefix("Font");
				_ExportButton.Enabled = _AddObjectMenu.Enabled = true;
				if (s.HasNameValue(PdfNames.Subtype, PdfNames.Image)) {
					ShowDescription("图片", type: "stream");
					return;
				}
			}
		}
		if ((d.Value is not null && d.Value is PdfDictionary) || d.ExtensiveObject is PdfDictionary) {
			_AddObjectMenu.Enabled = true;
		}
		if (__XmlExportableTypes.Contains(d.Type)) {
			_ExportButton.Enabled = true;
		}
		if (d.Parent == null) {
			if (d.Type == PdfObjectType.Trailer) {
				ShowDescription("文档根节点", _fileName);
			}
			else if (d.Type == PdfObjectType.Pages) {
				ShowDescription("文档页面", "页数：" + _pdf.PageCount);
			}
			return;
		}
		var i = PdfStructInfo.GetInfo(d.Parent.GetContextName(), d.Name);
		string t = null;
		var o = d.ExtensiveObject as PdfObject ?? d.Value;
		if (o != null) {
			t = o.TypeKind.GetName();
		}
		ShowDescription(String.IsNullOrEmpty(i.Name) || d.Name == i.Name ? d.Name : $"{d.Name}:{i.Name}", i.Description, t, d);
		_DeleteButton.Enabled = !i.IsRequired && d != null
			&& (d.Type.CeqAny(PdfObjectType.Normal, PdfObjectType.Image, PdfObjectType.Form, PdfObjectType.Resources) || d.Type == PdfObjectType.Outline && d.Name == "Outlines");
	}

	void ShowPath(DocumentObject obj) {
		var sb = StringBuilderCache.Acquire(30).Append("位置：");
		var stack = new Stack<KeyValuePair<int, DocumentObject>>();
		stack.Push(GetPathNumericObject(obj));
		while ((obj = obj.Parent) != null) {
			stack.Push(GetPathNumericObject(obj));
		}
		while (stack.Count != 0) {
			var n = stack.Pop();
			sb.Append('/').Append(n.Value.Name);
			if (n.Key > 1) {
				sb.Append('[').Append(n.Key.ToText()).Append(']');
			}
		}
		_DescriptionBox.AppendLine().AppendText(StringBuilderCache.GetStringAndRelease(sb));
	}

	KeyValuePair<int, DocumentObject> GetPathNumericObject(DocumentObject obj) {
		var p = obj.Parent;
		return p is null
			? new KeyValuePair<int, DocumentObject>(1, obj)
			: new KeyValuePair<int, DocumentObject>(p.IndexOfChild(obj, true), obj);
	}

	Dictionary<string, int> InitOpNameIcons() {
		var p = new string[] { "Document", "Pages", "Page", "PageCommands", "Image", "Form", "Font", "Resources", "Hidden", "GoToPage", "Outline", "Null" };
		var n = new string[] {
			"q", "Tm", "cm", "gs", "ri", "CS", "cs",
			"RG", "rg", "scn", "SCN", "sc", "SC", "K", "k",
			"g", "G", "s", "S",
			"f", "F", "f*", "b", "B", "b*", "B*",
			"Tf", "Tz", "Ts", "T*", "Td", "TD",
			"TJ", "Tj", "'", "\"",
			"Tk", "Tr", "Tc", "Tw", "TL",
			"BI", "BT", "BDC", "BMC",
			"Do",
			"W*", "W", "c", "v", "y", "l", "re",
			"m", "h", "n", "w", "J", "j", "M", "d", "i",
			"pdf:number", "pdf:string", "pdf:name", "pdf:dictionary", "pdf:array", "pdf:boolean",
			"?" };
		var ico = new string[] {
			"op_q", "op_tm", "op_cm", "op_gs", "op_gs", "op_gs", "op_gs",
			"op_sc", "op_sc", "op_sc", "op_sc", "op_sc", "op_sc", "op_sc", "op_sc",
			"op_g", "op_g", "op_s", "op_s",
			"op_f", "op_f", "op_f", "op_b", "op_b", "op_b", "op_b",
			"Font", "op_Tz", "op_Ts", "op_Td", "op_Td", "op_Td",
			"op_TJ", "op_TJ", "op_TJ", "op_TJ",
			"op_Tr", "op_Tr", "op_Tc", "op_Tc", "op_Tl",
			"Image", "op_BT", "op_BDC", "op_BDC",
			"Resources",
			"op_W*", "op_W*", "op_c", "op_c", "op_c", "op_l", "op_re",
			"op_m", "op_h", "op_h", "op_w", "op_l", "op_l", "op_M_", "op_d", "op_gs",
			"Number", "String", "Name", "Dictionary", "Array", "Bool",
			"Error" };
		var d = new Dictionary<string, int>(n.Length + p.Length);
		foreach (var i in p) {
			d.Add(i, _ObjectTypeIcons.Images.IndexOfKey(i));
		}
		for (int i = 0; i < n.Length; i++) {
			d.Add(n[i], _ObjectTypeIcons.Images.IndexOfKey(ico[i]));
		}
		return d;
	}
	int[] InitPdfObjectIcons() {
		var n = new Kind[] { Kind.Null, Kind.Array, Kind.Boolean,
			Kind.Dictionary, Kind.Reference, Kind.Name,
			Kind.Float, Kind.Stream, Kind.String };
		var d = new int[(int)Kind.Unknown];
		foreach (Kind kind in n) {
			d[(int)kind] = _ObjectTypeIcons.Images.IndexOfKey(kind.GetName());
		}
		d[(int)Kind.Integer] = _ObjectTypeIcons.Images.IndexOfKey(Kind.Integer.GetName());
		return d;
	}

	static int GetImageKey(DocumentObject d) {
		if (d.Value is not null) {
			var po = d.Value;
			if (po.TypeKind == Kind.Reference && d.ExtensiveObject is PdfObject e) {
				po = e;
			}
			return __PdfObjectIcons[(int)po.TypeKind];
		}
		return __PdfObjectIcons[(int)Kind.Null];
	}

	void _GotoImportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
		AppContext.MainForm.SelectFunctionList(Function.Patcher);
	}

	void ControlEvent(object sender, EventArgs e) {
		if (sender == _OpenButton) {
			ExecuteCommand(Commands.Open);
		}
	}

	void LoadDocument(string path) {
		_MainToolbar.Enabled = _ObjectDetailBox.Enabled = false;
		_DescriptionBox.Text = "正在打开文档：" + path;
		_LoadDocumentWorker.RunWorkerAsync(path);
	}

	void ShowDescription(string name, string description = null, string type = null, DocumentObject obj = null) {
		_DescriptionBox.Text = String.Empty;
		if (String.IsNullOrEmpty(name)) {
			return;
		}

		_DescriptionBox.SetSelectionFontSize(13);
		_DescriptionBox.SetSelectionBold(true);
		_DescriptionBox.AppendText(name);
		_DescriptionBox.SetSelectionFontSize(9);
		if (type != null) {
			_DescriptionBox.AppendText(Environment.NewLine);
			_DescriptionBox.AppendText("类型：" + type);
		}
		if (obj != null) {
			ShowPath(obj);
		}
		if (description != null) {
			_DescriptionBox.AppendText(Environment.NewLine);
			_DescriptionBox.AppendText(description);
		}
	}

	void ToolbarItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		if (_ObjectDetailBox.FocusedItem == null) {
			return;
		}
		var ci = e.ClickedItem;
		if (ci == _SaveButton) {
			SaveDocument();
			return;
		}
		var cn = ci.Name;
		var n = _ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject;
		if (ci == _DeleteButton) {
			if (n == null || n.Parent == null) {
				return;
			}
			if (n.Parent.Value is not PdfObject po) {
				return;
			}
			if (po.TypeKind == Kind.Reference) {
				po = n.Parent.ExtensiveObject as PdfObject;
			}
			if (po.IsContainer()
				&& n.Parent.RemoveChildByName(n.Name)) {
				_ObjectDetailBox.RefreshObject(n.Parent);
			}
		}
		else if (ci == _ViewButton) {
			if (n == null) {
				return;
			}
			if (n.ExtensiveObject is not PdfStream s) {
				return;
			}
			if (s.HasNameValue(PdfNames.Subtype, PdfNames.Image)
				|| n.Name == "Thumb") {
				try {
					using var img = _pdf.Document.LoadImage(n.Value as PdfReference);
					using var pixmap = img.GetPixmap();
					new ImageViewerForm(pixmap).Show();
				}
				catch (Exception ex) {
					AppContext.MainForm.ErrorBox("查看图像时出现错误", ex);
				}
			}
			else {
				var isContentStream = n.Name == "Contents" && n.Parent.Type == PdfObjectType.Page
					|| n.Parent?.Name == "Contents" && n.Parent.Parent?.Type == PdfObjectType.Page
					|| n.Children.Any(i => i.Name == "Subtype" && i.LiteralValue == "Form");
				using var f = new TextViewerForm(s.GetBytes(), true, isContentStream);
				f.ShowDialog(FindForm());
			}
		}
		else if (cn == "_ExportBinary") {
			ci.HidePopupMenu();
			ExportBinaryStream(n, true);
		}
		else if (cn == "_ExportHexText") {
			ci.HidePopupMenu();
			ExportBinHexStream(n, true);
		}
		else if (cn == "_ExportUncompressedBinary") {
			ci.HidePopupMenu();
			ExportBinaryStream(n, false);
		}
		else if (cn == "_ExportUncompressedHexText") {
			ci.HidePopupMenu();
			ExportBinHexStream(n, false);
		}
		else if (cn == "_ExportToUnicode") {
			ci.HidePopupMenu();
			ExportToUnicode(n);
		}
		else if (cn == "_ExportXml") {
			ci.HidePopupMenu();
			var so = _ObjectDetailBox.SelectedObjects;
			var ep = new List<int>(so.Count);
			bool exportTrailer = false;
			if (_ObjectDetailBox.Items[0].Selected || n.Type == PdfObjectType.Trailer) {
				exportTrailer = true;
			}
			foreach (var item in so) {
				if (item is not DocumentObject d) {
					continue;
				}
				if (d.Type == PdfObjectType.Page) {
					ep.Add((int)d.ExtensiveObject);
				}
				else if (d.Type == PdfObjectType.Pages) {
					foreach (var r in PageRangeCollection.Parse((string)d.ExtensiveObject, 1, _pdf.PageCount, true)) {
						ep.AddRange(r);
					}
				}
			}
			//if (ep.Count == 1) {
			//	ExportXmlInfo(n.FriendlyName ?? n.Name, exportTrailer, [(int)n.ExtensiveObject]);
			//}
			//else {
			//	ExportXmlInfo(Path.GetFileNameWithoutExtension(_fileName), exportTrailer, ep.ToArray());
			//}
		}
		else if (cn == "_ExpandButton") {
			if (ModifierKeys.MatchFlags(Keys.Shift)) {
				_ObjectDetailBox.Expand(_ObjectDetailBox.SelectedObjects, true);
			}
			else {
				_ObjectDetailBox.ExpandSelected();
			}
		}
		else if (cn == "_CollapseButton") {
			_ObjectDetailBox.CollapseSelected();
		}
	}

	void AddChildNode(DocumentObject documentObject, Kind objectType) {
		using var f = new AddPdfObjectForm();
		f.PdfObjectType = objectType;
		if (f.ShowDialog() == DialogResult.OK) {
			var v = f.CreatePdfObject(_pdf.Document);
			if (f.CreateAsIndirect) {
				var id = _pdf.Document.CreateObject();
				_pdf.Document.UpdateObject(id, v);
				v = _pdf.Document.NewReference(id, 0);
			}
			((PdfDictionary)documentObject.Value.UnderlyingObject)[new PdfName(f.ObjectName)] = v;
			documentObject.PopulateChildren(true);
			_ObjectDetailBox.RefreshObject(documentObject);
		}
	}

	//void ExportXmlInfo(string fileName, bool exportTrailer, int[] pages) {
	//	using (var d = new SaveFileDialog() {
	//		AddExtension = true,
	//		FileName = fileName + Constants.FileExtensions.Xml,
	//		DefaultExt = Constants.FileExtensions.Xml,
	//		Filter = Constants.FileExtensions.XmlFilter,
	//		Title = "请选择信息文件的保存位置"
	//	}) {
	//		if (d.ShowDialog() == DialogResult.OK) {
	//			var exp = new PdfContentExport(new ExporterOptions() { ExtractPageDictionary = true, ExportContentOperators = true });
	//			using (XmlWriter w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
	//				w.WriteStartDocument();
	//				w.WriteStartElement(Constants.PdfInfo);
	//				w.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/", Constants.ContentNamespace);
	//				DocInfoExporter.WriteDocumentInfoAttributes(w, _fileName, _pdf.PageCount);
	//				if (exportTrailer) {
	//					exp.ExportTrailer(w, _pdf.Document);
	//				}
	//				exp.ExportPage(_pdf.Document, w, pages);
	//				w.WriteEndElement();
	//			}
	//		}
	//	}
	//}

	void ExportBinHexStream(DocumentObject n, bool decode) {
		using var d = new SaveFileDialog() {
			AddExtension = true,
			FileName = (n.FriendlyName ?? n.Name) + Constants.FileExtensions.Txt,
			DefaultExt = Constants.FileExtensions.Txt,
			Filter = "文本形式的二进制数据文件(*.txt)|*.txt|" + Constants.FileExtensions.AllFilter,
			Title = "请选择文件流的保存位置"
		};
		if (d.ShowDialog() == DialogResult.OK) {
			var s = n.ExtensiveObject as PdfStream;
			try {
				var sb = decode ? DecodeStreamBytes(n) : s.GetRawBytes();
				sb.DumpHexBinBytes(d.FileName);
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("导出流数据时出错", ex);
			}
		}
	}

	void ExportBinaryStream(DocumentObject n, bool decode) {
		using var d = new SaveFileDialog() {
			AddExtension = true,
			FileName = (n.FriendlyName ?? n.Name) + ".bin",
			DefaultExt = ".bin",
			Filter = "二进制数据文件(*.bin,*.dat)|*.bin;*.dat|" + Constants.FileExtensions.AllFilter,
			Title = "请选择文件流的保存位置"
		};
		if (d.ShowDialog() == DialogResult.OK) {
			var s = n.ExtensiveObject as PdfStream;
			try {
				var sb = decode ? DecodeStreamBytes(n) : s.GetRawBytes();
				sb.DumpBytes(d.FileName);
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("导出流数据时出错", ex);
			}
		}
	}

	void ExportToUnicode(DocumentObject n) {
		using var d = new SaveFileDialog {
			AddExtension = true,
			FileName = (n.Parent.FriendlyName ?? n.Name) + ".xml",
			DefaultExt = ".xml",
			Filter = "统一码映射信息文件(*.xml)|*.xml|" + Constants.FileExtensions.AllFilter,
			Title = "请选择统一码映射表的保存位置"
		};
		if (d.ShowDialog() == DialogResult.OK) {
			//var s = n.ExtensiveObject as PRStream;
			//try {
			//	var m = new CMapToUnicode();
			//	CMapParserEx.ParseCid("", m, new CidLocationFromByte(PdfReader.GetStreamBytes(s)));
			//	using (var w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
			//		w.WriteStartElement("toUnicode");
			//		w.WriteAttributeString("name", m.Name);
			//		w.WriteAttributeString("registry", m.Registry);
			//		w.WriteAttributeString("supplement", m.Supplement.ToText());
			//		w.WriteAttributeString("ordering", m.Ordering);
			//		w.WriteAttributeString("oneByteMappings", m.HasOneByteMappings().ToString());
			//		w.WriteAttributeString("twoByteMappings", m.HasTwoByteMappings().ToString());
			//		foreach (var item in m.CreateDirectMapping()) {
			//			w.WriteStartElement("map");
			//			w.WriteAttributeString("cid", item.Key.ToText());
			//			w.WriteAttributeString("uni", Char.ConvertFromUtf32(item.Value));
			//			w.WriteEndElement();
			//		}
			//		w.WriteEndElement();
			//	}
			//}
			//catch (Exception ex) {
			//	AppContext.MainForm.ErrorBox("导出统一码映射表数据时出错", ex);
			//}
		}
	}

	byte[] DecodeStreamBytes(DocumentObject d) {
		if (d.Type == PdfObjectType.Image) {
			using var img = _pdf.Document.LoadImage(d.Value as PdfReference);
			using var buffer = img.GetCompressedBuffer();
			using var stream = buffer.Open();
			return stream.ReadAll();
		}
		var s = d.Value as PdfStream ?? d.ExtensiveObject as PdfStream;
		return s?.GetBytes();
	}

	void SaveDocument() {
		string path;
		using (var d = new SaveFileDialog() {
			DefaultExt = Constants.FileExtensions.Pdf,
			Filter = Constants.FileExtensions.PdfFilter,
			AddExtension = true,
			FileName = FileHelper.GetNewFileNameFromSourceFile(_fileName, Constants.FileExtensions.Pdf),
			InitialDirectory = Path.GetDirectoryName(_fileName)
		}) {
			if (d.ShowDialog() != DialogResult.OK) {
				return;
			}
			path = d.FileName;
		}

		bool o = false;
		var n = String.Empty;
		if (FileHelper.ComparePath(path, _fileName) && FormHelper.YesNoBox("是否覆盖原始文件？") == DialogResult.Yes) {
			o = true;
		}
		_ObjectDetailBox.ClearObjects();
		try {
			n = o ? FileHelper.GetTempNameFromFileDirectory(path, Constants.FileExtensions.Pdf) : path;
			_pdf.Document.Save(n, new WriterOptions {
				UseObjectStreams = AppContext.Patcher.FullCompression,
				CompressImages = true,
				CompressFonts = true,
				CompressionMode = CompressionMode.ZLib
			});
			_pdf.Close();
			if (o) {
				File.Delete(path);
				File.Move(n, path);
			}
		}
		catch (Exception ex) {
			AppContext.MainForm.ErrorBox("保存文件时出错", ex);
			if (o && File.Exists(n)) {
				try {
					File.Delete(n);
				}
				catch (Exception e2) {
					AppContext.MainForm.ErrorBox("无法删除临时文件：" + n, e2);
				}
			}
			LoadDocument(_fileName);
			return;
		}
		LoadDocument(path);
	}

	void _LoadDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
		var path = e.Argument as string;
		try {
			var d = new PdfPathDocument(path);
			_pdf?.Close();
			_pdf = d;
			e.Result = path;
		}
		catch (iTextSharp.text.exceptions.BadPasswordException) {
			FormHelper.ErrorBox(Messages.PasswordInvalid);
		}
		catch (Exception ex) {
			AppContext.MainForm.ErrorBox("打开 PDF 文件时遇到错误", ex);
		}
	}

	void _LoadDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
		_DescriptionBox.Text = String.Empty;
		if (e.Result is string path) {
			AppContext.RecentItems.AddHistoryItem(AppContext.Recent.SourcePdfFiles, path);
			DocumentPath = path;
			ReloadPdf();
		}
		_MainToolbar.Enabled = _ObjectDetailBox.Enabled = true;
	}

	void ReloadPdf() {
		_imgExp = new ImageExtractor(_imgExpOption);

		_ObjectDetailBox.ClearObjects();
		_ObjectDetailBox.Objects = ((IHierarchicalObject<DocumentObject>)_pdf).Children;
		_SaveButton.Enabled = true;
		_AddObjectMenu.Enabled = false;
		_DeleteButton.Enabled = false;
	}

	void _ExportButton_DropDownOpening(object sender, EventArgs e) {
		var n = _ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject;
		var m = _ExportButton.DropDownItems;
		m["_ExportHexText"].Enabled
			= m["_ExportBinary"].Enabled
			= m["_ExportUncompressedHexText"].Enabled
			= m["_ExportUncompressedBinary"].Enabled
			= n.ExtensiveObject is PdfStream;
		m["_ExportXml"].Enabled
			= __XmlExportableTypes.Contains(n.Type);
		m["_ExportToUnicode"].Visible = n.ExtensiveObject is PdfStream && n.Name == "ToUnicode";
	}

	void _AddObjectMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		AddChildNode(
			_ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject,
			ValueHelper.MapValue(e.ClickedItem, _addPdfObjectMenuItems, _pdfTypeForAddObjectMenuItems)
		);
	}
}
