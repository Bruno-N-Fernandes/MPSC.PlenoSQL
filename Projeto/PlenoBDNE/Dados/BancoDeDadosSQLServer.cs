using System;
using System.Data.SqlClient;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLServer : BancoDeDados<SqlConnection>
	{
		public override String Descricao { get { return "Sql Server"; } }
		protected override String StringConexaoTemplate { get { return "Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;"; } }
		protected override String SQLAllTables(Boolean comDetalhes) { return @"Select T.Name As Nome, '' As Detalhes From SysObjects T With (NoLock) Where (T.Type = 'U') And (T.Name Like '{0}%')"; }

		protected override String SQLAllViews(String nome)
		{
			var detalhes = String.IsNullOrWhiteSpace(nome) ? String.Empty : ", '' As Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"
Select T.Name As Nome{0}
From SysObjects T With (NoLock)
Where (T.Type = 'V'){1}", detalhes, filtro);
		}

		protected override String SQLAllColumns(String parent, Boolean comDetalhes)
		{
			return @"
Select
	Nome = C.Name,
	Detalhes = ' (' + IsNull((
		Select Top 1 Case I.is_primary_key When 1 Then 'PK, ' Else 'FK, ' End From Sys.Indexes I With (NoLock)
		Inner Join Sys.index_columns IC With (NoLock) On IC.index_id = I.index_id And IC.Object_Id = I.Object_Id
		Where (I.Object_Id = C.Object_Id) And (IC.column_id = C.column_id)
	), '') + Case C.System_Type_Id
		When 127 Then 'BigInt'
		When 56  Then 'Int'
		When 106 Then 'Decimal(' + Convert(varchar, Precision) + ', ' + Convert(varchar, Scale) + ')'
		When 61  Then 'DateTime'
		When 175 Then 'Char(' + Convert(varchar, Max_Length) + ')'
		When 167 Then 'VarChar(' + Convert(varchar, Max_Length) + ')'
	End	+ Case
		When Is_Nullable = 1 Then ', NULL)'
		When Is_Identity = 0 Then ', NOT NULL)'
		Else ' Identity, NOT NULL)'
	End
From Sys.Columns C With (NoLock)
Where (C.Object_Id = Object_Id('{0}'))";
		}

		protected override String SQLAllProcedures(String nome) { throw new NotImplementedException("SQLAllProcedures"); }
		protected override String SQLAllDatabases(Boolean comDetalhes) { throw new NotImplementedException("AllDatabasesSQL"); }
	}
}