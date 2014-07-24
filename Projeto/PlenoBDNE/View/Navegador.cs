using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LBJC.NavegadorDeDados.Infra;

namespace LBJC.NavegadorDeDados
{
	public partial class Navegador : Form
	{
		private const String arquivoConfig = "NavegadorDeDados.txt";
		private IList<String> arquivos = new List<String>();
		private IQueryResult ActiveTab { get { return (tabQueryResult.TabPages.Count > 0) ? tabQueryResult.TabPages[tabQueryResult.SelectedIndex] as IQueryResult : NullQueryResult.Instance; } }

		public Navegador()
		{
			InitializeComponent();
		}

		private void btNovoDocumento_Click(object sender, EventArgs e)
		{
			tabQueryResult.Controls.Add(new QueryResult(null));
			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void btAbrirDocumento_Click(object sender, EventArgs e)
		{
			var arquivos = Extensions.GetFilesToOpen("Arquivos de Banco de Dados|*.sql;*.qry");
			foreach (var arquivo in arquivos)
				tabQueryResult.Controls.Add(new QueryResult(arquivo));

			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void btSalvarDocumento_Click(object sender, EventArgs e)
		{
			ActiveTab.Salvar();
		}

		private void btSalvarTodos_Click(object sender, EventArgs e)
		{
			Boolean salvouTodos = true;
			foreach (IQueryResult queryResult in tabQueryResult.Controls)
				salvouTodos = salvouTodos && queryResult.Salvar();
		}

		private void btExecutar_Click(object sender, EventArgs e)
		{
			ActiveTab.Executar();
		}

		private void btFechar_Click(object sender, EventArgs e)
		{
			var tab = ActiveTab;
			if (tab.PodeFechar())
			{
				tabQueryResult.Controls.Remove(tab as TabPage);
				tab.Fechar();
			}
		}

		private void tabQueryResult_Click(object sender, EventArgs e)
		{
			ActiveTab.Focus();
		}

		private void Navegador_Load(object sender, EventArgs e)
		{
			var arquivos = Util.FileToArray(arquivoConfig);

			foreach (var arquivo in arquivos)
				tabQueryResult.Controls.Add(new QueryResult(arquivo));

			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void Navegador_FormClosing(object sender, FormClosingEventArgs e)
		{
			arquivos.Clear();
			Boolean salvouTodos = true;
			while (salvouTodos && tabQueryResult.Controls.Count > 0)
			{
				var queryResult = tabQueryResult.Controls[0] as IQueryResult;
				if (queryResult.PodeFechar())
				{
					tabQueryResult.Controls.Remove(queryResult as TabPage);
					queryResult.Fechar();
				}
				else
					salvouTodos = false;

				if (File.Exists(queryResult.NomeDoArquivo))
					arquivos.Add(queryResult.NomeDoArquivo);
			}
			e.Cancel = !salvouTodos;
		}

		private void Navegador_FormClosed(object sender, FormClosedEventArgs e)
		{
			Util.ArrayToFile(arquivoConfig, arquivos.ToArray());
		}
	}
}