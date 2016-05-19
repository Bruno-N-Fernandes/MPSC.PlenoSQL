﻿using System.Linq;
using MPSC.PlenoSQL.AppWin.View;
using MPSC.PlenoSQL.Kernel.GestorDeAplicacao;
using MPSC.PlenoSQL.Kernel.Infra;
using System;
using System.Collections.Generic;
using System.IO;

namespace MPSC.PlenoSQL.AppWin
{
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Colocar informações no StatusBar (conexão, usuário, banco, registros alterados) 
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Melhorar a Listagem os objetos do banco de dados na coluna da esquerda (TreeView)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Exportar o resultado da query para TXT, XLS, XML, PDF, etc.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir escolher a fonte e o tamanho da mesma.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir Configurar o Colorir da Query
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Mensagem de aguarde, processsando
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir o cancelamento da query.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Criar um grupo de Favoritos (Cada grupo poderá agrupar vários arquivos)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Close All But This.
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Fazer Auto Reload dos arquivos alterados
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar F4 para mostrar propriedades da tabela e do campo
	//DONE: Bruno Fernandes (11/08/2014 18:30) - Melhorar a separação de blocos de código (ponto e virgula)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de arquivos (para arrastar pelo Windows Explorer)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de tabelas para o editor
	//TODO: Bruno Fernandes (19/05/2016 18:15) - Intelissense para joins
	//TODO: Bruno Fernandes (19/05/2016 18:15) - Cache dos intelissense
	//TODO: Bruno Fernandes (19/05/2016 18:15) - Agrupamento de informações

	public static class Principal
	{
		public static readonly String arquivoConfig1 = Path.GetTempPath() + "PlenoSQL.files";
		public static readonly String arquivoConfig2 = Path.GetTempPath() + "PlenoSQL.cgf";
		public static readonly String arquivoConfig3 = Path.GetTempPath() + "PlenoSQL.dic";
		public static readonly String dicFile = FileUtil.FileToArray(arquivoConfig3, 1).FirstOrDefault();

		[STAThread]
		public static Int32 Main(String[] args)
		{
			var linhaDeComando = new LinhaDeComando(args);
			try
			{
				if (linhaDeComando.PodeSerExecutada)
					return linhaDeComando.Executar();
				else
					return SingletonApplication.Run<Navegador>(args, onConfigurarParametro);
			}
			finally
			{
				FileUtil.ArrayToFile(arquivoConfig3, String.IsNullOrWhiteSpace(dicFile) ? @"D:\Dropbox\Empresa\Apps\User.dic" : dicFile);
			}
		}

		private static void onConfigurarParametro(Navegador form, Boolean appJaEstavaRodando, IEnumerable<String> parametros)
		{
			form.AbrirDocumentos(appJaEstavaRodando, parametros);
		}
	}
}