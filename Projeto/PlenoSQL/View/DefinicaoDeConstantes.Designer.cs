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
			this.ckGlobalOnly = new System.Windows.Forms.CheckBox();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgConstantes
			// 
			this.dgConstantes.AllowUserToOrderColumns = true;
			this.dgConstantes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgConstantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgConstantes.Location = new System.Drawing.Point(3, 20);
			this.dgConstantes.Name = "dgConstantes";
			this.dgConstantes.Size = new System.Drawing.Size(465, 212);
			this.dgConstantes.TabIndex = 0;
			// 
			// txtNome
			// 
			this.txtNome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtNome.Location = new System.Drawing.Point(3, 241);
			this.txtNome.Name = "txtNome";
			this.txtNome.Size = new System.Drawing.Size(100, 20);
			this.txtNome.TabIndex = 1;
			// 
			// txtValor
			// 
			this.txtValor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtValor.Location = new System.Drawing.Point(119, 240);
			this.txtValor.Name = "txtValor";
			this.txtValor.Size = new System.Drawing.Size(100, 20);
			this.txtValor.TabIndex = 1;
			// 
			// btIncluir
			// 
			this.btIncluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btIncluir.Location = new System.Drawing.Point(352, 238);
			this.btIncluir.Name = "btIncluir";
			this.btIncluir.Size = new System.Drawing.Size(55, 23);
			this.btIncluir.TabIndex = 2;
			this.btIncluir.Text = "Incluir";
			this.btIncluir.UseVisualStyleBackColor = true;
			this.btIncluir.Click += new System.EventHandler(this.btIncluir_Click);
			// 
			// btExcluir
			// 
			this.btExcluir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btExcluir.Location = new System.Drawing.Point(413, 238);
			this.btExcluir.Name = "btExcluir";
			this.btExcluir.Size = new System.Drawing.Size(55, 23);
			this.btExcluir.TabIndex = 2;
			this.btExcluir.Text = "Excluir";
			this.btExcluir.UseVisualStyleBackColor = true;
			this.btExcluir.Click += new System.EventHandler(this.btExcluir_Click);
			// 
			// cbEscopo
			// 
			this.cbEscopo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cbEscopo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbEscopo.FormattingEnabled = true;
			this.cbEscopo.Location = new System.Drawing.Point(225, 240);
			this.cbEscopo.Name = "cbEscopo";
			this.cbEscopo.Size = new System.Drawing.Size(121, 21);
			this.cbEscopo.TabIndex = 3;
			// 
			// ckGlobalOnly
			// 
			this.ckGlobalOnly.AutoSize = true;
			this.ckGlobalOnly.Location = new System.Drawing.Point(3, 0);
			this.ckGlobalOnly.Name = "ckGlobalOnly";
			this.ckGlobalOnly.Size = new System.Drawing.Size(100, 17);
			this.ckGlobalOnly.TabIndex = 4;
			this.ckGlobalOnly.Text = "Apenas Globais";
			this.ckGlobalOnly.UseVisualStyleBackColor = true;
			this.ckGlobalOnly.CheckedChanged += new System.EventHandler(this.ckLocalOnly_CheckedChanged);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(119, 0);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(85, 17);
			this.radioButton1.TabIndex = 5;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "radioButton1";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// DefinicaoDeConstantes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(470, 267);
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.cbEscopo);
			this.Controls.Add(this.btExcluir);
			this.Controls.Add(this.btIncluir);
			this.Controls.Add(this.txtValor);
			this.Controls.Add(this.txtNome);
			this.Controls.Add(this.dgConstantes);
			this.Controls.Add(this.ckGlobalOnly);
			this.Name = "DefinicaoDeConstantes";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Constantes";
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).EndInit();
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
		private System.Windows.Forms.CheckBox ckGlobalOnly;
		private System.Windows.Forms.RadioButton radioButton1;
	}
}