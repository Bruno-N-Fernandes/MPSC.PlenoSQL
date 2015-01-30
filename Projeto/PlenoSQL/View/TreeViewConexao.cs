using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MPSC.PlenoSQL.AppWin.Interface;
using MPSC.PlenoSQL.AppWin.View.DataSource;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class TreeViewConexao : TreeView, IDisposable
	{
		private Ramo conexoes = new Ramo(Ramo.cConexoes);
		private TNode root;

		public TreeViewConexao()
		{
			InitializeComponent();
		}

		public void CreateChildren()
		{
			root = new TNode(Ramo.cConexoes, false);
			Nodes.Add(root);
			BeforeExpand += new TreeViewCancelEventHandler(this.tvDataConnection_BeforeExpand);
			NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseClick);
			NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseDoubleClick);
		}

		public new virtual void Dispose()
		{
			root.Dispose();
		}

		private void tvDataConnection_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if ((e.Node is TNode) && e.Node.Text.Equals(Ramo.cConexoes))
			{
				IBancoDeDados banco = Autenticacao.Dialog(FindForm() as IMessageResult);
				if (banco != null)
				{
					var nodes = root.Nodes;
					nodes.Add(new DataNode(banco));
					var node = nodes[nodes.Count - 1];
					node.Nodes.Add(new TNode("Tabelas", true));
					node.Nodes.Add(new TNode("Views", true));
					node.Nodes.Add(new TNode("Procedures", true));
					node.Expand();
					root.Expand();
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
					var tabelas = bancoDeDados.ListarTabelas(null, true);
					foreach (var tabela in tabelas.OrderBy(t => t))
					{
						var tn = new TNode(tabela, false);
						tn.Nodes.Add(new TNode("Colunas", true));
						tn.Nodes.Add(new TNode("Índices", true));
						tn.Nodes.Add(new TNode("Triggers", true));
						activeNode.Nodes.Add(tn);
					}
				}
				else if (fullPath.EndsWith(@"\Views"))
				{
					activeNode.RemoveAll();
					var views = bancoDeDados.ListarViews(null, true).OrderBy(v => v);
					foreach (var view in views)
					{
						var tn = new TNode(view, false);
						tn.Nodes.Add(new TNode("Colunas", true));
						activeNode.Nodes.Add(tn);
					}
				}
				else if (fullPath.EndsWith(@"\Procedures"))
				{
					activeNode.RemoveAll();
					var procedures = bancoDeDados.ListarProcedures(null, true);
					foreach (var proc in procedures)
						activeNode.Nodes.Add(new TNode(proc, false));
				}
				else if (fullPath.Contains(@"\Tabelas\") || fullPath.Contains(@"\Views\"))
				{
					if (fullPath.EndsWith(@"\Colunas"))
					{
						activeNode.RemoveAll();
						var tableOrView = Path.GetDirectoryName(fullPath.Replace(":", "."));
						tableOrView = Path.GetFileNameWithoutExtension(tableOrView).Trim() + " ";
						tableOrView = tableOrView.Substring(0, tableOrView.IndexOfAny(" (".ToCharArray()));
						var colunas = bancoDeDados.ListarColunas(tableOrView, true);

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
			Application.DoEvents();
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

		public void Filtrar(String filtro)
		{
			root.ExpandAll();
			Nodes.RemoveAt(0);
			if (String.IsNullOrWhiteSpace(filtro))
				Nodes.Add(root);
			else
			{
				conexoes.RemoverTodos();
				Copiar(root.Nodes, conexoes);

				Atualizar(Nodes, conexoes.Filtrar(filtro));
			}
			Nodes[0].ExpandAll();
		}

		private void Copiar(TreeNodeCollection Nodes, Ramo ramo)
		{
			foreach (TreeNode node in Nodes)
			{
				var r = ramo.Adicionar(new Ramo(node.Text));
				Copiar(node.Nodes, r);
			}
		}

		private void Atualizar(TreeNodeCollection nodes, Ramo ramo)
		{
			if (ramo != null)
			{
				var node = nodes.Add(ramo.Descricao);
				foreach (var r in ramo.Ramos)
					Atualizar(node.Nodes, r);
			}
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

		protected virtual Boolean Match(String filtro)
		{
			return FullPath.ToUpper().Contains(filtro.ToUpper());
		}

		protected virtual IEnumerable<TNode> Children
		{
			get
			{
				foreach (var item in Nodes)
					if (item is TNode)
						yield return item as TNode;
			}
		}

		protected virtual IEnumerable<TNode> AllChildrens(IEnumerable<TNode> nodes)
		{
			var lista = new List<TNode>();
			foreach (var item in nodes)
			{
				if (item.Children.Count() == 0)
					lista.Add(item);
				else
					lista.AddRange(AllChildrens(item.Children));
			}
			return lista;
		}

		protected virtual IEnumerable<TNode> AllChildren { get { return AllChildrens(Children); } }

		public virtual TNode Clone(String filtro)
		{
			var node = NewClone();
			foreach (var item in this.Children)
				if (item.Match(filtro) || item.AllChildren.Any(c => c.Match(filtro)))
					node.Nodes.Add(item.Clone(filtro));
			return node;
		}

		public virtual TNode NewClone()
		{
			return new TNode(Text, false);
		}
	}

	public class NullTreeNode : TNode
	{
		public NullTreeNode()
			: base(String.Empty, false)
		{
		}
		public override TNode NewClone()
		{
			return new NullTreeNode();
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

		public override TNode NewClone()
		{
			return new DataNode(BancoDeDados);
		}
	}
}