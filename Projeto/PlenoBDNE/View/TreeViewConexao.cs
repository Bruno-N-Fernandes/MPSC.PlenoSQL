using System;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados;

namespace MP.PlenoBDNE.AppWin.View
{
	public class TreeViewConexao : TreeView, IDisposable
	{
		public new virtual void Dispose()
		{
			(Nodes[0] as TNode).Dispose();
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