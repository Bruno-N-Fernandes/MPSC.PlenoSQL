using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public interface IConexao
	{
		IDataReader Executar(String cmdSql);
	}

	public class AutoFactory
	{
		private IConexao Conexao;
		public AutoFactory(IConexao conexao)
		{
			Conexao = conexao;
		}

		public IEnumerable<TEntidade> Query<TEntidade>(String cmdSql)
		{
			return QueryImpl<TEntidade>(cmdSql).ToArray();
		}

		public IEnumerable<TEntidade> QueryImpl<TEntidade>(String cmdSql)
		{
			var reader = Executar(cmdSql);
			while (reader.Read())
				yield return New<TEntidade>(reader);
		}

		private TEntidade New<TEntidade>(IDataRecord dataRecord)
		{
			return Filler.New<TEntidade>(dataRecord);
		}

		private IDataReader Executar(String cmdSql)
		{
			return Conexao.Executar(cmdSql);
		}
	}
}