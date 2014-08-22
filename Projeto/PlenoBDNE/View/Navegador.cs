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
		private static readonly String arquivoConfig1 = Path.GetTempPath() + "NavegadorDeDados.files";
		private static readonly String arquivoConfig2 = Path.GetTempPath() + "NavegadorDeDados.cgf";
		private IList<String> arquivos = new List<String>();
		private IQueryResult ActiveTab { get { return (tabQueryResult.TabPages.Count > 0) ? tabQueryResult.TabPages[tabQueryResult.SelectedIndex] as IQueryResult : NullQueryResult.Instance; } }
		public Boolean SalvarAoExecutar { get { return ckSalvarAoExecutar.Checked; } private set { ckSalvarAoExecutar.Checked = value; } }
		public Boolean ConvertToUpper { get { return ckUpperCase.Checked; } private set { ckUpperCase.Checked = value; } }

		public Navegador()
		{
			InitializeComponent();
			tvDataConnection.Nodes.Add(new TNode(cConexoes, false));
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
			Util.ArrayToFile(arquivoConfig2, ConvertToUpper.ToString(), SalvarAoExecutar.ToString());
			tvDataConnection.Dispose();
			BancoDeDados.Clear();
		}

		private void tvDataConnection_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Text.Equals(cConexoes))
			{
				tvDataConnection.Nodes[0].Expand();
				IBancoDeDados banco = Autenticacao.Dialog();
				if (banco != null)
				{
					var nodes = tvDataConnection.Nodes[0].Nodes;
					nodes.Add(new DataNode(banco));
					var node = nodes[nodes.Count - 1];
					node.Nodes.Add(new TNode("Tabelas", true));
					node.Nodes.Add(new TNode("Views", true));
					node.Nodes.Add(new TNode("Procedures", true));
					node.Expand();
					tvDataConnection.Nodes[0].Expand();
				}
			}
		}

		private void tvDataConnection_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if ((e.Node.Nodes.Count >= 1) && (e.Node.Nodes[0] is NullTreeNode))
				Expandir(e.Node as TNode);
		}

		private void tvDataConnection_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			Expandir(e.Node as TNode);
		}

		private void Expandir(TNode activeNode)
		{
			var bancoDeDados = ObterBancoAtivo(activeNode);
			if ((bancoDeDados != null) && !(activeNode is DataNode))
			{
				String fullPath = activeNode.FullPath;
				if (fullPath.EndsWith(@"\Tabelas"))
				{
					activeNode.RemoveAll();
					var tabelas = bancoDeDados.ListarTabelas("");
					foreach (var tabela in tabelas.OrderBy(t => t))
					{
						var tn = new TNode(tabela, false);
						tn.Nodes.Add(new TNode("Colunas", true));
						tn.Nodes.Add(new TNode("Índices", true));
						tn.Nodes.Add(new TNode("Triggers", true));
						activeNode.Nodes.Add(tn);
					}
				}
				if (fullPath.EndsWith(@"\Views"))
				{
					activeNode.RemoveAll();
					var views = bancoDeDados.ListarViews("").OrderBy(v => v);
					foreach (var view in views)
					{
						var tn = new TNode(view, false);
						tn.Nodes.Add(new TNode("Colunas", true));
						activeNode.Nodes.Add(tn);
					}
				}
				if (fullPath.EndsWith(@"\Procedures"))
				{
					activeNode.RemoveAll();
					//var procedures = bancoDeDados.ListarProcedures("");
					//foreach (var proc in procedures)
					//    activeNode.Nodes.Add(new TNode(proc, false));
				}
				else if (fullPath.Contains(@"\Tabelas\") || fullPath.Contains(@"\Views\"))
				{
					if (fullPath.EndsWith(@"\Colunas"))
					{
						activeNode.RemoveAll();
						var tableOrView = Path.GetDirectoryName(fullPath);
						tableOrView = Path.GetFileNameWithoutExtension(tableOrView);
						var colunas = bancoDeDados.ListarColunasDasTabelas(tableOrView, true);

						foreach (var coluna in colunas)
							activeNode.Nodes.Add(new TNode(TratarNulos(coluna), false));
					}
					else if (fullPath.EndsWith(@"\Índices"))
					{
						activeNode.RemoveAll();
					}
					else if (fullPath.EndsWith(@"\Triggers"))
					{
						activeNode.RemoveAll();
					}
				}
				activeNode.Expand();
			}
		}

		private String TratarNulos(String coluna)
		{
			return coluna.Replace(", NOT NULL", ", Obrigatório").Replace(", NULL", ", Anulável");
		}

		private IBancoDeDados ObterBancoAtivo(TreeNode activeNode)
		{
			TreeNode treeNode = activeNode;
			while ((treeNode != null) && (treeNode.Parent != null) && !(treeNode is DataNode))
				treeNode = treeNode.Parent;

			return (treeNode as DataNode) == null ? null : (treeNode as DataNode).BancoDeDados;
		}

		public void Status(String mensagem)
		{
			tsslConexao.Text = mensagem;
		}
	}


}