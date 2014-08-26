using System;
using System.Collections.Generic;
using System.Data.SQLite;
using MP.PlenoBDNE.AppWin.Dados.Base;
using System.Data;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }
		protected override String AllTablesSQL { get { return @"Select T.Name As Nome, '' As Detalhes From sqlite_master T Where (T.Type = 'table') And (T.Name Like '{0}%')"; } }
		protected override String AllViewsSQL { get { return @"Select T.Name As Nome, '' As Detalhes From sqlite_master T Where (T.Type = 'view') And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return @"PRAGMA table_info({0})"; } }


		protected override String Formatar(IDataReader dataReader, Boolean listarDetalhes)
		{
			return Convert.ToString(dataReader["Name"]) + (
				listarDetalhes
				?	((Convert.ToInt16(dataReader["pk"]) == 1) ? "(PK, " : "(") + Convert.ToString(dataReader["type"]) +
					((Convert.ToInt16(dataReader["notnull"]) == 1) ? ", NOT NULL)" : ", NULL)")
				: String.Empty);
		}
	}
}