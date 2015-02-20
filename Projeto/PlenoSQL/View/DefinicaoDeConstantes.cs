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

		private String Escopo { get { return (cbEscopo.SelectedIndex == 0) ? _escopo : Constante.GLOBAL; } }
		public Constantes.Filtro Filtro
		{
			get
			{
				return gbFiltro.Controls.Cast<RadioButton>().Where(r => r.Checked).Select(r => (Constantes.Filtro)Convert.ToInt32(r.Tag)).FirstOrDefault();
			}
		}

		public DefinicaoDeConstantes()
		{
			InitializeComponent();
			rbAtivas.Checked = true;
		}

		public void Carregar(Constantes constantes, String escopo)
		{
			_constantes = constantes;
			_escopo = escopo;
			cbEscopo.DataSource = new String[] { Path.GetFileName(_escopo), Constante.GLOBAL };
			UpdateDataSource();
			ShowDialog();
		}

		private void btIncluir_Click(object sender, EventArgs e)
		{
			_constantes.Adicionar(Escopo, txtNome.Text, txtValor.Text);
			UpdateDataSource();
		}

		private void btExcluir_Click(object sender, EventArgs e)
		{
			var constante = dgConstantes.CurrentRow.DataBoundItem as Constante;
			_constantes.Remover(constante.Nome, constante.escopo);
			UpdateDataSource();
		}

		private void filtroChanged(object sender, EventArgs e)
		{
			UpdateDataSource();
		}

		private void UpdateDataSource()
		{
			dgConstantes.DataSource = null;
			if (_constantes != null)
				dgConstantes.DataSource = _constantes.Obter(_escopo, Filtro).ToList();
			dgConstantes.AutoResizeColumns();
		}

		public static void Visualizar(Constantes constantes, String nomeDoArquivo)
		{
			var definicaoDeConstantes = new DefinicaoDeConstantes();
			definicaoDeConstantes.Carregar(constantes, nomeDoArquivo);
		}
	}
}