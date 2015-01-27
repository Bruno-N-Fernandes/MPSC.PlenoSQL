using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var conexoes = new Ramo("Conexoes");
			var conexao = conexoes.Adicionar(new Conexao("IBM"));
			conexao.Adicionar(new Tabela("ItemProduto"));
			conexao.Adicionar(new Tabela("ItemProdutoServico"));
			//conexao.Adicionar(new View("ServicoGrupal"));
			//conexao.Adicionar(new View("ItemView"));
			//conexao.Adicionar(new Procedure("PC_Grupal"));
			//conexao.Adicionar(new Procedure("Procs"));
			var col = conexoes.Filtrar("a");
			Assert.IsNotNull(col);

		}
	}


	public static class Gerador
	{
		private static Int64 _id = 0;
		public static Int64 NewId { get { return ++_id; } }
	}

	public class Ramo
	{
		protected Ramo Pai;
		protected readonly Int64 Id;
		protected readonly List<Ramo> Ramos;
		public readonly String Descricao;
		private String Path { get { return (Pai != null) ? Pai.Path + "/" + Descricao.ToUpper() : Descricao.ToUpper(); } }

		public Ramo(String descricao) : this(descricao, new List<Ramo>()) { }
		private Ramo(String descricao, IEnumerable<Ramo> ramos) : this(Gerador.NewId, descricao, new List<Ramo>()) { }
		private Ramo(Int64 id, String descricao, IEnumerable<Ramo> ramos)
		{
			Id = id;
			Descricao = descricao;
			Ramos = ramos.ToList();
		}

		public Ramo Clone()
		{
			return new Ramo(Id, Descricao, new List<Ramo>()) { Pai = this.Pai };
		}

		public virtual TRamo Adicionar<TRamo>(TRamo ramo) where TRamo : Ramo
		{
			ramo.Pai = this;
			Ramos.Add(ramo);
			return ramo;
		}

		public Ramo Filtrar(String filtro)
		{
			filtro = filtro.ToUpper();
			var ramos = folhas(this).Where(r => (r.Ramos.Count == 0) && r.Path.Contains(filtro));
			return reconstruir(ramos);
		}

		private Ramo reconstruir(IEnumerable<Ramo> ramos)
		{
			var ramosPai = new List<Ramo>();
			foreach (var ramoFolha in ramos)
			{
				if (ramoFolha.Pai != null)
				{
					var ramoPai = ramosPai.FirstOrDefault(r => r.Id == ramoFolha.Pai.Id);
					if (ramoPai == null)
					{
						ramoPai = ramoFolha.Pai.Clone();
						ramosPai.Add(ramoPai);
					}
					ramoPai.Adicionar(ramoFolha.Clone());
				}
			}

			if (ramosPai.Count > 1)
				return reconstruir(ramosPai);
			else
				return ramosPai[0];
		}

		private IEnumerable<Ramo> folhas(Ramo ramo)
		{
			return ramo.Ramos.Count > 0 ? ramo.Ramos.SelectMany(r => folhas(r)) : Enumere(ramo);
		}

		private IEnumerable<Ramo> Enumere(Ramo ramo) { yield return ramo; }
	}

	public class Conexao : Ramo
	{
		private readonly Tabelas _tabela;
		private readonly Views _view;
		private readonly Procedures _procedure;
		public Conexao(String descricao)
			: base(descricao)
		{
			_tabela = base.Adicionar(new Tabelas());
			_view = base.Adicionar(new Views());
			_procedure = base.Adicionar(new Procedures());
		}

		public override TRamo Adicionar<TRamo>(TRamo ramo)
		{
			if (ramo is Tabela)
				return _tabela.Adicionar(ramo);
			else if (ramo is View)
				return _view.Adicionar(ramo);
			else if (ramo is Procedure)
				return _procedure.Adicionar(ramo);
			return ramo;
		}
	}


	public class Tabela : Ramo
	{
		private readonly Colunas _colunas;
		private readonly Indices _indices;
		private readonly Triggers _triggers;

		public Tabela(String descricao)
			: base(descricao)
		{
			_colunas = base.Adicionar(new Colunas());
			_indices = base.Adicionar(new Indices());
			_triggers = base.Adicionar(new Triggers());
		}
	}

	public class View : Ramo
	{
		public View(String descricao) : base(descricao) { }
	}

	public class Procedure : Ramo
	{
		public Procedure(String descricao) : base(descricao) { }
	}


	public class Tabelas : Ramo
	{
		public Tabelas() : base("Tabelas") { }
	}

	public class Views : Ramo
	{
		public Views() : base("Views") { }
	}

	public class Procedures : Ramo
	{
		public Procedures() : base("Procedures") { }
	}

	public class Colunas : Ramo
	{
		public Colunas() : base("Colunas") { }
	}

	public class Indices : Ramo
	{
		public Indices() : base("Indices") { }
	}

	public class Triggers : Ramo
	{
		public Triggers() : base("Triggers") { }
	}

}