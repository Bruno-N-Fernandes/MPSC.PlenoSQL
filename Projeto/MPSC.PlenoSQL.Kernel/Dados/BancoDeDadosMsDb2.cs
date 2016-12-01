using Microsoft.HostIntegration.MsDb2Client;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("MsDb2-HIS")]
	public class BancoDeDadosMsDb2 : BancoDeDadosDb2<MsDb2Connection>
	{
		protected override String StringConexaoTemplate { get { return "Network Address={0};Initial Catalog={0};User ID={2};Password={3};Data Source={1};Database Name={1};Default Schema={1};Package Collection={1};Default Qualifier={1};Cache Authentication=False;Persist Security Info=True;Authentication=Server;Defer Prepare=False;DateTime As Char=False;Use Early Metadata=False;Derive Parameters=False;Network Transport Library=TCPIP;Host CCSID=37;PC Code Page=1252;Network Port=446;DBMS Platform=DB2/AS400;Process Binary as Character=False;DateTime As Date=False;AutoCommit=False;Connection Pooling=True;Units of Work=RUW;"; } }

		public override IBancoDeDados Clone()
		{
			var iBancoDeDados = new BancoDeDadosMsDb2();
			iBancoDeDados.ConfigurarConexao(_server, _dataBase, _usuario, _senha);
			return iBancoDeDados;
		}
	}
}