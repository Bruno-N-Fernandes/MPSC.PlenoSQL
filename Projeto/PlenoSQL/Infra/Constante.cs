using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public class Constante
	{
		public const String GLOBAL = "GLOBAL";
		public String Nome { get; set; }
		public String Valor { get; set; }
		public Type Tipo { get; set; }
		public String Escopo { get; set; }

		public Constante(string nome, string valor, Type tipo, string escopo)
		{
			Nome = nome;
			Valor = valor;
			Tipo = tipo;
			Escopo = escopo;
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

		public void Adicionar(String nome, String valor, Type tipo, String escopo)
		{
			Adicionar(new Constante(nome.Trim(), valor, tipo, escopo));
		}

		private void Adicionar(Constante constante)
		{
			Remover(constante.Nome, constante.Escopo);
			_constantes.Add(constante);
		}

		public void Remover(String nome, String escopo)
		{
			var constante = _constantes.FirstOrDefault(c => c.Nome.Equals(nome) && c.Escopo.Equals(escopo));
			if (constante != null)
				_constantes.Remove(constante);
		}

		public IEnumerable<Constante> Obter(String escopo)
		{
			var constantes = _constantes.Where(c => c.Escopo.Equals(escopo));
			var union = _constantes.Where(c => c.Escopo.Equals(Constante.GLOBAL) && !constantes.Any(c2 => c2.Nome.Equals(c.Nome)));
			return constantes.Union(union);
		}
	}
}