using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
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

	public class Filler
	{
		public static readonly List<Fill> fills = new List<Fill>();
		public static void Registrar(Fill fill) { fills.Add(fill); }

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