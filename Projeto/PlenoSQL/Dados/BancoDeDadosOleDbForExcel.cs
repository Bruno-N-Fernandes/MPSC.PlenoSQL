using System;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForExcel : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "OleDb For MS Excel (97 - 2003)"; } }
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=YES;"""; } }
	}
}