using System;

namespace MPSC.PlenoSQL.TestesUnitarios.MDA
{
	public class Arvore<Tipo> where Tipo : IComparable
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




namespace TPArquitetura
{
	/// <summary>
	/// http://www.ebah.com.br/content/ABAAAfbkwAE/arvore-generica-implementada-c-sharp
	/// https://pt.scribd.com/doc/70962949/Apostila-de-Estrutura-de-Dados-C-atualizada
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			ArvoreG RAIZ = null;
			ArvoreG arv;
			int escolha, count;
			string nomepai, nomefilho;
			count = 0;

			do
			{
				Console.Clear();
				Console.WriteLine(" Menu Principal");
				Console.WriteLine("(1) - Insere um elemento na Árvore");
				Console.WriteLine("(2) - Lista a Árvore ");
				Console.WriteLine("(3) - Para SAIR");
				escolha = int.Parse(Console.ReadKey(true).KeyChar.ToString());

				switch (escolha)
				{
					case 1: // Insere um elemento na Arvore
						Console.Clear();
						arv = new ArvoreG();
						Console.Write("Entre com o nome do pai : ");
						nomepai = Console.ReadLine();

						if (RAIZ == null)
						{
							arv.Grava(nomepai, ref RAIZ);
						}
						else
						{
							Console.Write("Entre com o nome do filho: ");
							nomefilho = Console.ReadLine();
							arv.Grava(nomefilho, ref RAIZ);
						}
						count++;

						if (count > 1)
							RAIZ.Insere(nomepai, arv);
						break;

					case 2: // Lista Arvore
						RAIZ.MostraArvore(1, ref RAIZ);
						Console.ReadKey();
						break;
				}
			} while (escolha != 3);
		}

		class ArvoreG
		{
			private string info;
			ArvoreG filho;
			ArvoreG irmao;
			public ArvoreG() // Construtor 
			{
				info = "";
				filho = null;
				irmao = null;
			}

			public void Grava(string nome, ref ArvoreG RAIZ)
			{
				this.info = nome;
				if (RAIZ == null)
					RAIZ = this;
			}


			public void Insere(string nomepai, ArvoreG subarv)
			{
				ArvoreG temp;
				if (this.info == nomepai)
				{
					if (this.filho == null)
						this.filho = subarv;
					else
					{
						temp = this.filho;
						while (temp != null)
						{
							if (temp.irmao == null)
							{
								temp.irmao = subarv;
								temp = null;
							}
							else
								temp = temp.irmao;
						}
					}
				}
				else
				{
					if (this.irmao != null)
						this.irmao.Insere(nomepai, subarv);
					if (this.filho != null)
						this.filho.Insere(nomepai, subarv);
				}
			}

			public void MostraArvore(int Depth, ref ArvoreG Node)
			{
				string Branch = string.Empty;
				Depth = 0;
				//else 			{ 
				for (int i = 0; i < Depth - 5; i++)
					Branch += " ";

				Console.WriteLine(Branch + Node.info.ToString());
				if (Node.filho != null)
					Node.filho.MostraArvore(Depth + 5, ref Node.filho);

				if (Node.irmao != null)
					Node.irmao.MostraArvore(Depth, ref Node.irmao);

				Console.Write("Arvore Genérica excluída da Memória");
			}
		}
	}
}