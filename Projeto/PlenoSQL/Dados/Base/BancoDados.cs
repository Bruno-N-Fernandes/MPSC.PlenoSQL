using System;
using System.Collections.Generic;

namespace MP.PlenoSQL.AppWin.Dados.Base
{
	public abstract class BancoDados
	{
		public static IDictionary<String, IList<String>> cache = new Dictionary<String, IList<String>>();
	}
}