using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public abstract class BancoDeDados<TIDbConnection> : IBancoDeDados where TIDbConnection : DbConnection, IDbConnection
	{
		private String _server;
		private String _dataBase;
		private String _usuario;
		private String _senha;
		private Type _tipo = null;
		private TIDbConnection _iDbConnection = null;
		private IDbCommand _iDbCommand = null;
		private IDataReader _iDataReader = null;
		private IMessageResult _iMessageResult = null;

		public String Conexao { get { return String.Format("{3} em {1}@{0} por {2}", _server, _dataBase, _usuario, Descricao); } }
		public abstract String Descricao { get; }
		protected abstract String StringConexaoTemplate { get; }
		protected abstract String SQLSelectCountTemplate(String query);

		protected abstract String SQLAllDatabases(String nome, Boolean comDetalhes);
		protected abstract String SQLAllTables(String nome, Boolean comDetalhes);
		protected abstract String SQLAllViews(String nome, Boolean comDetalhes);
		protected abstract String SQLAllColumns(String parent, Boolean comDetalhes);
		protected abstract String SQLAllProcedures(String nome, Boolean comDetalhes);

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
			var dataReader = ExecuteReader(SQLAllDatabases(nome, comDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var dataReader = ExecuteReader(SQLAllTables(nome, comDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, comDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarViews(String nome, Boolean comDetalhes)
		{
			var dataReader = ExecuteReader(SQLAllViews(nome, comDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, comDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarColunas(String parent, Boolean comDetalhes)
		{
			var dataReader = ExecuteReader(SQLAllColumns(parent, comDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, comDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarProcedures(String nome, Boolean comDetalhes)
		{
			var dataReader = ExecuteReader(SQLAllProcedures(nome, comDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, comDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual Object Executar(String query)
		{
			Object result = null;
			if (Regex.Replace(query, "[^a-zA-Z0-9]", String.Empty).ToUpper().StartsWith("SELECT"))
			{
				try
				{
					result = ExecuteScalar(SQLSelectCountTemplate(query));
				}
				catch (Exception) { }
				_tipo = ClasseDinamica.CriarTipoVirtual(ExecuteReader(query), _iMessageResult);
			}
			else
				result = ExecuteNonQuery(query);
			return result;
		}

		private Int64 ExecuteNonQuery(String query)
		{
			FreeReader();
			FreeCommand();
			var iDbCommand = _iDbConnection.CriarComando(query);
			var result = iDbCommand.ExecuteNonQuery();
			iDbCommand.Dispose();
			return result;
		}

		private Object ExecuteScalar(String query)
		{
			FreeReader();
			FreeCommand();
			var iDbCommand = _iDbConnection.CriarComando(query);
			var result = iDbCommand.ExecuteScalar();
			iDbCommand.Dispose();
			return result;
		}

		private IDataReader ExecuteReader(String query)
		{
			FreeReader();
			FreeCommand();
			_iDbCommand = _iDbConnection.CriarComando(query);
			_iDataReader = _iDbCommand.ExecuteReader();
			return _iDataReader;
		}

		public virtual IEnumerable<Object> DataBinding()
		{
			var linhas = -1;
			yield return ClasseDinamica.CreateObjetoVirtual(_tipo, null);
			while (_iDataReader.IsOpen() && (++linhas < 100) && _iDataReader.Read())
				yield return ClasseDinamica.CreateObjetoVirtual(_tipo, _iDataReader);

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
				try
				{
					_iDbCommand.Cancel();
				}
				catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
				finally { _iDbCommand.Dispose(); }
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
				finally { _iDbConnection.Dispose(); }
				_iDbConnection = null;
			}
		}

		protected virtual String Formatar(IDataReader dataReader, Boolean comDetalhes)
		{
			return Convert.ToString(dataReader["Nome"]) + (comDetalhes ? Convert.ToString(dataReader["Detalhes"]) : String.Empty);
		}

		public void SetMessageResult(IMessageResult iMessageResult)
		{
			_iMessageResult = iMessageResult;
		}

		protected void ShowLog(String message, String tipo)
		{
			_iMessageResult.ShowLog(message, tipo);
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
						_iDbConnection.Open();
						_iDbConnection.Close();
						result = null;
					}
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
	}
}