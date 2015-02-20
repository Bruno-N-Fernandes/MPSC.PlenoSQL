using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public class Constante
	{
		public const String GLOBAL = "GLOBAL";
		private readonly String _escopo;
		public String Escopo { get { return _escopo; } }
		public String Nome { get; set; }
		public String Valor { get; set; }

		public Constante(String escopo, String nome, String valor)
		{
			_escopo = escopo;
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

		public void Adicionar(String escopo, String nome, String valor)
		{
			Adicionar(new Constante(escopo ?? Constante.GLOBAL, nome.Trim(), valor));
		}

		private void Adicionar(Constante constante)
		{
			Remover(constante.Escopo, constante.Nome);
			_constantes.Add(constante);
		}

		public void Remover(String escopo, String nome)
		{
			var constante = _constantes.FirstOrDefault(c => c.Nome.Equals(nome) && c.Escopo.Equals(escopo));
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
					constantes = _constantes;
					break;
				case Filtro.TodasDoArquivo:
					constantes = _constantes.Where(c => c.Escopo.Equals(Constante.GLOBAL)).Union(_constantes.Where(c => c.Escopo.Equals(escopo)));
					break;
				case Filtro.Globais:
					constantes = _constantes.Where(c => c.Escopo.Equals(Constante.GLOBAL));
					break;
				case Filtro.Locais:
					constantes = _constantes.Where(c => c.Escopo.Equals(escopo));
					break;
				case Filtro.Ativas:
					var constLocais = _constantes.Where(cl => cl.Escopo.Equals(escopo)).ToList();
					var constGlobais = _constantes.Where(cg => cg.Escopo.Equals(Constante.GLOBAL));
					constantes = constGlobais.Where(cg => !constLocais.Any(cl => cl.Nome.Equals(cg.Nome))).Union(constLocais);
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
	}
}