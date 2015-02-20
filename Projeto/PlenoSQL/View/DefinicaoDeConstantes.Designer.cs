namespace MPSC.PlenoSQL.AppWin.View
{
	partial class DefinicaoDeConstantes
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
			this.dgConstantes = new System.Windows.Forms.DataGridView();
			this.txtNome = new System.Windows.Forms.TextBox();
			this.txtValor = new System.Windows.Forms.TextBox();
			this.btIncluir = new System.Windows.Forms.Button();
			this.btExcluir = new System.Windows.Forms.Button();
			this.cbEscopo = new System.Windows.Forms.ComboBox();
			this.rbTodas = new System.Windows.Forms.RadioButton();
			this.gbFiltro = new System.Windows.Forms.GroupBox();
			this.rbAtivas = new System.Windows.Forms.RadioButton();
			this.rbTodasArquivo = new System.Windows.Forms.RadioButton();
			this.rbLocais = new System.Windows.Forms.RadioButton();
			this.rbGlobais = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).BeginInit();
			this.gbFiltro.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgConstantes
			// 
			this.dgConstantes.AllowUserToOrderColumns = true;
			this.dgConstantes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgConstantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgConstantes.Location = new System.Drawing.Point(0, 40);
			this.dgConstantes.Name = "dgConstantes";
			this.dgConstantes.Size = new System.Drawing.Size(470, 200);
			this.dgConstantes.TabIndex = 1;
			// 
			// txtNome
			// 
			this.txtNome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtNome.Location = new System.Drawing.Point(0, 244);
			this.txtNome.Name = "txtNome";
			this.txtNome.Size = new System.Drawing.Size(100, 20);
			this.txtNome.TabIndex = 2;
			// 
			// txtValor
			// 
			this.txtValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtValor.Location = new System.Drawing.Point(120, 244);
			this.txtValor.Name = "txtValor";
			this.txtValor.Size = new System.Drawing.Size(100, 20);
			this.txtValor.TabIndex = 3;
			// 
			// btIncluir
			// 
			this.btIncluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btIncluir.Location = new System.Drawing.Point(350, 243);
			this.btIncluir.Name = "btIncluir";
			this.btIncluir.Size = new System.Drawing.Size(55, 23);
			this.btIncluir.TabIndex = 5;
			this.btIncluir.Text = "Incluir";
			this.btIncluir.UseVisualStyleBackColor = true;
			this.btIncluir.Click += new System.EventHandler(this.btIncluir_Click);
			// 
			// btExcluir
			// 
			this.btExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btExcluir.Location = new System.Drawing.Point(410, 243);
			this.btExcluir.Name = "btExcluir";
			this.btExcluir.Size = new System.Drawing.Size(55, 23);
			this.btExcluir.TabIndex = 6;
			this.btExcluir.Text = "Excluir";
			this.btExcluir.UseVisualStyleBackColor = true;
			this.btExcluir.Click += new System.EventHandler(this.btExcluir_Click);
			// 
			// cbEscopo
			// 
			this.cbEscopo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbEscopo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbEscopo.FormattingEnabled = true;
			this.cbEscopo.Location = new System.Drawing.Point(224, 244);
			this.cbEscopo.Name = "cbEscopo";
			this.cbEscopo.Size = new System.Drawing.Size(120, 21);
			this.cbEscopo.TabIndex = 4;
			// 
			// rbTodas
			// 
			this.rbTodas.AutoSize = true;
			this.rbTodas.Location = new System.Drawing.Point(2, 12);
			this.rbTodas.Name = "rbTodas";
			this.rbTodas.Size = new System.Drawing.Size(161, 17);
			this.rbTodas.TabIndex = 0;
			this.rbTodas.TabStop = true;
			this.rbTodas.Tag = "0";
			this.rbTodas.Text = "Todas de Todos os Arquivos";
			this.rbTodas.UseVisualStyleBackColor = true;
			this.rbTodas.CheckedChanged += new System.EventHandler(this.filtroChanged);
			// 
			// gbFiltro
			// 
			this.gbFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbFiltro.Controls.Add(this.rbAtivas);
			this.gbFiltro.Controls.Add(this.rbTodasArquivo);
			this.gbFiltro.Controls.Add(this.rbLocais);
			this.gbFiltro.Controls.Add(this.rbGlobais);
			this.gbFiltro.Controls.Add(this.rbTodas);
			this.gbFiltro.Location = new System.Drawing.Point(0, 0);
			this.gbFiltro.Margin = new System.Windows.Forms.Padding(0);
			this.gbFiltro.Name = "gbFiltro";
			this.gbFiltro.Padding = new System.Windows.Forms.Padding(0);
			this.gbFiltro.Size = new System.Drawing.Size(470, 40);
			this.gbFiltro.TabIndex = 0;
			this.gbFiltro.TabStop = false;
			this.gbFiltro.Text = "Filtro";
			// 
			// rbAtivas
			// 
			this.rbAtivas.AutoSize = true;
			this.rbAtivas.Checked = true;
			this.rbAtivas.Location = new System.Drawing.Point(408, 12);
			this.rbAtivas.Name = "rbAtivas";
			this.rbAtivas.Size = new System.Drawing.Size(54, 17);
			this.rbAtivas.TabIndex = 4;
			this.rbAtivas.Tag = "4";
			this.rbAtivas.Text = "Ativas";
			this.rbAtivas.UseVisualStyleBackColor = true;
			this.rbAtivas.CheckedChanged += new System.EventHandler(this.filtroChanged);
			// 
			// rbTodasArquivo
			// 
			this.rbTodasArquivo.AutoSize = true;
			this.rbTodasArquivo.Location = new System.Drawing.Point(165, 12);
			this.rbTodasArquivo.Name = "rbTodasArquivo";
			this.rbTodasArquivo.Size = new System.Drawing.Size(123, 17);
			this.rbTodasArquivo.TabIndex = 1;
			this.rbTodasArquivo.Tag = "1";
			this.rbTodasArquivo.Text = "Todas deste Arquivo";
			this.rbTodasArquivo.UseVisualStyleBackColor = true;
			this.rbTodasArquivo.CheckedChanged += new System.EventHandler(this.filtroChanged);
			// 
			// rbLocais
			// 
			this.rbLocais.AutoSize = true;
			this.rbLocais.Location = new System.Drawing.Point(352, 12);
			this.rbLocais.Name = "rbLocais";
			this.rbLocais.Size = new System.Drawing.Size(56, 17);
			this.rbLocais.TabIndex = 3;
			this.rbLocais.TabStop = true;
			this.rbLocais.Tag = "3";
			this.rbLocais.Text = "Locais";
			this.rbLocais.UseVisualStyleBackColor = true;
			this.rbLocais.CheckedChanged += new System.EventHandler(this.filtroChanged);
			// 
			// rbGlobais
			// 
			this.rbGlobais.AutoSize = true;
			this.rbGlobais.Location = new System.Drawing.Point(291, 12);
			this.rbGlobais.Name = "rbGlobais";
			this.rbGlobais.Size = new System.Drawing.Size(60, 17);
			this.rbGlobais.TabIndex = 2;
			this.rbGlobais.Tag = "2";
			this.rbGlobais.Text = "Globais";
			this.rbGlobais.UseVisualStyleBackColor = true;
			this.rbGlobais.CheckedChanged += new System.EventHandler(this.filtroChanged);
			// 
			// DefinicaoDeConstantes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(469, 267);
			this.Controls.Add(this.gbFiltro);
			this.Controls.Add(this.cbEscopo);
			this.Controls.Add(this.btExcluir);
			this.Controls.Add(this.btIncluir);
			this.Controls.Add(this.txtValor);
			this.Controls.Add(this.txtNome);
			this.Controls.Add(this.dgConstantes);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DefinicaoDeConstantes";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Constantes";
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).EndInit();
			this.gbFiltro.ResumeLayout(false);
			this.gbFiltro.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgConstantes;
		private System.Windows.Forms.TextBox txtNome;
		private System.Windows.Forms.TextBox txtValor;
		private System.Windows.Forms.Button btIncluir;
		private System.Windows.Forms.Button btExcluir;
		private System.Windows.Forms.ComboBox cbEscopo;
		private System.Windows.Forms.GroupBox gbFiltro;
		private System.Windows.Forms.RadioButton rbTodas;
		private System.Windows.Forms.RadioButton rbTodasArquivo;
		private System.Windows.Forms.RadioButton rbLocais;
		private System.Windows.Forms.RadioButton rbGlobais;
		private System.Windows.Forms.RadioButton rbAtivas;
	}
}