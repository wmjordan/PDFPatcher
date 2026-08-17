using System;

namespace PDFPatcher.Common;

/// <summary>表示带有附加数据的 <see cref="EventArgs"/>。</summary>
/// <typeparam name="TData">附加事件数据的类型。</typeparam>
/// <remarks>使用附加的事件数据初始化 <see cref="EventArgs{TData}"/> 实例。</remarks>
/// <param name="data">附加的事件数据。</param>
public class EventArgs<TData>(TData data) : EventArgs
{
	/// <summary>获取附加的事件数据。</summary>
	public TData Data { get; } = data;
}
