#define DEBUGOCR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Processor;

public class ModiOcr
{
	//static string _modiPath;
	private const string __MachineRegistryPath = @"Installer\Components\61BA386016BD0C340BBEAC273D84FD5F";

	private const string __UserRegistryPath =
		@"Software\Microsoft\Installer\Components\61BA386016BD0C340BBEAC273D84FD5F";

	private static readonly List<int> __InstalledLanguage = DetectInstalledLanguages();

	public int LangID { get; set; }
	public bool StretchPage { get; set; }
	public bool OrientPage { get; set; }
	public WritingDirection WritingDirection { get; set; }

	internal static bool ModiInstalled { get; } = DetectModi();

	private static bool DetectModi() {
		if (__InstalledLanguage.Count == 0) {
			return false;
		}

		object ocr = Create("MODI.Document");
		if (ocr == null) {
			return false;
		}

		FinalReleaseComObjects(ocr);
		return true;
	}

	internal static bool IsLanguageInstalled(int langID) {
		return __InstalledLanguage.Contains(langID);
	}

	private static List<int> DetectInstalledLanguages() {
		List<int> l = new();
		try {
			using (RegistryKey k = Registry.CurrentUser.OpenSubKey(__UserRegistryPath)) {
				DetectInstalledLanguages(k, l);
			}

			using (RegistryKey k = Registry.ClassesRoot.OpenSubKey(__MachineRegistryPath)) {
				DetectInstalledLanguages(k, l);
			}
		}
		catch (Exception ex) {
			Tracker.DebugMessage("OCR registry error: " + ex.Message);
		}

		return l;
	}

	private static void DetectInstalledLanguages(RegistryKey k, ICollection<int> list) {
		if (k == null) {
			return;
		}

		foreach (string n in k.GetValueNames()) {
			int i = n.ToInt32();
			if (i == 0) {
				continue;
			}

			if (Constants.Ocr.LangIDs.Contains(i) && list.Contains(i) == false) {
				list.Add(i);
			}
		}
	}

	internal void Ocr(string path, string saveImagePath, List<TextLine> results) {
		object ocr = null, images = null, image = null, layout = null;
		object merge = null, mergeImages = null;
		IEnumerable words = null, rects = null;
		TextLine line = null;
		int lineID = -1, regionID = -1;
		TextInfo cti = null;
		try {
#if DEBUGOCR
			Tracker.TraceMessage("创建识别引擎对象。");
#endif
			ocr = Create("MODI.Document");
			string p = Environment.CurrentDirectory;
			//Environment.CurrentDirectory = _modiPath;
#if DEBUGOCR
			Tracker.TraceMessage("读取识别图像：" + path);
#endif
			Call(ocr, "Create", path);
#if DEBUGOCR
			Tracker.TraceMessage("执行识别：" + LangID);
#endif
			Call(ocr, "OCR", ValueHelper.MapValue(LangID, Constants.Ocr.LangIDs, Constants.Ocr.OcrLangIDs), OrientPage,
				StretchPage);

			Environment.CurrentDirectory = p;
#if DEBUGOCR
			Tracker.TraceMessage("读取识别结果。");
#endif
			images = Get(ocr, "Images");
			image = Get(images, "Item", 0);
			layout = Get(image, "Layout");
#if DEBUGOCR && DEBUG
			StreamWriter l = new(@"m:\ocr.txt", true, Encoding.Default);
			l.WriteLine("path: " + path);
#endif
			words = Get<IEnumerable>(layout, "Words");
			foreach (object word in words) {
				TextInfo ti = new();
				string w = Get<string>(word, "Text");
				ti.Text = w;
				float[] pos = new float[4];
				rects = Get<IEnumerable>(word, "Rects");
				foreach (object rect in rects) {
					int r = Get<int>(rect, "Left");
					if (r < pos[0] || pos[0] == 0) {
						pos[0] = r;
					}

					r = Get<int>(rect, "Top");
					if (r > pos[1]) {
						pos[1] = r;
					}

					r = Get<int>(rect, "Right");
					if (r > pos[2]) {
						pos[2] = r;
					}

					r = Get<int>(rect, "Bottom");
					if (r < pos[3] || pos[3] == 0) {
						pos[3] = r;
					}

					FinalReleaseComObjects(rect);
				}

				Marshal.ReleaseComObject(rects);
				ti.Region = new Bound(pos[0], pos[3], pos[2], pos[1]);
				ti.Font = null;
				ti.Size = pos[3] - pos[1];
				if (ti.Text.Length == 1) {
					switch (ti.Text[0]) {
						case '一':
							Bound r = ti.Region;
							ti.Size = r.Width > r.Height ? r.Width : r.Height;
							int s = (int)Math.Ceiling((ti.Size / 2) - ((r.Width > r.Height ? r.Height : r.Width) / 2));
							ti.Region = new Bound(r.Left, r.Bottom + s, r.Right, r.Top - s);
							break;
						case '\u2022':
							ti.Text = "·";
							break;
					}
				}

				if (char.IsLetterOrDigit(ti.Text[0]) && ti.Size > 0) {
					ti.LetterWidth = ti.Size;
				}

				int cl = Get<int>(word, "LineID");
				int cr = Get<int>(word, "RegionID");
				bool sl = cl == lineID && cr == regionID; // 处于同一行
				if (sl && WritingDirection != WritingDirection.Unknown) {
					sl = cti != null
						 && cti.Region.IsAlignedWith(ti.Region, WritingDirection);
				}

				if (sl) {
					if (cti != null && cti.Region == ti.Region) {
						cti.Text += ti.Text;
					}
					else {
						line.AddText(ti);
					}
				}
				else {
					if (line != null) {
						results.Add(line);
					}

					line = new TextLine(ti) { SuppressTextInfoArrangement = true };
					lineID = cl;
					regionID = cr;
				}

				cti = ti;
#if DEBUGOCR && DEBUG
				l.WriteLine(string.Concat(ti.Size, "\t", ti.Text, "\t", ti.Region.Top, " ", ti.Region.Left, " ",
					ti.Region.Bottom, " ", ti.Region.Right));
#endif
				FinalReleaseComObjects(word);
			}

			Marshal.ReleaseComObject(words);
			if (line != null) {
				results.Add(line);
			}
#if DEBUGOCR && DEBUG
			l.Close();
#endif
			if (FileHelper.IsPathValid(saveImagePath)) {
#if DEBUGOCR
				Tracker.TraceMessage("保存识别后图像路径：" + saveImagePath);
#endif
				if (File.Exists(saveImagePath) == false) {
					Call(ocr, "SaveAs", saveImagePath, -1);
				}
				else {
					merge = Create("MODI.Document");
					Call(merge, "Create", saveImagePath);
					mergeImages = Get(merge, "Images");
					Call(mergeImages, "Add", image, Get(mergeImages, "Item", 0));
					Call(merge, "Save");
				}
			}
#if DEBUGOCR
			Tracker.TraceMessage("完成识别操作。");
#endif
			Call(ocr, "Close", false);
		}
		finally {
#if DEBUGOCR
			Tracker.TraceMessage("释放识别引擎对象。");
#endif
			FinalReleaseComObjects(rects, words, layout, image, images, ocr);
			FinalReleaseComObjects(merge, mergeImages);
			words = rects = null;
			ocr = images = image = layout = null;
			merge = mergeImages = null;
		}
	}

	#region COMInterop

	private static readonly object[] EmptyArray = new object[0];

	private static object Create(string type) {
		Type t = Type.GetTypeFromProgID(type);
		return t == null ? null : Activator.CreateInstance(t);
	}

	private static object Call(object instance, string method, params object[] parameters) {
		try {
			return instance.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, instance, parameters);
		}
		catch (Exception ex) {
			if (ex.InnerException != null) {
				throw ex.InnerException;
			}

			throw;
		}
	}

	private static object Get(object instance, string propertyName) {
		return instance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, instance, EmptyArray);
	}

	private static T Get<T>(object instance, string propertyName) {
		return (T)Get(instance, propertyName);
	}

	private static object Get(object instance, string propertyName, int index) {
		return instance.GetType()
			.InvokeMember(propertyName, BindingFlags.GetProperty, null, instance, new object[1] { index });
	}

	private static void FinalReleaseComObjects(params object[] objs) {
		for (int i = 0; i < objs.Length; i++) {
			if (objs[i] == null) {
				continue;
			}

			try {
				int r = Marshal.ReleaseComObject(objs[i]);
				Debug.Assert(r == 0);
			}
			catch (Exception ex) {
				Debug.WriteLine("释放对象时出现错误：" + ex.Message);
			}
		}
	}

	#endregion

	//private static string FindModi () {
	//    var p = Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools", "SharedFilesDir", null) as string;
	//    if (p != null && Directory.Exists (p)) {
	//        var m = String.Concat (p, @"\MODI\12.0\");
	//        if (Directory.Exists (m)) {
	//            return m;
	//        }
	//        m = String.Concat (p, @"\MODI\11.0\");
	//        if (Directory.Exists (m)) {
	//            return m;
	//        }
	//    }
	//    throw new FileNotFoundException ("无法找到微软 Office 文档图像处理引擎。");
	//}
}