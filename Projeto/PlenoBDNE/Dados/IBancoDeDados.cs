using System;
using System.Collections.Generic;
using System.Data;

namespace LBJC.NavegadorDeDados.Dados
{
	public interface IBancoDeDados : IDisposable
	{
		String Descricao { get; }
		IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha);

		void Executar(String query);
		IDataReader ExecutarQuery(String query);
		IEnumerable<Object> Transformar();
		IEnumerable<Object> Cabecalho();

		IEnumerable<String> ListarColunasDasTabelas(String tabela);
		IEnumerable<String> ListarTabelas(String tabela);
	}
}