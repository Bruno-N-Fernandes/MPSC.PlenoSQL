﻿using MPSC.PlenoSQL.Kernel.Dados.Base;
using MPSC.PlenoSQL.Kernel.Infra;
using MPSC.PlenoSQL.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
{
	public partial class Navegador : Form, INavegador
	{
		private IList<FileInfo> arquivos = new List<FileInfo>();
		private IQueryResult ActiveTab { get { return (tabQueryResult.TabPages.Count > 0) ? tabQueryResult.TabPages[tabQueryResult.SelectedIndex] as IQueryResult : NullQueryResult.Instance; } }

		public Boolean ShowEstatisticas { get { return ckEstatisticas.Checked; } private set { ckEstatisticas.Checked = value; } }
		public Boolean SalvarAoExecutar { get { return ckSalvarAoExecutar.Checked; } private set { ckSalvarAoExecutar.Checked = value; } }
		public Boolean ConverterToUpper { get { return ckUpperCase.Checked; } private set { ckUpperCase.Checked = value; } }
		public Boolean ColorirTextosSql { get { return ckColorir.Checked; } private set { ckColorir.Checked = value; } }

		public Navegador()
		{
			InitializeComponent();
			Text += " " + CoreAssembly.VersionString;
		}

		private void btNovoDocumento_Click(object sender, EventArgs e)
		{
			tabQueryResult.Controls.Add(new QueryResult());
			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
		}

		private void btAbrirDocumento_Click(object sender, EventArgs e)
		{
			AbrirArquivosImpl(FileUtil.GetFilesToOpen("Arquivos de Banco de Dados|*.sql;*.qry"));
		}

		public Navegador AbrirDocumentos(Boolean appJaEstavaRodando, IEnumerable<String> arquivos)
		{
			if (!appJaEstavaRodando)
				AbrirArquivosImpl(FileUtil.FileToArray(Cache.cArquivosAbertos, 1));
			AbrirArquivosImpl(arquivos);
			return this;
		}

		private void AbrirArquivosImpl(IEnumerable<String> arquivos)
		{
			foreach (var arquivo in arquivos.Where(a => !String.IsNullOrWhiteSpace(a)))
				if (!tabQueryResult.TabPages.OfType<IQueryResult>().Any(qr => qr.Arquivo.FullName == arquivo))
					tabQueryResult.Controls.Add(new QueryResult(new FileInfo(arquivo)));

			tabQueryResult.SelectedIndex = tabQueryResult.TabCount - 1;
			ActiveTab.Focus();
		}

		private void btSalvarDocumento_Click(object sender, EventArgs e)
		{
			ActiveTab.Salvar();
		}

		private void btSalvarTodos_Click(object sender, EventArgs e)
		{
			Boolean salvouTodos = true;
			foreach (IQueryResult queryResult in tabQueryResult.Controls)
				salvouTodos = salvouTodos && queryResult.Salvar();
		}

		private void btExecutar_Click(object sender, EventArgs e)
		{
			ActiveTab.Executar();
		}

		private void btAlterarConexao_Click(object sender, EventArgs e)
		{
			ActiveTab.AlterarConexao();
		}

		private void btFechar_Click(object sender, EventArgs e)
		{
			var tab = ActiveTab;
			if (tab.PodeFechar())
			{
				tabQueryResult.Controls.Remove(tab as TabPage);
				tab.Fechar();
			}
		}

		private void tabQueryResult_Click(object sender, EventArgs e)
		{
			ActiveTab.Focus();
		}

		private void Navegador_Load(object sender, EventArgs e)
		{
			var arquivos = FileUtil.FileToArray(Cache.cArquivosAbertos, 1);

			ConverterToUpper = Configuracao.Instancia.ObterValorConfiguracao(Cache.cEditor_ConverterToUpper)?.Equals(true.ToString()) ?? false;
			SalvarAoExecutar = Configuracao.Instancia.ObterValorConfiguracao(Cache.cEditor_SalvarAoExecutar)?.Equals(true.ToString()) ?? false;
			ColorirTextosSql = Configuracao.Instancia.ObterValorConfiguracao(Cache.cEditor_ColorirTextosSql)?.Equals(true.ToString()) ?? false;
			ShowEstatisticas = Configuracao.Instancia.ObterValorConfiguracao(Cache.cEditor_ShowEstatisticas)?.Equals(true.ToString()) ?? false;

			if (tabQueryResult.TabPages.Count == 0)
				AbrirArquivosImpl(arquivos);
			tvDataConnection.CreateChildren();
		}

		private void Navegador_FormClosing(object sender, FormClosingEventArgs e)
		{
			arquivos.Clear();
			Boolean salvouTodos = true;
			var i = 0;
			while (salvouTodos && (i < tabQueryResult.Controls.Count))
			{
				var queryResult = tabQueryResult.Controls[i++] as QueryResult;
				if (!queryResult.PodeFechar())
					salvouTodos = false;
			}

			while (salvouTodos && tabQueryResult.Controls.Count > 0)
			{
				var queryResult = tabQueryResult.Controls[0] as QueryResult;
				tabQueryResult.Controls.Remove(queryResult);
				queryResult.Fechar();

				if (File.Exists(queryResult.Arquivo.FullName))
					arquivos.Add(queryResult.Arquivo);
			}

			e.Cancel = !salvouTodos;
		}

		private void Navegador_FormClosed(object sender, FormClosedEventArgs e)
		{
			Visible = false;
			FileUtil.ArrayToFile(Cache.cArquivosAbertos, arquivos.Select(f => f.FullName).ToArray());

			Configuracao.Instancia.GravarValorConfiguracao(Cache.cEditor_ConverterToUpper, ConverterToUpper);
			Configuracao.Instancia.GravarValorConfiguracao(Cache.cEditor_SalvarAoExecutar, SalvarAoExecutar);
			Configuracao.Instancia.GravarValorConfiguracao(Cache.cEditor_ColorirTextosSql, ColorirTextosSql);
			Configuracao.Instancia.GravarValorConfiguracao(Cache.cEditor_ShowEstatisticas, ShowEstatisticas);
			tvDataConnection.Dispose();
			BancoDados.LimparCache();
			Configuracao.Instancia.SaveConstantes();
		}

		public void Status(String mensagem)
		{
			tsslConexao.Text = mensagem;
		}

		private void txtFiltroTreeView_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				tvDataConnection.Filtrar(txtFiltroTreeView.Text);
		}

		private void btDefinirConstantes_Click(object sender, EventArgs e)
		{
			DefinicaoDeConstantes.Visualizar(Constantes.Instancia, ActiveTab.Arquivo.FullName);
		}

		private void btGerarVO_Click(object sender, EventArgs e)
		{
			new ClasseUtilForm().ShowDialog();
		}

		private void btGerarExcel_Click(object sender, EventArgs e)
		{
			ActiveTab.GerarExcel();
		}
	}
}