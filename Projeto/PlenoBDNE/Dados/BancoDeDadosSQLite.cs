using System;
using System.Data.SQLite;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLite : BancoDeDados<SQLiteConnection>
	{
		public override String Descricao { get { return "SQLite"; } }
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Version=3;"; } }
		protected override String AllTablesSQL { get { return @"Select T.Name As Nome, '' As Detalhes From sqlite_master T Where (T.Type = 'table') And (T.Name Like '{0}%')"; } }
		protected override String AllViewsSQL { get { return @"Select T.Name As Nome, '' As Detalhes From sqlite_master T Where (T.Type = 'view') And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return @"Select C.Name As Nome, '()' As Detalhes From SysColumns C Where (C.Name = '{0}')"; } }
	}
}