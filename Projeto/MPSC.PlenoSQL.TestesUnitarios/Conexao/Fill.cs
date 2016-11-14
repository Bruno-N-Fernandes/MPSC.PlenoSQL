using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public abstract class Fill<TEntidade>
	{
		private static Fill<TEntidade> _filler;
		protected Fill() { _filler = this; }

		protected abstract TEntidade Preencher(IDataRecord dataRecord);
		public static TEntidade New(IDataRecord dataRecord)
		{
			return _filler.Preencher(dataRecord);
		}
	}
}