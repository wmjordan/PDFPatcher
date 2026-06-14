using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CLR;

namespace PDFPatcher.Processor.ContentParser;

/// <summary>
/// 将 Operation 列表重新构建为 PDF 内容流字节数组
/// </summary>
static class ContentStreamBuilder
{
	static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
	const byte Space = (byte)' ', NewLine = (byte)'\n';

	/// <summary>
	/// 从操作序列构建 PDF 内容流字节
	/// </summary>
	public static byte[] Build(IEnumerable<Operation> operations) {
		using var ms = new MemoryStream();
		foreach (var op in operations) {
			// 跳过孤立的 BI，由后续的 EI 处理
			switch (op.Kind) {
				case RenderCommandKind.BeginInlineImage:
					continue;
				case RenderCommandKind.EndInlineImage:
					// 处理内联图像（必须包含一个 InlineImage 操作数）
					if (op.Operands.Length == 1 && op.Operands[0].Type == TokenType.InlineImage) {
						WriteInlineImage(ms, (InlineImageContent)op.Operands[0].Value);
						continue;
					}
					// 降级为普通操作符输出（容错）
					break;
			}

			// 普通操作：先输出所有操作数，后跟操作符
			foreach (var token in op.Operands) {
				WriteToken(ms, token);
				ms.WriteByte(Space);
			}
			WriteOperator(ms, op.Operator);
			ms.WriteByte(NewLine);
		}
		return ms.ToArray();
	}

	#region 序列化核心

	private static void WriteToken(Stream stream, Token token) {
		// 如果 Token 持有原始字节且不是复合类型，直接写入，避免重复计算
		if (token.Buffer != null
			&& !token.Type.CeqAny(TokenType.Array, TokenType.Dictionary, TokenType.InlineImage)) {
			stream.Write(token.Buffer, token.Offset, token.Length);
			return;
		}

		switch (token.Type) {
			case TokenType.Null:
				WriteString(stream, "null");
				break;
			case TokenType.Boolean:
				WriteString(stream, (bool)token.Value ? "true" : "false");
				break;
			case TokenType.Integer:
				WriteString(stream, ((long)token.Value).ToString(CultureInfo.InvariantCulture));
				break;
			case TokenType.Real:
				WriteString(stream, ((double)token.Value).ToString("G", CultureInfo.InvariantCulture));
				break;
			case TokenType.String:
				WriteLiteralString(stream, (string)token.Value);
				break;
			case TokenType.HexString:
				WriteHexString(stream, (string)token.Value);
				break;
			case TokenType.Name:
				WriteName(stream, (string)token.Value);
				break;
			case TokenType.Keyword:
				WriteString(stream, (string)token.Value);
				break;
			case TokenType.Array:
				WriteArray(stream, (List<Token>)token.Value);
				break;
			case TokenType.Dictionary:
				WriteDictionary(stream, (Dictionary<string, Token>)token.Value);
				break;
			case TokenType.InlineImage:
				// 不应作为常规操作数出现
			default:
				throw new NotSupportedException($"Unsupported token type: {token.Type}");
		}
	}

	private static void WriteLiteralString(Stream stream, string s) {
		byte[] bytes = Latin1.GetBytes(s);
		stream.WriteByte((byte)'(');
		foreach (byte b in bytes) {
			if (b.CeqAny('(', ')', '\\')) {
				stream.WriteByte((byte)'\\');
			}
			stream.WriteByte(b);
		}
		stream.WriteByte((byte)')');
	}

	private static void WriteHexString(Stream stream, string hex) {
		stream.WriteByte((byte)'<');
		WriteString(stream, hex);
		stream.WriteByte((byte)'>');
	}

	private static void WriteName(Stream stream, string name) {
		stream.WriteByte((byte)'/');
		WriteString(stream, EncodeName(name));
	}

	private static void WriteArray(Stream stream, List<Token> list) {
		stream.WriteByte((byte)'[');
		for (int i = 0; i < list.Count; i++) {
			if (i > 0) {
				stream.WriteByte(Space);
			}
			WriteToken(stream, list[i]);
		}
		stream.WriteByte((byte)']');
	}

	private static void WriteDictionary(Stream stream, Dictionary<string, Token> dict) {
		stream.WriteByte((byte)'<');
		stream.WriteByte((byte)'<');
		bool first = true;
		foreach (var kvp in dict) {
			if (!first) {
				stream.WriteByte(Space);
			}
			else {
				first = false;
			}

			// 键（名称）
			stream.WriteByte((byte)'/');
			WriteString(stream, EncodeName(kvp.Key));
			stream.WriteByte(Space);
			// 值
			WriteToken(stream, kvp.Value);
		}
		stream.WriteByte((byte)'>');
		stream.WriteByte((byte)'>');
	}

	private static void WriteInlineImage(Stream stream, InlineImageContent img) {
		// BI
		WriteString(stream, "BI");
		stream.WriteByte(Space);

		// 字典键值对
		bool first = true;
		foreach (var kvp in img.Dictionary) {
			if (!first) {
				stream.WriteByte(Space);
			}
			else {
				first = false;
			}

			// 键（名称）
			stream.WriteByte((byte)'/');
			WriteString(stream, EncodeName(kvp.Key));
			stream.WriteByte(Space);
			// 值
			WriteToken(stream, kvp.Value);
		}

		// ID 和图像数据
		WriteString(stream, "ID");
		// ID 后通常紧跟图像数据，无空白
		stream.Write(img.Data.Buffer, img.Data.Offset, img.Data.Length);
		// EI
		WriteString(stream, "EI");
	}

	private static void WriteOperator(Stream stream, string opName) {
		WriteString(stream, opName);
	}

	private static void WriteString(Stream stream, string s) {
		byte[] bytes = Latin1.GetBytes(s);
		stream.Write(bytes, 0, bytes.Length);
	}

	private static readonly char[] HexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

	private static string EncodeName(string name) {
		// 快速路径：检测是否需要转义，同时统计转义次数
		int escapeCount = 0;
		int firstEscape = -1;
		for (int i = 0; i < name.Length; i++) {
			char c = name[i];
			if (c <= 0x20 || c >= 0x7F || c == '#' || IsDelimiter(c)) {
				if (firstEscape == -1) firstEscape = i;
				escapeCount++;
			}
		}

		// 无需转义时直接返回原字符串
		if (escapeCount == 0)
			return name;

		// 预分配容量：每个转义字符增加2个字符（#XX 比原字符多2）
		var sb = new StringBuilder(name.Length + escapeCount * 2);

		int segmentStart = 0;
		for (int i = 0; i < name.Length; i++) {
			char c = name[i];
			if (c <= 0x20 || c >= 0x7F || IsDelimiter(c)) {
				// 批量追加之前的不需转义片段
				if (segmentStart < i)
					sb.Append(name, segmentStart, i - segmentStart);
				// 追加转义序列（避免临时字符串）
				int val = c;
				sb.Append('#');
				sb.Append(HexChars[val >> 4]);
				sb.Append(HexChars[val & 0xF]);
				segmentStart = i + 1;
			}
		}
		// 追加最后剩余的不需转义片段
		if (segmentStart < name.Length)
			sb.Append(name, segmentStart, name.Length - segmentStart);

		return sb.ToString();
	}

	private static bool IsDelimiter(char ch) {
		return ch is '#' or '(' or ')' or '<' or '>' or
			   '[' or ']' or '{' or '}' or
			   '/' or '%';
	}

	#endregion
}