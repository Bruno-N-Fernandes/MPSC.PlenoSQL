using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.GestorDeAplicacao.PoC;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace MPSC.PlenoSQL.Kernel.Infra
{
	public class LinhaDeComando
	{
		private readonly Parametro _parametro;
		public Boolean PodeSerExecutada { get { return _parametro.EhValido; } }

		public LinhaDeComando(String[] args) { _parametro = new Parametro(args); }

		public void MostrarCopyRight()
		{
			try
			{
				GuiRedirect.Redirect();
				Console.WriteLine("PlenoSql - Versão " + CoreAssembly.VersionString);
				Console.WriteLine("Mercado Pleno Soluções em Computação Ltda M.E.");
				Console.WriteLine("http://www.mercadopleno.com.br");
				Console.WriteLine("\r\n");
			}
			catch (Exception) { }
		}

		public Int32 Executar()
		{
			MostrarCopyRight();
			try
			{
				var erros = 0;
				var tipo = BancoDeDadosExtension.ListaDeBancoDeDados.FirstOrDefault(b => b.Key.StartsWith(_parametro.Rdb));
				var banco = Activator.CreateInstance(tipo.Value) as IBancoDeDados;
				banco.ConfigurarConexao(_parametro.Srv, _parametro.Bco, _parametro.Usr, _parametro.Pwd);

				foreach (var cmdSql in ObterListaDeComandos(_parametro))
				{
					try
					{
						var query = cmdSql.AllTrim().Replace(";", "").AllTrim();
						var inicio = DateTime.Now;
						var result = banco.Executar(query);
						Console.WriteLine("#{0:###,###,###,###,##0} linhas afetadas em {1} milissegundos pela Query:\r\n{2};", Convert.ToInt64("0" + Convert.ToString(result)), (DateTime.Now - inicio).TotalMilliseconds, query);
						Console.WriteLine("/* = * = * = * = * = * = * = * = * = * = * = * = * = * = * = * = * = * = * = */\r\n\r\n");
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
				retorno.AddRange(ObterListaDeComandosDaListaDeArquivos(arquivos, parametro.Brk));
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

		private IEnumerable<String> ObterListaDeComandosDaListaDeArquivos(IEnumerable<String> arquivos, String separadorLotesDeComando)
		{
			var comandos = new List<String>();
			foreach (var arquivo in arquivos)
				comandos.AddRange(ObterListaDeComandosDoArquivo(arquivo, separadorLotesDeComando));
			return comandos;
		}

		private IEnumerable<String> ObterListaDeComandosDoArquivo(String arquivo, String separadorLotesDeComando)
		{
			var script = File.ReadAllText(arquivo);
			return script.Split(new String[] { separadorLotesDeComando }, StringSplitOptions.RemoveEmptyEntries);
		}

		private class Parametro
		{
			internal readonly String Rdb;
			internal readonly String Srv;
			internal readonly String Usr;
			internal readonly String Pwd;
			internal readonly String Bco;
			internal readonly String Brk;

			internal readonly Boolean EhValido;
			internal readonly String Dir;
			internal readonly String Cmd;

			internal Parametro(String[] parametros) : this(parametros, Factory(parametros)) { }

			private Parametro(String[] parametros, Parametro parametro)
			{
				Rdb = parametros.Get("-Rdb=", (parametro ?? this).Rdb);
				Srv = parametros.Get("-Srv=", (parametro ?? this).Srv);
				Usr = parametros.Get("-Usr=", (parametro ?? this).Usr);
				Pwd = parametros.Get("-Pwd=", (parametro ?? this).Pwd);
				Bco = parametros.Get("-Bco=", (parametro ?? this).Bco);
				Dir = parametros.Get("-Dir=", (parametro ?? this).Dir) ?? @"C:\Scripts\";
				Cmd = parametros.Get("-Cmd=", (parametro ?? this).Cmd) ?? "ALLSQL";
				Brk = parametros.Get("-Brk=", (parametro ?? this).Brk) ?? ";";
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
	}
}