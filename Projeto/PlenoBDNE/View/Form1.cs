using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class Form1 : Form
	{

		public Form1()
		{
			InitializeComponent();
			textBox1.Text = "Select * From Tabela Where Campo Is Not Null;\r\nSelect * From Tabela Where Campo Is Not Null;";
			textBox2.Text = @"(^|\s|\n)(Select)(\s|\n|$)";
			textBox3.Text = @"$1--$2--$3";
		}

		private void update(object sender, EventArgs e)
		{
			try
			{
				textBox4.Text = Regex.Replace(textBox1.Text, textBox2.Text, textBox3.Text, RegexOptions.IgnoreCase);
				var mc = Regex.Matches(textBox1.Text, textBox2.Text, RegexOptions.IgnoreCase);
				textBox5.Text = "*" + String.Join("*\r\n*", mc.Cast<Match>().Select(m => m.Value).ToArray()) + "*";
			}
			catch (Exception ex)
			{
				textBox4.Text = textBox5.Text = ex.Message;
			}
		}
	}
}
