using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace MP.PlenoSQL.AppWin.GestorDeAplicacao
{
	public class SingletonApplicationOf<TForm> : WindowsFormsApplicationBase where TForm : Form, new()
	{
		public delegate void OnConfigurarParametro(TForm form, Boolean appJaEstavaRodando, IEnumerable<String> parametros);
		private OnConfigurarParametro configurarParametro;
		private readonly String[] parametros;
		private Int32 exitCode = -1;

		public SingletonApplicationOf(String[] args, Boolean enableVisualStyles)
		{
			this.parametros = args;
			this.IsSingleInstance = true;
			this.EnableVisualStyles = enableVisualStyles;
			this.ShutdownStyle = ShutdownMode.AfterMainFormCloses;
			this.StartupNextInstance += new StartupNextInstanceEventHandler(application_StartupNextInstance);
			this.UnhandledException += application_Exception;
			Application.ThreadException += application_Exception;
			AppDomain.CurrentDomain.FirstChanceException += new EventHandler<FirstChanceExceptionEventArgs>(application_Exception);
			AppDomain.CurrentDomain.UnhandledException += application_Exception;
		}

		private void application_Exception(object sender, EventArgs e)
		{
			var e1 = e as ThreadExceptionEventArgs;
			var e2 = e as System.UnhandledExceptionEventArgs;
			var e3 = e as Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs;
			var e4 = e as FirstChanceExceptionEventArgs;
		}

		public Int32 Run(OnConfigurarParametro configurarParametro)
		{
			this.configurarParametro = configurarParametro;
			base.Run(parametros);
			GC.Collect();
			return this.exitCode;
		}

		protected override void OnCreateMainForm()
		{
			MainForm = new TForm();
			configurarParametro.Invoke((TForm)MainForm, false, CommandLineArgs);
			exitCode = 0;
		}

		protected void application_StartupNextInstance(Object sender, StartupNextInstanceEventArgs eventArgs)
		{
			configurarParametro.Invoke((TForm)MainForm, true, eventArgs.CommandLine);
		}
	}
}