using System;
using System.Collections.Generic;
using System.Text;

namespace MuPdfSharp
{
	public class MuDocumentInfo
	{
		MuPdfDictionary _info;
		public MuDocumentInfo (MuPdfDictionary dictionary) {
			_info = dictionary;
		}
		public string Title { get { return _info["Title"].StringValue; } }
		public string Subject { get { return _info["Subject"].StringValue; } }
		public string Producer { get { return _info["Producer"].StringValue; } }
		public string Creator { get { return _info["Creator"].StringValue; } }
		public string Author { get { return _info["Author"].StringValue; } }
		public string Keywords { get { return _info["Keywords"].StringValue; } }
		public string CreationDate { get { return _info["CreationDate"].StringValue; } }
		public string ModificationDate { get { return _info["ModDate"].StringValue; } }
	}
}
