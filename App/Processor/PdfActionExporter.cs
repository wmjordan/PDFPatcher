using System;
using System.Collections.Generic;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal sealed class PdfActionExporter
	{
		readonly Common.UnitConverter _unitConverter;

		public PdfActionExporter(Common.UnitConverter unitConverter) {
			_unitConverter = unitConverter;
		}

		internal void ExportFileLocation(XmlWriter w, PdfObject file) {
			switch (file.Type) {
				case PdfObject.DICTIONARY:
					var fs = file as PdfDictionary;
					if (fs.Contains(PdfName.UF)) {
						w.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(fs.GetAsString(PdfName.UF).ToUnicodeString()));
					}
					else if (fs.Contains(PdfName.F)) {
						file = fs.Get(PdfName.F);
					}
					break;
				case PdfObject.STRING:
					w.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(((PdfString)file).Decode(System.Text.Encoding.Default)));
					break;
			}
		}

		internal void ExportAction(PdfDictionary action, Dictionary<string, PdfObject> names, Dictionary<int, int> pageRefMap, XmlWriter target) {
			PdfObject dest;
			if (action == null) {
				return;
			}

			var actionType = PdfReader.GetPdfObjectRelease(action.Get(PdfName.S));
			if (PdfName.GOTO.Equals(actionType)) {
				dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
				if (dest != null) {
					ExportGotoAction(dest, names, target, pageRefMap);
				}
			}
			else if (PdfName.URI.Equals(actionType)) {
				target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Uri);
				target.WriteAttributeString(Constants.DestinationAttributes.Path, PdfHelper.GetValidXmlString(action.Locate<PdfString>(PdfName.URI).ToUnicodeString()));
			}
			else if (PdfName.GOTOR.Equals(actionType)) {
				target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.GotoR);
				dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
				if (dest != null) {
					if (dest.IsString()) {
						target.WriteAttributeString(Constants.DestinationAttributes.Named, PdfHelper.GetValidXmlString(dest.ToString()));
					}
					else if (dest.IsName()) {
						target.WriteAttributeString(Constants.DestinationAttributes.NamedN, PdfName.DecodeName(dest.ToString()));
					}
					else if (dest.IsArray()) {
						var arr = (PdfArray)dest;
						if (arr.Size > 0 && arr[0].IsNumber()) {
							target.WriteAttributeString(Constants.DestinationAttributes.Page, PdfHelper.GetValidXmlString(((arr[0] as PdfNumber).IntValue + 1).ToText()));
							ExportDestinationView(target, arr);
						}
					}
				}
				var file = action.Locate<PdfObject>(PdfName.F) ?? action.Locate<PdfObject>(PdfName.WIN);
				if (file != null) {
					ExportFileLocation(target, file);
				}
				var newWindow = action.Locate<PdfBoolean>(PdfName.NEWWINDOW);
				if (newWindow != null)
					target.WriteAttributeString(Constants.DestinationAttributes.NewWindow, newWindow.BooleanValue ? Constants.Boolean.True : Constants.Boolean.False);
			}
			else if (PdfName.LAUNCH.Equals(actionType)) {
				target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Launch);
				var file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F)) ?? PdfReader.GetPdfObjectRelease(action.Get(PdfName.WIN));
				if (file != null) {
					ExportFileLocation(target, file);
				}
			}
			else if (PdfName.JAVASCRIPT.Equals(actionType)) {
				target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Javascript);
				target.WriteAttributeString(Constants.DestinationAttributes.ScriptContent, PdfReader.GetPdfObjectRelease(action.Get(PdfName.JS)).ToString());
			}
		}

		internal void ExportGotoAction(PdfObject dest, Dictionary<string, PdfObject> names, XmlWriter target, Dictionary<int, int> pages) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
			string name;
			if (dest.Type == PdfObject.STRING) {
				name = StringHelper.ReplaceControlAndBomCharacters((dest as PdfString).ToUnicodeString());
				if (names.ContainsKey(name) == false) {
					return;
				}
				target.WriteAttributeString(Constants.DestinationAttributes.Named, name);
			}
			else if (dest.Type == PdfObject.NAME) {
				name = PdfName.DecodeName(dest.ToString());
				if (names.ContainsKey(name) == false) {
					return;
				}
				target.WriteAttributeString(Constants.DestinationAttributes.Named, name);
			}
			else if (dest.Type == PdfObject.ARRAY) {
				var a = dest as PdfArray;
				if (a.Size > 0) {
					var p = a[0];
					int pn = 0;
					if (p.Type == PdfObject.INDIRECT && pages.TryGetValue(GetNumber((PdfIndirectReference)a[0]), out pn)) {
						// use pn
					}
					else if (p.Type == PdfObject.NUMBER) {
						pn = (p as PdfNumber).IntValue + 1;
					}
					if (pn > 0) {
						target.WriteAttributeString(Constants.DestinationAttributes.Page, pn.ToText());
					}
				}
				ExportDestinationView(target, a);
			}
		}

		private void ExportDestinationView(XmlWriter target, PdfArray dest) {
			if (dest.Size < 2) {
				return;
			}
			if (dest[1] is not PdfName pn) {
				return;
			}
			var m = PdfHelper.GetPdfFriendlyName(pn);
			target.WriteAttributeString(Constants.DestinationAttributes.View, m);
			var p = new string[dest.Size - 2];
			PdfObject o;
			for (int i = 0; i < p.Length; i++) {
				o = dest[i + 2];
				if (o == null) {
					p[i] = String.Empty;
				}
				p[i] = (o.Type == PdfObject.NUMBER) ? (m == Constants.DestinationAttributes.ViewType.XYZ && i == 2
							? ((PdfNumber)o).FloatValue
							: _unitConverter.FromPoint(((PdfNumber)o).FloatValue)).ToText(
						)
					: (o.Type == PdfObject.NULL) ? Constants.Coordinates.Unchanged
					: o.ToString();
			}
			switch (m) {
				case Constants.DestinationAttributes.ViewType.XYZ:
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
				case Constants.DestinationAttributes.ViewType.Fit:
				case Constants.DestinationAttributes.ViewType.FitB:
					break;
				case Constants.DestinationAttributes.ViewType.FitBH:
				case Constants.DestinationAttributes.ViewType.FitH:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Top, p[0]);
					break;
				case Constants.DestinationAttributes.ViewType.FitV:
				case Constants.DestinationAttributes.ViewType.FitBV:
					if (p.Length < 1) {
						goto default;
					}
					target.WriteAttributeString(Constants.Coordinates.Left, p[0]);
					break;
				case Constants.DestinationAttributes.ViewType.FitR:
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

		private static int GetNumber(PdfIndirectReference indirect) {
			if (PdfReader.GetPdfObjectRelease(indirect) is not PdfDictionary pdfObj) {
				return 0;
			}
			if (pdfObj.Contains(PdfName.TYPE)
				&& PdfName.PAGES.Equals(pdfObj.GetAsName(PdfName.TYPE))
				&& pdfObj.Contains(PdfName.KIDS)) {
				var kids = (PdfArray)pdfObj.Get(PdfName.KIDS);
				indirect = (PdfIndirectReference)kids[0];
			}
			return indirect.Number;
		}

	}
}
