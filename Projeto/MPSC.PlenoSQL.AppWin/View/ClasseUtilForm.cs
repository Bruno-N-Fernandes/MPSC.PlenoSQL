using System;
using System.Linq;
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
				var isPublic = ckbGerarFieldsPublicos.Checked;
				textBox2.Text = String.Empty;
				var fields = String.Empty;
				var props = String.Empty;
				var parms = String.Empty;
				var atrib = String.Empty;
				var classe = textBox3.Text;
				var source = textBox1.Text;
				var linhas = source.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (linhas.Length >= 0)
				{
					var linha = linhas.FirstOrDefault(l => l.Contains("class "));
					if (linha != null)
					{
						source = source.Replace(linha, String.Empty);
						classe = linha.Substring(linha.IndexOf("class ") + 5).Trim() + " ";
						classe = classe.Substring(0, classe.IndexOfAny(": ".ToCharArray()));
					}
				}

				var matches = regex.Matches(source);
				foreach (Match match in matches)
				{
					var tipo = match.Groups[3].Value.Trim();
					var a = match.Groups[4].Value.Replace(";", String.Empty).Trim();
					var f = LowerFirst(a);
					var p = UpperFirst(a);

					if (isPublic)
					{
						fields += "\tpublic readonly " + tipo + " " + p + ";\r\n";
						atrib += "\t\t" + p + " = " + f + ";\r\n";
					}
					else
					{
						fields += "\tprivate readonly " + tipo + " " + f + ";\r\n";
						props += "\tpublic " + tipo + " " + p + " { get { return this." + f + "; } }\r\n";
						atrib += "\t\tthis." + f + " = " + f + ";\r\n";
					}

					parms += ", " + tipo + " " + f;
				}

				textBox2.Text += "\r\npublic class " + classe + "\r\n{\r\n"
					+ fields + "\r\n"
					+ props + "\r\n"
					+ String.Format("\tpublic " + classe + "({0})\r\n", (parms + " ").Substring(1).Trim())
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
					(sender as TextBox).SelectAll();
				else if (e.KeyCode == Keys.C)
					Clipboard.SetText((sender as TextBox).SelectedText);
			}
		}
	}
}