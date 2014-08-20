using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public abstract class BancoDeDadosOleDb : BancoDeDados<OleDbConnection>
	{
		public override IEnumerable<String> ListarTabelas(String tabela)
		{
			var userTables = AbrirConexao().GetSchema("Tables");
			for (int i = 0; i < userTables.Rows.Count; i++)
			{
				var tb = userTables.Rows[i][2].ToString();
				if (tb.Contains("$") && tb.StartsWith(tabela))
					yield return "[" + tb + "]";
			}
		}

		public override IEnumerable<String> ListarViews(String tabela)
		{
			yield break;
		}
	}
}