﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
		protected abstract String SQLAllTables(Boolean comDetalhes);
		protected abstract String SQLAllViews(String nome);
		protected abstract String SQLAllColumns(String parent, Boolean comDetalhes);
		protected abstract String SQLAllProcedures(String nome);
		protected abstract String SQLAllDatabases(Boolean comDetalhes);

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

		public virtual IEnumerable<String> ListarTabelas(String nome)
		{
			var dataReader = ExecuteReader(String.Format(SQLAllTables(false), nome));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarViews(String nome)
		{
			var dataReader = ExecuteReader(SQLAllViews(nome));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarColunas(String parent, Boolean listarDetalhes)
		{
			var dataReader = ExecuteReader(SQLAllColumns(parent, listarDetalhes));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, listarDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarProcedures(String nome)
		{
			var dataReader = ExecuteReader(SQLAllProcedures(nome));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual Object Executar(String query)
		{
			var iDataReader = ExecuteReader(query);
			_tipo = ClasseDinamica.CriarTipoVirtual(iDataReader, _iMessageResult);
			return null;
		}

		private Object ExecuteScalar(String query)
		{
			FreeReader();
			FreeCommand();
			_iDbCommand = _iDbConnection.CriarComando(query);
			return _iDbCommand.ExecuteScalar();
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

		protected virtual String Formatar(IDataReader dataReader, Boolean listarDetalhes)
		{
			return Convert.ToString(dataReader["Nome"]) + (listarDetalhes ? Convert.ToString(dataReader["Detalhes"]) : String.Empty);
		}

		public void SetMessageResult(IMessageResult iMessageResult)
		{
			_iMessageResult = iMessageResult;
		}

		protected void ShowLog(String message, String tipo)
		{
			_iMessageResult.ShowLog(message, tipo);
		}

		public virtual String TestarConexao(String server, String dataBase, String usuario, String senha)
		{
			String result = "Indefinido";
			try
			{
				IDbConnection iDbConnection = CriarConexao(server, dataBase, usuario, senha);
				try
				{
					if (iDbConnection != null)
					{
						iDbConnection.Open();
						iDbConnection.Close();
						result = null;
					}
				}
				catch (Exception exception)
				{
					result = "Houve um problema ao tentar conectar ao banco de dados. Detalhes:\n\n" + exception.Message;
					if (iDbConnection != null)
						iDbConnection.Dispose();
				}
				finally
				{
					iDbConnection = null;
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

		private IDbConnection CriarConexao(String server, String dataBase, String usuario, String senha)
		{
			_iDbConnection = Activator.CreateInstance(typeof(TIDbConnection)) as TIDbConnection;
			_iDbConnection.ConnectionString = String.Format(StringConexaoTemplate, server, dataBase, usuario, senha);
			_server = server;
			_dataBase = dataBase;
			_usuario = usuario;
			_senha = senha;
			return _iDbConnection;
		}

	}
}