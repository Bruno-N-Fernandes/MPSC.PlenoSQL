namespace MP.PlenoBDNE.AppWin.View
{
	partial class QueryResult
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.scHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtQuery = new MPSC.LanguageEditor.LanguageEditor();
			this.tcResultados = new System.Windows.Forms.TabControl();
			this.tpMensagens = new System.Windows.Forms.TabPage();
			this.txtMensagens = new System.Windows.Forms.TextBox();
			this.tpDados = new System.Windows.Forms.TabPage();
			this.btBinding = new System.Windows.Forms.Button();
			this.dgResult = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).BeginInit();
			this.scHorizontal.Panel1.SuspendLayout();
			this.scHorizontal.Panel2.SuspendLayout();
			this.scHorizontal.SuspendLayout();
			this.tcResultados.SuspendLayout();
			this.tpMensagens.SuspendLayout();
			this.tpDados.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
			this.SuspendLayout();
			// 
			// scHorizontal
			// 
			this.scHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scHorizontal.Location = new System.Drawing.Point(0, 0);
			this.scHorizontal.Name = "scHorizontal";
			this.scHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scHorizontal.Panel1
			// 
			this.scHorizontal.Panel1.Controls.Add(this.txtQuery);
			// 
			// scHorizontal.Panel2
			// 
			this.scHorizontal.Panel2.Controls.Add(this.tcResultados);
			this.scHorizontal.Size = new System.Drawing.Size(400, 400);
			this.scHorizontal.SplitterDistance = 300;
			this.scHorizontal.TabIndex = 1;
			// 
			// txtQuery
			// 
			this.txtQuery.AcceptsTab = true;
			this.txtQuery.AllowDrop = true;
			this.txtQuery.AutoWordSelection = true;
			this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtQuery.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtQuery.Location = new System.Drawing.Point(0, 0);
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Size = new System.Drawing.Size(400, 300);
			this.txtQuery.TabIndex = 0;
			this.txtQuery.Text = "";
			this.txtQuery.WordWrap = false;
			this.txtQuery.TextChanged += new System.EventHandler(this.txtQuery_TextChanged);
			this.txtQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuery_KeyDown);
			// 
			// tcResultados
			// 
			this.tcResultados.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.tcResultados.Controls.Add(this.tpMensagens);
			this.tcResultados.Controls.Add(this.tpDados);
			this.tcResultados.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcResultados.Location = new System.Drawing.Point(0, 0);
			this.tcResultados.Margin = new System.Windows.Forms.Padding(0);
			this.tcResultados.Name = "tcResultados";
			this.tcResultados.SelectedIndex = 0;
			this.tcResultados.Size = new System.Drawing.Size(400, 96);
			this.tcResultados.TabIndex = 1;
			// 
			// tpMensagens
			// 
			this.tpMensagens.Controls.Add(this.txtMensagens);
			this.tpMensagens.Location = new System.Drawing.Point(4, 4);
			this.tpMensagens.Margin = new System.Windows.Forms.Padding(0);
			this.tpMensagens.Name = "tpMensagens";
			this.tpMensagens.Size = new System.Drawing.Size(392, 70);
			this.tpMensagens.TabIndex = 0;
			this.tpMensagens.Text = "Mensagens";
			this.tpMensagens.UseVisualStyleBackColor = true;
			// 
			// txtMensagens
			// 
			this.txtMensagens.AcceptsReturn = true;
			this.txtMensagens.AcceptsTab = true;
			this.txtMensagens.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtMensagens.Location = new System.Drawing.Point(0, 0);
			this.txtMensagens.Margin = new System.Windows.Forms.Padding(0);
			this.txtMensagens.Multiline = true;
			this.txtMensagens.Name = "txtMensagens";
			this.txtMensagens.ReadOnly = true;
			this.txtMensagens.Size = new System.Drawing.Size(392, 70);
			this.txtMensagens.TabIndex = 0;
			this.txtMensagens.WordWrap = false;
			// 
			// tpDados
			// 
			this.tpDados.Controls.Add(this.btBinding);
			this.tpDados.Controls.Add(this.dgResult);
			this.tpDados.Location = new System.Drawing.Point(4, 4);
			this.tpDados.Margin = new System.Windows.Forms.Padding(0);
			this.tpDados.Name = "tpDados";
			this.tpDados.Size = new System.Drawing.Size(392, 70);
			this.tpDados.TabIndex = 1;
			this.tpDados.Text = "Dados";
			this.tpDados.UseVisualStyleBackColor = true;
			// 
			// btBinding
			// 
			this.btBinding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btBinding.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btBinding.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btBinding.Location = new System.Drawing.Point(375, 53);
			this.btBinding.Margin = new System.Windows.Forms.Padding(0);
			this.btBinding.Name = "btBinding";
			this.btBinding.Size = new System.Drawing.Size(17, 17);
			this.btBinding.TabIndex = 3;
			this.btBinding.Text = "+";
			this.btBinding.UseVisualStyleBackColor = false;
			this.btBinding.Click += new System.EventHandler(this.btBinding_Click);
			// 
			// dgResult
			// 
			this.dgResult.AllowUserToAddRows = false;
			this.dgResult.AllowUserToDeleteRows = false;
			this.dgResult.AllowUserToOrderColumns = true;
			this.dgResult.AllowUserToResizeRows = false;
			this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgResult.Location = new System.Drawing.Point(0, 0);
			this.dgResult.Margin = new System.Windows.Forms.Padding(0);
			this.dgResult.Name = "dgResult";
			this.dgResult.ReadOnly = true;
			this.dgResult.Size = new System.Drawing.Size(392, 70);
			this.dgResult.TabIndex = 2;
			// 
			// QueryResult
			// 
			this.Controls.Add(this.scHorizontal);
			this.Size = new System.Drawing.Size(400, 400);
			this.scHorizontal.Panel1.ResumeLayout(false);
			this.scHorizontal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).EndInit();
			this.scHorizontal.ResumeLayout(false);
			this.tcResultados.ResumeLayout(false);
			this.tpMensagens.ResumeLayout(false);
			this.tpMensagens.PerformLayout();
			this.tpDados.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scHorizontal;
		private MPSC.LanguageEditor.LanguageEditor txtQuery;
		private System.Windows.Forms.TabControl tcResultados;
		private System.Windows.Forms.TabPage tpMensagens;
		private System.Windows.Forms.TabPage tpDados;
		private System.Windows.Forms.DataGridView dgResult;
		private System.Windows.Forms.TextBox txtMensagens;
		private System.Windows.Forms.Button btBinding;
	}
}
