using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.AppWin.Infra;
using System;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class TrechoTest
	{
		private const String sql = @"
Select F.*
From Fatura F
Inner Join Contrato C On C.Id = F.ContratoId
Inner Join Cliente Cli On Cli.Id = F.ClienteId

Where (F.Id = 1) And (C.Id = 2) And (Cli.Nome = '@nome')
Order By F.Id Asc";


		[TestMethod]
		public void DeveSerCapazDeRetornarALinhaAnteriorNormal()
		{
			var posicao = sql.IndexOf("On C.Id =") + 6;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("From Fatura F", trecho.LinhaAnterior);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarALinhaAtualNormal()
		{
			var posicao = sql.IndexOf("On C.Id =") + 6;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("Inner Join Contrato C On C.Id = F.ContratoId", trecho.LinhaAtual);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarALinhaPosteriorNormal()
		{
			var posicao = sql.IndexOf("On C.Id =") + 6;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("Inner Join Cliente Cli On Cli.Id = F.ClienteId", trecho.LinhaPosterior);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarOCaracterAtual()
		{
			var posicao = sql.IndexOf("On C.Id =") + 6;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("I", trecho.CaracterAtual);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarOTokenCompleto()
		{
			var posicao = sql.IndexOf("On C.Id =") + 5;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("C.Id", trecho.Token.Completo);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("C.Id", trecho.Token.Completo);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("C.Id", trecho.Token.Completo);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarOTokenParcial()
		{
			var posicao = sql.IndexOf("On C.Id =") + 5;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("C.I", trecho.Token.Parcial);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("C.", trecho.Token.Parcial);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("C.Id", trecho.Token.Parcial);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarOTokenPrimeiraParte()
		{
			var posicao = sql.IndexOf("On C.Id =") + 5;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("C", trecho.Token.Primeiro);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("C", trecho.Token.Primeiro);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("C", trecho.Token.Primeiro);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarONomeBaseadoNaPrimeiraParteContrato()
		{
			var posicao = sql.IndexOf("On C.Id =") + 5;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("Contrato", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("Contrato", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("Contrato", trecho.Token.Tabela);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarONomeBaseadoNaPrimeiraParteCliente()
		{
			var posicao = sql.IndexOf("On Cli.Id") + 5;
			var trecho = Trecho.Get(sql, posicao);

			Assert.AreEqual("Cliente", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("Cliente", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("Cliente", trecho.Token.Tabela);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarONomeBaseadoNaPrimeiraParteFatura()
		{
			var posicao = sql.IndexOf("On C.Id") + 12;

			var trecho = Trecho.Get(sql, posicao);
			Assert.AreEqual("Fatura", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("Fatura", trecho.Token.Tabela);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("Fatura", trecho.Token.Tabela);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarONomeDaTabelaBaseadoNoApelido()
		{
			var sql = "select * From Fatura f where f.";
			var posicao = sql.Length;

			var trecho = Trecho.Get(sql, posicao);
			Assert.AreEqual("Fatura", trecho.Token.Tabela);
		}

		[TestMethod]
		public void DeveSerCapazDeRetornarOTokenParcial2()
		{
			var sql = "select * From Fatura f where f.";
			var posicao = sql.Length;

			var trecho = Trecho.Get(sql, posicao);
			Assert.AreEqual("f.", trecho.Token.Parcial);

			trecho = Trecho.Get(sql, posicao - 1);
			Assert.AreEqual("f", trecho.Token.Parcial);

			trecho = Trecho.Get(sql, posicao + 1);
			Assert.AreEqual("", trecho.Token.Parcial);
		}


	}
}