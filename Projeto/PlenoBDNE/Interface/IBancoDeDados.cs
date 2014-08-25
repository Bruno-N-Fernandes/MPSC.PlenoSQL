using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Interface
{
	public interface IBancoDeDados : IDisposable
	{
		String Descricao { get; }
		String Conexao { get; }
		IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha);

		void Executar(String query, IMessageResult messageResult);
		IDataReader ExecutarQuery(String query);
		IEnumerable<Object> Transformar();
		IEnumerable<Object> Cabecalho();

		IEnumerable<String> ListarColunasDasTabelas(String tabela, Boolean listarDetalhes);
		IEnumerable<String> ListarTabelas(String tabela);
		IEnumerable<String> ListarViews(String view);
	}
}