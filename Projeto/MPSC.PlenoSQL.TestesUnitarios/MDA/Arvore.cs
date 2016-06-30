using System;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	public class Arvore<Tipo> where Tipo: IComparable
	{
		private No<Tipo> Raiz;

		public void Adicionar(Tipo valor)
		{
			var no = new No<Tipo>(valor);
			if (Raiz == null)
				Raiz = no;

			if (valor.CompareTo(Raiz.Valor) > 0)
			{
				no.Esquerda = Raiz;
				Raiz = no;
			}

			if (valor.CompareTo(Raiz.Valor) > 0)
			{
				no.Direita = Raiz;
				Raiz = no;
			}
		}
	}

	public class No<Tipo>
	{
		public No<Tipo> Esquerda;
		public readonly Tipo Valor;
		public No<Tipo> Direita;

		public No(Tipo valor)
		{
			Valor = valor;
		}
	}
}