using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace MPSC.PlenoSQL.Kernel.GestorDeAplicacao
{
	public class NativeMethods
	{
		public static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		public static readonly Int32 WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
		public const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;


		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Boolean AttachConsole(UInt32 dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Boolean DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwOptions);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Boolean FreeConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Boolean GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern FileType GetFileType(SafeFileHandle handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle GetStdHandle(StandardHandle nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Boolean SetStdHandle(StandardHandle nStdHandle, SafeFileHandle handle);


		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

		[DllImport("User32.dll")]
		public static extern Boolean IsIconic([In] IntPtr windowHandle);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Boolean PostMessage(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Int32 RegisterWindowMessage(String message);

		[DllImport("user32.dll")]
		public static extern Boolean SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

		[DllImport("User32.dll")]
		public static extern Boolean ShowWindow([In] IntPtr windowHandle, [In] ShowWindowCommand command);


		public enum StandardHandle : uint
		{
			Input = unchecked((uint)-10),
			Output = unchecked((uint)-11),
			Error = unchecked((uint)-12),
		}

		public enum FileType : uint
		{
			Unknown = 0x0000,
			Disk = 0x0001,
			Char = 0x0002,
			Pipe = 0x0003
		}

		public enum ShowWindowCommand : int
		{
			Hide = 0x0,
			ShowNormal = 0x1,
			ShowMinimized = 0x2,
			ShowMaximized = 0x3,
			ShowNormalNotActive = 0x4,
			Show = 0x5,
			Minimize = 0x6,
			ShowMinimizedNotActive = 0x7,
			ShowCurrentNotActive = 0x8,
			Restore = 0x9,
			ShowDefault = 0xA,
			ForceMinimize = 0xB
		}

		public struct BY_HANDLE_FILE_INFORMATION
		{
			public UInt32 FileAttributes;
			public FileTime CreationTime;
			public FileTime LastAccessTime;
			public FileTime LastWriteTime;
			public UInt32 VolumeSerialNumber;
			public UInt32 FileSizeHigh;
			public UInt32 FileSizeLow;
			public UInt32 NumberOfLinks;
			public UInt32 FileIndexHigh;
			public UInt32 FileIndexLow;
		}
	}
}