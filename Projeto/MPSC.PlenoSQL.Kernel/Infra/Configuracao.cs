using MPSC.PlenoSQL.Kernel.Dados.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public class Configuracao
	{
		private static Configuracao _instanciaUnica;
		private static readonly String strConexao = String.Format(@"Data Source={0};Version=3;", Path.Combine(Cache.cRootPath, "PlenoSQL.db"));
		private IList<Conexao> _conexoes;
		public IEnumerable<Conexao> Conexoes
		{
			get { return _conexoes ?? (_conexoes = LoadConexoes().ToList()); }
		}

		private static SQLiteConnection NewConnection => new SQLiteConnection(strConexao);

		private Configuracao()
		{
			if (!ExisteTabela("Configuracao"))
				ExecuteNonQuery(CmdSql.CreateTableConfiguracao);

			if (!ExisteTabela("Conexao"))
				ExecuteNonQuery(CmdSql.CreateTableConexao);

			if (!ExisteTabela("Constante"))
				ExecuteNonQuery(CmdSql.CreateTableConstante);
		}

		public Configuracao NovaConexao(Int32 tipoBanco, String servidor, String usuario, String senha, String banco, Boolean salvaSenha)
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
				_conexoes.Remove(existente);

			_conexoes.Add(conexao);
			return this;
		}

		public void SaveConexao()
		{
			var conexoes = Conexoes.OrderBy(c => c.Ordem);
			ExecuteNonQuery(CmdSql.DeleteFromConexao);
			var iDbConnection = NewConnection;
			var ordem = 1;
			foreach (var conexao in conexoes)
			{
				var iDbCommand = iDbConnection.CriarComando(CmdSql.InsertIntoConexao);
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

		public void SaveConstantes()
		{
			var constantes = Constantes.Instancia.Obter(null, Constantes.Filtro.TodasDeTodos);
			ExecuteNonQuery(CmdSql.DeleteFromConstante);
			var iDbConnection = NewConnection;
			var iDbCommand = iDbConnection.CriarComando(CmdSql.InsertIntoConstante);
			var pEscopo = iDbCommand.AdicionarParametro("@escopo", "escopo", DbType.String);
			var pNome = iDbCommand.AdicionarParametro("@nome", "Nome", DbType.String);
			var pValor = iDbCommand.AdicionarParametro("@valor", "Valor", DbType.String);
			foreach (var constante in constantes)
			{
				pEscopo.Value = constante.escopo;
				pNome.Value = constante.Nome;
				pValor.Value = constante.Valor;
				iDbCommand.ExecuteNonQuery();
			}
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
		}

		public object ObterValorConfiguracao(object cEditor_ColorirTextosSql)
		{
			throw new NotImplementedException();
		}

		public String ObterValorConfiguracao(String chave)
		{
			var cmdSql = String.Format(CmdSql.SelectFromConfiguracao, chave);
			return ExecuteScalar(cmdSql)?.ToString();
		}

		public Int32 GravarValorConfiguracao(String chave, Object value)
		{
			var cmdSql = String.Format(CmdSql.UpdateSetConfiguracao, Convert.ToString(value), chave);
			var qtd = ExecuteNonQuery(cmdSql);
			if (qtd == 0)
			{
				cmdSql = String.Format(CmdSql.InsertIntoConfiguracao, Convert.ToString(value), chave);
				qtd = ExecuteNonQuery(cmdSql);
			}
			return qtd;
		}

		private Int32 ExecuteNonQuery(String cmdSql)
		{
			return NewConnection.ExecuteNonQuery(cmdSql);
		}

		private Object ExecuteScalar(String cmdSql)
		{
			return NewConnection.ExecuteScalar(cmdSql);
		}

		private Boolean ExisteTabela(String nomeTabela)
		{
			return nomeTabela.Equals(ExecuteScalar(String.Format(CmdSql.ExisteTabela, nomeTabela)));
		}

		private IEnumerable<Conexao> LoadConexoes()
		{
			var iDbConnection = NewConnection;
			var iDbCommand = iDbConnection.CriarComando(CmdSql.SelectFromConexao);
			var iDataReader = iDbCommand.ExecuteReader();
			while (iDataReader.Read())
				yield return new Conexao(iDataReader.GetInt32("Id"), iDataReader.GetInt16("TipoBanco"), iDataReader.GetString("Servidor"), iDataReader.GetString("Usuario"), Conexao.Cripto(iDataReader.GetString("Senha")), iDataReader.GetString("Banco"), iDataReader.GetInt16("Ordem"), iDataReader.GetBoolean("SalvarSenha"));
			iDataReader.Close();
			iDataReader.Dispose();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
		}

		public Constantes LoadConstantes(Constantes constantes)
		{
			var iDbConnection = NewConnection;
			var iDbCommand = iDbConnection.CriarComando(CmdSql.SelectFromConstante);
			var iDataReader = iDbCommand.ExecuteReader();
			while (iDataReader.Read())
				constantes.Adicionar(iDataReader.GetString("Escopo"), iDataReader.GetString("Nome"), iDataReader.GetString("Valor"));
			iDataReader.Close();
			iDataReader.Dispose();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
			return constantes;
		}

		public static Configuracao Instancia
		{
			get { return (_instanciaUnica ?? (Instancia = new Configuracao())); }
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

		public static class CmdSql
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
	Chave	Varchar(100)	Not Null,
	Valor	Varchar(500)	Not Null);";
			public const String SelectFromConfiguracao = @"Select Valor From Configuracao Where (Chave = '{0}');";
			public const String UpdateSetConfiguracao = @"Update Configuracao Set Valor = '{0}' Where (Chave = '{1}');";
			public const String InsertIntoConfiguracao = @"Insert Into Configuracao (Valor, Chave) Values ('{0}', '{1}');";
			public const String DeleteFromConfiguracao = @"Delete From Configuracao;";

			public const String CreateTableConstante = @"Create Table Constante (
	Escopo	Varchar(250)	Not Null,
	Nome	Varchar(250)	Not Null,
	Valor	Varchar(250)	Not Null);";
			public const String SelectFromConstante = @"Select * From Constante;";
			public const String InsertIntoConstante = @"Insert Into Constante (Escopo, Nome, Valor) Values (@escopo, @nome, @valor);";
			public const String DeleteFromConstante = @"Delete From Constante;";
		}
	}
}