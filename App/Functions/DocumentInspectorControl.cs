﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Xml;
using BrightIdeasSoftware;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.fonts.cmaps;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions
{
	[ToolboxItem(false)]
	public sealed partial class DocumentInspectorControl : FunctionControl, IDocumentEditor
	{
		static readonly PdfObjectType[] __XmlExportableTypes = [PdfObjectType.Page, PdfObjectType.Pages, PdfObjectType.Trailer];
		static Dictionary<string, int> __OpNameIcons;
		static Dictionary<int, int> __PdfObjectIcons;

		PdfPathDocument _pdf;
		ImageExtractor _imgExp;
		string _fileName;
		ToolStripItem[] _addPdfObjectMenuItems;
		int[] _pdfTypeForAddObjectMenuItems;

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
			_MainToolbar.ScaleIcons(16);
			_ObjectDetailBox.ScaleColumnWidths();

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
				AspectGetter = (DocumentObject d) => d.FriendlyName ?? d.Name,
				ImageGetter = (DocumentObject d) => {
					if (d.ImageKey != null) {
						return d.ImageKey;
					}
					if (d.Type == PdfObjectType.Normal) {
						return GetImageKey(d);
					}
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
						case PdfObjectType.Form:
							return __OpNameIcons["Form"];
						case PdfObjectType.Resources:
							return __OpNameIcons["Resources"];
						case PdfObjectType.Outline:
							return __OpNameIcons["Outline"];
						case PdfObjectType.PageCommands:
							return __OpNameIcons["PageCommands"];
						case PdfObjectType.PageCommand:
							if (d.ImageKey == null) {
								var n = d.ExtensiveObject as string;
								if ((n != null && __OpNameIcons.TryGetValue(n, out int ic))
									|| (d.Name.StartsWith(Constants.ContentPrefix + ":") && __OpNameIcons.TryGetValue(d.Name, out ic))
									) {
									d.ImageKey = ic;
								}
								else {
									d.ImageKey = __OpNameIcons["?"];
								}
							}
							return d.ImageKey;
						case PdfObjectType.Hidden:
							return __OpNameIcons["Hidden"];
					}
					return GetImageKey(d);
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
					switch (po.Type) {
						case PdfObject.BOOLEAN:
							args.Control = new CheckBox() { Checked = ((PdfBoolean)po).BooleanValue, Bounds = args.CellBounds };
							return;
						case PdfObject.NUMBER:
							t = ((PdfNumber)po).DoubleValue.ToText();
							readOnly = false;
							goto MAKE_CONTROL;
						case PdfObject.NAME:
							t = PdfName.DecodeName(((PdfName)po).ToString());
							readOnly = false;
							goto MAKE_CONTROL;
						case PdfObject.STRING:
							t = ((PdfString)po).ToUnicodeString();
							readOnly = false;
							goto MAKE_CONTROL;
						case PdfObject.DICTIONARY:
						case PdfObject.STREAM:
						case PdfObject.NULL:
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
			_ObjectDetailBox.CanExpandGetter = (object o) => {
				if (o is not DocumentObject d) {
					return false;
				}
				if (d.Type == PdfObjectType.GoToPage) {
					d.ImageKey = __OpNameIcons["GoToPage"];
				}
				return d.HasChildren;
			};
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
			_pdfTypeForAddObjectMenuItems = [PdfObject.NAME, PdfObject.STRING, PdfObject.DICTIONARY, PdfObject.ARRAY, PdfObject.NUMBER, PdfObject.BOOLEAN];

			_OpenButton.DropDownOpening += FileListHelper.OpenPdfButtonDropDownOpeningHandler;
			_OpenButton.DropDownItemClicked += (s, args) => {
				args.ClickedItem.Owner.Hide();
				LoadDocument(args.ClickedItem.ToolTipText);
			};
			Disposed += (s, args) => _pdf?.Document.Dispose();
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
			if (r.Value == null || r.Value.Type != PdfObject.INDIRECT) {
				return;
			}
			var v = r.Value as PdfIndirectReference;
			var l = _ObjectDetailBox.VirtualListSize;
			for (int i = 0; i < l; i++) {
				if (_ObjectDetailBox.GetModelObject(i) is not DocumentObject m) {
					continue;
				}
				if (m.Type == PdfObjectType.PageCommands) {
					i += (_ObjectDetailBox.VirtualListDataSource as TreeListView.Tree).GetVisibleDescendentCount(m);
				}
				if (m.ExtensiveObject != null && m.Value != null && m.Value.Type == PdfObject.INDIRECT) {
					var mv = m.Value as PdfIndirectReference;
					if (mv.Number == v.Number && mv.Generation == v.Generation && m != r) {
						_ObjectDetailBox.RefreshObject(m);
					}
				}
			}
		}

		public void CloseDocument() {
			_pdf.Document?.SafeFile.Close();
		}

		public void Reopen() {
			_pdf.Document?.SafeFile.ReOpen();
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
			if (d.Value != null && (d.Value.Type == PdfObject.INDIRECT || d.Value.Type == PdfObject.STREAM)) {
				var s = d.Value as PRStream ?? d.ExtensiveObject as PRStream;
				if (s != null) {
					_ViewButton.Enabled = !d.Name.HasPrefix("Font");
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
			var i = PdfStructInfo.GetInfo(d.Parent.GetContextName(), d.Name);
			string t = null;
			var o = d.ExtensiveObject as PdfObject ?? d.Value;
			if (o != null) {
				t = PdfHelper.GetTypeName(o.Type);
			}
			ShowDescription(String.IsNullOrEmpty(i.Name) || d.Name == i.Name ? d.Name : $"{d.Name}:{i.Name}", i.Description, t);
			_DeleteButton.Enabled = !i.IsRequired && d != null
				&& (d.Type == PdfObjectType.Normal || d.Type == PdfObjectType.Image || d.Type == PdfObjectType.Form || d.Type == PdfObjectType.Resources || d.Type == PdfObjectType.Outline && d.Name == "Outlines");
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
		Dictionary<int, int> InitPdfObjectIcons() {
			var n = new int[] { PdfObject.NULL, PdfObject.ARRAY, PdfObject.BOOLEAN,
				PdfObject.DICTIONARY, PdfObject.INDIRECT, PdfObject.NAME,
				PdfObject.NUMBER, PdfObject.STREAM, PdfObject.STRING };
			var d = new Dictionary<int, int>(n.Length);
			for (int i = 0; i < n.Length; i++) {
				d.Add(n[i], _ObjectTypeIcons.Images.IndexOfKey(PdfHelper.GetTypeName(n[i])));
			}
			return d;
		}

		static int GetImageKey(DocumentObject d) {
			if (d.Value != null) {
				var po = d.Value;
				if (po.Type == PdfObject.INDIRECT && d.ExtensiveObject is PdfObject e) {
					po = e;
				}
				return __PdfObjectIcons.GetOrDefault(po.Type);
			}
			return __PdfObjectIcons[PdfObject.NULL];
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

		void ShowDescription(string name, string description, string type) {
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
				if (po.Type == PdfObject.INDIRECT) {
					po = n.Parent.ExtensiveObject as PdfObject;
				}
				if (PdfHelper.CompoundTypes.Contains(po.Type)
					&& n.Parent.RemoveChildByName(n.Name)) {
					_ObjectDetailBox.RefreshObject(n.Parent);
				}
			}
			else if (ci == _ViewButton) {
				if (n == null) {
					return;
				}
				if (n.ExtensiveObject is not PRStream s) {
					return;
				}
				if (PdfName.IMAGE.Equals(s.GetAsName(PdfName.SUBTYPE))
					|| n.Name == "Thumb") {
					var info = new Processor.Imaging.ImageInfo(s);
					var bytes = info.DecodeImage(_imgExpOption);
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
					using (var f = new TextViewerForm(PdfReader.GetStreamBytes(s), true)) {
						f.ShowDialog(FindForm());
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
				if (ep.Count == 1) {
					ExportXmlInfo(n.FriendlyName ?? n.Name, exportTrailer, [(int)n.ExtensiveObject]);
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

		void AddChildNode(DocumentObject documentObject, int objectType) {
			using (var f = new AddPdfObjectForm()) {
				f.PdfObjectType = objectType;
				if (f.ShowDialog() == DialogResult.OK) {
					var v = f.PdfValue;
					((PdfDictionary)documentObject.ExtensiveObject).Put(new PdfName(f.ObjectName), f.CreateAsIndirect ? _pdf.Document.AddPdfObject(v) : v);
					documentObject.PopulateChildren(true);
					_ObjectDetailBox.RefreshObject(documentObject);
				}
			}
		}

		void ExportXmlInfo(string fileName, bool exportTrailer, int[] pages) {
			using (var d = new SaveFileDialog() {
				AddExtension = true,
				FileName = fileName + Constants.FileExtensions.Xml,
				DefaultExt = Constants.FileExtensions.Xml,
				Filter = Constants.FileExtensions.XmlFilter,
				Title = "请选择信息文件的保存位置"
			}) {
				if (d.ShowDialog() == DialogResult.OK) {
					var exp = new PdfContentExport(new ExporterOptions() { ExtractPageDictionary = true, ExportContentOperators = true });
					using (XmlWriter w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
						w.WriteStartDocument();
						w.WriteStartElement(Constants.PdfInfo);
						w.WriteAttributeString(Constants.ContentPrefix, "http://www.w3.org/2000/xmlns/", Constants.ContentNamespace);
						DocInfoExporter.WriteDocumentInfoAttributes(w, _fileName, _pdf.PageCount);
						if (exportTrailer) {
							exp.ExportTrailer(w, _pdf.Document);
						}
						exp.ExportPage(_pdf.Document, w, pages);
						w.WriteEndElement();
					}
				}
			}
		}

		void ExportBinHexStream(DocumentObject n, bool decode) {
			using (var d = new SaveFileDialog() {
				AddExtension = true,
				FileName = (n.FriendlyName ?? n.Name) + Constants.FileExtensions.Txt,
				DefaultExt = Constants.FileExtensions.Txt,
				Filter = "文本形式的二进制数据文件(*.txt)|*.txt|" + Constants.FileExtensions.AllFilter,
				Title = "请选择文件流的保存位置"
			}) {
				if (d.ShowDialog() == DialogResult.OK) {
					var s = n.ExtensiveObject as PRStream;
					try {
						var sb = decode ? DecodeStreamBytes(n) : PdfReader.GetStreamBytesRaw(s);
						sb.DumpHexBinBytes(d.FileName);
					}
					catch (Exception ex) {
						AppContext.MainForm.ErrorBox("导出流数据时出错", ex);
					}
				}
			}
		}

		void ExportBinaryStream(DocumentObject n, bool decode) {
			using (var d = new SaveFileDialog() {
				AddExtension = true,
				FileName = (n.FriendlyName ?? n.Name) + ".bin",
				DefaultExt = ".bin",
				Filter = "二进制数据文件(*.bin,*.dat)|*.bin;*.dat|" + Constants.FileExtensions.AllFilter,
				Title = "请选择文件流的保存位置"
			}) {
				if (d.ShowDialog() == DialogResult.OK) {
					var s = n.ExtensiveObject as PRStream;
					try {
						var sb = decode ? DecodeStreamBytes(n) : PdfReader.GetStreamBytesRaw(s);
						sb.DumpBytes(d.FileName);
					}
					catch (Exception ex) {
						AppContext.MainForm.ErrorBox("导出流数据时出错", ex);
					}
				}
			}
		}

		void ExportToUnicode(DocumentObject n) {
			using (var d = new SaveFileDialog {
				AddExtension = true,
				FileName = (n.Parent.FriendlyName ?? n.Name) + ".xml",
				DefaultExt = ".xml",
				Filter = "统一码映射信息文件(*.xml)|*.xml|" + Constants.FileExtensions.AllFilter,
				Title = "请选择统一码映射表的保存位置"
			}) {
				if (d.ShowDialog() == DialogResult.OK) {
					var s = n.ExtensiveObject as PRStream;
					try {
						var m = new CMapToUnicode();
						CMapParserEx.ParseCid("", m, new CidLocationFromByte(PdfReader.GetStreamBytes(s)));
						using (var w = XmlWriter.Create(d.FileName, DocInfoExporter.GetWriterSettings())) {
							w.WriteStartElement("toUnicode");
							w.WriteAttributeString("name", m.Name);
							w.WriteAttributeString("registry", m.Registry);
							w.WriteAttributeString("supplement", m.Supplement.ToText());
							w.WriteAttributeString("ordering", m.Ordering);
							w.WriteAttributeString("oneByteMappings", m.HasOneByteMappings().ToString());
							w.WriteAttributeString("twoByteMappings", m.HasTwoByteMappings().ToString());
							foreach (var item in m.CreateDirectMapping()) {
								w.WriteStartElement("map");
								w.WriteAttributeString("cid", item.Key.ToText());
								w.WriteAttributeString("uni", Char.ConvertFromUtf32(item.Value));
								w.WriteEndElement();
							}
							w.WriteEndElement();
						}
					}
					catch (Exception ex) {
						AppContext.MainForm.ErrorBox("导出统一码映射表数据时出错", ex);
					}
				}
			}
		}

		byte[] DecodeStreamBytes(DocumentObject d) {
			var s = d.Value as PRStream ?? d.ExtensiveObject as PRStream;
			if (d.Type == PdfObjectType.Image) {
				var info = new Processor.Imaging.ImageInfo(s);
				return info.DecodeImage(_imgExpOption);
			}
			return PdfReader.GetStreamBytes(s);
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
			_pdf.Document.RemoveUnusedObjects();
			try {
				n = o ? FileHelper.GetTempNameFromFileDirectory(path, Constants.FileExtensions.Pdf) : path;
				using (var s = new FileStream(n, FileMode.Create)) {
					var w = new PdfStamper(_pdf.Document, s);
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
				= n.ExtensiveObject is PRStream;
			m["_ExportXml"].Enabled
				= __XmlExportableTypes.Contains(n.Type);
			m["_ExportToUnicode"].Visible = n.ExtensiveObject is PRStream && n.Name == "ToUnicode";
		}

		void _AddObjectMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			AddChildNode(
				_ObjectDetailBox.GetModelObject(_ObjectDetailBox.FocusedItem.Index) as DocumentObject,
				ValueHelper.MapValue(e.ClickedItem, _addPdfObjectMenuItems, _pdfTypeForAddObjectMenuItems)
			);
		}
	}
}
