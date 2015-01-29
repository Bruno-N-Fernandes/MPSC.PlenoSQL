using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.Dados.Base
{
	public class Cache
	{
		private readonly List<Tabela> _tabelas;

		public Cache(IDataReader dataReader)
		{
			_tabelas = new List<Tabela>();
			processar(dataReader);
		}

		private void processar(IDataReader dataReader)
		{
			while (dataReader.IsOpen() && dataReader.Read())
			{
				var tabela = _tabelas.FirstOrDefault(t => t.ConfirmarNomeInterno(dataReader));
				if (tabela == null)
				{
					tabela = new Tabela(dataReader);
					_tabelas.Add(tabela);
				}
				tabela.Adicionar(dataReader);
			}
		}

		public IEnumerable<String> Tabelas(String nome, Boolean comDetalhes)
		{
			return _tabelas.Where(t => t.TipoTabela == "T" && t.ConfirmarNome(nome, true)).Select(t => t.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Views(String nome, Boolean comDetalhes)
		{
			return _tabelas.Where(v => v.TipoTabela == "V" && v.ConfirmarNome(nome, true)).Select(v => v.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Colunas(String parent, Boolean comDetalhes)
		{
			return _tabelas.Where(t => t.ConfirmarNome(parent, false)).SelectMany(t => t.Colunas.Select(c => c.ObterNome(comDetalhes)));
		}



		public class Tabela
		{
			private readonly List<Coluna> _colunas;
			public readonly String TipoTabela;
			public readonly String NomeTabela;
			public readonly String NomeInternoTabela;
			public readonly String DescricaoTabela;
			public IEnumerable<Coluna> Colunas { get { return _colunas; } }

			public Tabela(IDataReader dataReader)
			{
				TipoTabela = Convert.ToString(dataReader["TipoTabela"]).Trim();
				NomeTabela = Convert.ToString(dataReader["NomeTabela"]).Trim();
				NomeInternoTabela = Convert.ToString(dataReader["NomeInternoTabela"]).Trim();
				DescricaoTabela = Convert.ToString(dataReader["DescricaoTabela"]).Replace("\r", " ").Replace("\n", " ").Replace("\\", " ").Replace("/", " ").Trim();
				_colunas = new List<Coluna>();
			}

			internal Boolean ConfirmarNomeInterno(IDataReader dataReader)
			{
				return NomeInternoTabela.ToUpper().Equals(Convert.ToString(dataReader["NomeInternoTabela"]).Trim().ToUpper());
			}

			internal Boolean ConfirmarNome(String nome, Boolean parcial)
			{
				return String.IsNullOrWhiteSpace(nome) || NomeTabela.ToUpper().Equals(nome.Trim().ToUpper()) || (parcial && NomeTabela.ToUpper().Contains(nome.Trim().ToUpper()));
			}

			internal void Adicionar(IDataReader dataReader)
			{
				_colunas.Add(new Coluna(dataReader));
			}

			internal String ObterNome(Boolean comDetalhes)
			{
				return NomeTabela + (comDetalhes ? String.Format("({0}: {1})", NomeInternoTabela, DescricaoTabela) : String.Empty);
			}
		}

		public class Coluna
		{
			public readonly String NomeColuna;
			public readonly String DetalhesColuna;
			public Coluna(IDataReader dataReader)
			{
				NomeColuna = Convert.ToString(dataReader["NomeColuna"]);
				DetalhesColuna = Convert.ToString(dataReader["DetalhesColuna"]);
			}

			internal String ObterNome(Boolean comDetalhes)
			{
				return NomeColuna + (comDetalhes ? String.Format(" ({0})", DetalhesColuna) : String.Empty);
			}
		}
	}
}