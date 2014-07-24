using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using IBM.Data.DB2.iSeries;
using LBJC.NavegadorDeDados.View;

namespace LBJC.NavegadorDeDados.Dados
{
	public abstract class BancoDeDados<TIDbConnection> : IBancoDeDados where TIDbConnection : class, IDbConnection
	{
		public static IList<IBancoDeDados> ListaDeBancoDeDados = Load();

		private Type _tipo = null;
		private TIDbConnection iDbConnection = null;
		private IDbCommand iDbCommand = null;
		private IDataReader iDataReader = null;

		protected abstract String StringConexaoTemplate { get; }
		public abstract String Descricao { get; }
		public abstract String AllTablesSQL { get; }

		public IDbConnection ObterConexao(String server, String dataBase, String usuario, String senha)
		{
			return iDbConnection = CriarNovaConexao(typeof(TIDbConnection), server, dataBase, usuario, senha);
		}

		protected virtual TIDbConnection CriarNovaConexao(Type tipo, String server, String dataBase, String usuario, String senha)
		{
			var stringConexao = String.Format(StringConexaoTemplate, server, dataBase, usuario, senha);
			return Activator.CreateInstance(tipo, stringConexao) as TIDbConnection;
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
			iDbCommand = CriarComando(iDbConnection, query);
			iDataReader = iDbCommand.ExecuteReader();
			return iDataReader;
		}

		public void Executar(String query)
		{
			_tipo = ClasseDinamica.CriarTipoVirtual(ExecutarQuery(query));
		}

		public IEnumerable<Object> Transformar()
		{
			var linhas = -1;
			while ((iDataReader != null) && !iDataReader.IsClosed && (++linhas < 100) && iDataReader.Read())
				yield return ClasseDinamica.CreateObjetoVirtual(_tipo, iDataReader);

			if (linhas == 0)
			{
				iDataReader.Close();
				iDataReader.Dispose();
				iDataReader = null;
			}
			else if ((linhas < 100) && (iDataReader != null) && !iDataReader.IsClosed && !iDataReader.Read())
			{
				iDataReader.Close();
				iDataReader.Dispose();
				iDataReader = null;
			}
		}

		public IEnumerable<Object> Cabecalho()
		{
			yield return ClasseDinamica.CreateObjetoVirtual(_tipo, null);
		}

		public virtual void Dispose()
		{
			Free();

			if (iDbConnection != null)
			{
				try
				{
					if (iDbConnection.State != ConnectionState.Closed)
						iDbConnection.Close();
				}
				catch (Exception) { }
				finally { iDbConnection.Dispose(); }
				iDbConnection = null;
			}

			ListaDeBancoDeDados.Clear();
			ListaDeBancoDeDados = null;
		}

		private void Free()
		{
			if (iDataReader != null)
			{
				try
				{
					if (!iDataReader.IsClosed)
						iDataReader.Close();
				}
				catch (Exception) { }
				finally { iDataReader.Dispose(); }
				iDataReader = null;
			}

			if (iDbCommand != null)
			{
				try
				{
					iDbCommand.Cancel();
				}
				catch (Exception) { }
				finally { iDbCommand.Dispose(); }
				iDbCommand = null;
			}
		}

		public static IBancoDeDados Conectar()
		{
			return Autenticacao.Dialog();
		}

		private static IDbCommand CriarComando(IDbConnection iDbConnection, String query)
		{
			if (iDbConnection.State != ConnectionState.Open)
				iDbConnection.Open();
			IDbCommand iDbCommand = iDbConnection.CreateCommand();
			iDbCommand.CommandText = query;
			iDbCommand.CommandType = CommandType.Text;
			iDbCommand.CommandTimeout = 60;
			return iDbCommand;
		}

		private static IList<IBancoDeDados> Load()
		{
			return new List<IBancoDeDados>(
				new IBancoDeDados[]
				{
					new SQLServer(),
					new OleDb(),
					new IBMDB2()
				}
			);
		}
	}

	public class IBMDB2 : BancoDeDados<iDB2Connection>
	{
		public override String Descricao { get { return "IBM DB2"; } }
		public override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables Where (Table_Name Like '{0}%')"; } }
		protected override String StringConexaoTemplate { get { return "DataSource={0};UserID={2};Password={3};DataCompression=True;SortSequence=SharedWeight;SortLanguageId=PTG;DefaultCollection={1};"; } }
	}

	public class OleDb : BancoDeDados<OleDbConnection>
	{
		public override String Descricao { get { return "Ole DB"; } }
		public override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables"; } }
		protected override String StringConexaoTemplate { get { return "Provider=IBMDA400;Data Source={0};Default Collection={1};User ID={2};Password={3}"; } }
	}

	public class SQLServer : BancoDeDados<SqlConnection>
	{
		public override String Descricao { get { return "Sql Server"; } }
		public override String AllTablesSQL { get { return "Select Name as Tabela, Owner as Banco, Name as NomeInterno From SysTables"; } }
		protected override String StringConexaoTemplate { get { return "Persist Security Info=True;Data Source={0};Initial Catalog={1};User ID={2};Password={3};MultipleActiveResultSets=True;"; } }
	}
}