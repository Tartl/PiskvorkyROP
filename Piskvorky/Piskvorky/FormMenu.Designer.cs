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
            this.buttonMenu = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonBest = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonQuit = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonDemo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonMenu
            // 
            this.buttonMenu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonMenu.Location = new System.Drawing.Point(248, 144);
            this.buttonMenu.Name = "buttonMenu";
            this.buttonMenu.Size = new System.Drawing.Size(312, 35);
            this.buttonMenu.TabIndex = 0;
            this.buttonMenu.Text = "Nová hra";
            this.buttonMenu.UseVisualStyleBackColor = true;
            this.buttonMenu.Click += new System.EventHandler(this.buttonMenu_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSettings.Location = new System.Drawing.Point(248, 185);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(312, 35);
            this.buttonSettings.TabIndex = 1;
            this.buttonSettings.Text = "Nastavení";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // buttonBest
            // 
            this.buttonBest.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonBest.Location = new System.Drawing.Point(407, 267);
            this.buttonBest.Name = "buttonBest";
            this.buttonBest.Size = new System.Drawing.Size(153, 35);
            this.buttonBest.TabIndex = 2;
            this.buttonBest.Text = "Historie nejlepších";
            this.buttonBest.UseVisualStyleBackColor = true;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonLoad.Location = new System.Drawing.Point(407, 226);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(153, 35);
            this.buttonLoad.TabIndex = 3;
            this.buttonLoad.Text = "Otevření";
            this.buttonLoad.UseVisualStyleBackColor = true;
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonQuit.Location = new System.Drawing.Point(248, 308);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(312, 35);
            this.buttonQuit.TabIndex = 4;
            this.buttonQuit.Text = "Ukončit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSave.Location = new System.Drawing.Point(248, 226);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(153, 35);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Uložení";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // buttonDemo
            // 
            this.buttonDemo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonDemo.Location = new System.Drawing.Point(248, 267);
            this.buttonDemo.Name = "buttonDemo";
            this.buttonDemo.Size = new System.Drawing.Size(153, 35);
            this.buttonDemo.TabIndex = 6;
            this.buttonDemo.Text = "Demo";
            this.buttonDemo.UseVisualStyleBackColor = true;
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonDemo);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonQuit);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonBest);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonMenu);
            this.Name = "FormMenu";
            this.Text = "FormMenu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonMenu;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonBest;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonDemo;
    }
}