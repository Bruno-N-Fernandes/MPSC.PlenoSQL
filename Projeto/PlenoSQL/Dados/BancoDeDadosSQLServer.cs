using System;
using System.Data.SqlClient;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosSQLServer : BancoDeDados<SqlConnection>
	{
		public override String Descricao { get { return "Sql Server"; } }
		protected override String StringConexaoTemplate { get { return "Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query.ToUpper().Replace("SELECT", "Select Top (100) Percent")); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " Where (B.Name Like '" + nome + "%')";
			return String.Format("Select Nome = B.Name{0} From Sys.SysDataBases B With (NoLock){1}", detalhes, filtro);
		}

		protected override String SQLAllTables(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Nome{0} From SysObjects T With (NoLock) Where (T.Type = 'U'){1}", detalhes, filtro);
		}

		protected override String SQLAllViews(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (T.Name Like '" + nome + "%')";
			return String.Format(@"Select T.Name As Nome{0} From SysObjects T With (NoLock) Where (T.Type = 'V'){1}", detalhes, filtro);
		}

		protected override String SQLAllColumns(String parent, Boolean comDetalhes)
		{
			var filtro = String.IsNullOrWhiteSpace(parent) ? String.Empty : " Where (C.Object_Id = Object_Id('" + parent + "'))";
			var detalhes = comDetalhes ? @",
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
	End" : String.Empty;

			return String.Format(@"Select Nome = C.Name{0} From Sys.Columns C With (NoLock){1}", detalhes, filtro);
		}

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (P.Name Like '" + nome + "%')";
			return String.Format(@"Select Nome = P.Name{0} From SysObjects P With (NoLock) Where (P.Type = 'P'){1}", detalhes, filtro);
		}
	}
}