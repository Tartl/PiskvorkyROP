namespace Piskvorky
{
    partial class FormHistoryOfBest
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
            this.leaderboardGridView = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.leaderboardGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // leaderboardGridView
            // 
            this.leaderboardGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.leaderboardGridView.Location = new System.Drawing.Point(12, 12);
            this.leaderboardGridView.Name = "leaderboardGridView";
            this.leaderboardGridView.Size = new System.Drawing.Size(776, 368);
            this.leaderboardGridView.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(649, 396);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 42);
            this.button1.TabIndex = 1;
            this.button1.Text = "Zavřít";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormHistoryOfBest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.leaderboardGridView);
            this.Name = "FormHistoryOfBest";
            this.Text = "FormHistoryOfBest";
            ((System.ComponentModel.ISupportInitialize)(this.leaderboardGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView leaderboardGridView;
        private System.Windows.Forms.Button button1;
    }
}