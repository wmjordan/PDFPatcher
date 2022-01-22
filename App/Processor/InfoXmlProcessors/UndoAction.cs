using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PDFPatcher.Processor;

internal interface IUndoAction
{
	IEnumerable<XmlNode> AffectedElements { get; }
	bool Undo();
}

internal sealed class UndoManager
{
	public delegate void OnUndoDelegate(UndoManager undoManager, IUndoAction action);

	private readonly List<string> _names = new();
	private readonly Stack<IUndoAction> _undoActions = new();

	public OnUndoDelegate OnAddUndo = null;
	public OnUndoDelegate OnUndo = null;

	public bool CanUndo => _names.Count > 0;

	public void Clear() {
		_names.Clear();
		_undoActions.Clear();
	}

	public void AddUndo(string name, IUndoAction action) {
		if (action == null) {
			return;
		}

		_names.Add(name);
		_undoActions.Push(action);
		OnAddUndo?.Invoke(this, action);
	}

	public IList<string> GetActionNames(int limit) {
		string[] n = new string[_names.Count > limit ? limit : _names.Count];
		int j = 0;
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

		IUndoAction a = _undoActions.Pop();
		a.Undo();
		OnUndo?.Invoke(this, a);
		return a.AffectedElements;
	}
}

internal sealed class UndoActionGroup : IUndoAction
{
	private readonly List<IUndoAction> _actions = new();

	public int Count => _actions.Count;

	public void Add(IUndoAction action) {
		switch (action) {
			case null:
				return;
			case UndoActionGroup g:
				_actions.AddRange(g._actions);
				break;
			default:
				_actions.Add(action);
				break;
		}
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
			Dictionary<XmlNode, byte> d = new();
			foreach (XmlNode e in _actions.SelectMany(item => item.AffectedElements)) {
				d[e] = 0;
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

internal abstract class UndoElementAction : IUndoAction
{
	protected UndoElementAction(XmlElement target) {
		TargetElement = target ?? throw new ArgumentNullException("undo/element/target");
		Parent = target.ParentNode;
	}

	public XmlNode Parent { get; }
	public XmlElement TargetElement { get; }

	#region IUndoAction 成员

	public IEnumerable<XmlNode> AffectedElements => new[] { Parent };
	public abstract bool Undo();

	#endregion
}

internal sealed class RemoveElementAction : UndoElementAction
{
	public RemoveElementAction(XmlElement target) : base(target) {
	}

	public override bool Undo() {
		TargetElement.ParentNode.RemoveChild(TargetElement);
		return true;
	}
}

internal sealed class AddElementAction : UndoElementAction
{
	public AddElementAction(XmlElement target) : base(target) {
		RefNode = target.NextSibling;
	}

	public XmlNode RefNode { get; }

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

internal abstract class UndoAttributeAction : IUndoAction
{
	protected UndoAttributeAction(XmlElement targetNode, string name) {
		if (string.IsNullOrEmpty(name)) {
			throw new ArgumentNullException("undo/attr/name");
		}

		TargetElement = targetNode ?? throw new ArgumentNullException("undo/attr/target");
		Name = name;
	}

	public XmlElement TargetElement { get; }
	public string Name { get; }

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
	///     设置目标元素的属性值，并返回撤销动作。
	/// </summary>
	/// <param name="targetNode">需要修改的元素节点。</param>
	/// <param name="name">属性名称。</param>
	/// <param name="newValue">新属性值。</param>
	/// <returns>撤销设置属性的动作。</returns>
	internal static UndoAttributeAction GetUndoAction(XmlElement targetNode, string name, string newValue) {
		if (targetNode.HasAttribute(name)) {
			string v = targetNode.GetAttribute(name);
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

		if (newValue != null) {
			targetNode.SetAttribute(name, newValue);
		}

		return new RemoveAttributeAction(targetNode, name);
	}

	#region IUndoAction 成员

	public IEnumerable<XmlNode> AffectedElements => new XmlNode[] { TargetElement };
	public abstract bool Undo();

	#endregion
}

internal sealed class RemoveAttributeAction : UndoAttributeAction
{
	public RemoveAttributeAction(XmlElement targeNode, string name) : base(targeNode, name) {
	}

	public override bool Undo() {
		TargetElement.RemoveAttribute(Name);
		return true;
	}
}

internal sealed class SetAttributeAction : UndoAttributeAction
{
	public SetAttributeAction(XmlElement targeNode, string name, string value) : base(targeNode, name) {
		Value = value;
	}

	public string Value { get; }

	public override bool Undo() {
		TargetElement.SetAttribute(Name, Value);
		return true;
	}
}