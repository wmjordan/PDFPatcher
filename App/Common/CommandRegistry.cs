using System;
using System.Collections.Generic;
using System.Text;

namespace PDFPatcher.Common
{
	/// <summary>
	/// 表示在指定上下文下执行的处理命令。
	/// </summary>
	/// <typeparam name="P">处理命令时的上下文类型。</typeparam>
	interface ICommand<P>
	{
		void Process(P context, params string[] parameters);
	}

	/// <summary>
	/// 不区分字符串大小写匹配的容器集合。用于编辑器命令模式。
	/// </summary>
	/// <typeparam name="P">命令模式的处理参数类型。</typeparam>
	sealed class CommandRegistry<P>
	{
		readonly Dictionary<string, ICommand<P>> _container = new Dictionary<string, ICommand<P>>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// 注册执行处理的命令处理器。
		/// </summary>
		/// <param name="command">执行命令的处理器。</param>
		/// <param name="commandIDs">触发该命令的命令标识符。</param>
		public void Register(ICommand<P> command, params string[] commandIDs) {
			foreach (var cmd in commandIDs) {
				_container.Add(cmd, command);
			}
		}

		/// <summary>
		/// 执行指定的命令。
		/// </summary>
		/// <param name="commandID">命令标识符。</param>
		/// <param name="context">处理命令时的上下文变量。</param>
		/// <param name="parameters">参数。</param>
		/// <returns>如找到对应的命令处理，则返回 true，否则返回 false。</returns>
		public bool Process(string commandID, P context, params string[] parameters) {
			if (_container.TryGetValue(commandID, out ICommand<P> cmd)) {
				cmd.Process(context, parameters);
				return true;
			}
			return false;
		}
	}
}
