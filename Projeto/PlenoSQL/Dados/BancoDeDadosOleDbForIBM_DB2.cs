using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	[DisplayName("OleDb For IBM DB2")]
	public class BancoDeDadosOleDbForIBM_DB2 : BancoDeDadosOleDb
	{
		protected override String StringConexaoTemplate { get { return "Provider=IBMDA400;Data Source={0};Default Collection={1};User ID={2};Password={3}"; } }
	}
}