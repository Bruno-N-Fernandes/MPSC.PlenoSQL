using System;
using System.Collections.Generic;
using System.Data;

namespace MP.PlenoBDNE.AppWin.Interface
{
	public interface IBancoDeDados : IDisposable
	{
		String Descricao { get; }
		String Conexao { get; }
		IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha);

		Object Executar(String query);
		IDataReader ExecutarQuery(String query);
		IEnumerable<Object> DataBinding();

		IEnumerable<String> ListarColunasDasTabelas(String tabela, Boolean listarDetalhes);
		IEnumerable<String> ListarTabelas(String tabela);
		IEnumerable<String> ListarViews(String view);

		void SetMessageResult(IMessageResult iMessageResult);
	}
}