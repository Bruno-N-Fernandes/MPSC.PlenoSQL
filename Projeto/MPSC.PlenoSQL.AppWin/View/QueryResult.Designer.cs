﻿namespace MPSC.PlenoSQL.AppWin.View
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
			this.components = new System.ComponentModel.Container();
			this.scHorizontal = new System.Windows.Forms.SplitContainer();
			this.txtQuery = new FastColoredTextBoxNS.FastColoredTextBox();
			this.tcResultados = new System.Windows.Forms.TabControl();
			this.tpMensagens = new System.Windows.Forms.TabPage();
			this.txtMensagens = new System.Windows.Forms.TextBox();
			this.tpDados = new System.Windows.Forms.TabPage();
			this.dgResult = new MPSC.PlenoSQL.AppWin.View.DataGridViewOnDemand();
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).BeginInit();
			this.scHorizontal.Panel1.SuspendLayout();
			this.scHorizontal.Panel2.SuspendLayout();
			this.scHorizontal.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).BeginInit();
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
			this.scHorizontal.Size = new System.Drawing.Size(425, 361);
			this.scHorizontal.SplitterDistance = 270;
			this.scHorizontal.TabIndex = 1;
			this.scHorizontal.TabStop = false;
			// 
			// txtQuery
			// 
			this.txtQuery.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
			this.txtQuery.AutoIndent = false;
			this.txtQuery.AutoIndentCharsPatterns = "";
			this.txtQuery.AutoScrollMinSize = new System.Drawing.Size(2, 16);
			this.txtQuery.BackBrush = null;
			this.txtQuery.CharHeight = 16;
			this.txtQuery.CharWidth = 9;
			this.txtQuery.CommentPrefix = "--";
			this.txtQuery.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtQuery.DelayedTextChangedInterval = 100;
			this.txtQuery.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtQuery.Font = new System.Drawing.Font("Courier New", 11.25F);
			this.txtQuery.IsReplaceMode = false;
			this.txtQuery.Language = FastColoredTextBoxNS.Language.SQL;
			this.txtQuery.LeftBracket = '(';
			this.txtQuery.Location = new System.Drawing.Point(0, 0);
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.Paddings = new System.Windows.Forms.Padding(0);
			this.txtQuery.RightBracket = ')';
			this.txtQuery.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.txtQuery.Size = new System.Drawing.Size(425, 270);
			this.txtQuery.TabIndex = 2;
			this.txtQuery.Zoom = 100;
			this.txtQuery.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.txtQuery_TextChanged);
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
			this.tcResultados.Size = new System.Drawing.Size(425, 87);
			this.tcResultados.TabIndex = 1;
			// 
			// tpMensagens
			// 
			this.tpMensagens.Controls.Add(this.txtMensagens);
			this.tpMensagens.Location = new System.Drawing.Point(4, 4);
			this.tpMensagens.Margin = new System.Windows.Forms.Padding(0);
			this.tpMensagens.Name = "tpMensagens";
			this.tpMensagens.Size = new System.Drawing.Size(417, 61);
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
			this.txtMensagens.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMensagens.Size = new System.Drawing.Size(417, 61);
			this.txtMensagens.TabIndex = 0;
			this.txtMensagens.WordWrap = false;
			this.txtMensagens.DoubleClick += new System.EventHandler(this.txtMensagens_LimparLog);
			// 
			// tpDados
			// 
			this.tpDados.Controls.Add(this.dgResult);
			this.tpDados.Location = new System.Drawing.Point(4, 4);
			this.tpDados.Margin = new System.Windows.Forms.Padding(0);
			this.tpDados.Name = "tpDados";
			this.tpDados.Size = new System.Drawing.Size(417, 61);
			this.tpDados.TabIndex = 1;
			this.tpDados.Text = "Dados";
			this.tpDados.UseVisualStyleBackColor = true;
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
			this.dgResult.Size = new System.Drawing.Size(417, 61);
			this.dgResult.TabIndex = 2;
			this.dgResult.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgResult_DataError);
			this.dgResult.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dgResult_PreviewKeyDown);
			// 
			// QueryResult
			// 
			this.Controls.Add(this.scHorizontal);
			this.Size = new System.Drawing.Size(425, 361);
			this.scHorizontal.Panel1.ResumeLayout(false);
			this.scHorizontal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).EndInit();
			this.scHorizontal.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.txtQuery)).EndInit();
			this.tcResultados.ResumeLayout(false);
			this.tpMensagens.ResumeLayout(false);
			this.tpMensagens.PerformLayout();
			this.tpDados.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scHorizontal;
		private System.Windows.Forms.TabControl tcResultados;
		private System.Windows.Forms.TabPage tpMensagens;
		private System.Windows.Forms.TabPage tpDados;
		private DataGridViewOnDemand dgResult;
		private System.Windows.Forms.TextBox txtMensagens;
        private FastColoredTextBoxNS.FastColoredTextBox txtQuery;
	}
}