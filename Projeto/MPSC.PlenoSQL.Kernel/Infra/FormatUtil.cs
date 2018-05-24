using MPSC.PlenoSQL.Kernel.Dados.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class FormatUtil
	{
		public static readonly String[] globalBreak = { "Select", "From", "Inner Join", "Left  Join", "Right Join", "Full Join", "Where", "And", "Or", "Group By", "Order By" };
		public static String Embelezar(String sqlCode, Boolean manterCRLFDasPontas)
		{
			var retorno = ToInLine(sqlCode);
			var mapa = Mapear(retorno);
			retorno = Encode(retorno, mapa);

			retorno = EmbelezarPalavas(retorno);
			retorno = QuebrarLinhas(retorno);
			retorno = AlinharOnJoins(retorno);
			retorno = IdentarSubQuery(retorno);

			retorno = Decode(retorno, mapa);
			return VerificarCRLF(retorno, sqlCode, manterCRLFDasPontas);
		}

		public static String IdentarSubQuery(String retorno)
		{
			var matches = Regex.Matches(retorno, @"\((.|\r|\n)+?\)+");
			foreach (Match match in matches)
			{
				var subSelect = match.Groups[0].Value;
				if (subSelect.StartsWith("(") && subSelect.Contains("Select") && subSelect.Contains("From") && subSelect.EndsWith(")"))
				{
					subSelect = subSelect.Substring(1, subSelect.Length - 2);
					var subFormatado = Embelezar(subSelect, false);
					var allLines = subFormatado.Split(new[] { "\r\n" }, StringSplitOptions.None);
					var formatado = String.Join("\r\n", allLines.Select(l => "\t\t" + l));
					retorno = retorno.Replace(subSelect, "\r\n" + formatado + "\r\n\t");
				}
			}
			return retorno;
		}

		private static String VerificarCRLF(String retorno, String sqlCode, Boolean manterCRLF)
		{
			if (manterCRLF && !String.IsNullOrWhiteSpace(sqlCode))
			{
				retorno = retorno.Trim();
				if (!String.IsNullOrWhiteSpace(retorno))
				{
					sqlCode = sqlCode.ToUpper();
					var r = retorno.ToUpper();

					var posicao1 = sqlCode.IndexOf(r[0]);
					if (posicao1 >= 0)
					{
						var qtdCRLF1 = sqlCode.Substring(0, posicao1).Count(c => c == '\n');
						retorno = Replicar("\r\n", qtdCRLF1) + retorno;
					}

					var posicao2 = sqlCode.LastIndexOf(r[r.Length - 1]);
					if (posicao2 >= 0)
					{
						var qtdCRLF2 = sqlCode.Substring(posicao2).Count(c => c == '\n');
						retorno = retorno + Replicar("\r\n", qtdCRLF2);
					}
				}
			}
			return retorno;
		}

		private static String Replicar(String s, Int32 qtd)
		{
			return (qtd == 1) ? s : ((qtd <= 0) ? String.Empty : (s + Replicar(s, qtd - 1)));
		}

		private static String AlinharOnJoins(String texto)
		{
			var retorno = texto;
			var allLines = texto.Split(new[] { "\r\n" }, StringSplitOptions.None);
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
					if ((posicao > 0) && (posicao < maiorPosicao))
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
			String[] globalKeyWord = { "Select ", " From ", " Inner Join ", " Left  Join ", " Right Join ", " Full Join ", " Where ", " And ", " Or ", " Group By ", " Order By ", " On ", " As " };
			var retorno = Cache.Traduzir(texto);
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
			retorno = QuebrarLinhasAntesDe(texto, globalBreak);
			retorno = retorno.Replace("--", "\r\n-- ");
			retorno = retorno.Replace(",", ",\r\n\t");
			retorno = retorno.Replace(",\r\n\t ", ",\r\n\t");
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
				var token = String.Format(@"#{{[<{0}>]}}#", mapa.Count);
				var value = match.Groups[0].Value;
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

		public static Int32 ObterPosicaoInicial(String texto, Func<String, Int32> func)
		{
			var posicao = func(RemoverTextoEntreAspas(texto));
			return (posicao >= 0) ? posicao + 1 : 0;
		}

		public static Int32 ObterPosicaoFinal(String texto, Func<String, Int32> func)
		{
			var posicao = func(RemoverTextoEntreAspas(texto));
			return (posicao < 0) ? texto.Length : posicao;
		}

		public static String RemoverTextoEntreAspas(String texto)
		{
			var matches = Regex.Matches(texto, "('[^']*')|(\"[^\"]*\")");
			foreach (Match match in matches)
			{
				var value = match.Groups[0].Value;
				texto = texto.Replace(value, new String('#', value.Length));
			}
			return texto;
		}
	}
}