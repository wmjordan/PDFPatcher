using System;
using System.Collections.Generic;
using System.Xml;

namespace PDFPatcher.Processor
{
	interface IUndoAction
	{
		IEnumerable<XmlNode> AffectedElements { get; }
		bool Undo();
	}

	sealed class UndoManager
	{
		readonly Stack<IUndoAction> _undoActions = new();
		readonly List<string> _names = [];
		int _CleanLevel;

		public delegate void OnUndoDelegate(UndoManager undoManager, IUndoAction action);
		public OnUndoDelegate OnAddUndo;
		public OnUndoDelegate OnUndo;

		public bool CanUndo => _names.Count > 0;

		public bool IsDirty => _CleanLevel != _undoActions.Count;

		public void MarkClean() {
			_CleanLevel = _undoActions.Count;
		}

		public void Clear() {
			_names.Clear();
			_undoActions.Clear();
			_CleanLevel = 0;
		}

		public void AddUndo(string name, IUndoAction action) {
			if (action == null) {
				return;
			}
			_names.Add(name);
			_undoActions.Push(action);
			OnAddUndo?.Invoke(this, action);
		}

		public string[] GetActionNames(int limit) {
			var n = new string[_names.Count > limit ? limit : _names.Count];
			var j = 0;
			for (int i = n.Length - 1; i >= 0; i--) {
				n[j++] = _names[i];
			}
			return n;
		}

		public IEnumerable<XmlNode> Undo() {
			if (!CanUndo) {
				return null;
			}
			_names.RemoveAt(_names.Count - 1);
			if (_names.Count > 100 && _names.Capacity > 200) {
				_names.TrimExcess();
			}
			var a = _undoActions.Pop();
			a.Undo();
			OnUndo?.Invoke(this, a);
			return a.AffectedElements;
		}
	}

	sealed class UndoActionGroup : IUndoAction
	{
		readonly List<IUndoAction> _actions = new();

		public int Count => _actions.Count;

		public UndoActionGroup() {
		}

		public UndoActionGroup(IEnumerable<IUndoAction> actions) {
			_actions.AddRange(actions);
		}

		public void Add(IUndoAction action) {
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

		public void RemoveElement(XmlElement target) {
			Add(new RemoveElementAction(target));
		}
		public void AddElement(XmlElement target) {
			Add(new AddElementAction(target));
		}
		public void SetAttribute(XmlElement targetNode, string name, string newValue) {
			Add(UndoAttributeAction.GetUndoAction(targetNode, name, newValue));
		}
		public void RemoveAttribute(XmlElement target, string name) {
			Add(UndoAttributeAction.GetUndoAction(target, name, null));
		}
		#region IUndoAction 成员

		public IEnumerable<XmlNode> AffectedElements {
			get {
				var d = new Dictionary<XmlNode, byte>();
				foreach (var item in _actions) {
					foreach (var e in item.AffectedElements) {
						d[e] = 0;
					}
				}
				return d.Keys;
			}
		}
		public bool Undo() {
			for (int i = _actions.Count - 1; i >= 0; i--) {
				_actions[i].Undo();
			}
			return true;
		}

		#endregion
	}

	abstract class UndoElementAction(XmlElement target) : IUndoAction
	{
		public XmlNode Parent { get; } = target.ParentNode;
		public XmlElement TargetElement { get; } = target ?? throw new ArgumentNullException("undo/element/target");

		#region IUndoAction 成员

		public IEnumerable<XmlNode> AffectedElements => [Parent];
		public abstract bool Undo();

		#endregion
	}

	sealed class RemoveElementAction(XmlElement target) : UndoElementAction(target)
	{
		public override bool Undo() {
			TargetElement.ParentNode.RemoveChild(TargetElement);
			return true;
		}
	}

	sealed class AddElementAction(XmlElement target) : UndoElementAction(target)
	{
		public XmlNode RefNode { get; } = target.NextSibling;

		public override bool Undo() {
			if (RefNode == null) {
				Parent.AppendChild(TargetElement);
			}
			else {
				Parent.InsertBefore(TargetElement, RefNode);
			}
			return true;
		}
	}

	abstract class UndoAttributeAction : IUndoAction
	{
		public XmlElement TargetElement { get; }
		public string Name { get; }

		protected UndoAttributeAction(XmlElement targetNode, string name) {
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("undo/attr/name");
			}
			TargetElement = targetNode ?? throw new ArgumentNullException("undo/attr/target");
			Name = name;
		}

		/// <summary>
		/// 设置目标元素的属性值，并返回撤销动作。
		/// </summary>
		/// <param name="targetNode">需要修改的元素节点。</param>
		/// <param name="name">属性名称。</param>
		/// <param name="newValue">新属性值。</param>
		/// <returns>撤销设置属性的动作。</returns>
		internal static UndoAttributeAction GetUndoAction(XmlElement targetNode, string name, string newValue) {
			if (targetNode.HasAttribute(name)) {
				var v = targetNode.GetAttribute(name);
				if (v == newValue) {
					return null;
				}
				if (newValue != null) {
					targetNode.SetAttribute(name, newValue);
				}
				else {
					targetNode.RemoveAttribute(name);
				}
				return new SetAttributeAction(targetNode, name, v);
			}
			else if (newValue != null) {
				targetNode.SetAttribute(name, newValue);
			}
			return new RemoveAttributeAction(targetNode, name);
		}

		#region IUndoAction 成员
		public IEnumerable<XmlNode> AffectedElements => [TargetElement];
		public abstract bool Undo();
		#endregion
	}

	sealed class RemoveAttributeAction(XmlElement targetNode, string name) : UndoAttributeAction(targetNode, name)
	{
		public override bool Undo() {
			TargetElement.RemoveAttribute(Name);
			return true;
		}
	}

	sealed class SetAttributeAction(XmlElement targetNode, string name, string value) : UndoAttributeAction(targetNode, name)
	{
		public string Value { get; } = value;

		public override bool Undo() {
			TargetElement.SetAttribute(Name, Value);
			return true;
		}
	}
}
