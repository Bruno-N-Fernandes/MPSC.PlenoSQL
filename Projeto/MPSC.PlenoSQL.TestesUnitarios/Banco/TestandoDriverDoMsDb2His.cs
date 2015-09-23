using Microsoft.HostIntegration.MsDb2Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Banco
{
	[TestClass]
	public class TestandoDriverDoMsDb2His
	{
		private const string con =
			@"Network Address={0};Initial Catalog={0};User ID={2};Password={3};Data Source={1};Database Name={1};Default Schema={1};Package Collection={1};Default Qualifier={1};Cache Authentication=False;Persist Security Info=True;Authentication=Server;Defer Prepare=False;DateTime As Char=False;Use Early Metadata=False;Derive Parameters=False;Network Transport Library=TCPIP;Host CCSID=37;PC Code Page=1252;Network Port=446;DBMS Platform=DB2/AS400;Process Binary as Character=False;DateTime As Date=False;AutoCommit=False;Connection Pooling=True;Units of Work=RUW;";

		private const string sql_Sem_Parenteses_SoFuncionaNoHIS_2010 = @"
Select 
	Count ( Distinct E.EnvioMaquinaImpressaoId )
From EnvioMaquinaImpressao E
Where E.SolicitacaoId Is Not Null
And E.DocumentoMaquinaImpressaoId = @documentoMaquinaImpressaoId
And E.TipoSaidaDocumentoId = @tipoSaidaDocumentoId
And E.DataEnvio = @dataEnvio ";

		private const string sql_Com_Parenteses = @"
Select 
	Count ( Distinct E.EnvioMaquinaImpressaoId )
From EnvioMaquinaImpressao E
Where (E.SolicitacaoId Is Not Null)
And (E.DocumentoMaquinaImpressaoId = @documentoMaquinaImpressaoId)
And (E.TipoSaidaDocumentoId = @tipoSaidaDocumentoId)
And (E.DataEnvio = @dataEnvio) ";


		[TestMethod]
		public void Quando_Usa_Sql_Sem_Parenteses_Nas_Clausulas_Do_Where_Esta_Causando_Erro_No_HIS2013()
		{
            Executar(sql_Sem_Parenteses_SoFuncionaNoHIS_2010, p => p);
		}

		[TestMethod]
		public void Quando_Usa_Sql_Com_Parenteses_Nas_Clausulas_Do_Where()
		{
			Executar(sql_Com_Parenteses, p => p);
		}

		[TestMethod]
		public void Quando_Usa_Sql_ToUpper_Sem_Parenteses_Nas_Clausulas_Do_Where_Esta_Causando_Erro_No_HIS2013()
		{
			Executar(sql_Sem_Parenteses_SoFuncionaNoHIS_2010.ToUpper(), p => p.ToUpper());
		}

		[TestMethod]
		public void Quando_Usa_Sql_ToUpper_Com_Parenteses_Nas_Clausulas_Do_Where()
		{
			Executar(sql_Com_Parenteses.ToUpper(), p => p.ToUpper());
		}

		private static void Executar(String comandoSQL, Func<String, String> funcao)
        {
            var msDb2Connection = new MsDb2Connection(String.Format(con, "MtzSrva7", "ESIM", "UsrBen", "@poiuy"));
            msDb2Connection.Open();

            var msDb2Command = msDb2Connection.CreateCommand();
            msDb2Command.CommandTimeout = 60;
            msDb2Command.CommandType = CommandType.Text;
            msDb2Command.CommandText = comandoSQL;
			AdicionarParametros(msDb2Command, funcao);

            try
            {
                var a = msDb2Command.ExecuteScalar();
                Assert.IsNotNull(a);
            }
            catch (Exception exception)
            {
				Console.WriteLine(exception.Message);
                Assert.Fail(exception.Message);
            }
            finally
            {
                msDb2Command.Dispose();
                msDb2Connection.Close();
                msDb2Connection.Dispose();
            }
        }

		private static void AdicionarParametros(MsDb2Command msDb2Command, Func<String, String> funcao)
		{
			var p = msDb2Command.CreateParameter();
			p.ParameterName = funcao("@documentoMaquinaImpressaoId");
			p.MsDb2Type = MsDb2Type.SmallInt;
			p.Direction = ParameterDirection.Input;
			p.Value = (Int16)1;
			msDb2Command.Parameters.Add(p);

			p = msDb2Command.CreateParameter();
			p.ParameterName = funcao("@tipoSaidaDocumentoId");
			p.MsDb2Type = MsDb2Type.SmallInt;
			p.Direction = ParameterDirection.Input;
			p.Value = (Int16)3;
			msDb2Command.Parameters.Add(p);

			p = msDb2Command.CreateParameter();
			p.ParameterName = funcao("@dataEnvio");
			p.MsDb2Type = MsDb2Type.Date;
			p.Direction = ParameterDirection.Input;
			p.Value = DateTime.Today;
			msDb2Command.Parameters.Add(p);
		}
	}
}