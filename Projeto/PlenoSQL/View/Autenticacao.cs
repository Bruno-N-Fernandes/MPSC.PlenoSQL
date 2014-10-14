using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados.Base;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;
using MP.PlenoSQL.AppWin.Infra;

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
			cbTipoBanco.DataSource = BancoDeDadosExtension.ListaDeBancoDeDados.ToList();
			var conexao = Parametro.Instancia.Conexoes.FirstOrDefault(c => c.Id == 1) ?? Parametro.Instancia.Conexoes.FirstOrDefault();
			Configurar(conexao);
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
				Parametro.Instancia.NovaConexao(cbTipoBanco.SelectedIndex, txtServidor.Text, txtUsuario.Text, txtSenha.Text, cbBancoSchema.Text, ckSalvarSenha.Checked).Save();
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

		private void txtServidor_KeyUp(object sender, KeyEventArgs e)
		{
			var pesquisa = txtServidor.Text;
			if ((e.KeyCode == Keys.Back) && (pesquisa.Length > 0))
				pesquisa = pesquisa.Substring(0, pesquisa.Length - 1);

			if (!String.IsNullOrWhiteSpace(pesquisa))
			{
				var conexao = Parametro.Instancia.Conexoes
					.Where(c => c.TipoBanco == cbTipoBanco.SelectedIndex)
					.Where(c => c.Servidor.ToUpper().StartsWith(pesquisa.ToUpper()))
					.FirstOrDefault();

				if (conexao != null)
				{
					Configurar(conexao);
					txtServidor.SelectionStart = pesquisa.Length;
					txtServidor.SelectionLength = txtServidor.Text.Length - pesquisa.Length;
				}
				else
				{
					txtUsuario.Text = String.Empty;
					cbBancoSchema.DataSource = null;
					cbBancoSchema.Text = String.Empty;
					txtSenha.Text = String.Empty;
					ckSalvarSenha.Checked = false;
				}
			}
		}

		private void Configurar(Parametro.Conexao conexao)
		{
			if (conexao != null)
			{
				cbTipoBanco.SelectedIndex = conexao.TipoBanco;
				txtServidor.Text = conexao.Servidor;
				txtUsuario.Text = conexao.Usuario;
				cbBancoSchema.Text = conexao.Banco;
				txtSenha.Text = conexao.SalvarSenha ? conexao.Senha : String.Empty;
				ckSalvarSenha.Checked = conexao.SalvarSenha;
			}
		}
	}
}