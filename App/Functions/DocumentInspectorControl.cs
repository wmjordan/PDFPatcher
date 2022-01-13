using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.fonts.cmaps;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;
using PDFPatcher.Processor.Imaging;
using PDFPatcher.Properties;

namespace PDFPatcher.Functions;

[ToolboxItem(false)]
public sealed partial class DocumentInspectorControl : FunctionControl, IDocumentEditor
{
	private static readonly PdfObjectType[] __XmlExportableTypes =
		new PdfObjectType[] { PdfObjectType.Page, PdfObjectType.Pages, PdfObjectType.Trailer };

	private static Dictionary<string, int> __OpNameIcons;
	private static Dictionary<int, int> __PdfObjectIcons;

	private static readonly ImageExtracterOptions _imgExpOption = new() {
		OutputPath = Path.GetTempPath(),
		MergeImages = false
	};

	private ToolStripItem[] _addPdfObjectMenuItems;
	private string _fileName;
	private ImageExtractor _imgExp;

	private PdfPathDocument _pdf;
	private int[] _pdfTypeForAddObjectMenuItems;

	public DocumentInspectorControl() {
		InitializeComponent();
		//this.Icon = Common.FormHelper.ToIcon (Properties.Resources.DocumentInspector);
	}

	public override string FunctionName => "文档结构探查器";

	public override Bitmap IconImage => Resources.DocumentInspector;

	public event EventHandler<DocumentChangedEventArgs> DocumentChanged;

	public string DocumentPath {
		get => _fileName;
		set {
			if (_fileName != value) {
				_fileName = value;
				if (DocumentChanged != null) {
					DocumentChanged(this, new DocumentChangedEventArgs(value));
				}
			}
		}
	}

	public void CloseDocument() {
		if (_pdf.Document != null) {
			_pdf.Document.SafeFile.Close();
		}
	}

	public void Reopen() {
		if (_pdf.Document != null) {
			_pdf.Document.SafeFile.ReOpen();
		}
	}

	private void DocumentInspectorControl_OnLoad(object sender, EventArgs e) {
		_ObjectDetailBox.EmptyListMsg = "请使用“打开”按钮加载需要检查结构的 PDF 文件，或从资源管理器拖放文件到本列表框";

		if (__OpNameIcons == null || __OpNameIcons.Count == 0) {
			__OpNameIcons = InitOpNameIcons();
		}

		if (__PdfObjectIcons == null || __PdfObjectIcons.Count == 0) {
			__PdfObjectIcons = InitPdfObjectIcons();
		}

		#region TreeListView init

		_ObjectDetailBox.SetTreeViewLine();
		_ObjectDetailBox.FixEditControlWidth();
		new TypedColumn<DocumentObject>(_NameColumn) {
			AspectGetter = (DocumentObject d) => {
				return d.FriendlyName ?? d.Name;
			},
			ImageGetter = (DocumentObject d) => {
				if (d.ImageKey != null) {
					return d.ImageKey;
				}

				if (d.Type != PdfObjectType.Normal) {
					switch (d.Type) {
						case PdfObjectType.Trailer:
							return __OpNameIcons["Document"];
						case PdfObjectType.Root:
							break;
						case PdfObjectType.Pages:
							return __OpNameIcons["Pages"];
						case PdfObjectType.Page:
							return __OpNameIcons["Page"];
						case PdfObjectType.Image:
							return __OpNameIcons["Image"];
						case PdfObjectType.Outline:
							return __OpNameIcons["Outline"];
						case PdfObjectType.PageCommands:
							return __OpNameIcons["PageCommands"];
						case PdfObjectType.PageCommand:
							if (d.ImageKey == null) {
								string n = d.ExtensiveObject as string;
								if ((n != null && __OpNameIcons.TryGetValue(n, out int ic))
									|| (d.Name.StartsWith(Constants.ContentPrefix + ":") &&
										__OpNameIcons.TryGetValue(d.Name, out ic))
								   ) {
									d.ImageKey = ic;
								}
								else {
									d.ImageKey = __OpNameIcons["Null"];
								}
							}

							return d.ImageKey;
						case PdfObjectType.Hidden:
							return __OpNameIcons["Hidden"];
					}
				}

				return GetImageKey(d);
			}
		};
		new TypedColumn<DocumentObject>(_ValueColumn) {
			AspectGetter = (DocumentObject d) => {
				return d.FriendlyValue ?? d.LiteralValue;
			},
			AspectPutter = (DocumentObject d, object value) => {
				if (d.UpdateDocumentObject(value)) {
					DocumentObject r = d.FindReferenceAncestor();
					if (r != null) {
						RefreshReferences(r);
					}
				}
				else if (d.Parent != null && d.Parent.Type == PdfObjectType.Outline && d.Name == "Title") {
					d.Parent.Description = (string)value;
					_ObjectDetailBox.RefreshObject(d.Parent);
				}
			}
		};
		_DescriptionColumn.AspectGetter = (object o) => {
			return ((DocumentObject)o).Description;
		};
		_ObjectDetailBox.PrimarySortColumn = null;
		_ObjectDetailBox.CopySelectionOnControlC = true;
		_ObjectDetailBox.CellEditStarting += (s, args) => {
			DocumentObject d = args.RowObject as DocumentObject;
			PdfObject po = d.Value as PdfObject;
			if (po == null) {
				args.Cancel = true;
				return;
			}

			if (po.Type == PdfObject.BOOLEAN) {
				args.Control = new CheckBox() { Checked = (po as PdfBoolean).BooleanValue, Bounds = args.CellBounds };
			}
			else if (po.Type == PdfObject.NUMBER) {
				args.Control = new TextBox() { Text = (po as PdfNumber).DoubleValue.ToText(), Bounds = args.CellBounds };
			}
			else if (po.Type == PdfObject.INDIRECT || PdfHelper.CompoundTypes.Contains(po.Type)) {
				args.Cancel = true;
			}
		};
		_ObjectDetailBox.CanExpandGetter = (object o) => {
			DocumentObject d = o as DocumentObject;
			if (d == null) {
				return false;
			}

			if (d.Type == PdfObjectType.GoToPage) {
				d.ImageKey = __OpNameIcons["GoToPage"];
			}

			return d.HasChildren;
		};
		_ObjectDetailBox.ChildrenGetter = delegate (object o) {
			DocumentObject d = o as DocumentObject;
			if (d == null) {
				return null;
			}

			return d.Children;
		};
		_ObjectDetailBox.RowFormatter = (OLVListItem olvItem) => {
			DocumentObject o = olvItem.RowObject as DocumentObject;
			if (o == null) {
				return;
			}

			if (o.Type == PdfObjectType.Normal) {
				PdfObject po = o.Value;
				if (po == null) {
					return;
				}

				if (po.Type == PdfObject.INDIRECT) {
					olvItem.UseItemStyleForSubItems = false;
					olvItem.SubItems[_ValueColumn.Index].ForeColor = SystemColors.HotTrack;
				}
				else if (PdfHelper.CompoundTypes.Contains(po.Type)) {
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
				ListViewItem.ListViewSubItem s = olvItem.SubItems[_DescriptionColumn.Index];
				s.Font = new Font(olvItem.Font, FontStyle.Underline);
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

		_addPdfObjectMenuItems = new ToolStripItem[] {
			_AddNameNode, _AddStringNode, _AddDictNode, _AddArrayNode, _AddNumberNode, _AddBooleanNode
		};
		_pdfTypeForAddObjectMenuItems = new int[] {
			PdfObject.NAME, PdfObject.STRING, PdfObject.DICTIONARY, PdfObject.ARRAY, PdfObject.NUMBER, PdfObject.BOOLEAN
		};

		_OpenButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
		_OpenButton.DropDownItemClicked += (s, args) => {
			args.ClickedItem.Owner.Hide();
			LoadDocument(args.ClickedItem.ToolTipText);
		};
		Disposed += (s, args) => {
			if (_pdf != null) {
				_pdf.Document.Dispose();
			}
		};
	}

	public override void SetupCommand(ToolStripItem item) {
		string n = item.Name;
		switch (n) {
			case Commands.Action:
				item.Text = _SaveButton.Text;
				item.Image = _SaveButton.Image;
				item.ToolTipText = _SaveButton.ToolTipText;
				return;
			case Commands.Delete:
				EnableCommand(item, _DeleteButton.Enabled, true);
				return;
			default:
				break;
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
				string p = AppContext.MainForm.ShowPdfFileDialog();
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
			case Commands.InvertSelectItem:
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
			case Keys.O:
				ExecuteCommand(Commands.Open);
				return true;
			case Keys.C:
				ExecuteCommand(Commands.Copy);
				return true;
			case Keys.S:
				ExecuteCommand(Commands.Action);
				return true;
		}

		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void RefreshReferences(DocumentObject r) {
		if (r.Value == null || r.Value.Type != PdfObject.INDIRECT) {
			return;
		}

		PdfIndirectReference v = r.Value as PdfIndirectReference;
		int l = _ObjectDetailBox.VirtualListSize;
		for (int i = 0; i < l; i++) {
			DocumentObject m = _ObjectDetailBox.GetModelObject(i) as DocumentObject;
			if (m == null) {
				continue;
			}

			if (m.Type == PdfObjectType.PageCommands) {
				i += (_ObjectDetailBox.VirtualListDataSource as TreeListView.Tree).GetVisibleDescendentCount(m);
			}

			if (m.ExtensiveObject != null && m.Value != null && m.Value.Type == PdfObject.INDIRECT) {
				PdfIndirectReference mv = m.Value as PdfIndirectReference;
				if (mv.Number == v.Number && mv.Generation == v.Generation && m != r) {
					_ObjectDetailBox.RefreshObject(m);
				}
			}
		}
	}

	private void _ObjectDetailBox_CanDrop(object sender, OlvDropEventArgs e) {
		DataObject o = e.DataObject as DataObject;
		if (o == null) {
			return;
		}

		StringCollection f = o.GetFileDropList();
		foreach (string item in f) {
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

	private void _ObjectDetailBox_Dropped(object sender, OlvDropEventArgs e) {
		DataObject o = e.DataObject as DataObject;
		if (o == null) {
			return;
		}

		StringCollection f = o.GetFileDropList();
		if (f.Count == 0) {
			return;
		}

		LoadDocument(f[0]);
	}

	private void _ObjectDetailBox_SelectionChanged(object sender, EventArgs e) {
		OLVListItem si = _ObjectDetailBox.SelectedItem;
		if (si == null) {
			return;
		}

		_ExpandButton.Enabled = _CollapseButton.Enabled = true;
		DocumentObject d = _ObjectDetailBox.GetModelObject(si.Index) as DocumentObject;
		_ViewButton.Enabled = false;
		_DeleteButton.Enabled = false;
		_ExportButton.Enabled = false;
		_AddObjectMenu.Enabled = false;
		if (d == null) {
			return;
		}

		if (d.Value != null && (d.Value.Type == PdfObject.INDIRECT || d.Value.Type == PdfObject.STREAM)) {
			PRStream s = d.Value as PRStream ?? d.ExtensiveObject as PRStream;
			if (s != null) {
				_ViewButton.Enabled = d.Name.StartsWith("Font") == false;
				_ExportButton.Enabled = _AddObjectMenu.Enabled = true;
				if (PdfName.IMAGE.Equals(s.GetAsName(PdfName.SUBTYPE))) {
					ShowDescription("图片", null, PdfHelper.GetTypeName(PdfObject.STREAM));
					return;
				}
			}
		}

		if ((d.Value != null && d.Value is PdfDictionary) || d.ExtensiveObject is PdfDictionary) {
			_AddObjectMenu.Enabled = true;
		}

		if (__XmlExportableTypes.Contains(d.Type)) {
			_ExportButton.Enabled = true;
		}

		if (d.Parent == null) {
			if (d.Type == PdfObjectType.Trailer) {
				ShowDescription("文档根节点", _fileName, null);
			}
			else if (d.Type == PdfObjectType.Pages) {
				ShowDescription("文档页面", "页数：" + _pdf.PageCount, null);
			}

			return;
		}

		PdfStructInfo i = PdfStructInfo.GetInfo(d.Parent.GetContextName(), d.Name);
		string t = null;
		PdfObject o = d.ExtensiveObject as PdfObject ?? d.Value;
		if (o != null) {
			t = PdfHelper.GetTypeName(o.Type);
		}

		ShowDescription(
			string.IsNullOrEmpty(i.Name) || d.Name == i.Name ? d.Name : string.Concat(d.Name, ":", i.Name),
			i.Description, t);
		_DeleteButton.Enabled = !i.IsRequired && d != null
											  && (d.Type == PdfObjectType.Normal || d.Type == PdfObjectType.Image ||
												  (d.Type == PdfObjectType.Outline && d.Name == "Outlines"));
	}

	private Dictionary<string, int> InitOpNameIcons() {
		string[] p = new string[] {
			"Document", "Pages", "Page", "PageCommands", "Image", "Hidden", "GoToPage", "Outline", "Null"
		};
		string[] n = new string[] {
			"q", "Tm", "cm", "gs", "ri", "CS", "cs", "RG", "rg", "scn", "SCN", "sc", "SC", "K", "k", "g", "G", "s", "S",
			"f", "F", "f*", "b", "B", "b*", "B*", "Tf", "Tz", "Ts", "T*", "Td", "TD", "TJ", "Tj", "'", "\"", "Tk", "Tr",
			"Tc", "Tw", "TL", "BI", "BT", "BDC", "BMC", "Do", "W*", "W", "c", "v", "y", "l", "re", "m", "h", "n", "w",
			"J", "j", "M", "d", "i", "pdf:number", "pdf:string", "pdf:name", "pdf:dictionary", "pdf:array",
			"pdf:boolean"
		};
		string[] ico = new string[] {
			"op_q", "op_tm", "op_cm", "op_gs", "op_gs", "op_gs", "op_gs", "op_sc", "op_sc", "op_sc", "op_sc", "op_sc",
			"op_sc", "op_sc", "op_sc", "op_g", "op_g", "op_s", "op_s", "op_f", "op_f", "op_f", "op_b", "op_b", "op_b",
			"op_b", "Font", "op_Tz", "op_Ts", "op_Td", "op_Td", "op_Td", "op_TJ", "op_TJ", "op_TJ", "op_TJ", "op_Tr",
			"op_Tr", "op_Tc", "op_Tc", "op_Tl", "Image", "op_BT", "op_BDC", "op_BDC", "Resources", "op_W*", "op_W*",
			"op_c", "op_c", "op_c", "op_l", "op_re", "op_m", "op_h", "op_h", "op_w", "op_l", "op_l", "op_M_", "op_d",
			"op_gs", "Number", "String", "Name", "Dictionary", "Array", "Bool"
		};
		Dictionary<string, int> d = new(n.Length + p.Length);
		foreach (string i in p) {
			d.Add(i, _ObjectTypeIcons.Images.IndexOfKey(i));
		}

		for (int i = 0; i < n.Length; i++) {
			d.Add(n[i], _ObjectTypeIcons.Images.IndexOfKey(ico[i]));
		}

		return d;
	}

	private Dictionary<int, int> InitPdfObjectIcons() {
		int[] n = new int[] {
			PdfObject.NULL, PdfObject.ARRAY, PdfObject.BOOLEAN, PdfObject.DICTIONARY, PdfObject.INDIRECT,
			PdfObject.NAME, PdfObject.NUMBER, PdfObject.STREAM, PdfObject.STRING
		};
		Dictionary<int, int> d = new(n.Length);
		for (int i = 0; i < n.Length; i++) {
			d.Add(n[i], _ObjectTypeIcons.Images.IndexOfKey(PdfHelper.GetTypeName(n[i])));
		}

		return d;
	}

	private static int GetImageKey(DocumentObject d) {
		if (d.Value != null) {
			PdfObject po = d.Value;
			if (po.Type == PdfObject.INDIRECT && d.ExtensiveObject is PdfObject) {
				po = d.ExtensiveObject as PdfObject;
			}

			return __PdfObjectIcons.GetOrDefault(po.Type);
		}

		return __PdfObjectIcons[PdfObject.NULL];
	}

	private void _GotoImportLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
		AppContext.MainForm.SelectFunctionList(Function.Patcher);
	}

	private void bookmarkEditor1_DragEnter(object sender, DragEventArgs e) {
		e.FeedbackDragFileOver(Constants.FileExtensions.PdfAndAllBookmarkExtension);
	}

	private void ControlEvent(object sender, EventArgs e) {
		if (sender == _OpenButton) {
			ExecuteCommand(Commands.Open);
		}
	}

	private void LoadDocument(string path) {
		_MainMenu.Enabled = _ObjectDetailBox.Enabled = false;
		_DescriptionBox.Text = "正在打开文档：" + path;
		_LoadDocumentWorker.RunWorkerAsync(path);
	}

	private void ShowDescription(string name, string description, string type) {
		_DescriptionBox.Text = string.Empty;
		if (string.IsNullOrEmpty(name) == false) {
			_DescriptionBox.SetSelectionFontSize(13);
			_DescriptionBox.SetSelectionBold(true);
			_DescriptionBox.AppendText(name);
			_DescriptionBox.SetSelectionFontSize(9);
			if (type != null) {
				_DescriptionBox.AppendText(Environment.NewLine);
				_DescriptionBox.AppendText("类型：" + type);
			}

			if (description != null) {
				_DescriptionBox.AppendText(Environment.NewLine);
				_DescriptionBox.AppendText(description);
			}
		}
	}

	private void ToolbarItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		if (_ObjectDetailBox.FocusedItem == null) {
			return;
		}

		ToolStripItem ci = e.ClickedItem;
		string cn = ci.Name;
		DocumentObject n = _ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject;
		if (ci == _SaveButton) {
			SaveDocument();
		}
		else if (ci == _DeleteButton) {
			//if (this.ActiveControl == _DocumentTree) {
			if (n == null || n.Parent == null) {
				return;
			}

			PdfObject po = n.Parent.Value as PdfObject;
			if (po == null) {
				return;
			}

			if (po.Type == PdfObject.INDIRECT) {
				po = n.Parent.ExtensiveObject as PdfObject;
			}

			if (PdfHelper.CompoundTypes.Contains(po.Type)) {
				if (n.Parent.RemoveChildByName(n.Name)) {
					_ObjectDetailBox.RefreshObject(n.Parent);
				}
			}
			//}
		}
		else if (ci == _ViewButton) {
			if (n == null) {
				return;
			}

			PRStream s = n.ExtensiveObject as PRStream;
			if (s == null) {
				return;
			}

			if (PdfName.IMAGE.Equals(s.GetAsName(PdfName.SUBTYPE))
				|| n.Name == "Thumb") {
				ImageInfo info = new(s);
				byte[] bytes = info.DecodeImage(_imgExpOption);
				if (bytes != null) {
					if (info.LastDecodeError != null) {
						FormHelper.ErrorBox("导出图像时出现错误：" + info.LastDecodeError);
					}
					else if (info.ExtName != Constants.FileExtensions.Dat) {
						new ImageViewerForm(info, bytes).Show();
					}
				}
			}
			else {
				byte[] b = PdfReader.GetStreamBytes(s);
				using (MemoryStream ms = new(b))
				using (StreamReader r = new(ms))
				using (TextViewerForm f = new(r.ReadToEnd(), true)) {
					f.ShowDialog(FindForm());
					//_DescriptionBox.Text = String.Empty;
					//while (r.Peek () != -1) {
					//    _DescriptionBox.AppendText (r.ReadLine ());
					//    _DescriptionBox.AppendText (Environment.NewLine);
					//}
				}
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
			IList so = _ObjectDetailBox.SelectedObjects;
			List<int> ep = new(so.Count);
			bool exportTrailer = false;
			if (_ObjectDetailBox.Items[0].Selected || n.Type == PdfObjectType.Trailer) {
				exportTrailer = true;
			}

			foreach (object item in so) {
				DocumentObject d = item as DocumentObject;
				if (d == null) {
					continue;
				}

				if (d.Type == PdfObjectType.Page) {
					ep.Add((int)d.ExtensiveObject);
				}
				else if (d.Type == PdfObjectType.Pages) {
					foreach (PageRange r in PageRangeCollection.Parse((string)d.ExtensiveObject, 1, _pdf.PageCount,
								 true)) {
						foreach (int p in r) {
							ep.Add(p);
						}
					}
				}
			}

			if (ep.Count == 1) {
				ExportXmlInfo(n.FriendlyName ?? n.Name, exportTrailer, new int[] { (int)n.ExtensiveObject });
			}
			else {
				ExportXmlInfo(Path.GetFileNameWithoutExtension(_fileName), exportTrailer, ep.ToArray());
			}
		}
		else if (cn == "_ExpandButton") {
			_ObjectDetailBox.ExpandSelected();
		}
		else if (cn == "_CollapseButton") {
			_ObjectDetailBox.CollapseSelected();
		}
	}

	private void AddChildNode(DocumentObject documentObject, int objectType) {
		using (AddPdfObjectForm f = new()) {
			f.PdfObjectType = objectType;
			if (f.ShowDialog() == DialogResult.OK) {
				PdfDictionary d = (documentObject.ExtensiveObject ?? documentObject.ExtensiveObject) as PdfDictionary;
				PdfObject v = f.PdfValue;
				d.Put(new PdfName(f.ObjectName), f.CreateAsIndirect ? _pdf.Document.AddPdfObject(v) : v);
				documentObject.PopulateChildren(true);
				_ObjectDetailBox.RefreshObject(documentObject);
			}
		}
	}

	private void ExportXmlInfo(string fileName, bool exportTrailer, int[] pages) {
		using (SaveFileDialog d = new() {
			AddExtension = true,
			FileName = fileName + Constants.FileExtensions.Xml,
			DefaultExt = Constants.FileExtensions.Xml,
			Filter = Constants.FileExtensions.XmlFilter,
			Title = "请选择信息文件的保存位置"
		}) {
			if (d.ShowDialog() == DialogResult.OK) {
				PdfContentExport exp = new(new ExporterOptions() {
					ExtractPageDictionary = true,
					ExportContentOperators = true
				});
				using (XmlWriter w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
					w.WriteStartDocument();
					w.WriteStartElement(Constants.PdfInfo);
					w.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/",
						Constants.ContentNamespace);
					DocInfoExporter.WriteDocumentInfoAttributes(w, _fileName, _pdf.PageCount);
					if (exportTrailer) {
						exp.ExportTrailer(w, _pdf.Document);
					}

					exp.ExtractPage(_pdf.Document, w, pages);
					w.WriteEndElement();
				}
			}
		}
	}

	private void ExportBinHexStream(DocumentObject n, bool decode) {
		using (SaveFileDialog d = new() {
			AddExtension = true,
			FileName = (n.FriendlyName ?? n.Name) + Constants.FileExtensions.Txt,
			DefaultExt = Constants.FileExtensions.Txt,
			Filter = "文本形式的二进制数据文件(*.txt)|*.txt|" + Constants.FileExtensions.AllFilter,
			Title = "请选择文件流的保存位置"
		}) {
			if (d.ShowDialog() == DialogResult.OK) {
				PRStream s = n.ExtensiveObject as PRStream;
				try {
					byte[] sb = decode ? DecodeStreamBytes(n) : PdfReader.GetStreamBytesRaw(s);
					sb.DumpHexBinBytes(d.FileName);
				}
				catch (Exception ex) {
					FormHelper.ErrorBox("在导出流数据时出错：" + ex.Message);
				}
			}
		}
	}

	private void ExportBinaryStream(DocumentObject n, bool decode) {
		using (SaveFileDialog d = new() {
			AddExtension = true,
			FileName = (n.FriendlyName ?? n.Name) + ".bin",
			DefaultExt = ".bin",
			Filter = "二进制数据文件(*.bin,*.dat)|*.bin;*.dat|" + Constants.FileExtensions.AllFilter,
			Title = "请选择文件流的保存位置"
		}) {
			if (d.ShowDialog() == DialogResult.OK) {
				PRStream s = n.ExtensiveObject as PRStream;
				try {
					byte[] sb = decode ? DecodeStreamBytes(n) : PdfReader.GetStreamBytesRaw(s);
					sb.DumpBytes(d.FileName);
				}
				catch (Exception ex) {
					FormHelper.ErrorBox("在导出流数据时出错：" + ex.Message);
				}
			}
		}
	}

	private void ExportToUnicode(DocumentObject n) {
		using (SaveFileDialog d = new() {
			AddExtension = true,
			FileName = (n.Parent.FriendlyName ?? n.Name) + ".xml",
			DefaultExt = ".xml",
			Filter = "统一码映射信息文件(*.xml)|*.xml|" + Constants.FileExtensions.AllFilter,
			Title = "请选择统一码映射表的保存位置"
		}) {
			if (d.ShowDialog() == DialogResult.OK) {
				PRStream s = n.ExtensiveObject as PRStream;
				try {
					byte[] touni = PdfReader.GetStreamBytes((PRStream)s);
					CidLocationFromByte lb = new(touni);
					CMapToUnicode m = new();
					CMapParserEx.ParseCid("", m, lb);
					using (XmlWriter w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
						w.WriteStartElement("toUnicode");
						w.WriteAttributeString("name", m.Name);
						w.WriteAttributeString("registry", m.Registry);
						w.WriteAttributeString("supplement", m.Supplement.ToText());
						w.WriteAttributeString("ordering", m.Ordering);
						w.WriteAttributeString("oneByteMappings", m.HasOneByteMappings().ToString());
						w.WriteAttributeString("twoByteMappings", m.HasTwoByteMappings().ToString());
						foreach (KeyValuePair<int, int> item in m.CreateDirectMapping()) {
							w.WriteStartElement("map");
							w.WriteAttributeString("cid", item.Key.ToText());
							w.WriteAttributeString("uni", char.ConvertFromUtf32(item.Value));
							w.WriteEndElement();
						}

						w.WriteEndElement();
					}
				}
				catch (Exception ex) {
					FormHelper.ErrorBox("在导出统一码映射表数据时出错：" + ex.Message);
				}
			}
		}
	}

	private byte[] DecodeStreamBytes(DocumentObject d) {
		PRStream s = d.Value as PRStream ?? d.ExtensiveObject as PRStream;
		if (d.Type == PdfObjectType.Image) {
			ImageInfo info = new(s);
			return info.DecodeImage(_imgExpOption);
		}

		return PdfReader.GetStreamBytes(s);
	}

	private void SaveDocument() {
		string path;
		using (SaveFileDialog d = new() {
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
		string n = string.Empty;
		if (FileHelper.ComparePath(path, _fileName) && FormHelper.YesNoBox("是否覆盖原始文件？") == DialogResult.Yes) {
			o = true;
		}

		_ObjectDetailBox.ClearObjects();
		_pdf.Document.RemoveUnusedObjects();
		try {
			n = o ? FileHelper.GetTempNameFromFileDirectory(path, Constants.FileExtensions.Pdf) : path;
			using (FileStream s = new(n, FileMode.Create)) {
				PdfStamper w = new(_pdf.Document, s);
				if (AppContext.Patcher.FullCompression) {
					w.SetFullCompression();
				}

				w.Close();
				_pdf.Close();
			}

			if (o) {
				File.Delete(path);
				File.Move(n, path);
			}
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("保存文件时出错：" + ex.Message);
			if (o && File.Exists(n)) {
				try {
					File.Delete(n);
				}
				catch (Exception) {
					FormHelper.ErrorBox("无法删除临时文件：" + n);
				}
			}

			LoadDocument(_fileName);
			return;
		}

		LoadDocument(path);
	}

	private void _LoadDocumentWorker_DoWork(object sender, DoWorkEventArgs e) {
		string path = e.Argument as string;
		try {
			PdfPathDocument d = new(path);
			_pdf?.Close();
			_pdf = d;
			e.Result = path;
			//Common.Form.Action ev = delegate () { _FilePathBox.Text = path; };
			//_FilePathBox.Invoke (ev);
		}
		catch (BadPasswordException) {
			FormHelper.ErrorBox(Messages.PasswordInvalid);
		}
		catch (Exception ex) {
			FormHelper.ErrorBox("在打开 PDF 文件时遇到错误：\n" + ex.Message);
		}
	}

	private void _LoadDocumentWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
		string path = e.Result as string;
		_DescriptionBox.Text = string.Empty;
		if (path != null) {
			AppContext.RecentItems.AddHistoryItem(AppContext.Recent.SourcePdfFiles, path);
			DocumentPath = path;
			ReloadPdf();
		}

		_MainMenu.Enabled = _ObjectDetailBox.Enabled = true;
	}

	private void ReloadPdf() {
		_imgExp = new ImageExtractor(_imgExpOption, _pdf.Document);

		_ObjectDetailBox.ClearObjects();
		_ObjectDetailBox.Objects = ((IHierarchicalObject<DocumentObject>)_pdf).Children;
		_SaveButton.Enabled = true;
		_AddObjectMenu.Enabled = false;
		_DeleteButton.Enabled = false;
	}

	private void _ExportButton_DropDownOpening(object sender, EventArgs e) {
		DocumentObject n = _ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject;
		ToolStripItemCollection m = _ExportButton.DropDownItems;
		m["_ExportHexText"].Enabled
			= m["_ExportBinary"].Enabled
				= m["_ExportUncompressedHexText"].Enabled
					= m["_ExportUncompressedBinary"].Enabled
						= n.ExtensiveObject as PRStream != null;
		m["_ExportXml"].Enabled
			= __XmlExportableTypes.Contains(n.Type);
		m["_ExportToUnicode"].Visible = n.ExtensiveObject as PRStream != null && n.Name == "ToUnicode";
	}

	private void _AddObjectMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
		AddChildNode(
			_ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject,
			ValueHelper.MapValue(e.ClickedItem, _addPdfObjectMenuItems, _pdfTypeForAddObjectMenuItems)
		);
	}
}