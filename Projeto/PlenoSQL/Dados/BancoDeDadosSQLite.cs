﻿using System;
using System.Data;
using System.Data.SQLite;
using MPSC.PlenoSQL.AppWin.Dados.Base;
using System.Collections.Generic;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }
		protected override String SQLAllDatabases(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLTablesColumns { get { return QueryOf.cQueryCacheTablesColumns; } }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { return String.Empty; }

		public override IEnumerable<String> ListarColunas(String parent, Boolean comDetalhes)
		{
			var sql = String.Format(@"PRAGMA table_info ({0})", parent);
			var dataReader = ExecuteReader(sql);
			while (dataReader.IsOpen() && dataReader.Read())
				yield return Formatar(dataReader, comDetalhes);
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

		protected class QueryOf : Query
		{
			public const String cQueryCacheTablesColumns = @"
Select
	Upper(SubStr(Tab.Type, 1, 1)) As TipoTabela,
	Tab.Name As NomeTabela,
	Tab.Name As NomeInternoTabela,
	'' As NomeColuna,
	'' As DetalhesColuna
From sqlite_master Tab";
		}
	}
}