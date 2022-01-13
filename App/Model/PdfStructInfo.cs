using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PDFPatcher.Model;

internal struct PdfStructInfo
{
	private static readonly Dictionary<string, PdfStructInfo> _Info = InitStructInfo();
	private readonly string _Name;
	private readonly bool _IsKeyObject;
	private readonly bool _IsRequired;
	private readonly string _Description;
	private readonly string _ImageKey;

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
		if (_Info.TryGetValue(string.Concat(context, "/", name), out i)) {
			return i;
		}
		else {
			_Info.TryGetValue(name, out i);
			return i;
		}
	}

	private static Dictionary<string, PdfStructInfo> InitStructInfo() {
		Dictionary<string, PdfStructInfo> d = new();
		using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly()
			       .GetManifestResourceStream("PDFPatcher.Model.PDFStructInfo.xml")) {
			XmlDocument doc = new();
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

		string currentToken = element.GetAttribute("Token");
		foreach (XmlNode item in element.ChildNodes) {
			XmlElement e = item as XmlElement;
			if (e == null) {
				continue;
			}

			string t = e.GetAttribute("Token");
			if (string.IsNullOrEmpty(t)) {
				continue;
			}

			if (e.Name == "Info") {
				AddItem(d, string.IsNullOrEmpty(currentToken) ? t : string.Concat(currentToken, "/", t),
					new PdfStructInfo(e.GetAttribute("Name"), e.HasChildNodes, e.GetAttribute("Required") == "true",
						e.GetAttribute("Description"), e.GetAttribute("ImageKey")));
				AddSubItems(d, e);
			}
			else if (e.Name == "RefInfo" && d.ContainsKey(t)) {
				AddItem(d, string.IsNullOrEmpty(currentToken) ? t : string.Concat(currentToken, "/", t), d[t]);
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