using IBM.Data.DB2.iSeries;
using Microsoft.HostIntegration.MsDb2Client;
using MPSC.PlenoSQL.Kernel.Infra;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace MPSC.PlenoSQL.Kernel.Dados.Base
{
	public abstract class BancoDeDados<TIDbConnection> : BancoDados, IBancoDeDados, IMessageResult where TIDbConnection : DbConnection, IDbConnection
	{
		protected String _server;
		protected String _dataBase;
		protected String _usuario;
		protected String _senha;
		private Type _tipo = null;
		private TIDbConnection _iDbConnection = null;
		private IDbCommand _iDbCommand = null;
		private IDataReader _iDataReader = null;
		private IMessageResult _iMessageResult = null;

		public Type Tipo { get { return _tipo; } }

		public String Conexao { get { return String.Format("{3} em {1}@{0} por {2}", _server, _dataBase, _usuario, GetType().DisplayName()); } }
		protected abstract String StringConexaoTemplate { get; }
		protected abstract String SQLSelectCountTemplate(String query);
		protected abstract String SQLAllDatabases(String nome, Boolean comDetalhes);
		protected abstract String SQLAllProcedures(String nome, Boolean comDetalhes);
		protected abstract String SQLTablesIndexes { get; }
		protected abstract String SQLTablesColumns { get; }

		protected Cache Cache
		{
			get
			{
				var conexao = Conexao;
				if (!cache.ContainsKey(conexao))
				{
					cache[conexao] = new Cache();
					var dataReader = ExecuteReader(SQLTablesColumns);
					cache[conexao] = new Cache(dataReader);
				}
				return cache[conexao];
			}
		}

		public virtual void AlterarBancoAtual(String nome)
		{
			try
			{
				_dataBase = nome;
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}

		public virtual IEnumerable<String> ListarBancosDeDados(String nome, Boolean comDetalhes)
		{
			IDataReader dataReader = null;
			try
			{
				dataReader = ExecuteReader(SQLAllDatabases(nome, comDetalhes));
			}
			catch (Exception) { }

			while (dataReader.IsOpen() && dataReader.Read())
				yield return Convert.ToString(dataReader["Nome"]);

			if (dataReader != null)
			{
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			return Cache.Tabelas(nome, comDetalhes);
		}

		public virtual IEnumerable<String> ListarViews(String nome, Boolean comDetalhes)
		{
			return Cache.Views(nome, comDetalhes);
		}

		public virtual IEnumerable<String> ListarColunas(String parent, String filtro, Boolean comDetalhes)
		{
			return Cache.Colunas(parent, filtro, comDetalhes);
		}

		public virtual IEnumerable<String> ListarIndices(String parent, String filtro, Boolean comDetalhes)
		{
			return Cache.Colunas(parent, filtro, comDetalhes);
		}

		public virtual IEnumerable<String> ListarProcedures(String nome, Boolean comDetalhes)
		{
			IList<String> lista = null;
			var key = String.Format("{0}_{1}_{2}_{3}", "Procedures", "ALL", Conexao, comDetalhes);
			if (cache.ContainsKey(key))
				lista = cacheOld[key];
			else
			{
				lista = cacheOld[key] = new List<String>();
				var dataReader = ExecuteReader(SQLAllProcedures(String.Empty, comDetalhes));
				while (dataReader.IsOpen() && dataReader.Read())
					lista.Add(Formatar(dataReader, comDetalhes));
				if (dataReader != null)
				{
					dataReader.Close();
					dataReader.Dispose();
				}
			}
			return lista.Where(i => String.IsNullOrWhiteSpace(nome) || i.ToUpper().StartsWith(nome.ToUpper()));
		}

		public virtual Object Executar(String query, Boolean comEstatisticas, out Boolean retornaDados)
		{
			Object result = 0;
			retornaDados = Regex.Replace(query, "[^a-zA-Z0-9]", String.Empty).ToUpper().StartsWith("SELECT");

			if (retornaDados)
			{
				_tipo = ClasseDinamica.CriarTipoVirtual(ExecuteReader(query), this);
				if (comEstatisticas && (_tipo != null))
				{
					try
					{
						result = ExecuteScalar(SQLSelectCountTemplate(query));
					}
					catch (Exception) { }
				}
			}
			else
				result = ExecuteNonQuery(query);
			return result;
		}

		private Int64 ExecuteNonQuery(String query)
		{
			var iDbCommand = _iDbConnection.CriarComando(query);
			var result = iDbCommand.ExecuteNonQuery();
			iDbCommand.Dispose();
			return result;
		}

		private Object ExecuteScalar(String query)
		{
			var iDbCommand = _iDbConnection.CriarComando(query);
			var result = iDbCommand.ExecuteScalar();
			iDbCommand.Dispose();
			return result;
		}

		protected IDataReader ExecuteReader(String query)
		{
			FreeReader();
			FreeCommand();
			_iDbCommand = _iDbConnection.CriarComando(query);
			if (_iDbCommand != null)
			{
				try
				{
					_iDataReader = _iDbCommand.ExecuteReader();
				}
				catch (MsDb2Exception exception)
				{
					if (exception.SqlState == "HY000")
					{
						CriarConexao();
						AbrirConexao(true);
					}
					throw exception;
				}
				catch (Exception vException) { throw vException; }
			}
			return _iDataReader;
		}

		public virtual IEnumerable<Object> DataBinding(Int64 limite)
		{
			var linhas = -1;
			var tipo = _tipo ?? typeof(Object);
			var properties = tipo.GetProperties();
			yield return ClasseDinamica.CriarObjetoVirtual(tipo, null, properties);
			while (_iDataReader.IsOpen() && (++linhas < limite) && _iDataReader.Read())
			{
				ProcessarEventos();
				yield return ClasseDinamica.CriarObjetoVirtual(tipo, _iDataReader, properties);
			}

			if ((linhas <= 0) || ((linhas < 100) && _iDataReader.IsOpen() && !_iDataReader.Read()))
				FreeReader();
		}

		public virtual void Dispose()
		{
			FreeReader();
			FreeCommand();
			FreeConnection();
		}

		protected virtual DataTable GetSchema(String collectionName)
		{
			try
			{
				return AbrirConexao(false).GetSchema(collectionName);
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); return null; }
		}

		protected virtual TIDbConnection AbrirConexao(Boolean reset)
		{
			try
			{
				if (reset)
				{
					if (_iDbConnection.State == ConnectionState.Open)
						_iDbConnection.Close();
				}
				if (_iDbConnection.State != ConnectionState.Open)
					_iDbConnection.Open();
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
			return _iDbConnection;
		}

		private void FreeReader()
		{
			if (_iDataReader != null)
			{
				try
				{
					if (!_iDataReader.IsClosed)
						_iDataReader.Close();
				}
				catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
				finally { _iDataReader.Dispose(); }
				_iDataReader = null;
			}
		}

		private void FreeCommand()
		{
			if (_iDbCommand != null)
			{
				_iDbCommand.Dispose();
				_iDbCommand = null;
			}
			_tipo = null;
		}

		private void FreeConnection()
		{
			if (_iDbConnection != null)
			{
				try
				{
					if (_iDbConnection.State != ConnectionState.Closed)
						_iDbConnection.Close();
				}
				catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
				finally { /*_iDbConnection.Dispose();*/ }
				_iDbConnection = null;
			}
		}

		protected virtual String Formatar(IDataReader dataReader, Boolean comDetalhes)
		{
			return Convert.ToString(dataReader["Nome"]) + (comDetalhes ? Convert.ToString(dataReader["Detalhes"]) : String.Empty);
		}

		public virtual void ConfigurarConexao(String server, String dataBase, String usuario, String senha)
		{
			_server = server;
			_dataBase = dataBase;
			_usuario = usuario;
			_senha = senha;
			CriarConexao();
		}

		public virtual String TestarConexao()
		{
			String result = "Indefinido";
			try
			{
				if (_iDbConnection == null)
					CriarConexao();
				try
				{
					if (_iDbConnection != null)
					{
						if (_iDbConnection.State == ConnectionState.Open)
							_iDbConnection.Close();
						_iDbConnection.Open();
						_iDbConnection.Close();
						result = null;
					}
				}
				catch (iDB2Exception exception)
				{
					result = "Houve um problema ao tentar conectar ao banco de dados. Detalhes:\n\n" + exception.MessageDetails;
				}
				catch (Exception exception)
				{
					result = "Houve um problema ao tentar conectar ao banco de dados. Detalhes:\n\n" + exception.Message;
				}
				finally
				{
					if (!String.IsNullOrWhiteSpace(result))
						ShowLog(result, "Erro");
				}
			}
			catch (Exception exception)
			{
				result = "Houve um problema ao tentar conectar ao banco de dados. Detalhes:\n\n" + exception.Message;
			}

			return result;
		}

		private void CriarConexao()
		{
			FreeConnection();
			_iDbConnection = Activator.CreateInstance(typeof(TIDbConnection)) as TIDbConnection;
			_iDbConnection.ConnectionString = String.Format(StringConexaoTemplate, _server, _dataBase, _usuario, _senha);
		}

		public virtual IBancoDeDados Clone()
		{
			return MemberwiseClone() as BancoDeDados<TIDbConnection>;
		}

		public void SetMessageResult(IMessageResult iMessageResult)
		{
			_iMessageResult = iMessageResult;
		}

		void IMessageResult.ShowLog(String message, String tipo)
		{
			ShowLog(message, tipo);
		}

		void IMessageResult.ProcessarEventos()
		{
			ProcessarEventos();
		}

		protected void ShowLog(String message, String tipo)
		{
			if (_iMessageResult != null)
				_iMessageResult.ShowLog(message, tipo);
		}

		protected void ProcessarEventos()
		{
			if (_iMessageResult != null)
				_iMessageResult.ProcessarEventos();
		}

		protected class Query
		{

		}
	}
}