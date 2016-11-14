using System;
using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public class Torpedo
	{
		public Int64 Id { get; set; }
		public DateTime Enviado { get; set; }
	}

	public class FillTorpedo : Fill<Torpedo>
	{
		protected override Torpedo Preencher(IDataRecord dataRecord)
		{
			return new Torpedo
			{
				Id = (Int64)dataRecord["Id"],
				Enviado = (DateTime)dataRecord["Enviado"]
			};
		}
	}
}