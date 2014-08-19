using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Infra.Interface;
using MP.PlenoBDNE.AppWin.View;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public abstract class BancoDeDados<TIDbConnection> : IBancoDeDados where TIDbConnection : class, IDbConnection
	{
		public abstract String Descricao { get; }

		protected abstract String StringConexaoTemplate { get; }
		protected abstract String AllTablesSQL { get; }
		protected abstract String AllColumnsSQL { get; }

		private Type _tipo = null;
		private TIDbConnection _iDbConnection = null;
		private IDbCommand _iDbCommand = null;
		private IDataReader _iDataReader = null;

		public virtual IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha)
		{
			_iDbConnection = Activator.CreateInstance(typeof(TIDbConnection)) as TIDbConnection;
			_iDbConnection.ConnectionString = String.Format(StringConexaoTemplate, server, dataBase, usuario, senha);
			return _iDbConnection;
		}

		public IEnumerable<String> ListarColunasDasTabelas(String tabela)
		{
			var dataReader = ExecutarQuery(String.Format(AllColumnsSQL, tabela));
			while ((dataReader != null) && (!dataReader.IsClosed) && dataReader.Read())
				yield return Convert.ToString(dataReader["Coluna"]);

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

		public void Executar(String query, IMessageResult messageResult)
		{
			_tipo = ClasseDinamica.CriarTipoVirtual(ExecutarQuery(query), messageResult);
		}

		public IEnumerable<Object> Transformar()
		{
			var linhas = -1;
			while (_iDataReader.IsOpen() && (++linhas < 100) && _iDataReader.Read())
				yield return ClasseDinamica.CreateObjetoVirtual(_tipo, _iDataReader);

			var dispose = (linhas <= 0) || ((linhas < 100) && _iDataReader.IsOpen() && !_iDataReader.Read());

			if ((dispose) && (_iDataReader != null))
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