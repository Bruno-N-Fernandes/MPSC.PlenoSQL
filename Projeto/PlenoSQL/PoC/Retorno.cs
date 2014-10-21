using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MP.PlenoSQL.AppWin.PoC
{
	public class Mensagem
	{
		public enum Tipo { }
		private readonly Tipo _tipo;
		private readonly String _mensagem;

		public Tipo Tipo { get { return _tipo; } }
		public String Mensagem { get { return _mensagem; } }
		public Mensagem(String mensagem, Tipo tipo)
		{
			_tipo = tipo;
			_mensagem = mensagem;
		}
	}
	public class Retorno<TDados>
	{
		protected internal readonly List<Mensagem> mensagens = new List<Mensagem>();
		protected internal Boolean result = true;
		protected internal TDados Dados { get; set; }

		public Retorno() : this(default(TDados)) { }
		public Retorno(TDados dados)
		{
			this.Dados = dados;
		}
		public Retorno(Retorno<TDados> self)
		{
			mensagens.AddRange(self.mensagens);			
			Dados = self.Dados;
		}



		public static implicit operator Retorno<TDados>(TDados dados)
		{
			return new Retorno<TDados>(dados);
		}
		public static implicit operator TDados(Retorno<TDados> self)
		{
			return self.Dados;
		}
		public static explicit operator Boolean(Retorno<TDados> self)
		{
			return self.result;
		}
	}

	public class RetornoXML : Retorno<String>
	{
		public Retorno<String> Join
		{
			set
			{
				Dados += value.Dados;
				mensagens.AddRange(value.mensagens);
			}
		}
	}

	public class Teste
	{
		public Boolean Processar()
		{
			var retorno = Processar1();
			retorno.Join = Processar2();
			retorno.Join = Processar3();

			return (Boolean)retorno;
		}

		public RetornoXML Processar1()
		{
			return new RetornoXML();
		}
		public RetornoXML Processar2()
		{
			return new RetornoXML();
		}
		public RetornoXML Processar3()
		{
			return new RetornoXML();
		}

	}
}