using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public delegate void SelecionarEventHandler(String item);

	public partial class AutoCompletar : TextBox
	{
		private event SelecionarEventHandler _onSelecionar;
		private ListBox _listBox = new ListBox();

		private AutoCompletar(IList<String> listaString, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			InitializeComponent();
			Reset(listaString, parent, position, onSelecionar);
			BringToFront();
			Focus();
		}

		private void Reset(IList<String> listaString, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listBoxOld = parent.Controls.Cast<Control>().FirstOrDefault(c => (c.Name == Name) && (c is ListBox));
			var textBoxOld = parent.Controls.Cast<Control>().FirstOrDefault(c => (c.Name == Name) && (c is TextBox));
			Remover(parent, listBoxOld as ListBox, textBoxOld as AutoCompletar);

			Top = position.Y - 15;
			Left = position.X - 5;
			_onSelecionar = onSelecionar;
			parent.Controls.Add(this);
			Leave += lostFocus;

			NewListBox(listaString, parent, position);
		}

		private void NewListBox(IList<String> listaString, Control parent, Point position)
		{
			parent.Controls.Add(_listBox);
			_listBox.Text = String.Empty;
			_listBox.Name = Name;
			_listBox.Top = position.Y + 4;
			_listBox.Left = Left;
			_listBox.Width = Width;
			_listBox.Height = 300;
			_listBox.Visible = true;
			_listBox.BackColor = Color.White;
			_listBox.DataSource = listaString;
			_listBox.ScrollAlwaysVisible = true;
			_listBox.BringToFront();
			_listBox.DoubleClick += Selecionar;
			_listBox.PreviewKeyDown += Selecionar;
			_listBox.Leave += lostFocus;
		}

		private void keyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				DoSelecionar(null);

			else if (e.KeyCode == Keys.Enter)
				DoSelecionar(Convert.ToString(_listBox.SelectedItem));

			else if (e.KeyCode == Keys.Up)
			{
				_listBox.Focus();
				if (_listBox.SelectedIndex >= 0) _listBox.SelectedIndex--;
			}
			else if (e.KeyCode == Keys.Down)
			{
				_listBox.Focus();
				if (_listBox.SelectedIndex < _listBox.Items.Count) _listBox.SelectedIndex++;
			}

			else if ((e.KeyCode != Keys.Left) && (e.KeyCode != Keys.Right))
				DoPesquisar(Text.ToUpper());
		}

		private Boolean DoPesquisar(String search)
		{
			var lista = _listBox.DataSource as IEnumerable<String>;

			var item = lista.FirstOrDefault(i => i.ToUpper().Equals(search)) ?? String.Empty;

			if (String.IsNullOrWhiteSpace(item))
				item = lista.FirstOrDefault(i => i.ToUpper().StartsWith(search)) ?? String.Empty;

			if (String.IsNullOrWhiteSpace(item))
				item = lista.FirstOrDefault(i => i.ToUpper().Contains(search)) ?? String.Empty;

			if (!String.IsNullOrWhiteSpace(item))
				_listBox.SelectedIndex = _listBox.FindStringExact(item);

			return !String.IsNullOrWhiteSpace(item);
		}

		private void lostFocus(Object sender, EventArgs e)
		{
			if (!this.Focused && !_listBox.Focused)
				DoSelecionar(null);
		}

		private void Selecionar(Object sender, EventArgs e)
		{
			var previewKeyDownEventArgs = (e as PreviewKeyDownEventArgs);

			if (previewKeyDownEventArgs == null) // Double Click
				DoSelecionar(Convert.ToString(_listBox.SelectedItem));
			else if ((previewKeyDownEventArgs.KeyCode == Keys.Enter) || (previewKeyDownEventArgs.KeyCode == Keys.Tab))
				DoSelecionar(Convert.ToString(_listBox.SelectedItem));
			else if ((previewKeyDownEventArgs.KeyCode == Keys.Escape))
				DoSelecionar(null);
			else if ((previewKeyDownEventArgs.KeyCode != Keys.Up) && (previewKeyDownEventArgs.KeyCode != Keys.Down))
			{
				Focus();
				if ((previewKeyDownEventArgs.KeyValue >= 65) && (previewKeyDownEventArgs.KeyValue <= 90))
					Text += ((Char)previewKeyDownEventArgs.KeyValue);
				SelectionStart = Text.Length;
				SelectionLength = 0;
			}
		}

		private void DoSelecionar(String selectedItem)
		{
			try
			{
				if (_onSelecionar != null)
					_onSelecionar(selectedItem);
			}
			catch (Exception vException)
			{
				if (Parent is IMessageResult)
					(Parent as IMessageResult).ShowLog(vException.Message, "Erro");
			}
			finally
			{
				Remover(Parent, _listBox, this);
				GC.Collect();
			}
		}

		private void Remover(Control parent, ListBox listBox, AutoCompletar textBox)
		{
			if (parent != null)
			{
				if (listBox != null)
				{
					parent.Controls.Remove(listBox);
					listBox.DataSource = null;
					listBox.Dispose();
				}

				if (textBox != null)
				{
					parent.Controls.Remove(textBox);
					textBox._onSelecionar = null;
					textBox.Dispose();
				}
			}
		}

		public static void Exibir(IEnumerable<String> campos, Control parent, Point position, SelecionarEventHandler onSelecionar)
		{
			var listaString = campos.OrderBy(a => a).ToList();
			new AutoCompletar(listaString, parent, position, onSelecionar);
		}
	}
}