namespace Piskvorky
{
    partial class FormBoard
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label_hrac2 = new System.Windows.Forms.Label();
            this.label_hrac1 = new System.Windows.Forms.Label();
            this.label_score = new System.Windows.Forms.Label();
            this.buttonMenu = new System.Windows.Forms.Button();
            this.playingBoard1 = new Piskvorky.PlayingBoard();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.label_hrac2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label_hrac1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.playingBoard1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_score, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonMenu, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.81236F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.18764F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(760, 437);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label_hrac2
            // 
            this.label_hrac2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_hrac2.AutoSize = true;
            this.label_hrac2.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hrac2.Location = new System.Drawing.Point(642, 387);
            this.label_hrac2.Name = "label_hrac2";
            this.label_hrac2.Size = new System.Drawing.Size(115, 37);
            this.label_hrac2.TabIndex = 2;
            this.label_hrac2.Text = "Hráč 2";
            // 
            // label_hrac1
            // 
            this.label_hrac1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_hrac1.AutoSize = true;
            this.label_hrac1.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hrac1.Location = new System.Drawing.Point(3, 387);
            this.label_hrac1.Name = "label_hrac1";
            this.label_hrac1.Size = new System.Drawing.Size(115, 37);
            this.label_hrac1.TabIndex = 1;
            this.label_hrac1.Text = "Hráč 1";
            // 
            // label_score
            // 
            this.label_score.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_score.AutoSize = true;
            this.label_score.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_score.Location = new System.Drawing.Point(348, 387);
            this.label_score.Name = "label_score";
            this.label_score.Size = new System.Drawing.Size(63, 37);
            this.label_score.TabIndex = 4;
            this.label_score.Text = "0:0";
            // 
            // buttonMenu
            // 
            this.buttonMenu.Location = new System.Drawing.Point(3, 3);
            this.buttonMenu.Name = "buttonMenu";
            this.buttonMenu.Size = new System.Drawing.Size(115, 35);
            this.buttonMenu.TabIndex = 5;
            this.buttonMenu.Text = "Menu";
            this.buttonMenu.UseVisualStyleBackColor = true;
            this.buttonMenu.Click += new System.EventHandler(this.buttonMenu_Click);
            // 
            // playingBoard1
            // 
            this.playingBoard1.BackColor = System.Drawing.Color.Transparent;
            this.playingBoard1.BoardSize = 15;
            this.playingBoard1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playingBoard1.FieldSize = 20;
            this.playingBoard1.GridColor = System.Drawing.Color.Black;
            this.playingBoard1.Location = new System.Drawing.Point(231, 3);
            this.playingBoard1.MinimumSize = new System.Drawing.Size(301, 301);
            this.playingBoard1.Name = "playingBoard1";
            this.playingBoard1.Size = new System.Drawing.Size(301, 369);
            this.playingBoard1.Symbol1Color = System.Drawing.Color.Red;
            this.playingBoard1.Symbol2Color = System.Drawing.Color.Blue;
            this.playingBoard1.TabIndex = 3;
            // 
            // FormBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormBoard";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormBoard_FormClosed);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_hrac2;
        private System.Windows.Forms.Label label_hrac1;
        private PlayingBoard playingBoard1;
        public System.Windows.Forms.Label label_score;
        private System.Windows.Forms.Button buttonMenu;
    }
}

