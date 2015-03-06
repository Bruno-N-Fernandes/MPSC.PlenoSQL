using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public class Constante
	{
		public const String GLOBAL = "GLOBAL";
		public readonly String escopo;
		public String Escopo { get { return Path.GetFileName(escopo); } }
		public String Nome { get; set; }
		public String Valor { get; set; }

		public Constante(String escopo, String nome, String valor)
		{
			this.escopo = escopo;
			Nome = nome;
			Valor = valor;
		}

		public override String ToString()
		{
			return String.Format("{0}={1}", Nome, Valor);
		}
	}

	public class Constantes
	{
		private readonly List<Constante> _constantes = new List<Constante>();
		public Int32 Count { get { return _constantes.Count; } }

		private Constantes() { }

		public Constante Adicionar(String escopo, String nome, String valor)
		{
			return Adicionar(new Constante(escopo ?? Constante.GLOBAL, nome.Trim(), valor));
		}

		private Constante Adicionar(Constante constante)
		{
			Remover(constante.escopo, constante.Nome);
			_constantes.Add(constante);
			return constante;
		}

		public void Remover(String escopo, String nome)
		{
			var constante = _constantes.FirstOrDefault(c => c.Nome.Equals(nome) && c.escopo.Equals(escopo));
			if (constante != null)
				_constantes.Remove(constante);
		}

		public IEnumerable<Constante> Obter(String escopo)
		{
			return Obter(escopo, Filtro.Ativas);
		}

		public IEnumerable<Constante> Obter(String escopo, Filtro filtro)
		{
			IEnumerable<Constante> constantes;
			switch (filtro)
			{
				case Filtro.TodasDeTodos:
					constantes = _constantes.Where(c => c.escopo.Equals(Constante.GLOBAL)).OrderBy(cl => cl.Nome).Union(_constantes.Where(c => !c.escopo.Equals(Constante.GLOBAL)).OrderBy(cl => cl.Escopo).ThenBy(cl => cl.Nome));
					break;
				case Filtro.TodasDoArquivo:
					constantes = _constantes.Where(c => c.escopo.Equals(Constante.GLOBAL)).OrderBy(cl => cl.Nome).Union(_constantes.Where(c => c.escopo.Equals(escopo)).OrderBy(cl => cl.Nome));
					break;
				case Filtro.Globais:
					constantes = _constantes.Where(c => c.escopo.Equals(Constante.GLOBAL)).OrderBy(cl => cl.Nome);
					break;
				case Filtro.Locais:
					constantes = _constantes.Where(c => c.escopo.Equals(escopo)).OrderBy(cl => cl.Nome);
					break;
				case Filtro.Ativas:
					var constLocais = _constantes.Where(cl => cl.escopo.Equals(escopo)).ToList();
					var constGlobais = _constantes.Where(cg => cg.escopo.Equals(Constante.GLOBAL));
					constantes = constGlobais.Where(cg => !constLocais.Any(cl => cl.Nome.Equals(cg.Nome))).OrderBy(cl => cl.Nome).Union(constLocais.OrderBy(cl => cl.Nome));
					break;
				default:
					constantes = new List<Constante>();
					break;
			}
			return constantes;
		}

		public enum Filtro
		{
			TodasDeTodos = 0,
			TodasDoArquivo = 1,
			Globais = 2,
			Locais = 3,
			Ativas = 4,
		}

		private static Constantes _instancia;
		public static Constantes Instancia
		{
			get { return (_instancia ?? (Instancia = Instanciar(true))); }
			private set { _instancia = _instancia ?? value ?? Instanciar(true); }
		}

		public static Constantes Instanciar(Boolean autoLoad)
		{
			var instancia = _instancia ?? new Constantes();
			return autoLoad ? Configuracao.Instancia.LoadConstantes(instancia) : instancia;
		}
	}
}