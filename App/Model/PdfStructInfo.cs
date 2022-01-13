﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PDFPatcher.Model;

internal struct PdfStructInfo
{
	private static readonly Dictionary<string, PdfStructInfo> _Info = InitStructInfo();

	public string Name { get; }

	public bool IsKeyObject { get; }

	public bool IsRequired { get; }

	public string Description { get; }

	public string ImageKey { get; }

	public PdfStructInfo(string name, bool isKeyObject) : this(name, isKeyObject, false, null, null) {
	}

	public PdfStructInfo(string name, bool isKeyObject, bool isRequired, string description, string imageKey) {
		Name = name;
		IsKeyObject = isKeyObject;
		IsRequired = isRequired;
		Description = description;
		ImageKey = imageKey;
	}

	internal static PdfStructInfo GetInfo(string context, string name) {
		PdfStructInfo i;
		if (_Info.TryGetValue(string.Concat(context, "/", name), out i)) {
			return i;
		}

		_Info.TryGetValue(name, out i);
		return i;
	}

	private static Dictionary<string, PdfStructInfo> InitStructInfo() {
		Dictionary<string, PdfStructInfo> d = new();
		using (Stream s = Assembly.GetExecutingAssembly()
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
			Debug.WriteLine("已添加 " + key);
			return;
		}

		d.Add(key, item);
	}
}