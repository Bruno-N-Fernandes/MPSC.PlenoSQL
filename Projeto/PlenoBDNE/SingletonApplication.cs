using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace MP.PlenoBDNE.AppWin
{
	public static class SingletonApplication
	{
		public static Int32 Run<TForm>(String[] args, SingletonApplication<TForm>.OnConfigurarParametro configurarParametro) where TForm : Form, new()
		{
			return (new SingletonApplication<TForm>(args, true)).Run(configurarParametro);
		}
	}

	public class SingletonApplication<TForm> : WindowsFormsApplicationBase where TForm : Form, new()
	{
		public delegate void OnConfigurarParametro(TForm form, Boolean isNovo, IEnumerable<String> parametros);
		private OnConfigurarParametro configurarParametro;
		private readonly String[] parametros;
		private Int32 exitCode = -1;

		public SingletonApplication(String[] args, Boolean enableVisualStyles)
		{
			this.parametros = args;
			this.IsSingleInstance = true;
			this.EnableVisualStyles = enableVisualStyles;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
			this.StartupNextInstance += new StartupNextInstanceEventHandler(this.SIApp_StartupNextInstance);
		}

		public Int32 Run(OnConfigurarParametro configurarParametro)
		{
			this.configurarParametro = configurarParametro;
			base.Run(parametros);
			GC.Collect();
			return this.exitCode;
		}

		protected override void OnCreateMainForm()
		{
			MainForm = new TForm();
			configurarParametro.Invoke((TForm)MainForm, true, CommandLineArgs);
			exitCode = 0;
		}

		protected void SIApp_StartupNextInstance(Object sender, StartupNextInstanceEventArgs eventArgs)
		{
			configurarParametro.Invoke((TForm)MainForm, false, eventArgs.CommandLine);
		}
	}

	public static class NativeMethods
	{
		private static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		private static readonly Int32 WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		private enum ShowWindowCommand : int
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