using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.error_messages;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PDFPatcher.Model;
using GraphicsState = PDFPatcher.Model.GraphicsState;

namespace PDFPatcher.Processor;

internal class PdfContentStreamProcessor
{
	public static readonly IContentOperator NopOperator = new IgnoreOperatorContentOperator();
	private readonly Dictionary<int, WeakReference> _FontCache = new();
	private readonly Dictionary<int, string> _FontNameCache = new();

	private readonly Dictionary<PdfName, IXObjectDoHandler> _XObjectDoHandlers = new();

	// Fields
	private readonly Stack<GraphicsState> gsStack = new();
	private readonly Stack<MarkedContentInfo> markedContentStack = new();

	private readonly IDictionary<string, IContentOperator> operators = new Dictionary<string, IContentOperator>();

	//IRenderListener renderListener;
	private Matrix _TextLineMatrix;

	internal PdfContentStreamProcessor() {
		Reset();
	}

	// Methods
	internal GraphicsState CurrentGraphicState => gsStack.Peek();
	internal Matrix TextMatrix { get; private set; }

	internal IContentOperator DefaultOperator { get; set; }
	protected IDictionary<int, string> Fonts => _FontNameCache;
	protected ResourceDictionary Resource { get; private set; }

	protected void ApplyTextAdjust(float tj) {
		float adjustBy = -tj / 1000f * CurrentGraphicState.FontSize * CurrentGraphicState.HorizontalScaling;
		TextMatrix = new Matrix(adjustBy, 0f).Multiply(TextMatrix);
	}

	protected void AdjustTextMatrixX(float x) {
		TextMatrix = new Matrix(x, 0f).Multiply(TextMatrix);
	}

	protected void BeginMarkedContent(PdfName tag, PdfDictionary dict) {
		markedContentStack.Push(new MarkedContentInfo(tag, dict));
	}

	private static void BeginText() {
		//this.renderListener.BeginTextBlock ();
	}

	private FontInfo GetFont(PRIndirectReference fontRef) {
		if (_FontCache.TryGetValue(fontRef.Number, out WeakReference r)) {
			return (FontInfo)(r.Target ?? (r.Target = new FontInfo(fontRef)));
		}

		FontInfo f = new(fontRef);
		_FontCache.Add(fontRef.Number, new WeakReference(f));
		_FontNameCache.Add(fontRef.Number, f.FontName);
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

	private static void EndText() {
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
		SetTextCharacterSpacing tcOperator = new();
		RegisterContentOperator("Tc", tcOperator);
		SetTextWordSpacing twOperator = new();
		RegisterContentOperator("Tw", twOperator);
		RegisterContentOperator("Tz", new SetTextHorizontalScaling());
		SetTextLeading tlOperator = new();
		RegisterContentOperator("TL", tlOperator);
		RegisterContentOperator("Tf", new SetTextFont());
		RegisterContentOperator("Tr", new SetTextRenderMode());
		RegisterContentOperator("Ts", new SetTextRise());
		RegisterContentOperator("BT", new BeginTextC());
		RegisterContentOperator("ET", new EndTextC());
		RegisterContentOperator("BMC", new BeginMarkedContentC());
		RegisterContentOperator("BDC", new BeginMarkedContentDictionary());
		RegisterContentOperator("EMC", new EndMarkedContentC());
		TextMoveStartNextLine tdOperator = new();
		RegisterContentOperator("Td", tdOperator);
		RegisterContentOperator("TD", new TextMoveStartNextLineWithLeading(tdOperator, tlOperator));
		RegisterContentOperator("Tm", new TextSetTextMatrix());
		TextMoveNextLine tstarOperator = new(tdOperator);
		RegisterContentOperator("T*", tstarOperator);
		ShowText tjOperator = new();
		RegisterContentOperator("Tj", new ShowText());
		RegisterContentOperator("TJ", new ShowTextArray());
		MoveNextLineAndShowText tickOperator = new(tstarOperator, tjOperator);
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
		Resource.Push(resources);
		PRTokeniser tokenizer = new(new RandomAccessFileOrArray(contentBytes));
		PdfContentParser ps = new(tokenizer);
		List<PdfObject> operands = new();
		while (ps.Parse(operands).Count > 0) {
			PdfLiteral oper = (PdfLiteral)operands[operands.Count - 1];
			if ("BI".Equals(oper.ToString())) {
				PdfImageData img = InlineImageUtils.ParseInlineImage(ps, resources.GetAsDict(PdfName.COLORSPACE));
				InvokeOperator(oper, new List<PdfObject> { img, oper });
				//    this.renderListener.RenderImage (renderInfo);
			}
			else {
				InvokeOperator(oper, operands);
			}
		}

		Resource.Pop();
	}

	internal IContentOperator RegisterContentOperator(string operatorString, IContentOperator oper) {
		if (oper != null) {
			return operators[operatorString] = oper;
		}

		operators.Remove(operatorString);
		return null;

	}

	internal IXObjectDoHandler RegisterXObjectDoHandler(PdfName xobjectSubType, IXObjectDoHandler handler) {
		_XObjectDoHandlers.TryGetValue(xobjectSubType, out IXObjectDoHandler old);
		_XObjectDoHandlers[xobjectSubType] = handler;
		return old;
	}

	internal virtual void Reset() {
		gsStack.Clear();
		gsStack.Push(new GraphicsState());
		TextMatrix = null;
		_TextLineMatrix = null;
		Resource = new ResourceDictionary();
	}

	internal static class InlineImageUtils
	{
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

		static InlineImageUtils() {
			// static initializer
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
			PdfDictionary d = ParseInlineImageDictionary(ps);
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
			PdfDictionary dictionary = new();

			for (PdfObject key = ps.ReadPRObject();
				 key != null && !"ID".Equals(key.ToString());
				 key = ps.ReadPRObject()) {
				PdfObject value = ps.ReadPRObject();

				inlineImageEntryAbbreviationMap.TryGetValue((PdfName)key, out PdfName resolvedKey);
				resolvedKey ??= (PdfName)key;

				dictionary.Put(resolvedKey, GetAlternateValue(resolvedKey, value));
			}

			int ch = ps.GetTokeniser().Read();
			if (!PRTokeniser.IsWhitespace(ch)) {
				throw new IOException("Unexpected character " + ch + " found after ID in inline image");
			}

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
				switch (value) {
					case PdfName name: {
							inlineImageFilterAbbreviationMap.TryGetValue(name, out PdfName altValue);
							if (altValue != null) {
								return altValue;
							}

							break;
						}
					case PdfArray pdfArray: {
							PdfArray altArray = new();
							int count = pdfArray.Size;
							for (int i = 0; i < count; i++) {
								altArray.Add(GetAlternateValue(key, pdfArray[i]));
							}

							return altArray;
						}
				}
			}
			else if (key == PdfName.COLORSPACE) {
				if (value is not PdfName) {
					return value;
				}

				inlineImageColorSpaceAbbreviationMap.TryGetValue((PdfName)value, out PdfName altValue);
				if (altValue != null) {
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
			if (colorSpaceName == null) {
				return 1;
			}

			if (colorSpaceName.Equals(PdfName.DEVICEGRAY)) {
				return 1;
			}

			if (colorSpaceName.Equals(PdfName.DEVICERGB)) {
				return 3;
			}

			if (colorSpaceName.Equals(PdfName.DEVICECMYK)) {
				return 4;
			}

			if (colorSpaceDic == null) {
				throw new ArgumentException("Unexpected color space " + colorSpaceName);
			}

			PdfArray colorSpace = colorSpaceDic.GetAsArray(colorSpaceName);
			if (colorSpace == null) {
				throw new ArgumentException("Unexpected color space " + colorSpaceName);
			}

			if (PdfName.INDEXED.Equals(colorSpace.GetAsName(0))) {
				return 1;
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
			PdfNumber wObj = imageDictionary.GetAsNumber(PdfName.WIDTH);
			PdfNumber bpcObj = imageDictionary.GetAsNumber(PdfName.BITSPERCOMPONENT);
			int cpp = GetComponentsPerPixel(imageDictionary.GetAsName(PdfName.COLORSPACE), colorSpaceDic);

			int w = wObj.IntValue;
			int bpc = bpcObj?.IntValue ?? 1;


			int bytesPerRow = ((w * bpc * cpp) + 7) / 8;

			return bytesPerRow;
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
		private static byte[] ParseUnfilteredSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic,
			PdfContentParser ps) {
			// special case:  when no filter is specified, we just read the number of bits
			// per component, multiplied by the width and height.
			if (imageDictionary.Contains(PdfName.FILTER)) {
				throw new ArgumentException("Dictionary contains filters");
			}

			PdfNumber h = imageDictionary.GetAsNumber(PdfName.HEIGHT);

			int bytesToRead = ComputeBytesPerRow(imageDictionary, colorSpaceDic) * h.IntValue;
			byte[] bytes = new byte[bytesToRead];
			PRTokeniser tokeniser = ps.GetTokeniser();

			int shouldBeWhiteSpace =
				tokeniser.Read(); // skip next character (which better be a whitespace character - I suppose we could check for this)
								  // from the PDF spec:  Unless the image uses ASCIIHexDecode or ASCII85Decode as one of its filters, the ID operator shall be followed by a single white-space character, and the next character shall be interpreted as the first byte of image data.
								  // unfortunately, we've seen some PDFs where there is no space following the ID, so we have to capture this case and handle it
			int startIndex = 0;
			if (!PRTokeniser.IsWhitespace(shouldBeWhiteSpace) || shouldBeWhiteSpace == 0) {
				bytes[0] = (byte)shouldBeWhiteSpace;
				startIndex++;
			}

			for (int i = startIndex; i < bytesToRead; i++) {
				int ch = tokeniser.Read();
				if (ch == -1) {
					throw new InlineImageParseException("End of content stream reached before end of image data");
				}

				bytes[i] = (byte)ch;
			}

			PdfObject ei = ps.ReadPRObject();
			if (!ei.ToString().Equals("EI")) {
				throw new InlineImageParseException("EI not found after end of image data");
			}

			return bytes;
		}

		/**
		 * Parses the samples of the image from the underlying content parser, accounting for filters
		 * The parser must be positioned immediately after the ID operator that ends the inline image's dictionary.
		 * The parser will be left positioned immediately following the EI operator.
		 * <b>Note:</b>
		 * This implementation does not actually apply the filters at this time
		 * @param imageDictionary the dictionary of the inline image
		 * @param ps the content parser
		 * @return the samples of the image
		 * @throws IOException if anything bad happens during parsing
		 */
		private static byte[] ParseInlineImageSamples(PdfDictionary imageDictionary, PdfDictionary colorSpaceDic,
			PdfContentParser ps) {
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
			MemoryStream baos = new();
			MemoryStream accumulated = new();
			int ch;
			int found = 0;
			PRTokeniser tokeniser = ps.GetTokeniser();

			while ((ch = tokeniser.Read()) != -1) {
				switch (found) {
					case 0 when PRTokeniser.IsWhitespace(ch):
						found++;
						accumulated.WriteByte((byte)ch);
						break;
					case 1 when ch == 'E':
						found++;
						accumulated.WriteByte((byte)ch);
						break;
					default: {
							byte[] ff = null;
							switch (found) {
								case 1 when PRTokeniser.IsWhitespace(ch):
									// this clause is needed if we have a white space character that is part of the image data
									// followed by a whitespace character that precedes the EI operator.  In this case, we need
									// to flush the first whitespace, then treat the current whitespace as the first potential
									// character for the end of stream check.  Note that we don't increment 'found' here.
									baos.Write(ff = accumulated.ToArray(), 0, ff.Length);
									accumulated.SetLength(0);
									accumulated.WriteByte((byte)ch);
									break;
								case 2 when ch == 'I':
									found++;
									accumulated.WriteByte((byte)ch);
									break;
								case 3 when PRTokeniser.IsWhitespace(ch):
									return baos.ToArray();
								default:
									baos.Write(ff = accumulated.ToArray(), 0, ff.Length);
									accumulated.SetLength(0);

									baos.WriteByte((byte)ch);
									found = 0;
									break;
							}

							break;
						}
				}
			}

			throw new InlineImageParseException("Could not find image data or EI");
		}

		/**
			 * Simple class in case users need to differentiate an exception from processing
			 * inline images vs other exceptions 
			 * @since 5.0.4
			 */
		public sealed class InlineImageParseException : IOException
		{
			public InlineImageParseException(string message)
				: base(message) {
			}
		}
	}

	#region Nested Types

	internal interface IContentOperator
	{
		void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands);
	}

	internal interface IXObjectDoHandler
	{
		void HandleXObject(PdfContentStreamProcessor processor, PdfStream stream, PdfIndirectReference refi);
	}

	protected sealed class BeginMarkedContentC : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.BeginMarkedContent((PdfName)operands[0], new PdfDictionary());
		}
	}

	protected sealed class BeginMarkedContentDictionary : IContentOperator
	{
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfObject properties = operands[1];
			processor.BeginMarkedContent((PdfName)operands[0], GetPropertiesDictionary(properties, processor.Resource));
		}

		// Methods
		private static PdfDictionary GetPropertiesDictionary(PdfObject operand1, PdfDictionary resources) {
			if (operand1.IsDictionary()) {
				return (PdfDictionary)operand1;
			}

			PdfName dictionaryName = (PdfName)operand1;
			return resources.GetAsDict(dictionaryName);
		}
	}

	protected sealed class BeginTextC : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.TextMatrix = new Matrix();
			processor._TextLineMatrix = processor.TextMatrix;
			BeginText();
		}
	}

	protected sealed class Do : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfName xobjectName = (PdfName)operands[0];
			processor.DisplayXObject(xobjectName);
		}
	}

	protected sealed class EndMarkedContentC : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.EndMarkedContent();
		}
	}

	protected sealed class EndTextC : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.TextMatrix = null;
			processor._TextLineMatrix = null;
			EndText();
		}
	}

	protected sealed class FormXObjectDoHandler : IXObjectDoHandler
	{
		// Methods
		public void HandleXObject(PdfContentStreamProcessor processor, PdfStream stream, PdfIndirectReference refi) {
			PdfDictionary resources = stream.GetAsDict(PdfName.RESOURCES);
			byte[] contentBytes = ContentByteUtils.GetContentBytesFromContentObject(stream);
			PdfArray matrix = stream.GetAsArray(PdfName.MATRIX);
			new PushGraphicsState().Invoke(processor, null, null);
			if (matrix != null) {
				float a = matrix.GetAsNumber(0).FloatValue;
				float b = matrix.GetAsNumber(1).FloatValue;
				float c = matrix.GetAsNumber(2).FloatValue;
				float d = matrix.GetAsNumber(3).FloatValue;
				float e = matrix.GetAsNumber(4).FloatValue;
				float f = matrix.GetAsNumber(5).FloatValue;
				processor.CurrentGraphicState.TransMatrix =
					new Matrix(a, b, c, d, e, f).Multiply(processor.CurrentGraphicState.TransMatrix);
			}

			processor.ProcessContent(contentBytes, resources);
			new PopGraphicsState().Invoke(processor, null, null);
		}
	}

	protected sealed class IgnoreOperatorContentOperator : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
		}
	}

	protected sealed class IgnoreXObjectDoHandler : IXObjectDoHandler
	{
		// Methods
		public void HandleXObject(PdfContentStreamProcessor processor, PdfStream xobjectStream,
			PdfIndirectReference refi) {
		}
	}

	protected sealed class ImageXObjectDoHandler : IXObjectDoHandler
	{
		// Methods
		public void HandleXObject(PdfContentStreamProcessor processor, PdfStream xobjectStream,
			PdfIndirectReference refi) {
			//ImageRenderInfo renderInfo = ImageRenderInfo.CreateForXObject (processor.CurrentGraphicState.TransMatrix, refi);
			//processor.renderListener.RenderImage (renderInfo);
		}
	}

	protected sealed class ModifyCurrentTransformationMatrix : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			float a = ((PdfNumber)operands[0]).FloatValue;
			float b = ((PdfNumber)operands[1]).FloatValue;
			float c = ((PdfNumber)operands[2]).FloatValue;
			float d = ((PdfNumber)operands[3]).FloatValue;
			float e = ((PdfNumber)operands[4]).FloatValue;
			float f = ((PdfNumber)operands[5]).FloatValue;
			Matrix matrix = new(a, b, c, d, e, f);
			GraphicsState gs = processor.gsStack.Peek();
			gs.TransMatrix = matrix.Multiply(gs.TransMatrix);
		}
	}

	protected sealed class MoveNextLineAndShowText : IContentOperator
	{
		// Fields
		private readonly IContentOperator showText;
		private readonly IContentOperator textMoveNextLine;

		// Methods
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
		// Fields
		private readonly IContentOperator moveNextLineAndShowText;
		private readonly IContentOperator setTextCharacterSpacing;
		private readonly IContentOperator setTextWordSpacing;

		// Methods
		public MoveNextLineAndShowTextWithSpacing(IContentOperator setTextWordSpacing,
			IContentOperator setTextCharacterSpacing, IContentOperator moveNextLineAndShowText) {
			this.setTextWordSpacing = setTextWordSpacing;
			this.setTextCharacterSpacing = setTextCharacterSpacing;
			this.moveNextLineAndShowText = moveNextLineAndShowText;
		}

		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber aw = (PdfNumber)operands[0];
			PdfNumber ac = (PdfNumber)operands[1];
			PdfString str = (PdfString)operands[2];
			List<PdfObject> twOperands = new(1);
			twOperands.Insert(0, aw);
			setTextWordSpacing.Invoke(processor, null, twOperands);
			List<PdfObject> tcOperands = new(1);
			tcOperands.Insert(0, ac);
			setTextCharacterSpacing.Invoke(processor, null, tcOperands);
			List<PdfObject> tickOperands = new(1);
			tickOperands.Insert(0, str);
			moveNextLineAndShowText.Invoke(processor, null, tickOperands);
		}
	}

	protected sealed class PopGraphicsState : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.gsStack.Pop();
		}
	}

	protected sealed class ProcessGraphicsStateResource : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfName dictionaryName = (PdfName)operands[0];
			PdfDictionary extGState = processor.Resource.GetAsDict(PdfName.EXTGSTATE);
			if (extGState == null) {
				throw new ArgumentException(
					MessageLocalization.GetComposedMessage(
						"resources.do.not.contain.extgstate.entry.unable.to.process.oper.1", oper));
			}

			PdfDictionary gsDic = extGState.GetAsDict(dictionaryName);
			if (gsDic == null) {
				throw new ArgumentException(
					MessageLocalization.GetComposedMessage("1.is.an.unknown.graphics.state.dictionary",
						dictionaryName));
			}

			PdfArray fontParameter = gsDic.GetAsArray(PdfName.FONT);
			if (fontParameter == null) {
				return;
			}

			FontInfo font = processor.GetFont((PRIndirectReference)fontParameter[0]);
			float size = fontParameter.GetAsNumber(1).FloatValue;
			processor.CurrentGraphicState.Font = font;
			processor.CurrentGraphicState.FontSize = size;
		}
	}

	protected sealed class PushGraphicsState : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			processor.gsStack.Push(processor.gsStack.Peek().Copy());
		}
	}

	protected sealed class ResourceDictionary : PdfDictionary
	{
		// Fields
		private readonly IList<PdfDictionary> resourcesStack = new List<PdfDictionary>();

		// Methods
		public override PdfObject GetDirectObject(PdfName key) {
			for (int i = resourcesStack.Count - 1; i >= 0; i--) {
				PdfDictionary subResource = resourcesStack[i];

				PdfObject obj = subResource?.GetDirectObject(key);
				if (obj != null) {
					return obj;
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
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber charSpace = (PdfNumber)operands[0];
			processor.CurrentGraphicState.CharacterSpacing = charSpace.FloatValue;
		}
	}

	protected sealed class SetTextFont : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfName fontResourceName = (PdfName)operands[0];
			float size = ((PdfNumber)operands[1]).FloatValue;
			PdfObject f = processor.Resource.GetAsDict(PdfName.FONT).Get(fontResourceName);
			GraphicsState g = processor.CurrentGraphicState;
			if (f is not PRIndirectReference fref) {
				Tracker.DebugMessage("字体（" + fontResourceName + "）不为引用。");
				PdfDictionary fd = f as PdfDictionary;
				g.FontID = 0;
				g.Font = new FontInfo(fd, 0);
			}
			else {
				FontInfo font = processor.GetFont(fref);
				g.FontID = fref.Number;
				g.Font = font;
			}

			g.FontSize = size;
		}
	}

	protected sealed class SetTextHorizontalScaling : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber scale = (PdfNumber)operands[0];
			processor.CurrentGraphicState.HorizontalScaling = scale.FloatValue / 100f;
		}
	}

	protected sealed class SetTextLeading : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber leading = (PdfNumber)operands[0];
			processor.CurrentGraphicState.Leading = leading.FloatValue;
		}
	}

	protected sealed class SetTextRenderMode : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber render = (PdfNumber)operands[0];
			processor.CurrentGraphicState.RenderMode = render.IntValue;
		}
	}

	protected sealed class SetTextRise : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber rise = (PdfNumber)operands[0];
			processor.CurrentGraphicState.Rise = rise.FloatValue;
		}
	}

	protected sealed class SetTextWordSpacing : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfNumber wordSpace = (PdfNumber)operands[0];
			processor.CurrentGraphicState.WordSpacing = wordSpace.FloatValue;
		}
	}

	protected sealed class ShowText : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfString str = (PdfString)operands[0];
			processor.DisplayPdfString(str);
		}
	}

	protected sealed class AccumulatedShowTextArray : IContentOperator
	{
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfArray array = (PdfArray)operands[0];
			float adj = 0;
			using (MemoryStream ms = new(array.Length)) {
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
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			PdfArray array = (PdfArray)operands[0];
			//using (var ms = new System.IO.MemoryStream (array.Length)) {
			//    foreach (PdfObject item in array.ArrayList) {
			//        if (item.Type == PdfObject.STRING) {
			//            ms.Write ((item as PdfString).GetBytes (), 0, item.Length);
			//        }
			//        else {
			//            processor.ApplyTextAdjust (((PdfNumber)item).FloatValue);
			//        }
			//    }
			//    processor.DisplayPdfString (new PdfString (ms.ToArray ()));
			//}

			foreach (PdfObject entryObj in array.ArrayList) {
				float tj = 0f;
				if (entryObj is PdfString) {
					processor.DisplayPdfString((PdfString)entryObj);
					tj = 0f;
				}
				else {
					tj = ((PdfNumber)entryObj).FloatValue;
					processor.ApplyTextAdjust(tj);
				}
			}
		}
	}

	protected sealed class TextMoveNextLine : IContentOperator
	{
		// Fields
		private readonly TextMoveStartNextLine moveStartNextLine;

		// Methods
		public TextMoveNextLine(TextMoveStartNextLine moveStartNextLine) {
			this.moveStartNextLine = moveStartNextLine;
		}

		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			List<PdfObject> tdoperands = new(2);
			tdoperands.Insert(0, new PdfNumber(0));
			tdoperands.Insert(1, new PdfNumber(-processor.CurrentGraphicState.Leading));
			moveStartNextLine.Invoke(processor, null, tdoperands);
		}
	}

	protected sealed class TextMoveStartNextLine : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			float tx = ((PdfNumber)operands[0]).FloatValue;
			float ty = ((PdfNumber)operands[1]).FloatValue;
			processor.TextMatrix = new Matrix(tx, ty).Multiply(processor._TextLineMatrix);
			processor._TextLineMatrix = processor.TextMatrix;
		}
	}

	protected sealed class TextMoveStartNextLineWithLeading : IContentOperator
	{
		// Fields
		private readonly TextMoveStartNextLine moveStartNextLine;
		private readonly SetTextLeading setTextLeading;

		// Methods
		public TextMoveStartNextLineWithLeading(TextMoveStartNextLine moveStartNextLine,
			SetTextLeading setTextLeading) {
			this.moveStartNextLine = moveStartNextLine;
			this.setTextLeading = setTextLeading;
		}

		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			float ty = ((PdfNumber)operands[1]).FloatValue;
			List<PdfObject> tlOperands = new(1);
			tlOperands.Insert(0, new PdfNumber(-ty));
			setTextLeading.Invoke(processor, null, tlOperands);
			moveStartNextLine.Invoke(processor, null, operands);
		}
	}

	protected sealed class TextSetTextMatrix : IContentOperator
	{
		// Methods
		public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands) {
			float a = ((PdfNumber)operands[0]).FloatValue;
			float b = ((PdfNumber)operands[1]).FloatValue;
			float c = ((PdfNumber)operands[2]).FloatValue;
			float d = ((PdfNumber)operands[3]).FloatValue;
			float e = ((PdfNumber)operands[4]).FloatValue;
			float f = ((PdfNumber)operands[5]).FloatValue;
			processor._TextLineMatrix = new Matrix(a, b, c, d, e, f);
			processor.TextMatrix = processor._TextLineMatrix;
		}
	}

	#endregion
}