using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
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
			var labelOld = parent.Controls.Cast<Control>().FirstOrDefault(c => c.Name == Name) as Label;
			if (labelOld != null)
			{
				parent.Controls.Remove(labelOld);
				labelOld.Dispose();
			}

			Top = position.Y;
			Left = position.X - 5;
			OnSelecionar = onSelecionar;
			DataSource = listaString;
			parent.Controls.Add(this);
			NewLabel(parent, position);
			Visible = true;
			BringToFront();
			Focus();
		}

		Label label = new Label();
		private void NewLabel(Control parent, Point position)
		{
			parent.Controls.Add(label);
			label.Text = String.Empty;
			label.Name = "Label";
			label.Top = Top - 12;
			label.Left = Left - 2;
			label.Width = Width;
			label.Visible = true;
			label.BackColor = Color.White;
			label.BringToFront();
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
			else if ((e.KeyCode != Keys.Down) && (e.KeyCode != Keys.Up) && (e.KeyCode != Keys.Left) && (e.KeyCode != Keys.Right) && (e.KeyCode != Keys.Shift))
			{
				//e.SuppressKeyPress = false;
				if (e.KeyCode == Keys.Delete)
				{
					_lastKey = DateTime.Now.AddMinutes(-1);
					DoPesquisar(String.Empty);
				}
				//else
					//DoPesquisar(Convert.ToString((Char)e.KeyValue));
			}
		}

		private void ListaDeCampos_KeyPress(object sender, KeyPressEventArgs e)
		{
			DoPesquisar(Convert.ToString(e.KeyChar));
		}

		private Boolean DoPesquisar(String chr)
		{
			var tempoDecorridoEmMiliSegundos = (DateTime.Now - _lastKey).TotalMilliseconds;
			_lastKey = DateTime.Now;
			_search = (tempoDecorridoEmMiliSegundos <= 5000) ? ((chr.Equals("\b") && (_search.Length > 0)) ? _search.Substring(0, _search.Length - 1) : _search + chr) : (chr.Equals("\b") ? String.Empty : chr);
			label.Text = _search;
			var search = _search.ToUpper();

			var lista = DataSource as IEnumerable<String>;

			var item = lista.FirstOrDefault(i => i.ToUpper().Equals(search)) ?? String.Empty;

			if (String.IsNullOrWhiteSpace(item))
				item = lista.FirstOrDefault(i => i.ToUpper().StartsWith(search)) ?? String.Empty;
			
			if (String.IsNullOrWhiteSpace(item))
				item = lista.FirstOrDefault(i => i.ToUpper().Contains(search)) ?? String.Empty;

			if (!String.IsNullOrWhiteSpace(item))
				SelectedIndex = this.FindStringExact(item);

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
				{
					Parent.Controls.Remove(label);
					Parent.Controls.Remove(this);
				}
				OnSelecionar = null;
				DataSource = null;
				Dispose();
				label.Dispose();
				GC.Collect();
			}
			catch (Exception vException)
			{
				if (Parent is IMessageResult)
					(Parent as IMessageResult).ShowLog(vException.Message, "Erro");
			}
		}

		public static void Exibir(IEnumerable<String> campos, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listaString = campos.OrderBy(a => a).ToList();
			new ListaDeCampos(listaString, parent, position, onSelecionar);
		}
	}
}