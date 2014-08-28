using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Interface;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class QueryResult : TabPage, IQueryResult, IMessageResult
	{
		private static Int32 _quantidade = 0;
		public String NomeDoArquivo { get; private set; }
		private IBancoDeDados _bancoDeDados = null;
		private String originalQuery = String.Empty;
		private IBancoDeDados BancoDeDados
		{
			get
			{
				return _bancoDeDados ?? (BancoDeDados = Autenticacao.Dialog(this));
			}
			set
			{
				_bancoDeDados = value;
				if (_bancoDeDados != null)
					ShowLog(_bancoDeDados.Conexao, "Conexão");
			}
		}
		private String QueryAtiva
		{
			get
			{
				return ((txtQuery.SelectedText.Length > 1) ? txtQuery.SelectedText : txtQuery.Text);
			}
		}

		public QueryResult(String nomeDoArquivo)
		{
			InitializeComponent();
			Abrir(nomeDoArquivo);
			AutoCompleteManager.Configurar(txtQuery);
		}

		public void Abrir(String nomeDoArquivo)
		{
			if (!String.IsNullOrWhiteSpace(nomeDoArquivo) && File.Exists(nomeDoArquivo))
			{
				txtQuery.Text = File.ReadAllText(NomeDoArquivo = nomeDoArquivo);
				originalQuery = txtQuery.Text;
			}
			else
				NomeDoArquivo = String.Format("Query{0}.sql", ++_quantidade);
			UpdateDisplay();
		}

		public Boolean Salvar()
		{
			if (String.IsNullOrWhiteSpace(NomeDoArquivo) || NomeDoArquivo.StartsWith("Query") || !File.Exists(NomeDoArquivo))
				NomeDoArquivo = FileUtil.GetFileToSave("Arquivos de Banco de Dados|*.sql") ?? NomeDoArquivo;

			if (!String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && (originalQuery != txtQuery.Text))
			{
				File.WriteAllText(NomeDoArquivo, txtQuery.Text);
				originalQuery = txtQuery.Text;
			}
			UpdateDisplay();

			return !String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && File.Exists(NomeDoArquivo);
		}

		public void AlterarConexao()
		{
			FecharConexao();
			var b = BancoDeDados;
		}

		public void Executar()
		{
			var query = QueryAtiva;
			if (!String.IsNullOrWhiteSpace(query))
			{
				var bancoDeDados = BancoDeDados;
				if (bancoDeDados != null)
				{
					try
					{
						dgResult.DataSource = null;
						query = txtQuery.ConverterParametrosEmConstantes(query);
						var result = bancoDeDados.Executar(query);
						ShowLog("#" + Convert.ToString(result) + " linhas afetadas pela Query: " + query + ";", "Resultado Query");
						Binding();
						if (FindNavegador().SalvarAoExecutar)
							Salvar();
					}
					catch (NullReferenceException vException) { ShowLog(vException.Message, "Erro"); }
					catch (Exception vException)
					{
						var msg = "Houve um problema ao executar a Query. Detalhes:\n" + vException.Message;
						ShowLog(msg + "\r\n" + query, "Erro Query");
						MessageBox.Show(msg, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
		}

		public void Binding()
		{
			var result = BancoDeDados.DataBinding();
			if (dgResult.DataSource == null)
			{
				var lista = result.Skip(1).ToList();
				var enabled = lista.Count > 0;
				dgResult.Enabled = enabled;
				dgResult.DataSource = enabled ? lista : result.ToList();
				tcResultados.SelectedIndex = enabled ? 1 : 0;
			}
			else
			{
				var linha = dgResult.FirstDisplayedScrollingRowIndex;
				dgResult.DataSource = (dgResult.DataSource as IEnumerable<Object>).Union(result.Skip(1)).ToList();
				if (linha >= 0)
					dgResult.FirstDisplayedScrollingRowIndex = linha;
			}

			dgResult.AutoResizeColumns();
			if (dgResult.Enabled)
				dgResult.Focus();
			Application.DoEvents();
		}

		public void Fechar()
		{
			FecharConexao();

			originalQuery = null;

			txtQuery.Clear();
			txtQuery.Dispose();

			dgResult.DataSource = null;
			dgResult.Dispose();

			base.Dispose();
			GC.Collect();
		}


		public Boolean PodeFechar()
		{
			Boolean fechar = true;

			if (txtQuery.Text != originalQuery)
			{
				var dialogResult = MessageBox.Show(String.Format("O arquivo '{0}' foi alterado. Deseja Salvá-lo?", NomeDoArquivo), "Confirmação", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
					fechar = Salvar();
				else
					fechar = (dialogResult == DialogResult.No);
			}

			return fechar;
		}

		private void FecharConexao()
		{
			if (_bancoDeDados != null)
			{
				_bancoDeDados.Dispose();
				_bancoDeDados = null;
			}
			UpdateDisplay();
		}

		private void txtQuery_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F5)
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.R))
				e.SuppressKeyPress = (new ExpressaoRegularBuilder()).ShowDialog() == DialogResult.Abort;
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtQuery.SelectAll();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.S))
				Salvar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.T))
				e.SuppressKeyPress = AutoCompletarTabelas(true);
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Space))
				e.SuppressKeyPress = AutoCompletar(true);
			else if ((e.Modifiers == Keys.None) && ((e.KeyValue == 190) || (e.KeyValue == 194)))
				e.SuppressKeyPress = AutoCompletarCampos(false);
		}

		private void txtQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateDisplay();
		}

		private void btBinding_Click(object sender, EventArgs e)
		{
			Binding();
		}

		private void dgResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Focus();
		}

		private Boolean AutoCompletar(Boolean controle)
		{
			if ((txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].Equals('.'))
				return AutoCompletarCampos(controle);
			else
				return AutoCompletarTabelas(controle);
		}

		private Boolean AutoCompletarCampos(Boolean controle)
		{
			try
			{
				var apelido = txtQuery.ObterApelidoAntesDoPonto();
				var tabela = txtQuery.ObterNomeTabelaPorApelido(apelido);
				var campos = BancoDeDados.ListarColunas(tabela, false);
				Application.DoEvents();
				ListaDeCampos.Exibir(campos, this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
			return controle;
		}

		private Boolean AutoCompletarTabelas(Boolean controle)
		{
			try
			{
				var apelido = txtQuery.ObterPrefixo();
				var tabelas = BancoDeDados.ListarTabelas(apelido, false);
				tabelas = tabelas.Union(BancoDeDados.ListarViews(apelido, false));
				ListaDeCampos.Exibir(tabelas, this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
			return controle;
		}

		private void OnSelecionarAutoCompletar(String item)
		{
			if (!String.IsNullOrWhiteSpace(item))
				txtQuery.Paste(item);
			txtQuery.Focus();
		}

		public new Boolean Focus()
		{
			UpdateDisplay();
			return txtQuery.Focus();
		}

		public void ShowLog(String message, String tipo)
		{
			txtMensagens.AppendText("// " + tipo.ToUpper() + ": \r\n" + message + "\r\n\r\n");
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			var iNavegador = FindNavegador();
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
			txtQuery.TextMode = iNavegador.ConvertToUpper ? TextModeType.UPPERCASE : TextModeType.NormalCase;
			iNavegador.Status(_bancoDeDados != null ? "Conectado à " + _bancoDeDados.Conexao : "Desconectado");
			Application.DoEvents();
		}

		private INavegador FindNavegador()
		{
			return (FindForm() as INavegador) ?? NavegadorNulo.Instancia;
		}

		private void dgResult_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			e.Cancel = true;
		}
	}
}