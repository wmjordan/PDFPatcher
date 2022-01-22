using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace PDFPatcher.Common;

/// <summary>用于创建或管理快捷方式文件的类。</summary>
public sealed class ShortcutFile
{
	private readonly IShellLink _link;

	private ShortcutFile() {
		_link = (IShellLink)new ShellLink();
	}

	/// <summary>创建快捷方式。</summary>
	/// <param name="destination">快捷方式指向的目标文件路径。</param>
	public ShortcutFile(string destination) {
		_link = (IShellLink)new ShellLink();
		Destination = destination;
		_link.SetPath(destination);
	}

	/// <summary>获取或设置快捷方式的目标路径。</summary>
	public string Destination { get; private set; }

	/// <summary>获取或设置快捷方式的工作目录。</summary>
	public string WorkingDirectory { get; set; }

	/// <summary>获取或设置快捷方式的描述文本。</summary>
	public string Description { get; set; }

	/// <summary>获取或设置快捷方式的启动参数。</summary>
	public string Arguments { get; set; }

	/// <summary>获取或设置快捷方式的图标文件位置。</summary>
	public string IconLocation { get; set; }

	/// <summary>获取或设置快捷方式的图标文件索引。</summary>
	public int IconIndex { get; set; }

	/// <summary>加载快捷方式。</summary>
	/// <param name="shortcutFilePath">快捷方式文件的位置。</param>
	/// <returns><see cref="ShortcutFile" /> 实例。</returns>
	public static ShortcutFile Load(string shortcutFilePath) {
		ShortcutFile s = new();
		IShellLink l = s._link;
		IPersistFile file = (IPersistFile)s._link;
		file.Load(shortcutFilePath, 0);
		s.Destination = shortcutFilePath;
		StringBuilder sb = new();
		l.GetDescription(sb, 512);
		s.Description = sb.ToString();
		sb.Length = 0;
		l.GetWorkingDirectory(sb, 256);
		s.WorkingDirectory = sb.ToString();
		sb.Length = 0;
		l.GetIconLocation(sb, 256, out int _);
		s.IconLocation = sb.ToString();
		sb.Length = 0;
		l.GetArguments(sb, 256);
		s.Arguments = sb.ToString();
		return s;
	}

	/// <summary>将快捷方式保存到目标位置。</summary>
	/// <param name="position">快捷方式文件的位置。</param>
	public void Save(string position) {
		if (string.IsNullOrEmpty(WorkingDirectory) == false) {
			_link.SetWorkingDirectory(WorkingDirectory);
		}

		if (string.IsNullOrEmpty(Description) == false) {
			_link.SetDescription(Description);
		}

		if (string.IsNullOrEmpty(Arguments) == false) {
			_link.SetArguments(Arguments);
		}

		if (string.IsNullOrEmpty(IconLocation) == false) {
			_link.SetIconLocation(IconLocation, IconIndex >= 0 ? IconIndex : 0);
		}

		IPersistFile file = (IPersistFile)_link;
		file.Save(position, false);
	}

	#region COM Interops

	[ComImport]
	[Guid("00021401-0000-0000-C000-000000000046")]
	private class ShellLink
	{
	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("000214F9-0000-0000-C000-000000000046")]
	private interface IShellLink
	{
		void GetPath([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd,
			int fFlags);

		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);
		void GetDescription([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetWorkingDirectory([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
		void GetArguments([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		void GetHotkey(out short pwHotkey);
		void SetHotkey(short wHotkey);
		void GetShowCmd(out int piShowCmd);
		void SetShowCmd(int iShowCmd);

		void GetIconLocation([Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath,
			out int piIcon);

		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
		void Resolve(IntPtr hwnd, int fFlags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}

	#endregion
}