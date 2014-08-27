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

		IEnumerable<String> ListarColunas(String parent, Boolean listarDetalhes);
		IEnumerable<String> ListarTabelas(String nome);
		IEnumerable<String> ListarViews(String nome);
		IEnumerable<String> ListarProcedures(String nome);

		void SetMessageResult(IMessageResult iMessageResult);
	}
}