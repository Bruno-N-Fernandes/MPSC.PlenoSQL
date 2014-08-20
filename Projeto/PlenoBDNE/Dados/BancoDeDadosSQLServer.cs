using System;
using System.Data.SqlClient;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLServer : BancoDeDados<SqlConnection>
	{
		public override String Descricao { get { return "Sql Server"; } }
		protected override String AllTablesSQL { get { return "Select T.Name as Tabela, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type = 'U') And (T.Name Like '{0}%')"; } }
		protected override String AllViewsSQL { get { return "Select T.Name as NomeView, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type = 'V') And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return "Select C.Name as Coluna From SysColumns C Where (C.Id = Object_Id('{0}')) Order By C.Name"; } }
		protected override String StringConexaoTemplate { get { return "Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;"; } }
	}
}