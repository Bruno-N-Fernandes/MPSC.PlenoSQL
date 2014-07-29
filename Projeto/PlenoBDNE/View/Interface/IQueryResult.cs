using System;

namespace MP.PlenoBDNE.AppWin.View.Interface
{
	public interface IQueryResult
	{
		String NomeDoArquivo { get; }
		void Executar();
		Boolean Salvar();
		Boolean PodeFechar();
		void Fechar();
		Boolean Focus();
	}

	public class NullQueryResult : IQueryResult
	{
		public String NomeDoArquivo { get { return null; } }
		public void Executar() { }
		public Boolean Focus() { return false; }
		public Boolean Salvar() { return false; }
		public Boolean PodeFechar() { return true; }
		public void Fechar() { _instance = null; }

		private static IQueryResult _instance;
		public static IQueryResult Instance { get { return _instance ?? (_instance = new NullQueryResult()); } }
	}
}