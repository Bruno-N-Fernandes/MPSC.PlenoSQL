using System;
using IBM.Data.DB2.iSeries;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosIBMDB2 : BancoDeDados<iDB2Connection>
	{
		public override String Descricao { get { return "IBM DB2"; } }
		protected override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables Where (Table_Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return "Select C.Column_Name as Coluna From SysColumns C Where (C.Table_Name = '{0}') Order By C.Name"; } }
		protected override String StringConexaoTemplate { get { return "DataSource={0};UserID={2};Password={3};DataCompression=True;SortSequence=SharedWeight;SortLanguageId=PTG;DefaultCollection={1};"; } }
	}
}