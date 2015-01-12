using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MP.PlenoBDNE.AppWin.Dados.Base;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoSQL.AppWin
{
	public static class LinhaDeComando
	{
		public static Int32 Executar(Param par)
		{
			try
			{
				var erros = 0;
				var tipo = BancoDeDadosExtension.ListaDeBancoDeDados.FirstOrDefault(b => b.Key.StartsWith(par.Rdb));
				IBancoDeDados acesso = Activator.CreateInstance(tipo.Value) as IBancoDeDados;
				acesso.ConfigurarConexao(par.Srv, par.Bco, par.Usr, par.Pwd);
				var cmdSQLs = par.Comandos.ToList();

				foreach (var cmdSQL in cmdSQLs)
				{
					try
					{
						var result = acesso.Executar(cmdSQL.Replace(";", ""));
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
				return -1;
			}
		}

		internal static String Get(this String[] args, String parametro, String padrao)
		{
			var par = (args == null) ? null : args.FirstOrDefault(a => a.ToUpper().StartsWith(parametro.ToUpper()));
			return String.IsNullOrWhiteSpace(par) || par.Length <= parametro.Length ? padrao : par.Replace("\"", "").Substring(parametro.Length).Replace("\"", "");
		}
	}

	public class Param
	{
		public readonly String Rdb;
		public readonly String Srv;
		public readonly String Usr;
		public readonly String Pwd;
		public readonly String Bco;

		public readonly Boolean EhLinhaDeComandos;
		private readonly String Dir;
		private readonly String Cmd;

		private Param()
		{
			Rdb = null;
			Srv = null;
			Usr = null;
			Pwd = null;
			Bco = null;
			Dir = null;
			Cmd = null;
			EhLinhaDeComandos = false;
		}

		public Param(String[] args)
		{
			var p = Load(args.Get("-Dir=", null), args.Get("-Cfg=", null));
			Rdb = args.Get("-Rdb=", null) ?? p.Rdb;
			Srv = args.Get("-Srv=", null) ?? p.Srv;
			Usr = args.Get("-Usr=", null) ?? p.Usr;
			Pwd = args.Get("-Pwd=", null) ?? p.Pwd;
			Bco = args.Get("-Bco=", null) ?? p.Bco;
			Dir = args.Get("-Dir=", null) ?? p.Dir ?? @"C:\Scripts\";
			Cmd = args.Get("-Cmd=", null) ?? p.Cmd ?? "DIR";
			EhLinhaDeComandos = !String.IsNullOrWhiteSpace(Rdb)
				&& !String.IsNullOrWhiteSpace(Srv)
				&& !String.IsNullOrWhiteSpace(Usr)
				&& !String.IsNullOrWhiteSpace(Pwd)
				&& !String.IsNullOrWhiteSpace(Bco);
		}

		public IEnumerable<String> Comandos
		{
			get
			{
				if (Cmd.ToUpper().StartsWith("ALLSQL"))
				{
					var files = ArquivosDoDiretorio(Dir);
					return ObterComandosDosArquivos(files);
				}
				else if (Cmd.ToUpper().StartsWith("LISTA:"))
				{
					var files = ArquivosDoArquivo(Path.Combine(Dir, Cmd.Substring(6)));
					return ObterComandosDosArquivos(files);
				}
				else
					return Enumerar(Cmd);
			}
		}


		private IEnumerable<String> ArquivosDoDiretorio(String diretorio)
		{
			return Directory.EnumerateFiles(diretorio, "*.sql", SearchOption.AllDirectories).OrderBy(f => new FileInfo(f).Name);
		}

		private IEnumerable<String> ArquivosDoArquivo(String arquivo)
		{
			return File.ReadAllLines(arquivo);
		}

		private IEnumerable<String> ObterComandosDosArquivos(IEnumerable<String> files)
		{
			var comandos = new List<String>();
			foreach (var arquivo in files)
				comandos.AddRange(ObterComandosDoArquivo(arquivo));
			return comandos;
		}

		private IEnumerable<String> ObterComandosDoArquivo(String arquivo)
		{
			yield return File.ReadAllText(arquivo);
		}

		private IEnumerable<String> Enumerar(String comando)
		{
			yield return comando;
		}

		private Param Load(String diretorio, String arquivo)
		{
			if (String.IsNullOrWhiteSpace(arquivo))
				return new Param();
			else if (File.Exists(arquivo))
				return new Param(File.ReadAllLines(arquivo));
			else if (String.IsNullOrWhiteSpace(diretorio))
				return new Param();
			else if (File.Exists(Path.Combine(diretorio, arquivo)))
				return new Param(File.ReadAllLines(Path.Combine(diretorio, arquivo)));
			else
				return new Param();
		}
	}
}