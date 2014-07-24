using System;
using System.Data;
using System.Windows.Forms;
using LBJC.NavegadorDeDados.Dados;
using LBJC.NavegadorDeDados.Infra;

namespace LBJC.NavegadorDeDados.View
{
	public partial class Autenticacao : Form
	{
		private const String arquivoConfig = "Autenticacao.txt";
		private IBancoDeDados _bancoDeDados;

		public Autenticacao()
		{
			InitializeComponent();
		}

		private void Autenticacao_Load(object sender, EventArgs e)
		{
			cbTipoBanco.DataSource = BancoDeDados<IDbConnection>.ListaDeBancoDeDados;
			var config = Util.FileToArray(arquivoConfig);
			cbTipoBanco.SelectedIndex = Convert.ToInt32("0" + config[0]);
			txtServidor.Text = config[1];
			txtUsuario.Text = config[2];
			cbBancoSchema.Text = config[3];
		}

		private void Autenticacao_Shown(object sender, EventArgs e)
		{
			var f = Foco(cbTipoBanco) || Foco(txtServidor) || Foco(txtUsuario) || Foco(txtSenha) || Foco(cbBancoSchema);
		}

		private bool Foco(Control control)
		{
			return String.IsNullOrWhiteSpace(control.Text) && control.Focus();
		}

		private void Autenticacao_FormClosed(object sender, FormClosedEventArgs e)
		{
			Util.ArrayToFile(arquivoConfig, cbTipoBanco.SelectedIndex.ToString(), txtServidor.Text, txtUsuario.Text, cbBancoSchema.Text);
			cbTipoBanco.DataSource = null;
		}

		private void btConectar_Click(object sender, EventArgs e)
		{
			_bancoDeDados = cbTipoBanco.SelectedValue as IBancoDeDados;
			if (_bancoDeDados != null)
			{
				IDbConnection iDbConnection = null;
				try
				{
					iDbConnection = _bancoDeDados.ObterConexao(txtServidor.Text, cbBancoSchema.Text, txtUsuario.Text, txtSenha.Text);
					if (iDbConnection != null)
					{
						iDbConnection.Open();
						iDbConnection.Close();
						DialogResult = DialogResult.OK;
					}
				}
				catch (Exception exception)
				{
					MessageBox.Show("Houve um problema ao tentar conectar ao banco de dados. Detalhes:\n\n" + exception.Message, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				finally
				{
					if (iDbConnection != null)
						iDbConnection.Dispose();
					iDbConnection = null;
				}
			}
		}

		public static IBancoDeDados Dialog()
		{
			IBancoDeDados iBancoDeDados = null;
			var autenticacao = new Autenticacao();
			if (autenticacao.ShowDialog() == DialogResult.OK)
				iBancoDeDados = autenticacao._bancoDeDados;
			autenticacao.Close();
			autenticacao.Dispose();
			autenticacao = null;
			return iBancoDeDados;
		}
	}
}