using System;
using System.Collections.Generic;
using System.Data;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.Dados.Base
{
	public abstract class BancoDeDadosGenerico<TIDbConnection> : IBancoDeDados, IMessageResult where TIDbConnection : class, IDbConnection
	{
		public BancoDeDadosGenerico() { }
		public abstract String Descricao { get; }
		public virtual String Conexao { get; private set; }

		protected abstract String StringConexaoTemplate { get; }
		protected abstract String AllTablesSQL { get; }
		protected abstract String AllViewsSQL { get; }
		protected abstract String AllColumnsSQL { get; }

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

		public virtual IEnumerable<String> ListarColunasDasTabelas(String tabela, Boolean listarDetalhes)
		{
			var dataReader = ExecutarQuery(String.Format(AllColumnsSQL, tabela));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Transformar(dataReader, listarDetalhes);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		private String Transformar(IDataReader dataReader, Boolean listarDetalhes)
		{
			return Convert.ToString(dataReader["Nome"]) + (listarDetalhes ? Convert.ToString(dataReader["Detalhes"]) : String.Empty);
		}

		public virtual IEnumerable<String> ListarTabelas(String tabela)
		{
			var dataReader = ExecutarQuery(String.Format(AllTablesSQL, tabela));
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
			var dataReader = ExecutarQuery(String.Format(AllViewsSQL, view));
			if (dataReader != null)
			{
				while ((!dataReader.IsClosed) && dataReader.Read())
					yield return Convert.ToString(dataReader["Nome"]);
				dataReader.Close();
				dataReader.Dispose();
			}
		}

		public virtual IDataReader ExecutarQuery(String query)
		{
			Free();
			_iDbCommand = _iDbConnection.CriarComando(query);
			_iDataReader = _iDbCommand.ExecuteReader();
			return _iDataReader;
		}

		public virtual void Executar(String query, IMessageResult messageResult)
		{
			_tipo = ClasseDinamica.CriarTipoVirtual(ExecutarQuery(query), messageResult);
		}

		public virtual IEnumerable<Object> Transformar()
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

		public virtual IEnumerable<Object> Cabecalho()
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
				catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
				finally { _iDbConnection.Dispose(); }
				_iDbConnection = null;
			}
		}

		protected virtual TIDbConnection AbrirConexao()
		{
			return AbrirConexao(false);
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

		private void Free()
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

		public virtual void ShowLog(String message, String tipo)
		{

		}
	}
}