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
		private static Boolean _isOpen = true;
		private static readonly IList<Thread> _threads = new List<Thread>();
		public static readonly IDictionary<String, IList<String>> cacheOld = new Dictionary<String, IList<String>>();
		public static readonly IDictionary<String, Cache> cache = new Dictionary<String, Cache>();

		public void PreencherCache()
		{
			if (_isOpen)
			{
				var iBancoDeDados = (this as IBancoDeDados).Clone();
			}
			
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