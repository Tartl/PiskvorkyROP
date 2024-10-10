using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public class Calculations
    {
        private int boardSize = 15;
        private GameSymbol[,] symbolsOnBoard;

        private short winLength = 5;
        private short[,,,] symbolsInRow;
        private short[,] DirectionSigns;

        public Calculations(int boardSize)
        {
            this.boardSize = boardSize;
            DirectionSigns = new short[(short)Direction.Diag2 + 1, (short)Coordinate.Y + 1] 
            { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
        }

        public short WinLength
        {
            get { return winLength; }
            set { winLength = value; }
        }

        public short[,,,] SymbolsInRow
        {
            get
            {
                if (symbolsInRow == null) ClearSymbolsInRow();
                return symbolsInRow;
            }
            set
            {

            }
        }
        
        public void ClearSymbolsInRow()
        {
            symbolsInRow = new short[boardSize, boardSize,(short)Direction.Diag2 + 1,(short)GameSymbol.Symbol2 + 1];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    foreach(Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        for (GameSymbol symbol = GameSymbol.Symbol1; symbol < GameSymbol.Free; symbol++)
                        {
                            symbolsInRow[x, y, (short)direction, (short)symbol] = 0;
                        }
                    }
                   
                }
            }
        }

        public GameSymbol[,] SymbolsOnBoard
        {
            get
            {
                if (symbolsOnBoard == null) ClearBoard();
                return symbolsOnBoard;
            }
        }
        public void ClearBoard()
        {
            symbolsOnBoard = new GameSymbol[boardSize, boardSize];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    symbolsOnBoard[x, y] = GameSymbol.Free;
                }
            }
        }

        public bool CordsOnBoard(int x, int y)
        {
            return x < boardSize && x >= 0 && y < boardSize && y >= 0;
        }

        public GameResult AddSymbol(int x, int y, GameSymbol player)
        {
            return GameResult.Continue;
        }
    }
}
