using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class FormatUtil
	{
		public static String Embelezar(String sqlCode)
		{
			var sql = ToInLine(sqlCode);

			return sql;
		}

		public static String ToInLine(String texto)
		{
			var mapa = new Dictionary<String, String>();
			var retorno = Mapear(mapa, texto);

			retorno = retorno.Replace("\r\n", " ");
			retorno = retorno.Replace("\r", " ");
			retorno = retorno.Replace("\n", " ");
			retorno = retorno.Replace("\t", " ");
			retorno = retorno.Trim();
			retorno = Regex.Replace(retorno, "  *", " ");
			retorno = retorno.Replace("( ", "(");
			retorno = retorno.Replace(" )", ")");

			foreach (var item in mapa)
				retorno = retorno.Replace(item.Key, item.Value);

			return retorno;
		}

		private static String Mapear(Dictionary<String, String> mapa, String texto)
		{
			var regex = "('[^']*')|(\"[^\"]*\")";

			var matches = Regex.Matches(texto, regex);
			foreach (Match match in matches)
			{
				var token = String.Format(@"#{{[({0})]}}#", mapa.Count);
				var value = match.Captures[0].Value;
				mapa[token] = value;
				texto = texto.Replace(value, token);
			}

			return texto;
		}
	}
}