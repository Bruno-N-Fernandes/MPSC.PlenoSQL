using System;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.View;

namespace MP.PlenoBDNE.AppWin
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