using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
	/// <summary>
	/// Reserved to be used by the compiler for tracking metadata.
	/// This class should not be used by developers in source code.
	/// This dummy class is required to make 'init' properties work 
	/// in older target frameworks like .NET Framework or .NET Standard.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class IsExternalInit
	{
	}
}
