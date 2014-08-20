using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Infra.Interface;

namespace MP.PlenoBDNE.AppWin.Dados
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

		IEnumerable<String> ListarColunasDasTabelas(String tabela);
		IEnumerable<String> ListarTabelas(String tabela);
		IEnumerable<String> ListarViews(String view);
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

		public static void Clear()
		{
			if (ListaDeBancoDeDados != null)
				ListaDeBancoDeDados.Clear();
			ListaDeBancoDeDados = null;
		}
	}
}