
namespace PDFPatcher.Processor
{
	sealed class SetTextStyleProcessor : IPdfInfoXmlProcessor
	{
		public enum Style
		{
			SetBold = 0, SetItalic = 1, RemoveBold = 10, RemoveItalic = 11
		}

		readonly Style _style;
		public SetTextStyleProcessor(System.Xml.XmlElement element, Style style) {
			var s = element.GetAttribute(Constants.BookmarkAttributes.Style);
			if (style == Style.SetBold) {
				_style = s != Constants.BookmarkAttributes.StyleType.Bold && s != Constants.BookmarkAttributes.StyleType.BoldItalic
					? Style.SetBold
					: Style.RemoveBold;
			}
			else if (style == Style.SetItalic) {
				_style = s != Constants.BookmarkAttributes.StyleType.Italic && s != Constants.BookmarkAttributes.StyleType.BoldItalic
					? Style.SetItalic
					: Style.RemoveItalic;
			}
		}

		#region IInfoDocProcessor 成员

		public string Name {
			get {
				return _style switch {
					Style.SetBold => "设置书签文本为粗体",
					Style.SetItalic => "设置书签文本为斜体",
					Style.RemoveBold => "清除书签文本粗体样式",
					Style.RemoveItalic => "清除书签文本斜体样式",
					_ => "",
				};
			}
		}

		public IUndoAction Process(System.Xml.XmlElement item) {
			var value = item.GetAttribute(Constants.BookmarkAttributes.Style);
			var style = 0;
			switch (value) {
				case Constants.BookmarkAttributes.StyleType.Bold: style = 1; break;
				case Constants.BookmarkAttributes.StyleType.Italic: style = 2; break;
				case Constants.BookmarkAttributes.StyleType.BoldItalic: style = 3; break;
				default:
					break;
			}
			switch (_style) {
				case Style.SetBold:
					if ((style & 0x01) > 0) {
						return null;
					}
					style |= 0x01;
					break;
				case Style.SetItalic:
					if ((style & 0x02) > 0) {
						return null;
					}
					style |= 0x02;
					break;
				case Style.RemoveBold:
					if ((style & 0x01) == 0) {
						return null;
					}
					style ^= 0x01;
					break;
				case Style.RemoveItalic:
					if ((style & 0x02) == 0) {
						return null;
					}
					style ^= 0x02;
					break;
				default: throw new System.ArgumentOutOfRangeException("Style");
			}
			value = style switch {
				1 => Constants.BookmarkAttributes.StyleType.Bold,
				2 => Constants.BookmarkAttributes.StyleType.Italic,
				3 => Constants.BookmarkAttributes.StyleType.BoldItalic,
				_ => null,
			};
			return UndoAttributeAction.GetUndoAction(item, Constants.BookmarkAttributes.Style, value);
		}

		#endregion
	}
}
