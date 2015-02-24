using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MPSC.PlenoSQL.AppWin.Interface;

namespace MPSC.PlenoSQL.AppWin.View
{
	public class DataGridViewLazy : DataGridView
	{
		private IBancoDeDados BancoDeDados;
		public IBancoDeDados Source { set { DataSource = null; BancoDeDados = value; } }

		public DataGridViewLazy()
		{

		}

		protected override void OnScroll(ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
				if (e.NewValue >= Rows.Count)
					Binding();
			base.OnScroll(e);
		}

		public void Binding()
		{
			var result = BancoDeDados.DataBinding();
			if (DataSource == null)
			{
				var lista = result.Skip(1).ToList();
				var enabled = lista.Count > 0;
				Enabled = enabled;
				DataSource = enabled ? lista : result.ToList();
			}
			else
			{
				var linha = FirstDisplayedScrollingRowIndex;
				this.DataSource = (DataSource as IEnumerable<Object>).Union(result.Skip(1)).ToList();
				if (linha >= 0)
					FirstDisplayedScrollingRowIndex = linha;
			}

			AutoResizeColumns();
			if (Enabled)
				Focus();
			Application.DoEvents();
		}

	}
}
