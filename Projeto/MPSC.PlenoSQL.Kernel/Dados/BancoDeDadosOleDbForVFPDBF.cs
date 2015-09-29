using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MPSC.PlenoSQL.Kernel.Dados
{
	[DisplayName("OleDb For DBF VFP Files")]
    public class BancoDeDadosOleDbForVFPDBF : BancoDeDadosOleDb
	{
        protected override String StringConexaoTemplate { get { return @"Provider=VFPOLEDB;Data Source={0};"; } }

		public override IEnumerable<String> ListarTabelas(String nome, Boolean comDetalhes)
		{
			var format = comDetalhes ? "{0} ({1}\\{2})" : "[{0}]";
			var files = Directory.GetFiles(_server, "*.dbf", SearchOption.TopDirectoryOnly);
			return files.Select(f => new FileInfo(f))
				.Where(f => String.IsNullOrEmpty(nome) || f.Name.ToUpper().Contains(nome.ToUpper()))
				.Select(f => String.Format(format, Path.GetFileNameWithoutExtension(f.Name), f.Directory.FullName, f.Name));
		}
	}
}