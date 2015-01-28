using System;
using System.Data;
using System.Data.SQLite;
using MPSC.PlenoSQL.AppWin.Dados.Base;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			return String.Empty;
		}

		protected override String SQLAllTables(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Name{0} From sqlite_master T Where (T.Type = 'table'){1}", detalhes, filtro);
		}

		protected override String SQLAllViews(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Name{0} From sqlite_master T Where (T.Type = 'view'){1}", detalhes, filtro);
		}

		protected override String SQLAllColumns(String parent, Boolean comDetalhes)
		{
			return String.Format(@"PRAGMA table_info({0})", parent);
		}

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			return String.Empty;
		}

		protected override String Formatar(IDataReader dataReader, Boolean comDetalhes)
		{
			String result = Convert.ToString(dataReader["Name"]);
			if (comDetalhes)
			{
				if (dataReader.GetOrdinal("Detalhes") > 0)
					result += Convert.ToString(dataReader["Detalhes"]);
				else
				{
					result += (Convert.ToInt16(dataReader["pk"]) == 1) ? "(PK, " : "(";
					result += Convert.ToString(dataReader["type"]);
					result += (Convert.ToInt16(dataReader["notnull"]) == 1) ? ", NOT NULL)" : ", NULL)";
				}
			}
			return result;
		}
	}
}