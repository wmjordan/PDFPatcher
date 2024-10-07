using System;
using System.Collections.Generic;
using System.Xml;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	readonly struct PdfStructInfo(string name, bool isKeyObject, bool isRequired, string description, string imageKey)
	{
		static readonly Dictionary<string, PdfStructInfo> _Info = InitStructInfo();

		public string Name { get; } = name;
		public bool IsKeyObject { get; } = isKeyObject;
		public bool IsRequired { get; } = isRequired;
		public string Description { get; } = description;
		public string ImageKey { get; } = imageKey;

		public PdfStructInfo(string name, bool isKeyObject) : this(name, isKeyObject, false, null, null) {
		}

		internal static PdfStructInfo GetInfo(string context, string name) {
			PdfStructInfo i;
			if (!_Info.TryGetValue($"{context}/{name}", out i)) {
				_Info.TryGetValue(name, out i);
			}
			return i;
		}

		static Dictionary<string, PdfStructInfo> InitStructInfo() {
			var d = new Dictionary<string, PdfStructInfo>();
			using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PDFPatcher.Model.PDFStructInfo.xml")) {
				var doc = new XmlDocument();
				doc.Load(s);
				AddSubItems(d, doc.GetElement("PDF").GetElement("Global"));
				AddSubItems(d, doc.GetElement("PDF"));
			}
			return d;
		}

		static void AddSubItems(Dictionary<string, PdfStructInfo> d, XmlElement element) {
			if (element.HasChildNodes == false) {
				return;
			}
			var currentToken = element.GetAttribute("Token");
			foreach (XmlNode item in element.ChildNodes) {
				if (item is not XmlElement e) {
					continue;
				}
				var t = e.GetAttribute("Token");
				if (String.IsNullOrEmpty(t)) {
					continue;
				}
				if (e.Name == "Info") {
					AddItem(d, String.IsNullOrEmpty(currentToken) ? t : $"{currentToken}/{t}", new PdfStructInfo(e.GetAttribute("Name"), e.HasChildNodes, e.GetAttribute("Required") == "true", e.GetAttribute("Description"), e.GetAttribute("ImageKey")));
					AddSubItems(d, e);
				}
				else if (e.Name == "RefInfo" && d.TryGetValue(t, out var s)) {
					AddItem(d, String.IsNullOrEmpty(currentToken) ? t : $"{currentToken}/{t}", s);
				}
			}
		}

		static void AddItem(Dictionary<string, PdfStructInfo> d, string key, PdfStructInfo item) {
			if (d.ContainsKey(key)) {
				System.Diagnostics.Debug.WriteLine("已添加 " + key);
				return;
			}
			d.Add(key, item);
		}

	}
}
