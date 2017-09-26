using FastColoredTextBoxNS;
using MPSC.PlenoSQL.Kernel.Infra;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Tester;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class QueryResult : TabPage, IQueryResult, IMessageResult
	{
		private static readonly string[] _separadores = new[] { ";\t\r\n", "; \r\n", ";\r\n", ";\t\r", "; \r", ";\r", ";\t\n", "; \n", ";\n" };
		private readonly List<Action> _acoesPendentes = new List<Action>();
		private static Int32 _quantidade = 0;
		private Boolean _showLog = true;

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
				return ((txtQuery.SelectedText.Length > 1) ? txtQuery.SelectedText.AllTrim() : txtQuery.Text.AllTrim()) + "\r\n";
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
				txtQuery.OpenFile(NomeDoArquivo = nomeDoArquivo);
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

		private static Int32[] Selecionar(String texto, String textoSelecionado, Int32 inicioSelecao)
		{
			if (textoSelecionado.Length < 1 && !String.IsNullOrWhiteSpace(texto))
			{
				var query = texto + ";";
				var cursor = Math.Max(inicioSelecao, 1);
				if ((cursor > 2) && (cursor < query.Length) && (query[cursor] == '\r') && (query[cursor - 1] == ';'))
					cursor--;

				var indiceF = FormatUtil.ObterPosicaoFinal(query, q => q.IndexOf(";", cursor));
				query = query.Substring(0, indiceF);
				var indiceI = FormatUtil.ObterPosicaoInicial(query.Substring(0, cursor - 1), q => q.LastIndexOf(";"));
				query = query.Substring(indiceI);

				var anterior = String.Empty;
				var tamanho = query.Length;
				while (!String.IsNullOrWhiteSpace(query) && !Regex.IsMatch(query, "^[a-zA-Z]") && (anterior != query))
				{
					anterior = query;
					if (query.StartsWith("\r") || query.StartsWith("\n") || query.StartsWith("\t") || query.StartsWith(" "))
						query = query.Substring(1).TrimStart();
					else if (query.StartsWith("--"))
						query = query.Substring(query.IndexOfAny("\r\n".ToCharArray()) + 1).TrimStart();
					else if (query.StartsWith("/*") && query.Contains("*/"))
						query = query.Substring(query.IndexOf("*/") + 2).TrimStart();
					else if (query.StartsWith("*/"))
						query = query.Substring(2).TrimStart();
				}

				indiceI += (tamanho - query.Length);
				tamanho = indiceF - indiceI;

				return new[] { indiceI, tamanho };
			}

			return new[] { inicioSelecao, 0 };
		}
		private Boolean QueryEhUmDDL(String query)
		{
			query = Regex.Replace(query.ToUpper(), "[^A-Z;]", String.Empty);
			return query.EndsWith("END;") && (query.StartsWith("CREATE") || query.StartsWith("ALTER") || query.StartsWith("REPLACE"));
		}

		public void Selecionar()
		{
			var indices = Selecionar(txtQuery.Text, txtQuery.SelectedText, txtQuery.SelectionStart);

			txtQuery.SelectionStart = indices[0];
			txtQuery.SelectionLength = indices[1];
		}

		public void Executar()
		{
			_showLog = true;
			var ok = false;
			var query = QueryAtiva;
			var mostrarEstatisticas = FindNavegador().MostrarEstatisticas;
			if (txtQuery.SelectedText.Length > 1)
			{
				if (QueryEhUmDDL(query))
					ok = executarImpl(query, mostrarEstatisticas, Decimal.One);
				else
					ok = executarVarios(query, mostrarEstatisticas);
			}
			else
			{
				Selecionar();
				ok = executarImpl(query, mostrarEstatisticas, Decimal.One);
			}

			if (ok && FindNavegador().SalvarAoExecutar)
				Salvar();
		}

		private Boolean executarVarios(String queryAtiva, Boolean mostrarEstatisticas)
		{
			_showLog = false;
			var ok = true;
			var queries = queryAtiva.Split(_separadores, StringSplitOptions.RemoveEmptyEntries).Select(Extensions.AllTrim).ToArray();

			Decimal total = queries.Length;
			var conta = Decimal.Zero;
			foreach (var query in queries)
			{
				if (!executarImpl(query, mostrarEstatisticas, ++conta / total))
				{
					txtQuery.SelectionStart = txtQuery.Text.IndexOf(query);
					txtQuery.SelectionLength = query.Length;
					ok = false;
					break;
				}
			}
			_showLog = true;
			return ok;
		}

		private Boolean executarImpl(String query, Boolean mostrarEstatisticas, Decimal percentual = Decimal.One)
		{
			var retorno = true;
			if (!String.IsNullOrWhiteSpace(query))
			{
				var bancoDeDados = BancoDeDados;
				if (bancoDeDados != null)
				{
					try
					{
						var inicio = DateTime.Now;

						dgResult.BancoDeDados = bancoDeDados;
						var result = bancoDeDados.Executar(query, mostrarEstatisticas);
						tcResultados.SelectedIndex = dgResult.Binding();
						if (_showLog || ((percentual * 100M) - Decimal.Truncate(percentual * 100M) < 0.005M))
							ShowLog(String.Format("{0}% #{1:###,###,###,###,##0} linhas afetadas em {2} milissegundos pela Query:\r\n{3};", (percentual * 100M).ToString("##0.00"), result, (DateTime.Now - inicio).TotalMilliseconds, query), "Resultado Query");
					}
					catch (NullReferenceException exception)
					{
						retorno = false;
						ShowLog(exception.Message, "Erro");
					}
					catch (Exception exception)
					{
						retorno = false;
						var msg = "Houve um problema ao executar a Query. Detalhes:\n" + exception.Message;
						ShowLog(msg + "\r\n" + query, "Erro Query");
						MessageBox.Show(msg, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
			return retorno;
		}

		public void Fechar()
		{
			FecharConexao();

			originalQuery = null;

			txtQuery.Clear();
			txtQuery.Dispose();

			dgResult.Free();

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
			if (e.KeyCode == Keys.F6)
				Selecionar();
			else if (e.KeyCode == Keys.F8)
				Embelezar();
			else if ((e.KeyCode == Keys.F5) || ((e.Modifiers == Keys.Control) && ((e.KeyCode == Keys.E) || (e.KeyCode == Keys.Y))))
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.R))
				e.SuppressKeyPress = (new ExpressaoRegularBuilder()).ShowDialog() == DialogResult.Abort;
			else if ((e.Modifiers == Keys.Control) && (e.KeyValue == 186)) //Ç
				e.SuppressKeyPress = (new MainForm()).ShowDialog() == DialogResult.Abort;
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtQuery.SelectAll();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.S))
				Salvar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.T))
				e.SuppressKeyPress = AutoCompletarTabelas(true);
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Space))
				e.SuppressKeyPress = DetectarAutoCompletar(true);
			else if ((e.Modifiers == Keys.None) && ((e.KeyValue == 190) || (e.KeyValue == 194)))
				_acoesPendentes.Add(() => AutoCompletarCampos(false));
		}

		private void Embelezar()
		{
			if (txtQuery.SelectedText.Length == 0)
				Selecionar();

			if (txtQuery.SelectedText.Length > 1)
			{
				var novaQuery = FormatUtil.Embelezar(txtQuery.SelectedText, true);
				txtQuery.Paste(novaQuery);

				txtQuery.SelectionStart -= 2;
				txtQuery.SelectionLength = 0;
				Selecionar();
			}
		}

		private void txtQuery_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateDisplay();
			_acoesPendentes.RemoveAll(a => { a.Invoke(); return true; });
		}

		private void dgResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Focus();
		}

		private Boolean DetectarAutoCompletar(Boolean controle)
		{
			var token = Trecho.Get(txtQuery.Text, txtQuery.SelectionStart).Token;

			if (token.Completo.Contains(".") || ((txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].Equals('.')))
				return AutoCompletarCampos(controle);
			else
				return AutoCompletarTabelas(controle);
		}

		private Boolean AutoCompletarCampos(Boolean controle)
		{
			try
			{
				if (BancoDeDados != null)
				{
					var token = Trecho.Get(txtQuery.Text, txtQuery.SelectionStart).Token;
					var campos = BancoDeDados.ListarColunas(token.Tabela, token.Parcial, false);
					Application.DoEvents();
					AutoCompletar.Exibir(token.Parcial, campos, this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
					txtQuery.SelectionStart -= token.Parcial.Length;
					txtQuery.SelectionLength = token.Parcial.Length;
				}
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
			return controle;
		}

		private Boolean AutoCompletarTabelas(Boolean controle)
		{
			try
			{
				if (BancoDeDados != null)
				{
					var token = Trecho.Get(txtQuery.Text, txtQuery.SelectionStart).Token;
					var tabelas = BancoDeDados.ListarTabelas(token.Tabela, false);
					var views = BancoDeDados.ListarViews(token.Tabela, false);
					AutoCompletar.Exibir(token.Parcial, tabelas.Union(views), this, txtQuery.GetPointAtSelectionStart(), OnSelecionarAutoCompletar);
					txtQuery.SelectionStart -= token.Parcial.Length;
					txtQuery.SelectionLength = token.Parcial.Length;
				}
			}
			catch (Exception vException) { ShowLog(vException.Message, "Erro"); }
			return controle;
		}

		private void OnSelecionarAutoCompletar(String item, Boolean parcial)
		{
			if (parcial)
				txtQuery.SelectionStart += txtQuery.SelectionLength;

			if (!String.IsNullOrWhiteSpace(item))
				txtQuery.Paste(FindNavegador().ConvertToUpper ? item.ToUpper() : item);

			txtQuery.Focus();
		}

		public new Boolean Focus()
		{
			UpdateDisplay();
			return txtQuery.Focus();
		}

		public void ProcessarEventos()
		{
			Application.DoEvents();
		}

		public void ShowLog(String message, String tipo)
		{
			txtMensagens.AppendText("// " + tipo.ToUpper() + ": \r\n" + message + "\r\n\r\n");
			txtMensagens.SelectionStart = txtMensagens.TextLength;
			txtMensagens.ScrollToCaret();
			UpdateDisplay();
			if (tipo.ToUpper().Equals("ERRO"))
				MessageBox.Show(String.Format("Houve um problema ao executar a query. Detalhes:\r\n{0}", message), "PlenoSQL", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
		}

		private void UpdateDisplay()
		{
			var iNavegador = FindNavegador();
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
			txtQuery.TextMode = iNavegador.ConvertToUpper ? TextModeType.UPPERCASE : TextModeType.NormalCase;
			txtQuery.ShowHighlight = iNavegador.Colorir ? HighlightType.Both : HighlightType.None;
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

		public void txtMensagens_LimparLog(Object sender, EventArgs args)
		{
			_showLog = !_showLog;
			txtMensagens.Clear();
			txtMensagens.ClearUndo();
		}
	}
}