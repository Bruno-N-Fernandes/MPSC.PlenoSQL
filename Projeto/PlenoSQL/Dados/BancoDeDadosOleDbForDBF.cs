using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MP.PlenoBDNE.AppWin.Infra;
using System.Data;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForDBF : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "OleDb For DBF Files"; } }
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;"; } }

		public override IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0}" : "[{0}]";
			var files = Directory.GetFiles(@"C:\", "*.dbf", SearchOption.TopDirectoryOnly);
			return files.Select(f => new FileInfo(f)).Where(f => String.IsNullOrEmpty(nome) || f.Name.ToUpper().Contains(nome.ToUpper())).Select(f => String.Format(format, f.Name));
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
			if (tipo == 130)
			{
				retorno += String.Format("VarChar({0})", linha.Get(Field.CHARACTER_MAXIMUM_LENGTH));
			}
			else if (tipo == 5)
			{
				retorno += String.Format("Decimal({0},{1})", linha.Get(Field.NUMERIC_PRECISION), linha.Get(Field.NUMERIC_SCALE));
			}
			else
				retorno += tipo.ToString();

			return retorno;
		}
	}
}