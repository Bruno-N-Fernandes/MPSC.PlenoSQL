﻿using System;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForAccess : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "Ole DB For MS Access"; } }
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;"; } }
	}
}