using System;
using System.Collections.Generic;

namespace MPSC.PlenoSQL.Kernel.Interface
{
	public interface IBancoDeDados : IDisposable
	{
		Type Tipo { get; }
		String Conexao { get; }
		void ConfigurarConexao(String server, String dataBase, String usuario, String senha);
		String TestarConexao();

		Object Executar(String query, Boolean comEstatisticas, out Boolean retornaDados);
		IEnumerable<Object> DataBinding(Int64 limite);

		IEnumerable<String> ListarBancosDeDados(String nome, Boolean comDetalhes);
		IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes);
		IEnumerable<String> ListarColunas(String parent, String filtro, Boolean comDetalhes);
		IEnumerable<String> ListarViews(String nome, Boolean comDetalhes);
		IEnumerable<String> ListarProcedures(String nome, Boolean comDetalhes);

		void PreencherCache();
		void SetMessageResult(IMessageResult iMessageResult);
		IBancoDeDados Clone();
	}
}