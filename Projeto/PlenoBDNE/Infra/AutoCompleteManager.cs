using System.Text.RegularExpressions;
using FastColoredTextBoxNS;

namespace MP.PlenoBDNE.AppWin.View
{
	public class AutoCompleteManager
	{
		private AutocompleteMenu popupMenu;
		private static readonly AutocompleteItem[] declarationSnippets = { 
               new DeclarationSnippet("Sel", "Select * From ^;"),
               new DeclarationSnippet("SelW", "Select * From ^ Where ();"),
			   new DeclarationSnippet("Upd", "Update ^ Set\nCampo1 = Valor1,\nCampo2 = Valor2\nWhere (PrimaryKey = Valor);"),
			   new DeclarationSnippet("Del", "Delete From ^ Where (PrimaryKey = Valor);"),
			   new DeclarationSnippet("Ins", "Insert Into ^ (Campo1, Campo2) Values (Valor1, Valor2);"),
			   new DeclarationSnippet("SelWF", "Select * From ^ Where () Fetch First 1 Rows Only;"),
			   new DeclarationSnippet("SelF", "Select * From ^ Fetch First 1 Rows Only;"),
			   new DeclarationSnippet("FFRO", "Fetch First ^ Rows Only"),
               };

		public AutoCompleteManager(FastColoredTextBox txtQuery)
		{
			popupMenu = new AutocompleteMenu(txtQuery);
			//popupMenu.Items.ImageList = imageList1;
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
			popupMenu.AllowTabKey = true;
			popupMenu.Items.SetAutocompleteItems(declarationSnippets, false);
		}

		class DeclarationSnippet : SnippetAutocompleteItem
		{
			public DeclarationSnippet(string snippet, string code) : base(snippet, code) { }

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