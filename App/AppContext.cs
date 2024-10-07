using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MuPDF;
using PDFPatcher.Common;
using PowerJson;

namespace PDFPatcher
{
	internal static class AppContext
	{
		static readonly string AppConfigFilePath = FileHelper.CombinePath(
				Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),
				"AppConfig.json");

		static readonly SerializationManager JsonSm = new SerializationManager(new JsonReflectionController(true)) {
			UseExtensions = false,
			SerializeEmptyCollections = false,
			SerializeNullValues = false,
			SerializeReadOnlyFields = false,
			SerializeReadOnlyProperties = false,
			CanSerializePrivateMembers = true
		};

		internal const int MaxHistoryItemCount = 16;

		internal static MainForm MainForm { get; set; }

		static AppContext() {
			SaveAppSettings = true;
			BookmarkFile = String.Empty;
			TargetFile = String.Empty;
			CheckUpdateDate = DateTime.Now;
			CheckUpdateInterval = 14;
			Exporter = new ExporterOptions();
			Importer = new ImporterOptions();
			Merger = new MergerOptions();
			Patcher = new PatcherOptions();
			Editor = new PatcherOptions();
			Reader = new ReaderOptions();
			AutoBookmarker = new AutoBookmarkOptions();
			Encodings = new EncodingOptions();
			ImageExtracter = new ImageExtracterOptions();
			ImageRenderer = new ImageRendererOptions();
			ExtractPage = new ExtractPageOptions();
			Ocr = new OcrOptions();
			Toolbar = new ToolbarOptions();
			WindowStatus = new WindowStatus();
			Recent = new RecentItems();
		}
		public static bool SaveAppSettings { get; set; }

		///<summary>获取或指定是否在加载 PDF 文档时仅加载部分文档。</summary>
		public static bool LoadPartialPdfFile { get; set; }

		private static string[] _SourceFiles = new string[0];
		///<summary>获取或指定要处理的源文件路径列表。</summary>
		public static string[] SourceFiles {
			get => _SourceFiles;
			set => _SourceFiles = value ?? new string[0];
		}

		///<summary>获取或指定检查更新的日期。</summary>
		public static DateTime CheckUpdateDate { get; set; }
		///<summary>获取或指定检查更新的日期间隔。</summary>
		public static int CheckUpdateInterval { get; set; }

		///<summary>获取或指定是否应取消批处理操作。</summary>
		public static bool Abort { get; set; }

		///<summary>获取或指定书签文件的路径。</summary>
		public static string BookmarkFile { get; set; }

		///<summary>获取或指定目标文件的路径。</summary>
		public static string TargetFile { get; set; }

		///<summary>获取导出设置。</summary>
		public static ExporterOptions Exporter { get; internal set; }
		///<summary>获取导入设置。</summary>
		public static ImporterOptions Importer { get; internal set; }
		///<summary>获取生成文档的设置。</summary>
		public static MergerOptions Merger { get; internal set; }
		///<summary>获取生成文档的设置。</summary>
		public static PatcherOptions Patcher { get; internal set; }
		///<summary>获取文档编辑器的设置。</summary>
		public static PatcherOptions Editor { get; internal set; }
		///<summary>获取阅读器的设置。</summary>
		public static ReaderOptions Reader { get; internal set; }
		///<summary>获取自动生成书签的设置。</summary>
		public static AutoBookmarkOptions AutoBookmarker { get; internal set; }
		///<summary>获取应用程序设置。</summary>
		public static EncodingOptions Encodings { get; internal set; }
		///<summary>获取导出图像的设置。</summary>
		public static ImageExtracterOptions ImageExtracter { get; internal set; }
		///<summary>获取转换为图片的设置。</summary>
		public static ImageRendererOptions ImageRenderer { get; internal set; }
		///<summary>获取提取页面的设置。</summary>
		public static ExtractPageOptions ExtractPage { get; internal set; }
		///<summary>获取光学字符识别功能的设置。</summary>
		public static OcrOptions Ocr { get; internal set; }
		///<summary>获取或指定自定义工具栏的项目。</summary>
		public static ToolbarOptions Toolbar { get; internal set; }
		///<summary>获取或指定窗口状态。</summary>
		public static WindowStatus WindowStatus { get; internal set; }

		public static RecentItems Recent { get; internal set; }

		[JsonSerializable]
		public sealed class RecentItems
		{
			///<summary>获取最近使用的 PDF 文件列表。</summary>
			[JsonField("源文件")]
			public List<string> SourcePdfFiles { get; } = new List<string>();
			///<summary>获取最近使用的 PDF 输出文件列表。</summary>
			[JsonField("输出文件")]
			public List<string> TargetPdfFiles { get; } = new List<string>();
			///<summary>获取最近使用的信息文件列表。</summary>
			[JsonField("信息文件")]
			public List<string> InfoDocuments { get; } = new List<string>();
			///<summary>获取最近使用的文件名模板列表。</summary>
			[JsonField("文件名模板")]
			public List<string> FileNameTemplates { get; } = new List<string>();
			///<summary>获取最近使用的文件夹列表。</summary>
			[JsonField("文件夹")]
			public List<string> Folders { get; } = new List<string>();
			///<summary>获取最近使用的查找字符串列表。</summary>
			[JsonField("查找项")]
			public List<string> SearchPatterns { get; } = new List<string>();
			///<summary>获取最近使用的替换字符串列表。</summary>
			[JsonField("替换项")]
			public List<string> ReplacePatterns { get; } = new List<string>();

			internal static void AddHistoryItem(IList<string> list, string item) {
				if (String.IsNullOrEmpty(item)) {
					return;
				}
				var i = -1;
				var m = false;
				foreach (var li in list) {
					i++;
					if (String.Equals(li, item, StringComparison.OrdinalIgnoreCase)) {
						m = true;
						break;
					}
				}
				if (m) {
					if (i == 0) {
						return;
					}
					if (i != -1) {
						list.RemoveAt(i);
					}
				}
				list.Insert(0, item);
				while (list.Count > MaxHistoryItemCount) {
					list.RemoveAt(list.Count - 1);
				}
			}

		}

		internal static void CleanUpInexistentFiles(List<string> list) {
			list.RemoveAll(item => FileHelper.HasFileNameMacro(item) == false && File.Exists(item) == false);
		}

		internal static void CleanUpInexistentFolders(List<string> list) {
			list.RemoveAll(item => FileHelper.HasFileNameMacro(item) == false && Directory.Exists(item) == false);
		}

		internal static bool Load(string path) {
			return LoadJson(path);
		}

		internal static bool LoadJson(string path) {
			if (String.IsNullOrEmpty(path)) {
				path = AppConfigFilePath;
			}
			if (File.Exists(path) == false) {
				return false;
			}
			ConfigurationSerialization conf;
			try {
				conf = Json.ToObject<ConfigurationSerialization>(File.ReadAllText(path, Encoding.UTF8), JsonSm);
				if (conf == null || conf.SaveAppSettings == false) {
					SaveAppSettings = false;
					return false;
				}
			}
			catch (Exception) {
				return false;
			}
			CheckUpdateDate = conf.CheckUpdateDate;
			CheckUpdateInterval = conf.CheckUpdateInterval;
			LoadPartialPdfFile = conf.PdfLoadMode == Configuration.OptimalMemoryUsage;
			if (conf.Recent != null) {
				Recent = conf.Recent;
			}
			if (conf.ExporterOptions != null) {
				Exporter = conf.ExporterOptions;
			}
			if (conf.ImporterOptions != null) {
				Importer = conf.ImporterOptions;
			}
			if (conf.MergerOptions != null) {
				Merger = conf.MergerOptions;
			}
			if (conf.PatcherOptions != null) {
				Patcher = conf.PatcherOptions;
			}
			if (conf.EditorOptions != null) {
				Editor = conf.EditorOptions;
			}
			if (conf.ReaderOptions != null) {
				Reader = conf.ReaderOptions;
			}
			if (conf.AutoBookmarkOptions != null) {
				AutoBookmarker = conf.AutoBookmarkOptions;
			}
			if (conf.Encodings != null) {
				Encodings = conf.Encodings;
			}
			if (conf.ImageExporterOptions != null) {
				ImageExtracter = conf.ImageExporterOptions;
			}
			if (conf.ImageRendererOptions != null) {
				ImageRenderer = conf.ImageRendererOptions;
			}
			if (conf.OcrOptions != null) {
				Ocr = conf.OcrOptions;
			}
			if (conf.ExtractPageOptions != null) {
				ExtractPage = conf.ExtractPageOptions;
			}
			if (conf.ToolbarOptions != null) {
				Toolbar = conf.ToolbarOptions;
			}
			if (conf.WindowStatus != null) {
				WindowStatus = conf.WindowStatus;
			}
			return true;
		}

		/// <summary>
		/// 保存应用程序配置。
		/// </summary>
		/// <param name="path">保存路径。路径为空时，保存到默认位置。</param>
		/// <param name="saveHistoryFileList">是否保存历史文件列表。</param>
		/// <param name="skipReadonly">是否跳过只读文件。</param>
		internal static void Save(string path, bool saveHistoryFileList, bool skipReadonly) {
			try {
				SaveJson(path ?? AppConfigFilePath, saveHistoryFileList, skipReadonly);
			}
			catch (Exception ex) {
				AppContext.MainForm.ErrorBox("保存程序设置时出错", ex);
			}
		}

		static void SaveJson(FilePath path, bool saveHistoryFileList, bool skipReadonly) {
			if (skipReadonly && path.ExistsFile && (path.ToFileInfo().Attributes & FileAttributes.ReadOnly) > 0) {
				return;
			}
			var s = SaveAppSettings
				? new ConfigurationSerialization {
					SaveAppSettings = true,
					CheckUpdateDate = CheckUpdateDate,
					CheckUpdateInterval = CheckUpdateInterval,
					PdfLoadMode = LoadPartialPdfFile ? Configuration.OptimalMemoryUsage : Configuration.OptimalSpeed,
					MergerOptions = Merger,
					ExporterOptions = Exporter,
					ImporterOptions = Importer,
					PatcherOptions = Patcher,
					EditorOptions = Editor,
					ReaderOptions = Reader,
					AutoBookmarkOptions = AutoBookmarker,
					Encodings = Encodings,
					ImageExporterOptions = ImageExtracter,
					ImageRendererOptions = ImageRenderer,
					ExtractPageOptions = ExtractPage,
					OcrOptions = Ocr,
					ToolbarOptions = Toolbar,
					WindowStatus = new WindowStatus(MainForm),
					Recent = saveHistoryFileList ? Recent : null
				}
				: new ConfigurationSerialization { SaveAppSettings = false };
			path.WriteAllText(false, Encoding.UTF8, Json.ToJson(s, JsonSm));
		}

		private static void WriteRecentFiles(XmlWriter writer, IList<string> list, string name) {
			foreach (var item in list) {
				writer.WriteStartElement(name);
				writer.WriteAttributeString(Configuration.Path, item);
				writer.WriteEndElement();
			}
		}
	}
}
