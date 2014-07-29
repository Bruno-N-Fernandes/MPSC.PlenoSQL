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
		private static String[] keyWords = { "Select", "From", "Where", "Inner", "Left", "Right", "Outter", "Join", "Order", "Group", "Between", "Case", "When", "Having" };
		private static String[] literals = { "Null", "Is", "In", "On", "And", "Or", "Not", "Like", "Union", "By", "Asc", "Desc", "=", "<", ">", "<=", "=>", "<>", "!=", "Then", "End" };

		public static void Colorir(this RichTextBox richTextBox, Boolean convertToUpper)
		{
			var selStart = richTextBox.SelectionStart;
			richTextBox.Rtf = Colorir(convertToUpper ? richTextBox.Text.ToUpper() : richTextBox.Text);
			richTextBox.SelectionStart = selStart;
		}

		private static String Colorir(String source)
		{
			source = ColorirKeyWords1(source);
			source = ColorirKeyWords2(source);
			source = ColorirStringsEComantarios(source);
			source = TratarPosicaoDoEspaco(source);
			//source = RemoverMultiCores(source);
			source = TratarQuebraDeLinha(source);
			return AplicarTemplateRTF(source);
		}

		private static String ColorirKeyWords1(String source)
		{
			foreach (String key in keyWords)
				source = Replace(source, key, Cor.Azul);
			return source;
		}

		private static String ColorirKeyWords2(String source)
		{
			foreach (String key in literals)
				source = Replace(source, key, Cor.Aqua);
			return source;
		}

		private static String ColorirStringsEComantarios(String source)
		{
			const String replFormat = @"\cf{#Cor#}$0\cf0";
			source = Regex.Replace(source, @"(""[^""]*"")", replFormat.Colorir(Cor.Vermelho));
			source = Regex.Replace(source, @"('[^']*')", replFormat.Colorir(Cor.Vermelho));
			source = Regex.Replace(source, @"(/\*[^\*/]*\*/)", replFormat.Colorir(Cor.Verde));
			return source;
		}

		private static String RemoverMultiCores(String source)
		{
			return Regex.Replace(source, @"(\\cf\d(\s*))(\\cf\d(\s*))+", "$3");
		}

		private static String TratarPosicaoDoEspaco(String source)
		{
			source = Regex.Replace(source, @"(\\cf.)(\s+)", "$2$1");
			return source;
		}

		private static String TratarQuebraDeLinha(String source)
		{
			source = source.Replace("\r\n", @"\line ");
			source = source.Replace("\r", @"\line ");
			source = source.Replace("\n", @"\line ");
			return source;
		}

		private static String AplicarTemplateRTF(String source)
		{
			return rtfHeader.Replace("{#Cores#}", tabelaCores).Replace("{#Texto#}", source);
		}

		private static String Replace(String source, String key, Cor cor)
		{
			const String findFormat = @"(^|\s)({#Key#})(\s+|$)";
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