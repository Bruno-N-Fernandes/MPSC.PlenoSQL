using MPSC.PlenoSQL.AppWin.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public class DataGridViewOnDemand : DataGridView
	{
		private Int32 _linhasVisiveis;
		private IBancoDeDados _bancoDeDados;
		private IEnumerable<Object> _dados
		{
			get { return DataSource as IEnumerable<Object>; }
			set { DataSource = value == null ? null : value.ToList(); }
		}
		public IBancoDeDados BancoDeDados { set { _dados = null; _bancoDeDados = value; } }

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_linhasVisiveis = DisplayedRowCount(true);
		}

		protected override void OnScroll(ScrollEventArgs e)
		{
			if ((e.ScrollOrientation == ScrollOrientation.VerticalScroll) && (e.NewValue > e.OldValue) && (e.OldValue > 0))
			{
				if (_linhasVisiveis == 0)
					_linhasVisiveis = DisplayedRowCount(true);
				if (_linhasVisiveis + e.NewValue >= Rows.Count)
					Binding();
			}
			base.OnScroll(e);
		}

		public Int32 Binding()
		{
			var result = _bancoDeDados.DataBinding();
			if (_dados == null)
			{
				Enabled = result.Skip(1).Any();
				_dados = Enabled ? result.Skip(1) : result;
				_linhasVisiveis = DisplayedRowCount(true);
			}
			else
			{
				var cell = new Point(CurrentCell.ColumnIndex, FirstDisplayedScrollingRowIndex);
				_dados = _dados.Union(result.Skip(1));
				cell = SelecionarCelula(cell);
			}

			Application.DoEvents();
			AutoResizeColumns();
			if (Enabled)
				Focus();
			return Enabled ? 1 : 0;
		}

		private Point SelecionarCelula(Point cell)
		{
			if ((cell.X >= 0) && (cell.Y >= 0))
			{
				FirstDisplayedScrollingRowIndex = cell.Y;
				CurrentCell = this[cell.X, cell.Y];
			}
			return cell;
		}
	}
}