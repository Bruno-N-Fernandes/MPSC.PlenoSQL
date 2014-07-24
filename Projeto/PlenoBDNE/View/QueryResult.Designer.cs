namespace LBJC.NavegadorDeDados
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
			this.txtQuery = new System.Windows.Forms.TextBox();
			this.btBinding = new System.Windows.Forms.Button();
			this.dgResult = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).BeginInit();
			this.scHorizontal.Panel1.SuspendLayout();
			this.scHorizontal.Panel2.SuspendLayout();
			this.scHorizontal.SuspendLayout();
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
			this.scHorizontal.Panel2.Controls.Add(this.btBinding);
			this.scHorizontal.Panel2.Controls.Add(this.dgResult);
			this.scHorizontal.Size = new System.Drawing.Size(400, 400);
			this.scHorizontal.SplitterDistance = 300;
			this.scHorizontal.TabIndex = 1;
			// 
			// txtQuery
			// 
			this.txtQuery.AcceptsReturn = true;
			this.txtQuery.AcceptsTab = true;
			this.txtQuery.AllowDrop = true;
			this.txtQuery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtQuery.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtQuery.Location = new System.Drawing.Point(0, 0);
			this.txtQuery.Multiline = true;
			this.txtQuery.Name = "txtQuery";
			this.txtQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtQuery.Size = new System.Drawing.Size(400, 300);
			this.txtQuery.TabIndex = 0;
			this.txtQuery.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtQuery_KeyDown);
			this.txtQuery.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtQuery_KeyUp);
			// 
			// btBinding
			// 
			this.btBinding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btBinding.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btBinding.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btBinding.Location = new System.Drawing.Point(383, 79);
			this.btBinding.Margin = new System.Windows.Forms.Padding(0);
			this.btBinding.Name = "btBinding";
			this.btBinding.Size = new System.Drawing.Size(17, 17);
			this.btBinding.TabIndex = 0;
			this.btBinding.Text = "+";
			this.btBinding.UseVisualStyleBackColor = false;
			this.btBinding.Click += new System.EventHandler(this.btBinding_Click);
			// 
			// dgResult
			// 
			this.dgResult.AllowUserToAddRows = false;
			this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgResult.Location = new System.Drawing.Point(0, 0);
			this.dgResult.MultiSelect = false;
			this.dgResult.Name = "dgResult";
			this.dgResult.ReadOnly = true;
			this.dgResult.Size = new System.Drawing.Size(400, 96);
			this.dgResult.TabIndex = 0;
			// 
			// QueryResult
			// 
			this.Controls.Add(this.scHorizontal);
			this.Size = new System.Drawing.Size(400, 400);
			this.scHorizontal.Panel1.ResumeLayout(false);
			this.scHorizontal.Panel1.PerformLayout();
			this.scHorizontal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scHorizontal)).EndInit();
			this.scHorizontal.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer scHorizontal;
		private System.Windows.Forms.TextBox txtQuery;
		private System.Windows.Forms.DataGridView dgResult;
		private System.Windows.Forms.Button btBinding;
	}
}
