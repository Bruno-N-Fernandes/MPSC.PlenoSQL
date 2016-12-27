using MPSC.PlenoSQL.Kernel.Dados.Base;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("Db2")]
	public abstract class BancoDeDadosDb2<TDb2Connection> : BancoDeDados<TDb2Connection> where TDb2Connection : DbConnection, IDbConnection
	{
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}\r\n) As SubQueryOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (B.Schema_Name Like '" + nome + "%')";
			return @"Select B.Schema_Name as Nome{0} From SysIBM.Schemata B Where (B.Schema_Owner <> 'QSYS'){1}";
		}

		protected override String SQLTablesColumns { get { return QueryOf.cQueryCacheTablesColumns; } }

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? " || ' (' || R.External_name || ')'" : String.Empty;
			var definicao = String.IsNullOrWhiteSpace(nome) ? ", '' As Detalhes" : ", R.Routine_Definition As Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : "And (R.Routine_Name = '" + nome + "')";
			return String.Format(@"
Select R.Routine_Name{0} As Nome {1}
From SysIBM.Routines R
Where (R.Routine_Type = 'PROCEDURE') {2}
And (R.Specific_Schema = (values current schema))
Order by R.Routine_Schema, R.Routine_Name", detalhes, definicao, filtro);
		}

		protected virtual String SQLAllSequences()
		{
			return @"
Select
	IFNull(S.Long_Comment, S.Sequence_Text) As Observacao,
	(
		Trim(S.Sequence_Schema) || '.' || Trim(S.Sequence_Name) || ' (' ||
		Trim(S.System_Seq_Schema) || '.' || Trim(S.System_Seq_Name) ||')'
	) As Sequence,
	S.Sequence_Created,
	S.Sequence_Definer
From QSys2.SysSeqObjects S
Where (S.Sequence_Schema = (values current schema))
Order By S.Sequence_Name";
		}

		protected class QueryOf : Query
		{
			public const String cQueryCacheTablesColumns = @"
Select
	Tab.Table_Type As TipoTabela,
	Tab.Table_Name As NomeTabela,
	Tab.System_Table_Name As NomeInternoTabela,
	Col.Column_Name as NomeColuna,
	(
		IfNull(
			(Select
				Case R.Constraint_Type
					When 'PRIMARY KEY' Then 'PK, '
					When 'FOREIGN KEY' Then 'FK, '
					When 'UNIQUE' Then 'UQ, '
					When 'CHECK' Then 'CK, '
					Else 'IX, '
				End
			From SysCstCol CC
			Inner Join SysCst R On (R.Constraint_Name = CC.Constraint_Name) And (R.Table_Name = CC.Table_Name)
			Where (CC.Table_Name = Col.Table_Name) And (CC.Column_Name = Col.Column_Name)
			And (R.Table_Name = Col.Table_Name) Fetch First 1 Row Only
		), '') ||
		Col.Data_Type ||
		Case 
			When (Col.Numeric_Scale Is Not Null) And (Col.Numeric_Scale > 0) Then  '(' || Cast(Col.Length as varchar(5)) || ', ' || Cast(Col.Numeric_Scale As VarChar(5)) || '), '
			When Col.Data_Type Like '%CHAR%' Then '(' || Cast(Col.Length as varchar(10)) || '), '
			Else ', '
		End || 
		Case When Col.Is_Nullable = 'Y' Then 'NULL' Else 'NOT NULL' End
	) As DetalhesColuna
From SysColumns Col
Inner Join SysTables Tab On (Tab.System_Table_Name = Col.System_Table_Name)
Where (Tab.Table_Type <> 'P')";
		}

	}
}
/*
Set Schema <SchemaName>;

Select Replace(SQ.CmdSql, 'PUBLIC', 'NewUser') As CmdSql
From (
	Select ('GRANT EXECUTE ON'		|| ' ' || R.Routine_Type ||
			' ' || R.Routine_Schema	|| '.' || R.Routine_Name ||
			' TO ' || 'PUBLIC'		|| ';' ) As CmdSql --, R.*
	From SysIBM.Routines R Where (R.Routine_Schema = (values current schema)) And (R.Routine_Type In ('FUNCTION', 'PROCEDURE'))

	Union All Select ('GRANT USAGE ON'	|| ' ' || 'SEQUENCE' ||
			' ' || S.Sequence_Schema	|| '.' || S.Sequence_Name ||
			' TO ' || 'PUBLIC'			|| ';' ) As CmdSql --, S.*
	From QSys2.SysSeqObjects S Where (S.Sequence_Schema = (values current schema))
) As SQ
Order By SQ.CmdSql;
 * 
 * 
 SET LOC_CUPDATE = 'update esim.ressegurofinanceiroparcialfluxocaixa '
|| 'set ' || LOC_CCOLUNA || ' = null'
|| ' where tipovalor = 2' ;

EXECUTE IMMEDIATE LOC_CUPDATE ;
*/