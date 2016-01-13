IF (Object_Id('dbo.RegExpReplace') Is Not Null)
	Drop Function RegExpReplace;
GO

IF (Object_Id('dbo.Concatena') Is Not Null)
	Drop Aggregate Concatena;
GO


/* + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + */
IF (Exists(Select * From Sys.Assemblies Where (Name = 'MPSC.PlenoSQL.SqlServer.Extensions')))
	Drop Assembly [MPSC.PlenoSQL.SqlServer.Extensions];
GO
Create Assembly [MPSC.PlenoSQL.SqlServer.Extensions] Authorization dbo
From  N'E:\Instala\MPSC.PlenoSQL.SqlServer.Extensions.dll' With Permission_Set = Safe;
GO
/* + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + */



Create Aggregate Concatena(@valor nVarChar(Max), @separador nVarChar(16))
Returns nVarChar(Max) External Name [MPSC.PlenoSQL.SqlServer.Extensions].[MPSC.PlenoSQL.SqlServer.Extensions.Agregacoes.Concatena];
GO

Create Function RegExpReplace(@input nVarChar(Max), @expressaoRegular nVarChar(1024), @substituicao nVarChar(Max))
Returns nVarChar(Max) As External Name [MPSC.PlenoSQL.SqlServer.Extensions].[MPSC.PlenoSQL.SqlServer.Extensions.Strings].RegExpReplace;
GO



/* + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + */
SP_Configure 'clr enabled', 1;
GO
Reconfigure;
GO
/* + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + - + */




Select Top 10
	ObjRef_Medicamento,
	dbo.RegExpReplace(dbo.Concatena(MA.Apresentacao, null), '[^0-9]', '')
From ARKMED_MedicamentoApresentacao MA
Group By ObjRef_Medicamento;
GO
