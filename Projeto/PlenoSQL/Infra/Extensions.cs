using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastColoredTextBoxNS;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public static class Extensions
	{
		public const String TokenKeys = "\r\n\t(){}[] ";

		public static String Concatenar<T>(this IEnumerable<T> source, String join)
		{
			return String.Join<T>(join, source);
		}

		public static String ObterPrefixo(this FastColoredTextBox textBox)
		{
			Int32 selectionStart = textBox.SelectionStart;
			String query = textBox.Text.Substring(0, selectionStart).ToUpper();
			if (query.Length > 0)
			{
				Int32 i = selectionStart + 1;
				while (!TokenKeys.Contains(query[--i - 1])) ;

				var tamanho = selectionStart - i;
				textBox.SelectionStart = i;
				textBox.SelectionLength = tamanho;
				query = query.Substring(i, tamanho).Trim();
			}
			return query;
		}

		public static String ObterApelidoAntesDoPonto(this FastColoredTextBox textBox)
		{
			String query = textBox.Text;
			Int32 selectionStart = textBox.SelectionStart;

			query = query.ToUpper().Insert(selectionStart, ".");

			Int32 i = selectionStart;
			while (!TokenKeys.Contains(query[--i - 1])) ;

			return query.Substring(i, selectionStart - i);
		}

		public static String ObterNomeTabelaPorApelido(this FastColoredTextBox textBox, String apelido)
		{
			String query = textBox.Text;
			Int32 selectionStart = textBox.SelectionStart;

			String nomeDaTabela = String.Empty;
			query = query.ToUpper().Insert(selectionStart, ".");
			var tokens = query.Split(TokenKeys.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

			var index = tokens.LastIndexOf(apelido.ToUpper().Replace(".", ""));
			if (index > 1)
			{
				if (tokens[index - 1].Equals("AS"))
					nomeDaTabela = tokens[index - 2];
				else if (tokens[index - 1].Equals("FROM") || tokens[index - 1].Equals("JOIN"))
					nomeDaTabela = tokens[index];
				else
					nomeDaTabela = tokens[index - 1];
			}

			return nomeDaTabela;
		}

		public static String ConverterParametrosEmConstantes(this FastColoredTextBox textBox, String selectedQuery)
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
					tempQuery += "/**/";
					var comentarios = tempQuery.Substring(tempQuery.IndexOf("/*") + 2);
					comentarios = comentarios.Substring(0, comentarios.IndexOf("*/"));
					var variaveis = comentarios.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					foreach (String variavel in variaveis)
					{
						var param = variavel.Substring(0, variavel.IndexOf("=") + 1).Replace("=", "").Trim();
						var valor = variavel.Substring(variavel.IndexOf("=") + 1).Trim().Replace(";", "");
						if (!String.IsNullOrWhiteSpace(param))
							selectedQuery = selectedQuery.Replace(param, valor);
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