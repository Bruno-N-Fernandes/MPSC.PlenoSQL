using System.Text.RegularExpressions;
using FastColoredTextBoxNS;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class AutoCompleteManager
	{
		private static readonly AutocompleteItem[] declarationSnippets =
		{ 
			new DeclarationSnippet("Sel", "Select * From ^;"),
			new DeclarationSnippet("SelW", "Select * From ^ Where ();"),
			new DeclarationSnippet("Upd", "Update ^ Set\nCampo1 = Valor1,\nCampo2 = Valor2\nWhere (PrimaryKey = Valor);"),
			new DeclarationSnippet("Del", "Delete From ^ Where (PrimaryKey = Valor);"),
			new DeclarationSnippet("Ins", "Insert Into ^ (Campo1, Campo2) Values (Valor1, Valor2);"),
			new DeclarationSnippet("SelWF", "Select * From ^ Where () Fetch First 1 Rows Only;"),
			new DeclarationSnippet("SelF", "Select * From ^ Fetch First 1 Rows Only;"),
			new DeclarationSnippet("FFRO", "Fetch First ^ Rows Only"),
		};

		public static void Configurar(FastColoredTextBox textBox)
		{
			var popupMenu = new AutocompleteMenu(textBox);
			//popupMenu.Items.ImageList = imageList1;
			popupMenu.SearchPattern = @"[\w\.:=!<>]";
			popupMenu.AllowTabKey = true;
			popupMenu.Items.SetAutocompleteItems(declarationSnippets, false);
		}

		private static AutocompleteMenu popupMenu = null;
		public static void Configurar(FastColoredTextBox textBox, IEnumerable<String> members)
		{
			popupMenu = popupMenu ?? new AutocompleteMenu(textBox);
			popupMenu.Items.SetAutocompleteItems(members.OrderBy(m => m).Select(m => new MethodAutocompleteItem(m)), false);
		}


		public class MethodAutocompleteItem : AutocompleteItem
		{
			string firstPart;
			string lowercaseText;

			public MethodAutocompleteItem(string text)
				: base(text)
			{
				lowercaseText = Text.ToLower();
			}

			public override CompareResult Compare(string fragmentText)
			{
				int i = fragmentText.LastIndexOf('.');
				if (i < 0)
					return CompareResult.Hidden;
				string lastPart = fragmentText.Substring(i + 1);
				firstPart = fragmentText.Substring(0, i);

				if (lastPart == "") return CompareResult.Visible;
				if (Text.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase))
					return CompareResult.VisibleAndSelected;
				if (lowercaseText.Contains(lastPart.ToLower()))
					return CompareResult.Visible;

				return CompareResult.Hidden;
			}

			public override string GetTextForReplace()
			{
				return firstPart + "." + Text;
			}
		}

		public class DeclarationSnippet : SnippetAutocompleteItem
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