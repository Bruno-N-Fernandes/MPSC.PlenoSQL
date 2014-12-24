using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoSQL.AppWin.Dados.Base
{
	public abstract class BancoDados
	{
		private static Boolean _isOpen = true;
		private static readonly IList<Thread> _threads = new List<Thread>();
		public static readonly IDictionary<String, IList<String>> cache = new Dictionary<String, IList<String>>();

		public void PreencherCache()
		{
			var iBancoDeDados = (this as IBancoDeDados).Clone();

			var thread = new Thread(() =>
			{
				var tables = iBancoDeDados.ListarTabelas(null, true);
				var views = iBancoDeDados.ListarViews(null, true);
				var tablesOrViews = tables.Union(views);

				foreach (var tableOrView in tablesOrViews)
				{
					if (_isOpen)
					{
						iBancoDeDados.ListarColunas(tableOrView, true);
						Application.DoEvents();
					}
				}
				iBancoDeDados.Dispose();
				GC.Collect();
			}
			);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_threads.Add(thread);
			Application.DoEvents();
		}


		public static void LimparCache()
		{
			_isOpen = false;

			while (_threads.Count > 0)
			{
				var t = _threads[0];
				_threads.RemoveAt(0);
				try
				{
					t.Interrupt();
					t.Abort();
				}
				catch (Exception) { }
			}
		}
	}
}