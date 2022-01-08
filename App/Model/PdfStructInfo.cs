using System;
using System.Collections.Generic;
using System.Xml;
namespace PDFPatcher.Model
{
	struct PdfStructInfo
	{
		static readonly Dictionary<string, PdfStructInfo> _Info = InitStructInfo();
		readonly string _Name;
		readonly bool _IsKeyObject;
		readonly bool _IsRequired;
		readonly string _Description;
		readonly string _ImageKey;

		public string Name => _Name;
		public bool IsKeyObject => _IsKeyObject;
		public bool IsRequired => _IsRequired;
		public string Description => _Description;
		public string ImageKey => _ImageKey;

		public PdfStructInfo(string name, bool isKeyObject) : this(name, isKeyObject, false, null, null) {
		}
		public PdfStructInfo(string name, bool isKeyObject, bool isRequired, string description, string imageKey) {
			_Name = name;
			_IsKeyObject = isKeyObject;
			_IsRequired = isRequired;
			_Description = description;
			_ImageKey = imageKey;
		}
		internal static PdfStructInfo GetInfo(string context, string name) {
			PdfStructInfo i;
			if (_Info.TryGetValue(String.Concat(context, "/", name), out i)) {
				return i;
			}
			else {
				_Info.TryGetValue(name, out i);
				return i;
			}
		}

		static Dictionary<string, PdfStructInfo> InitStructInfo() {
			var d = new Dictionary<string, PdfStructInfo>();
			using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PDFPatcher.Model.PDFStructInfo.xml")) {
				var doc = new XmlDocument();
				doc.Load(s);
				AddSubItems(d, doc.SelectSingleNode("PDF/Global") as XmlElement);
				AddSubItems(d, doc.SelectSingleNode("PDF") as XmlElement);
			}
			return d;
		}

		private static void AddSubItems(Dictionary<string, PdfStructInfo> d, XmlElement element) {
			if (element.HasChildNodes == false) {
				return;
			}
			var currentToken = element.GetAttribute("Token");
			foreach (XmlNode item in element.ChildNodes) {
				var e = item as XmlElement;
				if (e == null) {
					continue;
				}
				var t = e.GetAttribute("Token");
				if (String.IsNullOrEmpty(t)) {
					continue;
				}
				if (e.Name == "Info") {
					AddItem(d, String.IsNullOrEmpty(currentToken) ? t : String.Concat(currentToken, "/", t), new PdfStructInfo(e.GetAttribute("Name"), e.HasChildNodes, e.GetAttribute("Required") == "true", e.GetAttribute("Description"), e.GetAttribute("ImageKey")));
					AddSubItems(d, e);
				}
				else if (e.Name == "RefInfo" && d.ContainsKey(t)) {
					AddItem(d, String.IsNullOrEmpty(currentToken) ? t : String.Concat(currentToken, "/", t), d[t]);
				}
			}
		}

		private static void AddItem(Dictionary<string, PdfStructInfo> d, string key, PdfStructInfo item) {
			if (d.ContainsKey(key)) {
				System.Diagnostics.Debug.WriteLine("已添加 " + key);
				return;
			}
			d.Add(key, item);
		}

	}
}
