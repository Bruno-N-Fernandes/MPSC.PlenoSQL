using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao.AF
{
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
			var classFactory = ClassFactory<TEntidade>.Get();
			var dataReader = Executar(cmdSql);
			while (dataReader.Read())
				yield return classFactory.New(dataReader);
			dataReader.Close();
			dataReader.Dispose();
		}

		private IDataReader Executar(String cmdSql)
		{
			return Conexao.Executar(cmdSql);
		}

		public static void Registrar<TEntidade, TFill>() where TFill : ClassFactory<TEntidade>, new()
		{
			new TFill();
		}
	}
}