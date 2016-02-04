using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using MPSC.PlenoSQL.Kernel.Infra;

namespace MPSC.PlenoSQL.Kernel.Dados.Base
{
	public class Cache
	{
		private readonly List<Tabela> _tabelas = new List<Tabela>();
		private readonly static List<String> _dicionario = new List<String>();

		public static readonly String arquivoConfig3 = Path.GetTempPath() + "PlenoSQL.dic";
		public static readonly String dicFile = FileUtil.FileToArray(arquivoConfig3, 1).FirstOrDefault();

		public Cache() { _tabelas.Add(new Tabela()); }
		public Cache(IDataReader dataReader)
		{
			if (File.Exists(dicFile))
			{
				_dicionario.Clear();
				_dicionario.AddRange(File.ReadAllLines(dicFile).OrderBy(d => d.Length).ThenBy(d => d).Distinct());
			}

			var thread = new Thread(() => { processar(dataReader); });
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		private void processar(IDataReader dataReader)
		{
			while (BancoDados._isOpen && dataReader.IsOpen() && dataReader.Read())
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
			return _tabelas.Where(t => t.TipoTabela.Contains("T") && t.ConfirmarNome(nome, true)).Select(t => t.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Views(String nome, Boolean comDetalhes)
		{
			return _tabelas.Where(v => v.TipoTabela.Contains("V") && v.ConfirmarNome(nome, true)).Select(v => v.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Colunas(String parent, Boolean comDetalhes)
		{
			return _tabelas.Where(t => t.ConfirmarNome(parent, false)).SelectMany(t => t.Colunas.Select(c => c.ObterNome(comDetalhes)));
		}

		private static String Traduzir(String valor)
		{
			valor = valor.Trim();
			var tamanhoMaximo = valor.Length;
			foreach (var palavra in _dicionario.Where(p => p.Length <= tamanhoMaximo))
			{
				var inicio = valor.IndexOf(palavra, StringComparison.InvariantCultureIgnoreCase);
				if (inicio >= 0)
					valor = valor.Replace(valor.Substring(inicio, palavra.Length), palavra);
			}
			return valor;
		}

		public class Tabela
		{
			private readonly List<Coluna> _colunas = new List<Coluna>();
			public readonly String TipoTabela;
			public readonly String NomeTabela;
			public readonly String NomeInternoTabela;
			public IEnumerable<Coluna> Colunas { get { return _colunas; } }

			public Tabela()
			{
				TipoTabela = "TV";
				NomeTabela = "Aguarde. Pesquisando Informações!";
				NomeInternoTabela = "Atualizando o Cache!";
				_colunas.Add(new Coluna());
			}

			public Tabela(IDataReader dataReader)
			{
				TipoTabela = Convert.ToString(dataReader["TipoTabela"]).Trim();
				NomeTabela = Traduzir(Convert.ToString(dataReader["NomeTabela"]));
				NomeInternoTabela = Traduzir(Convert.ToString(dataReader["NomeInternoTabela"]));
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
				return NomeTabela + (comDetalhes && !String.IsNullOrWhiteSpace(NomeInternoTabela) ? String.Format(" ({0})", NomeInternoTabela) : String.Empty);
			}
		}

		public class Coluna
		{
			public readonly String NomeColuna;
			public readonly String DetalhesColuna;
			public Coluna()
			{
				NomeColuna = "Pesquisando Informações";
				DetalhesColuna = "Atualizando o Cache!";
			}
			public Coluna(IDataReader dataReader)
			{
				NomeColuna = Traduzir(Convert.ToString(dataReader["NomeColuna"]));
				DetalhesColuna = Traduzir(Convert.ToString(dataReader["DetalhesColuna"]));
			}

			internal String ObterNome(Boolean comDetalhes)
			{
				return NomeColuna + (comDetalhes && !String.IsNullOrWhiteSpace(DetalhesColuna) ? String.Format(" ({0})", DetalhesColuna) : String.Empty);
			}
		}
	}
}