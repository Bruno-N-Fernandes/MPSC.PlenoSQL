using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Infra.Interface;
using MP.PlenoBDNE.AppWin.View.Interface;

//TODO: Bruno Fernandes (08/08/2014 17:35) - Colocar informações no StatusBar (conexão, usuário, banco, registros alterados) 
//TODO: Bruno Fernandes (08/08/2014 17:35) - Listar os objetos do banco de dados na coluna da esquerda (TreeView)
//TODO: Bruno Fernandes (08/08/2014 17:35) - Exportar o resultado da query para TXT, XLS, XML, PDF, etc.
//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir escolher a fonte e o tamanho da mesma.
//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir Configurar o Colorir da Query
//TODO: Bruno Fernandes (08/08/2014 17:35) - Mensagem de aguarde, processsando
//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir o cancelamento da query.
//TODO: Bruno Fernandes (08/08/2014 17:35) - Criar um grupo de Favoritos (Cada grupo poderá agrupar vários arquivos)
//TODO: Bruno Fernandes (08/08/2014 17:35) - Close All But This.
//TODO: Bruno Fernandes (11/08/2014 18:30) - Fazer Auto Reload dos arquivos alterados
//TODO: Bruno Fernandes (11/08/2014 18:30) - Permitir associar extensão ao aplicativo
//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar F4 para mostrar propriedades da tabela e do campo
//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar Code Snippet (Pressionando TAB)
//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar a separação de blocos de código (ponto e virgula)

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class QueryResult : TabPage, IQueryResult, IMessageResult
	{
		private static Int32 _quantidade = 0;
		private IBancoDeDados _bancoDeDados = null;
		public IBancoDeDados BancoDeDados { get { return _bancoDeDados ?? (_bancoDeDados = BancoDeDados<IDbConnection>.Conectar()); } }

		private String originalQuery = String.Empty;
		public String NomeDoArquivo { get; private set; }
		private String QueryAtiva { get { return ((txtQuery.SelectedText.Length > 1) ? txtQuery.SelectedText : txtQuery.Text); } }

		public QueryResult(String nomeDoArquivo)
		{
			InitializeComponent();
			Abrir(nomeDoArquivo);
			//new AutoCompleteManager(txtQuery, BancoDeDados);
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
				NomeDoArquivo = Util.GetFileToSave("Arquivos de Banco de Dados|*.sql") ?? NomeDoArquivo;

			if (!String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && (originalQuery != txtQuery.Text))
			{
				File.WriteAllText(NomeDoArquivo, txtQuery.Text);
				originalQuery = txtQuery.Text;
			}
			UpdateDisplay();

			return !String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && File.Exists(NomeDoArquivo);
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
			else if ((e.KeyValue == 190) || (e.KeyValue == 194))
				e.SuppressKeyPress = AutoCompletarCampos(false);
		}

		private void UpdateDisplay()
		{
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
		}

		private void txtQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateDisplay();
		}

		private INavegador FindNavegador()
		{
			return (FindForm() as INavegador) ?? NavegadorNulo.Instancia;
		}

		private void btBinding_Click(object sender, EventArgs e)
		{
			Binding();
		}

		public void Executar()
		{
			var query = QueryAtiva;
			if (!String.IsNullOrWhiteSpace(query))
			{
				try
				{
					var form = FindForm() as INavegador;
					if ((form != null) && form.SalvarAoExecutar)
						Salvar();

					query = Util.ConverterParametrosEmConstantes(txtQuery.Text, query, txtQuery.SelectionStart);
					dgResult.DataSource = null;
					Processar(query, "Query");
					BancoDeDados.Executar(query, this);
					Binding();
					tcResultados.SelectedIndex = 1;
					dgResult.Focus();
					dgResult.AutoResizeColumns();
				}
				catch (Exception vException)
				{
					MessageBox.Show("Houve um problema ao executar a query. Detalhes:\n" + vException.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		public void Binding()
		{
			var result = BancoDeDados.Transformar();
			if (dgResult.DataSource == null)
			{
				var lista = result.ToList();
				dgResult.Enabled = lista.Count > 0;
				dgResult.DataSource = (lista.Count == 0) ? BancoDeDados.Cabecalho().ToList() : lista;
				if (lista.Count == 0)
					MessageBox.Show("A query não retornou resultados!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				var linha = dgResult.FirstDisplayedScrollingRowIndex;
				dgResult.DataSource = (dgResult.DataSource as IEnumerable<Object>).Union(result).ToList();
				if (linha >= 0)
					dgResult.FirstDisplayedScrollingRowIndex = linha;
			}
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
				var apelido = Util.ObterApelidoAntesDoPonto(txtQuery.Text, txtQuery.SelectionStart);
				var tabela = Util.ObterNomeTabelaPorApelido(txtQuery.Text, txtQuery.SelectionStart, apelido);
				var campos = BancoDeDados.ListarColunasDasTabelas(tabela);
				if (!controle) txtQuery.Paste(".");
				ListaDeCampos.Exibir(campos, this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
			return controle;
		}

		private Boolean AutoCompletarTabelas(Boolean controle)
		{
			try
			{
				var apelido = txtQuery.ObterPrefixo();
				var campos = BancoDeDados.ListarTabelas(apelido);
				ListaDeCampos.Exibir(campos, this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
			return controle;
		}

		private void OnSelecionarAutoCompletar(String item)
		{
			if (!String.IsNullOrWhiteSpace(item))
				txtQuery.Paste(item);
			txtQuery.Focus();
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

		public void Fechar()
		{
			if (_bancoDeDados != null)
			{
				_bancoDeDados.Dispose();
				_bancoDeDados = null;
			}

			originalQuery = null;

			txtQuery.Clear();
			txtQuery.Dispose();

			dgResult.DataSource = null;
			dgResult.Dispose();

			base.Dispose();
			GC.Collect();
		}

		public new Boolean Focus()
		{
			return txtQuery.Focus();
		}

		private void dgResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				txtQuery.Focus();
		}

		public void Processar(String message, String tipo)
		{
			txtMensagens.AppendText("// " + tipo.ToUpper() + ": \r\n" + message + "\r\n\r\n");
		}
	}
}