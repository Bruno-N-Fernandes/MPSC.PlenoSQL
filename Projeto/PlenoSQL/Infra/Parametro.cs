using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using MP.PlenoBDNE.AppWin.Dados.Base;
using System.IO;

namespace MP.PlenoSQL.AppWin.Infra
{
	public class Parametro
	{
		private static readonly String strConexao = String.Format(@"Data Source={0};Version=3;", Path.Combine(Path.GetTempPath(), "PlenoSQL.db"));
		private const String cmdSqlExisteTabela = @"Select T.Name From sqlite_master T Where (T.Type = 'table') And (T.Name = '{0}')";
		private const String cmdSqlInsertIntoConexao = @"Insert Into Conexao (TipoBanco, Servidor, Usuario, Senha, Banco, SalvarSenha, Ordem) Values (@tipoBanco, @servidor, @usuario, @senha, @banco, @salvarSenha, @ordem);";
		private const String cmdSqlDeleteFromConexao = @"Delete From Conexao;";
		private const String cmdSqlCreateConexao = @"Create Table Conexao (
	Id			Integer			Not Null	Primary Key		AutoIncrement,
	TipoBanco	Integer			Not Null,
	Servidor	Varchar(250)	Not Null,
	Usuario		Varchar(250)	Not Null,
	Senha		Varchar(250)		Null,
	Banco		Varchar(250)		Null,
	SalvarSenha	Boolean			Not Null,
	Ordem		Integer			Not Null
);";

		private const String cmdSqlCreateConfiguracao = @"Create Table Configuracao (
	Id		Integer			Not Null	Primary Key		AutoIncrement,
	Chave	Varchar(250)	Not Null,
	Valor	Varchar(250)	Not Null
);";

		private IList<Conexao> _conexoes;

		public IEnumerable<Conexao> Conexoes
		{
			get { return _conexoes ?? (_conexoes = LoadConexoes().ToList()); }
		}

		private Object ObterValorConfiguracao(String chave)
		{
			return ExecuteScalar(String.Format("Select Valor From Configuracao Where (Chave = '{0}');", chave)) ?? String.Empty;
		}

		private void GravarValorConfiguracao(String chave, Object value)
		{
			var qtd = ExecuteNonQuery(String.Format("Update Configuracao Set Valor = '{0}' Where (Chave = '{1}');", Convert.ToString(value), chave));
			if (qtd == 0)
				qtd = ExecuteNonQuery(String.Format("Insert Into Configuracao (Valor, Chave) Values ('{0}', '{1}');", Convert.ToString(value), chave));

		}


		private Parametro()
		{
			if (!ExisteTabela("Configuracao"))
				ExecuteNonQuery(cmdSqlCreateConfiguracao);

			if (!ExisteTabela("Conexao"))
				ExecuteNonQuery(cmdSqlCreateConexao);

		}

		public void Save()
		{
			ExecuteNonQuery(cmdSqlDeleteFromConexao);
			var iDbConnection = new SQLiteConnection(strConexao);
			var conexoes = Conexoes.OrderBy(c => c.Ordem);
			var ordem = 1;
			foreach (var conexao in conexoes)
			{
				var iDbCommand = iDbConnection.CriarComando(cmdSqlInsertIntoConexao);
				iDbCommand.AdicionarParametro("@tipoBanco", conexao.TipoBanco, DbType.Int16);
				iDbCommand.AdicionarParametro("@servidor", conexao.Servidor, DbType.String);
				iDbCommand.AdicionarParametro("@usuario", conexao.Usuario, DbType.String);
				iDbCommand.AdicionarParametro("@senha", conexao.Senha, DbType.String);
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
			var iDbConnection = new SQLiteConnection(strConexao);
			var iDbCommand = iDbConnection.CriarComando(String.Format(cmdSqlExisteTabela, nomeTabela));
			var iDataReader = iDbCommand.ExecuteReader();
			Boolean existe = (iDataReader.Read()) && nomeTabela.Equals(iDataReader.GetString("Name"));
			iDataReader.Close();
			iDataReader.Dispose();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
			return existe;
		}

		private IEnumerable<Conexao> LoadConexoes()
		{
			var iDbConnection = new SQLiteConnection(strConexao);
			var iDbCommand = iDbConnection.CriarComando("Select * From Conexao");
			var iDataReader = iDbCommand.ExecuteReader();
			while (iDataReader.Read())
				yield return new Conexao(iDataReader.GetInt32("Id"), iDataReader.GetInt16("TipoBanco"), iDataReader.GetString("Servidor"), iDataReader.GetString("Usuario"), iDataReader.GetString("Senha"), iDataReader.GetString("Banco"), iDataReader.GetInt16("Ordem"), iDataReader.GetBoolean("SalvarSenha"));
			iDataReader.Close();
			iDataReader.Dispose();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
		}

		private static Parametro _instanciaUnica;
		public static Parametro Instancia
		{
			get { return (_instanciaUnica ?? (Instancia = new Parametro())); }
			set
			{
				if (_instanciaUnica == null)
				{
					lock (strConexao)
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
			private readonly String _senha;

			public Int32 Id { get { return _id; } }
			public Int32 TipoBanco { get { return _tipoBanco; } }
			public String Servidor { get { return _servidor; } }
			public String Usuario { get { return _usuario; } }
			public String Senha { get { return _senha; } }
			public String Banco { get; private set; }
			public Int32 Ordem { get; private set; }
			public Boolean SalvarSenha { get; private set; }

			public Conexao(Int32 id, Int32 tipoBanco, String servidor, String usuario, String senha, String banco, Int32 ordem, Boolean salvarSenha)
			{
				_id = id;
				_tipoBanco = tipoBanco;
				_servidor = servidor;
				_usuario = usuario;
				_senha = senha;
				Banco = banco;
				Ordem = ordem;
				SalvarSenha = salvarSenha;
			}

			public void Configurar(Int32 ordem, Boolean salvarSenha)
			{
				Ordem = ordem;
				SalvarSenha = salvarSenha;
			}
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
	}
}