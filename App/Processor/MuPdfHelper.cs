using System;
using System.Text;

namespace PDFPatcher.Processor;

internal static class MuPdfHelper
{

	internal static MuPDF.Document OpenMuDocument(string sourceFile) {
		var d = MuPDF.Document.Open(sourceFile);
		if (d.NeedsPassword) {
			var authenticated = false;
			if (PdfHelper.PasswordCache.TryGetValue(sourceFile, out byte[] password)) {
				authenticated = d.CheckPassword(password != null ? Encoding.Default.GetString(password) : String.Empty);
			}
			while (!authenticated) {
				using (var f = new PasswordEntryForm(sourceFile)) {
					if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) {
						throw new MuPDF.MuException("密码错误，没有权限打开 PDF 文件。");
					}
					PdfHelper.PasswordCache[sourceFile] = password = Encoding.Default.GetBytes(f.Password);
				}
				authenticated = d.CheckPassword(password != null ? Encoding.Default.GetString(password) : String.Empty);
			}
		}
		return d;
	}
}