﻿using System.Xml;

namespace PDFPatcher.Processor;

internal sealed class NullXmlWriter : XmlWriter
{
	public override WriteState WriteState => WriteState.Content;

	public override void Close() {
	}

	public override void Flush() {
	}

	public override string LookupPrefix(string ns) {
		return string.Empty;
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

	public override void WriteString(string text) {
	}

	public override void WriteSurrogateCharEntity(char lowChar, char highChar) {
	}

	public override void WriteWhitespace(string ws) {
	}
}