using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	[DisplayName("OleDb For MS Excel (97 - 2003)")]
	public class BancoDeDadosOleDbForExcel : BancoDeDadosOleDb
	{
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;"""; } }
	}
}