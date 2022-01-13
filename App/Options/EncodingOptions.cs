using System.Text;
using System.Xml.Serialization;
using PDFPatcher.Common;

namespace PDFPatcher;

public class EncodingOptions
{
	private Encoding _bookmarkEncoding;

	private string _bookmarkEncodingName;

	private Encoding _docInfoEncoding;

	private string _docInfoEncodingName;

	private Encoding _fontNameEncoding;

	private string _fontNameEncodingName;

	private Encoding _textEncoding;

	private string _textEncodingName;

	public Encoding BookmarkEncoding {
		get {
			GetEncoding(_bookmarkEncodingName, ref _bookmarkEncoding);
			return _bookmarkEncoding;
		}
	}

	///<summary>获取或指定读取书签时所用的编码。</summary>
	[XmlAttribute("书签文本编码")]
	public string BookmarkEncodingName {
		get => _bookmarkEncodingName;
		set => SetEncoding(ref _bookmarkEncodingName, ref _bookmarkEncoding, value);
	}

	public Encoding DocInfoEncoding {
		get {
			GetEncoding(_docInfoEncodingName, ref _docInfoEncoding);
			return _docInfoEncoding;
		}
	}

	///<summary>获取或指定读取文档元数据时所用的编码。</summary>
	[XmlAttribute("文档元数据编码")]
	public string DocInfoEncodingName {
		get => _docInfoEncodingName;
		set => SetEncoding(ref _docInfoEncodingName, ref _docInfoEncoding, value);
	}

	public Encoding TextEncoding {
		get {
			GetEncoding(_textEncodingName, ref _textEncoding);
			return _textEncoding;
		}
	}

	///<summary>获取或指定读取文本时所用的编码。</summary>
	[XmlAttribute("内容文本编码")]
	public string TextEncodingName {
		get => _textEncodingName;
		set => SetEncoding(ref _textEncodingName, ref _textEncoding, value);
	}

	public Encoding FontNameEncoding {
		get {
			GetEncoding(_fontNameEncodingName, ref _fontNameEncoding);
			return _fontNameEncoding;
		}
	}

	///<summary>获取或指定读取文本时所用的编码。</summary>
	[XmlAttribute("字体名称编码")]
	public string FontNameEncodingName {
		get => _fontNameEncodingName;
		set => SetEncoding(ref _fontNameEncodingName, ref _fontNameEncoding, value);
	}

	public static void SetEncoding(ref string encodingName, ref Encoding encoding, string value) {
		encoding = null;
		encodingName = value == Constants.Encoding.Automatic ? null : value;
	}

	private static void GetEncoding(string encodingName, ref Encoding encoding) {
		if (encoding == null && string.IsNullOrEmpty(encodingName) == false) {
			encoding = ValueHelper.MapValue(encodingName, Constants.Encoding.EncodingNames,
				Constants.Encoding.Encodings);
		}
	}
}