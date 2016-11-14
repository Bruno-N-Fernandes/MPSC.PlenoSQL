using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.TestesUnitarios.Conexao.AF;
using MPSC.PlenoSQL.TestesUnitarios.Conexao.AF.Fake;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	[TestClass]
	public class TestandoAutoFactory
	{
		public const Int32 cTotal = 1000000;
		private struct Metrica
		{
			public String Descricao;
			public readonly DateTime DataHora;
			public Metrica(String descricao)
			{
				Descricao = descricao;
				DataHora = DateTime.Now;
			}
		}

		[TestMethod]
		public void TestMethod1()
		{
			var tempos = new List<Metrica>();
			tempos.Add(new Metrica("Inicio"));
			var autoFactory = new AutoFactory(new ConexaoFake());
			tempos.Add(new Metrica("CriarConexaoFake"));


			var torpedos1 = autoFactory.Query<Torpedo>("Select * From Torpedo");
			tempos.Add(new Metrica("Selecionar Com Factory Automatica"));
			Assert.IsNotNull(torpedos1);
			Assert.AreEqual(cTotal, torpedos1.Count());

			tempos.Add(new Metrica("Assertions"));
			AutoFactory.Registrar<Torpedo, TorpedoFactory>();
			tempos.Add(new Metrica("Registar Factory"));

			var torpedos2 = autoFactory.Query<Torpedo>("Select * From Torpedo");
			tempos.Add(new Metrica("Selecionar Com Factory Manual"));
			Assert.IsNotNull(torpedos2);
			Assert.AreEqual(cTotal, torpedos2.Count());
			tempos.Add(new Metrica("Assertions"));

			var metricaAnterior = tempos[0];
			foreach (var metrica in tempos.Skip(1))
			{
				Console.WriteLine("{0} = {1}", metrica.Descricao, metrica.DataHora - metricaAnterior.DataHora);
				metricaAnterior = metrica;
			}
		}
	}


	public class ConexaoFake : IConexao
	{
		private static Dictionary<String, Object>[] _lista;

		public ConexaoFake()
		{
			var lista = new List<Dictionary<String, Object>>();
			for (Int64 id = 1; id <= TestandoAutoFactory.cTotal; id++)
			{
				lista.Add(
					new Dictionary<String, Object>
					{
						{"Id", id},
						{"Enviado", DateTime.Now},
						{"Mensagem", "Mensagem"}
					}
				);
			}
			_lista = lista.ToArray();
		}

		public IDataReader Executar(String cmdSql)
		{
			return ObterTorpedo();
		}


		private IDataReader ObterTorpedo()
		{
			return new DataReaderFake(_lista);
		}
	}
}