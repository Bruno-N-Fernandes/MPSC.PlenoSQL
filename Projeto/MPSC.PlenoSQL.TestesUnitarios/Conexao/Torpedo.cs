using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public class Torpedo
	{
		public Int64 Id { get; set; }
		public DateTime Enviado { get; set; }
	}


	public abstract class Fill
	{
		public readonly Type Tipo;
		private readonly Func<IDataRecord, Object> _fill;
		protected Fill(Type tipo, Func<IDataRecord, Object> fill)
		{
			Tipo = tipo;
			_fill = fill;
		}

		public TEntidade New<TEntidade>(IDataRecord dataRecord)
		{
			return (TEntidade)_fill(dataRecord);
		}
	}

	public class Fill<TEntidade> : Fill
	{
		public Fill(Func<IDataRecord, Object> fill)
			: base(typeof(TEntidade), fill)
		{
		}
	}

	public class Filler
	{
		public static readonly Fill[] fills;
		static Filler()
		{
			fills = Inicializar().ToArray();
		}

		private static IEnumerable<Fill> Inicializar()
		{
			yield return new Fill<Torpedo>(d => new Torpedo { Id = (Int64)d["Id"], Enviado = (DateTime)d["Enviado"] });
		}

		public static TEntidade New<TEntidade>(IDataRecord dataRecord)
		{
			var tipo = typeof(TEntidade);
			return fills.FirstOrDefault(f => f.Tipo == tipo).New<TEntidade>(dataRecord);
		}
	}
}