using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class TesteUnitario
	{
		[TestMethod]
		public void TestIPCA()
		{
			var indiceIBGE = new IndiceIBGE();
			var competencia = indiceIBGE.Competencia;
			Assert.IsNotNull(competencia);


			var cotacaoIPCA1 = indiceIBGE.ObterPenultimaCotacaoIPCA();
			Assert.IsNotNull(cotacaoIPCA1);
			Console.WriteLine("IPCA {0}: {1:##0.00} %", competencia.AddMonths(-1).ToString("MMMM"), cotacaoIPCA1 * 100.0M);


			var cotacaoIPCA2 = indiceIBGE.ObterUltimaCotacaoIPCA();
			Assert.IsNotNull(cotacaoIPCA2);
			Console.WriteLine("IPCA {0}: {1:##0.00} %", competencia.ToString("MMMM"), cotacaoIPCA2 * 100.0M);


			var cotacaoINPC1 = indiceIBGE.ObterPenultimaCotacaoINPC();
			Assert.IsNotNull(cotacaoINPC1);
			Console.WriteLine("INPC {0}: {1:##0.00} %", competencia.AddMonths(-1).ToString("MMMM"), cotacaoINPC1 * 100.0M);


			var cotacaoINPC2 = indiceIBGE.ObterUltimaCotacaoINPC();
			Assert.IsNotNull(cotacaoINPC2);
			Console.WriteLine("INPC {0}: {1:##0.00} %", competencia.ToString("MMMM"), cotacaoINPC2 * 100.0M);
		}


		public void TesteSerializacao()
		{
			var myWriter = new StreamWriter("d:\\myFileName1.xml");

			var myObject0 = new Classe { ExibeLogo = true, Property1 = DateTime.Today, Property3 = 5 };
			var mySerializer0 = new XmlSerializer(myObject0.GetType());
			mySerializer0.Serialize(myWriter, myObject0);

			myWriter.WriteLine("\r\n\r\n");

			var myObject1 = new ClasseQueNaoSerializa3 { ExibeLogo = true, Property1 = DateTime.Today, Property3 = 4 };
			var mySerializer1 = new XmlSerializer(myObject1.GetType());
			mySerializer1.Serialize(myWriter, myObject1);

			myWriter.Close();
		}

		[Serializable]
		public class Classe
		{
			[XmlElement(Order = 4)]
			public virtual Boolean ExibeLogo { get; set; }

			[XmlElement(Order = 3)]
			public virtual Int64 Property3 { get; set; }

			[XmlElement(Order = 1)]
			public virtual DateTime Property1 { get; set; }

		}

		[Serializable, XmlRoot("Classe")]
		public class ClasseQueNaoSerializa3 : Classe
		{
			[XmlElement(Order = 3)]
			public override Int64 Property3 { get; set; }

			[XmlElement(Order = 2)]
			public virtual Int64 Property2 { get; set; }

			[XmlElement(Order = 5)]
			public virtual Int64 Property5 { get; set; }
		}
	}

	public class IndiceIBGE
	{
		private const String urlBase = "http://www.ibge.gov.br/home/estatistica/indicadores/precos/inpc_ipca/ipca-inpc_{0}_1.shtm";

		public readonly DateTime Competencia;
		private readonly String _html;

		public IndiceIBGE()
		{
			var hoje = DateTime.Today;
			Competencia = hoje.AddDays(1 - hoje.Day).AddMonths(-1);
			_html = ObterHtml(Competencia);

			Competencia = Competencia.AddMonths(-1);
			_html = ObterHtml(Competencia);

			_html = ExtrairTabela(_html);
		}

		public Decimal? ObterUltimaCotacaoIPCA()
		{
			return ExtrairIndice(_html, 1);
		}

		public Decimal? ObterPenultimaCotacaoIPCA()
		{
			return ExtrairIndice(_html, 2);
		}

		public Decimal? ObterUltimaCotacaoINPC()
		{
			return ExtrairIndice(_html, 3);
		}

		public Decimal? ObterPenultimaCotacaoINPC()
		{
			return ExtrairIndice(_html, 4);
		}

		private Decimal? ExtrairIndice(String tr, Int32 posicao)
		{
			Decimal? cotacao = null;
			if (!String.IsNullOrWhiteSpace(tr))
			{
				var loop = 0;
				var td = tr.ExtrairXml("td", true);
				while (!String.IsNullOrWhiteSpace(tr) && !String.IsNullOrWhiteSpace(td) && (++loop <= posicao))
				{
					tr = tr.Replace(td, String.Empty).Trim();
					td = tr.ExtrairXml("td", true);
				}

				var valor = td.ExtrairXml("div", false).ExtrairXml("b", false);
				cotacao = Convert.ToDecimal(valor) / 100.00M;
			}

			return cotacao;
		}

		private static String ExtrairTabela(String html)
		{
			if (!String.IsNullOrWhiteSpace(html))
			{
				var tbody = html.ExtrairXml("body", false).ExtrairXml("table", false).ExtrairXml("tbody", false).Trim();

				var tr = tbody.ExtrairXml("tr", true);
				while (!tr.Contains("Geral") && !String.IsNullOrWhiteSpace(tbody))
				{
					tbody = tbody.Replace(tr, String.Empty).Trim();
					tr = tbody.ExtrairXml("tr", true);
				}
				html = tr;
			}
			return html;
		}

		private static String ObterHtml(DateTime dataReferencia)
		{
			String html = null;
			try
			{
				var request = WebRequest.Create(String.Format(urlBase, dataReferencia.ToString("yyyyMM")));
				var response = request.GetResponse();
				var stream = response.GetResponseStream();
				var reader = new StreamReader(stream);
				html = reader.ReadToEnd();
				reader.Close();
				response.Close();
			}
			catch { }
			return html;
		}
	}

	public static class StringExtensions
	{
		public static String ExtrairXml(this String origem, String elemento, Boolean comElemento)
		{
			var atributos = origem.Extrair("<" + elemento, ">", true);
			return origem.Extrair(atributos, "</" + elemento + ">", comElemento);
		}

		public static String Extrair(this String origem, String inicio, String fim, Boolean comInicioEFim)
		{
			var indiceInicio = origem.IndexOf(inicio) + inicio.Length;
			var indiceFim = origem.IndexOf(fim, indiceInicio);

			if ((indiceInicio >= inicio.Length) && (indiceFim >= indiceInicio))
				origem = origem.Substring(indiceInicio, indiceFim - indiceInicio);

			return comInicioEFim ? inicio + origem + fim : origem;
		}
	}


}