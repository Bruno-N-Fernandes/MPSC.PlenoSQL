namespace MPSC.PlenoSQL.AppWin.View
{
	partial class Constantes
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
			this.Nome = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Valor = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.txtNome = new System.Windows.Forms.TextBox();
			this.txtValor = new System.Windows.Forms.TextBox();
			this.btIncluir = new System.Windows.Forms.Button();
			this.btExcluir = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgConstantes
			// 
			this.dgConstantes.AllowUserToOrderColumns = true;
			this.dgConstantes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgConstantes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nome,
            this.Valor});
			this.dgConstantes.Location = new System.Drawing.Point(-2, 3);
			this.dgConstantes.Name = "dgConstantes";
			this.dgConstantes.Size = new System.Drawing.Size(291, 212);
			this.dgConstantes.TabIndex = 0;
			// 
			// Nome
			// 
			this.Nome.Frozen = true;
			this.Nome.HeaderText = "Nome";
			this.Nome.Name = "Nome";
			// 
			// Valor
			// 
			this.Valor.Frozen = true;
			this.Valor.HeaderText = "Valor";
			this.Valor.Name = "Valor";
			// 
			// txtNome
			// 
			this.txtNome.Location = new System.Drawing.Point(-2, 222);
			this.txtNome.Name = "txtNome";
			this.txtNome.Size = new System.Drawing.Size(100, 20);
			this.txtNome.TabIndex = 1;
			// 
			// txtValor
			// 
			this.txtValor.Location = new System.Drawing.Point(119, 224);
			this.txtValor.Name = "txtValor";
			this.txtValor.Size = new System.Drawing.Size(100, 20);
			this.txtValor.TabIndex = 1;
			// 
			// btIncluir
			// 
			this.btIncluir.Location = new System.Drawing.Point(225, 222);
			this.btIncluir.Name = "btIncluir";
			this.btIncluir.Size = new System.Drawing.Size(55, 23);
			this.btIncluir.TabIndex = 2;
			this.btIncluir.Text = "Incluir";
			this.btIncluir.UseVisualStyleBackColor = true;
			// 
			// btExcluir
			// 
			this.btExcluir.Location = new System.Drawing.Point(286, 222);
			this.btExcluir.Name = "btExcluir";
			this.btExcluir.Size = new System.Drawing.Size(55, 23);
			this.btExcluir.TabIndex = 2;
			this.btExcluir.Text = "Excluir";
			this.btExcluir.UseVisualStyleBackColor = true;
			// 
			// Constantes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(343, 273);
			this.Controls.Add(this.btExcluir);
			this.Controls.Add(this.btIncluir);
			this.Controls.Add(this.txtValor);
			this.Controls.Add(this.txtNome);
			this.Controls.Add(this.dgConstantes);
			this.Name = "Constantes";
			this.Text = "Constantes";
			((System.ComponentModel.ISupportInitialize)(this.dgConstantes)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dgConstantes;
		private System.Windows.Forms.DataGridViewTextBoxColumn Nome;
		private System.Windows.Forms.DataGridViewTextBoxColumn Valor;
		private System.Windows.Forms.TextBox txtNome;
		private System.Windows.Forms.TextBox txtValor;
		private System.Windows.Forms.Button btIncluir;
		private System.Windows.Forms.Button btExcluir;
	}
}