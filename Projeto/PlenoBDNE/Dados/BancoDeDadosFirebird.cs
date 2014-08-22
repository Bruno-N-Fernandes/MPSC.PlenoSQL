using System;
using FirebirdSql.Data.FirebirdClient;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosFireBird : BancoDeDados<FbConnection>
	{
		public override String Descricao { get { return "Fire Bird"; } }
		protected override String StringConexaoTemplate { get { return @"DataSource={0};Database={1};User={2};Password={3};Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;"; } }
		protected override String AllTablesSQL { get { return @"Select rdb$relation_name As Nome, '' As Detalhes From rdb$relations Where ((rdb$system_flag is null) Or (rdb$system_flag = 0)) And (rdb$relation_name Like '{0}%')"; } }
		protected override String AllViewsSQL { get { throw new NotImplementedException("AllViewsSQL"); } }
		protected override String AllColumnsSQL { get { throw new NotImplementedException("AllColumnsSQL"); } }
	}
}