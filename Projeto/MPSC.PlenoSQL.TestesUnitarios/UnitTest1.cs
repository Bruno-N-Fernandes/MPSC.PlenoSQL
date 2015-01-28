using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.AppWin.View.DataSource;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var conexoes = new Conexoes();
			var conexao = conexoes.Adicionar(new Ramo("IBM"));
			conexao.Adicionar(new Ramo("ItemProduto"));
			conexao.Adicionar(new Ramo("ItemProdutoServico"));
			conexao.Adicionar(new Ramo("ServicoGrupal"));
			conexao.Adicionar(new Ramo("ItemView"));
			conexao.Adicionar(new Ramo("PC_ItemGrupal"));
			conexao.Adicionar(new Ramo("ProcedureMegaZord"));
			var conexoesFiltradas1 = conexoes.Filtrar("Item");
			var conexoesFiltradas2 = conexoes.Filtrar("ItemProduto");
			var conexoesFiltradas3 = conexoes.Filtrar("ProcedureMegaZord");
			Assert.IsNotNull(conexoesFiltradas1);
			Assert.IsNotNull(conexoesFiltradas2);
			Assert.IsNotNull(conexoesFiltradas3);
		}
	}
}