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
			Show();
			_constantes = constantes;
			_escopo = escopo;
			cbEscopo.DataSource = new String[] { Path.GetFileName(_escopo), Constante.GLOBAL };
			UpdateDataSource();
		}

		private void btIncluir_Click(object sender, EventArgs e)
		{
			_constantes.Adicionar(Escopo, txtNome.Text, txtValor.Text);
			UpdateDataSource();
		}

		private void btExcluir_Click(object sender, EventArgs e)
		{
			var constante = dgConstantes.CurrentRow.DataBoundItem as Constante;
			if (constante != null)
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
			Application.DoEvents();
			dgConstantes.AutoResizeColumns();
		}

		private void DefinicaoDeConstantes_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Visible = false;
		}

		private void btFechar_Click(object sender, EventArgs e)
		{
			Visible = false;
		}

		private void DefinicaoDeConstantes_Activated(object sender, EventArgs e)
		{
			Opacity = 1;
		}

		private void DefinicaoDeConstantes_Deactivate(object sender, EventArgs e)
		{
			Opacity = 0.25;
		}

		private void dgConstantes_SelectionChanged(object sender, EventArgs e)
		{
			if (dgConstantes.CurrentRow != null)
			{
				var constante = dgConstantes.CurrentRow.DataBoundItem as Constante;
				if (constante != null)
				{
					txtNome.Text = constante.Nome;
					txtValor.Text = constante.Valor;
				}
			}
		}

		private static DefinicaoDeConstantes _instancia;
		public static void Visualizar(Constantes constantes, String nomeDoArquivo)
		{
			_instancia = _instancia ?? new DefinicaoDeConstantes();
			_instancia.Carregar(constantes, nomeDoArquivo);
		}
	}
}