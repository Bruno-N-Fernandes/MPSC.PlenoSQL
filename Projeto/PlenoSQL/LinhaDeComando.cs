using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MP.PlenoBDNE.AppWin.Dados.Base;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoSQL.AppWin
{
	public class LinhaDeComando
	{
		private readonly Parametro _parametro;
		public Boolean PodeSerExecutada { get { return _parametro.EhValido; } }

		public LinhaDeComando(String[] args) { _parametro = new Parametro(args); }

		public Int32 Executar()
		{
			try
			{
				var erros = 0;
				var tipo = BancoDeDadosExtension.ListaDeBancoDeDados.FirstOrDefault(b => b.Key.StartsWith(_parametro.Rdb));
				var banco = Activator.CreateInstance(tipo.Value) as IBancoDeDados;
				banco.ConfigurarConexao(_parametro.Srv, _parametro.Bco, _parametro.Usr, _parametro.Pwd);

				foreach (var cmdSQL in ObterListaDeComandos(_parametro))
				{
					try
					{
						var result = banco.Executar(cmdSQL.Replace(";", ""));
						Console.WriteLine(result);
					}
					catch (Exception)
					{
						erros++;
					}
				}
				return -erros;
			}
			catch (Exception)
			{
				return Int32.MinValue;
			}
		}

		private List<String> ObterListaDeComandos(Parametro parametro)
		{
			var retorno = new List<String>();

			var arquivos = ObterListaDeArquivosDoParametro(parametro);
			if (arquivos != null)
				retorno.AddRange(ObterListaDeComandosDaListaDeArquivos(arquivos));
			else
				retorno.Add(parametro.Cmd);

			return retorno;
		}

		private IEnumerable<String> ObterListaDeArquivosDoParametro(Parametro parametro)
		{
			IEnumerable<String> arquivos = null;
			if (parametro.Cmd.ToUpper().StartsWith("ALLSQL"))
				arquivos = ObterListaDeArquivosDoDiretorio(parametro.Dir);
			else if (parametro.Cmd.ToUpper().StartsWith("LISTA:"))
				arquivos = ObterListaDeArquivosDoArquivo(Path.Combine(parametro.Dir, parametro.Cmd.Substring(6)));
			return arquivos;
		}

		private IEnumerable<String> ObterListaDeArquivosDoDiretorio(String diretorio)
		{
			return Directory.EnumerateFiles(diretorio, "*.sql", SearchOption.AllDirectories).OrderBy(f => new FileInfo(f).Name);
		}

		private IEnumerable<String> ObterListaDeArquivosDoArquivo(String arquivo)
		{
			return File.ReadAllLines(arquivo);
		}

		private IEnumerable<String> ObterListaDeComandosDaListaDeArquivos(IEnumerable<String> arquivos)
		{
			var comandos = new List<String>();
			foreach (var arquivo in arquivos)
				comandos.AddRange(ObterListaDeComandosDoArquivo(arquivo));
			return comandos;
		}

		private IEnumerable<String> ObterListaDeComandosDoArquivo(String arquivo)
		{
			yield return File.ReadAllText(arquivo);
		}
	}


	public class Parametro
	{
		public readonly String Rdb;
		public readonly String Srv;
		public readonly String Usr;
		public readonly String Pwd;
		public readonly String Bco;

		public readonly Boolean EhValido;
		public readonly String Dir;
		public readonly String Cmd;

		public Parametro(String[] parametros) : this(parametros, Factory(parametros)) { }

		private Parametro(String[] parametros, Parametro parametro)
		{
			Rdb = parametros.Get("-Rdb=", (parametro ?? this).Rdb);
			Srv = parametros.Get("-Srv=", (parametro ?? this).Srv);
			Usr = parametros.Get("-Usr=", (parametro ?? this).Usr);
			Pwd = parametros.Get("-Pwd=", (parametro ?? this).Pwd);
			Bco = parametros.Get("-Bco=", (parametro ?? this).Bco);
			Dir = parametros.Get("-Dir=", (parametro ?? this).Dir) ?? @"C:\Scripts\";
			Cmd = parametros.Get("-Cmd=", (parametro ?? this).Cmd) ?? "ALLSQL";
			EhValido = !String.IsNullOrWhiteSpace(Rdb)
				&& !String.IsNullOrWhiteSpace(Srv)
				&& !String.IsNullOrWhiteSpace(Usr)
				&& !String.IsNullOrWhiteSpace(Pwd)
				&& !String.IsNullOrWhiteSpace(Bco);
		}

		private static Parametro Factory(String[] parametros)
		{
			String dir = parametros.Get("-Dir=", @"C:\Scripts\");
			String arq = parametros.Get("-Cfg=", @"PlenoSql.Cfg");
			if (String.IsNullOrWhiteSpace(arq))
				return new Parametro(null, null);
			else if (File.Exists(arq))
				return new Parametro(File.ReadAllLines(arq), null);
			else if (String.IsNullOrWhiteSpace(dir))
				return new Parametro(null, null);
			else if (File.Exists(Path.Combine(dir, arq)))
				return new Parametro(File.ReadAllLines(Path.Combine(dir, arq)), null);
			else
				return new Parametro(null, null);
		}
	}

	public static class ParamExtension
	{
		internal static String Get(this String[] args, String parametro, String padrao)
		{
			var par = (args == null) ? null : args.FirstOrDefault(a => a.ToUpper().StartsWith(parametro.ToUpper()));
			return String.IsNullOrWhiteSpace(par) || (par.Length <= parametro.Length) ? padrao : par.Replace("\"", "").Substring(parametro.Length);
		}
	}
}