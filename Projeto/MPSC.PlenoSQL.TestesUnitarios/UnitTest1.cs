﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		[TestMethod]
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
		public void TestMethod1()
		{
			var conexoes = new Ramo(Ramo.cConexoes);
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

		[TestMethod]
		public void TestMethod2()
		{
			var constantes = new Constantes();
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