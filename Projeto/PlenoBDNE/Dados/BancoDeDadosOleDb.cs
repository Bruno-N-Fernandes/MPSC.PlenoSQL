using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public abstract class BancoDeDadosOleDb : BancoDeDados<OleDbConnection>
	{
		protected override String AllTablesSQL(Boolean comDetalhes) { return String.Empty; }
		protected override String AllViewsSQL(Boolean comDetalhes) { return String.Empty; }
		protected override String AllColumnsSQL(Boolean comDetalhes) { return String.Empty; }
		protected override String AllProceduresSQL(String procedureName) { throw new NotImplementedException("AllProceduresSQL"); }
		protected override String AllDatabasesSQL(Boolean comDetalhes) { throw new NotImplementedException("AllDatabasesSQL"); }

		public override IEnumerable<String> ListarColunas(String tabela, Boolean listarDetalhes)
		{
			var schema = GetSchema("Columns");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var tb = Convert.ToString(schema.Rows[i][2]);
				if (tb.Contains("$") && tb.ToUpper().StartsWith(tabela.ToUpper()))
					yield return Convert.ToString(schema.Rows[i][3]);
			}
		}

		public override IEnumerable<String> ListarTabelas(String tabela)
		{
			var schema = GetSchema("Tables");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var tb = Convert.ToString(schema.Rows[i][2]);
				if (tb.Contains("$") && tb.ToUpper().StartsWith(tabela.ToUpper()))
					yield return "[" + tb + "]";
			}
		}

		public override IEnumerable<String> ListarViews(String view)
		{
			var schema = GetSchema("Views");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var vw = Convert.ToString(schema.Rows[i][2]);
				if (vw.Contains("$") && vw.ToUpper().StartsWith(view.ToUpper()))
					yield return "[" + vw + "]";
			}
		}
	}
}