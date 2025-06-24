using System;
using System.Text;
using System.Xml.Serialization;
using PDFPatcher.Common;

namespace PDFPatcher
{
	public class EncodingOptions
	{

		Encoding _bookmarkEncoding;
		public Encoding BookmarkEncoding {
			get {
				GetEncoding(_bookmarkEncodingName, ref _bookmarkEncoding);
				return _bookmarkEncoding;
			}
		}

		string _bookmarkEncodingName;
		///<summary>获取或指定读取书签时所用的编码。</summary>
		[XmlAttribute("书签文本编码")]
		public string BookmarkEncodingName {
			get => _bookmarkEncodingName;
			set => SetEncoding(ref _bookmarkEncodingName, ref _bookmarkEncoding, value);
		}

		Encoding _docInfoEncoding;
		public Encoding DocInfoEncoding {
			get {
				GetEncoding(_docInfoEncodingName, ref _docInfoEncoding);
				return _docInfoEncoding;
			}
		}
		string _docInfoEncodingName;
		///<summary>获取或指定读取文档元数据时所用的编码。</summary>
		[XmlAttribute("文档元数据编码")]
		public string DocInfoEncodingName {
			get => _docInfoEncodingName;
			set => SetEncoding(ref _docInfoEncodingName, ref _docInfoEncoding, value);
		}

		Encoding _textEncoding;
		public Encoding TextEncoding {
			get {
				GetEncoding(_textEncodingName, ref _textEncoding);
				return _textEncoding;
			}
		}
		string _textEncodingName;
		///<summary>获取或指定读取文本时所用的编码。</summary>
		[XmlAttribute("内容文本编码")]
		public string TextEncodingName {
			get => _textEncodingName;
			set => SetEncoding(ref _textEncodingName, ref _textEncoding, value);
		}

		Encoding _fontNameEncoding;
		public Encoding FontNameEncoding {
			get {
				GetEncoding(_fontNameEncodingName, ref _fontNameEncoding);
				return _fontNameEncoding;
			}
		}
		string _fontNameEncodingName;
		///<summary>获取或指定读取文本时所用的编码。</summary>
		[XmlAttribute("字体名称编码")]
		public string FontNameEncodingName {
			get => _fontNameEncodingName;
			set => SetEncoding(ref _fontNameEncodingName, ref _fontNameEncoding, value);
		}
		public static void SetEncoding(ref string encodingName, ref Encoding encoding, string value) {
			encoding = null;
			encodingName = (value == Constants.Encoding.Automatic ? null : value);
		}
		static void GetEncoding(string encodingName, ref Encoding encoding) {
			if (encoding == null && !String.IsNullOrEmpty(encodingName)) {
				encoding = ValueHelper.MapValue(encodingName, Constants.Encoding.EncodingNames, Constants.Encoding.Encodings);
			}
		}

	}
}
