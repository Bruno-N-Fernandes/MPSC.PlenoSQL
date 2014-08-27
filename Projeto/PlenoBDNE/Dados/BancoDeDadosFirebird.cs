using System;
using FirebirdSql.Data.FirebirdClient;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosFireBird : BancoDeDados<FbConnection>
	{
		public override String Descricao { get { return "Fire Bird"; } }
		protected override String StringConexaoTemplate { get { return @"DataSource={0};Database={1};User={2};Password={3};Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;"; } }

		protected override String SQLAllDatabases(String nome) { throw new NotImplementedException("AllDatabasesSQL"); }
		protected override String SQLAllTables(String nome, Boolean comDetalhes) { return @"Select rdb$relation_name As Nome, '' As Detalhes From rdb$relations Where ((rdb$system_flag is null) Or (rdb$system_flag = 0)) And (rdb$relation_name Like '{0}%')"; }
		protected override String SQLAllViews(String nome, Boolean comDetalhes) { throw new NotImplementedException("AllViewsSQL"); }
		protected override String SQLAllColumns(String parent, Boolean comDetalhes) { throw new NotImplementedException("SQLAllColumns"); }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { throw new NotImplementedException("SQLAllProcedures"); }
	}
}