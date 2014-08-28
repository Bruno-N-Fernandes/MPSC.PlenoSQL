using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace MP.PlenoBDNE.AppWin
{
	public class GenericSingletornApplication<TForm> : SingletornApplication<TForm> where TForm : Form, new()
	{
		public GenericSingletornApplication(String[] parametros) : base(null, parametros) { }
		protected override TForm GetMainForm() { return new TForm(); }
	}

	public class SingletornApplication<TForm> : WindowsFormsApplicationBase where TForm : Form
	{
		public delegate void OnConfigurarParametro(TForm form, Boolean isNovo, String[] parametros);
		private OnConfigurarParametro configurarParametro;
		private TForm form;
		private String[] parametros;

		public SingletornApplication(TForm form, String[] parametros)
		{
			this.form = form;
			this.parametros = parametros;
			this.IsSingleInstance = true;
			this.EnableVisualStyles = true;
			this.ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
			this.StartupNextInstance += new StartupNextInstanceEventHandler(this.SIApp_StartupNextInstance);
		}

		public void Run(OnConfigurarParametro configurarParametro)
		{
			this.configurarParametro = configurarParametro;
			base.Run(parametros);
		}

		protected virtual TForm GetMainForm()
		{
			return this.form;
		}

		protected override void OnCreateMainForm()
		{
			this.MainForm = this.form = GetMainForm();
			configurarParametro.Invoke(this.form, true, this.CommandLineArgs.ToArray());
		}

		protected void SIApp_StartupNextInstance(Object sender, StartupNextInstanceEventArgs eventArgs)
		{
			configurarParametro.Invoke(this.form, false, eventArgs.CommandLine.ToArray());
		}
	}
}