using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MPSC.PlenoSQL.AppWin.Interface;

namespace MPSC.PlenoSQL.AppWin.Dados.Base
{
	public abstract class BancoDados
	{
		public static Boolean _isOpen = true;
		private static readonly IDictionary<String, Thread> _threads = new Dictionary<String, Thread>();
		public static readonly IDictionary<String, IList<String>> cacheOld = new Dictionary<String, IList<String>>();
		protected static readonly IDictionary<String, Cache> cache = new Dictionary<String, Cache>();

		public void PreencherCache()
		{
			var iBancoDeDados = (this as IBancoDeDados).Clone();

			if (!_threads.ContainsKey(iBancoDeDados.Conexao))
			{
				var thread = new Thread(() =>
					{
						var tables = iBancoDeDados.ListarTabelas(null, false);
						//iBancoDeDados.Dispose();
						GC.Collect();
					}
				);
				_threads.Add(iBancoDeDados.Conexao, thread);
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
			Application.DoEvents();
		}


		public static void LimparCache()
		{
			_isOpen = false;
			while (_threads.Count > 0)
			{
				var t = _threads.FirstOrDefault();
				_threads.Remove(t.Key);
				try
				{
					t.Value.Interrupt();
					t.Value.Abort();
				}
				catch (Exception) { }
			}
		}
	}
}