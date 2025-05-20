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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBoard));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label_player2 = new System.Windows.Forms.Label();
            this.label_player1 = new System.Windows.Forms.Label();
            this.label_score = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.hraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nováHraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otevřeníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uloženíToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ukončitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.playingBoard1 = new Piskvorky.PlayingBoard();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.27273F));
            this.tableLayoutPanel1.Controls.Add(this.label_player2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_player1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_score, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 409);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 52);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label_hrac2
            // 
            this.label_player2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label_player2.AutoSize = true;
            this.label_player2.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_player2.Location = new System.Drawing.Point(683, 10);
            this.label_player2.Name = "label_hrac2";
            this.label_player2.Size = new System.Drawing.Size(98, 32);
            this.label_player2.TabIndex = 2;
            this.label_player2.Text = "Hráč 2";
            // 
            // label_hrac1
            // 
            this.label_player1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label_player1.AutoSize = true;
            this.label_player1.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_player1.Location = new System.Drawing.Point(3, 10);
            this.label_player1.Name = "label_hrac1";
            this.label_player1.Size = new System.Drawing.Size(98, 32);
            this.label_player1.TabIndex = 1;
            this.label_player1.Text = "Hráč 1";
            // 
            // label_score
            // 
            this.label_score.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_score.AutoSize = true;
            this.label_score.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_score.Location = new System.Drawing.Point(364, 10);
            this.label_score.Name = "label_score";
            this.label_score.Size = new System.Drawing.Size(54, 32);
            this.label_score.TabIndex = 4;
            this.label_score.Text = "0:0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hraToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 5, 0, 5);
            this.menuStrip1.Size = new System.Drawing.Size(784, 35);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // hraToolStripMenuItem
            // 
            this.hraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nováHraToolStripMenuItem,
            this.menuToolStripMenuItem,
            this.otevřeníToolStripMenuItem,
            this.uloženíToolStripMenuItem,
            this.ukončitToolStripMenuItem});
            this.hraToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.hraToolStripMenuItem.Name = "hraToolStripMenuItem";
            this.hraToolStripMenuItem.Size = new System.Drawing.Size(51, 25);
            this.hraToolStripMenuItem.Text = "Hra";
            // 
            // nováHraToolStripMenuItem
            // 
            this.nováHraToolStripMenuItem.Name = "nováHraToolStripMenuItem";
            this.nováHraToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.nováHraToolStripMenuItem.Text = "Nová hra";
            this.nováHraToolStripMenuItem.Click += new System.EventHandler(this.nováHraToolStripMenuItem_Click);
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.menuToolStripMenuItem.Text = "Menu";
            this.menuToolStripMenuItem.Click += new System.EventHandler(this.menuToolStripMenuItem_Click);
            // 
            // otevřeníToolStripMenuItem
            // 
            this.otevřeníToolStripMenuItem.Name = "otevřeníToolStripMenuItem";
            this.otevřeníToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.otevřeníToolStripMenuItem.Text = "Otevření";
            this.otevřeníToolStripMenuItem.Click += new System.EventHandler(this.otevřeníToolStripMenuItem_Click);
            // 
            // uloženíToolStripMenuItem
            // 
            this.uloženíToolStripMenuItem.Name = "uloženíToolStripMenuItem";
            this.uloženíToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.uloženíToolStripMenuItem.Text = "Uložení";
            this.uloženíToolStripMenuItem.Click += new System.EventHandler(this.uloženíToolStripMenuItem_Click);
            // 
            // ukončitToolStripMenuItem
            // 
            this.ukončitToolStripMenuItem.Name = "ukončitToolStripMenuItem";
            this.ukončitToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.ukončitToolStripMenuItem.Text = "Ukončit";
            this.ukončitToolStripMenuItem.Click += new System.EventHandler(this.ukončitToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // playingBoard1
            // 
            this.playingBoard1.AIDifficulty = null;
            this.playingBoard1.BackColor = System.Drawing.Color.Transparent;
            this.playingBoard1.BoardSize = 15;
            this.playingBoard1.CurrentPlayer = Piskvorky.GameSymbol.Symbol1;
            this.playingBoard1.FieldSize = 20;
            this.playingBoard1.GridColor = System.Drawing.Color.Black;
            this.playingBoard1.IsPlayingAI = false;
            this.playingBoard1.LastDrawnColor = System.Drawing.Color.Yellow;
            this.playingBoard1.Location = new System.Drawing.Point(237, 45);
            this.playingBoard1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.playingBoard1.MovesToWin = 0;
            this.playingBoard1.MovesToWinMin = 626;
            this.playingBoard1.Name = "playingBoard1";
            this.playingBoard1.Size = new System.Drawing.Size(305, 305);
            this.playingBoard1.Symbol1Color = System.Drawing.Color.Red;
            this.playingBoard1.Symbol1Emoji = "❌";
            this.playingBoard1.Symbol2Color = System.Drawing.Color.Blue;
            this.playingBoard1.Symbol2Emoji = "⭕";
            this.playingBoard1.TabIndex = 3;
            // 
            // FormBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.playingBoard1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "FormBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Piškvorky";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBoard_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormBoard_FormClosed);
            this.Load += new System.EventHandler(this.FormBoard_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_player2;
        private System.Windows.Forms.Label label_player1;
        private PlayingBoard playingBoard1;
        public System.Windows.Forms.Label label_score;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem hraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nováHraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otevřeníToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uloženíToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ukončitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

