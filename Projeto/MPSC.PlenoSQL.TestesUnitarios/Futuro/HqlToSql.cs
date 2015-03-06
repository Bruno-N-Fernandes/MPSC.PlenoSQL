using System;
using System.Collections.Generic;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	public class HqlToSql
	{

		public String Converter(String hql)
		{
			return hql;
		}

		public class Conversor
		{
			private List<String> select = new List<String>();
			private List<String> from = new List<String>();
			private List<String> Join = new List<String>();
			public Conversor()
			{

			}
		}
	}
}
