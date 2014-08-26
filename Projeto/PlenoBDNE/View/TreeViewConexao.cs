using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.View
{
	public class TreeViewConexao : TreeView, IDisposable
	{
		private const String cConexoes = @"Conexões";

		public void CreateChildren()
		{
			Nodes.Add(new TNode(cConexoes, false));
			BeforeExpand += new TreeViewCancelEventHandler(this.tvDataConnection_BeforeExpand);
			NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseClick);
			NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseDoubleClick);
		}

		public new virtual void Dispose()
		{
			if (Nodes.Count > 0 && Nodes[0] is TNode)
				(Nodes[0] as TNode).Dispose();
		}

		private void tvDataConnection_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Text.Equals(cConexoes))
			{
				Nodes[0].Expand();
				IBancoDeDados banco = Autenticacao.Dialog();
				if (banco != null)
				{
					var nodes = Nodes[0].Nodes;
					nodes.Add(new DataNode(banco));
					var node = nodes[nodes.Count - 1];
					node.Nodes.Add(new TNode("Tabelas", true));
					node.Nodes.Add(new TNode("Views", true));
					node.Nodes.Add(new TNode("Procedures", true));
					node.Expand();
					Nodes[0].Expand();
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

		private IBancoDeDados ObterBancoAtivo(TreeNode treeNode)
		{
			while ((treeNode != null) && (treeNode.Parent != null) && !(treeNode is DataNode))
				treeNode = treeNode.Parent;

			return (treeNode as DataNode) == null ? null : (treeNode as DataNode).BancoDeDados;
		}
	}

	public class TNode : TreeNode, IDisposable
	{
		public TNode(String text, Boolean insertNull)
			: base(text)
		{
			if (insertNull)
				Nodes.Add(new NullTreeNode());
		}

		public virtual void Dispose()
		{
			foreach (TreeNode node in Nodes)
			{
				if (node is TNode)
					(node as TNode).Dispose();
			}
			RemoveAll();
		}

		public void RemoveAll()
		{
			while (Nodes.Count > 0)
				Nodes.RemoveAt(0);
		}
	}

	public class NullTreeNode : TNode
	{
		public NullTreeNode()
			: base(String.Empty, false)
		{
		}
	}

	public class DataNode : TNode
	{
		public IBancoDeDados BancoDeDados { get; private set; }
		public DataNode(IBancoDeDados bancoDeDados) : base(bancoDeDados.Conexao, false) { BancoDeDados = bancoDeDados; }

		public override void Dispose()
		{
			if (BancoDeDados != null)
				BancoDeDados.Dispose();
			base.Dispose();
		}
	}
}