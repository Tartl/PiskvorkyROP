﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private float Symbol1Thickness { get; set; }
        private float Symbol2Thickness { get; set; }

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
                boardSize= value;
                Refresh();
            }
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
        public Pen Symbol1Pen
        {
            get
            {
                if (symbol1Pen == null) symbol1Pen = new Pen(symbol1Color, symbol1Thickness);
                return symbol1Pen;
            }
        }
        public Pen Symbol2Pen
        {
            get
            {
                if (symbol2Pen == null) symbol2Pen = new Pen(symbol2Color, symbol2Thickness);
                return symbol2Pen;
            }
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
            if (!Calc.CordsOnBoard(x,y))
                throw new Exception("Souřadnice se nachází mimo hrací plochu!");
            if (symbol == GameSymbol.Symbol1)
            {
                graphics.DrawLine(Symbol1Pen, x * fieldSize + 1, y * fieldSize + 1, x * fieldSize + fieldSize - 1, y * fieldSize + fieldSize - 1);
                graphics.DrawLine(Symbol1Pen, x * fieldSize + 1, y * fieldSize + fieldSize - 1, x * fieldSize + fieldSize - 1, y * fieldSize + 1);

            }
            if (symbol == GameSymbol.Symbol2)
            {
                graphics.DrawEllipse(Symbol2Pen, x * fieldSize + 1, y * fieldSize + 1, fieldSize - 2, fieldSize - 2);
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
            currentPlayer = GameSymbol.Symbol2;
            Refresh();
        }

        private void PlayingBoard_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / fieldSize;
            int y = e.Y / fieldSize;
            if (!Calc.CordsOnBoard(x,y))
                return;
            if (Calc.SymbolsOnBoard[x, y] != GameSymbol.Free)
            {
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
            currentPlayer = Opponent;
            
        }
    }
}
