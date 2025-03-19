using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class FormNewGame : Form
    {
        public FormNewGame()
        {
            InitializeComponent();
        }

        private void FormNewGame_Load(object sender, EventArgs e)
        {
            if (GameSettings.IsAgainstAI)
            {
                textBox2.Enabled = false;
                textBox2.Text = "Počítač";
                checkBox1.Checked = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                GameSettings.IsAgainstAI = true;
            }
            else
            {
                GameSettings.IsAgainstAI = false;
            }

            if (GameSettings.IsAgainstAI)
            {
                textBox2.Enabled = false;
                textBox2.Text = "Počítač";
            }
            else
            {
                textBox2.Enabled = true;
                textBox2.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                GameSettings.Player1Name = "Hráč 1";
            else            
                GameSettings.Player1Name = textBox1.Text;            
            if (textBox2.Text == "")
                GameSettings.Player2Name = "Hráč 2";
            else
                GameSettings.Player2Name = textBox2.Text;

        }
    }
}
