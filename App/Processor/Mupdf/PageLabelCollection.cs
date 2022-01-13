﻿using System.Collections;
using System.Collections.Generic;

namespace MuPdfSharp;

public sealed class PageLabelCollection : ICollection<PageLabel>
{
	private readonly List<PageLabel> _labels = new();

	internal PageLabelCollection(MuDocument document) {
		List<PageLabel> pl = new();
		MuPdfArray l = document.Trailer.Locate("Root/PageLabels/Nums").AsArray();
		if (l.Kind == MuPdfObjectKind.PDF_ARRAY) {
			for (int i = 0; i < l.Count; i++) {
				int n = l[i].IntegerValue;
				MuPdfDictionary d = l[++i].AsDictionary();
				int sp = d["St"].IntegerValue;
				string p = d["P"].StringValue;
				string s = d["S"].NameValue;
				pl.Add(new PageLabel(n, sp, p, s.Length == 0 ? PageLabelStyle.Digit : (PageLabelStyle)(byte)s[0]));
			}

			pl.Sort();
		}

		_labels = pl;
	}

	public PageLabel this[int index] => _labels[index];

	/// <summary>
	///     添加页码标签。如集合中存在相同页码的页码标签，则先将旧的标签删除，再添加新的页码标签。
	/// </summary>
	/// <param name="label">需要添加的页码标签。</param>
	public void Add(PageLabel label) {
		Remove(label);
		_labels.Add(label);
		_labels.Sort();
	}

	public void Clear() {
		_labels.Clear();
	}

	/// <summary>
	///     返回集合中是否包含具有与 <paramref name="item" /> 相同起始页码的页码标签。
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
	///     删除集合中具有与 <paramref name="item" /> 相同起始页码的页码标签。
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

	IEnumerator IEnumerable.GetEnumerator() {
		return _labels.GetEnumerator();
	}

	/// <summary>
	///     根据传入的页码，返回当前页码标签集合格式化后生成的页码。
	/// </summary>
	/// <param name="pageNumber">绝对页码。</param>
	/// <returns>格式化后的页码文本。</returns>
	public string Format(int pageNumber) {
		int l = _labels.Count;
		if (l == 0) {
			return string.Empty;
		}

		for (int i = l - 1; i >= 0; i--) {
			PageLabel p = _labels[i];
			if (pageNumber > p.FromPageNumber) {
				return p.Format(pageNumber);
			}
		}

		return string.Empty;
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
}