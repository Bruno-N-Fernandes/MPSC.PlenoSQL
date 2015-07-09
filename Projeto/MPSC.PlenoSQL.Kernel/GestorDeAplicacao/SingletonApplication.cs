using System;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.Kernel.GestorDeAplicacao
{
	public static class SingletonApplication
	{
		public static Int32 Run<TForm>(String[] args, SingletonApplicationOf<TForm>.OnConfigurarParametro configurarParametro) where TForm : Form, new()
		{
			return (new SingletonApplicationOf<TForm>(args, true)).Run(configurarParametro);
		}
	}
}