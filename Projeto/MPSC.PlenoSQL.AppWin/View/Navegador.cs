using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Infra;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class Navegador : Form, INavegador
	{
		private static readonly String arquivoConfig1 = Path.GetTempPath() + "NavegadorDeDados.files";
		private static readonly String arquivoConfig2 = Path.GetTempPath() + "NavegadorDeDados.cgf";
		private IList<String> arquivos = new List<String>();
		private IQueryResult ActiveTab { get { return (tabQueryResult.TabPages.Count > 0) ? tabQueryResult.TabPages[tabQueryResult.SelectedIndex] as IQueryResult : NullQueryResult.Instance; } }
		public Boolean SalvarAoExecutar { get { return ckSalvarAoExecutar.Checked; } private set { ckSalvarAoExecutar.Checked = value; } }
		public Boolean ConvertToUpper { get { return ckUpperCase.Checked; } private set { ckUpperCase.Checked = value; } }
		public Boolean Colorir { get { return ckColorir.Checked; } private set { ckColorir.Checked = value; } }

		public Navegador()
		{
			InitializeComponent();
			Text += " " + CoreAssembly.VersionString;
		}

		private void btNovoDocumento_Click(object sender, EventArgs e)
		{
			tabQueryResult.Controls.Add(new QueryResult(null));
			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void btAbrirDocumento_Click(object sender, EventArgs e)
		{
			AbrirArquivosImpl(FileUtil.GetFilesToOpen("Arquivos de Banco de Dados|*.sql;*.qry"));
		}

		public Navegador AbrirDocumentos(Boolean appJaEstavaRodando, IEnumerable<String> arquivos)
		{
			if (!appJaEstavaRodando)
				AbrirArquivosImpl(FileUtil.FileToArray(arquivoConfig1, 1));
			AbrirArquivosImpl(arquivos);
			return this;
		}

		private void AbrirArquivosImpl(IEnumerable<String> arquivos)
		{
			foreach (var arquivo in arquivos)
				if (!tabQueryResult.TabPages.OfType<IQueryResult>().Any(qr => qr.NomeDoArquivo == arquivo))
					tabQueryResult.Controls.Add(new QueryResult(arquivo));

			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
			ActiveTab.Focus();
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

		private void btAlterarConexao_Click(object sender, EventArgs e)
		{
			ActiveTab.AlterarConexao();
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
			var arquivos = FileUtil.FileToArray(arquivoConfig1, 1);
			var config = FileUtil.FileToArray(arquivoConfig2, 3);
			ConvertToUpper = config[0].Equals(true.ToString());
			SalvarAoExecutar = config[1].Equals(true.ToString());
			Colorir = config[2].Equals(true.ToString());
			if (tabQueryResult.TabPages.Count == 0)
				AbrirArquivosImpl(arquivos);
			tvDataConnection.CreateChildren();
		}

		private void Navegador_FormClosing(object sender, FormClosingEventArgs e)
		{
			arquivos.Clear();
			Boolean salvouTodos = true;
			var i = 0;
			while (salvouTodos && (i < tabQueryResult.Controls.Count))
			{
				var queryResult = tabQueryResult.Controls[i++] as QueryResult;
				if (!queryResult.PodeFechar())
					salvouTodos = false;
			}

			while (salvouTodos && tabQueryResult.Controls.Count > 0)
			{
				var queryResult = tabQueryResult.Controls[0] as QueryResult;
				tabQueryResult.Controls.Remove(queryResult);
				queryResult.Fechar();

				if (File.Exists(queryResult.NomeDoArquivo))
					arquivos.Add(queryResult.NomeDoArquivo);
			}

			e.Cancel = !salvouTodos;
		}

		private void Navegador_FormClosed(object sender, FormClosedEventArgs e)
		{
			Visible = false;
			FileUtil.ArrayToFile(arquivoConfig1, arquivos.ToArray());
			FileUtil.ArrayToFile(arquivoConfig2, ConvertToUpper.ToString(), SalvarAoExecutar.ToString(), Colorir.ToString());
			tvDataConnection.Dispose();
			BancoDados.LimparCache();
			Configuracao.Instancia.SaveConstantes();
		}

		public void Status(String mensagem)
		{
			tsslConexao.Text = mensagem;
		}

		private void txtFiltroTreeView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				tvDataConnection.Filtrar(txtFiltroTreeView.Text);
		}

		private void btDefinirConstantes_Click(object sender, EventArgs e)
		{
			DefinicaoDeConstantes.Visualizar(Constantes.Instancia, ActiveTab.NomeDoArquivo);
		}

		private void btGerarVO_Click(object sender, EventArgs e)
		{
			new ClasseUtilForm().ShowDialog();
		}
	}
}