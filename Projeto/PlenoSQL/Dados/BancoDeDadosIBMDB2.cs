using IBM.Data.DB2.iSeries;
using MPSC.PlenoSQL.AppWin.Dados.Base;
using MPSC.PlenoSQL.AppWin.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.Dados
{
	public class BancoDeDadosIBMDB2 : BancoDeDados<iDB2Connection>
	{
		public override String Descricao { get { return "IBM DB2"; } }
		protected override String StringConexaoTemplate { get { return "DataSource={0};UserID={2};Password={3};DataCompression=True;SortSequence=SharedWeight;SortLanguageId=PTG;DefaultCollection={1};"; } }
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }

		protected override String SQLAllDatabases(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? ", '' As Detalhes" : String.Empty;
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " And (B.Schema_Name Like '" + nome + "%')";
			return @"Select B.Schema_Name as Nome{0} From SYSIBM.Schemata B Where (B.Schema_Owner <> 'QSYS'){1}";
		}

		protected override String SQLTablesColumns { get { return QueryOf.cQueryCacheTablesColumns; } }

		protected override String SQLAllProcedures(String nome, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? " || ' (' || External_name || ')'" : String.Empty;
			var definicao = String.IsNullOrWhiteSpace(nome) ? ", '' as Detalhes" : ", Routine_Definition as Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : "And (Routine_Name = '" + nome + "')";
			return String.Format(@"
Select Routine_Name{0} As Nome {1}
From SYSIBM.Routines
Where (Routine_Type = 'PROCEDURE') {2}
And (Specific_Schema = (values current schema))
Order by Routine_Schema, Routine_Name", detalhes, definicao, filtro);
		}


		public override IBancoDeDados Clone()
		{
			var iBancoDeDados = new BancoDeDadosIBMDB2();
			iBancoDeDados.ConfigurarConexao(_server, _dataBase, _usuario, _senha);
			return iBancoDeDados;
		}

		protected class QueryOf : Query
		{
			public const String cQueryCacheTablesColumns = @"
Select
	Tab.Table_Type As TipoTabela,
	Tab.Table_Name As NomeTabela,
	Tab.System_Table_Name As NomeInternoTabela,
	Tab.Table_Text As DescricaoTabela,
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
			From SYSCSTCOL CC
			Inner Join SYSCST R On (R.Constraint_Name = CC.Constraint_Name) And (R.Table_Name = CC.Table_Name)
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
set schema <SchemaName>
*/