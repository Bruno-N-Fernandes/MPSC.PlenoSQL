using System;
using System.Data.SQLite;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }
		protected override String AllTablesSQL { get { return @"Select T.Name as Tabela, 'SQLite' as Banco, Name as NomeInterno From sqlite_master T Where (T.Type In ('table', 'view')) And (T.Name Like '{0}%')"; } }
		protected override String AllViewsSQL { get { return "Select T.Name as NomeView, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type In ('V')) And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return "Select C.Name as Coluna, '()' As Detalhes From SysColumns C Where (C.Name = '{0}')"; } }
	}
}