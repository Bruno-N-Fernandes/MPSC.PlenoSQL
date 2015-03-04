using System;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	public static class Strings
	{
		public const Char CR = '\r';
		public const Char LF = '\n';
		public const Char TB = '\t';
		public const Char PL = '\'';
		public const Char SPC = ' ';
		public static readonly String BREAK = new String(new Char[] { CR, LF, TB, PL, SPC, '"', '(', ')', '[', ']', '{', '}', '/', '*', '+', '-', ',', ';' });
		public static readonly Char[] ENTER = { CR, LF };
	}

	public class Trecho
	{
		private readonly String _sql;
		private readonly Int32 _posicao;

		public Trecho(String sql, Int32 posicao)
		{
			_sql = sql;
			_posicao = posicao;
		}

		public String LinhaAnterior
		{
			get
			{
				var retorno = String.Empty;
				var posicaoFinal = _sql.LastIndexOfAny(Strings.ENTER, _posicao) - 2;
				if (posicaoFinal >= 0)
				{
					var posicaoInicial = _sql.LastIndexOfAny(Strings.ENTER, posicaoFinal);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial);
				}

				return retorno;
			}
		}

		public String LinhaAtual
		{
			get
			{
				var retorno = String.Empty;
				var posicaoInicial = _sql.LastIndexOfAny(Strings.ENTER, _posicao);
				if (posicaoInicial >= 0)
				{
					var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 1);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial - 1);
				}

				return retorno;
			}
		}

		public String LinhaPosterior
		{
			get
			{
				var retorno = String.Empty;
				var posicaoInicial = _sql.IndexOfAny(Strings.ENTER, _posicao);
				if (posicaoInicial >= 0)
				{
					var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 2);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 2, posicaoFinal - posicaoInicial - 2);
				}

				return retorno;
			}
		}

		public String CaracterAtual { get { return _sql.Substring(_posicao, 1); } }

		public Tokens Token { get { return new Tokens(_sql, _posicao); } }


		public class Tokens
		{
			public readonly String First;
			public readonly String Completo;
			public readonly String Parcial;

			public Tokens(String sql, Int32 posicao)
			{
				var posicaoInicial = ObterPosicao(sql, posicao, -1);
				if ((posicaoInicial >= 0) && (posicaoInicial < posicao))
				{
					Parcial = sql.Substring(posicaoInicial, posicao - posicaoInicial + 1);
					var posicaoFinal = ObterPosicao(sql, posicao, +1);
					if (posicaoFinal > posicaoInicial)
						Completo = sql.Substring(posicaoInicial, posicaoFinal - posicaoInicial + 1);
					else
						Completo = Parcial;
				}
				else
				{
					Parcial = String.Empty;
					Completo = String.Empty;
				}
				var posicaoPonto = Completo.IndexOf(".");
				First = (posicaoPonto > 0) ? Completo.Substring(0, posicaoPonto) : Completo;

			}

			private int ObterPosicao(String sql, Int32 posicao, Int32 controle)
			{
				while (!IsToken(sql, posicao, controle))
					posicao += controle;
				return posicao;
			}

			private Boolean IsToken(String sql, Int32 posicao, Int32 controle)
			{
				var retorno = (posicao >= 0) && (posicao < sql.Length);
				retorno &= !IsBreakToken(sql, posicao, controle);
				retorno &= IsBreakToken(sql, posicao + controle, controle);
				return retorno;
			}

			private Boolean IsBreakToken(String sql, Int32 posicao, Int32 controle)
			{
				var retorno = (posicao < 0) || (posicao >= sql.Length);
				retorno |= Strings.BREAK.Contains(sql[posicao].ToString()) && !IsBreakToken(sql, posicao + controle, controle);
				return retorno;
			}


		}
	}
}