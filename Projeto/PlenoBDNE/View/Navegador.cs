using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.View.Interface;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class Navegador : Form, INavegador
	{
		private const String cConexoes = @"Conexões (Click p/ Incluir)";
		private const String arquivoConfig1 = "NavegadorDeDados.files";
		private const String arquivoConfig2 = "NavegadorDeDados.cgf";
		private IList<String> arquivos = new List<String>();
		private IQueryResult ActiveTab { get { return (tabQueryResult.TabPages.Count > 0) ? tabQueryResult.TabPages[tabQueryResult.SelectedIndex] as IQueryResult : NullQueryResult.Instance; } }
		public Boolean SalvarAoExecutar { get { return ckSalvarAoExecutar.Checked; } private set { ckSalvarAoExecutar.Checked = value; } }
		public Boolean ConvertToUpper { get { return ckUpperCase.Checked; } private set { ckUpperCase.Checked = value; } }
		public Boolean Colorir { get { return ckColorir.Checked; } private set { ckColorir.Checked = value; } }

		public Navegador()
		{
			InitializeComponent();
			tvDataConnection.Nodes.Add(cConexoes);
		}

		private void btNovoDocumento_Click(object sender, EventArgs e)
		{
			tabQueryResult.Controls.Add(new QueryResult(null));
			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void btAbrirDocumento_Click(object sender, EventArgs e)
		{
			var arquivos = Util.GetFilesToOpen("Arquivos de Banco de Dados|*.sql;*.qry");
			foreach (var arquivo in arquivos)
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
			var arquivos = Util.FileToArray(arquivoConfig1, 1);

			foreach (var arquivo in arquivos)
				tabQueryResult.Controls.Add(new QueryResult(arquivo));

			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
			ActiveTab.Focus();

			var config = Util.FileToArray(arquivoConfig2, 3);
			ConvertToUpper = config[0].Equals(true.ToString());
			SalvarAoExecutar = config[1].Equals(true.ToString());
			Colorir = config[2].Equals(true.ToString());
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
			Util.ArrayToFile(arquivoConfig1, arquivos.ToArray());
			Util.ArrayToFile(arquivoConfig2, ConvertToUpper.ToString(), SalvarAoExecutar.ToString(), Colorir.ToString());
			BancoDeDados.Clear();
		}

		private void tvDataConnection_DoubleClick(object sender, EventArgs e)
		{
			var bancoDeDados = ObterBancoAtivo();
			if (bancoDeDados == null)
			{
				tvDataConnection.Nodes[0].Expand();
				IBancoDeDados banco = Autenticacao.Dialog();
				if (banco != null)
				{
					var nodes = tvDataConnection.Nodes[0].Nodes;
					nodes.Add(new DataTreeItem(banco, banco.Conexao));
					var node = nodes[nodes.Count - 1];
					node.Nodes.Add("Tabelas");
					node.Nodes.Add("Views");
					node.Nodes.Add("Procedures");
					node.Expand();
					tvDataConnection.Nodes[0].Expand();
				}
			}
			else
			{
				String fullPath = tvDataConnection.SelectedNode.FullPath;
				if (fullPath.EndsWith(@"\Tabelas"))
				{
					var tabelas = bancoDeDados.ListarTabelas("").OrderBy(t => t);
					foreach (var tabela in tabelas)
						tvDataConnection.SelectedNode.Nodes.Add(tabela);
				}
				if (fullPath.EndsWith(@"\Views"))
				{
					var tabelas = bancoDeDados.ListarViews("").OrderBy(v => v);
					foreach (var tabela in tabelas)
						tvDataConnection.SelectedNode.Nodes.Add(tabela);
				}
				if (fullPath.EndsWith(@"\Procedures"))
				{
					//var tabelas = bancoDeDados.ListarProcedures("");
					//foreach (var tabela in tabelas)
					//	tvDataConnection.SelectedNode.Nodes.Add(tabela);
				}
				else if (fullPath.Contains(@"\Tabelas\") || fullPath.Contains(@"\Views\"))
				{
					var colunas = bancoDeDados.ListarColunasDasTabelas(Path.GetFileNameWithoutExtension(tvDataConnection.SelectedNode.FullPath));
					foreach (var coluna in colunas.OrderBy(c => c))
						tvDataConnection.SelectedNode.Nodes.Add(coluna);
				}
				tvDataConnection.SelectedNode.ExpandAll();
			}
		}

		private IBancoDeDados ObterBancoAtivo()
		{
			TreeNode treeNode = tvDataConnection.SelectedNode;
			while ((treeNode != null) && (treeNode.Parent != null) && !(treeNode is DataTreeItem))
				treeNode = treeNode.Parent;

			return (treeNode as DataTreeItem) == null ? null : (treeNode as DataTreeItem).BancoDeDados;
		}

		public void Status(String mensagem)
		{
			tsslConexao.Text = mensagem;
		}
	}

	public class DataTreeItem : TreeNode
	{
		public IBancoDeDados BancoDeDados { get; private set; }
		public DataTreeItem(IBancoDeDados bancoDeDados, String text)
			: base(text)
		{
			BancoDeDados = bancoDeDados;
		}
	}
}