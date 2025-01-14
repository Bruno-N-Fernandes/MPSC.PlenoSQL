﻿using MPSC.PlenoSQL.Kernel.Dados.Base;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	/// <summary>
	/// https://www.ibm.com/support/knowledgecenter/SSAE4W_9.0.0/com.ibm.etools.iseries.langref2.doc/rbafzcatalogtbls.htm
	/// </summary>
	/// <typeparam name="TDb2Connection"></typeparam>
	[DisplayName("Db2")]
	public abstract class BancoDeDadosDb2<TDb2Connection> : BancoDeDados<TDb2Connection> where TDb2Connection : DbConnection, IDbConnection
	{
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}\r\n) As SubQueryOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (B.Schema_Name Like '" + nome + "%')";
			return $@"Select B.Schema_Name as Nome{detalhes} From SysIBM.Schemata B Where (B.Schema_Owner <> 'QSYS'){filtro} Order By 1 Asc";
		}

		protected override String SQLTablesIndexes { get => QueryOf.cQueryCacheTablesIndexes; }
		protected override String SQLTablesColumns { get => QueryOf.cQueryCacheTablesColumns; }

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? " || ' (' || R.External_name || ')'" : String.Empty;
			var definicao = String.IsNullOrWhiteSpace(nome) ? ", '' As Detalhes" : ", R.Routine_Definition As Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : "And (R.Routine_Name = '" + nome + "')";
			return $@"
Select R.Routine_Name{detalhes} As Nome {definicao}
From SysIBM.Routines R
Where (R.Routine_Type = 'PROCEDURE') {filtro}
And (R.Specific_Schema = (values current schema))
Order by R.Routine_Schema, R.Routine_Name";
		}

		protected virtual String SQLAllSequences()
		{
			return @"
Select
	Coalesce(S.Long_Comment, S.Sequence_Text) As Observacao,
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
			public const String cQueryCacheTablesIndexes = @"
Select
	Ind.Table_Schema,
	Ind.Table_Name,
	Ind.System_Index_Name,
	Ind.Index_Name,
	Ind.Is_Unique,
	Ind.Column_Count,
	Ind.Index_Definer,
	Ind.Index_Has_Search_Condition
From QSys2.SysIndexes Ind
Where (Ind.Table_Schema = (values current schema))";


			public const String cQueryCacheTablesColumns = @"
Select
	Tab.Table_Type As TipoTabela,
	Tab.Table_Name As NomeTabela,
	Tab.System_Table_Name As NomeInternoTabela,
	Col.Column_Name as NomeColuna,
	(
		Coalesce(
			(Select
				Case R.Constraint_Type
					When 'PRIMARY KEY' Then 'PK, '
					When 'FOREIGN KEY' Then 'FK, '
					When 'UNIQUE' Then 'UQ, '
					When 'CHECK' Then 'CK, '
					Else 'IX, '
				End
			From QSys2.SysCstCol CC
			Inner Join QSys2.SysCst R On (R.Constraint_Name = CC.Constraint_Name)
				And (R.Table_Name = CC.Table_Name) And (R.Constraint_Schema = CC.Table_Schema)
			Where (CC.Column_Name = Col.Column_Name) And (CC.Table_Name = Col.Table_Name)
			And (CC.Table_Schema = Col.Table_Schema) Fetch First 1 Row Only
		), '') ||
		Col.Data_Type ||
		Case 
			When (Col.Numeric_Scale Is Not Null) And (Col.Numeric_Scale > 0) Then  '(' || Cast(Col.Length as VarChar(5)) || ', ' || Cast(Col.Numeric_Scale As VarChar(5)) || '), '
			When Col.Data_Type Like '%CHAR%' Then '(' || Cast(Col.Length as VarChar(10)) || '), '
			Else ', '
		End || 
		Case When Col.Is_Nullable = 'Y' Then 'NULL' Else 'NOT NULL' End
	) As DetalhesColuna
From QSys2.SysTables Tab
Inner Join QSys2.SysColumns Col On (Col.System_Table_Name = Tab.System_Table_Name) And (Col.Table_Schema = Tab.Table_Schema)
Where (Tab.Table_Schema = (values current schema)) And (Tab.Table_Type <> 'P')";
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
