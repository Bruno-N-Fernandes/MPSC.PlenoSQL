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
			var data = DateTime.Today.AddMonths(-1);
			var html = ObterHtml(data) ?? ObterHtml(data.AddMonths(-1));
			var cotacao = ExtrairIndice(html);
			Assert.IsNotNull(cotacao);

			Console.WriteLine("{0} %", cotacao * 100.0M);
		}

		private String ObterHtml(DateTime dataReferencia)
		{
			const String urlBase = "http://www.ibge.gov.br/home/estatistica/indicadores/precos/inpc_ipca/ipca-inpc_{0}_1.shtm";
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

		private Decimal? ExtrairIndice(String html)
		{
			Decimal? cotacao = null;
			if (!String.IsNullOrWhiteSpace(html))
			{
				var body = html.ExtrairXml("body");
				var table = body.ExtrairXml("table");
				var tbody = table.ExtrairXml("tbody").Trim();

				var tr = tbody.ExtrairXml("tr", true);
				while (!tr.Contains("Geral") && !String.IsNullOrWhiteSpace(tbody))
				{
					tbody = tbody.Replace(tr, String.Empty).Trim();
					tr = tbody.ExtrairXml("tr", true);
				}

				var td = tr.ExtrairXml("td", true);
				tr = tr.Replace(td, String.Empty).Trim();
				td = tr.ExtrairXml("td");
				var div = td.ExtrairXml("div");
				var valor = div.ExtrairXml("b");
				cotacao = Convert.ToDecimal(valor) / 100.00M;
			}

			return cotacao;
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
	}

	public static class StringExtensions
	{
		public static String ExtrairXml(this String origem, String elemento, Boolean comElemento = false)
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