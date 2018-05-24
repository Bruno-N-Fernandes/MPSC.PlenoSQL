using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.Kernel.Dados.Base;

namespace MPSC.PlenoSQL.TestesUnitarios.Infra
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var cache = new Cache();

			Assert.AreEqual("Inclusao_UsuarioId", Cache.Traduzir("InCLUSAO_UsuarioId"));
			Assert.AreEqual("ItemCertificadoApoliceId", Cache.Traduzir("ITEMCERTIFICADOAPOLICEID"));
			Assert.AreEqual("StatusItemCertificadoApoliceId", Cache.Traduzir("statusitemcertificadoapoliceid"));
			Assert.AreEqual("DataHoraInclusao", Cache.Traduzir("DataHoraInCLUSAO"));
			Assert.AreEqual("CompetenciaReajuste", Cache.Traduzir("CompetenciareAjuste"));

		}
	}
}
