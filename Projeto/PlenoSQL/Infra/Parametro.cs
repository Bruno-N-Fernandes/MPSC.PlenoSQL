using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using MPSC.PlenoBDNE.AppWin.Dados.Base;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public class Parametro
	{
		private static Parametro _instanciaUnica;
		private static readonly String strConexao = String.Format(@"Data Source={0};Version=3;", Path.Combine(Path.GetTempPath(), "PlenoSQL.db"));
		private IList<Conexao> _conexoes;
		public IEnumerable<Conexao> Conexoes
		{
			get { return _conexoes ?? (_conexoes = LoadConexoes().ToList()); }
		}

		private Parametro()
		{
			if (!ExisteTabela("Configuracao"))
				ExecuteNonQuery(cmdSql.CreateTableConfiguracao);

			if (!ExisteTabela("Conexao"))
				ExecuteNonQuery(cmdSql.CreateTableConexao);
		}

		public Parametro NovaConexao(Int32 tipoBanco, String servidor, String usuario, String senha, String banco, Boolean salvaSenha)
		{
			var conexao = new Conexao(0, tipoBanco, servidor, usuario, senha, banco, 0, salvaSenha);
			var existente = Conexoes
				.Where(c => c.TipoBanco == conexao.TipoBanco)
				.Where(c => c.Servidor.ToUpper() == conexao.Servidor.ToUpper())
				.Where(c => c.Usuario.ToUpper() == conexao.Usuario.ToUpper())
				.Where(c => c.Senha.ToUpper() == conexao.Senha.ToUpper())
				.Where(c => c.Banco.ToUpper() == conexao.Banco.ToUpper())
				.FirstOrDefault();

			if (existente != null)
				existente.Configurar(0, salvaSenha);
			else
				_conexoes.Add(conexao);
			return this;
		}

		public void Save()
		{
			ExecuteNonQuery(cmdSql.DeleteFromConexao);
			var iDbConnection = new SQLiteConnection(strConexao);
			var conexoes = Conexoes.OrderBy(c => c.Ordem);
			var ordem = 1;
			foreach (var conexao in conexoes)
			{
				var iDbCommand = iDbConnection.CriarComando(cmdSql.InsertIntoConexao);
				iDbCommand.AdicionarParametro("@tipoBanco", conexao.TipoBanco, DbType.Int16);
				iDbCommand.AdicionarParametro("@servidor", conexao.Servidor, DbType.String);
				iDbCommand.AdicionarParametro("@usuario", conexao.Usuario, DbType.String);
				iDbCommand.AdicionarParametro("@senha", Conexao.Cripto(conexao.Senha), DbType.String);
				iDbCommand.AdicionarParametro("@banco", conexao.Banco, DbType.String);
				iDbCommand.AdicionarParametro("@salvarSenha", conexao.SalvarSenha, DbType.Boolean);
				iDbCommand.AdicionarParametro("@ordem", ordem++, DbType.Int16);
				iDbCommand.ExecuteNonQuery();
				iDbCommand.Dispose();
			}
			iDbConnection.Close();
			iDbConnection.Dispose();
			_conexoes = LoadConexoes().ToList();
		}

		private Object ObterValorConfiguracao(String chave)
		{
			return ExecuteScalar(String.Format(cmdSql.SelectFromConfiguracao, chave)) ?? String.Empty;
		}

		private Int32 GravarValorConfiguracao(String chave, Object value)
		{
			var qtd = ExecuteNonQuery(String.Format(cmdSql.UpdateSetConfiguracao, Convert.ToString(value), chave));
			if (qtd == 0)
				qtd = ExecuteNonQuery(String.Format(cmdSql.InsertIntoConfiguracao, Convert.ToString(value), chave));
			return qtd;
		}

		private Int32 ExecuteNonQuery(String cmdSql)
		{
			return new SQLiteConnection(strConexao).ExecuteNonQuery(cmdSql);
		}

		private Object ExecuteScalar(String cmdSql)
		{
			return new SQLiteConnection(strConexao).ExecuteScalar(cmdSql);
		}

		private Boolean ExisteTabela(String nomeTabela)
		{
			return nomeTabela.Equals(ExecuteScalar(String.Format(cmdSql.ExisteTabela, nomeTabela)));
		}

		private IEnumerable<Conexao> LoadConexoes()
		{
			var iDbConnection = new SQLiteConnection(strConexao);
			var iDbCommand = iDbConnection.CriarComando(cmdSql.SelectFromConexao);
			var iDataReader = iDbCommand.ExecuteReader();
			while (iDataReader.Read())
				yield return new Conexao(iDataReader.GetInt32("Id"), iDataReader.GetInt16("TipoBanco"), iDataReader.GetString("Servidor"), iDataReader.GetString("Usuario"), Conexao.Cripto(iDataReader.GetString("Senha")), iDataReader.GetString("Banco"), iDataReader.GetInt16("Ordem"), iDataReader.GetBoolean("SalvarSenha"));
			iDataReader.Close();
			iDataReader.Dispose();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
		}

		public static Parametro Instancia
		{
			get { return (_instanciaUnica ?? (Instancia = new Parametro())); }
			set
			{
				if (_instanciaUnica == null)
				{
					lock (String.Empty)
					{
						if (_instanciaUnica == null)
							_instanciaUnica = value;
					}
				}
			}
		}


		public class Conexao
		{
			private readonly Int32 _id;
			private readonly Int32 _tipoBanco;
			private readonly String _servidor;
			private readonly String _usuario;

			public Int32 Id { get { return _id; } }
			public Int32 TipoBanco { get { return _tipoBanco; } }
			public String Servidor { get { return _servidor; } }
			public String Usuario { get { return _usuario; } }
			public String Senha { get; private set; }
			public String Banco { get; private set; }
			public Int32 Ordem { get; private set; }
			public Boolean SalvarSenha { get; private set; }

			public Conexao(Int32 id, Int32 tipoBanco, String servidor, String usuario, String senha, String banco, Int32 ordem, Boolean salvarSenha)
			{
				_id = id;
				_tipoBanco = tipoBanco;
				_servidor = servidor;
				_usuario = usuario;
				Banco = banco;
				Ordem = ordem;
				SalvarSenha = salvarSenha;
				Senha = salvarSenha ? senha : String.Empty;
			}

			public void Configurar(Int32 ordem, Boolean salvarSenha)
			{
				Ordem = ordem;
				SalvarSenha = salvarSenha;
				if (!salvarSenha)
					Senha = String.Empty;
			}

			public static String Cripto(String valor)
			{
				var senha = new StringBuilder();
				var controle = Convert.ToChar(valor.Length % 126 + 1);
				foreach (var chr in valor)
					senha.Append(Convert.ToChar(chr ^ controle));
				return senha.ToString();
			}
		}

		public static class cmdSql
		{
			public const String ExisteTabela = @"Select T.Name From sqlite_master T Where (T.Type = 'table') And (T.Name = '{0}')";


			public const String CreateTableConexao = @"Create Table Conexao (
	Id			Integer			Not Null	Primary Key		AutoIncrement,
	TipoBanco	Integer			Not Null,
	Servidor	Varchar(250)	Not Null,
	Usuario		Varchar(250)	Not Null,
	Senha		Varchar(250)		Null,
	Banco		Varchar(250)		Null,
	SalvarSenha	Boolean			Not Null,
	Ordem		Integer			Not Null);";
			public const String SelectFromConexao = @"Select * From Conexao;";
			public const String DeleteFromConexao = @"Delete From Conexao;";
			public const String InsertIntoConexao = @"Insert Into Conexao (TipoBanco, Servidor, Usuario, Senha, Banco, SalvarSenha, Ordem) Values (@tipoBanco, @servidor, @usuario, @senha, @banco, @salvarSenha, @ordem);";

			public const String CreateTableConfiguracao = @"Create Table Configuracao (
	Id		Integer			Not Null	Primary Key		AutoIncrement,
	Chave	Varchar(250)	Not Null,
	Valor	Varchar(250)	Not Null);";
			public const String SelectFromConfiguracao = @"Select Valor From Configuracao Where (Chave = '{0}');";
			public const String UpdateSetConfiguracao = @"Update Configuracao Set Valor = '{0}' Where (Chave = '{1}');";
			public const String InsertIntoConfiguracao = @"Insert Into Configuracao (Valor, Chave) Values ('{0}', '{1}');";
			public const String DeleteFromConfiguracao = @"Delete From Configuracao;";
		}

	}
}