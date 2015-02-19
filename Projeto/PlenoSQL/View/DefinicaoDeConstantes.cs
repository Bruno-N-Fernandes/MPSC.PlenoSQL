using MPSC.PlenoSQL.AppWin.Infra;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class DefinicaoDeConstantes : Form
	{
		private Constantes _constantes;
		private String _escopo;
		private String Escopo { get { return (cbEscopo.SelectedIndex == 0) ? Constante.GLOBAL : _escopo; } }

		public DefinicaoDeConstantes()
		{
			InitializeComponent();
		}

		public void Carregar(Constantes constantes, String escopo)
		{
			_constantes = constantes;
			_escopo = escopo;
			cbEscopo.DataSource = new String[] { Constante.GLOBAL, Path.GetFileNameWithoutExtension(_escopo) };
			UpdateDataSource();
			ShowDialog();
		}

		private void btIncluir_Click(object sender, EventArgs e)
		{
			_constantes.Adicionar(txtNome.Text, txtValor.Text, null, Escopo);
			UpdateDataSource();
		}


		private void btExcluir_Click(object sender, EventArgs e)
		{
			var row = dgConstantes.Rows[dgConstantes.SelectedCells[0].RowIndex];
			_constantes.Remover(row.Cells[0].Value.ToString(), row.Cells[3].Value.ToString());
			UpdateDataSource();
		}

		private void ckLocalOnly_CheckedChanged(object sender, EventArgs e)
		{
			UpdateDataSource();
		}

		private void UpdateDataSource()
		{
			dgConstantes.DataSource = null;
			dgConstantes.DataSource = (ckGlobalOnly.Checked ? _constantes.Obter(null) : _constantes.Obter(_escopo)).ToList();
		}
	}
}