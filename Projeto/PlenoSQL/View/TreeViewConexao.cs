using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class TreeViewConexao : TreeView, IDisposable
	{
		private const String cConexoes = @"Conexões";
		private Boolean _isOpen = true;
		private IList<Thread> _threads = new List<Thread>();

		public TreeViewConexao()
		{
			InitializeComponent();
		}

		public void CreateChildren()
		{
			Nodes.Add(new TNode(cConexoes, false));
			BeforeExpand += new TreeViewCancelEventHandler(this.tvDataConnection_BeforeExpand);
			NodeMouseClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseClick);
			NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(this.tvDataConnection_NodeMouseDoubleClick);
		}

		public new virtual void Dispose()
		{
			_isOpen = false;
			if (Nodes.Count > 0 && Nodes[0] is TNode)
				(Nodes[0] as TNode).Dispose();

			while (_threads.Count > 0)
			{
				var t = _threads[0];
				_threads.RemoveAt(0);
				try
				{
					t.Interrupt();
					t.Abort();
				}
				catch (Exception) { }
			}
		}

		private void tvDataConnection_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node.Text.Equals(cConexoes))
			{
				Nodes[0].Expand();
				IBancoDeDados banco = Autenticacao.Dialog(FindForm() as IMessageResult);
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
					var tabelas = bancoDeDados.ListarTabelas(null, true);
					CachearCampos(bancoDeDados.Clone(), tabelas);
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
					var views = bancoDeDados.ListarViews(null, true).OrderBy(v => v);
					CachearCampos(bancoDeDados.Clone(), views);
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
					var procedures = bancoDeDados.ListarProcedures(null, true);
					foreach (var proc in procedures)
						activeNode.Nodes.Add(new TNode(proc, false));
				}
				else if (fullPath.Contains(@"\Tabelas\") || fullPath.Contains(@"\Views\"))
				{
					if (fullPath.EndsWith(@"\Colunas"))
					{
						activeNode.RemoveAll();
						var tableOrView = Path.GetDirectoryName(fullPath);
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
		}

		private void CachearCampos(IBancoDeDados bancoDeDados, IEnumerable<String> tablesOrViews)
		{
			var thread = new Thread(() =>
				{
					foreach (var item in tablesOrViews)
					{
						if (_isOpen)
						{
							var tableOrView = item + " ";
							tableOrView = tableOrView.Substring(0, tableOrView.IndexOfAny(" (".ToCharArray())).Trim();
							bancoDeDados.ListarColunas(tableOrView, true);
							Application.DoEvents();
						}
					}
					bancoDeDados.Dispose();
					GC.Collect();
				}
			);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_threads.Add(thread);
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