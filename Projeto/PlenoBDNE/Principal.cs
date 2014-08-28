using System;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.View;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;

namespace MP.PlenoBDNE.AppWin
{
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Colocar informações no StatusBar (conexão, usuário, banco, registros alterados) 
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Listar os objetos do banco de dados na coluna da esquerda (TreeView)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Exportar o resultado da query para TXT, XLS, XML, PDF, etc.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir escolher a fonte e o tamanho da mesma.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir Configurar o Colorir da Query
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Mensagem de aguarde, processsando
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir o cancelamento da query.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Criar um grupo de Favoritos (Cada grupo poderá agrupar vários arquivos)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Close All But This.
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Fazer Auto Reload dos arquivos alterados
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Permitir associar extensão ao aplicativo
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar F4 para mostrar propriedades da tabela e do campo
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar Code Snippet (Pressionando TAB)
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar a separação de blocos de código (ponto e virgula)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar associação de extensão de arquivo (para abrir pelo Windows Explorer)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de arquivos (para arrastar pelo Windows Explorer)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de tabelas para o editor

	public static class Principal
	{
		private static readonly Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
		[STAThread]
		public static void Main(String[] arquivos)
		{
			if (mutex.WaitOne(TimeSpan.Zero, true))
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Navegador().AbrirDocumentos(arquivos));
				mutex.ReleaseMutex();
				GC.Collect();
			}
			else
			{
				var p = NativeMethods.PriorProcess();
				IntPtr handle = p != null ? p.MainWindowHandle : NativeMethods.HWND_BROADCAST;

				NativeMethods.SetForegroundWindow(handle);
				NativeMethods.PostMessage(handle, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}

	// this class just wraps some Win32 stuff that we're going to use
	internal class NativeMethods
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