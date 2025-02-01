using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace PDFPatcher.Common
{
	static class XmlHelper
	{
		const string BooleanYes = "yes";
		const string BooleanNo = "no";

		[DebuggerStepThrough]
		public static bool GetValue(this XmlElement element, string name, bool defaultValue) {
			if (element == null) {
				return defaultValue;
			}
			var a = element.GetAttributeNode(name);
			return a != null ? a.Value.ToBoolean(defaultValue) : defaultValue;
		}

		/// <summary>获取 <paramref name="element"/> 元素名称为 <paramref name="name"/> 的属性值，如不存在该属性，或属性不能解析为整数值，则返回 <paramref name="defaultValue"/>。</summary>
		/// <param name="element">需要获取属性值的元素。</param>
		/// <param name="name">属性的名称。</param>
		/// <param name="defaultValue">属性的默认值。</param>
		/// <returns>属性的值；如不存在该属性，返回默认值。</returns>
		[DebuggerStepThrough]
		public static int GetValue(this XmlElement element, string name, int defaultValue) {
			if (element == null) {
				return defaultValue;
			}
			var a = element.GetAttributeNode(name);
			return a != null ? a.Value.ToInt32(defaultValue) : defaultValue;
		}

		[DebuggerStepThrough]
		public static long GetValue(this XmlElement element, string name, long defaultValue) {
			if (element == null) {
				return defaultValue;
			}
			var a = element.GetAttributeNode(name);
			return a != null ? a.Value.ToInt64(defaultValue) : defaultValue;
		}

		[DebuggerStepThrough]
		public static float GetValue(this XmlElement element, string name, float defaultValue) {
			if (element == null) {
				return defaultValue;
			}
			var a = element.GetAttributeNode(name);
			return a != null ? a.Value.ToSingle(defaultValue) : defaultValue;
		}

		[DebuggerStepThrough]
		public static double GetValue(this XmlElement element, string name, double defaultValue) {
			if (element == null) {
				return defaultValue;
			}
			var a = element.GetAttributeNode(name);
			if (a == null) {
				return defaultValue;
			}
			return a.Value.ToDouble(defaultValue);
		}
		[DebuggerStepThrough]
		public static bool GetValue(this XmlReader reader, string name, bool defaultValue) {
			if (reader == null) {
				return defaultValue;
			}
			var a = reader.GetAttribute(name);
			return a?.ToBoolean(defaultValue) ?? defaultValue;
		}
		[DebuggerStepThrough]
		public static int GetValue(this XmlReader reader, string name, int defaultValue) {
			if (reader == null) {
				return defaultValue;
			}
			var a = reader.GetAttribute(name);
			return a?.ToInt32(defaultValue) ?? defaultValue;
		}
		[DebuggerStepThrough]
		public static float GetValue(this XmlReader reader, string name, float defaultValue) {
			if (reader == null) {
				return defaultValue;
			}
			var a = reader.GetAttribute(name);
			return a?.ToSingle(defaultValue) ?? defaultValue;
		}

		[DebuggerStepThrough]
		public static string GetValue(this XmlElement element, string name) {
			return element?.GetAttributeNode(name)?.Value;
		}

		[DebuggerStepThrough]
		public static string GetValue(this XmlElement element, string name, string defaultValue) {
			return element?.GetAttributeNode(name)?.Value ?? defaultValue;
		}
		[DebuggerStepThrough]
		public static void SetValue(this XmlElement element, string name, bool value, bool defaultValue) {
			if (element == null) { return; }
			if (value == defaultValue) {
				element.RemoveAttribute(name);
			}
			else {
				element.SetAttribute(name, value ? BooleanYes : BooleanNo);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this XmlElement element, string name, int value, int defaultValue) {
			if (element == null) { return; }
			if (value == defaultValue) {
				element.RemoveAttribute(name);
			}
			else {
				element.SetAttribute(name, value.ToText());
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this XmlElement element, string name, float value, float defaultValue) {
			if (element == null) { return; }
			if (value == defaultValue) {
				element.RemoveAttribute(name);
			}
			else {
				element.SetAttribute(name, value.ToText());
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this XmlElement element, string name, string value) {
			if (element == null) { return; }
			if (string.IsNullOrEmpty(value)) {
				element.RemoveAttribute(name);
			}
			else {
				element.SetAttribute(name, value);
			}
		}

		[DebuggerStepThrough]
		public static void SetValue(this XmlElement element, string name, string value, string defaultValue) {
			if (element == null) { return; }
			if (value == null || value == defaultValue) {
				element.RemoveAttribute(name);
			}
			else {
				element.SetAttribute(name, value);
			}
		}
		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, bool value) {
			writer?.WriteAttributeString(name, value ? BooleanYes : BooleanNo);
		}

		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, bool value, bool defaultValue) {
			if (writer != null && value != defaultValue) {
				writer.WriteAttributeString(name, value ? BooleanYes : BooleanNo);
			}
		}

		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, int value) {
			writer?.WriteAttributeString(name, value.ToText());
		}

		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, int value, int defaultValue) {
			if (writer != null && value != defaultValue) {
				writer.WriteAttributeString(name, value.ToText());
			}
		}

		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, float value) {
			writer?.WriteAttributeString(name, value.ToText());
		}

		[DebuggerStepThrough]
		public static void WriteValue(this XmlWriter writer, string name, string value, string defaultValue) {
			if (writer != null && string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase) == false) {
				writer.WriteAttributeString(name, value);
			}
		}

		public static XmlElement GetOrCreateElement(this XmlNode parent, string name) {
			return parent == null
				? null
				: GetElement(parent, name) ?? parent.AppendElement(name);
		}
		public static XmlElement GetElement(this XmlNode parent, string name) {
			if (parent == null) {
				return null;
			}
			var n = parent.FirstChild;
			while (n != null) {
				if (n.NodeType == XmlNodeType.Element && n.Name == name) {
					return n as XmlElement;
				}
				n = n.NextSibling;
			}
			return null;
		}
		[DebuggerStepThrough]
		public static XmlElement AppendElement(this XmlNode element, string name) {
			if (element == null) {
				return null;
			}
			var d = element.NodeType != XmlNodeType.Document ? element.OwnerDocument : element as XmlDocument;
			var e = d.CreateElement(name);
			element.AppendChild(e);
			return e;
		}

		public static XmlNode[] ToXmlNodeArray(this XmlNodeList nodes) {
			if (nodes == null) {
				return Empty<XmlNode>.Item;
			}
			var a = new XmlNode[nodes.Count];
			var i = -1;
			foreach (XmlNode item in nodes) {
				a[++i] = item;
			}
			return a;
		}
		public static IList<TNode> ToNodeList<TNode>(this XmlNodeList nodes) where TNode : XmlNode {
			if (nodes == null) {
				return Empty<TNode>.Item;
			}
			var a = new List<TNode>(7);
			foreach (var item in nodes) {
				if (item is TNode n) {
					a.Add(n);
				}
			}
			return a;
		}

		public static IEnumerable<XmlElement> ChildrenOrFollowingSiblings(this XmlElement element) {
			return new ChildrenOrFollowingElementEnumerator(element);
		}

		static class Empty<TNode>
		{
			public static readonly TNode[] Item = new TNode[0];
		}

		sealed class ChildrenOrFollowingElementEnumerator(XmlElement baseElement) : IEnumerable<XmlElement>, IEnumerator<XmlElement>
		{
			readonly XmlElement _base = baseElement;
			XmlNode _active = baseElement;

			public XmlElement Current => _active as XmlElement;
			object IEnumerator.Current => _active;

			public void Dispose() {}

			public IEnumerator<XmlElement> GetEnumerator() {
				return this;
			}

			public bool MoveNext() {
				return _active != null
					&& (_active.HasChildNodes && TryGetFirstChildElement(ref _active)
						|| TryGetFirstFollowingElement(ref _active));
			}

			static bool TryGetFirstChildElement(ref XmlNode active) {
				var c = active.FirstChild;
				do {
					if (c.NodeType == XmlNodeType.Element) {
						active = c;
						return true;
					}
				}
				while ((c = c.NextSibling) != null);
				return false;
			}

			static bool TryGetFirstFollowingElement(ref XmlNode active) {
				XmlNode s;
				while ((s = active.NextSibling) != null) {
					if (s.NodeType == XmlNodeType.Element) {
						active = s;
						return true;
					}
				}
				return false;
			}

			public void Reset() {
				_active = _base;
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return this;
			}
		}
	}
}
