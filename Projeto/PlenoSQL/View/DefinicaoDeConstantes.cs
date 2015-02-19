using MPSC.PlenoSQL.AppWin.Infra;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class DefinicaoDeConstantes : Form
	{
		public DefinicaoDeConstantes()
		{
			InitializeComponent();
		}

		public void Carregar(IEnumerable<Constante> constantes)
		{
			dgConstantes.DataSource = constantes.ToList();
			ShowDialog();
		}
	}
}