using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	[TestClass]
	public class TestandoMapa
	{
		[TestMethod]
		public void TestMethod1()
		{
			var classe = MDD.Load(definition);
			Assert.IsNotNull(classe);
		}

		[TestMethod]
		public void TestMethod2()
		{
			Console.WriteLine("Cadastro");
			var dic = Mapa.De<String, Int32>(20)
				.Add("0", 00)
				.Add("1", 10)
				.Add("2", 20)
				.Add("3", 30)
				.Add("4", 40)
				.Add("5", 50)
				.Add("6", 60)
				.Add("7", 70)
				.Add("8", 80)
				.Add("9", 90)
				.Build();
			Console.WriteLine("Pesquisa");

			Assert.IsNotNull(dic);
			Assert.AreEqual(00, dic["0"]);
			Assert.AreEqual(10, dic["1"]);
			Assert.AreEqual(20, dic["2"]);
			Assert.AreEqual(30, dic["3"]);
			Assert.AreEqual(40, dic["4"]);
			Assert.AreEqual(70, dic["7"]);
			Assert.AreEqual(80, dic["8"]);
			Assert.AreEqual(90, dic["9"]);
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void TestMethod3()
		{
			var mapa =
				Mapa.De<Int32, Int32>(10)
				.Add(0, 00)
				.Add(1, 10)
				.Add(2, 20)
				.Add(3, 30)
				.Add(4, 40)
				.Add(5, 50)
				.Add(6, 60)
				.Add(7, 70)
				.Add(0, 80)
				.Add(9, 90);
			var dic = mapa.Build();
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void TestMethod4()
		{
			var mapa =
				Mapa.De<Int32, Int32>(5)
				.Add(0, 00)
				.Add(1, 10)
				.Add(2, 20)
				.Add(3, 30)
				.Add(4, 40)
				.Add(5, 50)
				.Add(6, 60)
				.Add(7, 70);
			var dic = mapa.Build();
		}


		public const String definition = @"
int32 idade
string nome
datetime data
string email
";
	}
}