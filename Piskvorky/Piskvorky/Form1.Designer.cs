namespace Piskvorky
{
    partial class Form1
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
            this.label_hrac1 = new System.Windows.Forms.Label();
            this.label_hrac2 = new System.Windows.Forms.Label();
            this.playingBoard1 = new Piskvorky.PlayingBoard();
            this.SuspendLayout();
            // 
            // label_hrac1
            // 
            this.label_hrac1.AutoSize = true;
            this.label_hrac1.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hrac1.Location = new System.Drawing.Point(143, 356);
            this.label_hrac1.Name = "label_hrac1";
            this.label_hrac1.Size = new System.Drawing.Size(125, 37);
            this.label_hrac1.TabIndex = 1;
            this.label_hrac1.Text = "Hráč 1:";
            // 
            // label_hrac2
            // 
            this.label_hrac2.AutoSize = true;
            this.label_hrac2.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_hrac2.Location = new System.Drawing.Point(478, 356);
            this.label_hrac2.Name = "label_hrac2";
            this.label_hrac2.Size = new System.Drawing.Size(125, 37);
            this.label_hrac2.TabIndex = 2;
            this.label_hrac2.Text = "Hráč 2:";
            // 
            // playingBoard1
            // 
            this.playingBoard1.BackColor = System.Drawing.Color.Transparent;
            this.playingBoard1.BoardSize = 15;
            this.playingBoard1.FieldSize = 20;
            this.playingBoard1.GridColor = System.Drawing.SystemColors.Highlight;
            this.playingBoard1.Location = new System.Drawing.Point(212, 40);
            this.playingBoard1.Name = "playingBoard1";
            this.playingBoard1.Size = new System.Drawing.Size(314, 313);
            this.playingBoard1.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.playingBoard1);
            this.Controls.Add(this.label_hrac2);
            this.Controls.Add(this.label_hrac1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_hrac1;
        private System.Windows.Forms.Label label_hrac2;
        private PlayingBoard playingBoard1;
    }
}

