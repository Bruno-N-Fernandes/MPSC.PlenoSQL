using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.AppWin.Infra;
using MPSC.PlenoSQL.AppWin.View.DataSource;
using System;
using System.Linq;
using System.Net;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class UnitTest1
	{
		//[TestMethod]
		public void webClient()
		{
			const int Len = 4096;
			var buffer = new Byte[Len];
			var html = String.Empty;
			var webClient = new WebClient();

			var stream = webClient.OpenRead("http://www.google.com.br");
			var read = stream.Read(buffer, 0, Len);
			while (read > 0)
			{
				html += System.Text.Encoding.Default.GetString(buffer, 0, read);
				read = stream.Read(buffer, 0, Len);
			}
			stream.Close();
			stream.Dispose();
			Assert.IsNotNull(html);
		}

		[TestMethod]
		public void DeveSerCapazDeFiltrarATreeView()
		{
			var ramos = new Ramo(Ramo.cConexoes);
			var ramo = ramos.Adicionar(new Ramo("IBM"));
			ramo.Adicionar(new Ramo("ItemProduto"));
			ramo.Adicionar(new Ramo("ItemProdutoServico"));
			ramo.Adicionar(new Ramo("ServicoGrupal"));
			ramo.Adicionar(new Ramo("ItemView"));
			ramo.Adicionar(new Ramo("PC_ItemGrupal"));
			ramo.Adicionar(new Ramo("ProcedureMegaZord"));
			var ramosFiltradas1 = ramos.Filtrar("Item");
			var ramosFiltradas2 = ramos.Filtrar("ItemProduto");
			var ramosFiltradas3 = ramos.Filtrar("ProcedureMegaZord");
			Assert.IsNotNull(ramosFiltradas1);
			Assert.IsNotNull(ramosFiltradas2);
			Assert.IsNotNull(ramosFiltradas3);
		}

		[TestMethod]
		public void TestMethod2()
		{
			var constantes = Constantes.Instancia;
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