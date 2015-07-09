using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("OleDb For MS Access")]
	public class BancoDeDadosOleDbForAccess : BancoDeDadosOleDb
	{
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;"; } }
	}
}