using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MPSC.PlenoBDNE.AppWin.Infra;
using System.Data;

namespace MPSC.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForDBF : BancoDeDadosOleDb
	{
		public override String Descricao { get { return "OleDb For DBF Files"; } }
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;"; } }

		public override IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0} ({1}{2})" : "[{0}]";
			var files = Directory.GetFiles(_server, "*.dbf", SearchOption.TopDirectoryOnly);
			return files.Select(f => new FileInfo(f))
				.Where(f => String.IsNullOrEmpty(nome) || f.Name.ToUpper().Contains(nome.ToUpper()))
				.Select(f => String.Format(format, Path.GetFileNameWithoutExtension(f.Name), f.Directory.FullName, f.Name));
		}
	}
}