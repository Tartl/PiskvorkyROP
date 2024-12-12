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
        private int rowsLeftOnBoard;

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
            rowsLeftOnBoard = 4 * (2 * boardSize - (winLength - 1)) * (boardSize - (winLength - 1));
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
            GameResult result = GameResult.Continue;
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                short dirHor = DirectionSigns[(short)dir, (short)Coordinate.X];
                short dirVer = DirectionSigns[(short)dir, (short)Coordinate.Y];
                for (int i = 0; i < winLength; i++)
                {
                    int posX = x + dirHor * i;
                    int posY = y + dirVer * i;
                    if (PositionWithinBounds(posX, posY, dirHor, dirVer))
                    {
                        result = IncludeDraw(ref SymbolsInRow[posX,posY,(short)dir,(short)player]);
                        if (result != GameResult.Continue)
                        {
                            break;
                        }
                    }
                }
                if (result != GameResult.Continue)
                {
                    break;
                }
            }

            SymbolsOnBoard[x, y] = player;
            if (result == GameResult.Continue && rowsLeftOnBoard <= 0)
                result = GameResult.Draw;
            return result;
        }
        private bool PositionWithinBounds(int posX, int posY, short dirHor, short dirVer)
        {
            bool withinHorizontalBounds = (dirHor == -1 && posX >= 0 && posX <= boardSize - winLength) || 
                                          (dirHor == 1 && posX >= winLength - 1 && posX < boardSize) || 
                                          (dirHor == 0);

            bool withinVerticalBounds = (dirVer == -1 && posY >= 0 && posY <= boardSize - winLength) ||
                                        (dirVer == 1 && posY >= winLength - 1 && posY < boardSize) ||
                                        (dirVer == 0);

            return withinHorizontalBounds && withinVerticalBounds;
        }

        private GameResult IncludeDraw(ref short numberInRow)
        {
            numberInRow++;
            if (numberInRow == winLength)
                return GameResult.Win;
            if (numberInRow == 1)
                rowsLeftOnBoard--;
            return GameResult.Continue;
        }

    }


}
