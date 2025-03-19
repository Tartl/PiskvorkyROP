namespace Piskvorky
{
    partial class FormMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMenu));
            this.buttonNewGame = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonBest = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.buttonDemo = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonNewGame
            // 
            this.buttonNewGame.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonNewGame.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonNewGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonNewGame.FlatAppearance.BorderSize = 0;
            this.buttonNewGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNewGame.Location = new System.Drawing.Point(224, 232);
            this.buttonNewGame.Name = "buttonNewGame";
            this.buttonNewGame.Size = new System.Drawing.Size(312, 35);
            this.buttonNewGame.TabIndex = 0;
            this.buttonNewGame.Text = "Nová hra";
            this.buttonNewGame.UseVisualStyleBackColor = false;
            this.buttonNewGame.Click += new System.EventHandler(this.buttonNewGame_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSettings.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSettings.FlatAppearance.BorderSize = 0;
            this.buttonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSettings.Location = new System.Drawing.Point(224, 273);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(312, 35);
            this.buttonSettings.TabIndex = 1;
            this.buttonSettings.Text = "Nastavení";
            this.buttonSettings.UseVisualStyleBackColor = false;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonBest
            // 
            this.buttonBest.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonBest.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonBest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBest.FlatAppearance.BorderSize = 0;
            this.buttonBest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBest.Location = new System.Drawing.Point(383, 314);
            this.buttonBest.Name = "buttonBest";
            this.buttonBest.Size = new System.Drawing.Size(153, 35);
            this.buttonBest.TabIndex = 2;
            this.buttonBest.Text = "Historie nejlepších";
            this.buttonBest.UseVisualStyleBackColor = false;
            this.buttonBest.Click += new System.EventHandler(this.buttonBest_Click);
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonQuit.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonQuit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonQuit.FlatAppearance.BorderSize = 0;
            this.buttonQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonQuit.Location = new System.Drawing.Point(224, 355);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(312, 35);
            this.buttonQuit.TabIndex = 4;
            this.buttonQuit.Text = "Ukončit";
            this.buttonQuit.UseVisualStyleBackColor = false;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // buttonDemo
            // 
            this.buttonDemo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonDemo.BackColor = System.Drawing.Color.LightSteelBlue;
            this.buttonDemo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDemo.FlatAppearance.BorderSize = 0;
            this.buttonDemo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDemo.Location = new System.Drawing.Point(224, 314);
            this.buttonDemo.Name = "buttonDemo";
            this.buttonDemo.Size = new System.Drawing.Size(153, 35);
            this.buttonDemo.TabIndex = 6;
            this.buttonDemo.Text = "Demo";
            this.buttonDemo.UseVisualStyleBackColor = false;
            this.buttonDemo.Click += new System.EventHandler(this.buttonDemo_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(60, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(658, 173);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 431);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonDemo);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonBest);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonNewGame);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 470);
            this.Name = "FormMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Piškvorky";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonNewGame;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonBest;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.Button buttonDemo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}