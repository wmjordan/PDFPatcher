using System;
using System.Xml;
using System.Collections.Generic;

namespace PDFPatcher.Processor
{
	interface IUndoAction
	{
		IEnumerable<XmlNode> AffectedElements { get; }
		bool Undo ();
	}

	sealed class UndoManager
	{
		readonly Stack<IUndoAction> _undoActions = new Stack<IUndoAction> ();
		readonly List<string> _names = new List<string> ();
		
		public delegate void OnUndoDelegate (UndoManager undoManager, IUndoAction action);

		public bool CanUndo => _names.Count > 0;

		public OnUndoDelegate OnAddUndo = null;
		public OnUndoDelegate OnUndo = null;

		public void Clear () {
			_names.Clear ();
			_undoActions.Clear ();
		}

		public void AddUndo (string name, IUndoAction action) {
			if (action == null) {
				return;
			}
			_names.Add (name);
			_undoActions.Push (action);
			OnAddUndo?.Invoke(this, action);
		}

		public IList<string> GetActionNames (int limit) {
			var n = new string[_names.Count > limit ? limit : _names.Count];
			var j = 0;
			for (int i = n.Length - 1; i >= 0; i--) {
				n[j++] = _names[i];
			}
			return n;
		}

		public IEnumerable<XmlNode> Undo () {
			if (CanUndo) {
				_names.RemoveAt (_names.Count - 1);
				if (_names.Count > 100 && _names.Capacity > 200) {
					_names.TrimExcess();
				}
				var a = _undoActions.Pop ();
				a.Undo ();
				OnUndo?.Invoke(this, a);
				return a.AffectedElements;
			}
			return null;
		}

	}

	sealed class UndoActionGroup : IUndoAction
	{
		readonly List<IUndoAction> _actions = new List<IUndoAction> ();

		public int Count => _actions.Count;

		public UndoActionGroup () {
		}

		public UndoActionGroup (IEnumerable<IUndoAction> actions) {
			_actions.AddRange (actions);
		}

		public void Add (IUndoAction action) {
			if (action == null) {
				return;
			}
			if (action is UndoActionGroup g) {
				_actions.AddRange(g._actions);
			}
			else {
				_actions.Add(action);
			}
		}

		public void RemoveElement (XmlElement target) {
			Add (new RemoveElementAction (target));
		}
		public void AddElement (XmlElement target) {
			Add (new AddElementAction (target));
		}
		public void SetAttribute (XmlElement targetNode, string name, string newValue) {
			Add (UndoAttributeAction.GetUndoAction (targetNode, name, newValue));
		}
		public void RemoveAttribute (XmlElement target, string name) {
			Add (UndoAttributeAction.GetUndoAction (target, name, null));
		}
		#region IUndoAction 成员

		public IEnumerable<XmlNode> AffectedElements {
			get {
				var d = new Dictionary<XmlNode, byte> ();
				foreach (var item in _actions) {
					foreach (var e in item.AffectedElements) {
						d[e] = 0;
					}
				}
				return d.Keys;
			}
		}
		public bool Undo () {
			for (int i = _actions.Count - 1; i >= 0; i--) {
				_actions[i].Undo ();
			}
			return true;
		}

		#endregion
	}

	abstract class UndoElementAction : IUndoAction
	{
		public XmlNode Parent { get; private set; }
		public XmlElement TargetElement { get; private set; }

		protected UndoElementAction (XmlElement target) {
			TargetElement = target ?? throw new ArgumentNullException ("undo/element/target");
			Parent = target.ParentNode;
		}

		#region IUndoAction 成员

		public IEnumerable<XmlNode> AffectedElements {
			get { return new XmlNode[] { Parent }; }
		}
		public abstract bool Undo ();

		#endregion
	}

	sealed class RemoveElementAction : UndoElementAction
	{
		public RemoveElementAction (XmlElement target) : base (target) {
		}

		public override bool Undo () {
			TargetElement.ParentNode.RemoveChild (TargetElement);
			return true;
		}
	}

	sealed class AddElementAction : UndoElementAction
	{
		public XmlNode RefNode { get; private set; }

		public AddElementAction (XmlElement target) : base (target) {
			RefNode = target.NextSibling;
		}

		public override bool Undo () {
			if (RefNode == null) {
				Parent.AppendChild (TargetElement);
			}
			else {
				Parent.InsertBefore (TargetElement, RefNode);
			}
			return true;
		}
	}

	abstract class UndoAttributeAction : IUndoAction
	{
		public XmlElement TargetElement { get; private set; }
		public string Name { get; private set; }

		protected UndoAttributeAction (XmlElement targeNode, string name) {
			if (String.IsNullOrEmpty (name)) {
				throw new ArgumentNullException ("undo/attr/name");
			}
			TargetElement = targeNode ?? throw new ArgumentNullException ("undo/attr/target");
			Name = name;
		}

		//internal static UndoActionGroup GetUndoAttributeGroup (XmlElement targetNode, params string[] names) {
		//	var undoList = new UndoActionGroup ();
		//	foreach (var item in names) {
		//		if (targetNode.HasAttribute (item)) {
		//			undoList.Add (new SetAttributeAction (targetNode, item, targetNode.GetAttribute (item)));
		//		}
		//		else {
		//			undoList.Add (new RemoveAttributeAction (targetNode, item));
		//		}
		//	}
		//	return undoList;
		//}

		//internal static IUndoAction[] GetUndoListForAttributes (XmlElement targetNode, params string[] names) {
		//	var undoList = new IUndoAction[names.Length];
		//	var i = 0;
		//	foreach (var item in names) {
		//		if (targetNode.HasAttribute (item)) {
		//			undoList[i++] = new SetAttributeAction (targetNode, item, targetNode.GetAttribute (item));
		//		}
		//		else {
		//			undoList[i++] = new RemoveAttributeAction (targetNode, item);
		//		}
		//	}
		//	return undoList;
		//}

		/// <summary>
		/// 设置目标元素的属性值，并返回撤销动作。
		/// </summary>
		/// <param name="targetNode">需要修改的元素节点。</param>
		/// <param name="name">属性名称。</param>
		/// <param name="newValue">新属性值。</param>
		/// <returns>撤销设置属性的动作。</returns>
		internal static UndoAttributeAction GetUndoAction (XmlElement targetNode, string name, string newValue) {
			if (targetNode.HasAttribute (name)) {
				var v = targetNode.GetAttribute (name);
				if (v == newValue) {
					return null;
				}
				if (newValue != null) {
					targetNode.SetAttribute (name, newValue);
				}
				else {
					targetNode.RemoveAttribute (name);
				}
				return new SetAttributeAction (targetNode, name, v);
			}
			else if (newValue != null) {
				targetNode.SetAttribute (name, newValue);
			}
			return new RemoveAttributeAction (targetNode, name);
		}

		#region IUndoAction 成员
		public IEnumerable<XmlNode> AffectedElements {
			get { return new XmlNode[] { TargetElement }; }
		}
		public abstract bool Undo ();
		#endregion
	}

	sealed class RemoveAttributeAction : UndoAttributeAction
	{
		public RemoveAttributeAction (XmlElement targeNode, string name) : base (targeNode, name) {
		}

		public override bool Undo () {
			TargetElement.RemoveAttribute (Name);
			return true;
		}
	}

	sealed class SetAttributeAction : UndoAttributeAction
	{
		public string Value { get; private set; }

		public SetAttributeAction (XmlElement targeNode, string name, string value) : base (targeNode, name) {
			Value = value;
		}

		public override bool Undo () {
			TargetElement.SetAttribute (Name, Value);
			return true;
		}
	}
}
