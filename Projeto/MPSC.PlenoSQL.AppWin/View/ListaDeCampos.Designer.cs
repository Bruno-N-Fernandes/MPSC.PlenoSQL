using System.Windows.Forms;

namespace MPSC.PlenoSQL.AppWin.View
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
			this.Name = "listBox";
			this.ScrollAlwaysVisible = true;
			this.Size = new System.Drawing.Size(300, 300);
			this.DoubleClick += new System.EventHandler(this.Selecionar);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ListaDeCampos_KeyPress);
			this.Leave += new System.EventHandler(this.ListaDeCampos_Leave);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ListaDeCampos_PreviewKeyDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}