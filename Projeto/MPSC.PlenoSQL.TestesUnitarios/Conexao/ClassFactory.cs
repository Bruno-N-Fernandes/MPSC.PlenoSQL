using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public abstract class ClassFactory<TEntidade>
	{
		private static ClassFactory<TEntidade> _filler;
		protected ClassFactory() { _filler = this; }

		public abstract TEntidade New(IDataRecord dataRecord);

		public static ClassFactory<TEntidade> Get()
		{
			return _filler;
		}
	}
}