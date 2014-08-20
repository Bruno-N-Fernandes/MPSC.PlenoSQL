﻿using System;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForIBM_DB2 : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "Ole DB For IBM DB2"; } }
		protected override String StringConexaoTemplate { get { return "Provider=IBMDA400;Data Source={0};Default Collection={1};User ID={2};Password={3}"; } }
	}
}