using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public abstract class BancoDeDadosAbstrato : BancoDeDadosGenerico<IDbConnection>
	{
		public static IList<IBancoDeDados> ListaDeBancoDeDados = Load();
		private static IList<IBancoDeDados> Load()
		{
			return new List<IBancoDeDados>(
				new IBancoDeDados[]
				{
					new BancoDeDadosSQLServer(),
					new BancoDeDadosSQLite(),
					new BancoDeDadosIBMDB2(),
					new BancoDeDadosFireBird(),
					new BancoDeDadosOleDbForIBM_DB2(),
					new BancoDeDadosOleDbForExcel(),
					new BancoDeDadosOleDbForAccess(),
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