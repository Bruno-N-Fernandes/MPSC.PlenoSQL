using System;
using IBM.Data.DB2.iSeries;
using MP.PlenoBDNE.AppWin.Dados.Base;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosIBMDB2 : BancoDeDados<iDB2Connection>
	{
		public override String Descricao { get { return "IBM DB2"; } }
		protected override String StringConexaoTemplate { get { return "DataSource={0};UserID={2};Password={3};DataCompression=True;SortSequence=SharedWeight;SortLanguageId=PTG;DefaultCollection={1};"; } }
		protected override String AllTablesSQL(Boolean comDetalhes) { return @"Select Table_Name As Nome, '' As Detalhes From SysTables Where (Table_Name Like '{0}%')"; }

		protected override String AllViewsSQL(String nome)
		{
			var detalhes = String.IsNullOrWhiteSpace(nome) ? String.Empty : ", '' As Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : " Where (V.Name Like '" + nome + "%')";
			return String.Format(@"Select V.Name As Nome{0} From SysViews V{1}", detalhes, filtro);
		}

		protected override String AllColumnsSQL(String parent, Boolean comDetalhes)
		{
			var detalhes = comDetalhes ? @",
	(' (' ||
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
		Case When Col.Is_Nullable = 'Y' Then 'NULL)' Else 'NOT NULL)' End
	) As Detalhes" : string.Empty;
			var filtro = String.IsNullOrWhiteSpace(parent) ? String.Empty : " Where (Col.Table_Name = '" + parent + "')";


			return String.Format(@"Select Col.Column_Name as Nome{0} From SysColumns Col{1}", detalhes, filtro);
		}

		protected override String AllProceduresSQL(String nome)
		{
			var detalhes = String.IsNullOrWhiteSpace(nome) ? String.Empty : ", Routine_Definition as Detalhes";
			var filtro = String.IsNullOrWhiteSpace(nome) ? String.Empty : "And (Routine_Name = '" + nome + "')";
			return String.Format(@"
Select Routine_Schema || '.' || Routine_Name || ' (' || External_name || ')' As Nome {0}
From SYSIBM.Routines
Where (Routine_Type = 'PROCEDURE') {1}
And (Specific_Schema In (Select Current_Schema From SYSIBM.SysDummy1))
Order by Routine_Schema, Routine_Name", detalhes, filtro);
		}

		protected override String AllDatabasesSQL(Boolean comDetalhes)
		{
			return @"Select Schema_Name From SYSIBM.Schemata Where (Schema_Owner <> 'QSYS') Order by Schema_Name";
		}
	}
}
/*
   


*/