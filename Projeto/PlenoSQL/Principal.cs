using System;
using System.Collections.Generic;
using System.Linq;
using MP.PlenoBDNE.AppWin.View;

namespace MP.PlenoSQL.AppWin
{
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Colocar informações no StatusBar (conexão, usuário, banco, registros alterados) 
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Listar os objetos do banco de dados na coluna da esquerda (TreeView)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Exportar o resultado da query para TXT, XLS, XML, PDF, etc.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir escolher a fonte e o tamanho da mesma.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir Configurar o Colorir da Query
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Mensagem de aguarde, processsando
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Permitir o cancelamento da query.
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Criar um grupo de Favoritos (Cada grupo poderá agrupar vários arquivos)
	//TODO: Bruno Fernandes (08/08/2014 17:35) - Close All But This.
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Fazer Auto Reload dos arquivos alterados
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Implementar F4 para mostrar propriedades da tabela e do campo
	//TODO: Bruno Fernandes (11/08/2014 18:30) - Melhorar a separação de blocos de código (ponto e virgula)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de arquivos (para arrastar pelo Windows Explorer)
	//TODO: Bruno Fernandes (20/08/2014 01:41) - Implementar Drag And Drop de tabelas para o editor

	public static class Principal
	{
		[STAThread]
		public static Int32 Main(String[] args)
		{
			var linhaDeComando = new LinhaDeComando(args);
			if (linhaDeComando.PodeSerExecutada)
				return linhaDeComando.Executar();
			else
				return SingletonApplication.Run<Navegador>(args, onConfigurarParametro);
		}

		private static void onConfigurarParametro(Navegador form, Boolean appJaEstavaRodando, IEnumerable<String> parametros)
		{
			form.AbrirDocumentos(appJaEstavaRodando, parametros);
		}
	}
}