using System;
using System.Linq;
using System.Windows.Forms;
using MPSC.PlenoSQL.AppWin.Dados.Base;
using MPSC.PlenoSQL.AppWin.Interface;
using MPSC.PlenoSQL.AppWin.Infra;

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
			Limpar(5);
		}

		private void txtServidor_KeyUp(object sender, KeyEventArgs e)
		{
			var pesquisa = txtServidor.Text;
			if ((e.KeyCode == Keys.Back) && (pesquisa.Length > 0))
				pesquisa = pesquisa.Substring(0, pesquisa.Length - 1);

			AutoCompletarServidor(pesquisa);
		}

		private void txtUsuario_KeyUp(object sender, KeyEventArgs e)
		{
			var pesquisa = txtUsuario.Text;
			if ((e.KeyCode == Keys.Back) && (pesquisa.Length > 0))
				pesquisa = pesquisa.Substring(0, pesquisa.Length - 1);

			AutoCompletarUsuario(pesquisa);
		}

		private void AutoCompletarServidor(string pesquisa)
		{
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
					Limpar(4);
			}
		}

		private void AutoCompletarUsuario(string pesquisa)
		{
			if (!String.IsNullOrWhiteSpace(pesquisa))
			{
				var conexao = Parametro.Instancia.Conexoes
					.Where(c => c.TipoBanco == cbTipoBanco.SelectedIndex)
					.Where(c => c.Servidor.ToUpper().Equals(txtServidor.Text.ToUpper()))
					.Where(c => c.Usuario.ToUpper().StartsWith(pesquisa.ToUpper()))
					.FirstOrDefault();

				if (conexao != null)
				{
					Configurar(conexao);
					txtUsuario.SelectionStart = pesquisa.Length;
					txtUsuario.SelectionLength = txtUsuario.Text.Length - pesquisa.Length;
				}
				else
					Limpar(3);
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