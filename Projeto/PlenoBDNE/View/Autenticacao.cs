using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados.Base;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class Autenticacao : Form
	{
		private static readonly String arquivoConfig = Path.GetTempPath() + "NavegadorDeDados.Auth";
		private IBancoDeDados _bancoDeDados;

		private Autenticacao()
		{
			InitializeComponent();
		}

		private void Autenticacao_Load(object sender, EventArgs e)
		{
			var config = FileUtil.FileToArray(arquivoConfig, 5);
			cbTipoBanco.DataSource = BancoDeDadosExtension.ListaDeBancoDeDados;
			cbTipoBanco.SelectedIndex = Convert.ToInt32("0" + config[0]);
			txtServidor.Text = config[1];
			txtUsuario.Text = config[2];
			cbBancoSchema.Text = config[3];
			txtSenha.Text = config[4];
		}

		private void Autenticacao_Shown(object sender, EventArgs e)
		{
			var f = Foco(cbTipoBanco) || Foco(txtServidor) || Foco(txtUsuario) || Foco(txtSenha) || Foco(cbBancoSchema) || ckSalvarSenha.Focus();
		}

		private bool Foco(Control control)
		{
			return String.IsNullOrWhiteSpace(control.Text) && control.Focus();
		}

		private void Autenticacao_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DialogResult == DialogResult.OK)
				FileUtil.ArrayToFile(arquivoConfig, cbTipoBanco.SelectedIndex.ToString(), txtServidor.Text, txtUsuario.Text, cbBancoSchema.Text, (ckSalvarSenha.Checked ? txtSenha.Text : String.Empty));
			cbTipoBanco.DataSource = null;
		}

		private void btConectar_Click(object sender, EventArgs e)
		{
			if (ObterBancoDeDados(cbBancoSchema.Text))
			{
				var result = _bancoDeDados.TestarConexao();
				if (String.IsNullOrWhiteSpace(result))
					DialogResult = DialogResult.OK;
				else
					MessageBox.Show(result, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private Boolean ObterBancoDeDados(String bancoDeDados)
		{
			var tipo = cbTipoBanco.SelectedValue as Type;
			if (tipo != null)
			{
				if (_bancoDeDados != null)
					_bancoDeDados.Dispose();
				_bancoDeDados = null;
				_bancoDeDados = Activator.CreateInstance(tipo) as IBancoDeDados;
				_bancoDeDados.ConfigurarConexao(txtServidor.Text, bancoDeDados, txtUsuario.Text, txtSenha.Text);
			}
			return _bancoDeDados != null;
		}

		public static IBancoDeDados Dialog(IMessageResult iMessageResult)
		{
			IBancoDeDados iBancoDeDados = null;
			var autenticacao = new Autenticacao();
			if (autenticacao.ShowDialog() == DialogResult.OK)
			{
				iBancoDeDados = autenticacao._bancoDeDados;
				iBancoDeDados.SetMessageResult(iMessageResult);
			}
			autenticacao.Close();
			autenticacao.Dispose();
			autenticacao = null;
			return iBancoDeDados;
		}

		private void cbBancoSchema_DropDown(object sender, EventArgs e)
		{
			if (cbBancoSchema.Items.Count == 0)
			{
				if (ObterBancoDeDados(String.Empty))
				{
					cbBancoSchema.DataSource = _bancoDeDados.ListarBancosDeDados(cbBancoSchema.Text, false).OrderBy(b => b).ToList();
				}
			}
		}
	}
}