using System;
using System.Collections.Generic;
using iTextSharp.text.error_messages;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor
{
	internal class PdfContentStreamProcessor
	{
		// Fields
		readonly Stack<Model.GraphicsState> gsStack = new Stack<Model.GraphicsState>();
		readonly Stack<MarkedContentInfo> markedContentStack = new Stack<MarkedContentInfo>();
		readonly IDictionary<string, IContentOperator> operators = new Dictionary<string, IContentOperator>();
		readonly Dictionary<PdfName, IXObjectDoHandler> _XObjectDoHandlers = new Dictionary<PdfName, IXObjectDoHandler>();
		readonly Dictionary<int, FontInfoCache> _FontCache = new Dictionary<int, FontInfoCache>();
		readonly Dictionary<int, string> _FontNameCache = new Dictionary<int, string>();
		//IRenderListener renderListener;
		ResourceDictionary _Resources;
		Matrix _TextLineMatrix;
		Matrix _TextMatrix;
		public readonly static IContentOperator NopOperator = new IgnoreOperatorContentOperator();

		internal PdfContentStreamProcessor() {
			Reset();
		}

		// Methods
		internal Model.GraphicsState CurrentGraphicState => gsStack.Peek();
		internal Matrix TextMatrix => _TextMatrix;
		internal IContentOperator DefaultOperator { get; set; }
		protected IDictionary<int, string> Fonts => _FontNameCache;
		protected ResourceDictionary Resource => _Resources;

		protected void ApplyTextAdjust(float tj) {
			float adjustBy = -tj / 1000f * CurrentGraphicState.FontSize * CurrentGraphicState.HorizontalScaling;
			_TextMatrix = new Matrix(adjustBy, 0f).Multiply(_TextMatrix);
		}

		protected void AdjustTextMatrixX(float x) {
			_TextMatrix = new Matrix(x, 0f).Multiply(_TextMatrix);
		}

		protected void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
			markedContentStack.Push(new MarkedContentInfo(tag, dict));
		}

		private void BeginText() {
			//this.renderListener.BeginTextBlock ();
		}
		private FontInfo GetFont(PRIndirectReference fontRef) {
			if (_FontCache.TryGetValue(fontRef.Number, out var r)) {
				r.Access++;
				return r.Info;
			}
			var f = new FontInfo(fontRef);
			_FontCache.Add(fontRef.Number, new FontInfoCache(f));
			_FontNameCache.Add(fontRef.Number, f.FontName);
			if (_FontCache.Count > 53) {
				var l = new List<KeyValuePair<int, FontInfoCache>>(_FontCache);
				l.Sort((x, y) => x.Value.Access - y.Value.Access);
				for (int i = 0; i < 10; i++) {
					int k = l[i].Key;
					_FontCache.Remove(k);
					_FontNameCache.Remove(k);
				}
			}
			return f;
		}

		protected virtual void DisplayPdfString(PdfString str) {
			//TextRenderInfo renderInfo = new TextRenderInfo (this.Decode (str), this.CurrentGraphicState, this.textMatrix, this.markedContentStack);
			//this.renderListener.RenderText (renderInfo);
			//this.textMatrix = new Matrix (renderInfo.GetUnscaledWidth (), 0f).Multiply (this.textMatrix);
		}

		protected virtual void DisplayXObject(PdfName xobjectName) {
			//IXObjectDoHandler handler;
			//PdfDictionary xobjects = this.resources.GetAsDict (PdfName.XOBJECT);
			//PdfObject xobject = xobjects.GetDirectObject (xobjectName);
			//PdfStream xobjectStream = (PdfStream)xobject;
			//PdfName subType = xobjectStream.GetAsName (PdfName.SUBTYPE);
			//if (!xobject.IsStream ()) {
			//    throw new InvalidOperationException (MessageLocalization.GetComposedMessage ("XObject.1.is.not.a.stream", xobjectName));
			//}
			//this.xobjectDoHandlers.TryGetValue (subType, out handler);
			//if (handler == null) {
			//    handler = this.xobjectDoHandlers[PdfName.DEFAULT];
			//}
			//handler.HandleXObject (this, xobjectStream, xobjects.GetAsIndirectObject (xobjectName));
		}

		private void EndMarkedContent() {
			markedContentStack.Pop();
		}

		private void EndText() {
			//this.renderListener.EndTextBlock ();
		}

		protected virtual void InvokeOperator(PdfLiteral oper, List<PdfObject> operands) {
			if (operators.TryGetValue(oper.ToString(), out IContentOperator op)) {
				op.Invoke(this, oper, operands);
			}
			else {
				DefaultOperator?.Invoke(this, oper, operands);
			}
		}

		protected void PopulateOperators() {
			RegisterContentOperator("q", new PushGraphicsState());
			RegisterContentOperator("Q", new PopGraphicsState());
			RegisterContentOperator("cm", new ModifyCurrentTransformationMatrix());
			RegisterContentOperator("gs", new ProcessGraphicsStateResource());
			var tcOperator = new SetTextCharacterSpacing();
			RegisterContentOperator("Tc", tcOperator);
			var twOperator = new SetTextWordSpacing();
			RegisterContentOperator("Tw", twOperator);
			RegisterContentOperator("Tz", new SetTextHorizontalScaling());
			var tlOperator = new SetTextLeading();
			RegisterContentOperator("TL", tlOperator);
			RegisterContentOperator("Tf", new SetTextFont());
			RegisterContentOperator("Tr", new SetTextRenderMode());
			RegisterContentOperator("Ts", new SetTextRise());
			RegisterContentOperator("BT", new BeginTextC());
			RegisterContentOperator("ET", new EndTextC());
			RegisterContentOperator("BMC", new BeginMarkedContentC());
			RegisterContentOperator("BDC", new BeginMarkedContentDictionary());
			RegisterContentOperator("EMC", new EndMarkedContentC());
			var tdOperator = new TextMoveStartNextLine();
			RegisterContentOperator("Td", tdOperator);
			RegisterContentOperator("TD", new TextMoveStartNextLineWithLeading(tdOperator, tlOperator));
			RegisterContentOperator("Tm", new TextSetTextMatrix());
			var tstarOperator = new TextMoveNextLine(tdOperator);
			RegisterContentOperator("T*", tstarOperator);
			var tjOperator = new ShowText();
			RegisterContentOperator("Tj", new ShowText());
			RegisterContentOperator("TJ", new ShowTextArray());
			var tickOperator = new MoveNextLineAndShowText(tstarOperator, tjOperator);
			RegisterContentOperator("'", tickOperator);
			RegisterContentOperator("\"", new MoveNextLineAndShowTextWithSpacing(twOperator, tcOperator, tickOperator));
			RegisterContentOperator("Do", new Do());
		}

		protected void PopulateXObjectDoHandlers() {
			RegisterXObjectDoHandler(PdfName.DEFAULT, new IgnoreXObjectDoHandler());
			RegisterXObjectDoHandler(PdfName.FORM, new FormXObjectDoHandler());
			RegisterXObjectDoHandler(PdfName.IMAGE, new ImageXObjectDoHandler());
		}

		public void ProcessContent(byte[] contentBytes, PdfDictionary resources) {
			if (contentBytes.HasContent() == false || resources == null) {
				return;
			}
			_Resources.Push(resources);
			var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(contentBytes));
			var ps = new PdfContentParser(tokenizer);
			var operands = new List<PdfObject>();
			while (ps.Parse(operands).Count > 0) {
				var oper = operands[operands.Count - 1] as PdfLiteral;
				if (oper is null) {
					break;
				}
				if ("BI".Equals(oper.ToString())) {
					var img = InlineImageUtils.ParseInlineImage(ps, resources.GetAsDict(PdfName.COLORSPACE));
					InvokeOperator(oper, new List<PdfObject> { img, oper });
					//    this.renderListener.RenderImage (renderInfo);
				}
				else {
					try {
						InvokeOperator(oper, operands);
					}
					catch (Exception) {
						continue;
					}
				}
			}
			tokenizer.Close();
			_Resources.Pop();
		}

		internal IContentOperator RegisterContentOperator(string operatorString, IContentOperator oper) {
			if (oper == null) {
				operators.Remove(operatorString);
				return null;
			}
			return operators[operatorString] = oper;
		}

		internal IXObjectDoHandler RegisterXObjectDoHandler(PdfName xobjectSubType, IXObjectDoHandler handler) {
			IXObjectDoHandler old;
			_XObjectDoHandlers.TryGetValue(xobjectSubType, out old);
			_XObjectDoHandlers[xobjectSubType] = handler;
			return old;
		}

		internal virtual void Reset() {
			gsStack.Clear();
			gsStack.Push(new Model.GraphicsState());
			_TextMatrix = null;
			_TextLineMatrix = null;
			_Resources = new ResourceDictionary();
		}

		#region Nested Types
		internal interface IContentOperator
		{
			bool HasOutput { get; }
			void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands);
		}
		internal interface IXObjectDoHandler
		{
			void HandleXObject(PdfContentStreamProcessor processor, PdfStream stream, PdfIndirectReference refi);
		}

		protected sealed class BeginMarkedContentC : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.BeginMarkedContent((PdfName)operands[0], new PdfDictionary());
			}
		}

		protected sealed class BeginMarkedContentDictionary : IContentOperator
		{
			public bool HasOutput => false;
			static PdfDictionary GetPropertiesDictionary(PdfObject operand1, ResourceDictionary resources) {
				return operand1.IsDictionary()
					? (PdfDictionary)operand1
					: resources.GetAsDict((PdfName)operand1);
			}

			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.BeginMarkedContent((PdfName)operands[0],
					GetPropertiesDictionary(operands[1], processor._Resources));
			}
		}

		protected sealed class BeginTextC : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor._TextMatrix = new Matrix();
				processor._TextLineMatrix = processor._TextMatrix;
				processor.BeginText();
			}
		}

		protected sealed class Do : IContentOperator
		{
			public bool HasOutput => true;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.DisplayXObject((PdfName)operands[0]);
			}
		}

		protected sealed class EndMarkedContentC : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.EndMarkedContent();
			}
		}

		protected sealed class EndTextC : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor._TextMatrix = null;
				processor._TextLineMatrix = null;
				processor.EndText();
			}
		}

		protected sealed class FormXObjectDoHandler : IXObjectDoHandler
		{
			// Methods
			public void HandleXObject(PdfContentStreamProcessor processor, PdfStream stream, PdfIndirectReference refi) {
				var resources = stream.GetAsDict(PdfName.RESOURCES);
				var contentBytes = ContentByteUtils.GetContentBytesFromContentObject(stream);
				var matrix = stream.GetAsArray(PdfName.MATRIX);
				new PushGraphicsState().Invoke(processor, null, null);
				if (matrix != null) {
					float a = matrix[0].ValueAsFloat();
					float b = matrix[1].ValueAsFloat();
					float c = matrix[2].ValueAsFloat();
					float d = matrix[3].ValueAsFloat();
					float e = matrix[4].ValueAsFloat();
					float f = matrix[5].ValueAsFloat();
					processor.CurrentGraphicState.TransMatrix = new Matrix(a, b, c, d, e, f).Multiply(processor.CurrentGraphicState.TransMatrix);
				}
				processor.ProcessContent(contentBytes, resources);
				new PopGraphicsState().Invoke(processor, null, null);
			}
		}

		protected sealed class IgnoreOperatorContentOperator : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			}
		}

		protected sealed class IgnoreXObjectDoHandler : IXObjectDoHandler
		{
			// Methods
			public void HandleXObject(PdfContentStreamProcessor processor, PdfStream xobjectStream, PdfIndirectReference refi) {
			}
		}

		protected sealed class ImageXObjectDoHandler : IXObjectDoHandler
		{
			// Methods
			public void HandleXObject(PdfContentStreamProcessor processor, PdfStream xobjectStream, PdfIndirectReference refi) {
				//ImageRenderInfo renderInfo = ImageRenderInfo.CreateForXObject (processor.CurrentGraphicState.TransMatrix, refi);
				//processor.renderListener.RenderImage (renderInfo);
			}
		}

		protected sealed class ModifyCurrentTransformationMatrix : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				float a = operands[0].ValueAsFloat();
				float b = operands[1].ValueAsFloat();
				float c = operands[2].ValueAsFloat();
				float d = operands[3].ValueAsFloat();
				float e = operands[4].ValueAsFloat();
				float f = operands[5].ValueAsFloat();
				var gs = processor.gsStack.Peek();
				gs.TransMatrix = new Matrix(a, b, c, d, e, f).Multiply(gs.TransMatrix);
			}
		}

		protected sealed class MoveNextLineAndShowText : IContentOperator
		{
			readonly IContentOperator showText;
			readonly IContentOperator textMoveNextLine;

			public bool HasOutput => true;
			public MoveNextLineAndShowText(IContentOperator textMoveNextLine, IContentOperator showText) {
				this.textMoveNextLine = textMoveNextLine;
				this.showText = showText;
			}

			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				textMoveNextLine.Invoke(processor, null, new List<PdfObject>(0));
				showText.Invoke(processor, null, operands);
			}
		}

		protected sealed class MoveNextLineAndShowTextWithSpacing : IContentOperator
		{
			readonly IContentOperator moveNextLineAndShowText;
			readonly IContentOperator setTextCharacterSpacing;
			readonly IContentOperator setTextWordSpacing;

			public bool HasOutput => true;
			public MoveNextLineAndShowTextWithSpacing(IContentOperator setTextWordSpacing, IContentOperator setTextCharacterSpacing, IContentOperator moveNextLineAndShowText) {
				this.setTextWordSpacing = setTextWordSpacing;
				this.setTextCharacterSpacing = setTextCharacterSpacing;
				this.moveNextLineAndShowText = moveNextLineAndShowText;
			}

			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var aw = (PdfNumber)operands[0];
				var ac = (PdfNumber)operands[1];
				var str = (PdfString)operands[2];
				var twOperands = new List<PdfObject>(1);
				twOperands.Insert(0, aw);
				setTextWordSpacing.Invoke(processor, null, twOperands);
				var tcOperands = new List<PdfObject>(1);
				tcOperands.Insert(0, ac);
				setTextCharacterSpacing.Invoke(processor, null, tcOperands);
				var tickOperands = new List<PdfObject>(1);
				tickOperands.Insert(0, str);
				moveNextLineAndShowText.Invoke(processor, null, tickOperands);
			}
		}

		protected sealed class PopGraphicsState : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				try {
					processor.gsStack.Pop();
				}
				catch (InvalidOperationException) {
					Tracker.DebugMessage("绘图状态堆栈为空。");
				}
			}
		}

		protected sealed class ProcessGraphicsStateResource : IContentOperator
		{
			public bool HasOutput => false;

			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var extGState = processor._Resources.GetAsDict(PdfName.EXTGSTATE)
					?? throw new ArgumentException(MessageLocalization.GetComposedMessage("resources.do.not.contain.extgstate.entry.unable.to.process.oper.1", oper));

				var dictionaryName = (PdfName)operands[0];
				var gsDic = extGState.GetAsDict(dictionaryName)
					?? throw new ArgumentException(MessageLocalization.GetComposedMessage("1.is.an.unknown.graphics.state.dictionary", dictionaryName));

				var fontParameter = gsDic.GetAsArray(PdfName.FONT);
				if (fontParameter != null) {
					var font = processor.GetFont((PRIndirectReference)fontParameter[0]);
					float size = fontParameter.GetAsNumber(1).FloatValue;
					processor.CurrentGraphicState.Font = font;
					processor.CurrentGraphicState.FontSize = size;
				}
			}
		}

		protected sealed class PushGraphicsState : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.gsStack.Push(processor.gsStack.Peek().Copy());
			}
		}

		protected sealed class ResourceDictionary : PdfDictionary
		{
			readonly IList<PdfDictionary> resourcesStack = new List<PdfDictionary>();

			public override PdfObject GetDirectObject(PdfName key) {
				for (int i = resourcesStack.Count - 1; i >= 0; i--) {
					var subResource = resourcesStack[i];
					if (subResource != null) {
						var obj = subResource.GetDirectObject(key);
						if (obj != null) {
							return obj;
						}
					}
				}
				return base.GetDirectObject(key);
			}

			public void Pop() {
				resourcesStack.RemoveAt(resourcesStack.Count - 1);
			}

			public void Push(PdfDictionary resources) {
				resourcesStack.Add(resources);
			}
		}

		protected sealed class SetTextCharacterSpacing : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.CharacterSpacing = ((PdfNumber)operands[0]).FloatValue;
			}
		}

		protected sealed class SetTextFont : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var fontResourceName = (PdfName)operands[0];
				var f = processor._Resources.GetAsDict(PdfName.FONT).Get(fontResourceName);
				var g = processor.CurrentGraphicState;
				if (f is PRIndirectReference fref) {
					g.FontID = fref.Number;
					g.Font = processor.GetFont(fref);
				}
				else {
					Tracker.DebugMessage($"字体（{fontResourceName}）不为引用。");
					g.FontID = 0;
					g.Font = new FontInfo((PdfDictionary)f, 0);
				}
				g.FontSize = ((PdfNumber)operands[1]).FloatValue;
			}
		}

		protected sealed class SetTextHorizontalScaling : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.HorizontalScaling = ((PdfNumber)operands[0]).FloatValue / 100f;
			}
		}

		protected sealed class SetTextLeading : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.Leading = ((PdfNumber)operands[0]).FloatValue;
			}
		}

		protected sealed class SetTextRenderMode : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.RenderMode = ((PdfNumber)operands[0]).IntValue;
			}
		}

		protected sealed class SetTextRise : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.Rise = ((PdfNumber)operands[0]).FloatValue;
			}
		}

		protected sealed class SetTextWordSpacing : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				processor.CurrentGraphicState.WordSpacing = ((PdfNumber)operands[0]).FloatValue;
			}
		}

		protected sealed class ShowText : IContentOperator
		{
			public bool HasOutput => true;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var str = (PdfString)operands[0];
				processor.DisplayPdfString(str);
			}
		}

		protected sealed class AccumulatedShowTextArray : IContentOperator
		{
			public bool HasOutput => true;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var array = (PdfArray)operands[0];
				float adj = 0;
				using (var ms = new System.IO.MemoryStream(array.Length)) {
					foreach (PdfObject item in array.ArrayList) {
						if (item.Type == PdfObject.STRING) {
							ms.Write((item as PdfString).GetBytes(), 0, item.Length);
						}
						else {
							adj += ((PdfNumber)item).FloatValue;
						}
					}
					processor.DisplayPdfString(new PdfString(ms.ToArray()));
				}
				processor.ApplyTextAdjust(adj);
			}
		}

		protected sealed class ShowTextArray : IContentOperator
		{
			public bool HasOutput => true;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				var array = (PdfArray)operands[0];
				foreach (PdfObject entryObj in array.ArrayList) {
					if (entryObj is PdfString s) {
						processor.DisplayPdfString(s);
					}
					else if (entryObj is PdfNumber n) {
						processor.ApplyTextAdjust(n.FloatValue);
					}
					else {
						// ignore
					}
				}
			}
		}

		protected sealed class TextMoveNextLine : IContentOperator
		{
			readonly TextMoveStartNextLine _MoveStartNextLine;

			public TextMoveNextLine(TextMoveStartNextLine moveStartNextLine) {
				_MoveStartNextLine = moveStartNextLine;
			}

			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				_MoveStartNextLine.Invoke(processor, null, new List<PdfObject>(2) {
					new PdfNumber(0),
					new PdfNumber(-processor.CurrentGraphicState.Leading)
				});
			}
		}

		protected sealed class TextMoveStartNextLine : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				float tx = ((PdfNumber)operands[0]).FloatValue;
				float ty = ((PdfNumber)operands[1]).FloatValue;
				processor._TextMatrix = new Matrix(tx, ty).Multiply(processor._TextLineMatrix);
				processor._TextLineMatrix = processor._TextMatrix;
			}
		}

		protected sealed class TextMoveStartNextLineWithLeading : IContentOperator
		{
			readonly TextMoveStartNextLine _MoveStartNextLine;
			readonly SetTextLeading _SetTextLeading;

			public TextMoveStartNextLineWithLeading(TextMoveStartNextLine moveStartNextLine, SetTextLeading setTextLeading) {
				_MoveStartNextLine = moveStartNextLine;
				_SetTextLeading = setTextLeading;
			}

			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				float ty = ((PdfNumber)operands[1]).FloatValue;
				_SetTextLeading.Invoke(processor, null, new List<PdfObject>(1) { new PdfNumber(-ty) });
				_MoveStartNextLine.Invoke(processor, null, operands);
			}
		}

		protected sealed class TextSetTextMatrix : IContentOperator
		{
			public bool HasOutput => false;
			public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
				float a = operands[0].ValueAsFloat();
				float b = operands[1].ValueAsFloat();
				float c = operands[2].ValueAsFloat();
				float d = operands[3].ValueAsFloat();
				float e = operands[4].ValueAsFloat();
				float f = operands[5].ValueAsFloat();
				processor._TextLineMatrix = new Matrix(a, b, c, d, e, f);
				processor._TextMatrix = processor._TextLineMatrix;
			}
		}

		#endregion

		internal static class InlineImageUtils
		{
			/**
			 * Simple class in case users need to differentiate an exception from processing
			 * inline images vs other exceptions 
			 * @since 5.0.4
			 */
			public sealed class InlineImageParseException : System.IO.IOException
			{
				public InlineImageParseException(String message)
					: base(message) {
				}
			}

			/**
			 * Map between key abbreviations allowed in dictionary of inline images and their
			 * equivalent image dictionary keys
			 */
			private static readonly IDictionary<PdfName, PdfName> inlineImageEntryAbbreviationMap;
			/**
			 * Map between value abbreviations allowed in dictionary of inline images for COLORSPACE
			 */
			private static readonly IDictionary<PdfName, PdfName> inlineImageColorSpaceAbbreviationMap;
			/**
			 * Map between value abbreviations allowed in dictionary of inline images for FILTER
			 */
			private static readonly IDictionary<PdfName, PdfName> inlineImageFilterAbbreviationMap;
			static InlineImageUtils() { // static initializer
				inlineImageEntryAbbreviationMap = new Dictionary<PdfName, PdfName> {

					// allowed entries - just pass these through
					[PdfName.BITSPERCOMPONENT] = PdfName.BITSPERCOMPONENT,
					[PdfName.COLORSPACE] = PdfName.COLORSPACE,
					[PdfName.DECODE] = PdfName.DECODE,
					[PdfName.DECODEPARMS] = PdfName.DECODEPARMS,
					[PdfName.FILTER] = PdfName.FILTER,
					[PdfName.HEIGHT] = PdfName.HEIGHT,
					[PdfName.IMAGEMASK] = PdfName.IMAGEMASK,
					[PdfName.INTENT] = PdfName.INTENT,
					[PdfName.INTERPOLATE] = PdfName.INTERPOLATE,
					[PdfName.WIDTH] = PdfName.WIDTH,

					// abbreviations - transform these to corresponding correct values
					[new PdfName("BPC")] = PdfName.BITSPERCOMPONENT,
					[new PdfName("CS")] = PdfName.COLORSPACE,
					[new PdfName("D")] = PdfName.DECODE,
					[new PdfName("DP")] = PdfName.DECODEPARMS,
					[new PdfName("F")] = PdfName.FILTER,
					[new PdfName("H")] = PdfName.HEIGHT,
					[new PdfName("IM")] = PdfName.IMAGEMASK,
					[new PdfName("I")] = PdfName.INTERPOLATE,
					[new PdfName("W")] = PdfName.WIDTH
				};

				inlineImageColorSpaceAbbreviationMap = new Dictionary<PdfName, PdfName> {
					[new PdfName("G")] = PdfName.DEVICEGRAY,
					[new PdfName("RGB")] = PdfName.DEVICERGB,
					[new PdfName("CMYK")] = PdfName.DEVICECMYK,
					[new PdfName("I")] = PdfName.INDEXED
				};

				inlineImageFilterAbbreviationMap = new Dictionary<PdfName, PdfName> {
					[new PdfName("AHx")] = PdfName.ASCIIHEXDECODE,
					[new PdfName("A85")] = PdfName.ASCII85DECODE,
					[new PdfName("LZW")] = PdfName.LZWDECODE,
					[new PdfName("Fl")] = PdfName.FLATEDECODE,
					[new PdfName("RL")] = PdfName.RUNLENGTHDECODE,
					[new PdfName("CCF")] = PdfName.CCITTFAXDECODE,
					[new PdfName("DCT")] = PdfName.DCTDECODE
				};
			}

			/**
			 * Parses an inline image from the provided content parser.  The parser must be positioned immediately following the BI operator in the content stream.
			 * The parser will be left with current position immediately following the EI operator that terminates the inline image
			 * @param ps the content parser to use for reading the image. 
			 * @return the parsed image
			 * @throws IOException if anything goes wring with the parsing
			 * @throws InlineImageParseException if parsing of the inline image failed due to issues specific to inline image processing
			 */
			public static PdfImageData ParseInlineImage(PdfContentParser ps, PdfDictionary colorSpaceDic) {
				var d = ParseInlineImageDictionary(ps);
				return new PdfImageData(d, ParseInlineImageSamples(d, colorSpaceDic, ps));
			}

			/**
			 * Parses the next inline image dictionary from the parser.  The parser must be positioned immediately following the EI operator.
			 * The parser will be left with position immediately following the whitespace character that follows the ID operator that ends the inline image dictionary.
			 * @param ps the parser to extract the embedded image information from
			 * @return the dictionary for the inline image, with any abbreviations converted to regular image dictionary keys and values
			 * @throws IOException if the parse fails
			 */
			private static PdfDictionary ParseInlineImageDictionary(PdfContentParser ps) {
				// by the time we get to here, we have already parsed the BI operator
				var dictionary = new PdfDictionary();

				for (PdfObject key = ps.ReadPRObject(); key != null && !"ID".Equals(key.ToString()); key = ps.ReadPRObject()) {
					var value = ps.ReadPRObject();

					PdfName resolvedKey;
					inlineImageEntryAbbreviationMap.TryGetValue((PdfName)key, out resolvedKey);
					resolvedKey ??= (PdfName)key;

					dictionary.Put(resolvedKey, GetAlternateValue(resolvedKey, value));
				}

				int ch = ps.GetTokeniser().Read();
				if (!PRTokeniser.IsWhitespace(ch))
					throw new System.IO.IOException("Unexpected character " + ch + " found after ID in inline image");

				return dictionary;
			}

			/**
			 * Transforms value abbreviations into their corresponding real value 
			 * @param key the key that the value is for
			 * @param value the value that might be an abbreviation
			 * @return if value is an allowed abbreviation for the key, the expanded value for that abbreviation.  Otherwise, value is returned without modification 
			 */
			private static PdfObject GetAlternateValue(PdfName key, PdfObject value) {
				if (key == PdfName.FILTER) {
					if (value is PdfName pdfName
						&& inlineImageFilterAbbreviationMap.TryGetValue(pdfName, out PdfName altValue)
						&& altValue != null) {
						return altValue;
					}
					else if (value is PdfArray array) {
						var altArray = new PdfArray();
						int count = array.Size;
						for (int i = 0; i < count; i++) {
							altArray.Add(GetAlternateValue(key, array[i]));
						}
						return altArray;
					}
				}
				else if (key == PdfName.COLORSPACE) {
					if (value is PdfName pdfName
						&& inlineImageColorSpaceAbbreviationMap.TryGetValue(pdfName, out PdfName altValue)
						&& altValue != null) {
						return altValue;
					}
				}

				return value;
			}

			/**
			 * @param colorSpaceName the name of the color space. If null, a bi-tonal (black and white) color space is assumed.
			 * @return the components per pixel for the specified color space
			 */
			private static int GetComponentsPerPixel(PdfName colorSpaceName, PdfDictionary colorSpaceDic) {
				if (colorSpaceName == null)
					return 1;
				if (colorSpaceName.Equals(PdfName.DEVICEGRAY))
					return 1;
				if (colorSpaceName.Equals(PdfName.DEVICERGB))
					return 3;
				if (colorSpaceName.Equals(PdfName.DEVICECMYK))
					return 4;

				if (colorSpaceDic != null) {
					var colorSpace = colorSpaceDic.GetAsArray(colorSpaceName);
					if (colorSpace != null && PdfName.INDEXED.Equals(colorSpace.GetAsName(0))) {
						return 1;
					}
				}

				throw new ArgumentException("Unexpected color space " + colorSpaceName);
			}

			/**
			 * Computes the number of unfiltered bytes that each row of the image will contain.
			 * If the number of bytes results in a partial terminating byte, this number is rounded up
			 * per the PDF specification
			 * @param imageDictionary the dictionary of the inline image
			 * @return the number of bytes per row of the image
			 */
			private static int ComputeBytesPerRow(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic) {
				var wObj = imageDictionary.GetAsNumber(PdfName.WIDTH);
				var bpcObj = imageDictionary.GetAsNumber(PdfName.BITSPERCOMPONENT);
				int cpp = GetComponentsPerPixel(imageDictionary.GetAsName(PdfName.COLORSPACE), colorSpaceDic);
				int w = wObj.IntValue;
				int bpc = bpcObj?.IntValue ?? 1;
				return (w * bpc * cpp + 7) / 8;
			}

			/**
			 * Parses the samples of the image from the underlying content parser, ignoring all filters.
			 * The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
			 * The parser will be left positioned immediately following the EI operator.
			 * This is primarily useful if no filters have been applied. 
			 * @param imageDictionary the dictionary of the inline image
			 * @param ps the content parser
			 * @return the samples of the image
			 * @throws IOException if anything bad happens during parsing
			 */
			private static byte[] ParseUnfilteredSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic, PdfContentParser ps) {
				// special case:  when no filter is specified, we just read the number of bits
				// per component, multiplied by the width and height.
				if (imageDictionary.Contains(PdfName.FILTER))
					throw new ArgumentException("Dictionary contains filters");

				var h = imageDictionary.GetAsNumber(PdfName.HEIGHT);

				int bytesToRead = ComputeBytesPerRow(imageDictionary, colorSpaceDic) * h.IntValue;
				var bytes = new byte[bytesToRead];
				var tokeniser = ps.GetTokeniser();

				int shouldBeWhiteSpace = tokeniser.Read(); // skip next character (which better be a whitespace character - I suppose we could check for this)
														   // from the PDF spec:  Unless the image uses ASCIIHexDecode or ASCII85Decode as one of its filters, the ID operator shall be followed by a single white-space character, and the next character shall be interpreted as the first byte of image data.
														   // unfortunately, we've seen some PDFs where there is no space following the ID, so we have to capture this case and handle it
				int startIndex = 0;
				if (!PRTokeniser.IsWhitespace(shouldBeWhiteSpace) || shouldBeWhiteSpace == 0) {
					bytes[0] = (byte)shouldBeWhiteSpace;
					startIndex++;
				}
				for (int i = startIndex; i < bytesToRead; i++) {
					int ch = tokeniser.Read();
					if (ch == -1)
						throw new InlineImageParseException("End of content stream reached before end of image data");

					bytes[i] = (byte)ch;
				}
				var ei = ps.ReadPRObject();
				if (!ei.ToString().Equals("EI"))
					throw new InlineImageParseException("EI not found after end of image data");

				return bytes;
			}

			/**
			 * Parses the samples of the image from the underlying content parser, accounting for filters
			 * The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
			 * The parser will be left positioned immediately following the EI operator.
			 * <b>Note:</b>This implementation does not actually apply the filters at this time
			 * @param imageDictionary the dictionary of the inline image
			 * @param ps the content parser
			 * @return the samples of the image
			 * @throws IOException if anything bad happens during parsing
			 */
			private static byte[] ParseInlineImageSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic, PdfContentParser ps) {
				// by the time we get to here, we have already parsed the ID operator

				if (!imageDictionary.Contains(PdfName.FILTER)) {
					return ParseUnfilteredSamples(imageDictionary, colorSpaceDic, ps);
				}


				// read all content until we reach an EI operator surrounded by whitespace.
				// The following algorithm has two potential issues: what if the image stream 
				// contains <ws>EI<ws> ?
				// Plus, there are some streams that don't have the <ws> before the EI operator
				// it sounds like we would have to actually decode the content stream, which
				// I'd rather avoid right now.
				var baos = new System.IO.MemoryStream();
				var accumulated = new System.IO.MemoryStream();
				int ch;
				int found = 0;
				var tokeniser = ps.GetTokeniser();
				byte[] ff = null;

				while ((ch = tokeniser.Read()) != -1) {
					if (found == 0 && PRTokeniser.IsWhitespace(ch)) {
						found++;
						accumulated.WriteByte((byte)ch);
					}
					else if (found == 1 && ch == 'E') {
						found++;
						accumulated.WriteByte((byte)ch);
					}
					else if (found == 1 && PRTokeniser.IsWhitespace(ch)) {
						// this clause is needed if we have a white space character that is part of the image data
						// followed by a whitespace character that precedes the EI operator.  In this case, we need
						// to flush the first whitespace, then treat the current whitespace as the first potential
						// character for the end of stream check.  Note that we don't increment 'found' here.
						baos.Write(ff = accumulated.ToArray(), 0, ff.Length);
						accumulated.SetLength(0);
						accumulated.WriteByte((byte)ch);
					}
					else if (found == 2 && ch == 'I') {
						found++;
						accumulated.WriteByte((byte)ch);
					}
					else if (found == 3 && PRTokeniser.IsWhitespace(ch)) {
						return baos.ToArray();
					}
					else {
						baos.Write(ff = accumulated.ToArray(), 0, ff.Length);
						accumulated.SetLength(0);

						baos.WriteByte((byte)ch);
						found = 0;
					}
				}
				throw new InlineImageParseException("Could not find image data or EI");
			}
		}

		sealed class FontInfoCache
		{
			public readonly FontInfo Info;
			public int Access;

			public FontInfoCache(FontInfo info) {
				Info = info;
			}
		}
	}
}
