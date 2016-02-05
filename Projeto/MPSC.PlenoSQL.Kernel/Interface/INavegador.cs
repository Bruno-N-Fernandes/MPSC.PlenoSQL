using System;

namespace MPSC.PlenoSQL.Kernel.Interface
{
	public interface INavegador
	{
		Boolean MostrarEstatisticas { get; }
		Boolean SalvarAoExecutar { get; }
		Boolean ConvertToUpper { get; }
		Boolean Colorir { get; }
		void Status(String mensagem);
	}

	public class NavegadorNulo : INavegador
	{
		private static NavegadorNulo _instancia;
		public static NavegadorNulo Instancia { get { return (_instancia ?? (_instancia = new NavegadorNulo())); } }
		private NavegadorNulo() { }

		public Boolean MostrarEstatisticas { get { return false; } }
		public Boolean SalvarAoExecutar { get { return false; } }
		public Boolean ConvertToUpper { get { return false; } }
		public Boolean Colorir { get { return false; } }
		public void Status(String mensagem) { }
	}
}