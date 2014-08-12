using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public static class Util
	{
		public const String TokenKeys = "\r\n\t(){}[] ";

		public static String[] FileToArray(String fullFileName, Int32 fields)
		{
			var retorno = FileToString(fullFileName).Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			if (retorno.Length < fields)
				retorno = retorno.Union(String.Empty.PadLeft(fields - retorno.Length, '\n').Split('\n')).ToArray();
			return retorno;
		}

		public static String FileToString(String fullFileName)
		{
			String retorno = String.Empty;
			if (File.Exists(fullFileName))
			{
				StreamReader streamReader = new StreamReader(fullFileName);
				retorno = streamReader.ReadToEnd() ?? String.Empty;
				streamReader.Close();
				streamReader.Dispose();
			}
			return retorno;
		}

		public static Boolean ArrayToFile(String fullFileName, params String[] conteudo)
		{
			return StringToFile(conteudo.Concatenar("\r\n"), fullFileName);
		}

		public static Boolean StringToFile(String conteudo, String fullFileName)
		{
			return StringToFile(conteudo, fullFileName, false);
		}

		public static Boolean StringToFile(String conteudo, String fullFileName, Boolean append)
		{
			Boolean retorno = false;
			try
			{
				StreamWriter streamWriter = new StreamWriter(fullFileName, append);
				streamWriter.Write(conteudo);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
				retorno = true;
			}
			catch (Exception) { retorno = false; }

			return retorno;
		}
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
			var tokens = query.Split(Util.TokenKeys.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

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

		public static String ConverterParametrosEmConstantes(String tempQuery, String selectedQuery, Int32 cursorPosition)
		{
			try
			{
				if (tempQuery.Equals(selectedQuery))
				{
					selectedQuery = ";" + selectedQuery + ";";
					selectedQuery = selectedQuery.Substring(0, selectedQuery.IndexOf(";", cursorPosition));
					selectedQuery = selectedQuery.Substring(selectedQuery.LastIndexOf(";") + 1);
				}
				
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
			}
			catch (Exception) { }

			return selectedQuery;
		}
	}
}