using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MP.PlenoBDNE.AppWin.View
{
	public delegate void SelecionarEventHandler(String item);

	public partial class ListaDeCampos : ListBox
	{
		private event SelecionarEventHandler OnSelecionar;
		private DateTime _lastKey = DateTime.Now;
		private String _search = String.Empty;

		private ListaDeCampos(IList<String> listaString, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			InitializeComponent();
			Reset(listaString, parent, position, onSelecionar);
		}

		private void Reset(IList<String> listaString, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listaCamposOld = parent.Controls.Cast<Control>().FirstOrDefault(c => c.Name == Name) as ListaDeCampos;
			if (listaCamposOld != null)
			{
				parent.Controls.Remove(listaCamposOld);
				listaCamposOld.OnSelecionar = null;
				listaCamposOld.DataSource = null;
				listaCamposOld.Dispose();
			}

			OnSelecionar = onSelecionar;
			DataSource = listaString;
			parent.Controls.Add(this);
			Top = position.Y;
			Left = position.X;
			Visible = true;
			BringToFront();
			Focus();
		}

		private void ListaDeCampos_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Tab)
				DoSelecionar(Convert.ToString(SelectedItem));
		}

		private void listBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				DoSelecionar(null);
			else if (e.KeyCode == Keys.Enter)
				DoSelecionar(Convert.ToString(SelectedItem));
			else if ((e.KeyCode != Keys.Down) && (e.KeyCode != Keys.Up))
			{
				e.SuppressKeyPress = true;
				DoPesquisar(Convert.ToString((Char)e.KeyValue));
			}
		}

		private Boolean DoPesquisar(String chr)
		{
			var tempoDecorridoEmMiliSegundos = (DateTime.Now - _lastKey).TotalMilliseconds;
			_search = (tempoDecorridoEmMiliSegundos <= 800) ? _search + chr.ToUpper() : chr.ToUpper();
			_lastKey = DateTime.Now;

			String item = (DataSource as IEnumerable<String>).FirstOrDefault(i => i.ToUpper().StartsWith(_search)) ?? String.Empty;
			if (String.IsNullOrWhiteSpace(item))
				item = (DataSource as IEnumerable<String>).FirstOrDefault(i => i.ToUpper().EndsWith(_search)) ?? String.Empty;
			if (String.IsNullOrWhiteSpace(item))
				item = (DataSource as IEnumerable<String>).FirstOrDefault(i => i.ToUpper().Contains(_search)) ?? String.Empty;
			if (!String.IsNullOrWhiteSpace(item))
				SelectedItem = item;

			return !String.IsNullOrWhiteSpace(item);
		}

		private void ListaDeCampos_Leave(object sender, EventArgs e)
		{
			DoSelecionar(null);
		}

		private void Selecionar(object sender, EventArgs e)
		{
			DoSelecionar(Convert.ToString(SelectedItem));
		}

		private void DoSelecionar(String selectedItem)
		{
			try
			{
				if (OnSelecionar != null)
					OnSelecionar(selectedItem);
				if (Parent != null)
					Parent.Controls.Remove(this);
				OnSelecionar = null;
				DataSource = null;
				Dispose();
				GC.Collect();
			}
			catch (Exception) { }
		}

		public static void Exibir(IEnumerable<String> campos, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listaString = campos.OrderBy(a => a).ToList();
			new ListaDeCampos(listaString, parent, position, onSelecionar);
		}
	}
}