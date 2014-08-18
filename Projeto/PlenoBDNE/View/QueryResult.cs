using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.Infra.Interface;
using MP.PlenoBDNE.AppWin.View.Interface;
using MPSC.LanguageEditor.Syntax;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;

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
			//Colorir();
			AutocompleteSample2();
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
			if ((e.KeyValue == 190) || (e.KeyValue == 194))
				;//AutoCompletar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtQuery.SelectAll();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.S))
				Salvar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.T))
				ListarTabelas();
			else if (e.KeyCode == Keys.F5)
				Executar();
			else if ((e.KeyCode == Keys.Tab) && (txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].ToString().ToUpper().Equals("S") &&
				((txtQuery.SelectionStart == txtQuery.Text.Length) || txtQuery.Text[txtQuery.SelectionStart].Equals('\n')))
			{
				e.SuppressKeyPress = true;
				OnSelecionarAutoCompletar("elect * From ;");
				txtQuery.SelectionStart -= 1;
			}
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Y))
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Space))
			{
				//e.SuppressKeyPress = true;
				//if ((txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].Equals('.'))
				//    AutoCompletar();
				//else
				//    ListarTabelas();
			}
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.R))
			{
				e.SuppressKeyPress = true;
				(new ExpressaoRegularBuilder()).ShowDialog();
			}
		}

		private void UpdateDisplay()
		{
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
		}

		private void txtQuery_TextChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}
		/*
				private void Colorir()
				{
					txtQuery.FilterAutoComplete = true;
					txtQuery.Seperators.Add(' ');
					txtQuery.Seperators.Add('\r');
					txtQuery.Seperators.Add('\n');
					txtQuery.Seperators.Add('\t');
					txtQuery.Seperators.Add('.');
					txtQuery.Seperators.Add(',');
					txtQuery.Seperators.Add(';');
					//txtQuery.Seperators.Add('*');
					//txtQuery.Seperators.Add('/');
					//txtQuery.Seperators.Add('-');
					txtQuery.Seperators.Add('+');
					txtQuery.Seperators.Add('(');
					txtQuery.Seperators.Add(')');

					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Select", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Distinct", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("From", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Inner", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Left", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Right", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Outter", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Join", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Where", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Group", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Order", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Between", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Case", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("When", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Having", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Update", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Set", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Insert Into", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Delete", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Truncate", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Drop", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Table", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Column", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("End", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Then", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Else", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Null", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Is", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("As", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("In", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("On", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("And", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Or", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Not", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Like", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Union", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("By", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Asc", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Desc", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

					var azul = Color.FromArgb(0, 0, 160);
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("=", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor(">", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<=", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor(">=", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<>", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("!=", azul, null, DescriptorType.Word, DescriptorRecognition.WholeWord, false));

					//txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("'", "'", Color.Red, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("/*", "* /", Color.Green, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));
					txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("--", Color.Green, null, DescriptorType.ToEOL, DescriptorRecognition.StartsWith, false));

					txtQuery.Colorir();
				}
		*/
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

		private void ListarTabelas()
		{
			try
			{
				//var apelido = txtQuery.ObterPrefixo();
				//var campos = BancoDeDados.ListarTabelas(apelido);
				//ListaDeCampos.Exibir(campos, this, txtQuery.CurrentCharacterPosition(), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
		}

		private void AutoCompletar()
		{
			try
			{
				var apelido = Util.ObterApelidoAntesDoPonto(txtQuery.Text, txtQuery.SelectionStart);
				var tabela = Util.ObterNomeTabelaPorApelido(txtQuery.Text, txtQuery.SelectionStart, apelido);
				var campos = BancoDeDados.ListarColunasDasTabelas(tabela);
				ListaDeCampos.Exibir(campos, this, new Point(0, 0), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
		}

		private void OnSelecionarAutoCompletar(String item)
		{
			if (!String.IsNullOrWhiteSpace(item))
			{
				var old = Clipboard.GetText();
				Clipboard.SetText(item);
				txtQuery.Paste();
				if (!String.IsNullOrWhiteSpace(old))
					Clipboard.SetText(old);
			}
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


		AutocompleteMenu popupMenu;
		string[] keywords = { "Select", "From" };
		string[] declarationSnippets = { 
               "Select\n\t*\nFrom ^\nWhere ();", "Update ^ Set\nCampo1 = Valor1,\nCampo2 = Valor2\nWhere (PrimaryKey = Key);"
               };

		public void AutocompleteSample2()
		{

			//create autocomplete popup menu
			popupMenu = new AutocompleteMenu(txtQuery);
			//popupMenu.Items.ImageList = imageList1;
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
			popupMenu.AllowTabKey = true;
			//
			BuildAutocompleteMenu();
		}

		private void BuildAutocompleteMenu()
		{
			List<AutocompleteItem> items = new List<AutocompleteItem>();

			foreach (var item in declarationSnippets)
				items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
			foreach (var item in keywords)
				items.Add(new AutocompleteItem(item));

			items.Add(new InsertSpaceSnippet());
			items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
			items.Add(new InsertEnterSnippet());

			//set as autocomplete source
			popupMenu.Items.SetAutocompleteItems(items, false);
			popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu, txtQuery, BancoDeDados), true);

		}

		/// <summary>
		/// This item appears when any part of snippet text is typed
		/// </summary>
		class DeclarationSnippet : SnippetAutocompleteItem
		{
			public DeclarationSnippet(string snippet)
				: base(snippet)
			{
			}

			public override CompareResult Compare(string fragmentText)
			{
				var pattern = Regex.Escape(fragmentText);
				if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
					return CompareResult.Visible;
				return CompareResult.Hidden;
			}
		}

		/// <summary>
		/// Divides numbers and words: "123AND456" -> "123 AND 456"
		/// Or "i=2" -> "i = 2"
		/// </summary>
		class InsertSpaceSnippet : AutocompleteItem
		{
			string pattern;

			public InsertSpaceSnippet(string pattern)
				: base("")
			{
				this.pattern = pattern;
			}

			public InsertSpaceSnippet()
				: this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
			{
			}

			public override CompareResult Compare(string fragmentText)
			{
				if (Regex.IsMatch(fragmentText, pattern))
				{
					Text = InsertSpaces(fragmentText);
					if (Text != fragmentText)
						return CompareResult.Visible;
				}
				return CompareResult.Hidden;
			}

			public string InsertSpaces(string fragment)
			{
				var m = Regex.Match(fragment, pattern);
				if (m == null)
					return fragment;
				if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
					return fragment;
				return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
			}

			public override string ToolTipTitle
			{
				get
				{
					return Text;
				}
			}
		}

		/// <summary>
		/// Inerts line break after '}'
		/// </summary>
		class InsertEnterSnippet : AutocompleteItem
		{
			Place enterPlace = Place.Empty;

			public InsertEnterSnippet()
				: base("[Line break]")
			{
			}

			public override CompareResult Compare(string fragmentText)
			{
				var r = Parent.Fragment.Clone();
				while (r.Start.iChar > 0)
				{
					if (r.CharBeforeStart == '}')
					{
						enterPlace = r.Start;
						return CompareResult.Visible;
					}

					r.GoLeftThroughFolded();
				}

				return CompareResult.Hidden;
			}

			public override string GetTextForReplace()
			{
				//extend range
				Range r = Parent.Fragment;
				Place end = r.End;
				r.Start = enterPlace;
				r.End = r.End;
				//insert line break
				return Environment.NewLine + r.Text;
			}

			public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
			{
				base.OnSelected(popupMenu, e);
				if (Parent.Fragment.tb.AutoIndent)
					Parent.Fragment.tb.DoAutoIndent();
			}

			public override string ToolTipTitle
			{
				get
				{
					return "Insert line break after '}'";
				}
			}
		}
	}


	/// <summary>
	/// Builds list of methods and properties for current class name was typed in the textbox
	/// </summary>
	internal class DynamicCollection : IEnumerable<AutocompleteItem>
	{
		private AutocompleteMenu menu;
		private FastColoredTextBox tb;
		private IBancoDeDados bancoDeDados;

		public DynamicCollection(AutocompleteMenu menu, FastColoredTextBox tb, IBancoDeDados bancoDeDados)
		{
			this.menu = menu;
			this.tb = tb;
			this.bancoDeDados = bancoDeDados;
		}

		public IEnumerator<AutocompleteItem> GetEnumerator()
		{
			//get current fragment of the text
			var text = menu.Fragment.Text;
			if (text.Contains("."))
			{
				var tabela = Util.ObterNomeTabelaPorApelido(tb.Text, tb.SelectionStart, text);
				var campos = bancoDeDados.ListarColunasDasTabelas(tabela);
				foreach (var item in campos)
					yield return new MethodAutocompleteItem(item);
			}
			else
			{
				var parts = text.Split('.');
				if (parts.Length < 1)
					yield break;
				var className = parts[parts.Length - 1];
				var tabelas = bancoDeDados.ListarTabelas(className);
				foreach (var item in tabelas)
					yield return new AutocompleteItem(item);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}