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
			var modo = _threads.Count % 4;

			var thread = new Thread(() =>
			{
				var tables = iBancoDeDados.ListarTabelas(null, false);
				var views = iBancoDeDados.ListarViews(null, false);
				List<String> tablesOrViews1;
				List<String> tablesOrViews2;
				if (modo == 0)
				{
					tablesOrViews1 = tables.OrderBy(i => i).ToList();
					tablesOrViews2 = views.OrderBy(i => i).ToList();
				}
				else if (modo == 1)
				{
					tablesOrViews1 = tables.OrderByDescending(i => i).ToList();
					tablesOrViews2 = views.OrderByDescending(i => i).ToList();
				}
				else if (modo == 2)
				{
					tablesOrViews1 = views.OrderBy(i => i).ToList();
					tablesOrViews2 = tables.OrderBy(i => i).ToList();
				}
				else
				{
					tablesOrViews1 = views.OrderByDescending(i => i).ToList();
					tablesOrViews2 = tables.OrderByDescending(i => i).ToList();
				}

				foreach (var tableOrView in tablesOrViews1)
				{
					if (BancoDados._isOpen)
						iBancoDeDados.ListarColunas(tableOrView, true);
					Application.DoEvents();
				}

				foreach (var tableOrView in tablesOrViews2)
				{
					if (BancoDados._isOpen)
						iBancoDeDados.ListarColunas(tableOrView, true);
					Application.DoEvents();
				}

				iBancoDeDados.Dispose();
				GC.Collect();
			}
			);
			_threads.Add(thread);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
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