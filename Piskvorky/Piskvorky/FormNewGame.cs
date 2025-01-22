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
            }
        }

    }
}
