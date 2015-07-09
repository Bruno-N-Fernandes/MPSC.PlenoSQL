using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.Kernel.Infra;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class TestandoConstantes
	{
		[TestMethod]
		public void DeveSerCapazDeProcessarAsConstantes()
		{
			var constantes = Constantes.Instanciar(false);
			constantes.Adicionar(Constante.GLOBAL, "A", "0");
			constantes.Adicionar(Constante.GLOBAL, "B", "0");
			constantes.Adicionar(Constante.GLOBAL, "C", "0");
			constantes.Adicionar(Constante.GLOBAL, "G", "0");

			constantes.Adicionar("a", "A", "1");
			constantes.Adicionar("a", "A", "2");
			constantes.Adicionar("a", "B", "1");
			constantes.Adicionar("a", "B", "2");
			constantes.Adicionar("a", "C", "1");
			constantes.Adicionar("a", "C", "2");
			constantes.Adicionar("a", "D", "1");
			constantes.Adicionar("a", "D", "2");

			var csA = constantes.Obter("a").ToList();
			var csB = constantes.Obter("b").ToList();
			Assert.AreEqual(8, constantes.Count);
			Assert.AreEqual(5, csA.Count());
			Assert.AreEqual(4, csB.Count());
		}
	}
}