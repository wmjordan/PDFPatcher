using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MuPdfSharp
{
	public sealed class PageLabelCollection : ICollection<PageLabel>
	{
		readonly List<PageLabel> _labels = new List<PageLabel>();

		public PageLabel this[int index] => _labels[index];

		internal PageLabelCollection(MuDocument document) {
			var pl = new List<PageLabel>();
			var l = document.Trailer.Locate("Root/PageLabels/Nums").AsArray();
			if (l.Kind == MuPdfObjectKind.PDF_ARRAY) {
				for (int i = 0; i < l.Count; i++) {
					var n = l[i].IntegerValue;
					var d = l[++i].AsDictionary();
					var sp = d["St"].IntegerValue;
					var p = d["P"].StringValue;
					var s = d["S"].NameValue;
					pl.Add(new PageLabel(n, sp, p, s.Length == 0 ? PageLabelStyle.Default : (PageLabelStyle)(byte)s[0]));
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
}
