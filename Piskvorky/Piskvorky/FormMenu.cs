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
    public partial class FormMenu : Form
    {
        public FormMenu()
        {
            InitializeComponent();
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            FormBoard board = new FormBoard(this);
            board.Show();
            Hide();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            FormSettings formSettings = new FormSettings();
            formSettings.ShowDialog();
            if (formSettings.DialogResult == DialogResult.OK)
            {
                
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
