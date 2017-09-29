using System;
using System.IO;

namespace MPSC.PlenoSQL.Kernel.Interface
{
	public interface IQueryResult
	{
		FileInfo Arquivo { get; }
		void Executar();
		void GerarExcel();
		void AlterarConexao();
		Boolean Salvar();
		Boolean PodeFechar();
		void Fechar();
		Boolean Focus();
	}

	public class NullQueryResult : IQueryResult
	{
		public static readonly FileInfo NullFileInfo = new FileInfo(Environment.GetEnvironmentVariable("TEMP") + "\\Query.sql");
		public FileInfo Arquivo { get { return NullFileInfo; } }
		public void Executar() { }
		public void GerarExcel() { }
		public void AlterarConexao() { }
		public Boolean Focus() { return false; }
		public Boolean Salvar() { return false; }
		public Boolean PodeFechar() { return true; }
		public void Fechar() { }

		private static IQueryResult _instance;
		public static IQueryResult Instance { get { return _instance ?? (_instance = new NullQueryResult()); } }
	}
}