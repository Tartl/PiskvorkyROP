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
            height = 0;
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
        double player1Score = 0;
        double player2Score = 0;
        double player_winPercentage = 0;
        int movesCount = 0;
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
            if (GameSettings.Player1Name != "" && GameSettings.Player2Name != "")
            {
                player1_name = GameSettings.Player1Name;
                player2_name = GameSettings.Player2Name;
            }
            else
            {
                GameSettings.Player1Name = player1_name;
                GameSettings.Player2Name = player2_name;
            }
            label_hrac1.Text = player1_name;
            label_hrac2.Text = player2_name;
            leaderboard = LoadLeaderboard(leaderboardFilePath);
            if (GameSettings.DemoMode)
            {
                label_score.Text = "Pro start dema klikněte na hrací plochu";
                nováHraToolStripMenuItem.Enabled = false;
                player1_name = "Počítač 1";
                player2_name = "Počítač 2";
            }
                
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
            playingBoard1.Symbol1Color = GameSettings.Player1Color;
            playingBoard1.Symbol2Color = GameSettings.Player2Color;
            playingBoard1.IsPlayingAI = GameSettings.IsAgainstAI;
            playingBoard1.AIDifficulty = GameSettings.AI_Difficulty;
        }

        public void BoardRedraw()
        {
            if (Math.Abs(height - Height) > ResizeThreshold)
            {
                int currentAvg = (Width + Height) / 2;
                int minAvg = (this.MinimumSize.Width + this.MinimumSize.Height) / 2;
                playingBoard1.FieldSize = (int)(fieldSize * (float)currentAvg / minAvg);
                width = Width;
                height = Height;
                
            }
        }

        private void FormBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLeaderboard(leaderboardFilePath, leaderboard);
        }

        private void FormBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            formMenu.Show();
            if (GameSettings.DemoMode)
            {
                GameSettings.DemoMode = false;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            BoardRedraw();
            playingBoard1.Location = new Point((this.ClientSize.Width - playingBoard1.Width) / 2, playingBoard1.Location.Y);
        }

        private void UpdateScoreLabel()
        {
            label_score.Text = $"{player1Score}:{player2Score}";
        }

        private void OnPlayerWon(GameSymbol winner)
        {
            gamesPlayed++;
            if (!GameSettings.DemoMode)
            {
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
                        MessageBox.Show($"Partii vyhrál {player2_name}!");
                        player2Score++;
                    }
                }
                UpdateScoreLabel();
                if (gamesPlayed == gameLength)
                {
                    player_bestWinMoves = playingBoard1.MovesToWinMin;
                    MessageBox.Show($"Konec hry!\nFinální skóre je {label_score.Text} a nejkratší hra měla {player_bestWinMoves} tahů");
                    if (GameSettings.AI_Difficulty == "těžká" && GameSettings.IsAgainstAI)
                    {
                        player_score = (int)player1Score * 100 - player_losses * 25 - player_bestWinMoves;
                        player_winPercentage = (double)player_wins / gamesPlayed * 100;
                        AddToLeaderboard(player1_name, player_score, player_wins, player_losses, player_draws, player_bestWinMoves, player_winPercentage);
                    }
                    player1Score = 0;
                    player2Score = 0;
                    UpdateScoreLabel();
                    Close();
                }
            }
            else
            {
                if (winner == GameSymbol.Symbol1)
                {
                    winSound.Play();
                    MessageBox.Show($"Partii vyhrál {GameSettings.Player1Symbol}!");
                }
                else
                {
                    winSound.Play();
                    MessageBox.Show($"Partii vyhrál {GameSettings.Player2Symbol}!");
                }
            }
        }

        private void OnDraw()
        {
            gamesPlayed++;
            MessageBox.Show("Na hrací ploše již nejsou žádné výhry, došlo k remíze!");
            player_draws++;
            player1Score += 0.5;
            player2Score += 0.5;
            if(!GameSettings.DemoMode)
            {
                UpdateScoreLabel();

                if (gamesPlayed == gameLength)
                {
                    player_bestWinMoves = playingBoard1.MovesToWinMin;
                    MessageBox.Show($"Konec hry!\nFinální skóre je {label_score.Text} a nejkratší hra měla {player_bestWinMoves} tahů");
                    if (GameSettings.AI_Difficulty == "těžká" && GameSettings.IsAgainstAI)
                    {
                        player_score = (int)player1Score * 100 - player_losses * 25 - player_bestWinMoves;
                        player_winPercentage = (double)player_wins / gamesPlayed * 100;
                        AddToLeaderboard(player1_name, player_score, player_wins, player_losses, player_draws, player_bestWinMoves, player_winPercentage);
                    }
                    player1Score = 0;
                    player2Score = 0;
                    UpdateScoreLabel();
                    Close();
                }
            }
                
        }

        private void nováHraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormNewGame formNewGame = new FormNewGame();
            formNewGame.ShowDialog();
            if (formNewGame.DialogResult == DialogResult.OK)
            {
                GameSettings.Player1Name = formNewGame.textBox1.Text;
                GameSettings.Player2Name = formNewGame.textBox2.Text;
                FormBoard board = new FormBoard(formMenu);
                board.Show();
            }
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

        private void uloženíToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveFolder = Path.Combine(Application.StartupPath, "save");
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            var calc = playingBoard1.Calc;
            saveFileDialog1.InitialDirectory = saveFolder;
            saveFileDialog1.Filter = "DAT files (*.dat)|*.dat";
            saveFileDialog1.DefaultExt = "dat";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;
                SaveData data = new SaveData
                {
                    BoardSize = playingBoard1.BoardSize,
                    WinLength = calc.WinLength,
                    NextPlayer = (int)playingBoard1.CurrentPlayer,
                    GamesPlayed = gamesPlayed,
                    PlayerWins = player_wins,
                    PlayerLosses = player_losses,
                    PlayerDraws = player_draws,
                    ScoreText = label_score.Text,
                    IsAI = GameSettings.IsAgainstAI,
                    Player1Name = GameSettings.Player1Name,
                    Player2Name = GameSettings.Player2Name,
                    AIDiff = GameSettings.AI_Difficulty,
                    MovesCount = movesCount,
                    BoardRows = new List<string>()
                };
                for (int x = 0; x < playingBoard1.BoardSize; x++)
                {
                    List<string> rowSymbols = new List<string>();
                    for (int y = 0; y < playingBoard1.BoardSize; y++)
                    {
                        rowSymbols.Add(((int)calc.SymbolsOnBoard[x, y]).ToString());
                    }
                    data.BoardRows.Add(string.Join(";", rowSymbols));
                }

                // Serialize the SaveData object to XML
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                using (StringWriter stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, data);
                    string xmlContent = stringWriter.ToString();

                    // Convert the XML content into a byte array.
                    byte[] bytesToWrite = Encoding.UTF8.GetBytes(xmlContent);

                    // Use BinaryWriter to create the file in binary format.
                    using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
                    {
                        // Optionally, you can write a file header or the length of the data here.
                        // For example: bw.Write(bytesToWrite.Length);
                        bw.Write(bytesToWrite);
                    }
                }
                MessageBox.Show("Hra byla úspěšně uložena!");
            }
        }


        private void otevřeníToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var calc = playingBoard1.Calc;
            string saveFolder = Path.Combine(Application.StartupPath, "save");
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            openFileDialog1.InitialDirectory = saveFolder;
            openFileDialog1.Filter = "DAT files (*.dat)|*.dat";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                playingBoard1.ResetGame();
                string path = openFileDialog1.FileName;

                // Read the file using BinaryReader.
                string xmlContent;
                using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    byte[] fileBytes = br.ReadBytes((int)br.BaseStream.Length);
                    xmlContent = Encoding.UTF8.GetString(fileBytes);
                }

                // Deserialize the XML string back into the SaveData object.
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                SaveData data;
                using (StringReader sr = new StringReader(xmlContent))
                {
                    data = (SaveData)serializer.Deserialize(sr);
                }

                // Update the game state from the loaded data.
                calc.SetBoardSize(data.BoardSize);
                calc.WinLength = data.WinLength;
                playingBoard1.CurrentPlayer = (GameSymbol)data.NextPlayer;
                gamesPlayed = data.GamesPlayed;
                player_wins = data.PlayerWins;
                player_losses = data.PlayerLosses;
                player_draws = data.PlayerDraws;
                label_score.Text = data.ScoreText;
                GameSettings.IsAgainstAI = data.IsAI;
                GameSettings.Player1Name = data.Player1Name;
                GameSettings.Player2Name = data.Player2Name;
                GameSettings.AI_Difficulty = data.AIDiff;
                movesCount = data.MovesCount;
                playingBoard1.IsPlayingAI = GameSettings.IsAgainstAI;
                playingBoard1.AIDifficulty = GameSettings.AI_Difficulty;
                player1_name = GameSettings.Player1Name;
                player2_name = GameSettings.Player2Name;
                label_hrac1.Text = player1_name;
                label_hrac2.Text = player2_name;

                // Parse the board rows and update the board.
                for (int x = 0; x < data.BoardSize; x++)
                {
                    string[] symbols = data.BoardRows[x].Split(';');
                    for (int y = 0; y < symbols.Length; y++)
                    {
                        calc.SymbolsOnBoard[x, y] = (GameSymbol)int.Parse(symbols[y]);
                    }
                }
                calc.ClearSymbolsInRow();
                for (int x = 0; x < playingBoard1.BoardSize; x++)
                {
                    for (int y = 0; y < playingBoard1.BoardSize; y++)
                    {
                        if (calc.SymbolsOnBoard[x, y] != GameSymbol.Free)
                        {
                            calc.AddSymbol(x, y, calc.SymbolsOnBoard[x, y]);
                        }
                    }
                }
                playingBoard1.Invalidate();
                MessageBox.Show("Hra byla úspěšně načtena!");
            }
        }


        public void SaveLeaderboard(string filePath, List<BestOfLeaderboard> leaderboard)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<BestOfLeaderboard>));
            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, leaderboard);
                string xmlContent = stringWriter.ToString();
                string encodedContent = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));
                File.WriteAllText(filePath, encodedContent);
            }
        }

        public List<BestOfLeaderboard> LoadLeaderboard(string filePath)
        {
            if (!File.Exists(filePath)) return new List<BestOfLeaderboard>();
            string encodedContent = File.ReadAllText(filePath);
            string xmlContent = Encoding.UTF8.GetString(Convert.FromBase64String(encodedContent));
            XmlSerializer serializer = new XmlSerializer(typeof(List<BestOfLeaderboard>));
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                return (List<BestOfLeaderboard>)serializer.Deserialize(stringReader);
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
            leaderboard.Add(newEntry);
            leaderboard = leaderboard
                .OrderByDescending(entry => entry.Score)
                .ThenBy(entry => entry.BestWinMoves)
                .Take(10)
                .ToList();
            SaveLeaderboard(leaderboardFilePath, leaderboard);
        }
    }
}
