using System;

namespace MP.PlenoBDNE.AppWin.View.Interface
{
	public interface INavegador
	{
		Boolean SalvarAoExecutar { get; }
		Boolean ConvertToUpper { get; }
		Boolean Colorir { get; }
	}

	public class NavegadorNulo : INavegador
	{
		private static NavegadorNulo _instancia;
		public static NavegadorNulo Instancia { get { return (_instancia ?? (_instancia = new NavegadorNulo())); } }
		private NavegadorNulo() { }

		public Boolean SalvarAoExecutar { get { return false; } }
		public Boolean ConvertToUpper { get { return false; } }
		public Boolean Colorir { get { return false; } }
	}
}