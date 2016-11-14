using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	[TestClass]
	public class TestandoAutoFactory
	{
		[TestMethod]
		public void TestMethod1()
		{
			AutoFactory.Registrar<Torpedo, TorpedoFactory>();

			var autoFactory = new AutoFactory(new ConexaoFake());
			var torpedos = autoFactory.Query<Torpedo>("Select * From Torpedo");

			Assert.IsNotNull(torpedos);
			Assert.AreEqual(3, torpedos.Count());
		}
	}


	public class ConexaoFake : IConexao
	{
		public IDataReader Executar(String cmdSql)
		{
			return ObterTorpedo();
		}


		private IDataReader ObterTorpedo()
		{
			return new DataReaderFake(
				new Dictionary<String, Object>
				{
					{"Id", 1},
					{"Enviado", DateTime.Now}
				}
				,
				new Dictionary<String, Object>
				{
					{"Id", 2},
					{"Enviado", DateTime.Now}
				}
				,
				new Dictionary<String, Object>
				{
					{"Id", 3},
					{"Enviado", DateTime.Now}
				}
			);
		}
	}
}