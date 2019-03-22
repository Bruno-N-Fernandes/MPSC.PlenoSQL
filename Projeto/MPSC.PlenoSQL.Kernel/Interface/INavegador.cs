using System;

namespace MPSC.PlenoSQL.Kernel.Interface
{
	public interface INavegador
	{
		Boolean ShowEstatisticas { get; }
		Boolean SalvarAoExecutar { get; }
		Boolean ConverterToUpper { get; }
		Boolean ColorirTextosSql { get; }
		void Status(String mensagem);
	}

	public class NavegadorNulo : INavegador
	{
		private static NavegadorNulo _instancia;
		public static NavegadorNulo Instancia { get { return (_instancia ?? (_instancia = new NavegadorNulo())); } }
		private NavegadorNulo() { }

		public Boolean ShowEstatisticas { get { return false; } }
		public Boolean SalvarAoExecutar { get { return false; } }
		public Boolean ConverterToUpper { get { return false; } }
		public Boolean ColorirTextosSql { get { return false; } }
		public void Status(String mensagem) { }
	}
}