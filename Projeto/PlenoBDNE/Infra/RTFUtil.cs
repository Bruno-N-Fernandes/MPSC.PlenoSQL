using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public static class RTFUtil
	{
		private enum Cor
		{
			Preto = 0,
			Azul = 1,
			Vermelho = 2,
			Aqua = 3,
			Verde = 4
		}

		private const String tabelaCores = @"{\colortbl{
\red000\green000\blue000;
\red000\green000\blue255;
\red255\green000\blue000;
\red050\green160\blue200;
\red000\green160\blue000;
}}";

		private const String rtfHeader = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1046{\fonttbl{\f0\fnil\fcharset0 Courier New;}}
{#Cores#}
\viewkind4\uc1\pard\f0\fs23 {#Texto#}\par
}";
		private static String[] palavrasReservadas = { "Select", "From", "Where", "And", "Or", "Not", "Inner", "Left", "Right", "Outter", "Join" };
		private static String[] literals = { "Null", "Is", "In", "On", "Like", "Union" };

		public static void Colorir(this RichTextBox richTextBox)
		{
			var selStart = richTextBox.SelectionStart;
			richTextBox.Rtf = Colorir(richTextBox.Text);
			richTextBox.SelectionStart = selStart;
		}

		private static String Colorir(String texto)
		{
			texto = Trocar(texto);
			return rtfHeader.Replace("{#Cores#}", tabelaCores).Replace("{#Texto#}", texto.Replace("\n", @"\line "));
		}

		private static String Trocar(String source)
		{
			source = TrocarKeyWords(source);
			source = TrocarKeyStringsEComantarios(source);
			//source = TrocarMultiCores(source);
			source = TrocarPosicaoDoEspaco(source);
			return source.Replace("\r\n", @"\line ").Replace("\r", @"\line ");
		}

		private static String TrocarKeyWords(String source)
		{
			foreach (String key in palavrasReservadas)
				source = Replace(source, key, Cor.Azul);

			foreach (String key in literals)
				source = Replace(source, key, Cor.Aqua);

			return source;
		}

		private static String TrocarKeyStringsEComantarios(String source)
		{
			const String replFormat = @"\cf{#Cor#}$0\cf0";
			source = Regex.Replace(source, @"((""[^""]*\"")|('[^']*'))", replFormat.Colorir(Cor.Vermelho));
			source = Regex.Replace(source, @"(/\*[^\*/]*\*/)", replFormat.Colorir(Cor.Verde));
			return source;
		}

		private static String TrocarMultiCores(String source)
		{
			return Regex.Replace(source, @"(\\cf\d([ ]*))(\\cf\d([ ]*))+", "$3");
		}

		private static String TrocarPosicaoDoEspaco(String source)
		{
			source = Regex.Replace(source, @"(\\cf\d)(\s|\t|\n|/|;|\)|$)+", "$2$1");
			return source;
		}

		private static String Replace(String source, String key, Cor cor)
		{
			const String findFormat = @"(^|\s|cf\d|\(|/)({#Key#})(\s|\t|\n|\\cf\d|/|;|\)|$)";
			const String replFormat = @"$1\cf{#Cor#}$2$3\cf0";
			return Replace(source, findFormat.Replace("{#Key#}", key), replFormat.Colorir(cor));
		}

		private static String Replace(String source, String regExpIn, String regExpOut)
		{
			return Regex.Replace(source, regExpIn, regExpOut, RegexOptions.IgnoreCase);
		}

		private static String Colorir(this String pattern, Cor cor)
		{
			return pattern.Replace("{#Cor#}", cor.ToString("d"));
		}
	}
}