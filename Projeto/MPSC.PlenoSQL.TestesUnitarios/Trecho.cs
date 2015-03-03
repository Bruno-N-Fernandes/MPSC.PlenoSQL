using System;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	public static class Strings
	{
		public const Char CR = '\r';
		public const Char LF = '\n';
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
						retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial - 2);
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
	}
}