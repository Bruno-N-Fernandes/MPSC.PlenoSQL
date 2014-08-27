using System;
using System.Collections.Generic;
using System.Data;

namespace MP.PlenoBDNE.AppWin.Interface
{
	public interface IBancoDeDados : IDisposable
	{
		String Descricao { get; }
		String Conexao { get; }
		String TestarConexao(String server, String dataBase, String usuario, String senha);

		Object Executar(String query);
		IEnumerable<Object> DataBinding();

		IEnumerable<String> ListarBancosDeDados(String nome);
		IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes);
		IEnumerable<String> ListarColunas(String parent, Boolean comDetalhes);
		IEnumerable<String> ListarViews(String nome, Boolean comDetalhes);
		IEnumerable<String> ListarProcedures(String nome);

		void SetMessageResult(IMessageResult iMessageResult);
	}
}