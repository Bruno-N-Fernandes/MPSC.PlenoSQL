using IBM.Data.DB2.iSeries;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("IBM DB2")]
	public class BancoDeDadosIBMDB2 : BancoDeDados<iDB2Connection>
	{
		protected override String StringConexaoTemplate { get { return "DataSource={0};UserID={2};Password={3};DataCompression=True;SortSequence=SharedWeight;SortLanguageId=PTG;DefaultCollection={1};"; } }

		public override IBancoDeDados Clone()
		{
			var iBancoDeDados = new BancoDeDadosIBMDB2();
			iBancoDeDados.ConfigurarConexao(_server, _dataBase, _usuario, _senha);
			return iBancoDeDados;
		}
	}
}