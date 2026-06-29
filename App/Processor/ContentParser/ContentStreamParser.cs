using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CLR;
using PDFPatcher.Common;

namespace PDFPatcher.Processor.ContentParser;

sealed class ContentStreamParser
{
	byte[] _buffer;
	int _position;
	int _length;

	public static IEnumerable<Operation> Parse(byte[] contentBytes) => new ContentStreamParser().InternalParse(contentBytes);

	IEnumerable<Operation> InternalParse(byte[] contentBytes) {
		_buffer = contentBytes;
		_position = 0;
		_length = _buffer.Length;

		// 使用定长数组缓冲区
		Token[] opBuffer = new Token[6];
		int opCount = 0;
		List<Token> overflowList = null;

		while (_position < _length) {
			Token token = ParseNextObject();

			if (token == null) yield break;

			// 特殊处理内联图像
			if (token.Type == TokenType.Keyword) {
				if ((string)token.Value == OperatorTable.BI.Name) {
					yield return new Operation(OperatorTable.BI, Token.EmptyTokens);
					yield return new Operation(OperatorTable.EI, [ParseInlineImageBody()]);
					opCount = 0;
					overflowList = null;
					continue;
				}

				var opInfo = OperatorTable.Resolve(_buffer, token.Offset, token.Length);

				Token[] finalOperands;
				if (overflowList is null) {
					switch (opCount) {
						case 0: finalOperands = Token.EmptyTokens; break;
						case 1: finalOperands = [opBuffer[0]]; break;
						case 2: finalOperands = [opBuffer[0], opBuffer[1]]; break;
						default:
							finalOperands = new Token[opCount];
							Array.Copy(opBuffer, 0, finalOperands, 0, opCount);
							break;
					}
				}
				else {
					finalOperands = overflowList.ToArray();
				}

				yield return new Operation(opInfo, finalOperands);

				opCount = 0;
				overflowList = null;
			}
			else if (opCount < 6) {
				opBuffer[opCount++] = token;
			}
			else {
				overflowList ??= new List<Token>(opBuffer);
				overflowList.Add(token);
			}
		}
	}

	Token ParseNextObject() {
		SkipWhiteSpaceAndComments();
		if (_position >= _length) return null;

		int start = _position;
		var typeValue = (char)_buffer[_position] switch {
			'<' => _position + 1 < _length && _buffer[_position + 1] == '<'
					? new(TokenType.Dictionary, ParseDictionary())
					: new(TokenType.HexString, ParseHexString()),
			'[' => new(TokenType.Array, ParseArray()),
			'(' => new(TokenType.String, ParseLiteralString()),
			'/' => new(TokenType.Name, ParseName()),
			'-' or '+' or '.' or '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' => ParseNumber(),
			_ => ParseKeyword(),
		};
		return new Token(_buffer, start, _position - start, typeValue.Type, typeValue.Value);
	}

	TokenTypeValue ParseNumber() {
		int start = _position;
		bool hasDecimal = false;
		bool hasExponent = false;

		if (_position < _length && _buffer[_position].CeqAny('-', '+')) {
			_position++;
		}

		while (_position < _length) {
			switch ((char)_buffer[_position]) {
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					_position++;
					continue;
				case '.':
					if (!hasDecimal && !hasExponent) {
						hasDecimal = true;
						_position++;
						continue;
					}
					break;
				case 'e':
				case 'E':
					if (!hasExponent) {
						hasExponent = true;
						_position++;
						if (_position < _length
							&& _buffer[_position].CeqAny('-', '+')) {
							_position++;
						}
						continue;
					}
					break;
			}
			break;
		}

		if (start == _position) {
			return new(TokenType.Null, null);
		}

		string numStr = Encoding.ASCII.GetString(_buffer, start, _position - start);

		if (!hasDecimal && !hasExponent) {
			if (Int64.TryParse(numStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal)) {
				return new(TokenType.Integer, longVal);
			}
		}

		if (Double.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleVal)) {
			return new(TokenType.Real, doubleVal);
		}

		return new(TokenType.Real, double.NaN);
	}

	TokenTypeValue ParseKeyword() {
		int start = _position;
		while (_position < _length) {
			byte b = _buffer[_position];
			if (IsWhiteSpace(b) || IsDelimiter(b)) break;
			_position++;
		}

		var w = Encoding.ASCII.GetString(_buffer, start, _position - start);

		return w switch {
			"true" => new(TokenType.Boolean, true),
			"false" => new(TokenType.Boolean, false),
			"null" => new(TokenType.Null, null),
			// 默认返回关键字类型
			// 如果是已知操作符，opInfo.Name 是静态常量引用，没有额外分配
			// 如果是未知操作符（且不是布尔/null），opInfo.Name 是新字符串
			_ => new(TokenType.Keyword, w),
		};
	}

	Dictionary<string, Token> ParseDictionary() {
		_position += 2;
		var dict = new Dictionary<string, Token>();

		while (_position < _length) {
			SkipWhiteSpaceAndComments();
			if (_position + 1 < _length && _buffer[_position] == '>' && _buffer[_position + 1] == '>') {
				_position += 2;
				break;
			}

			var keyToken = ParseNextObject();
			if (keyToken == null || keyToken.Type != TokenType.Name) break;

			var valToken = ParseNextObject();
			dict[(string)keyToken.Value] = valToken;
		}
		return dict;
	}

	List<Token> ParseArray() {
		_position++;
		var list = new List<Token>();

		while (_position < _length) {
			SkipWhiteSpaceAndComments();
			if (_buffer[_position] == ']') {
				_position++;
				break;
			}
			list.Add(ParseNextObject());
		}
		return list;
	}

	string ParseHexString() {
		_position++;
		var sb = new StringBuilder();
		while (_position < _length) {
			byte b = _buffer[_position];
			if (b == '>') {
				_position++;
				break;
			}
			if (!IsWhiteSpace(b)) {
				sb.Append((char)b);
			}
			_position++;
		}
		return sb.ToString();
	}

	string ParseLiteralString() {
		_position++;
		int depth = 1;
		var sb = new StringBuilder();
		bool escape = false;

		while (_position < _length) {
			byte b = _buffer[_position];
			_position++;

			if (escape) {
				if (b.IsBetween('0', '7')) {
					int octalValue = b - '0';
					int count = 0;
					while (count < 2 && _position < _length) {
						byte nextByte = _buffer[_position];
						if (nextByte.IsBetween('0', '7')) {
							octalValue = (octalValue << 3) + (nextByte - '0');
							_position++;
							count++;
						}
						else {
							break;
						}
					}
					sb.Append((char)octalValue);
				}
				else {
					switch (b) {
						case (byte)'n': sb.Append('\n'); break;
						case (byte)'r': sb.Append('\r'); break;
						case (byte)'t': sb.Append('\t'); break;
						case (byte)'b': sb.Append('\b'); break;
						case (byte)'f': sb.Append('\f'); break;
						case (byte)'(': sb.Append('('); break;
						case (byte)')': sb.Append(')'); break;
						case (byte)'\\': sb.Append('\\'); break;
						default:
							if (b != '\r' && b != '\n') sb.Append((char)b);
							break;
					}
				}
				escape = false;
			}
			else if (b == (byte)'\\') {
				escape = true;
			}
			else if (b == (byte)'(') {
				depth++;
				sb.Append('(');
			}
			else if (b == (byte)')') {
				depth--;
				if (depth == 0) break;
				sb.Append(')');
			}
			else {
				sb.Append((char)b);
			}
		}
		return sb.ToString();
	}

	string ParseName() {
		_position++;
		var sb = StringBuilderCache.Acquire(32);

		while (_position < _length) {
			byte b = _buffer[_position];
			if (IsWhiteSpace(b) || IsDelimiter(b)) break;

			_position++;
			if (b == (byte)'#') {
				if (_position + 1 < _length) {
					int val = (ParseHexDigit(_buffer[_position]) << 4) | ParseHexDigit(_buffer[_position + 1]);
					sb.Append((char)val);
					_position += 2;
				}
			}
			else {
				sb.Append((char)b);
			}
		}
		return StringBuilderCache.GetStringAndRelease(sb);
	}

	Token ParseInlineImageBody() {
		int start = _position;
		var imageDict = new Dictionary<string, Token>();

		while (_position < _length) {
			SkipWhiteSpaceAndComments();
			if (MatchKeyword("ID")) break;

			var keyToken = ParseNextObject();
			if (keyToken == null || keyToken.Type != TokenType.Name) break;

			var valToken = ParseNextObject();
			imageDict[(string)keyToken.Value] = valToken;
		}

		SkipWhiteSpace();

		int dataStart = _position;
		int end = _length - 1;
		while (_position < end) {
			if (_buffer[_position] == 'E' && _buffer[_position + 1] == 'I') {
				if (_position + 2 >= _length || IsWhiteSpaceOrDelimiter(_buffer[_position + 2])) {
					_position += 2;
					break;
				}
			}
			_position++;
		}

		var content = new InlineImageContent(imageDict, new ByteSegment(_buffer, dataStart, _position - dataStart));

		return new Token(_buffer, start, _position - start, TokenType.InlineImage, content);
	}

	bool MatchKeyword(string keyword) {
		SkipWhiteSpaceAndComments();
		var keywordLength = keyword.Length;
		if (_position + keywordLength > _length) return false;

		for (int i = 0; i < keywordLength; i++) {
			if (_buffer[_position + i] != (byte)keyword[i]) return false;
		}

		if (!IsWhiteSpaceOrDelimiter(_buffer[_position + keywordLength])) return false;

		_position += keywordLength;
		return true;
	}

	void SkipWhiteSpace() {
		while (_position < _length && IsWhiteSpace(_buffer[_position])) {
			_position++;
		}
	}

	void SkipWhiteSpaceAndComments() {
		while (_position < _length) {
			byte b = _buffer[_position];
			if (b == (byte)'%') {
				while (_position < _length
					&& !_buffer[_position].CeqAny((byte)'\r', (byte)'\n')) {
					_position++;
				}
			}
			else if (IsWhiteSpace(b)) {
				_position++;
			}
			else {
				break;
			}
		}
	}

	static bool IsWhiteSpace(byte b) {
		return b == 0 || b.IsBetween(9, 13) || b == 32;
	}

	static bool IsDelimiter(byte b) {
		return b == (byte)'(' || b == (byte)')' || b == (byte)'<' || b == (byte)'>' ||
			   b == (byte)'[' || b == (byte)']' || b == (byte)'{' || b == (byte)'}' ||
			   b == (byte)'/' || b == (byte)'%';
	}

	static bool IsWhiteSpaceOrDelimiter(byte b) {
		return IsWhiteSpace(b) || IsDelimiter(b);
	}

	static int ParseHexDigit(byte b) {
		if (b >= '0' && b <= '9') return b - '0';
		if (b >= 'A' && b <= 'F') return 10 + b - 'A';
		if (b >= 'a' && b <= 'f') return 10 + b - 'a';
		return 0;
	}

	readonly struct TokenTypeValue(TokenType type, object value)
	{
		public readonly TokenType Type = type;
		public readonly object Value = value;
	}
}
