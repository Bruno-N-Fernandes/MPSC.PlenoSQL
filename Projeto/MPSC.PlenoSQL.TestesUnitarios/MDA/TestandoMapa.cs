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


		public const String definition = @"
int32 idade
string nome
datetime data
string email
";
	}
}