using System;
using System.Data.SQLite;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String AllTablesSQL { get { return @"Select T.Name as Tabela, 'SQLite' as Banco, Name as NomeInterno From sqlite_master T Where (T.Type = 'table') And (T.Name Like '{0}%')"; } }
		protected override String StringConexaoTemplate { get { return "Data Source={0};Version=3;"; } }
	}
}