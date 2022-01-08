using System;
using System.Windows.Forms;
using System.Xml;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions
{
	public partial class ActionEditorForm : System.Windows.Forms.Form
	{
		const string KeepZoomRate = "保持不变";
		const string NoAction = "无";
		public BookmarkElement Action { get; private set; }
		internal UndoActionGroup UndoActions { get; private set; }

		public ActionEditorForm(BookmarkElement element) {
			InitializeComponent();
			Action = element;
			_ActionBox.Items.AddRange(Constants.ActionType.Names);
			_ActionBox.Items.Add(NoAction);
			_ZoomRateBox.Items.AddRange(Constants.DestinationAttributes.ViewType.Names);
			_ZoomRateBox.Items.AddRange(new string[] { "————————", "4", "3", "2", "1.5", "1.3", "1.2", "1", "0", "0.9", "0.8", "0.5", "0.3" });

			int i = Array.IndexOf(Constants.ActionType.Names, element.GetAttribute(Constants.DestinationAttributes.Action));
			_ActionBox.SelectedIndex = (i != -1 ? i : 0);
			if (_ActionBox.SelectedIndex == 0 && element.HasAttribute(Constants.DestinationAttributes.Page) == false && element.HasAttribute(Constants.DestinationAttributes.Named) == false) {
				_ActionBox.SelectedItem = NoAction;
				_DestinationPanel.Enabled = false;
			}
			_DefaultOpenBox.Checked = element.IsOpen;
			i = Array.IndexOf(Constants.DestinationAttributes.ViewType.Names, element.GetAttribute(Constants.DestinationAttributes.View));
			_ZoomRateBox.SelectedIndex = (i != -1 ? i : 0);
			i = _ZoomRateBox.FindString(Constants.DestinationAttributes.ViewType.XYZ);
			if (i != -1) {
				_ZoomRateBox.Items[i] = KeepZoomRate;
			}

			if (_ZoomRateBox.Text == Constants.DestinationAttributes.ViewType.XYZ
				&& element.GetAttribute(Constants.Coordinates.ScaleFactor).TryParse(out float f)) {
				_ZoomRateBox.SelectedIndex = -1;
				_ZoomRateBox.Text = f.ToText();
			}
			_TitleBox.Text = element.GetAttribute(Constants.BookmarkAttributes.Title);
			_PathBox.Text = element.GetAttribute(Constants.DestinationAttributes.Path);
			_NewWindowBox.Checked = element.GetAttribute(Constants.DestinationAttributes.NewWindow) == Constants.Boolean.True;
			_NamedBox.Text = element.GetAttribute(Constants.DestinationAttributes.Named);
			_GotoNamedDestBox.Checked = String.IsNullOrEmpty(_NamedBox.Text) == false;
			_GotoLocationBox.Checked = element.HasAttribute(Constants.DestinationAttributes.Named) == false
				&& element.HasAttribute(Constants.DestinationAttributes.NamedN) == false;

			InitCoordinateValue(element, Constants.DestinationAttributes.Page, _PageBox, null);
			InitCoordinateValue(element, Constants.Coordinates.Left, _LeftBox, _KeepXBox);
			InitCoordinateValue(element, Constants.Coordinates.Top, _TopBox, _KeepYBox);
			InitCoordinateValue(element, Constants.Coordinates.Right, _WidthBox, null);
			_ScriptContentBox.Text = element.GetAttribute(Constants.DestinationAttributes.ScriptContent);
			if (_WidthBox.Enabled) {
				var v = _WidthBox.Value - _LeftBox.Value;
				if (v > _WidthBox.Maximum) {
					v = _WidthBox.Maximum;
				}
				else if (v < _WidthBox.Minimum) {
					v = _WidthBox.Minimum;
				}
				_WidthBox.Value = v;
			}
			InitCoordinateValue(element, Constants.Coordinates.Bottom, _HeightBox, null);
			if (_HeightBox.Enabled) {
				var v = _TopBox.Value - _HeightBox.Value;
				if (v > _HeightBox.Maximum) {
					v = _HeightBox.Maximum;
				}
				else if (v < _HeightBox.Minimum) {
					v = _HeightBox.Minimum;
				}
				_HeightBox.Value = v;
			}
			_AttrNameColumn.AspectGetter = (object o) => o is XmlAttribute attr ? attr.Name : (object)null;
			_AttrValueColumn.AspectGetter = (object o) => {
				if (o is XmlAttribute attr) {
					if (attr.Name == Constants.Font.ThisName && attr.Value.TryParse(out int fid)) {
						var n = attr.OwnerDocument.DocumentElement.SelectSingleNode(
							String.Concat(Constants.Font.DocumentFont, "/", Constants.Font.ThisName,
							"[@", Constants.Font.ID, "='", attr.Value, "']/@", Constants.Font.Name)
							);
						if (n != null) {
							return String.Concat(attr.Value, " (", n.Value, ")");
						}
					}
					return attr.Value;
				}
				return null;
			};
			_AttributesBox.SetObjects(element.Attributes);
		}

		void InitCoordinateValue(XmlElement element, string name, NumericUpDown control, CheckBox check) {
			if (element.HasAttribute(name)) {
				var s = element.GetAttribute(name);
				if (s.TryParse(out decimal x)) {
					control.SetValue(x);
				}
				else if (check != null) {
					check.Checked = true;
				}
			}
			else if (check != null) {
				check.Checked = true;
			}
		}

		void SetValue(string name, string value) {
			if (UndoActions == null) {
				UndoActions = new UndoActionGroup();
			}
			bool a = Action.HasAttribute(name);
			if ((value == null && a == false)
				|| (a && Action.GetAttribute(name) == value)) {
				return;
			}
			UndoActions.Add(UndoAttributeAction.GetUndoAction(Action, name, value));
		}

		void _OkButton_Click(object source, EventArgs args) {
			if (String.IsNullOrEmpty(_TitleBox.Text) == false) {
				SetValue(Constants.BookmarkAttributes.Title, _TitleBox.Text);
			}
			var act = _ActionBox.SelectedItem as string;
			if (act == NoAction) {
				act = null;
			}
			SetValue(Constants.DestinationAttributes.Action, act);
			SetValue(Constants.BookmarkAttributes.Open, _DefaultOpenBox.Checked ? Constants.Boolean.True : null);
			if (act == null) {
				SetValue(Constants.DestinationAttributes.Page, null);
			}
			else if (_ScriptBox.Visible) {
				SetValue(Constants.DestinationAttributes.ScriptContent, _ScriptContentBox.Text);
			}
			else if (_GotoLocationBox.Checked) {
				SetValue(Constants.DestinationAttributes.Page, _PageBox.Value.ToText());
				if (_ZoomRateBox.Text.TryParse(out float f)) {
					SetValue(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
					SetValue(Constants.Coordinates.ScaleFactor, f.ToText());
				}
				else if (_ZoomRateBox.Text == KeepZoomRate) {
					SetValue(Constants.DestinationAttributes.View, Constants.DestinationAttributes.ViewType.XYZ);
					SetValue(Constants.Coordinates.ScaleFactor, null);
				}
				else {
					SetValue(Constants.DestinationAttributes.View, _ZoomRateBox.Text);
				}
				if (_LeftBox.Enabled || _KeepXBox.Enabled) {
					SetValue(Constants.Coordinates.Left, _KeepXBox.Checked ? null : _LeftBox.Value.ToText());
				}
				if (_TopBox.Enabled || _KeepYBox.Enabled) {
					SetValue(Constants.Coordinates.Top, _KeepYBox.Checked ? null : _TopBox.Value.ToText());
				}
				if (_RectanglePanel.Enabled) {
					SetValue(Constants.Coordinates.Right, (_LeftBox.Value + _WidthBox.Value).ToText());
					SetValue(Constants.Coordinates.Bottom, (_TopBox.Value - _HeightBox.Value).ToText());
				}
			}
			else if (_GotoNamedDestBox.Checked) {
				SetValue(Constants.DestinationAttributes.Named, _NamedBox.Text);
			}
			if (_PathPanel.Enabled) {
				SetValue(Constants.DestinationAttributes.Path, _PathBox.Text);
				if (_NewWindowBox.Enabled) {
					SetValue(Constants.DestinationAttributes.NewWindow, _NewWindowBox.Checked ? Constants.Boolean.True : Constants.Boolean.False);
				}
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		void _CancelButton_Click(Object source, EventArgs args) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		void Control_ValueChanged(object sender, EventArgs e) {
			if (sender == _ActionBox) {
				var i = _ActionBox.SelectedItem as string;
				if (i == Constants.ActionType.Javascript) {
					_ScriptBox.Parent = _DestinationPanel.Parent;
					_ScriptBox.Top = _DestinationPanel.Top;
					_ScriptBox.Left = _DestinationPanel.Left;
					_ScriptBox.Size = _DestinationPanel.Size;
					_ScriptBox.Visible = true;
					_DestinationPanel.Visible = false;
				}
				else {
					_DestinationPanel.Visible = true;
					_DestinationPanel.Enabled = i != NoAction && i != Constants.ActionType.Javascript;
				}
				if (_DestinationPanel.Enabled) {
					_NewWindowBox.Enabled = ValueHelper.IsInCollection(i, Constants.ActionType.GotoR, Constants.ActionType.Uri);
					_PathPanel.Enabled = ValueHelper.IsInCollection(i, Constants.ActionType.GotoR, Constants.ActionType.Launch, Constants.ActionType.Uri);
				}
			}
			else if (sender == _GotoLocationBox || sender == _GotoNamedDestBox) {
				_LocationPanel.Enabled = _GotoLocationBox.Checked;
				_NamedBox.Enabled = _GotoNamedDestBox.Checked;
			}
			else if (sender == _KeepXBox) {
				_LeftBox.Enabled = !_KeepXBox.Checked;
			}
			else if (sender == _KeepYBox) {
				_TopBox.Enabled = !_KeepYBox.Checked;
			}
			else if (sender == _ZoomRateBox) {
				switch (_ZoomRateBox.Text) {
					case Constants.DestinationAttributes.ViewType.XYZ:
					case "保持不变":
						goto default;
					case Constants.DestinationAttributes.ViewType.Fit:
					case Constants.DestinationAttributes.ViewType.FitB:
						_TopBox.Enabled = _LeftBox.Enabled = _KeepXBox.Enabled = _KeepYBox.Enabled = _RectanglePanel.Enabled = false;
						break;
					case Constants.DestinationAttributes.ViewType.FitBH:
					case Constants.DestinationAttributes.ViewType.FitH:
						_TopBox.Enabled = _KeepYBox.Enabled = true;
						_LeftBox.Enabled = _KeepXBox.Enabled = _RectanglePanel.Enabled = false;
						break;
					case Constants.DestinationAttributes.ViewType.FitBV:
					case Constants.DestinationAttributes.ViewType.FitV:
						_LeftBox.Enabled = _KeepXBox.Enabled = true;
						_TopBox.Enabled = _KeepYBox.Enabled = _RectanglePanel.Enabled = false;
						break;
					case Constants.DestinationAttributes.ViewType.FitR:
						_TopBox.Enabled = _LeftBox.Enabled = _RectanglePanel.Enabled = true;
						_KeepXBox.Enabled = _KeepYBox.Enabled = false;
						break;
					default:
						_TopBox.Enabled = _LeftBox.Enabled = _KeepXBox.Enabled = _KeepYBox.Enabled = true;
						_RectanglePanel.Enabled = false;
						break;
				}
			}
		}
	}
}
