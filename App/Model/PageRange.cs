using System;
using System.Collections.Generic;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	internal struct PageRange : IEnumerable<int>
	{
		public int StartValue, EndValue;
		public PageRange(int startValue, int endValue) {
			StartValue = startValue;
			EndValue = endValue;
		}
		public bool Contains(int value) {
			return value >= StartValue && value <= EndValue
				|| value >= EndValue && value <= StartValue;
		}

		public override string ToString() {
			return StartValue != EndValue ? String.Concat(StartValue.ToText(), '-', EndValue.ToText()) : StartValue.ToText();
		}

		/// <summary>
		/// 返回范围中包含的数量。
		/// </summary>
		public int Count => (EndValue > StartValue ?
					EndValue - StartValue :
					StartValue - EndValue) + 1;

		#region IEnumerable<int> 成员

		IEnumerator<int> IEnumerable<int>.GetEnumerator() {
			return new PageRangeEnumerator(StartValue, EndValue);
		}

		#endregion

		#region IEnumerable 成员

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return new PageRangeEnumerator(StartValue, EndValue);
		}

		#endregion

		sealed class PageRangeEnumerator : IEnumerator<int>
		{
			readonly int _start, _end;
			readonly bool _isIncremental;
			int _Current;

			public PageRangeEnumerator(int start, int end) {
				_start = start;
				_isIncremental = start < end;
				_end = end;
				_Current = _isIncremental ? start - 1 : start + 1;
			}
			#region IEnumerator<int> 成员

			public int Current => _Current;

			#endregion

			#region IDisposable 成员

			public void Dispose() { }

			#endregion

			#region IEnumerator 成员

			object System.Collections.IEnumerator.Current => _Current;

			public bool MoveNext() {
				if (_isIncremental && _Current < _end) {
					_Current++;
					return true;
				}
				else if (_isIncremental == false && _Current > _end) {
					_Current--;
					return true;
				}
				return false;
			}

			public void Reset() {
				_Current = _start < _end ? _start : _end;
			}

			#endregion
		}
	}
}
