using System;
using System.Data.OleDb;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDb : BancoDeDados<OleDbConnection>
	{
		public override String Descricao { get { return "Ole DB"; } }
		protected override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables"; } }
		protected override String AllColumnsSQL { get { return "Select C.Name as Coluna From SysColumns C Where (C.Name = '{0}')"; } }
		protected override String StringConexaoTemplate { get { return "Provider=IBMDA400;Data Source={0};Default Collection={1};User ID={2};Password={3}"; } }
	}
}