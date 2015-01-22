using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace MP.PlenoSQL.AppWin.GestorDeAplicacao
{
	public class NativeMethodsEx : NativeMethods
	{
		/// <summary>
		/// Returns a System.Diagnostics.Process pointing to a pre-existing process with the
		/// same name as the current one, if any; or null if the current process is unique.
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
			var process = PriorProcess();
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