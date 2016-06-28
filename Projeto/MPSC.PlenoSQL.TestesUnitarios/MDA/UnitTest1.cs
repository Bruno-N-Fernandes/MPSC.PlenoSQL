using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	[TestClass]
	public class UnitTest1
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