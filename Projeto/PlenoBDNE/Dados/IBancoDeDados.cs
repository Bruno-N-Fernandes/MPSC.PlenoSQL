using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Infra.Interface;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public interface IBancoDeDados : IDisposable
	{
		String Descricao { get; }
		IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha);

		void Executar(String query, IMessageResult messageResult);
		IDataReader ExecutarQuery(String query);
		IEnumerable<Object> Transformar();
		IEnumerable<Object> Cabecalho();

		IEnumerable<String> ListarColunasDasTabelas(String tabela);
		IEnumerable<String> ListarTabelas(String tabela);
	}

	public abstract class BancoDeDados : BancoDeDados<IDbConnection>
	{
		public static IList<IBancoDeDados> ListaDeBancoDeDados = Load();
		private static IList<IBancoDeDados> Load()
		{
			return new List<IBancoDeDados>(
				new IBancoDeDados[]
				{
					new BancoDeDadosSQLServer(),
					new BancoDeDadosOleDb(),
					new BancoDeDadosIBMDB2(),
					new BancoDeDadosSQLite(),
					new BancoDeDadosFireBird()
				}
			);
		}
	}
}