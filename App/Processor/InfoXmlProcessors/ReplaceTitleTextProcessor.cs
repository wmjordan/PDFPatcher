﻿using System;
using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class ReplaceTitleTextProcessor : IPdfInfoXmlProcessor
{
	private static readonly BookmarkMatcher.SimpleReplacer __replacer = new();

	private readonly BookmarkMatcher _matcher;
	private readonly string _replacement;

	public ReplaceTitleTextProcessor(string replacement) {
		_matcher = __replacer;
		_replacement = replacement;
	}

	public ReplaceTitleTextProcessor(BookmarkMatcher matcher, string replacement) {
		if (matcher == null) {
			throw new ArgumentNullException("matcher");
		}

		_matcher = matcher;
		_replacement = replacement;
	}

	#region IInfoDocProcessor 成员

	public string Name => string.Concat("替换文本为“", _replacement, "”");

	public IUndoAction Process(XmlElement item) {
		XmlAttribute a = item.GetAttributeNode(Constants.BookmarkAttributes.Title);
		if (a == null) {
			return null;
		}

		return _matcher.Replace(item, _replacement);
	}

	#endregion
}