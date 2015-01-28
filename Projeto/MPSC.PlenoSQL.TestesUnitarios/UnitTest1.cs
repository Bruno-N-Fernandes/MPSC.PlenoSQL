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
			var conexao = conexoes.Adicionar(new Conexao("IBM"));
			conexao.Adicionar(new Tabela("ItemProduto"));
			conexao.Adicionar(new Tabela("ItemProdutoServico"));
			conexao.Adicionar(new Visao("ServicoGrupal"));
			conexao.Adicionar(new Visao("ItemView"));
			conexao.Adicionar(new Procedure("PC_ItemGrupal"));
			conexao.Adicionar(new Procedure("ProcedureMegaZord"));
			var conexoesFiltradas1 = conexoes.Filtrar("Item");
			var conexoesFiltradas2 = conexoes.Filtrar("ItemProduto");
			var conexoesFiltradas3 = conexoes.Filtrar("ProcedureMegaZord");
			Assert.IsNotNull(conexoesFiltradas1);
			Assert.IsNotNull(conexoesFiltradas2);
			Assert.IsNotNull(conexoesFiltradas3);
		}
	}
}