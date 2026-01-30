using System;
using System.Reflection;
using System.Reflection.Emit;
using FreeImageAPI;

namespace PDFPatcher.Processor.Imaging
{
	static class FreeImageHelper
	{
		public static bool UseDib(this FreeImageBitmap bmp, Func<FIBITMAP, FIBITMAP> handler) {
			if (handler != null) {
				var t = bmp.Tag; // ensure not disposed
				var dib = GetDib(bmp);
				dib = handler(dib);
				return ReplaceDib(bmp, dib);
			}
			return false;
		}

		public static FreeImageBitmap CreateFromBytes(byte[] bytes, FREE_IMAGE_LOAD_FLAGS flags = default) {
			return new FreeImageBitmap(new System.IO.MemoryStream(bytes), flags);
		}
		public static FreeImageBitmap CreateFromBytes(byte[] bytes, FREE_IMAGE_FORMAT format) {
			return new FreeImageBitmap(new System.IO.MemoryStream(bytes), format);
		}

		static readonly Func<FreeImageBitmap, FIBITMAP> GetDib = CreateGetDibMethod();
		static readonly Func<FreeImageBitmap, FIBITMAP, bool> ReplaceDib = CreateReplaceDibMethod();

		static Func<FreeImageBitmap, FIBITMAP> CreateGetDibMethod() {
			var m = new DynamicMethod("GetDib", typeof(FIBITMAP), [typeof(FreeImageBitmap)], true);
			var il = m.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, typeof(FreeImageBitmap).GetField("dib", BindingFlags.Instance | BindingFlags.NonPublic));
			il.Emit(OpCodes.Ret);
			return (Func<FreeImageBitmap, FIBITMAP>)m.CreateDelegate(typeof(Func<FreeImageBitmap, FIBITMAP>));
		}

		static Func<FreeImageBitmap, FIBITMAP, bool> CreateReplaceDibMethod() {
			var m = new DynamicMethod("ReplaceDib", typeof(bool), [typeof(FreeImageBitmap), typeof(FIBITMAP)], true);
			var il = m.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.EmitCall(OpCodes.Callvirt, typeof(FreeImageBitmap).GetMethod("ReplaceDib", BindingFlags.Instance | BindingFlags.NonPublic), null);
			il.Emit(OpCodes.Ret);
			return (Func<FreeImageBitmap, FIBITMAP, bool>)m.CreateDelegate(typeof(Func<FreeImageBitmap, FIBITMAP, bool>));
		}
	}
}
