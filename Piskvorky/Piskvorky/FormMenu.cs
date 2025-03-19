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
            FormNewGame formNewGame = new FormNewGame();
            formNewGame.ShowDialog();
            if (formNewGame.DialogResult == DialogResult.OK)
            {
                GameSettings.Player1Name = formNewGame.textBox1.Text;
                GameSettings.Player2Name = formNewGame.textBox2.Text;
                FormBoard board = new FormBoard(this);
                board.Show();
                Hide();
            }
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
            formSettings.AIDifficulty = GameSettings.AI_Difficulty;
            formSettings.Player1Color = GameSettings.Player1Color;
            formSettings.Player2Color = GameSettings.Player2Color;
            formSettings.ShowDialog();
            if (formSettings.DialogResult == DialogResult.OK)
            {
                GameSettings.WinLength = formSettings.WinLength;
                GameSettings.BoardSize = formSettings.BoardSize;
                GameSettings.GameLength = formSettings.GameLength;
                GameSettings.Player1Symbol = formSettings.Player1Symbol;
                GameSettings.Player2Symbol = formSettings.Player2Symbol;
                GameSettings.IsAgainstAI = formSettings.IsWithAI;
                GameSettings.AI_Difficulty = formSettings.AIDifficulty;
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonBest_Click(object sender, EventArgs e)
        {
            FormHistoryOfBest formHistoryOfBest = new FormHistoryOfBest();
            formHistoryOfBest.ShowDialog();
        }

        private void buttonDemo_Click(object sender, EventArgs e)
        {
            GameSettings.DemoMode = true;
            FormBoard board = new FormBoard(this);
            board.Show();
            Hide();
        }

        private void buttonLoadGame_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Piskvorky save files (*.txt)|*.txt";
        }

    }
}
