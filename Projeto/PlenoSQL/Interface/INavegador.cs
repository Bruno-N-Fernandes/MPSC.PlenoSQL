using System;

namespace MPSC.PlenoSQL.AppWin.Interface
{
	public interface INavegador
	{
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

		public Boolean SalvarAoExecutar { get { return false; } }
		public Boolean ConvertToUpper { get { return false; } }
		public Boolean Colorir { get { return false; } }
		public void Status(String mensagem) { }
	}
}