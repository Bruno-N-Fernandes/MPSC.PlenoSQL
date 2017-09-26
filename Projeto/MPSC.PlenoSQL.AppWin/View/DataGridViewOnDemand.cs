﻿using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections;
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
			set { DataSource = ((value == null) ? null : (value is IList ? value : value.ToList())); }
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
					Binding(100);
			}
			base.OnScroll(e);
		}

		public Int32 Binding(Int64 limite)
		{
			var result = _bancoDeDados.DataBinding(limite);
			if (_dados == null)
			{
				var lista = result.ToList();
				Enabled = lista.Count > 1;
				_dados = Enabled ? lista.Skip(1) : lista;
				_linhasVisiveis = DisplayedRowCount(true);
			}
			else
			{
				var point1 = new Point(CurrentCell.ColumnIndex, CurrentCell.RowIndex);
				var point2 = new Point(FirstDisplayedScrollingColumnIndex, FirstDisplayedScrollingRowIndex);
				_dados = _dados.Union(result.Skip(1));
				SelecionarCelula(point1, point2);
			}

			Application.DoEvents();
			AutoResizeColumns();
			if (Enabled)
				Focus();
			return Enabled ? 1 : 0;
		}

		private void SelecionarCelula(Point point1, Point point2)
		{
			if ((point1.X >= 0) && (point1.Y >= 0))
			{
				CurrentCell = this[point1.X, point1.Y];
				FirstDisplayedScrollingRowIndex = point2.Y;
				FirstDisplayedScrollingColumnIndex = point2.X;
			}
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