using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Math;


namespace MPSC.PlenoSQL.AppWin.View
{
	public class DataGridViewOnDemand : DataGridView
	{
		private Int32 _linhasVisiveis;
		private IBancoDeDados _bancoDeDados;
		private IEnumerable<Object> _dados
		{
			get { return DataSource as IEnumerable<Object>; }
			set { DataSource = ((value == null) ? null : ((value as IList) ?? value.ToList())); }
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
				if (_linhasVisiveis + e.NewValue >= (Rows.Count - 10))
					Binding(100);
			}
			base.OnScroll(e);
		}

		public Int32 Binding(Int64 limite)
		{
			var result = _bancoDeDados.DataBinding(limite);
			var lista = result.Skip(1);
			if (lista.Any())
			{
				if (_dados == null)
				{
					_dados = lista;
					_linhasVisiveis = DisplayedRowCount(true);
					AutoResizeColumns();
					Focus();
				}
				else
				{
					var point1 = new Point(Max(CurrentCell.ColumnIndex, 0), Max(CurrentCell.RowIndex, 0));
					var point2 = new Point(Max(FirstDisplayedScrollingColumnIndex, 0), Max(FirstDisplayedScrollingRowIndex, 0));
					_dados = _dados.Union(lista);
					SelecionarCelula(point1, point2);
				}
			}
			else if (_dados == null)
			{
				_dados = result;
				Enabled = false;
				AutoResizeColumns();
			}

			Application.DoEvents();

			return Enabled ? 1 : 0;
		}

		private void SelecionarCelula(Point point1, Point point2)
		{
			if (CurrentCell != this[point1.X, point1.Y])
				CurrentCell = this[point1.X, point1.Y];

			if (FirstDisplayedScrollingColumnIndex != point2.X)
				FirstDisplayedScrollingColumnIndex = point2.X;

			if (FirstDisplayedScrollingRowIndex != point2.Y)
				FirstDisplayedScrollingRowIndex = point2.Y;
		}

		public void Free()
		{
			DataSource = null;
			if (_bancoDeDados != null)
				_bancoDeDados.Dispose();
			_bancoDeDados = null;
			Dispose();
		}
	}
}