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
			var autoFactory = new AutoFactory(new ConexaoFake());
			var torpedos = autoFactory.Query<Torpedo>("Select * From Torpedo");

			Assert.IsNotNull(torpedos);
			Assert.AreEqual(3, torpedos.Count());
		}
	}

	public class DomainFiller
	{
		static DomainFiller()
		{
			new FillTorpedo();
 		}
	}

	public class ConexaoFake : DomainFiller, IConexao
	{
		public IDataReader Executar(String cmdSql)
		{
			return new DataReaderFake(
				new Dictionary<String, Object>
				{
					{"Id", DateTime.Now.Ticks},
					{"Enviado", DateTime.Now}
				}
				,
				new Dictionary<String, Object>
				{
					{"Id", DateTime.Now.Ticks},
					{"Enviado", DateTime.Now}
				}
				,
				new Dictionary<String, Object>
				{
					{"Id", DateTime.Now.Ticks},
					{"Enviado", DateTime.Now}
				}
			);
		}
	}
}