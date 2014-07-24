using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LBJC.NavegadorDeDados
{
	public delegate void SelecionarEventHandler(String item);

	public partial class ListaDeCampos : ListBox
	{
		private event SelecionarEventHandler OnSelecionar;

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

		private void listBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				DoSelecionar(null);
			else if (e.KeyCode == Keys.Enter)
				DoSelecionar(Convert.ToString(SelectedItem));
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
			if (OnSelecionar != null)
				OnSelecionar(selectedItem);
			if (Parent != null)
				Parent.Controls.Remove(this);
			OnSelecionar = null;
			DataSource = null;
			Dispose();
			GC.Collect();
		}

		public static void Exibir(IEnumerable<String> campos, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listaString = campos.OrderBy(a => a).ToList();
			new ListaDeCampos(listaString, parent, position, onSelecionar);
		}
	}
}