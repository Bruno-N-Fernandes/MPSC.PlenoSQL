using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class ClasseUtilForm : Form
	{
		private Regex regex = new Regex("(.*[^ ] )?((.*) (.*))");
		public ClasseUtilForm()
		{
			InitializeComponent();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			try
			{
				textBox2.Text = String.Empty;
				var fields = String.Empty;
				var props = String.Empty;
				var parms = String.Empty;
				var atrib = String.Empty;

				var matches = regex.Matches(textBox1.Text);
				foreach (Match match in matches)
				{
					//foreach (Group g in match.Groups.Cast<Group>().Skip(1)) textBox2.Text += g.Value + ";\r\n";

					var tipo = match.Groups[3].Value.Trim();
					var a = match.Groups[4].Value.Replace(";", String.Empty).Trim();
					var f = LowerFirst(a);
					var p = UpperFirst(a);

					fields += "\tprivate readonly " + tipo + " " + f + ";\r\n";
					props += "\tpublic " + tipo + " " + p + " { get { return this." + f + "; } }\r\n";
					parms += ", " + tipo + " " + f;
					atrib += "\t\tthis." + f + " = " + f + ";\r\n";
				}
				textBox2.Text += "\r\npublic class " + textBox3.Text + "\r\n{\r\n"
					+ fields + "\r\n"
					+ props + "\r\n"
					+ String.Format("\tpublic " + textBox3.Text + "({0})\r\n", (parms + " ").Substring(1).Trim())
					+ "\t{\r\n" + atrib + "\t}\r\n}";

			}
			catch { }
		}

		private string UpperFirst(string texto)
		{
			return (texto.Length > 0) ? texto.Substring(0, 1).ToUpper() + texto.Substring(1) : texto;
		}
		private string LowerFirst(string texto)
		{
			return (texto.Length > 0) ? texto.Substring(0, 1).ToLower() + texto.Substring(1) : texto;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (ckbIsProperty.Checked)
				regex = new Regex("(.*[^ ] )?((.*) (.*))( +\\{.*\\})");
			else
				regex = new Regex("(.*[^ ] )?((.*) (.*))");
			textBox1_TextChanged(this, e);
		}

		private void textBox2_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control)
			{
				if (e.KeyCode == Keys.A)
					textBox2.SelectAll();
				else if (e.KeyCode == Keys.C)
					Clipboard.SetText(textBox2.Text);
			}
		}
	}
}