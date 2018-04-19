using MPSC.PlenoSQL.Kernel.Infra;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

namespace MPSC.PlenoSQL.Kernel.Dados.Base
{
	public class Cache
	{
		public static readonly String cRootPath = @".\Config\";
		private readonly List<Tabela> _tabelas = new List<Tabela>();
		private readonly static List<String> _dicionario = new List<String>();

		public static readonly String arquivoConfig3 = cRootPath + "PlenoSQL.dic";
		public static readonly String dicFile = FileUtil.FileToArray(arquivoConfig3, 1).FirstOrDefault();

		public Cache() { _tabelas.Add(new Tabela()); }
		public Cache(IDataReader dataReader)
		{
			Open();
			var thread = new Thread(() => { processar(dataReader); });
			thread.SetApartmentState(ApartmentState.STA);
			thread.Priority = ThreadPriority.Highest;
			thread.Start();
		}

		private void processar(IDataReader dataReader)
		{
			var tabelas = new List<Tabela>();
			while (BancoDados._isOpen && dataReader.IsOpen() && dataReader.Read())
			{
				var tabela = tabelas.FirstOrDefault(t => t.ConfirmarNomeInterno(dataReader));
				if (tabela == null)
				{
					tabela = new Tabela(dataReader);
					tabelas.Add(tabela);
				}
				tabela.Adicionar(dataReader);
			}
			_tabelas.RemoveAll(i => true);
			_tabelas.AddRange(tabelas);
			Save(tabelas);
		}

		private void Save(List<Tabela> tabelas)
		{
			var cache = tabelas.OrderBy(t => t.NomeTabela).Serializar();
			File.WriteAllText(cRootPath + "CacheTabelas.txt", cache);
		}

		public void Open()
		{
			if (File.Exists(dicFile))
			{
				_dicionario.Clear();
				_dicionario.AddRange(File.ReadAllLines(dicFile).OrderBy(d => d.Length).ThenBy(d => d).Distinct());
			}
			var lista = File.Exists(cRootPath + "CacheTabelas.txt") ? File.ReadAllLines(cRootPath + "CacheTabelas.txt").ToList() : new List<String>();
			var tabelas = Tabela.Load(lista).ToArray();
			_tabelas.RemoveAll(i => true);
			_tabelas.AddRange(tabelas);
		}

		public IEnumerable<String> Tabelas(String nome, Boolean comDetalhes)
		{
			return _tabelas.Where(t => t.TipoTabela.Contains("T") && t.ConfirmarNome(nome, true)).Select(t => t.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Views(String nome, Boolean comDetalhes)
		{
			return _tabelas.Where(v => v.TipoTabela.Contains("V") && v.ConfirmarNome(nome, true)).Select(v => v.ObterNome(comDetalhes));
		}

		public IEnumerable<String> Colunas(String parent, String filtro, Boolean comDetalhes)
		{
			return _tabelas
				.Where(t => t.ConfirmarNome(parent, false))
				.SelectMany(t => t.Colunas
					.Where(c => (filtro == null) || c.NomeColuna.ToUpper().Contains(filtro.ToUpper()))
					.Select(c => c.ObterNome(comDetalhes))
				);
		}

		public static String Traduzir(String valor)
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

		public class Tabela : ISerializavel
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

			public String Serializar()
			{
				return String.Format("{0}-*-{1}-*-{2}-*-{3}\r\n{4}", Colunas.Count(), TipoTabela, NomeTabela, NomeInternoTabela, Colunas.Serializar());
			}
			internal Tabela(String[] campo, List<String> lista)
			{
				TipoTabela = campo[1];
				NomeTabela = campo[2];
				NomeInternoTabela = campo[3];
				_colunas.AddRange(Coluna.Load(lista));
			}

			public static IEnumerable<Tabela> Load(List<String> lista)
			{
				while (lista.Count > 0)
				{
					var linha = lista[0];
					lista.RemoveAt(0);
					var campo = linha.Split(new[] { "-*-" }, StringSplitOptions.RemoveEmptyEntries);
					var qtd = Convert.ToInt32(campo[0]);
					yield return new Tabela(campo, lista.Take(qtd).ToList());
					lista.RemoveRange(0, qtd);
				}
			}
		}

		public class Coluna : ISerializavel
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
			public String Serializar()
			{
				return String.Format("{0}-*-{1}", NomeColuna, DetalhesColuna);
			}
			internal Coluna(String linha)
			{
				var campo = linha.Split(new[] { "-*-" }, StringSplitOptions.RemoveEmptyEntries);
				NomeColuna = campo[0];
				DetalhesColuna = campo[1];
			}

			public static IEnumerable<Coluna> Load(List<String> lista)
			{
				while (lista.Count > 0)
				{
					var linha = lista[0];
					lista.RemoveAt(0);
					yield return new Coluna(linha);
				}
			}
		}
	}

	internal interface ISerializavel
	{
		String Serializar();
	}
	internal static class Serializavel
	{
		internal static String Serializar<T>(this IEnumerable<T> lista) where T : ISerializavel
		{
			return String.Join("\r\n", lista.Select(t => t.Serializar()));
		}
	}
}