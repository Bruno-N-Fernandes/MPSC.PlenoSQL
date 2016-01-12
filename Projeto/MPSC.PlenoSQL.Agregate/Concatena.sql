IF Object_Id('dbo.Concatena') Is Not Null
	Drop Aggregate Concatena;
GO

IF Exists(Select * From Sys.Assemblies Where (Name = 'MPSC_PlenoSQL_Agregate'))
	Drop Assembly MPSC_PlenoSQL_Agregate;
GO

Create Assembly MPSC_PlenoSQL_Agregate Authorization dbo From  N'E:\Instala\MPSC.PlenoSQL.Agregate.dll' With Permission_Set = Safe;
GO

Create Aggregate Concatena(@valor nVarChar(Max), @separador nVarChar(16)) Returns nVarChar(Max) External Name MPSC_PlenoSQL_Agregate.Concatena;
GO

SP_Configure 'clr enabled', 1;
GO

Reconfigure;
GO

Select ObjRef_Medicamento, dbo.Concatena(MA.Apresentacao, null)
From ARKMED_MedicamentoApresentacao MA Group by ObjRef_Medicamento;
GO
