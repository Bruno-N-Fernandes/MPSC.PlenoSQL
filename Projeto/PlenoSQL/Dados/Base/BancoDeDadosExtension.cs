using MPSC.PlenoSQL.AppWin.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.Dados.Base
{
	public static class BancoDeDadosExtension
	{
		private static readonly Type attributeDisplayName = typeof(DisplayNameAttribute);
		private static List<KeyValuePair<String, Type>> _lista;
		public static IEnumerable<KeyValuePair<String, Type>> ListaDeBancoDeDados { get { return _lista ?? (_lista = LoadEnum().ToList()); } }

		private static IEnumerable<KeyValuePair<String, Type>> LoadEnum()
		{
			yield return LoadBanco<BancoDeDadosSQLServer>();
			yield return LoadBanco<BancoDeDadosSQLite>();
			yield return LoadBanco<BancoDeDadosIBMDB2>();
			yield return LoadBanco<BancoDeDadosFireBird>();
			yield return LoadBanco<BancoDeDadosOleDbForIBM_DB2>();
			yield return LoadBanco<BancoDeDadosOleDbForExcel>();
			yield return LoadBanco<BancoDeDadosOleDbForAccess>();
			yield return LoadBanco<BancoDeDadosOleDbForDBF>();
		}

		private static KeyValuePair<String, Type> LoadBanco<TIBancoDeDados>() where TIBancoDeDados : class, IBancoDeDados
		{
			var tipo = typeof(TIBancoDeDados);
			Application.DoEvents();
			return new KeyValuePair<String, Type>(tipo.DisplayName(), tipo);
		}

		public static String DisplayName(this Type tipo)
		{
			var displayNameAttribute = tipo.GetCustomAttributes(attributeDisplayName, false).Select(a => a as DisplayNameAttribute).FirstOrDefault(a => a != null);
			return (displayNameAttribute != null) ? displayNameAttribute.DisplayName ?? tipo.Name : tipo.Name;
		}

		public static IDbCommand CriarComando(this IDbConnection iDbConnection, String query)
		{
			IDbCommand iDbCommand = null;
			if ((iDbConnection != null) && !String.IsNullOrWhiteSpace(query))
			{
				if (iDbConnection.State != ConnectionState.Open)
				{
					lock (String.Empty)
					{
						if (iDbConnection.State != ConnectionState.Open)
							iDbConnection.Open();
					}
				}

				iDbCommand = iDbConnection.CreateCommand();
				iDbCommand.CommandText = query;
				iDbCommand.CommandType = query.ToLower().StartsWith("exec") || (query.IndexOfAny("\r\n\t ".ToCharArray()) < 0) ? CommandType.StoredProcedure : CommandType.Text;
				iDbCommand.CommandTimeout = 3600;
			}
			return iDbCommand;
		}

		public static Int32 ExecuteNonQuery(this IDbConnection iDbConnection, String cmdSql)
		{
			var iDbCommand = iDbConnection.CriarComando(cmdSql);
			var retorno = iDbCommand.ExecuteNonQuery();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
			return retorno;
		}

		public static Object ExecuteScalar(this IDbConnection iDbConnection, String cmdSql)
		{
			var iDbCommand = iDbConnection.CriarComando(cmdSql);
			var retorno = iDbCommand.ExecuteScalar();
			iDbCommand.Dispose();
			iDbConnection.Close();
			iDbConnection.Dispose();
			return retorno;
		}

		public static IDbDataParameter AdicionarParametro(this IDbCommand iDbCommand, String parameterName, Object value, DbType dbType)
		{
			return iDbCommand.AdicionarParametro(parameterName, value, dbType, ParameterDirection.Input);
		}

		public static IDbDataParameter AdicionarParametro(this IDbCommand iDbCommand, String parameterName, Object value, DbType dbType, ParameterDirection parameterDirection)
		{
			var iDbDataParameter = iDbCommand.CreateParameter();
			iDbDataParameter.ParameterName = parameterName;
			iDbDataParameter.Value = value;
			iDbDataParameter.DbType = dbType;
			iDbDataParameter.Direction = parameterDirection;
			iDbCommand.Parameters.Add(iDbDataParameter);
			return iDbDataParameter;
		}

		public static Boolean IsOpen(this IDataReader iDataReader)
		{
			return (iDataReader != null) && !iDataReader.IsClosed;
		}

		public static Int16 GetInt16(this IDataReader iDataReader, String name)
		{
			return iDataReader.GetInt16(iDataReader.GetOrdinal(name));
		}

		public static Int32 GetInt32(this IDataReader iDataReader, String name)
		{
			return iDataReader.GetInt32(iDataReader.GetOrdinal(name));
		}

		public static Int64 GetInt64(this IDataReader iDataReader, String name)
		{
			return iDataReader.GetInt64(iDataReader.GetOrdinal(name));
		}

		public static String GetString(this IDataReader iDataReader, String name)
		{
			return iDataReader.GetString(iDataReader.GetOrdinal(name));
		}

		public static Boolean GetBoolean(this IDataReader iDataReader, String name)
		{
			return iDataReader.GetBoolean(iDataReader.GetOrdinal(name));
		}
	}
}