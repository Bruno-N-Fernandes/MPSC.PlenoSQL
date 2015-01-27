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
			var ramo = new Ramo("Conexoes");
			var conexao = ramo.Adicionar(new Conexao("IBM"));
			conexao.Adicionar(new Tabela());
			conexao.Adicionar(new Tabela());
			conexao.Adicionar(new View());
			conexao.Adicionar(new View());
			conexao.Adicionar(new Procedure());
			conexao.Adicionar(new Procedure());
			ramo.Filtrar("s");
		}
	}


	public interface IRamo
	{
		String Descricao { get; }
		Object Dados { get; set; }
	}

	public class Ramo : IRamo
	{
		protected Ramo Pai;
		protected readonly List<IRamo> Ramos;
		public String Descricao { get; protected set; }
		public Object Dados { get; set; }

		public Ramo(String descricao)
		{
			Descricao = descricao;
			Dados = descricao;
			Ramos = new List<IRamo>();
		}

		public virtual TRamo Adicionar<TRamo>(TRamo ramo) where TRamo : Ramo
		{
			ramo.Pai = this;
			Ramos.Add(ramo);
			return ramo;
		}

		public Ramo Filtrar(String filtro)
		{
			return this;
		}
	}

	public class Conexao : Ramo
	{
		private readonly Tabela _tabela;
		private readonly View _view;
		private readonly Procedure _procedure;
		public Conexao(String descricao)
			: base(descricao)
		{
			_tabela = base.Adicionar(new Tabela());
			_view = base.Adicionar(new View());
			_procedure = base.Adicionar(new Procedure());
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
		public Tabela() : base("Tabelas") { }
	}

	public class View : Ramo
	{
		public View() : base("Views") { }
	}

	public class Procedure : Ramo
	{
		public Procedure() : base("Procedures") { }
	}
}