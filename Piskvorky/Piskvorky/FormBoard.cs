using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class FormBoard : Form
    {
        int fieldSize;
        int width = 0,
            height  = 0;
        int gameLength;
        int gamesPlayed = 0;
        FormMenu formMenu;
        SoundPlayer winSound = new SoundPlayer(@"sound\win.wav");
        public FormBoard(FormMenu formMenu)
        {
            InitializeComponent();
            InitializeGameSettings();
            fieldSize = playingBoard1.FieldSize;
            this.formMenu = formMenu;
            playingBoard1.PlayerWon += OnPlayerWon;
            playingBoard1.Draw += OnDraw;
        }
        private const int ResizeThreshold = 5;

        private void InitializeGameSettings()
        {
            playingBoard1.Calc.WinLength = (short)GameSettings.WinLength;
            playingBoard1.BoardSize = GameSettings.BoardSize;
            playingBoard1.Calc.SetBoardSize(GameSettings.BoardSize);
            gameLength = GameSettings.GameLength;
            playingBoard1.Symbol1Emoji = GameSettings.Player1Symbol;
            playingBoard1.Symbol2Emoji = GameSettings.Player2Symbol;
            playingBoard1.IsPlayingAI = GameSettings.IsAgainstAI;
        }

        public void BoardRedraw()
        {
            if (Math.Abs(width - Width) > ResizeThreshold || Math.Abs(height - Height) > ResizeThreshold)
            {
                if ((float)Width / Height < 2f )
                {
                    int currentAvg = (Width + Height) / 2;
                    int minAvg = (this.MinimumSize.Width + this.MinimumSize.Height) / 2;
                    playingBoard1.FieldSize = (int)(fieldSize * (float)currentAvg / minAvg);

                    width = Width;
                    height = Height;
                }
            }
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {

        }

        private void FormBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            formMenu.Show();
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            BoardRedraw();
        }

        private void OnPlayerWon(GameSymbol winner)
        {
            string[] scores = label_score.Text.Split(':'); 
            double player1Score = int.Parse(scores[0]);
            double player2Score = int.Parse(scores[1]);
            gamesPlayed++;
            if (winner == GameSymbol.Symbol1)
            {
                winSound.Play();
                MessageBox.Show("Partii vyhrál Hráč 1!");
                player1Score++;
            }
            else if (winner == GameSymbol.Symbol2)
            {
                winSound.Play();
                MessageBox.Show("Partii vyhrál Hráč 2!");
                player2Score++;
            }

            label_score.Text = $"{player1Score}:{player2Score}";
            if (gamesPlayed == gameLength)
            {
                MessageBox.Show($"Konec hry!\nFinální skóre je {label_score.Text}");
                //Přidat skóre do tabulky nejlepších hráčů


                label_score.Text = "0:0";
            }
        }

        private void nováHraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            FormBoard formBoard = new FormBoard(formMenu);
            formBoard.Show();
        }

        private void ukončitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            formMenu.Show();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete jít do menu?", "Upozornění", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                formMenu.Show();
                Close();
            }

        }

        private void OnDraw()
        {
            MessageBox.Show("Na hrací ploše již nejsou žádné výhry, došlo k remíze!");

            string[] scores = label_score.Text.Split(':'); 
            double player1Score = double.Parse(scores[0]);
            double player2Score = double.Parse(scores[1]);

            
            player1Score += 0.5;
            player2Score += 0.5;

            label_score.Text = $"{player1Score}:{player2Score}"; 
        }
    }
}
