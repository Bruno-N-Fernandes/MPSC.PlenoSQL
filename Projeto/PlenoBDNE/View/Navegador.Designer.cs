namespace LBJC.NavegadorDeDados
{
	partial class Navegador
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Navegador));
			this.scVertical = new System.Windows.Forms.SplitContainer();
			this.tvDataConnection = new System.Windows.Forms.TreeView();
			this.tabQueryResult = new System.Windows.Forms.TabControl();
			this.ssStatus = new System.Windows.Forms.StatusStrip();
			this.tsBarraFerramentas = new System.Windows.Forms.ToolStrip();
			this.btNovoDocumento = new System.Windows.Forms.ToolStripButton();
			this.btAbrirDocumento = new System.Windows.Forms.ToolStripButton();
			this.btSalvarDocumento = new System.Windows.Forms.ToolStripButton();
			this.btSalvarTodos = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btExecutar = new System.Windows.Forms.ToolStripButton();
			this.btFechar = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.scVertical)).BeginInit();
			this.scVertical.Panel1.SuspendLayout();
			this.scVertical.Panel2.SuspendLayout();
			this.scVertical.SuspendLayout();
			this.tsBarraFerramentas.SuspendLayout();
			this.SuspendLayout();
			// 
			// scVertical
			// 
			this.scVertical.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scVertical.Location = new System.Drawing.Point(0, 28);
			this.scVertical.Name = "scVertical";
			// 
			// scVertical.Panel1
			// 
			this.scVertical.Panel1.Controls.Add(this.tvDataConnection);
			// 
			// scVertical.Panel2
			// 
			this.scVertical.Panel2.Controls.Add(this.tabQueryResult);
			this.scVertical.Size = new System.Drawing.Size(623, 320);
			this.scVertical.SplitterDistance = 64;
			this.scVertical.TabIndex = 0;
			// 
			// tvDataConnection
			// 
			this.tvDataConnection.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvDataConnection.Location = new System.Drawing.Point(0, 0);
			this.tvDataConnection.Name = "tvDataConnection";
			this.tvDataConnection.Size = new System.Drawing.Size(64, 320);
			this.tvDataConnection.TabIndex = 0;
			// 
			// tabQueryResult
			// 
			this.tabQueryResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabQueryResult.Location = new System.Drawing.Point(0, 0);
			this.tabQueryResult.Name = "tabQueryResult";
			this.tabQueryResult.SelectedIndex = 0;
			this.tabQueryResult.Size = new System.Drawing.Size(555, 320);
			this.tabQueryResult.TabIndex = 0;
			this.tabQueryResult.Click += new System.EventHandler(this.tabQueryResult_Click);
			// 
			// ssStatus
			// 
			this.ssStatus.Location = new System.Drawing.Point(0, 351);
			this.ssStatus.Name = "ssStatus";
			this.ssStatus.Size = new System.Drawing.Size(623, 22);
			this.ssStatus.TabIndex = 1;
			this.ssStatus.Text = "statusStrip1";
			// 
			// tsBarraFerramentas
			// 
			this.tsBarraFerramentas.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsBarraFerramentas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btNovoDocumento,
            this.btAbrirDocumento,
            this.btSalvarDocumento,
            this.btSalvarTodos,
            this.toolStripSeparator1,
            this.btExecutar,
            this.btFechar});
			this.tsBarraFerramentas.Location = new System.Drawing.Point(0, 0);
			this.tsBarraFerramentas.Name = "tsBarraFerramentas";
			this.tsBarraFerramentas.Size = new System.Drawing.Size(623, 27);
			this.tsBarraFerramentas.TabIndex = 2;
			this.tsBarraFerramentas.Text = "toolStrip1";
			// 
			// btNovoDocumento
			// 
			this.btNovoDocumento.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btNovoDocumento.Image = ((System.Drawing.Image)(resources.GetObject("btNovoDocumento.Image")));
			this.btNovoDocumento.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btNovoDocumento.Name = "btNovoDocumento";
			this.btNovoDocumento.Size = new System.Drawing.Size(24, 24);
			this.btNovoDocumento.Text = "Novo";
			this.btNovoDocumento.Click += new System.EventHandler(this.btNovoDocumento_Click);
			// 
			// btAbrirDocumento
			// 
			this.btAbrirDocumento.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btAbrirDocumento.Image = ((System.Drawing.Image)(resources.GetObject("btAbrirDocumento.Image")));
			this.btAbrirDocumento.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btAbrirDocumento.Name = "btAbrirDocumento";
			this.btAbrirDocumento.Size = new System.Drawing.Size(24, 24);
			this.btAbrirDocumento.Text = "Abrir";
			this.btAbrirDocumento.Click += new System.EventHandler(this.btAbrirDocumento_Click);
			// 
			// btSalvarDocumento
			// 
			this.btSalvarDocumento.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btSalvarDocumento.Image = ((System.Drawing.Image)(resources.GetObject("btSalvarDocumento.Image")));
			this.btSalvarDocumento.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btSalvarDocumento.Name = "btSalvarDocumento";
			this.btSalvarDocumento.Size = new System.Drawing.Size(24, 24);
			this.btSalvarDocumento.Text = "Salvar";
			this.btSalvarDocumento.Click += new System.EventHandler(this.btSalvarDocumento_Click);
			// 
			// btSalvarTodos
			// 
			this.btSalvarTodos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btSalvarTodos.Image = ((System.Drawing.Image)(resources.GetObject("btSalvarTodos.Image")));
			this.btSalvarTodos.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btSalvarTodos.Name = "btSalvarTodos";
			this.btSalvarTodos.Size = new System.Drawing.Size(24, 24);
			this.btSalvarTodos.Text = "Salvar Todos";
			this.btSalvarTodos.Click += new System.EventHandler(this.btSalvarTodos_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
			// 
			// btExecutar
			// 
			this.btExecutar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btExecutar.Image = ((System.Drawing.Image)(resources.GetObject("btExecutar.Image")));
			this.btExecutar.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btExecutar.Name = "btExecutar";
			this.btExecutar.Size = new System.Drawing.Size(24, 24);
			this.btExecutar.Text = "Executar";
			this.btExecutar.Click += new System.EventHandler(this.btExecutar_Click);
			// 
			// btFechar
			// 
			this.btFechar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btFechar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btFechar.Image = ((System.Drawing.Image)(resources.GetObject("btFechar.Image")));
			this.btFechar.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btFechar.Name = "btFechar";
			this.btFechar.Size = new System.Drawing.Size(24, 24);
			this.btFechar.Text = "Fechar";
			this.btFechar.Click += new System.EventHandler(this.btFechar_Click);
			// 
			// Navegador
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(623, 373);
			this.Controls.Add(this.tsBarraFerramentas);
			this.Controls.Add(this.ssStatus);
			this.Controls.Add(this.scVertical);
			this.Name = "Navegador";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Navegador De Banco De Dados";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Navegador_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Navegador_FormClosed);
			this.Load += new System.EventHandler(this.Navegador_Load);
			this.scVertical.Panel1.ResumeLayout(false);
			this.scVertical.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scVertical)).EndInit();
			this.scVertical.ResumeLayout(false);
			this.tsBarraFerramentas.ResumeLayout(false);
			this.tsBarraFerramentas.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer scVertical;
		private System.Windows.Forms.TreeView tvDataConnection;
		private System.Windows.Forms.StatusStrip ssStatus;
		private System.Windows.Forms.ToolStrip tsBarraFerramentas;
		private System.Windows.Forms.ToolStripButton btNovoDocumento;
		private System.Windows.Forms.TabControl tabQueryResult;
		private System.Windows.Forms.ToolStripButton btExecutar;
		private System.Windows.Forms.ToolStripButton btAbrirDocumento;
		private System.Windows.Forms.ToolStripButton btSalvarDocumento;
		private System.Windows.Forms.ToolStripButton btSalvarTodos;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton btFechar;
	}
}

