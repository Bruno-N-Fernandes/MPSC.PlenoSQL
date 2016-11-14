﻿using MPSC.PlenoSQL.TestesUnitarios.Conexao.AF;
using System;
using System.Data;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public class Torpedo
	{
		public Int64 Id { get; set; }
		public DateTime Enviado { get; set; }
		public String Mensagem { get; set; }
	}

	public class TorpedoFactory : ClassFactory<Torpedo>
	{
		public override Torpedo New(IDataRecord dataRecord)
		{
			return new Torpedo
			{
				Id = (Int64)dataRecord["Id"],
				Enviado = (DateTime)dataRecord["Enviado"]
			};
		}
	}
}