using System;
using System.Collections.Generic;
using PDFPatcher.Common;

namespace PDFPatcher.Model;

internal sealed class PageRangeCollection : List<PageRange>
{
	private PageRangeCollection() { }

	public int TotalPages {
		get {
			int c = 0;
			foreach (PageRange item in this) {
				c += item.Count;
			}

			return c;
		}
	}

	public bool IsInRange(int value) {
		foreach (PageRange item in this) {
			if ((item.StartValue < item.EndValue && value >= item.StartValue && value <= item.EndValue)
				|| (value >= item.EndValue && value <= item.StartValue)) {
				return true;
			}
		}

		return false;
	}

	internal void Collapse(int minValue, int maxValue) {
		if (maxValue < minValue) {
			throw new ArgumentException("maxValue must greater than minValue");
		}

		for (int i = Count - 1; i >= 0; i--) {
			PageRange r = this[i];
			SetReverseNumber(ref r.StartValue, maxValue);
			SetReverseNumber(ref r.EndValue, maxValue);
			if (r.StartValue < r.EndValue) {
				if (r.EndValue < minValue || maxValue < r.StartValue) {
					RemoveAt(i);
					continue;
				}

				if (minValue <= r.StartValue && r.EndValue <= maxValue) {
					continue;
				}

				if (r.StartValue < minValue) {
					r.StartValue = minValue;
				}

				if (r.EndValue > maxValue) {
					r.EndValue = maxValue;
				}
			}
			else /*if (r.StartValue >= r.EndValue)*/ {
				if (r.StartValue < minValue || maxValue < r.EndValue) {
					RemoveAt(i);
					continue;
				}

				if (maxValue >= r.StartValue && r.EndValue >= minValue) {
					continue;
				}

				if (r.EndValue < minValue) {
					r.EndValue = minValue;
				}

				if (r.StartValue > maxValue) {
					r.StartValue = maxValue;
				}
			}

			this[i] = r;
		}
	}

	internal static PageRangeCollection CreateSingle(int minValue, int maxValue) {
		PageRangeCollection r = new() { new PageRange(minValue, maxValue) };
		return r;
	}

	internal static PageRangeCollection Parse(string rangeText, int minValue, int maxValue, bool addDefaultRange) {
		PageRangeCollection r = new();
		if (string.IsNullOrEmpty(rangeText) == false) {
			string[] ranges = rangeText.Split(',', ';', ' ', '\t');
			string startRange, endRange;
			int startNum, endNum;

			foreach (string range in ranges) {
				if (range.Length == 0) {
					continue;
				}

				startNum = endNum = 0;
				int rangeIndicator = range.Length > 1 ? range.IndexOf('-', 1) /*排除首位可能是负数页码的可能*/ : -1;
				if (rangeIndicator > 0) {
					startRange = range.Substring(0, rangeIndicator);
					endRange = range.Substring(rangeIndicator + 1, range.Length - rangeIndicator - 1);
					if (startRange.TryParse(out startNum) && endRange.TryParse(out endNum) && startNum != 0 &&
						endNum != 0) {
						SetReverseNumber(ref startNum, maxValue);
						SetReverseNumber(ref endNum, maxValue);
						if (startNum < 0 || endNum < 0) {
							continue;
						}

						r.Add(new PageRange(startNum, endNum));
					}
				}
				else if (range.TryParse(out startNum)) {
					SetReverseNumber(ref startNum, maxValue);
					if (startNum < 0) {
						continue;
					}

					r.Add(new PageRange(startNum, startNum));
				}
			}
		}

		if (r.Count == 0 && addDefaultRange) {
			r.Add(new PageRange(minValue, maxValue));
		}
		else {
			r.Collapse(minValue, maxValue);
		}

		return r;
	}

	private static void SetReverseNumber(ref int refNum, int maxNum) {
		if (refNum < 0) {
			refNum = refNum + maxNum + 1;
		}
	}

	public override string ToString() {
		return string.Join(";", ConvertAll(r => { return r.ToString(); }).ToArray());
	}
}