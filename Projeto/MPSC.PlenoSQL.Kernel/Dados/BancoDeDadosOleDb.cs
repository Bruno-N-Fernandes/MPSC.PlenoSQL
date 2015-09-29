using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Infra;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	public abstract class BancoDeDadosOleDb : BancoDeDados<OleDbConnection>
	{
		protected override String SQLSelectCountTemplate(String query) { return String.Format("Select Count(*) From ({0}) As ViewOfSelectCountFrom", query); }
		protected override String SQLAllDatabases(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLAllProcedures(String nome, Boolean comDetalhes) { return String.Empty; }
		protected override String SQLTablesColumns { get { return String.Empty; } }

		public override IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0}" : "[{0}]";
			var schema = GetSchema("Tables");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var tb = Convert.ToString(schema.Rows[i][2]);
				if (tb.Contains("$") && (String.IsNullOrWhiteSpace(nome) || tb.ToUpper().StartsWith(nome.ToUpper())))
					yield return String.Format(format, tb);
			}
		}

		public override IEnumerable<String> ListarViews(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0}" : "[{0}]";
			var schema = GetSchema("Views");
			for (int i = 0; (schema != null) && (i < schema.Rows.Count); i++)
			{
				var vw = Convert.ToString(schema.Rows[i][2]);
				if (vw.Contains("$") && (String.IsNullOrWhiteSpace(nome) || vw.ToUpper().StartsWith(nome.ToUpper())))
					yield return String.Format(format, vw);
			}
		}

		public override IEnumerable<String> ListarColunas(String parent, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0} ({1}, {2})" : "{0}";
			parent = new FileInfo(parent).Name.ToUpper();
			var rows = GetSchema("Columns").Rows;
			for (int i = 0; (rows != null) && (i < rows.Count); i++)
			{
				var linha = rows[i];
				var tabela = Convert.ToString(linha.Get(Field.TABLE_NAME)).ToUpper();
				if (tabela.StartsWith(parent))
					yield return String.Format(format, linha.Get(Field.COLUMN_NAME), ObterTipo(linha), Convert.ToBoolean(linha.Get(Field.IS_NULLABLE)) ? "Anulável" : "Obrigatório");
			}
		}

		private String ObterTipo(DataRow linha)
		{
			var retorno = String.Empty;
			var tipo = Convert.ToInt32(linha.Get(Field.DATA_TYPE));
			if (tipo == 129)
			{
				retorno += String.Format("Char({0})", linha.Get(Field.CHARACTER_MAXIMUM_LENGTH));
			}
			else if (tipo == 130)
			{
				retorno += String.Format("VarChar({0})", linha.Get(Field.CHARACTER_MAXIMUM_LENGTH));
			}
			else if ((tipo == 5) || (tipo == 131))
			{
				retorno += String.Format("Decimal({0},{1})", linha.Get(Field.NUMERIC_PRECISION), linha.Get(Field.NUMERIC_SCALE));
			}
			else
				retorno += tipo.ToString();

			return retorno;
		}
	}

	internal enum Field
	{
		TABLE_CATALOG = 0,
		TABLE_SCHEMA = 1,
		TABLE_NAME = 2,
		COLUMN_NAME = 3,
		COLUMN_GUID = 4,
		COLUMN_PROPID = 5,
		ORDINAL_POSITION = 6,
		COLUMN_HASDEFAULT = 7,
		COLUMN_DEFAULT = 8,
		COLUMN_FLAGS = 9,
		IS_NULLABLE = 10,
		DATA_TYPE = 11,
		TYPE_GUID = 12,
		CHARACTER_MAXIMUM_LENGTH = 13,
		CHARACTER_OCTET_LENGTH = 14,
		NUMERIC_PRECISION = 15,
		NUMERIC_SCALE = 16,
		DATETIME_PRECISION = 17,
		CHARACTER_SET_CATALOG = 18,
		CHARACTER_SET_SCHEMA = 19,
		CHARACTER_SET_NAME = 20,
		COLLATION_CATALOG = 21,
		COLLATION_SCHEMA = 22,
		COLLATION_NAME = 23,
		DOMAIN_CATALOG = 24,
		DOMAIN_SCHEMA = 25,
		DOMAIN_NAME = 26,
		DESCRIPTION = 27,
	}

}