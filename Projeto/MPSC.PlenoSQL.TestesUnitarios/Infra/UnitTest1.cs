using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.Kernel.Dados.Base;

namespace MPSC.PlenoSQL.TestesUnitarios.Infra
{
	[TestClass]
	public class TestandoCache
	{
		[TestMethod]
		public void QuandoPedeParaTraduzir()
		{
			var cache = new Cache("Teste");

			Assert.AreEqual("Inclusao_UsuarioId", Cache.Traduzir("InCLUSAO_UsuarioId"));
			Assert.AreEqual("Inclusao_UsuarioId", Cache.Traduzir("INCLUSAO_USUARIOID"));
			Assert.AreEqual("Inclusao_UsuarioId", Cache.Traduzir("inclusao_usuarioid"));

			Assert.AreEqual("ItemCertificadoApoliceId", Cache.Traduzir("ITEMCERTIfICADOAPOLICeID"));
			Assert.AreEqual("ItemCertificadoApoliceId", Cache.Traduzir("ITEMCERTIFICADOAPOLICEID"));
			Assert.AreEqual("ItemCertificadoApoliceId", Cache.Traduzir("itemcertificadoapoliceid"));

			Assert.AreEqual("StatusItemCertificadoApoliceId", Cache.Traduzir("statusitEmcertificadoapOliceid"));
			Assert.AreEqual("StatusItemCertificadoApoliceId", Cache.Traduzir("STATUSITEMCERTIFICADOAPOLICEID"));
			Assert.AreEqual("StatusItemCertificadoApoliceId", Cache.Traduzir("statusitemcertificadoapoliceid"));

			Assert.AreEqual("DataHoraInclusao", Cache.Traduzir("DataHoraInCLUSAO"));
			Assert.AreEqual("DataHoraInclusao", Cache.Traduzir("DATAHORAINCLUSAO"));
			Assert.AreEqual("DataHoraInclusao", Cache.Traduzir("datahorainclusao"));

			Assert.AreEqual("CompetenciaReajuste", Cache.Traduzir("CompetenciareAjuste"));
			Assert.AreEqual("CompetenciaReajuste", Cache.Traduzir("COMPETENCIAREAJUSTE"));
			Assert.AreEqual("CompetenciaReajuste", Cache.Traduzir("competenciareajuste"));

		}
	}
}
