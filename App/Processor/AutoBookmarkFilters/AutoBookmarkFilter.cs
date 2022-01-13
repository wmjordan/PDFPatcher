using System;

namespace PDFPatcher.Processor
{
	public abstract class AutoBookmarkFilter
	{
		/// <summary>
		/// 检查传入的 <see cref="Model.AutoBookmarkContext"/> 是否符合指定的过滤条件。
		/// </summary>
		/// <param name="text">包含需要过滤的文本信息及其它上下文的 <see cref="Model.AutoBookmarkContext"/>。</param>
		/// <returns>过滤结果。</returns>
		internal abstract bool Matches(Model.AutoBookmarkContext context);

		/// <summary>
		/// 重置过滤器的内部状态。
		/// </summary>
		internal abstract void Reset();
	}
}