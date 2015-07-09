using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class ExpressaoRegularBuilder : Form
	{
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

		public ExpressaoRegularBuilder()
		{
			InitializeComponent();
			textBox1.Text = "Select * From Tabela Where Campo Is Not Null;";
			textBox2.Text = @"((\s)+(Select)(\s)+)";
			textBox3.Text = @"$2\cf1$3\cf0$4";
		}

		private void update(object sender, EventArgs e)
		{
			try
			{
				var rtf = Regex.Replace(textBox1.Text, textBox2.Text, textBox3.Text, RegexOptions.IgnoreCase);
				textBox6.Text = rtf;
				textBox4.Rtf = rtfHeader.Replace("{#Cores#}", tabelaCores).Replace("{#Texto#}", rtf);
				var mc = Regex.Matches(textBox1.Text, textBox2.Text, RegexOptions.IgnoreCase);
				textBox5.Text = "=" + String.Join("=\r\n=", mc.Cast<Match>().Select(m => m.Value).ToArray()) + "=";
			}
			catch (Exception ex)
			{
				textBox4.Text = textBox5.Text = ex.Message;
			}
		}
	}
}
