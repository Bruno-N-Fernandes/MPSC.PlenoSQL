using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.AppWin.View.DataSource;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class TestandoFiltroTreeView
	{
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
	}
}