using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LBJC.NavegadorDeDados.Infra;
using LBJC.NavegadorDeDados.Dados;

namespace LBJC.NavegadorDeDados
{
	public static class Extensions
	{
		public const String TokenKeys = "\r\n\t(){}[] ";

		public static String[] GetFilesToOpen(params String[] extensoes)
		{
			String[] retorno = new String[] { };
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = extensoes.Concatenar("|");
			openFileDialog.Multiselect = true;
			if (DialogResult.OK == openFileDialog.ShowDialog())
				retorno = openFileDialog.FileNames;
			openFileDialog.Dispose();
			openFileDialog = null;
			return retorno;
		}

		public static String GetFileToSave(params String[] extensoes)
		{
			String retorno = null;
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = extensoes.Concatenar("|");
			if (DialogResult.OK == saveFileDialog.ShowDialog())
				retorno = saveFileDialog.FileName;
			saveFileDialog.Dispose();
			saveFileDialog = null;
			return retorno;
		}
		public static String Concatenar<T>(this IEnumerable<T> source, String join)
		{
			return String.Join<T>(join, source);
		}

		public static String ObterApelidoAntesDoPonto(String query, Int32 selectionStart)
		{
			query = query.ToUpper().Insert(selectionStart, ".");

			Int32 i = selectionStart;
			while (!TokenKeys.Contains(query[--i - 1])) ;

			return query.Substring(i, selectionStart - i);
		}

		public static String ObterNomeTabelaPorApelido(String query, Int32 selectionStart, String apelido)
		{
			String nomeDaTabela = String.Empty;
			query = query.ToUpper().Insert(selectionStart, ".");
			var tokens = query.Split(Extensions.TokenKeys.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

			var index = tokens.LastIndexOf(apelido.Replace(".", ""));
			if (tokens.Count > 1)
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

		public static String ConverterParametrosEmConstantes(String tempQuery, String selectedQuery)
		{
			tempQuery += "/**/";
			var comentarios = tempQuery.Substring(tempQuery.IndexOf("/*") + 2);
			comentarios = comentarios.Substring(0, comentarios.IndexOf("*/"));
			var variaveis = comentarios.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (String variavel in variaveis)
			{
				var param = variavel.Substring(0, variavel.IndexOf("=") + 1).Replace("=", "").Trim();
				var valor = variavel.Substring(variavel.IndexOf("=") + 1).Trim().Replace(";", "");
				selectedQuery = selectedQuery.Replace(param, valor);
			}
			return selectedQuery;
		}

		public static Point CurrentCharacterPosition(this TextBox textBox)
		{
			int s = textBox.SelectionStart;
			int y = textBox.GetLineFromCharIndex(s) ;
			int x = s - textBox.GetFirstCharIndexFromLine(y);

			return new Point(x * 9, (y + 1) * textBox.Font.Height);
		}

		public static String ObterPrefixo(TextBox textBox)
		{
			Int32 selectionStart = textBox.SelectionStart;
			String query = textBox.Text.Substring(0, selectionStart).ToUpper();
	
			Int32 i = selectionStart+1;
			while (!TokenKeys.Contains(query[--i - 1])) ;

			var tamanho = selectionStart - i;
			textBox.SelectionLength = tamanho;
			textBox.SelectionStart = i;
			return query.Substring(i, tamanho).Trim();
		}
	}
}