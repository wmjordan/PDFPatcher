using System;
using System.Collections.Generic;
using System.Text;
using MuPDF;
using PDFPatcher.Common;

namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// PDF Token 对象，包含原始引用和解析后的值
/// </summary>
sealed class Token(byte[] buffer, int offset, int length, TokenType type, object value)
{
	public static readonly Token[] EmptyTokens = [];
	public static readonly Token Null = new(TokenType.Null, null);
	public static readonly Token True = new(TokenType.Boolean, true);
	public static readonly Token False = new(TokenType.Boolean, false);

	// 静态工厂方法
	public static Token Boolean(bool value) => new(TokenType.Boolean, value);
	public static Token Integer(long value) => new(TokenType.Integer, value);
	public static Token Real(double value) => new(TokenType.Real, value);
	public static Token String(string value) => new(TokenType.String, value);
	public static Token HexString(string value) => new(TokenType.HexString, value);
	public static Token Name(string value) => new(TokenType.Name, value);
	public static Token Keyword(string value) => new(TokenType.Keyword, value);
	public static Token Array(List<Token> value) => new(TokenType.Array, value);
	public static Token Dictionary(Dictionary<string, Token> value) => new(TokenType.Dictionary, value);
	public static Token InlineImage(InlineImageContent value) => new(TokenType.InlineImage, value);

	internal readonly byte[] Buffer = buffer; // 保留对原始字节数组的引用
	internal readonly int Offset = offset;    // 起始偏移量
	internal readonly int Length = length;    // 长度

	public TokenType Type { get; } = type;
	public object Value { get; } = value;

	public Token(TokenType type, object value) : this(null, 0, 0, type, value) { }

	public static implicit operator Token(bool value) => Boolean(value);
	public static implicit operator Token(long value) => Integer(value);
	public static implicit operator Token(double value) => Real(value);
	public static implicit operator Token(string value) => String(value); // 默认作为括号字符串

	/// <summary>
	/// 获取原始字符串表示（用于还原 PDF 内容）
	/// </summary>
	public string GetRawString() {
		if (Buffer == null) {
			throw new InvalidOperationException("此 Token 没有原始字节引用，无法获取原始字符串。");
		}
		// 使用 Latin-1 (ISO-8859-1) 编码，确保字节 0-255 与字符一一对应，不发生转换
		return Encoding.GetEncoding("Latin1").GetString(Buffer, Offset, Length);
	}

	public List<Token> AsArray() {
		return Type == TokenType.Array ? (List<Token>)Value : null;
	}
	public Dictionary<string, Token> AsDictionary() {
		return Type == TokenType.Dictionary ? (Dictionary<string, Token>)Value : null;
	}
	public long AsInt64() {
		return Type switch {
			TokenType.Integer => (long)Value,
			TokenType.Real => ((double)Value).ToInt64(),
			_ => 0,
		};
	}
	public long AsInt64(long defaultValue) {
		return Type switch {
			TokenType.Integer => (long)Value,
			TokenType.Real => ((double)Value).ToInt64(),
			_ => defaultValue,
		};
	}
	public float AsFloat() {
		return Type switch {
			TokenType.Integer => (long)Value,
			TokenType.Real => (float)(double)Value,
			_ => 0,
		};
	}
	public float AsFloat(float defaultValue) {
		return Type switch {
			TokenType.Integer => (long)Value,
			TokenType.Real => (float)(double)Value,
			_ => defaultValue,
		};
	}
	public double AsDouble() {
		return Type switch {
			TokenType.Integer => (double)(long)Value,
			TokenType.Real => (double)Value,
			_ => 0,
		};
	}
	public double AsDouble(double defaultValue) {
		return Type switch {
			TokenType.Integer => (double)(long)Value,
			TokenType.Real => (double)Value,
			_ => defaultValue,
		};
	}
	public string AsString() {
		return Type switch {
			TokenType.String
			or TokenType.Name => (string)Value,
			_ => null
		};
	}
	public string AsString(string defaultValue) {
		return Type switch {
			TokenType.String
			or TokenType.Name => (string)Value,
			_ => defaultValue
		};
	}

	public override string ToString() {
		return Type switch {
			TokenType.Null => "null",
			TokenType.Boolean
				or TokenType.Integer
				or TokenType.Real
				or TokenType.Keyword
				or TokenType.String
				or TokenType.HexString
				or TokenType.Name // // 对于名称对象，原始字节中已经包含了前导斜杠 /
				=> GetRawString(),
			TokenType.Array => GetArrayString(),// 数组：递归处理
			TokenType.Dictionary => GetDictionaryString(),// 字典：递归处理
			TokenType.InlineImage => GetInlineImageString(),// 内联图像：详细输出其字典内容和二进制数据长度
			_ => GetRawString(),// 默认回退：输出原始字节
		};
	}

	string GetArrayString() {
		if (Value is List<Token> list) {
			var sb = new StringBuilder("[", 32);
			bool hasItem = false;
			foreach (var item in list) {
				if (hasItem) {
					sb.Append(' ');
				}
				else {
					hasItem = true;
				}
				sb.Append(item.ToString());
			}
			return sb.Append(']').ToString();
		}
		return "[Array Error]";
	}

	string GetDictionaryString() {
		if (Value is Dictionary<string, Token> dict) {
			var sb = new StringBuilder("<< ", 32);
			sb.Append('<').Append('<').Append(' ');
			foreach (var kvp in dict) {
				// 1. 输出 Key：手动补上斜杠，然后输出名称字符串
				sb.Append('/');
				sb.Append(kvp.Key);
				sb.Append(' ');

				// 2. 输出 Value：递归调用
				sb.Append(kvp.Value.ToString());
				sb.Append(' ');
			}
			return sb.Append('>').Append('>').ToString();
		}
		return "<</Dictionary Error>>";
	}

	string GetInlineImageString() {
		if (Value is InlineImageContent img) {
			var sb = new StringBuilder("<< ", 32);
			// 遍历内联图像的字典参数
			foreach (var kvp in img.Dictionary) {
				sb.Append('/');
				sb.Append(kvp.Key);
				sb.Append(' ');
				sb.Append(kvp.Value.ToString());
				sb.Append(' ');
			}
			// 输出数据摘要
			return sb.Append(">> [Binary Data ")
				.Append(img.Data.Length)
				.Append(" bytes]")
				.ToString();
		}
		return "[Inline Image Error]";
	}
}
