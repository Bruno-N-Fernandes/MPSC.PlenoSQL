using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoBDNE.AppWin.Infra
{
	public static class FileUtil
	{
		public static String[] FileToArray(String fullFileName, Int32 fields)
		{
			var retorno = FileToString(fullFileName).Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
			if (retorno.Length < fields)
				retorno = retorno.Concat(String.Empty.PadLeft(fields, '\n').Split('\n')).ToArray();
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
	}
}