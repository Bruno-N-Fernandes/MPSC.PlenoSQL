using System;
using System.Linq;
using System.Data;
using System.Reflection;

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
	}

	internal class ClassFactoryAuto<TEntidade> : ClassFactory<TEntidade>
	{
		private readonly PropertyInfo[] properties;

		internal ClassFactoryAuto()
		{
			properties = typeof(TEntidade).GetProperties().Where(p => p.CanWrite).ToArray();
		}

		public override TEntidade New(IDataRecord dataRecord)
		{
			var entidade = Activator.CreateInstance<TEntidade>();

			foreach (var property in properties)
			{
				var index = dataRecord.GetOrdinal(property.Name);
				if ((index >= 0) && !dataRecord.IsDBNull(index))
					property.SetValue(entidade, dataRecord.GetValue(index), null);
			}

			return entidade;
		}

	}
}