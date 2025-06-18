using System;
using System.Xml;
using MuPDF;
using MuPDF.Extensions;
using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	partial class PdfActionExporter
	{
		internal void ExportGotoAction(Document pdf, PdfObject dest, PdfDictionary names, XmlWriter target) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
			string name;
			if (dest.IsString) {
				name = StringHelper.ReplaceControlAndBomCharacters((dest as PdfString).Value);
			}
			else if (dest.IsName) {
				name = dest.ToString();
			}
			else {
				goto ARRAY;
			}
			target.WriteAttributeString(Constants.DestinationAttributes.Named, name);
			dest = names.GetObject(name);
			if (dest.IsNull) {
				return;
			}
			if (dest.IsDictionary) {
				dest = ((PdfDictionary)dest).GetObject(PdfNames.D);
			}
			// dest 可能是 Array，转到下面处理
		ARRAY:
			if (dest.IsArray) {
				var a = (PdfArray)dest;
				if (a.Count > 0) {
					var p = a[0];
					int pn = 0;
					if (p.IsIndirect) {
						pn = pdf.LookupPageNumber(a[0]) + 1;
					}
					else if (p.IsNumber) {
						pn = p.IntegerValue + 1;
					}
					if (pn > 0) {
						target.WriteAttributeString(Constants.DestinationAttributes.Page, pn.ToText());
					}
				}
				ExportDestinationView(target, a);
			}
		}

		void ExportDestinationView(XmlWriter target, PdfArray dest) {
			if (dest.Count < 2) {
				return;
			}
			if (dest[1] is not PdfName pn) {
				return;
			}
			var m = GetDestFriendlyName(pn.PredefinedValue) ?? "无效";
			target.WriteAttributeString(Constants.DestinationAttributes.View, m);
			var p = new string[dest.Count - 2];
			PdfObject o;
			for (int i = 0; i < p.Length; i++) {
				o = dest[i + 2];
				p[i] = (o.IsNumber) ? (pn.PredefinedValue == PdfNames.XYZ && i == 2
							? o.FloatValue
							: _unitConverter.FromPoint(o.FloatValue)).ToText()
					: (o.IsNull) ? Constants.Coordinates.Unchanged
					: o.ToString();
			}
			switch (pn.PredefinedValue) {
				case PdfNames.XYZ:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Left, p[0]);
					if (p.Length > 1) {
						target.WriteAttributeString(Constants.Coordinates.Top, p[1]);
					}
					if (p.Length > 2) {
						target.WriteAttributeString(Constants.Coordinates.ScaleFactor, p[2]);
					}
					break;
				case PdfNames.Fit:
				case PdfNames.FitB:
					break;
				case PdfNames.FitBH:
				case PdfNames.FitH:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Top, p[0]);
					break;
				case PdfNames.FitV:
				case PdfNames.FitBV:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Left, p[0]);
					break;
				case PdfNames.FitR:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Left, p[0]);
					if (p.Length > 1) {
						target.WriteAttributeString(Constants.Coordinates.Bottom, p[1]);
					}
					if (p.Length > 2) {
						target.WriteAttributeString(Constants.Coordinates.Right, p[2]);
					}
					if (p.Length > 3) {
						target.WriteAttributeString(Constants.Coordinates.Top, p[3]);
					}
					break;
				default:
					System.Diagnostics.Trace.WriteLine("目标位置无效");
					break;
			}
		}

		internal static string GetDestFriendlyName(PdfNames destName) {
			return destName switch {
				PdfNames.XYZ => Constants.DestinationAttributes.ViewType.XYZ,
				PdfNames.Fit => Constants.DestinationAttributes.ViewType.Fit,
				PdfNames.FitB => Constants.DestinationAttributes.ViewType.FitB,
				PdfNames.FitBH => Constants.DestinationAttributes.ViewType.FitBH,
				PdfNames.FitBV => Constants.DestinationAttributes.ViewType.FitBV,
				PdfNames.FitV => Constants.DestinationAttributes.ViewType.FitV,
				PdfNames.FitH => Constants.DestinationAttributes.ViewType.FitH,
				PdfNames.FitR => Constants.DestinationAttributes.ViewType.FitR,
				_ => null
			};
		}

		internal void ExportAction(Document pdf, PdfDictionary action, PdfDictionary names, XmlWriter target) {
			PdfObject dest;
			if (action is null) {
				return;
			}
			var actionType = (action[PdfNames.S] as PdfName)?.PredefinedValue ?? PdfNames.GoTo;
			switch (actionType) {
				case PdfNames.GoTo:
					dest = action.GetObject(PdfNames.D);
					if (!dest.IsNull) {
						ExportGotoAction(pdf, dest, names, target);
					}
					return;
				case PdfNames.URI:
					target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Uri);
					target.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(action.Get<PdfString>(PdfNames.URI)?.Value));
					return;
				case PdfNames.GoToR:
					target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.GotoR);
					dest = action[PdfNames.D];
					if (dest.IsString) {
						target.WriteAttributeString(Constants.DestinationAttributes.Named, PdfHelper.GetValidXmlString(dest.ToString()));
					}
					else if (dest.IsName) {
						target.WriteAttributeString(Constants.DestinationAttributes.NamedN, PdfHelper.GetValidXmlString(dest.ToString()));
					}
					else if (dest.IsArray) {
						var arr = (PdfArray)dest;
						if (arr.Count > 0 && arr[0].IsNumber) {
							target.WriteAttributeString(Constants.DestinationAttributes.Page, PdfHelper.GetValidXmlString((arr[0].IntegerValue + 1).ToText()));
							ExportDestinationView(target, arr);
						}
					}
					var file = GetFile(action);
					if (file.IsNull == false) {
						ExportFileLocation(target, file);
					}
					var newWindow = action.Get<PdfBoolean>(PdfNames.NewWindow);
					if (newWindow is not null) {
						target.WriteAttributeString(Constants.DestinationAttributes.NewWindow, newWindow.Value ? Constants.Boolean.True : Constants.Boolean.False);
					}
					return;
				case PdfNames.Launch:
					target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Launch);
					file = GetFile(action);
					if (file.IsNull == false) {
						ExportFileLocation(target, file);
					}
					return;
				case PdfNames.JavaScript:
					target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Javascript);
					target.WriteAttributeString(Constants.DestinationAttributes.ScriptContent, action.Get<PdfString>(PdfNames.JS)?.Value);
					return;
			}
		}

		internal void ExportFileLocation(XmlWriter w, PdfObject file) {
			switch (file.TypeKind) {
				case Kind.Dictionary:
					var fs = file as PdfDictionary;
					if (fs.ContainsKey(PdfNames.UF)) {
						w.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(fs.Get<PdfString>(PdfNames.UF)?.Value));
					}
					else if (fs.ContainsKey(PdfNames.F)) {
						w.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(fs.Get<PdfString>(PdfNames.F)?.Value));
					}
					break;
				case Kind.String:
					w.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(((PdfString)file).Decode(System.Text.Encoding.Default)));
					break;
			}
		}

		static PdfObject GetFile(PdfDictionary action) {
			var r = action.GetValue(PdfNames.F).UnderlyingObject;
			return !r.IsNull
				? r
				: action.GetValue("Win").UnderlyingObject;
		}
	}
}