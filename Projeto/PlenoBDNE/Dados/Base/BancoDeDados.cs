using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public abstract class BancoDeDados<TIDbConnection> : IBancoDeDados where TIDbConnection : DbConnection, IDbConnection
	{
		public abstract String Descricao { get; }
		public virtual String Conexao { get; private set; }

		protected abstract String StringConexaoTemplate { get; }
		protected abstract String AllTablesSQL(Boolean comDetalhes);
		protected abstract String AllViewsSQL(Boolean comDetalhes);
		protected abstract String AllColumnsSQL(Boolean comDetalhes);
		protected abstract String AllProceduresSQL(String procedureName);
		protected abstract String AllDatabasesSQL(Boolean comDetalhes);

		private IMessageResult _iMessageResult = null;
		private Type _tipo = null;
		private TIDbConnection _iDbConnection = null;
		private IDbCommand _iDbCommand = null;
		private IDataReader _iDataReader = null;

		public virtual IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha)
		{
			_iDbConnection = Activator.CreateInstance(typeof(TIDbConnection)) as TIDbConnection;
			_iDbConnection.ConnectionString = String.Format(StringConexaoTemplate, server, dataBase, usuario, senha);
			Conexao = String.Format("{3} em {1}@{0} por {2}", server, dataBase, usuario, Descricao);
			return _iDbConnection;
		}

		public virtual IEnumerable<String> ListarColunas(String tabela, Boolean listarDetalhes)
		{
			var dataReader = ExecutarQuery(String.Format(AllColumnsSQL(listarDetalhes), tabela));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Formatar(dataReader, listarDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarTabelas(String tabela)
		{
			var dataReader = ExecutarQuery(String.Format(AllTablesSQL(false), tabela));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarViews(String view)
		{
			var dataReader = ExecutarQuery(String.Format(AllViewsSQL(false), view));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IEnumerable<String> ListarProcedures(String procedure)
		{
			var dataReader = ExecutarQuery(AllProceduresSQL(procedure));
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
			_tipo = ClasseDinamica.CriarTipoVirtual(ExecutarQuery(query), _iMessageResult);
			return null;
		}

		public virtual IDataReader ExecutarQuery(String query)
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
	}
}