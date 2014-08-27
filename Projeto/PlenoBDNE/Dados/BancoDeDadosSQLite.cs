using System;
using System.Data;
using System.Data.SQLite;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }

		protected override String SQLAllDatabases(String nome)
		{
			throw new NotImplementedException("AllDatabasesSQL");
		}

		protected override String SQLAllTables(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Nome{0} From sqlite_master T Where (T.Type = 'table'){1}", detalhes, filtro);
		}

		protected override String SQLAllViews(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Nome{0} From sqlite_master T Where (T.Type = 'view'){1}", detalhes, filtro);
		}

		protected override String SQLAllColumns(String parent, Boolean comDetalhes)
		{
			return String.Format(@"PRAGMA table_info({0})", parent);
		}

		protected override String SQLAllProcedures(String nome)
		{
			throw new NotImplementedException("SQLAllProcedures");
		}

		protected override String Formatar(IDataReader dataReader, Boolean comDetalhes)
		{
			return Convert.ToString(dataReader["Name"]) + (
				comDetalhes ? ((Convert.ToInt16(dataReader["pk"]) == 1) ? "(PK, " : "(") + Convert.ToString(dataReader["type"]) + ((Convert.ToInt16(dataReader["notnull"]) == 1) ? ", NOT NULL)" : ", NULL)") : String.Empty);
		}
	}
}