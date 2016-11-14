using System;
using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao.AF
{
	public abstract class ClassFactory<TEntidade>
	{
		private static ClassFactory<TEntidade> _filler;
		protected ClassFactory() { _filler = this; }

		public abstract TEntidade New(IDataRecord dataRecord);

		public static ClassFactory<TEntidade> Get()
		{
			return _filler ?? new ClassFactoryAuto<TEntidade>();
		}


		protected T GetValue<T>(IDataRecord dataRecord, String name, T defaultvalue)
		{
			var index = dataRecord.GetOrdinal(name);
			return (index < 0) ? defaultvalue : (T)GetValue(dataRecord, index, defaultvalue);
		}

		protected T GetValue<T>(IDataRecord dataRecord, Int32 index, T defaultvalue)
		{
			return dataRecord.IsDBNull(index) ? defaultvalue : (T)dataRecord.GetValue(index);
		}
	}
}