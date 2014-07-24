using System;
using System.IO;

namespace LBJC.NavegadorDeDados.Infra
{
	public static class Util
	{
		public static String[] FileToArray(String fullFileName)
		{
			return FileToString(fullFileName).Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}

		public static String FileToString(String fullFileName)
		{
			String retorno = "\n\n\n\n";
			if (File.Exists(fullFileName))
			{
				StreamReader streamReader = new StreamReader(fullFileName);
				retorno = streamReader.ReadToEnd() ?? "\r\n";
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
	}
}