using System;
using System.Collections.Generic;
using System.Text;

namespace MuPdfSharp;

[Serializable]
public sealed class MuPdfException : Exception
{
	public MuPdfException() { }
	public MuPdfException(string message) : base(message) { }

	public MuPdfException(string message, Exception inner) : base(message, inner) { }
	//protected MuPdfException (
	//  System.Runtime.Serialization.SerializationInfo info,
	//  System.Runtime.Serialization.StreamingContext context)
	//	: base (info, context) { }
}