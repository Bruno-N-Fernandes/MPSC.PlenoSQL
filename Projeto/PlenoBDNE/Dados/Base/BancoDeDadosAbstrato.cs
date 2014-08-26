using System;
using System.Collections.Generic;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public static class BancoDeDadosAbstrato
	{
		private static List<KeyValuePair<String, Type>> _lista;
		public static IEnumerable<KeyValuePair<String, Type>> ListaDeBancoDeDados { get { return _lista ?? (_lista = LoadList()); } }

		private static List<KeyValuePair<String, Type>> LoadList()
		{
			return new List<KeyValuePair<String, Type>>(LoadEnum());
		}

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
			var tipo = typeof(TIBancoDeDados);
			var banco = Activator.CreateInstance(tipo) as IBancoDeDados;
			var retorno = new KeyValuePair<String, Type>(banco.Descricao, tipo);
			banco.Dispose();
			return retorno;
		}

		public static void Clear()
		{
			if (_lista != null)
				_lista.Clear();
			_lista = null;
		}
	}
}