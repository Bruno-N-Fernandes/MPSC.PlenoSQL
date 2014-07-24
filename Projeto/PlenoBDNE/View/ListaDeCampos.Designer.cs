namespace LBJC.NavegadorDeDados
{
	partial class ListaDeCampos
	{

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ListaDeCampos
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.FormattingEnabled = true;
			this.Name = "listBox";
			this.ScrollAlwaysVisible = true;
			this.Size = new System.Drawing.Size(300, 300);
			this.DoubleClick += new System.EventHandler(this.Selecionar);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
			this.Leave += new System.EventHandler(this.ListaDeCampos_Leave);
			this.ResumeLayout(false);

		}

		#endregion
	}
}