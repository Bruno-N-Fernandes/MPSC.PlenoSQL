using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class FormatUtil
	{
		public static readonly String[] globalKeyWord = { "Select", "From", "Inner Join", "Left  Join", "Right Join", "Full Join", "Where", "And", "Or", "Group By", "Order By" };
		public static String Embelezar(String sqlCode)
		{
			var retorno = ToInLine(sqlCode);
			var mapa = Mapear(retorno);
			retorno = Encode(retorno, mapa);

			retorno = EmbelezarPalavas(retorno);
			retorno = QuebrarLinhas(retorno);
			retorno = AlinharOnJoins(retorno);

			retorno = Decode(retorno, mapa);
			return retorno;
		}

		private static String AlinharOnJoins(String texto)
		{
			var retorno = texto;
			var allLines = texto.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			var joinLines = allLines.Where(l => l.Contains(" Join ")).ToArray();

			var maiorPosicao = 0;
			foreach (var joinLine in joinLines)
			{
				var posicao = joinLine.IndexOf(" On ");
				if (posicao > maiorPosicao)
					maiorPosicao = posicao;
			}

			for (int l = 0; l < allLines.Length; l++)
			{
				var linha = allLines[l];
				if (joinLines.Contains(linha))
				{
					var posicao = linha.IndexOf(" On ");
					if (posicao < maiorPosicao)
					{
						var spaces = new String(' ', maiorPosicao - posicao);
						posicao = linha.Substring(0, posicao).LastIndexOf(" ");
						allLines[l] = linha.Insert(posicao, spaces);
					}
				}
			}

			return joinLines.Any() ? String.Join("\r\n", allLines) : retorno;
		}

		private static String EmbelezarPalavas(String texto)
		{
			var retorno = texto;
			foreach (var token in globalKeyWord)
			{
				var regex = token.Replace("  ", " +");
				retorno = Regex.Replace(retorno, regex, token, RegexOptions.IgnoreCase);
			}
			return retorno;
		}


		public static String QuebrarLinhas(String texto)
		{
			var retorno = texto;
			retorno = QuebrarLinhasAntesDe(texto, globalKeyWord);
			retorno = retorno.Replace(",", ",\r\n\t");
			retorno = retorno.Replace("Select ", "Select\r\n\t");
			return retorno;
		}

		private static String QuebrarLinhasAntesDe(String texto, params String[] tokens)
		{
			var retorno = texto;
			foreach (var token in tokens)
				retorno = retorno.Replace(" " + token + " ", "\r\n" + token + " ");
			return retorno;
		}


		public static String ToInLine(String texto)
		{
			var retorno = texto.Trim();
			var mapa = Mapear(retorno);
			retorno = Encode(retorno, mapa);

			retorno = retorno.Replace("\r\n", " ");
			retorno = retorno.Replace("\r", " ");
			retorno = retorno.Replace("\n", " ");
			retorno = retorno.Replace("\t", " ");
			retorno = Regex.Replace(retorno, "  *", " ");
			retorno = retorno.Replace("( ", "(");
			retorno = retorno.Replace(" )", ")");

			retorno = Decode(retorno, mapa);
			return retorno.Trim();
		}

		private static Dictionary<String, String> Mapear(String texto)
		{
			var mapa = new Dictionary<String, String>();

			var matches = Regex.Matches(texto, "('[^']*')|(\"[^\"]*\")");
			foreach (Match match in matches)
			{
				var token = String.Format(@"#{{[({0})]}}#", mapa.Count);
				var value = match.Captures[0].Value;
				mapa[token] = value;
				texto = texto.Replace(value, token);
			}

			return mapa;
		}

		private static String Encode(String retorno, Dictionary<String, String> mapa)
		{
			foreach (var item in mapa) retorno = retorno.Replace(item.Value, item.Key);
			return retorno;
		}

		private static String Decode(String retorno, Dictionary<String, String> mapa)
		{
			foreach (var item in mapa) retorno = retorno.Replace(item.Key, item.Value);
			return retorno;
		}
	}
}