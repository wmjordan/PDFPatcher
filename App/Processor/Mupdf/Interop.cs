using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace MuPdfSharp
{
	internal struct NativeObject<T> where T : struct
	{
		internal IntPtr Ptr;
		internal T Data;
		public NativeObject (IntPtr ptr) {
			Ptr = ptr;
			Data = ptr.MarshalAs<T> ();
		}
	}

	static class Interop
	{
		internal interface ILinkedList
		{
			IntPtr Next { get; }
		}

		internal static T MarshalAs<T> (this IntPtr ptr) where T : struct {
			return (T)Marshal.PtrToStructure (ptr, typeof (T));
		}

		internal static bool IsValid (this SafeHandle handle) {
			return handle != null && (handle.IsInvalid == true || handle.IsClosed == false);
		}

		internal static void DisposeHandle (this SafeHandle handle) {
			if (handle != null && handle.IsValid ()) {
				handle.Dispose ();
				//handle.SetHandleAsInvalid ();
			}
		}

		/// <summary>
		/// 将链表指针转换为 <see cref="IEnumerable{T}"/>
		/// </summary>
		/// <typeparam name="T">链表的类型。</typeparam>
		/// <param name="ptr">需要转换的指针。</param>
		/// <returns><see cref="IEnumerable{T}"/> 实例。</returns>
		internal static IEnumerable<NativeObject<T>> EnumerateLinkedList<T> (this IntPtr ptr) where T : struct, ILinkedList {
			return NativeLinkedList<T>.EnumerateLinkedList (ptr);
		}

		/// <summary>
		/// 将链表指针转换为 <see cref="IEnumerable{T}"/>
		/// </summary>
		/// <typeparam name="T">链表的类型。</typeparam>
		/// <param name="ptr">需要转换的指针。</param>
		/// <returns><see cref="IEnumerable{T}"/> 实例。</returns>
		internal static IEnumerable<NativeObject<T>> EnumerateLinkedList<T> (this SafeHandle ptr) where T : struct, ILinkedList {
			return NativeLinkedList<T>.EnumerateLinkedList (ptr.DangerousGetHandle ());
		}

		internal unsafe static string DecodeUtf8String (IntPtr chars) {
			var b = (byte*)chars.ToPointer ();
			using (var buffer = new System.IO.MemoryStream ()) {
				while (*b != 0) {
					buffer.WriteByte (*b);
					++b;
				}
				buffer.Position = 0;
				return System.Text.Encoding.UTF8.GetString (buffer.GetBuffer (), (int)buffer.Position, (int)buffer.Length);
			}
		}

		static class NativeLinkedList<T> where T : struct, Interop.ILinkedList
		{
			internal static IEnumerable<NativeObject<T>> EnumerateLinkedList (IntPtr pointer) {
				return new LinkedListEnumerable<T> (pointer);
			}

			sealed class LinkedListEnumerable<Node> : IEnumerable<NativeObject<Node>> where Node : struct, Interop.ILinkedList
			{
				IntPtr _pointer;
				public LinkedListEnumerable (IntPtr pointer) {
					_pointer = pointer;
				}
				IEnumerator<NativeObject<Node>> IEnumerable<NativeObject<Node>>.GetEnumerator () {
					return new LinkedListEnumerator<Node> (_pointer);
				}

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
					return new LinkedListEnumerator<Node> (_pointer);
				}
			}

			sealed class LinkedListEnumerator<Node> : IEnumerator<NativeObject<Node>> where Node : struct, Interop.ILinkedList
			{
				IntPtr _start, _current;
				NativeObject<Node> _Node;
				public LinkedListEnumerator (IntPtr pointer) {
					_current = _start = pointer;
				}

				#region IEnumerator<NativeObject<Node>>
				NativeObject<Node> IEnumerator<NativeObject<Node>>.Current {
					get { return _Node; }
				}

				void IDisposable.Dispose () {
				}

				object System.Collections.IEnumerator.Current {
					get { return _Node; }
				}

				bool System.Collections.IEnumerator.MoveNext () {
					if (_current == IntPtr.Zero) {
						return false;
					}
					_Node = new NativeObject<Node> (_current);
					_current = _Node.Data.Next;
					return true;
				}

				void System.Collections.IEnumerator.Reset () {
					_current = _start;
				}
				#endregion
			}

		}

	}

}
