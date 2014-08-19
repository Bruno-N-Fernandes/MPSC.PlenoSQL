using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;

namespace MP.PlenoBDNE.AppWin.View
{
	class AutoCompleteManager
	{
		private IBancoDeDados BancoDeDados;
		private FastColoredTextBox _txtQuery;
		private AutocompleteMenu popupMenu;
		private string[] keywords = { "Select", "From" };
		private string[] declarationSnippets = { 
               "Select\n*\nFrom ^\nWhere ();",
			   "Update ^ Set\nCampo1 = Valor1,\nCampo2 = Valor2\nWhere (PrimaryKey = Key);"
               };

		public AutoCompleteManager(FastColoredTextBox txtQuery, IBancoDeDados bancoDeDados)
		{
			_txtQuery = txtQuery;
			BancoDeDados = bancoDeDados;
			popupMenu = new AutocompleteMenu(txtQuery);
			//popupMenu.Items.ImageList = imageList1;
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
			popupMenu.AllowTabKey = true;

			List<AutocompleteItem> items = new List<AutocompleteItem>();

			foreach (var item in declarationSnippets)
				items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
			foreach (var item in keywords)
				items.Add(new AutocompleteItem(item));

			items.Add(new InsertEnterSnippet());

			popupMenu.Items.SetAutocompleteItems(items, false);
			popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu, txtQuery, BancoDeDados), true);
		}

		class DeclarationSnippet : SnippetAutocompleteItem
		{
			public DeclarationSnippet(string snippet) : base(snippet) { }

			public override CompareResult Compare(string fragmentText)
			{
				var pattern = Regex.Escape(fragmentText);
				if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
					return CompareResult.Visible;
				return CompareResult.Hidden;
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}