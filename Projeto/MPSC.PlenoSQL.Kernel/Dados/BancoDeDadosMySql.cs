using FirebirdSql.Data.FirebirdClient;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("My Sql")]
	public class BancoDeDadosMySql : BancoDeDados<MySqlConnection>
	{
		protected override String StringConexaoTemplate { get { return @"Data Source={0};Initial Catalog={1};User ID={2};Password={3};"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes) { throw new NotImplementedException("AllDatabasesSQL"); }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { throw new NotImplementedException("SQLAllProcedures"); }
		protected override String SQLTablesColumns
		{
			get
			{
				return @"
Select
    'T' As TipoTabela,
    T.TABLE_NAME As NomeTabela,
    T.TABLE_NAME As NomeInternoTabela,
    C.COLUMN_NAME As NomeColuna,
    CONCAT(
        Case
            When C.COLUMN_KEY = 'PRI' Then 'PK, '
            When C.COLUMN_KEY = 'MUL' Then 'FK, '
            Else ''
        End,
        C.COLUMN_TYPE, 
        Case
            When C.COLUMN_KEY = 'PRI' And EXTRA = 'auto_increment' Then ' Identity, NOT NULL'
            When C.IS_NULLABLE = 'NO' Then ', NOT NULL'
            Else ', NULL'
        End
   ) As DetalhesColuna
From INFORMATION_SCHEMA.TABLES T
Inner Join INFORMATION_SCHEMA.COLUMNS C On C.TABLE_NAME = T.TABLE_NAME
Where (T.TABLE_TYPE = 'BASE TABLE')";
			}
		}
	}
}