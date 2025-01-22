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
using System.IO;
using System.Xml.Serialization;

namespace Piskvorky
{
    public partial class FormBoard : Form
    {
        int fieldSize;
        int width = 0,
            height  = 0;
        int gameLength;
        int gamesPlayed = 0;
        string player1_name = "Hráč 1";
        string player2_name = "Hráč 2";

        private readonly string leaderboardFilePath = Path.Combine(Application.StartupPath, "leaderboard.xml");
        int player_score = 0;
        int player_wins = 0;
        int player_losses = 0;
        int player_draws = 0;
        int player_bestWinMoves = 0;
        double player_winPercentage = 0;

        FormMenu formMenu;
        SoundPlayer winSound = new SoundPlayer(@"sound\win.wav");
        private List<BestOfLeaderboard> leaderboard = new List<BestOfLeaderboard>();

        public FormBoard(FormMenu formMenu)
        {
            InitializeComponent();
            InitializeGameSettings();
            fieldSize = playingBoard1.FieldSize;
            this.formMenu = formMenu;
            playingBoard1.PlayerWon += OnPlayerWon;
            playingBoard1.Draw += OnDraw;
            player1_name = GameSettings.Player1Name;
            player2_name = GameSettings.Player2Name;
            label_hrac1.Text = player1_name;
            label_hrac2.Text = player2_name;
            
            LoadLeaderboard();
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
            playingBoard1.AIDifficulty = GameSettings.AI_Difficulty;
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

        private void FormBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLeaderboard();
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
            if (GameSettings.IsAgainstAI)
            {
                if (winner == GameSymbol.Symbol1)
                {
                    winSound.Play();
                    MessageBox.Show($"Partii vyhrál {player1_name}!");
                    player1Score++;
                    player_wins++;
                }
                else
                {
                    winSound.Play();
                    MessageBox.Show("Partii vyhrál Počítač!");
                    player2Score++;
                    player_losses++;
                }
            }
            else
            {
                if (winner == GameSymbol.Symbol1)
                {
                    winSound.Play();
                    MessageBox.Show($"Partii vyhrál {player1_name}!");
                    player1Score++;
                }
                else
                {
                    winSound.Play();
                    MessageBox.Show($"Partii vyhrál {player2_name}");
                    player2Score++;
                }
            }
            label_score.Text = $"{player1Score}:{player2Score}";
            if (gamesPlayed == gameLength)
            {
                MessageBox.Show($"Konec hry!\nFinální skóre je {label_score.Text}");
                //Přidat skóre do tabulky nejlepších hráčů
                if ((player1Score > player2Score) && GameSettings.IsAgainstAI)
                {
                    player_score = (int)player1Score * 100 - player_losses * 50;
                    player_winPercentage = (double)player_wins / gamesPlayed * 100;
                    AddToLeaderboard(player1_name, player_score, player_wins, player_losses, player_draws, player_bestWinMoves, player_winPercentage);
                }

                label_score.Text = "0:0";
            }
            
        }

        private void OnDraw()
        {
            MessageBox.Show("Na hrací ploše již nejsou žádné výhry, došlo k remíze!");

            string[] scores = label_score.Text.Split(':'); 
            double player1Score = double.Parse(scores[0]);
            double player2Score = double.Parse(scores[1]);

            player_draws++;

            player1Score += 0.5;
            player2Score += 0.5;

            label_score.Text = $"{player1Score}:{player2Score}"; 
        }

        private void nováHraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playingBoard1.ResetGame();
            label_score.Text = "0:0";
            MessageBox.Show($"Hra byla zresetována!");
        }

        private void ukončitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (MessageBox.Show("Opravdu chcete ukončit aplikaci?", "Upozornění", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete jít do menu?", "Upozornění", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                formMenu.Show();
                Close();
            }

        }

        private void SaveLeaderboard()
        {
            // Create the serializer for a list of BestOfLeaderboard
            var serializer = new XmlSerializer(typeof(List<BestOfLeaderboard>));

            // Write the serialized XML to file
            using (var fs = new FileStream(leaderboardFilePath, FileMode.Create))
            {
                serializer.Serialize(fs, leaderboard);
            }
        }

        private void LoadLeaderboard()
        {
            if (File.Exists(leaderboardFilePath))
            {
                // Create the same serializer
                var serializer = new XmlSerializer(typeof(List<BestOfLeaderboard>));

                // Read the XML from file
                using (var fs = new FileStream(leaderboardFilePath, FileMode.Open))
                {
                    leaderboard = (List<BestOfLeaderboard>)serializer.Deserialize(fs);
                }
            }
            else
            {
                // If no file exists yet, start with an empty list
                leaderboard = new List<BestOfLeaderboard>();
            }
        }

        private void AddToLeaderboard(string playerName, int score, int wins, int losses, int draws, int bestWinMoves, double winPercentage)
        {
            var newEntry = new BestOfLeaderboard
            {
                PlayerName = playerName,
                Score = score,
                Wins = wins,
                Losses = losses,
                Draws = draws,
                BestWinMoves = bestWinMoves,
                WinPercentage = winPercentage
            };

            // Add the new entry, sort, and keep top 10
            leaderboard.Add(newEntry);
            leaderboard = leaderboard
                .OrderByDescending(entry => entry.Score)
                .ThenBy(entry => entry.BestWinMoves)
                .Take(10)
                .ToList();

            // Save the updated list to XML
            SaveLeaderboard();
        }
    }
}
