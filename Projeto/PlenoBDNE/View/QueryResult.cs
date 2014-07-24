using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;
using System.Text.RegularExpressions;

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class QueryResult : TabPage, IQueryResult
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
			Colorir();
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

			if (!String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query"))
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
				AutoCompletar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtQuery.SelectAll();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.S))
				Salvar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.T))
				ListarTabelas();
			else if (e.KeyCode == Keys.F5)
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Y))
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Space))
			{
				if ((txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].Equals('.'))
					AutoCompletar();
				else
					ListarTabelas();
				e.SuppressKeyPress = true;
			}
		}

		private void UpdateDisplay()
		{
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
		}

		private Boolean _lock = false;
		private void txtQuery_TextChanged(object sender, EventArgs e)
		{
			if (!_lock)
			{
				_lock = true;
				Colorir();
				UpdateDisplay();
				_lock = false;
			}
		}

		private String[] palavrasReservadas = { "Select", "From", "Where", "And", "Or", "Not", "Inner", "Left", "Right", "Outter", "Join" };
		private String[] literals = { "Null", "Is", "On" };
		private const String bluFormat = @"\cf1{0}\cf0";
		private const String redFormat = @"\cf2{0}\cf0";
		private const String marFormat = @"\cf3{0}\cf0";
		private const String rtfHeader = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1046{\fonttbl{\f0\fnil\fcharset0 Courier New;}}
{\colortbl ;\red0\green0\blue255;\red255\green0\blue0;\red50\green160\blue200;}
\viewkind4\uc1\pard\f0\fs23";

		private void Colorir()
		{
			var selStart = txtQuery.SelectionStart;

			txtQuery.Rtf = rtfHeader + Trocar(txtQuery.Text);

			txtQuery.SelectionStart = selStart;
		}


		private String Trocar(String source)
		{
			source = source.Replace("\r\n", @"\line ");
			source = source.Replace("\r", @"\line ");
			source = source.Replace("\n", @"\line ");
			source = Regex.Replace(source, "((\"[^\"]*\")|('[^']*'))", String.Format(redFormat, "$0"));

			foreach (String key in palavrasReservadas)
				source = Trocar(source, key, bluFormat);

			foreach (String key in literals)
				source = Trocar(source, key, marFormat);

			return source;
		}

		private String Trocar(String source, String keys, String format)
		{
			//return Regex.Replace(source, keys + " ", String.Format(format, keys), RegexOptions.IgnoreCase);
			return Regex.Replace(source, @"(" + keys + @")[\s|\t]", String.Format(format, "$0"), RegexOptions.IgnoreCase);
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
					query = Util.ConverterParametrosEmConstantes(txtQuery.Text, query);
					dgResult.DataSource = null;
					BancoDeDados.Executar(query);
					Binding();
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
				var apelido = txtQuery.ObterPrefixo();
				var campos = BancoDeDados.ListarTabelas(apelido);
				ListaDeCampos.Exibir(campos, this, txtQuery.CurrentCharacterPosition(), OnSelecionarAutoCompletar);
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
				ListaDeCampos.Exibir(campos, this, txtQuery.CurrentCharacterPosition(), OnSelecionarAutoCompletar);
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
	}

	public interface IQueryResult
	{
		String NomeDoArquivo { get; }
		void Executar();
		Boolean Salvar();
		Boolean PodeFechar();
		void Fechar();
		Boolean Focus();
	}

	public class NullQueryResult : IQueryResult
	{
		public String NomeDoArquivo { get { return null; } }
		public void Executar() { }
		public Boolean Focus() { return false; }
		public Boolean Salvar() { return false; }
		public Boolean PodeFechar() { return true; }
		public void Fechar() { _instance = null; }

		private static IQueryResult _instance;
		public static IQueryResult Instance { get { return _instance ?? (_instance = new NullQueryResult()); } }
	}
}