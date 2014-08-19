using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FastColoredTextBoxNS;
using MP.PlenoBDNE.AppWin.Dados;

namespace MP.PlenoBDNE.AppWin.View
{
	class AutoCompleteManager
	{
		private AutocompleteMenu popupMenu;
		private string[] keywords = { "Select", "From" };
		private string[] declarationSnippets = { 
               "Select * From ^ Where ();",
			   "Update ^ Set\nCampo1 = Valor1,\nCampo2 = Valor2\nWhere (PrimaryKey = Valor);",
			   "Delete From ^ Where (PrimaryKey = Valor)",
			   "Insert Into ^ (Campo1, Campo2) Values (Valor1, Valor2);"
               };

		public AutoCompleteManager(FastColoredTextBox txtQuery)
		{
			popupMenu = new AutocompleteMenu(txtQuery);
			//popupMenu.Items.ImageList = imageList1;
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
			popupMenu.AllowTabKey = true;

			List<AutocompleteItem> items = new List<AutocompleteItem>();

			foreach (var item in declarationSnippets)
				items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
			foreach (var item in keywords)
				items.Add(new AutocompleteItem(item));

			popupMenu.Items.SetAutocompleteItems(items, false);
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
	}
}