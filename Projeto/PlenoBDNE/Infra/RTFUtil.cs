using System;
using System.Text.RegularExpressions;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public class RTFUtil
	{
		private static String[] palavrasReservadas = { "Select", "From", "Where", "And", "Or", "Not", "Inner", "Left", "Right", "Outter", "Join" };
		private static String[] literals = { "Null", "Is", "In", "On", "Like", "Union" };

		private const String regFormat = @"(^|\s|cf\d|\()({0})(\s|;|\)|$)";
		private const String bluFormat = @"$1\cf1$2$3\cf0";
		private const String redFormat = @"\cf2$0\cf0";
		private const String marFormat = @"$1\cf3$2$3\cf0";
		private const String greFormat = @"\cf4$0\cf0";
		private const String rtfHeader = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1046{\fonttbl{\f0\fnil\fcharset0 Courier New;}}
{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red50\green160\blue200;\red0\green160\blue0;}
\viewkind4\uc1\pard\f0\fs23";

		public static String Colorir(String texto)
		{
			return rtfHeader + Trocar(texto);
		}

		private static String Trocar(String source)
		{
			foreach (String key in palavrasReservadas)
				source = Trocar(source, key, bluFormat);

			foreach (String key in literals)
				source = Trocar(source, key, marFormat);

			source = Regex.Replace(source, "((\"[^\"]*\")|('[^']*'))", redFormat);
			source = Regex.Replace(source, "(/\\*[^\\*/]*\\*/)", greFormat);

			return source.Replace("\n", @"\line ");
		}

		private static String Trocar(String source, String key, String format)
		{
			return Regex.Replace(source, String.Format(regFormat, key), format, RegexOptions.IgnoreCase);
		}
	}
}