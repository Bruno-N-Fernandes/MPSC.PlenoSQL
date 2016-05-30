using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.Kernel.Dados.Base
{
	public abstract class BancoDados
	{
		public static Boolean _isOpen = true;
		public static readonly IDictionary<String, IList<String>> cacheOld = new Dictionary<String, IList<String>>();
		protected static readonly IDictionary<String, Cache> cache = new Dictionary<String, Cache>();

		public void PreencherCache()
		{
			var iBancoDeDados = (this as IBancoDeDados).Clone();
			iBancoDeDados.ListarTabelas(null, false);
			Application.DoEvents();
		}

		public static void LimparCache()
		{
			_isOpen = false;
		}
	}
}