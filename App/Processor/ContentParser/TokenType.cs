namespace PDFPatcher.Processor.ContentParser;

enum TokenType
{
	Null,           // null
	Boolean,        // true / false
	Integer,        // 123
	Real,           // 0.000008871
	String,         // (Literal String)
	HexString,      // <Hex String>
	Name,           // /Name
	Keyword,        // BT, Tj, ID, etc.
	Array,          // [ ... ]
	Dictionary,     // << ... >>
	InlineImage     // 特殊的 BI ... EI 结构
}
