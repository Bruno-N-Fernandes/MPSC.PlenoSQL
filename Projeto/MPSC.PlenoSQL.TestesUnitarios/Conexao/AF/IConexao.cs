using System;
using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao.AF
{
	public interface IConexao
	{
		IDataReader Executar(String cmdSql);
	}
}