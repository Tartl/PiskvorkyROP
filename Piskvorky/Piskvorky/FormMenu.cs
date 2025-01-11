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
            formSettings.BoardSize = GameSettings.BoardSize;
            formSettings.WinLength = GameSettings.WinLength;
            formSettings.GameLength = GameSettings.GameLength;
            formSettings.Player1Symbol = GameSettings.Player1Symbol;
            formSettings.Player2Symbol = GameSettings.Player2Symbol;
            formSettings.IsWithAI = GameSettings.IsAgainstAI;
            formSettings.ShowDialog();
            if (formSettings.DialogResult == DialogResult.OK)
            {
                GameSettings.WinLength = formSettings.WinLength;
                GameSettings.BoardSize = formSettings.BoardSize;
                GameSettings.GameLength = formSettings.GameLength;
                GameSettings.Player1Symbol = formSettings.Player1Symbol;
                GameSettings.Player2Symbol = formSettings.Player2Symbol;
                GameSettings.IsAgainstAI = formSettings.IsWithAI;
                GameSettings.AI_Difficulty = formSettings.AIDifficulty();
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
