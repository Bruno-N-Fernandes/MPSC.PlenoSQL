using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSQL.Kernel.Infra;
using System;

namespace MPSC.PlenoSQL.TestesUnitarios.Infra
{
	[TestClass]
	public class TestandoFormatUtil
	{

		[TestMethod]
		public void QuandoExecutaToInLine()
		{
			var retorno = FormatUtil.ToInLine(entrada);
			Assert.AreEqual(modeloInLine, retorno);
		}


		[TestMethod]
		public void QuandoExecutaToInLineESoTemAspasSimples()
		{
			var retorno = FormatUtil.ToInLine("   'Bruno'   'Nogueira'    'Fernandes'   'abc ");
			Assert.AreEqual("'Bruno' 'Nogueira' 'Fernandes' 'abc", retorno);
		}

		[TestMethod]
		public void QuandoExecutaToInLineESoTemAspasDuplas()
		{
			var retorno = FormatUtil.ToInLine(@"   ""Bruno""   ""Nogueira""    ""Fernandes""   ""abc ");
			Assert.AreEqual(@"""Bruno"" ""Nogueira"" ""Fernandes"" ""abc", retorno);
		}

		[TestMethod]
		public void QuandoExecutaToInLineETemAspasSimplesEAspasDuplas()
		{
			var retorno = FormatUtil.ToInLine(@"   'Bruno'   ""Nogueira""    'Fernandes'   ""abc' ");
			Assert.AreEqual(@"'Bruno' ""Nogueira"" 'Fernandes' ""abc'", retorno);
		}

		[TestMethod]
		public void QuandoExecutaToInLineETemAspasSimplesDentroDeAspasDuplas()
		{
			var retorno = FormatUtil.ToInLine(@"   ""'Bruno'""   ""Nogueira's""    "" e 'Fernandes' e ""   ""abc' ");
			Assert.AreEqual(@"""'Bruno'"" ""Nogueira's"" "" e 'Fernandes' e "" ""abc'", retorno);
		}

		[TestMethod]
		public void QuandoExecutaToInLineETemAspasDuplasDentroDeAspasSimples()
		{
			var retorno = FormatUtil.ToInLine(@"   '""Bruno""'   'Nogueira""s'    ' e ""Fernandes"" e '   'abc"" ");
			Assert.AreEqual(@"'""Bruno""' 'Nogueira""s' ' e ""Fernandes"" e ' 'abc""", retorno);
		}

		private const String entrada = @"
Select
	T1.*
From Tabela1 T1 
Inner Join Tabela2 T2 On T2.T1Id = T1.Id
Left Join View V On V.Id = T1.Id	
Where (T1.Campo = @variavel) And   (	
T2.Campo2 = 'teste   '   )	
Order By 1;";


		private const String modeloInLine = "Select T1.* From Tabela1 T1 Inner Join Tabela2 T2 On T2.T1Id = T1.Id Left Join View V On V.Id = T1.Id Where (T1.Campo = @variavel) And (T2.Campo2 = 'teste   ') Order By 1;";

		private const String modeloFormatado = @"Select
	T1.*
From Tabela1 T1
Inner Join Tabela2 T2 On T2.T1Id = T1.Id
Left  Join View     V On V.Id = T1.Id
Where (T1.Campo = @variavel)
And ( T2.Campo2 = 'teste' )
Order By 1;";


	}
}