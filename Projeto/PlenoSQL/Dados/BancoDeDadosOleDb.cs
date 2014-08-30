using System;
using System.Collections.Generic;
using System.Data.OleDb;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public abstract class BancoDeDadosOleDb : BancoDeDados<OleDbConnection>
	{
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }
		protected override String SQLAllDatabases(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLAllTables(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLAllViews(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLAllColumns(String parent, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { return String.Empty; }

		public override IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0}" : "[{0}]";
			var schema = GetSchema("Tables");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var tb = Convert.ToString(schema.Rows[i][2]);
				if (tb.Contains("$") && (String.IsNullOrWhiteSpace(nome) || tb.ToUpper().StartsWith(nome.ToUpper())))
					yield return String.Format(format, tb);
			}
		}

		public override IEnumerable<String> ListarViews(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0}" : "[{0}]";
			var schema = GetSchema("Views");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var vw = Convert.ToString(schema.Rows[i][2]);
				if (vw.Contains("$") && (String.IsNullOrWhiteSpace(nome) || vw.ToUpper().StartsWith(nome.ToUpper())))
					yield return String.Format(format, vw);
			}
		}

		public override IEnumerable<String> ListarColunas(String parent, Boolean comDetalhes)
		{
			var schema = GetSchema("Columns");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var tb = Convert.ToString(schema.Rows[i][2]);
				if (tb.Contains("$") && tb.ToUpper().StartsWith(parent.ToUpper()))
					yield return Convert.ToString(schema.Rows[i][3]);
			}
		}
	}
}