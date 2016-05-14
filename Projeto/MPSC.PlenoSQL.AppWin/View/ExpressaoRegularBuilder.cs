using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class ExpressaoRegularBuilder : Form
	{
		public ExpressaoRegularBuilder()
		{
			InitializeComponent();
			textBox1.Text = "Select\r\n\tCount(*) As Total,\r\n\t(Select\r\nCampo1,\r\nCampo2\r\nFrom Tabela\r\nWhere (Id = Valor)) As Nome,\r\nOutro\r\nFrom Tabela;";
			textBox2.Text = @"\((.|\r|\n)+?\)+";
		}

		private void update(object sender, EventArgs e)
		{
			try
			{
				var matches = Regex.Matches(textBox1.Text, textBox2.Text, RegexOptions.IgnoreCase);
				var result1 = String.Empty;
				var i = 0;
				foreach (Match match in matches)
				{
					var j = 0;
					foreach (Group item in match.Groups)
					{
						result1 += String.Format("matches[{0}].Groups[{1}].Value={2}\r\n", i, j, item.Value);
						j++;
					}
					i++;
				}
				textBox5.Text = result1;
			}
			catch (Exception ex)
			{
				textBox4.Text = textBox5.Text = ex.Message;
			}
		}
	}
}
