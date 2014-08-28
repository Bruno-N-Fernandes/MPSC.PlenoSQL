using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace MP.PlenoBDNE.AppWin
{
	// this class just wraps some Win32 stuff that we're going to use
	public class NativeMethods
	{
		private static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		private static readonly Int32 WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		public enum ShowWindowCommand : int
		{
			Hide = 0x0,
			ShowNormal = 0x1,
			ShowMinimized = 0x2,
			ShowMaximized = 0x3,
			ShowNormalNotActive = 0x4,
			Minimize = 0x6,
			ShowMinimizedNotActive = 0x7,
			ShowCurrentNotActive = 0x8,
			Restore = 0x9,
			ShowDefault = 0xA,
			ForceMinimize = 0xB
		}
		[DllImport("user32")]
		private static extern Boolean PostMessage(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32")]
		private static extern Int32 RegisterWindowMessage(String message);

		[DllImport("user32.dll")]
		private static extern Boolean SetForegroundWindow(IntPtr hWnd);

		[DllImport("User32.dll")]
		private static extern Boolean IsIconic([In] IntPtr windowHandle);

		[DllImport("User32.dll")]
		private static extern Boolean ShowWindow([In] IntPtr windowHandle, [In] ShowWindowCommand command);

		/// <summary>
		/// Returns a System.Diagnostics.Process pointing to
		/// a pre-existing process with the same name as the
		/// current one, if any; or null if the current process
		/// is unique.
		/// </summary>
		/// <returns></returns>
		private static Process PriorProcess()
		{
			Process meProcess = Process.GetCurrentProcess();
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				try
				{
					if ((process.Id != meProcess.Id) && (process.MainModule.FileName.ToLower().Replace(".vshost", String.Empty) == meProcess.MainModule.FileName.ToLower().Replace(".vshost", String.Empty)))
						return process;
				}
				catch (Exception) { }
			}
			return null;
		}

		public static void ShowOpenedApplication()
		{
			var process = NativeMethods.PriorProcess();
			IntPtr handle = (process != null) ? process.MainWindowHandle : NativeMethods.HWND_BROADCAST;
			NativeMethods.PostMessage(handle, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
			if (process != null)
			{
				NativeMethods.ShowWindow(handle, ShowWindowCommand.ShowMaximized);
				NativeMethods.SetForegroundWindow(handle);
			}
		}

		public static bool JaEstaRodando()
		{
			var assembly = Assembly.GetEntryAssembly().GetName().FullName;
			Mutex mutex = new Mutex(true, assembly);
			GC.KeepAlive(mutex);
			return !mutex.WaitOne(TimeSpan.Zero, true);
		}
	}
}

/*
protected override void WndProc(ref Message m)
{
	if (m.Msg == NativeMethods.WM_SHOWME)
		ShowMe();
	base.WndProc(ref m);
}
private void ShowMe()
{
	if (WindowState == FormWindowState.Minimized)
		WindowState = FormWindowState.Maximized;
	bool top = TopMost;
	TopMost = true;
	TopMost = top;
} 
*/