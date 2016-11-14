using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao.AF
{
	internal sealed class ClassFactoryAuto<TEntidade> : ClassFactory<TEntidade>
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
				var value = GetValue<Object>(dataRecord, property.Name, null);
				if (value != null) 
					property.SetValue(entidade, value, null);
			}

			return entidade;
		}
	}
}