namespace MP.PlenoBDNE.AppWin.View
{
	partial class Autenticacao
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
			this.txtServidor = new System.Windows.Forms.TextBox();
			this.txtUsuario = new System.Windows.Forms.TextBox();
			this.txtSenha = new System.Windows.Forms.TextBox();
			this.cbTipoBanco = new System.Windows.Forms.ComboBox();
			this.cbBancoSchema = new System.Windows.Forms.ComboBox();
			this.blTipoBanco = new System.Windows.Forms.Label();
			this.lbServidor = new System.Windows.Forms.Label();
			this.lbUsuario = new System.Windows.Forms.Label();
			this.lbSenha = new System.Windows.Forms.Label();
			this.lbBanco = new System.Windows.Forms.Label();
			this.btConectar = new System.Windows.Forms.Button();
			this.btCancelar = new System.Windows.Forms.Button();
			this.ckSalvarSenha = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// txtServidor
			// 
			this.txtServidor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServidor.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtServidor.Location = new System.Drawing.Point(100, 37);
			this.txtServidor.Name = "txtServidor";
			this.txtServidor.Size = new System.Drawing.Size(330, 21);
			this.txtServidor.TabIndex = 3;
			this.txtServidor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtServidor_KeyUp);
			// 
			// txtUsuario
			// 
			this.txtUsuario.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtUsuario.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtUsuario.Location = new System.Drawing.Point(100, 67);
			this.txtUsuario.Name = "txtUsuario";
			this.txtUsuario.Size = new System.Drawing.Size(330, 21);
			this.txtUsuario.TabIndex = 5;
			this.txtUsuario.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtUsuario_KeyUp);
			// 
			// txtSenha
			// 
			this.txtSenha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtSenha.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSenha.Location = new System.Drawing.Point(100, 97);
			this.txtSenha.Name = "txtSenha";
			this.txtSenha.PasswordChar = '*';
			this.txtSenha.Size = new System.Drawing.Size(330, 22);
			this.txtSenha.TabIndex = 7;
			// 
			// cbTipoBanco
			// 
			this.cbTipoBanco.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbTipoBanco.DisplayMember = "Key";
			this.cbTipoBanco.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTipoBanco.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbTipoBanco.FormattingEnabled = true;
			this.cbTipoBanco.Location = new System.Drawing.Point(100, 7);
			this.cbTipoBanco.Name = "cbTipoBanco";
			this.cbTipoBanco.Size = new System.Drawing.Size(330, 23);
			this.cbTipoBanco.TabIndex = 1;
			this.cbTipoBanco.ValueMember = "Value";
			this.cbTipoBanco.SelectedIndexChanged += new System.EventHandler(this.cbTipoBanco_SelectedIndexChanged);
			// 
			// cbBancoSchema
			// 
			this.cbBancoSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbBancoSchema.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbBancoSchema.FormattingEnabled = true;
			this.cbBancoSchema.Location = new System.Drawing.Point(100, 127);
			this.cbBancoSchema.Name = "cbBancoSchema";
			this.cbBancoSchema.Size = new System.Drawing.Size(330, 23);
			this.cbBancoSchema.TabIndex = 9;
			this.cbBancoSchema.DropDown += new System.EventHandler(this.cbBancoSchema_DropDown);
			// 
			// blTipoBanco
			// 
			this.blTipoBanco.AutoSize = true;
			this.blTipoBanco.Location = new System.Drawing.Point(10, 10);
			this.blTipoBanco.Name = "blTipoBanco";
			this.blTipoBanco.Size = new System.Drawing.Size(77, 13);
			this.blTipoBanco.TabIndex = 0;
			this.blTipoBanco.Text = "Tipo de Banco";
			// 
			// lbServidor
			// 
			this.lbServidor.AutoSize = true;
			this.lbServidor.Location = new System.Drawing.Point(10, 40);
			this.lbServidor.Name = "lbServidor";
			this.lbServidor.Size = new System.Drawing.Size(46, 13);
			this.lbServidor.TabIndex = 2;
			this.lbServidor.Text = "Servidor";
			// 
			// lbUsuario
			// 
			this.lbUsuario.AutoSize = true;
			this.lbUsuario.Location = new System.Drawing.Point(10, 70);
			this.lbUsuario.Name = "lbUsuario";
			this.lbUsuario.Size = new System.Drawing.Size(43, 13);
			this.lbUsuario.TabIndex = 4;
			this.lbUsuario.Text = "Usuário";
			// 
			// lbSenha
			// 
			this.lbSenha.AutoSize = true;
			this.lbSenha.Location = new System.Drawing.Point(10, 100);
			this.lbSenha.Name = "lbSenha";
			this.lbSenha.Size = new System.Drawing.Size(38, 13);
			this.lbSenha.TabIndex = 6;
			this.lbSenha.Text = "Senha";
			// 
			// lbBanco
			// 
			this.lbBanco.AutoSize = true;
			this.lbBanco.Location = new System.Drawing.Point(10, 130);
			this.lbBanco.Name = "lbBanco";
			this.lbBanco.Size = new System.Drawing.Size(88, 13);
			this.lbBanco.TabIndex = 8;
			this.lbBanco.Text = "Banco / Schema";
			// 
			// btConectar
			// 
			this.btConectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btConectar.Location = new System.Drawing.Point(270, 160);
			this.btConectar.Name = "btConectar";
			this.btConectar.Size = new System.Drawing.Size(75, 23);
			this.btConectar.TabIndex = 11;
			this.btConectar.Text = "Conectar";
			this.btConectar.Click += new System.EventHandler(this.btConectar_Click);
			// 
			// btCancelar
			// 
			this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancelar.Location = new System.Drawing.Point(355, 160);
			this.btCancelar.Name = "btCancelar";
			this.btCancelar.Size = new System.Drawing.Size(75, 23);
			this.btCancelar.TabIndex = 12;
			this.btCancelar.Text = "Cancelar";
			// 
			// ckSalvarSenha
			// 
			this.ckSalvarSenha.AutoSize = true;
			this.ckSalvarSenha.Location = new System.Drawing.Point(10, 160);
			this.ckSalvarSenha.Name = "ckSalvarSenha";
			this.ckSalvarSenha.Size = new System.Drawing.Size(90, 17);
			this.ckSalvarSenha.TabIndex = 10;
			this.ckSalvarSenha.Text = "Salvar Senha";
			this.ckSalvarSenha.UseVisualStyleBackColor = true;
			// 
			// Autenticacao
			// 
			this.AcceptButton = this.btConectar;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btCancelar;
			this.ClientSize = new System.Drawing.Size(442, 193);
			this.ControlBox = false;
			this.Controls.Add(this.ckSalvarSenha);
			this.Controls.Add(this.btCancelar);
			this.Controls.Add(this.btConectar);
			this.Controls.Add(this.lbBanco);
			this.Controls.Add(this.lbSenha);
			this.Controls.Add(this.lbUsuario);
			this.Controls.Add(this.lbServidor);
			this.Controls.Add(this.blTipoBanco);
			this.Controls.Add(this.cbBancoSchema);
			this.Controls.Add(this.cbTipoBanco);
			this.Controls.Add(this.txtSenha);
			this.Controls.Add(this.txtUsuario);
			this.Controls.Add(this.txtServidor);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Autenticacao";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Autenticação";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Autenticacao_FormClosed);
			this.Load += new System.EventHandler(this.Autenticacao_Load);
			this.Shown += new System.EventHandler(this.Autenticacao_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtServidor;
		private System.Windows.Forms.TextBox txtUsuario;
		private System.Windows.Forms.TextBox txtSenha;
		private System.Windows.Forms.ComboBox cbTipoBanco;
		private System.Windows.Forms.ComboBox cbBancoSchema;
		private System.Windows.Forms.Label blTipoBanco;
		private System.Windows.Forms.Label lbServidor;
		private System.Windows.Forms.Label lbUsuario;
		private System.Windows.Forms.Label lbSenha;
		private System.Windows.Forms.Label lbBanco;
		private System.Windows.Forms.Button btConectar;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.CheckBox ckSalvarSenha;
	}
}