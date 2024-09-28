using System;
using PDFPatcher.Common;

namespace PDFPatcher.Processor
{
	sealed class ShiftPageProcessor : IInfoDocProcessor
	{
		public int Offset { get; set; }

		#region IInfoDocProcessor 成员

		public bool Process(System.Xml.XmlElement item) {
			var a = item.GetAttributeNode(Constants.DestinationAttributes.Page);
			if (a != null && a.Value.TryParse(out int pageNum)/* && pageNum > 0*/) {
				pageNum += Offset;
				a.Value = pageNum.ToText();
				return true;
			}
			return false;
		}

		#endregion
	}
}
