using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.View;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public abstract class BancoDeDados<TIDbConnection> : IBancoDeDados where TIDbConnection : class, IDbConnection
	{
		public abstract String Descricao { get; }

		protected abstract String StringConexaoTemplate { get; }
		protected abstract String AllTablesSQL { get; }

		private Type _tipo = null;
		private TIDbConnection _iDbConnection = null;
		private IDbCommand _iDbCommand = null;
		private IDataReader _iDataReader = null;

		public virtual IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha)
		{
			var stringConexao = String.Format(StringConexaoTemplate, server, dataBase, usuario, senha);
			return _iDbConnection = Activator.CreateInstance(typeof(TIDbConnection), stringConexao) as TIDbConnection;
		}

		public IEnumerable<String> ListarColunasDasTabelas(String tabela)
		{
			var dataReader = ExecutarQuery("Select * From " + tabela + " Where 0=1");
			for (Int32 i = 0; (dataReader != null) && (!dataReader.IsClosed) && (i < dataReader.FieldCount); i++)
				yield return dataReader.GetName(i);

			dataReader.Close();
			dataReader.Dispose();
		}

		public IEnumerable<String> ListarTabelas(String tabela)
		{
			var dataReader = ExecutarQuery(String.Format(AllTablesSQL, tabela));
			while ((dataReader != null) && (!dataReader.IsClosed) && dataReader.Read())
				yield return Convert.ToString(dataReader["Tabela"]);
			dataReader.Close();
			dataReader.Dispose();
		}

		public IDataReader ExecutarQuery(String query)
		{
			Free();
			_iDbCommand = _iDbConnection.CriarComando(query);
			_iDataReader = _iDbCommand.ExecuteReader();
			return _iDataReader;
		}

		public void Executar(String query)
		{
			_tipo = ClasseDinamica.CriarTipoVirtual(ExecutarQuery(query));
		}

		public IEnumerable<Object> Transformar()
		{
			var linhas = -1;
			while (_iDataReader.IsOpen() && (++linhas < 100) && _iDataReader.Read())
				yield return ClasseDinamica.CreateObjetoVirtual(_tipo, _iDataReader);

			if (linhas <= 0)
			{
				_iDataReader.Close();
				_iDataReader.Dispose();
				_iDataReader = null;
			}
			else if ((linhas < 100) && _iDataReader.IsOpen() && !_iDataReader.Read())
			{
				_iDataReader.Close();
				_iDataReader.Dispose();
				_iDataReader = null;
			}
		}

		public IEnumerable<Object> Cabecalho()
		{
			yield return ClasseDinamica.CreateObjetoVirtual(_tipo, null);
		}

		public virtual void Dispose()
		{
			Free();

			if (_iDbConnection != null)
			{
				try
				{
					if (_iDbConnection.State != ConnectionState.Closed)
						_iDbConnection.Close();
				}
				catch (Exception) { }
				finally { _iDbConnection.Dispose(); }
				_iDbConnection = null;
			}

			BancoDeDados.ListaDeBancoDeDados.Clear();
			BancoDeDados.ListaDeBancoDeDados = null;
		}

		private void Free()
		{
			if (_iDataReader != null)
			{
				try
				{
					if (!_iDataReader.IsClosed)
						_iDataReader.Close();
				}
				catch (Exception) { }
				finally { _iDataReader.Dispose(); }
				_iDataReader = null;
			}

			if (_iDbCommand != null)
			{
				try
				{
					_iDbCommand.Cancel();
				}
				catch (Exception) { }
				finally { _iDbCommand.Dispose(); }
				_iDbCommand = null;
			}

			_tipo = null;
		}

		public static IBancoDeDados Conectar()
		{
			return Autenticacao.Dialog();
		}
	}
}