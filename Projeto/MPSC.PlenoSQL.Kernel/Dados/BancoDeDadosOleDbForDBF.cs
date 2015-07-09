using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("OleDb For DBF Files")]
	public class BancoDeDadosOleDbForDBF : BancoDeDadosOleDb
	{
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