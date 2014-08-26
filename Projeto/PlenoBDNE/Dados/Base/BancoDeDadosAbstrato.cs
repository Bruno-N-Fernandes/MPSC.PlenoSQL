using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Interface;
using MP.PlenoBDNE.AppWin.View;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public abstract class BancoDeDadosAbstrato : BancoDeDadosGenerico<IDbConnection>
	{
		public static IList<KeyValuePair<String, Type>> ListaDeBancoDeDados = new List<KeyValuePair<String, Type>>(Load());
		private static IEnumerable<KeyValuePair<String, Type>> Load()
		{
			yield return Banco<BancoDeDadosSQLServer>();
			yield return Banco<BancoDeDadosSQLite>();
			yield return Banco<BancoDeDadosIBMDB2>();
			yield return Banco<BancoDeDadosFireBird>();
			yield return Banco<BancoDeDadosOleDbForIBM_DB2>();
			yield return Banco<BancoDeDadosOleDbForExcel>();
			yield return Banco<BancoDeDadosOleDbForAccess>();
		}

		private static KeyValuePair<String, Type> Banco<TIBancoDeDados>() where TIBancoDeDados : class, IBancoDeDados
		{
			var tipo = typeof(TIBancoDeDados);
			var banco = Activator.CreateInstance(tipo) as IBancoDeDados;
			var retorno = new KeyValuePair<String, Type>(banco.Descricao, tipo);
			banco.Dispose();
			return retorno;
		}

		public static void Clear()
		{
			if (ListaDeBancoDeDados != null)
				ListaDeBancoDeDados.Clear();
			ListaDeBancoDeDados = null;
		}
	}
}