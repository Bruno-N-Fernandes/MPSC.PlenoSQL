using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace MPSC.PlenoSQL.Kernel.GestorDeAplicacao.PoC
{
	public class GuiRedirect : NativeMethods
	{
		private const UInt32 DUPLICATE_SAME_ACCESS = 2;

		private static Boolean IsRedirected(SafeFileHandle handle)
		{
			FileType fileType = NativeMethods.GetFileType(handle);
			return (fileType == FileType.Disk) || (fileType == FileType.Pipe);
		}

		public static void Redirect()
		{
			if (IsRedirected(NativeMethods.GetStdHandle(StandardHandle.Output)))
			{
				var initialiseOut = Console.Out;
			}

			bool errorRedirected = IsRedirected(NativeMethods.GetStdHandle(StandardHandle.Error));
			if (errorRedirected)
			{
				var initialiseError = Console.Error;
			}

			AttachConsole(ATTACH_PARENT_PROCESS);

			if (!errorRedirected)
				SetStdHandle(StandardHandle.Error, GetStdHandle(StandardHandle.Output));
		}

		public static void InitConsoleHandles()
		{
			BY_HANDLE_FILE_INFORMATION bhfi;
			SafeFileHandle hStdOutDup, hStdErrDup;
			SafeFileHandle hStdOut = GetStdHandle(StandardHandle.Output);
			SafeFileHandle hStdErr = GetStdHandle(StandardHandle.Error);
			IntPtr hProcess = Process.GetCurrentProcess().Handle;

			DuplicateHandle(hProcess, hStdOut, hProcess, out hStdOutDup, 0, true, DUPLICATE_SAME_ACCESS);
			DuplicateHandle(hProcess, hStdErr, hProcess, out hStdErrDup, 0, true, DUPLICATE_SAME_ACCESS);
			AttachConsole(ATTACH_PARENT_PROCESS);


			if (GetFileInformationByHandle(GetStdHandle(StandardHandle.Output), out bhfi))// Adjust the standard handles
				SetStdHandle(StandardHandle.Output, hStdOutDup);
			else
				SetStdHandle(StandardHandle.Output, hStdOut);

			if (GetFileInformationByHandle(GetStdHandle(StandardHandle.Error), out bhfi))
				SetStdHandle(StandardHandle.Error, hStdErrDup);
			else
				SetStdHandle(StandardHandle.Error, hStdErr);
		}

		public static void ShowConsoleWindow()
		{
			var handle = GetConsoleWindow();
			if (handle == IntPtr.Zero)
				AllocConsole();
			else
				ShowWindow(handle, ShowWindowCommand.Show);
		}

		public static void HideConsoleWindow()
		{
			var handle = GetConsoleWindow();
			ShowWindow(handle, ShowWindowCommand.Hide);
		}
	}


	// This always writes to the parent console window and also to a redirected stdout if there is one.
	// It would be better to do the relevant thing (eg write to the redirected file if there is one, otherwise
	// write to the console) but it doesn't seem possible.
	public class GUIConsoleWriter : NativeMethods//, IConsoleWriter
	{
		private readonly StreamWriter _stdOutWriter;

		public GUIConsoleWriter()// this must be called early in the program
		{
			// this needs to happen before attachconsole.
			// If the output is not redirected we still get a valid stream but it doesn't appear to write anywhere
			// I guess it probably does write somewhere, but nowhere I can find out about
			var stdout = Console.OpenStandardOutput();
			_stdOutWriter = new StreamWriter(stdout);
			_stdOutWriter.AutoFlush = true;
			AttachConsole(ATTACH_PARENT_PROCESS);
		}

		public void WriteLine(String line)
		{
			_stdOutWriter.WriteLine(line);
			Console.WriteLine(line);
		}
	}


	public class Program : NativeMethods
	{
		/*
		DEMO CODE ONLY: In general, this approach calls for re-thinking 
		your architecture!
		There are 4 possible ways this can run:
		1) User starts application from existing cmd window, and runs in GUI mode
		2) User double clicks to start application, and runs in GUI mode
		3) User starts applicaiton from existing cmd window, and runs in command mode
		4) User double clicks to start application, and runs in command mode.

		To run in console mode, start a cmd shell and enter:
			c:\path\to\Debug\dir\WindowsApplication.exe console
			To run in gui mode,  EITHER just double click the exe, OR start it from the cmd prompt with:
			c:\path\to\Debug\dir\WindowsApplication.exe (or pass the "gui" argument).
			To start in command mode from a double click, change the default below to "console".
		In practice, I'm not even sure how the console vs gui mode distinction would be made from a double click...
			string mode = args.Length > 0 ? args[0] : "console"; //default to console
		*/



		public static void Principal(string[] args)
		{
			//TODO: better handling of command args, (handle help (--help /?) etc.)
			string mode = args.Length > 0 ? args[0] : "gui"; //default to gui

			if (mode == "gui")
			{
				MessageBox.Show("Welcome to GUI mode");
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form());
			}
			else if (mode == "console")
			{
				//Get a pointer to the forground window.  The idea here is that
				//IF the user is starting our application from an existing console
				//shell, that shell will be the uppermost window.  We'll get it
				//and attach to it
				IntPtr ptr = GetForegroundWindow();
				int u;
				GetWindowThreadProcessId(ptr, out u);
				Process process = Process.GetProcessById(u);

				if (process.ProcessName == "cmd")    //Is the uppermost window a cmd process?
				{
					AttachConsole((UInt32)process.Id); //we have a console to attach to ..
					Console.WriteLine("hello. It looks like you started me from an existing console.");
				}
				else //no console AND we're in console mode ... create a new console.
				{
					AllocConsole();
					Console.WriteLine(@"hello. It looks like you double clicked me to startAND you want console mode.  Here's a new console.");
					Console.WriteLine("press any key to continue ...");
					Console.ReadLine();
				}
				FreeConsole();
			}
		}
	}
}