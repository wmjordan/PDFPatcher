using System;
using System.Collections.Generic;
using System.Diagnostics;
using PDFPatcher.Common;

namespace MuPDF
{
	public sealed class PageLabelCollection : ICollection<PageLabel>
	{
		readonly List<PageLabel> _labels = new List<PageLabel>();

		public PageLabel this[int index] => _labels[index];

		internal PageLabelCollection() {}
		internal PageLabelCollection(Document document) {
			var pl = new List<PageLabel>();
			var l = document.Trailer.Locate(PdfNames.Root, PdfNames.PageLabels, PdfNames.Nums);
			if (l.IsArray) {
				var a = l as PdfArray;
				for (int i = 0; i < a.Count; i++) {
					var n = a[i].IntegerValue;
					var d = a[++i].UnderlyingObject as PdfDictionary;
					var sp = d[PdfNames.St].IntegerValue;
					var p = d[PdfNames.P] as PdfString;
					pl.Add(new PageLabel(n, sp, p?.Value, d[PdfNames.S] is PdfName s ? (PageLabelStyle)(byte)s.Name[0] : PageLabelStyle.Default));
				}
				pl.Sort();
			}
			_labels = pl;
		}

		/// <summary>
		/// 添加页码标签。如集合中存在相同页码的页码标签，则先将旧的标签删除，再添加新的页码标签。
		/// </summary>
		/// <param name="label">需要添加的页码标签。</param>
		public void Add(PageLabel label) {
			Remove(label);
			_labels.Add(label);
			_labels.Sort();
		}

		/// <summary>
		/// 根据传入的页码，返回当前页码标签集合格式化后生成的页码。
		/// </summary>
		/// <param name="pageNumber">绝对页码。</param>
		/// <returns>格式化后的页码文本。</returns>
		public string Format(int pageNumber) {
			var l = _labels.Count;
			if (l == 0) {
				return String.Empty;
			}
			for (int i = l - 1; i >= 0; i--) {
				var p = _labels[i];
				if (pageNumber > p.FromPageNumber) {
					return p.Format(pageNumber);
				}
			}
			return String.Empty;
		}

		public PageLabel Find(int pageNumber) {
			--pageNumber;
			for (int i = _labels.Count - 1; i >= 0; i--) {
				if (_labels[i].FromPageNumber == pageNumber) {
					return _labels[i];
				}
			}
			return PageLabel.Empty;
		}

		public void Clear() {
			_labels.Clear();
		}

		/// <summary>
		/// 返回集合中是否包含具有与 <paramref name="item"/> 相同起始页码的页码标签。
		/// </summary>
		/// <param name="item">需要检查起始页码的页码标签。</param>
		/// <returns>如包含相同页码的页码标签，返回 true，否则返回 false。</returns>
		public bool Contains(PageLabel item) {
			for (int i = _labels.Count - 1; i >= 0; i--) {
				if (_labels[i].FromPageNumber == item.FromPageNumber) {
					return true;
				}
			}
			return false;
		}

		public void CopyTo(PageLabel[] array, int arrayIndex) {
			_labels.CopyTo(array, arrayIndex);
		}

		public int Count => _labels.Count;

		public bool IsReadOnly => false;

		/// <summary>
		/// 删除集合中具有与 <paramref name="item"/> 相同起始页码的页码标签。
		/// </summary>
		/// <param name="item">需要删除的页码标签。</param>
		/// <returns>如包含相同页码的页码标签，返回 true，否则返回 false。</returns>
		public bool Remove(PageLabel item) {
			for (int i = _labels.Count - 1; i >= 0; i--) {
				if (_labels[i].FromPageNumber == item.FromPageNumber) {
					_labels.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public IEnumerator<PageLabel> GetEnumerator() {
			return _labels.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _labels.GetEnumerator();
		}
	}

	[DebuggerDisplay("From: {FromPageNumber}, Format: {Format (1)}")]
	public readonly struct PageLabel : IComparable<PageLabel>
	{
		public readonly string Prefix;
		public readonly PageLabelStyle NumericStyle;
		public readonly int StartAt;
		public readonly int FromPageNumber;
		public static PageLabel Empty = new PageLabel(-1, -1, null, PageLabelStyle.Default);
		public bool IsEmpty => FromPageNumber < 0;

		public PageLabel(int pageNumber, int startAt, string prefix, PageLabelStyle numericStyle) {
			FromPageNumber = pageNumber;
			StartAt = startAt;
			Prefix = prefix;
			NumericStyle = numericStyle;
		}

		int IComparable<PageLabel>.CompareTo(PageLabel other) {
			return FromPageNumber.CompareTo(other.FromPageNumber);
		}

		public string Format(int pageNumber) {
			var n = pageNumber - FromPageNumber + (StartAt < 1 ? 0 : StartAt - 1);
			switch (NumericStyle) {
				case PageLabelStyle.Default:
					return Prefix;
				case PageLabelStyle.Digit:
					return String.Concat(Prefix, n.ToText());
				case PageLabelStyle.UpperRoman:
					return String.Concat(Prefix, n.ToRoman());
				case PageLabelStyle.LowerRoman:
					return String.Concat(Prefix, n.ToRoman()).ToLowerInvariant();
				case PageLabelStyle.UpperAlphabetic:
					return String.Concat(Prefix, n.ToAlphabet(true));
				case PageLabelStyle.LowerAlphabetic:
					return String.Concat(Prefix, n.ToAlphabet(false));
				default:
					goto case PageLabelStyle.Digit;
			}
		}
	}

	public enum PageLabelStyle : byte
	{
		Default = (byte)'-',
		Digit = (byte)'d',
		UpperRoman = (byte)'R',
		LowerRoman = (byte)'r',
		UpperAlphabetic = (byte)'A',
		LowerAlphabetic = (byte)'a'
	}
}
