using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public static class BancoDeDadosExtension
	{
		private static List<KeyValuePair<String, Type>> _lista;
		public static IEnumerable<KeyValuePair<String, Type>> ListaDeBancoDeDados { get { return _lista ?? (_lista = LoadEnum().ToList()); } }

		private static IEnumerable<KeyValuePair<String, Type>> LoadEnum()
		{
			yield return LoadBanco<BancoDeDadosSQLServer>();
			yield return LoadBanco<BancoDeDadosSQLite>();
			yield return LoadBanco<BancoDeDadosIBMDB2>();
			yield return LoadBanco<BancoDeDadosFireBird>();
			yield return LoadBanco<BancoDeDadosOleDbForIBM_DB2>();
			yield return LoadBanco<BancoDeDadosOleDbForExcel>();
			yield return LoadBanco<BancoDeDadosOleDbForAccess>();
		}

		private static KeyValuePair<String, Type> LoadBanco<TIBancoDeDados>() where TIBancoDeDados : class, IBancoDeDados
		{
			try
			{
				var tipo = typeof(TIBancoDeDados);
				var banco = Activator.CreateInstance(tipo) as IBancoDeDados;
				var retorno = new KeyValuePair<String, Type>(banco.Descricao, tipo);
				banco.Dispose();
				return retorno;
			}
			catch (Exception) { }

			return default(KeyValuePair<String, Type>);
		}

		public static void Clear()
		{
			if (_lista != null)
				_lista.Clear();
			_lista = null;
		}

		public static IDbCommand CriarComando(this IDbConnection iDbConnection, String query)
		{
			IDbCommand iDbCommand = null;
			if ((iDbConnection != null) && !String.IsNullOrWhiteSpace(query))
			{
				if (iDbConnection.State != ConnectionState.Open)
					iDbConnection.Open();

				iDbCommand = iDbConnection.CreateCommand();
				iDbCommand.CommandText = query;
				iDbCommand.CommandType = query.ToLower().StartsWith("exec") || (query.IndexOfAny("\r\n\t ".ToCharArray()) < 0) ? CommandType.StoredProcedure : CommandType.Text;
				iDbCommand.CommandTimeout = 3600;
			}
			return iDbCommand;
		}

		public static Boolean IsOpen(this IDataReader iDataReader)
		{
			return (iDataReader != null) && !iDataReader.IsClosed;
		}
	}
}