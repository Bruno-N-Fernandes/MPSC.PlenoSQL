using System;
using System.Windows.Forms;

namespace LBJC.NavegadorDeDados
{
	public static class Principal
	{
		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Navegador());
			GC.Collect();
		}
	}
}