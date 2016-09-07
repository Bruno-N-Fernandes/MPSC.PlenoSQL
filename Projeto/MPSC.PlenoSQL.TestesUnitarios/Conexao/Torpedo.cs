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
		protected Fill(Type tipo) { Tipo = tipo; }

		public TEntidade New<TEntidade>(IDataRecord dataRecord)
		{
			return (TEntidade)Preencher(dataRecord);
		}

		protected abstract Object Preencher(IDataRecord dataRecord);
	}

	public abstract class Fill<TEntidade> : Fill
	{
		protected Fill() : base(typeof(TEntidade)) { }
	}

	public class FillTorpedo : Fill<Torpedo>
	{
		protected override Object Preencher(IDataRecord dataRecord)
		{
			return new Torpedo
			{
				Id = (Int64)dataRecord["Id"],
				Enviado = (DateTime)dataRecord["Enviado"]
			};
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
			yield return new FillTorpedo();
		}

		public static TEntidade New<TEntidade>(IDataRecord dataRecord)
		{
			return getFill(typeof(TEntidade)).New<TEntidade>(dataRecord);
		}

		private static Fill getFill(Type tipo)
		{
			return fills.FirstOrDefault(f => f.Tipo == tipo);
		}
	}
}