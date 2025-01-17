﻿using FirebirdSql.Data.FirebirdClient;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("Fire Bird")]
	public class BancoDeDadosFireBird : BancoDeDados<FbConnection>
	{
		protected override String StringConexaoTemplate { get { return @"DataSource={0};Database={1};User={2};Password={3};Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes) { throw new NotImplementedException("AllDatabasesSQL"); }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { throw new NotImplementedException("SQLAllProcedures"); }
		protected override String SQLTablesIndexes { get => String.Empty; }
		protected override String SQLTablesColumns { get { return @"Select rdb$relation_name As Nome, '' As Detalhes From rdb$relations Where ((rdb$system_flag is null) Or (rdb$system_flag = 0)) And (rdb$relation_name Like '{0}%')"; } }
	}
}