using System;
using System.Data.SQLite;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String AllTablesSQL { get { return "Select T.Name as Tabela, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type In ('U', 'V')) And (T.Name Like '{0}%')"; } }
		protected override String StringConexaoTemplate { get { return "Data Source={0};Version=3;"; } }
	}
}