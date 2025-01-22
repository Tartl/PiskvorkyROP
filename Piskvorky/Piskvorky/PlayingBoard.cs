using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public partial class PlayingBoard : UserControl
    {
        private int boardSize = 15;
        private int fieldSize = 20;
        private Color gridColor = Color.Black;
        private Color symbol1Color = Color.Red;
        private Color symbol2Color = Color.Blue;
        private Pen gridPen;
        private Pen symbol1Pen;
        private Pen symbol2Pen;
        private Calculations calc;
        private GameSymbol currentPlayer = GameSymbol.Symbol1;
        private float gridThickness = 1f;
        private float symbol1Thickness = 2f;
        private float symbol2Thickness = 2f;
        private float GridThickness {  get; set; }
        SoundPlayer fieldFullSound = new SoundPlayer(@"sound\symbolExists-effect.wav");
        private bool isPlayingAI;
        private bool isAIThinking = false;
        private string aiDifficulty;

        public event Action<GameSymbol> PlayerWon;
        public event Action Draw;

        public Calculations Calc
        {
            get 
            {
                if (calc == null) calc = new Calculations(boardSize);
                return calc; 
            }
            set { calc = value; }
        }
        private GameSymbol Opponent
        {
            get 
            {
                if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
                if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
                throw new Exception("Hráč není správně určen!");
            }
        }
        public int BoardSize {
            get { return boardSize; }
            set
            {
                boardSize = value;
                Refresh();
            }
        }
        public bool IsPlayingAI
        {
            get { return isPlayingAI; }
            set { isPlayingAI = value; }
        }

        public int FieldSize {
            get { return fieldSize; } 
            set
            {
                fieldSize = value;
                Refresh();
            }
        }
        public Color GridColor { 
            get { return gridColor; }
            set
            {
                gridColor = value;
                Refresh();
            }
        }
        public Color Symbol1Color
        {
            get { return symbol1Color; }
            set
            {
                symbol1Color = value;
                Refresh();
            }
        }
        public Color Symbol2Color
        {
            get { return symbol2Color; }
            set
            {
                symbol2Color = value;
                Refresh();
            }
        }
        public Pen GridPen {
            get {
                if ( gridPen == null ) gridPen = new Pen(GridColor, gridThickness);
                return gridPen;
            }
        }
        public string Symbol1Emoji { get; set; } = "❌";
        public string Symbol2Emoji { get; set; } = "⭕";

        public string AIDifficulty
        {
            get { return aiDifficulty; }
            set { aiDifficulty = value; }
        }

        private void PlayingBoard_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e.Graphics);
            DrawSymbols(e.Graphics);
        }

        private void DrawBoard (Graphics graphics)
        {
            for (int i = 0; i <= boardSize; i++)
            {
                graphics.DrawLine(GridPen, 0, i * fieldSize, boardSize * fieldSize, i * fieldSize);
                graphics.DrawLine(GridPen, i * fieldSize, 0, i * fieldSize, boardSize * fieldSize);
            }
        }

        private void DrawSymbol(Graphics graphics, GameSymbol symbol, int x, int y)
        {
            if (!Calc.CordsOnBoard(x, y))
                throw new Exception("Souřadnice se nachází mimo hrací plochu!");

            string emoji = symbol == GameSymbol.Symbol1 ? Symbol1Emoji : (symbol == GameSymbol.Symbol2 ? Symbol2Emoji : null);
            if (emoji != null)
            {
                Color symbolColor = symbol == GameSymbol.Symbol1 ? Symbol1Color : Symbol2Color;

                using (Font emojiFont = new Font("Segoe UI Emoji", fieldSize / 2))
                using (Brush textBrush = new SolidBrush(symbolColor))
                {
                    SizeF textSize = graphics.MeasureString(emoji, emojiFont);
                    float posX = x * fieldSize + (fieldSize - textSize.Width) / 2;
                    float posY = y * fieldSize + (fieldSize - textSize.Height) / 2;

                    graphics.DrawString(emoji, emojiFont, textBrush, posX, posY);
                }
            }
        }

        private void DrawSymbols(Graphics graphics)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    DrawSymbol(graphics, Calc.SymbolsOnBoard[x, y], x, y);
                }
            }
        }

        public PlayingBoard()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void UpdateScore(Label scoreLabel, GameSymbol winner)
        {
            string[] scores = scoreLabel.Text.Split(':');
            int player1Score = int.Parse(scores[0]);
            int player2Score = int.Parse(scores[1]);

            if (winner == GameSymbol.Symbol1)
                player1Score++;
            else if (winner == GameSymbol.Symbol2)
                player2Score++;

            scoreLabel.Text = $"{player1Score}:{player2Score}";
        }

        public void ResetGame()
        {
            Calc.ClearBoard();
            Calc.ClearSymbolsInRow();
            Calc.ClearFieldValues();
            currentPlayer = GameSymbol.Symbol1;
            isAIThinking = false;
            Refresh();
        }

        private async void PlayingBoard_MouseClick(object sender, MouseEventArgs e)
        {
            if (isAIThinking)
                return;
            int x = e.X / fieldSize;
            int y = e.Y / fieldSize;
            await AddMove(x, y);
            
        }

        private Difficulty GetDifficulty(string aidifficulty)
        {
            switch (aidifficulty)
            {
                case "lehká":
                    return Difficulty.Easy;

                case "střední":
                    return Difficulty.Medium;
                case "těžká":
                    return Difficulty.Hard;
                default:
                    return Difficulty.Medium;
            }
        }

        private async Task AddMove(int x, int y)
        {
            

            if (!Calc.CordsOnBoard(x, y))
                return;
            if (Calc.SymbolsOnBoard[x, y] != GameSymbol.Free)
            {
                fieldFullSound.Play();
                MessageBox.Show("Tady už symbol je!");
                return;
            }
            GameResult result = Calc.AddSymbol(x, y, currentPlayer);
            Refresh();
            if (result == GameResult.Win)
            {
                PlayerWon?.Invoke(currentPlayer);
                ResetGame();
            }
            else if (result == GameResult.Draw)
            {
                Draw?.Invoke();
                ResetGame();
            }
            else if(isPlayingAI)
            {
                currentPlayer = Opponent;
                if (currentPlayer == GameSymbol.Symbol2)
                {
                    isAIThinking = true;
                    await Task.Delay(1000);
                    int optX;
                    int optY;
                    
                    Calc.GetBestMove(GetDifficulty(AIDifficulty), out optX, out optY, currentPlayer);
                    await AddMove(optX, optY);
                    isAIThinking = false;
                }
            }
            else
            {
                currentPlayer = Opponent;
            }
            
        }
    }   
}
