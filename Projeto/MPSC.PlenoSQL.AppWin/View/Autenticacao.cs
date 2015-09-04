using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Infra;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class Autenticacao : Form
	{
		private IBancoDeDados _bancoDeDados;

		private Autenticacao()
		{
			InitializeComponent();
		}

		private void Autenticacao_Load(object sender, EventArgs e)
		{
			cbTipoBanco.DataSource = BancoDeDadosExtension.ListaDeBancoDeDados.ToList();
			var conexao = Configuracao.Instancia.Conexoes.FirstOrDefault(c => c.Id == 1) ?? Configuracao.Instancia.Conexoes.FirstOrDefault();
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
				Configuracao.Instancia.NovaConexao(cbTipoBanco.SelectedIndex, txtServidor.Text, txtUsuario.Text, txtSenha.Text, cbBancoSchema.Text, ckSalvarSenha.Checked).SaveConexao();
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
				iBancoDeDados.PreencherCache();
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

		private void cbTipoBanco_SelectedIndexChanged(object sender, EventArgs e)
		{
			Limpar(1);
		}

		private void txtServidor_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode != Keys.Back) && (e.KeyCode != Keys.ShiftKey) && (e.KeyCode != Keys.ControlKey) && (e.KeyCode != Keys.Alt))
				AutoCompletarServidor(txtServidor.Text);
		}

		private void txtUsuario_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyCode != Keys.Back) && (e.KeyCode != Keys.ShiftKey) && (e.KeyCode != Keys.ControlKey) && (e.KeyCode != Keys.Alt))
				AutoCompletarUsuario(txtUsuario.Text);
		}

		private void AutoCompletarServidor(string pesquisa)
		{
			if (!String.IsNullOrWhiteSpace(pesquisa))
			{
				var conexao = Configuracao.Instancia.Conexoes
					.Where(c => c.TipoBanco == cbTipoBanco.SelectedIndex)
					.Where(c => c.Servidor.ToUpper().StartsWith(pesquisa.ToUpper()))
					.FirstOrDefault();

				Configurar(conexao);
				txtServidor.SelectionStart = pesquisa.Length;
				txtServidor.SelectionLength = txtServidor.Text.Length - pesquisa.Length;
			}
		}

		private void AutoCompletarUsuario(string pesquisa)
		{
			if (!String.IsNullOrWhiteSpace(pesquisa))
			{
				var conexao = Configuracao.Instancia.Conexoes
					.Where(c => c.TipoBanco == cbTipoBanco.SelectedIndex)
					.Where(c => c.Servidor.ToUpper().Equals(txtServidor.Text.ToUpper()))
					.Where(c => c.Usuario.ToUpper().StartsWith(pesquisa.ToUpper()))
					.FirstOrDefault();

				Configurar(conexao);
				txtUsuario.SelectionStart = pesquisa.Length;
				txtUsuario.SelectionLength = txtUsuario.Text.Length - pesquisa.Length;
			}
		}

		private void Configurar(Configuracao.Conexao conexao)
		{
			if (conexao != null)
			{
				cbTipoBanco.SelectedIndex = conexao.TipoBanco;
				txtServidor.Text = Mesclar(txtServidor.Text, conexao.Servidor);
				txtUsuario.Text = Mesclar(txtUsuario.Text, conexao.Usuario);
				cbBancoSchema.Text = Mesclar(cbBancoSchema.Text, conexao.Banco);
				txtSenha.Text = conexao.SalvarSenha ? conexao.Senha : String.Empty;
				ckSalvarSenha.Checked = conexao.SalvarSenha;
			}
			else
				Limpar(1);
		}

		private String Mesclar(String digitado, String encontrado)
		{
			return encontrado.ToUpper().StartsWith(digitado.ToUpper()) ? digitado + encontrado.Substring(digitado.Length) : encontrado;
		}

		private void Limpar(Int32 nivel)
		{
			if (nivel > 5)
				cbTipoBanco.Text = String.Empty;

			if (nivel > 4)
				txtServidor.Text = String.Empty;

			if (nivel > 3)
				txtUsuario.Text = String.Empty;

			if (nivel > 2)
				txtSenha.Text = String.Empty;

			if (nivel > 1)
			{
				cbBancoSchema.DataSource = null;
				cbBancoSchema.Text = String.Empty;
			}

			if (nivel > 0)
				ckSalvarSenha.Checked = false;
		}
	}
}