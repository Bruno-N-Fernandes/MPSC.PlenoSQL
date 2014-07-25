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
		}

		private void update(object sender, EventArgs e)
		{
			textBox4.Text = Regex.Replace(textBox1.Text, textBox2.Text, textBox3.Text, RegexOptions.IgnoreCase);
			var mc = Regex.Matches(textBox1.Text, textBox2.Text, RegexOptions.IgnoreCase);
			textBox5.Text =String.Concat(mc.Cast<Match>().Select(m => m.Value), "\n");
		}
	}
}
