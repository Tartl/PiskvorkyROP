using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Piskvorky
{
    public partial class PlayingBoard : UserControl
    {
        private int boardSize = 15;
        private int fieldSize = 20;
        private Color gridColor = Color.Black;
        private Color symbol1Color = Color.Red;
        private Color symbol2Color = Color.Blue;
        private Color lastDrawnColor = Color.Yellow;
        private Color winRowColor = Color.LightGreen;
        private Pen gridPen;
        private int lastDrawnX = -1, 
                    lastDrawnY = -1;
        private Calculations calc;
        private GameSymbol currentPlayer = GameSymbol.Symbol1;
        private float gridThickness = 1f;
        SoundPlayer fieldFullSound = new SoundPlayer(@"sound\symbolExists-effect.wav");
        private bool isPlayingAI;
        private bool isAIThinking = false;
        private string aiDifficulty;
        private int movesToWin = 0;
        private int movesToWinMin = 626;
        private List<List<Point>> winningRows;

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

        public GameSymbol CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
            set
            {
                currentPlayer = value;
            }
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
        public int BoardSize
        {
            get { return boardSize; }
            set
            {
                boardSize = value;
                FieldSize = 20 + (15 - boardSize);
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
                Size = new Size(BoardSize * fieldSize + 1, BoardSize * fieldSize + 1);
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

        public Color LastDrawnColor
        {
            get { return lastDrawnColor; }
            set
            {
                lastDrawnColor = value;
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

        public int MovesToWinMin
        {
            get { return movesToWinMin; }
            set { movesToWinMin = value; }
        }

        public int MovesToWin
        {
            get { return movesToWin; }
            set { movesToWin = value; }
        }

        private void PlayingBoard_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e.Graphics);
            HighlightLastMove(e.Graphics);
            HighlightWinRows(e.Graphics, winningRows);
            DrawSymbols(e.Graphics);
        }

        private void HighlightLastMove(Graphics graphics)
        {
            HighlightMove(graphics, lastDrawnColor, lastDrawnX, lastDrawnY);
            
        }

        private void HighlightMove(Graphics graphics, Color color, int x, int y)
        {
            if (Calc.CordsOnBoard(x, y))
            {
                graphics.FillRectangle(new SolidBrush(color), 
                                        x * fieldSize + 1, 
                                        y * fieldSize + 1, 
                                        fieldSize - 1, 
                                        fieldSize - 1);
            }
        }

        private void HighlightWinRows(Graphics graphics, List<List<Point>> winRows)
        {
            if (winRows != null)
            {
                foreach (List<Point> winRow in winRows)
                {
                    foreach (Point p in winRow)
                    {
                        HighlightMove(graphics, winRowColor, p.X, p.Y);
                    }
                }
            }
        }

        private void ClearHighlightedMove()
        {
            lastDrawnX = -1;
            lastDrawnY = -1;
            Refresh();
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

        public void ResetGame()
        {
            winningRows = null;
            Calc.ClearBoard();
            Calc.ClearSymbolsInRow();
            Calc.ClearFieldValues();
            ClearHighlightedMove();
            currentPlayer = GameSymbol.Symbol1;
            isAIThinking = false;
            movesToWin = 0;
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
                return;
            }
            movesToWin++;
            GameResult result = Calc.AddSymbol(x, y, currentPlayer, out winningRows);
            lastDrawnX = x;
            lastDrawnY = y;
            Refresh();
            if (result == GameResult.Win)
            {
                if (movesToWin < movesToWinMin && currentPlayer == GameSymbol.Symbol1)
                {
                    movesToWinMin = movesToWin;
                }
                PlayerWon?.Invoke(currentPlayer);
                ResetGame();
                return;
            }
            else if (result == GameResult.Draw)
            {
                Draw?.Invoke();
                ResetGame();
                return;
            }
            currentPlayer = Opponent;

            if (GameSettings.DemoMode)
            {

                isAIThinking = true;
                Stopwatch stopwatch = Stopwatch.StartNew();

                var bestMove = await Task.Run(() =>
                {
                    int aiX, aiY;
                    if (currentPlayer == GameSymbol.Symbol1)
                    {
                        Calc.GetBestMove(Difficulty.Hard, out aiX, out aiY, currentPlayer);
                    }
                    else
                    {
                        Calc.GetBestMove(Difficulty.Medium, out aiX, out aiY, currentPlayer);
                    }
                    return (aiX, aiY);
                });

                stopwatch.Stop();
                int elapsed = (int)stopwatch.ElapsedMilliseconds;
                int delay = Math.Max(0, 1000 - elapsed);
                await Task.Delay(delay);

                await AddMove(bestMove.aiX, bestMove.aiY);
                isAIThinking = false;
            }

            if (isPlayingAI && currentPlayer == GameSymbol.Symbol2)
            {
                isAIThinking = true;
                Stopwatch stopwatch = Stopwatch.StartNew();

                var bestMove = await Task.Run(() =>
                {
                    int optX, optY;
                    Calc.GetBestMove(GetDifficulty(AIDifficulty), out optX, out optY, currentPlayer);
                    return (optX, optY);
                });

                stopwatch.Stop();
                int elapsed = (int)stopwatch.ElapsedMilliseconds;
                int delay = Math.Max(0, 1000 - elapsed);
                await Task.Delay(delay);

                await AddMove(bestMove.optX, bestMove.optY);
                isAIThinking = false;
            }

        }
    }   
}
