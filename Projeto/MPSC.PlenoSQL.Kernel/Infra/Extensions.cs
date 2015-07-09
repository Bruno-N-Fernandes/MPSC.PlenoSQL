using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class Extensions
	{
		public const char CR = '\r';
		public const char LF = '\n';
		public const char TB = '\t';
		public const char PL = '\'';
		public const char SPC = ' ';
		public static readonly String BREAK = new String(new char[] { CR, LF, TB, PL, SPC, '"', '(', ')', '[', ']', '{', '}', '/', '*', '+', '-', ',', ';', '<', '>', '!', '=' });
		public static readonly char[] ENTER = { CR, LF };

		public static String Concatenar<T>(this IEnumerable<T> source, String join)
		{
			return String.Join<T>(join, source);
		}

		public static String SubstituirConstantesPelosSeusValores(this FastColoredTextBox textBox, String selectedQuery, IEnumerable<Constante> constantes)
		{
			String tempQuery = textBox.Text.AllTrim();
			Int32 cursorPosition = textBox.SelectionStart;
			try
			{
				if (tempQuery.Equals(selectedQuery))
				{
					selectedQuery = ";" + selectedQuery + ";";
					selectedQuery = selectedQuery.Substring(0, selectedQuery.IndexOf(";", cursorPosition + 1));
					selectedQuery = selectedQuery.Substring(selectedQuery.LastIndexOf(";") + 1);
				}

				if (!String.IsNullOrWhiteSpace(selectedQuery))
				{
					foreach (var constante in constantes)
					{
						if (!String.IsNullOrWhiteSpace(constante.Nome))
							selectedQuery = selectedQuery.Replace(constante.Nome, constante.Valor);
					}
				}
			}
			catch (Exception) { }

			return selectedQuery.AllTrim().Replace(";", String.Empty).AllTrim();
		}

		public static String AllTrim(this String str)
		{
			while (str.StartsWith("\r") || str.StartsWith("\n") || str.StartsWith("\t") || str.StartsWith(" "))
				str = str.Substring(1).Trim();

			while (str.EndsWith("\r") || str.EndsWith("\n") || str.EndsWith("\t") || str.EndsWith(" "))
				str = str.Substring(0, str.Length - 1).Trim();

			return str;
		}


		public static Object Get(this DataRow dataRow, Enum enumerado)
		{
			return dataRow[enumerado.ToInt()];
		}

		public static Int32 ToInt(this Enum enumerado)
		{
			return enumerado.GetHashCode();
		}

		public static String Get(this String[] args, String parametro, String padrao)
		{
			var par = (args == null) ? null : args.FirstOrDefault(a => a.ToUpper().StartsWith(parametro.ToUpper()));
			return String.IsNullOrWhiteSpace(par) || (par.Length <= parametro.Length) ? padrao : par.Replace("\"", "").Substring(parametro.Length);
		}
	}
}