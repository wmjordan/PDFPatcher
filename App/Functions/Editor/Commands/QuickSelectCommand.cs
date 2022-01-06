using System.Drawing;
using System.Windows.Forms;
using PDFPatcher.Common;
using PDFPatcher.Model;
using PDFPatcher.Processor;

namespace PDFPatcher.Functions.Editor
{
	sealed class QuickSelectCommand : IEditorCommand
	{
		const string __N = "[0-9０-９一二三四五六七八九十〇]+";
		const string __S = @"\s*";
		const string __D = __S + @"[\.． ]";
		const string __ND = __N + __D;
		const string __NN = @"(?:\s*[^0-9０-９\.．一二三四五六七八九十〇]|$)";
		static readonly MatchPattern[] __commands = {
			new MatchPattern ("^" + __S + "第" + __S + __N + __S + "(?:部分|部)|^" + __S + "part" + __S + "[0-9]", false, false, true){ Name =  "“第N部分”" },
			new MatchPattern ("^" + __S + "第" + __S + __N + __S + "[篇卷]|^" + __S + "(?:volume|vol)" + __S + "[0-9]", false, false, true){ Name =  "“第N篇”或“第N卷”" },
			new MatchPattern ("^" + __S + "第" + __S + __N + __S + "章|^" + __S + "chapter" + __S + "[0-9]", false, false, true){ Name =  "“第N章”" },
			new MatchPattern ("^" + __S + "第" + __S + __N + __S + "节|^" + __S + "section" + __S + "[0-9]", false, false, true){ Name =  "“第N节”" },
			new MatchPattern ("^" + __S + __ND + "?" + __NN, true, false, true){ Name =  "“N.”模式" },
			new MatchPattern ("^" + __S + __ND + __ND + "?" + __NN, true, false, true){ Name =  "“N.N”模式" },
			new MatchPattern ("^" + __S + __ND + __ND + __ND + "?" + __NN, true, false, true){ Name =  "“N.N.N”模式" },
			new MatchPattern ("^" + __S + __ND + __ND + __ND + __ND + "?" + __NN, true, false, true){ Name =  "“N.N.N.N”模式" },
			new MatchPattern ("^" + __S + __ND + __ND + __ND + __ND + __ND + "?" + __NN, true, false, true){ Name =  "“N.N.N.N.N”模式" }
		};
		internal static void RegisterCommands(CommandRegistry<Controller> registry) {
			foreach (var item in __commands) {
				registry.Register(new QuickSelectCommand(item), item.Name);
			}
		}
		internal static void RegisterMenuItems(ToolStripItemCollection container) {
			foreach (var item in __commands) {
				container.Add(new ToolStripMenuItem(item.Name) { Name = item.Name });
			}
		}

		readonly BookmarkMatcher _command;

		public QuickSelectCommand(MatchPattern command) {
			_command = BookmarkMatcher.Create(command.Text, BookmarkMatcher.MatcherType.Regex, command.MatchCase, command.FullMatch);
		}

		public void Process(Controller controller, params string[] parameters) {
			controller.View.Bookmark.SearchBookmarks(_command);
		}

	}
}
