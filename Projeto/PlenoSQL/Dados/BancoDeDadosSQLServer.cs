using MPSC.PlenoSQL.AppWin.Dados.Base;
using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	[DisplayName("Sql Server")]
	public class BancoDeDadosSQLServer : BancoDeDados<SqlConnection>
	{
		protected override String StringConexaoTemplate { get { return "Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query.ToUpper().Replace("SELECT", "Select Top (100) Percent")); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " Where (B.Name Like '" + nome + "%')";
			return String.Format("Select Nome = B.Name{0} From Sys.SysDataBases B With (NoLock){1}", detalhes, filtro);
		}

		protected override String SQLTablesColumns { get { return QueryOf.cQueryCacheTablesColumns; } }

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (P.Name Like '" + nome + "%')";
			return String.Format(@"Select Nome = P.Name{0} From SysObjects P With (NoLock) Where (P.Type = 'P'){1}", detalhes, filtro);
		}

		protected class QueryOf : Query
		{
			public const String cQueryCacheTablesColumns = @"
Select
	TipoTabela = (Case Tab.Type When 'U' Then 'T' Else Tab.Type End),
	NomeTabela = Tab.Name,
	NomeInternoTabela = Tab.Name,
	NomeColuna = Col.Name,
	DetalhesColuna = IsNull((
		Select Top 1 Case I.is_primary_key When 1 Then 'PK, ' Else 'FK, ' End From Sys.Indexes I With (NoLock)
		Inner Join Sys.index_columns IC With (NoLock) On IC.index_id = I.index_id And IC.Object_Id = I.Object_Id
		Where (I.Object_Id = Col.Object_Id) And (IC.column_id = Col.column_id)
	), '') + Case Col.System_Type_Id
		When 127 Then 'BigInt'
		When 56  Then 'Int'
		When 106 Then 'Decimal(' + Convert(varchar, Col.Precision) + ', ' + Convert(varchar, Col.Scale) + ')'
		When 61  Then 'DateTime'
		When 175 Then 'Char(' + Convert(varchar, Col.Max_Length) + ')'
		When 167 Then 'VarChar(' + Convert(varchar, Col.Max_Length) + ')'
	End + Case
		When Col.Is_Nullable = 1 Then ', NULL'
		When Col.Is_Identity = 0 Then ', NOT NULL'
		Else ' Identity, NOT NULL'
	End
From Sys.Columns Col With (NoLock)
Inner Join Sys.Objects Tab With (NoLock) On Tab.Object_Id = Col.Object_Id
Where (Tab.Type <> 'S')";
		}
	}
}