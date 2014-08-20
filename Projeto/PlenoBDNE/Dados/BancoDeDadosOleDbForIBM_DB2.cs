using System;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForIBM_DB2 : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "Ole DB For IBM DB2"; } }
		protected override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables"; } }
		protected override String AllViewsSQL { get { return "Select T.Name as NomeView, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type In ('V')) And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return "Select C.Name as Coluna From SysColumns C Where (C.Name = '{0}')"; } }
		protected override String StringConexaoTemplate { get { return "Provider=IBMDA400;Data Source={0};Default Collection={1};User ID={2};Password={3}"; } }
	}
}