using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MP.PlenoBDNE.AppWin
{
	// this class just wraps some Win32 stuff that we're going to use
	public class NativeMethods
	{
		public static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		public static readonly Int32 WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		[DllImport("user32")]
		public static extern Boolean PostMessage(IntPtr hwnd, Int32 msg, IntPtr wparam, IntPtr lparam);

		[DllImport("user32")]
		public static extern Int32 RegisterWindowMessage(String message);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// Returns a System.Diagnostics.Process pointing to
		/// a pre-existing process with the same name as the
		/// current one, if any; or null if the current process
		/// is unique.
		/// </summary>
		/// <returns></returns>
		public static Process PriorProcess()
		{
			Process curr = Process.GetCurrentProcess();
			Process[] procs = Process.GetProcesses();
			foreach (Process p in procs)
			{
				try
				{
					if ((p.Id != curr.Id) && (p.MainModule.FileName.ToLower().Replace(".vshost", String.Empty) == curr.MainModule.FileName.ToLower().Replace(".vshost", String.Empty)))
						return p;
				}
				catch (Exception) { }
			}
			return null;
		}
	}
}