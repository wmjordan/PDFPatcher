﻿using System.Xml.Serialization;

namespace PDFPatcher.Model
{
	public enum WritingDirection
	{
		[XmlEnum("混合")]
		Unknown,
		[XmlEnum("横排")]
		Horizontal,
		[XmlEnum("竖排")]
		Vertical
	}
}
