using System;
using System.Collections.Generic;
using System.Text;
#nullable disable
namespace PDFPatcher.Processor
{
	sealed class NullXmlWriter : System.Xml.XmlWriter
	{
		public override void Close() {

		}

		public override void Flush() {

		}

		public override string LookupPrefix(string ns) {
			return String.Empty;
		}

		public override void WriteBase64(byte[] buffer, int index, int count) {

		}

		public override void WriteCData(string text) {

		}

		public override void WriteCharEntity(char ch) {

		}

		public override void WriteChars(char[] buffer, int index, int count) {

		}

		public override void WriteComment(string text) {

		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset) {

		}

		public override void WriteEndAttribute() {

		}

		public override void WriteEndDocument() {

		}

		public override void WriteEndElement() {

		}

		public override void WriteEntityRef(string name) {

		}

		public override void WriteFullEndElement() {

		}

		public override void WriteProcessingInstruction(string name, string text) {

		}

		public override void WriteRaw(string data) {

		}

		public override void WriteRaw(char[] buffer, int index, int count) {

		}

		public override void WriteStartAttribute(string prefix, string localName, string ns) {

		}

		public override void WriteStartDocument(bool standalone) {

		}

		public override void WriteStartDocument() {

		}

		public override void WriteStartElement(string prefix, string localName, string ns) {

		}

		public override System.Xml.WriteState WriteState => System.Xml.WriteState.Content;

		public override void WriteString(string text) {

		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar) {

		}

		public override void WriteWhitespace(string ws) {

		}
	}
}
