using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class HqlToSqlTest
	{
		private readonly HqlToSql HqlToSql = new HqlToSql();
		[TestMethod]
		public void DeveSerCapazDeConverterUmHQLDeUmaEntidade()
		{
			var hql = "Select F From Fatura F";
			var sql = "Select F.* From Fatura F";
			Assert.AreEqual(sql, HqlToSql.Converter(hql));
		}
	}
}
