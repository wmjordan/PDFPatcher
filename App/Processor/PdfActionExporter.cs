using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using iTextSharp.text.pdf;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

internal sealed class PdfActionExporter
{
	private readonly UnitConverter _unitConverter;

	public PdfActionExporter(UnitConverter unitConverter) {
		_unitConverter = unitConverter;
	}

	internal void ExportFileLocation(XmlWriter w, PdfObject file) {
		switch (file.Type) {
			case PdfObject.DICTIONARY: {
					PdfDictionary fs = file as PdfDictionary;
					if (fs.Contains(PdfName.UF)) {
						w.WriteAttributeString(Constants.DestinationAttributes.Path,
							PdfHelper.GetValidXmlString(fs.GetAsString(PdfName.UF).ToUnicodeString()));
					}
					else if (fs.Contains(PdfName.F)) {
						file = fs.Get(PdfName.F);
					}

					break;
				}
			case PdfObject.STRING:
				w.WriteAttributeString(Constants.DestinationAttributes.Path,
					PdfHelper.GetValidXmlString(((PdfString)file).Decode(Encoding.Default)));
				break;
		}
	}

	internal void ExportAction(PdfDictionary action, Dictionary<int, int> pageRefMap, XmlWriter target) {
		PdfObject dest;
		if (action == null) {
			return;
		}

		PdfObject actionType = PdfReader.GetPdfObjectRelease(action.Get(PdfName.S));
		if (PdfName.GOTO.Equals(actionType)) {
			dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
			if (dest != null) {
				ExportGotoAction(dest, target, pageRefMap);
			}
		}
		else if (PdfName.URI.Equals(actionType)) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Uri);
			target.WriteAttributeString(Constants.DestinationAttributes.Path,
				PdfHelper.GetValidXmlString(action.Locate<PdfString>(PdfName.URI).ToUnicodeString()));
		}
		else if (PdfName.GOTOR.Equals(actionType)) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.GotoR);
			dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
			if (dest != null) {
				if (dest.IsString()) {
					target.WriteAttributeString(Constants.DestinationAttributes.Named,
						PdfHelper.GetValidXmlString(dest.ToString()));
				}
				else if (dest.IsName()) {
					target.WriteAttributeString(Constants.DestinationAttributes.NamedN,
						PdfName.DecodeName(dest.ToString()));
				}
				else if (dest.IsArray()) {
					PdfArray arr = (PdfArray)dest;
					if (arr.Size > 0 && arr[0].IsNumber()) {
						target.WriteAttributeString(Constants.DestinationAttributes.Page,
							PdfHelper.GetValidXmlString(((arr[0] as PdfNumber).IntValue + 1).ToText()));
						ExportDestinationView(target, arr);
					}
				}
			}

			PdfObject file = action.Locate<PdfObject>(PdfName.F) ?? action.Locate<PdfObject>(PdfName.WIN);
			if (file != null) {
				ExportFileLocation(target, file);
			}

			PdfBoolean newWindow = action.Locate<PdfBoolean>(PdfName.NEWWINDOW);
			if (newWindow != null) {
				target.WriteAttributeString(Constants.DestinationAttributes.NewWindow,
					newWindow.BooleanValue ? Constants.Boolean.True : Constants.Boolean.False);
			}
		}
		else if (PdfName.LAUNCH.Equals(actionType)) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Launch);
			PdfObject file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F)) ??
							 PdfReader.GetPdfObjectRelease(action.Get(PdfName.WIN));
			if (file != null) {
				ExportFileLocation(target, file);
			}
		}
		else if (PdfName.JAVASCRIPT.Equals(actionType)) {
			target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Javascript);
			target.WriteAttributeString(Constants.DestinationAttributes.ScriptContent,
				PdfReader.GetPdfObjectRelease(action.Get(PdfName.JS)).ToString());
		}
	}

	internal void ExportGotoAction(PdfObject dest, XmlWriter target, Dictionary<int, int> pages) {
		target.WriteAttributeString(Constants.DestinationAttributes.Action, Constants.ActionType.Goto);
		switch (dest.Type) {
			case PdfObject.STRING:
				target.WriteAttributeString(Constants.DestinationAttributes.Named,
					StringHelper.ReplaceControlAndBomCharacters((dest as PdfString).ToUnicodeString()));
				break;
			case PdfObject.NAME:
				target.WriteAttributeString(Constants.DestinationAttributes.Named, PdfName.DecodeName(dest.ToString()));
				break;
			case PdfObject.ARRAY: {
					PdfArray a = dest as PdfArray;
					if (a.Size > 0) {
						PdfObject p = a[0];
						int pn = 0;
						switch (p.Type) {
							case PdfObject.INDIRECT when pages.TryGetValue(GetNumber((PdfIndirectReference)a[0]), out pn):
								// use pn
								break;
							case PdfObject.NUMBER:
								pn = (p as PdfNumber).IntValue + 1;
								break;
						}

						if (pn > 0) {
							target.WriteAttributeString(Constants.DestinationAttributes.Page, pn.ToText());
						}
					}

					ExportDestinationView(target, a);
					break;
				}
		}
	}

	private void ExportDestinationView(XmlWriter target, PdfArray dest) {
		if (dest.Size < 2) {
			return;
		}

		if (dest[1] is not PdfName pn) {
			return;
		}

		string m = PdfHelper.GetPdfFriendlyName(pn);
		target.WriteAttributeString(Constants.DestinationAttributes.View, m);
		string[] p = new string[dest.Size - 2];
		for (int i = 0; i < p.Length; i++) {
			PdfObject o = dest[i + 2];
			if (o == null) {
				p[i] = string.Empty;
			}

			p[i] = o.Type switch {
				PdfObject.NUMBER => (m == Constants.DestinationAttributes.ViewType.XYZ && i == 2
					? ((PdfNumber)o).FloatValue
					: _unitConverter.FromPoint(((PdfNumber)o).FloatValue)).ToText(
				),
				PdfObject.NULL => Constants.Coordinates.Unchanged,
				_ => o.ToString()
			};
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
				Trace.WriteLine("目标位置无效");
				break;
		}
	}

	private static int GetNumber(PdfIndirectReference indirect) {
		PdfDictionary pdfObj = (PdfDictionary)PdfReader.GetPdfObjectRelease(indirect);
		if (pdfObj == null) {
			return 0;
		}

		if (!pdfObj.Contains(PdfName.TYPE) || !PdfName.PAGES.Equals(pdfObj.GetAsName(PdfName.TYPE)) ||
			!pdfObj.Contains(PdfName.KIDS)) {
			return indirect.Number;
		}

		PdfArray kids = (PdfArray)pdfObj.Get(PdfName.KIDS);
		indirect = (PdfIndirectReference)kids[0];

		return indirect.Number;
	}
}