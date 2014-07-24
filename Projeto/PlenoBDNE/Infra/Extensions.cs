using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public static class Extensions
	{
		public static String Concatenar<T>(this IEnumerable<T> source, String join)
		{
			return String.Join<T>(join, source);
		}

		public static Point CurrentCharacterPosition(this TextBox textBox)
		{
			int s = textBox.SelectionStart;
			int y = textBox.GetLineFromCharIndex(s);
			int x = s - textBox.GetFirstCharIndexFromLine(y);

			return new Point(x * 9, (y + 1) * textBox.Font.Height);
		}

		public static String ObterPrefixo(this TextBox textBox)
		{
			Int32 selectionStart = textBox.SelectionStart;
			String query = textBox.Text.Substring(0, selectionStart).ToUpper();

			Int32 i = selectionStart + 1;
			while (!Util.TokenKeys.Contains(query[--i - 1])) ;

			var tamanho = selectionStart - i;
			textBox.SelectionLength = tamanho;
			textBox.SelectionStart = i;
			return query.Substring(i, tamanho).Trim();
		}




		public static IDbCommand CriarComando(this IDbConnection iDbConnection, String query)
		{
			if (iDbConnection.State != ConnectionState.Open)
				iDbConnection.Open();
			IDbCommand iDbCommand = iDbConnection.CreateCommand();
			iDbCommand.CommandText = query;
			iDbCommand.CommandType = CommandType.Text;
			iDbCommand.CommandTimeout = 60;
			return iDbCommand;
		}

		public static Boolean IsOpen(this IDataReader iDataReader)
		{
			return (iDataReader != null) && !iDataReader.IsClosed;
		}
	}
}