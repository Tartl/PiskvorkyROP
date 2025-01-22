using System;
using System.Collections.Generic;

namespace Piskvorky
{
    public class Calculations
    {
        private int boardSize = 15;
        private GameSymbol[,] symbolsOnBoard;
        private short winLength = 5;
        private short[,,,] symbolsInRow;
        private short[,,,] openEnds;
        private short[,] DirectionSigns;
        private int rowsLeftOnBoard;
        private int[,,] fieldValues;
        private int[] Values;

        public Calculations(int boardSize)
        {
            this.boardSize = boardSize;
            DirectionSigns = new short[(short)Direction.Diag2 + 1, (short)Coordinate.Y + 1]
            { { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };
            Values = new int[7] { 0, 0, 4, 20, 100, 500, 0 };
        }

        public void SetBoardSize(int newSize)
        {
            boardSize = newSize;
            ClearBoard();
            ClearSymbolsInRow();
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

        public GameSymbol[,] SymbolsOnBoard
        {
            get
            {
                if (symbolsOnBoard == null) ClearBoard();
                return symbolsOnBoard;
            }
        }

        public int[,,] FieldValues
        {
            get
            {
                if (fieldValues == null) ClearFieldValues();
                return fieldValues;
            }
        }

        public void ClearSymbolsInRow()
        {
            symbolsInRow = new short[boardSize, boardSize, (short)Direction.Diag2 + 1, (short)GameSymbol.Symbol2 + 1];
            openEnds = new short[boardSize, boardSize, (short)Direction.Diag2 + 1, (short)GameSymbol.Symbol2 + 1];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                    {
                        for (GameSymbol symbol = GameSymbol.Symbol1; symbol < GameSymbol.Free; symbol++)
                        {
                            symbolsInRow[x, y, (short)direction, (short)symbol] = 0;
                            openEnds[x, y, (short)direction, (short)symbol] = 0;
                        }
                    }

                }
            }
            rowsLeftOnBoard = 4 * (2 * boardSize - (winLength - 1)) * (boardSize - (winLength - 1));
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

        public void ClearFieldValues()
        {
            fieldValues = new int[boardSize, boardSize, (short)GameSymbol.Symbol2 + 1];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    for (GameSymbol symbol = GameSymbol.Symbol1; symbol < GameSymbol.Free; symbol++)
                    {
                        fieldValues[x, y, (short)symbol] = 0;
                    }

                }
            }
        }

        public bool CordsOnBoard(int x, int y)
        {
            return x < boardSize && x >= 0 && y < boardSize && y >= 0;
        }

        private GameSymbol GetOpponent(GameSymbol currentPlayer)
        {
            if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
            if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
            throw new Exception("Hráč není správně určen!");
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
                        result = IncludeDraw(ref SymbolsInRow[posX, posY, (short)dir, (short)player]);
                        if (result != GameResult.Continue)
                        {
                            break;
                        }
                        for (int j = 0; j < winLength; j++)
                        {
                            short opponent = (short)GetOpponent(player);
                            int posXfield = posX - dirHor * j;
                            int posYfield = posY - dirVer * j;
                            RecalcValue(
                                SymbolsInRow[posX, posY, (short)dir, (short)player],
                                SymbolsInRow[posX, posY, (short)dir, opponent],
                                openEnds[posX, posY, (short)dir, (short)player],
                                openEnds[posX, posY, (short)dir, opponent],
                                ref FieldValues[posXfield, posYfield, (short)player],
                                ref FieldValues[posXfield, posYfield, opponent]);
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

        private void RecalcValue(
     short symbolsInRowCurrentPlayer,
     short symbolsInRowOpponent,
     short openEndsCurrentPlayer,
     short openEndsOpponent,
     ref int fieldValueForCurrentPlayer,
     ref int fieldValueOpponent)
        {
            // If the opponent has no stones in that row, we can add "attack" points
            if (symbolsInRowOpponent == 0)
            {
                // Base increment based on how many in a row
                int baseIncrement = Values[symbolsInRowCurrentPlayer + 1] - Values[symbolsInRowCurrentPlayer];

                // If openEndsCurrentPlayer == 0, it’s worthless
                if (openEndsCurrentPlayer == 0)
                {
                    baseIncrement /= 10;
                }
                else if (openEndsCurrentPlayer == 2)
                {
                    baseIncrement *= 2;
                }
                else if (openEndsCurrentPlayer == 1)
                {
                    baseIncrement += baseIncrement / 2;
                }

                fieldValueForCurrentPlayer += baseIncrement;
            }
            // Otherwise, if our row has only 1 stone, we reduce opponent's value
            else if (symbolsInRowCurrentPlayer == 1)
            {
                fieldValueOpponent -= Values[symbolsInRowOpponent];
            }
        }


        public void GetBestMove(Difficulty difficulty, out int x, out int y, GameSymbol player)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    GetBestMove_Easy(out x, out y, player);
                    break;
                case Difficulty.Medium:
                    GetBestMove_Medium(out x, out y, player);
                    break;
                case Difficulty.Hard:
                    GetBestMove_Hard(out x, out y, player);
                    break;
                default:
                    GetBestMove_Medium(out x, out y, player);
                    break;
            }

        }

        public void GetBestMove_Easy(out int x, out int y, GameSymbol player)
        {
            Random random = new Random();
            int chance = random.Next(0, 100);

            // Kdo je soupeř
            GameSymbol opponent = GetOpponent(player);

            // S 80% pravděpodobností zkusíme hledat výhru a blokovat
            if (chance < 80)
            {
                // 1) Zkus, zda můžeme vyhrát
                if (TryFindWinningMove(player, out x, out y))
                    return;

                // 2) Zkus, zda soupeř nemůže vyhrát, případně blokuj
                if (TryFindWinningMove(opponent, out x, out y))
                    return;
            }

            // 3) Buď rovnou (20% případů), nebo po prověření (80% případů) vyber "suboptimální" tah
            //    tzn. vyber z TOP 5 nejlepších tahů. Čím vyšší topN, tím "větší rezerva" na horší tah.
            PickMoveSuboptimalByFieldValue(out x, out y, player, opponent, topN: 2);
        }

        public void GetBestMove_Medium(out int x, out int y, GameSymbol player)
        {
            // 1) Zkus, zda můžeme vyhrát
            if (TryFindWinningMove(player, out x, out y))
                return;

            // 2) Zkus, zda soupeř nemůže vyhrát, případně zablokuj
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;

            // 3) Jinak vyber nejlepší tah dle FieldValues
            PickMoveByFieldValue(out x, out y, player, opponent);
        }

        /// <summary>
        /// TĚŽKÁ obtížnost – momentálně stejná jako Medium (lze dále rozvinout).
        /// </summary>
        public void GetBestMove_Hard(out int x, out int y, GameSymbol player)
        {
            // 1) Zkus, zda můžeme vyhrát
            if (TryFindWinningMove(player, out x, out y))
                return;

            // 2) Zkus, zda soupeř nemůže vyhrát, případně zablokuj
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;

        }

        private bool TryFindWinningMove(GameSymbol checkPlayer, out int winX, out int winY)
        {
            winX = -1;
            winY = -1;
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (SymbolsOnBoard[x, y] == GameSymbol.Free)
                    {
                        // Temporarily place the stone
                        SymbolsOnBoard[x, y] = checkPlayer;

                        bool isWinning = WouldThisMoveWin(x, y, checkPlayer);

                        // Remove the stone
                        SymbolsOnBoard[x, y] = GameSymbol.Free;

                        if (isWinning)
                        {
                            winX = x;
                            winY = y;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool WouldThisMoveWin(int x, int y, GameSymbol checkPlayer)
        {
            int[][] directions = new int[][]
            {
                new int[]{1, 0},
                new int[]{0, 1},
                new int[]{1, 1},
                new int[]{1, -1}
            };

            int required = winLength;
            foreach (var d in directions)
            {
                // FIXED: declare/reset 'count' each time
                int count = 1;  // the newly placed stone

                // forward direction
                count += CountDirection(x, y, d[0], d[1], checkPlayer);
                // backward direction
                count += CountDirection(x, y, -d[0], -d[1], checkPlayer);

                if (count >= required)
                    return true;
            }
            return false;
        }

        private int CountDirection(int startX, int startY, int dx, int dy, GameSymbol checkPlayer)
        {
            int cnt = 0;
            int x = startX + dx;
            int y = startY + dy;
            while (CordsOnBoard(x, y) && SymbolsOnBoard[x, y] == checkPlayer)
            {
                cnt++;
                x += dx;
                y += dy;
            }
            return cnt;
        }

        private void PickMoveByFieldValue(out int bestX, out int bestY, GameSymbol player, GameSymbol opponent)
        {
            int bestValue = int.MinValue;

            // Jako (alespoň) výchozí zkusíme střed desky, pokud je volný
            bestX = (boardSize + 1) / 2;
            bestY = (boardSize + 1) / 2;
            if (SymbolsOnBoard[bestX, bestY] == GameSymbol.Free)
            {
                bestValue = 4; // ať to není minValue
            }

            // Projdeme celou desku a hledáme pole s nejvyšší hodnotou
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        // Simple heuristika: FieldValues pro hráče vynásobíme 16,
                        // plus FieldValues pro soupeře vynásobíme 8
                        int value =
                            (FieldValues[i, j, (short)player] * 16)
                            + 1
                            + (FieldValues[i, j, (short)opponent] * 8);

                        if (value > bestValue)
                        {
                            bestValue = value;
                            bestX = i;
                            bestY = j;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Náhodně vybere jakékoli volné pole na desce.
        /// </summary>
        private void GetRandomFreeMove(out int x, out int y)
        {
            Random r = new Random();
            var freePositions = new List<(int, int)>();

            // Nasbíráme všechna volná pole
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        freePositions.Add((i, j));
                    }
                }
            }

            // Když není co hrát
            if (freePositions.Count == 0)
            {
                x = y = -1;
                return;
            }

            // Vybereme náhodný index
            var chosen = freePositions[r.Next(freePositions.Count)];
            x = chosen.Item1;
            y = chosen.Item2;
        }
        private void PickMoveSuboptimalByFieldValue(
            out int bestX,
            out int bestY,
            GameSymbol player,
            GameSymbol opponent,
            int topN = 5)
        {
            // Sesbíráme všechny volné pozice spolu s jejich "skóre"
            var candidates = new List<(int X, int Y, int Score)>();

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        int value =
                            (FieldValues[i, j, (short)player] * 16)
                            + 1
                            + (FieldValues[i, j, (short)opponent] * 8);

                        candidates.Add((i, j, value));
                    }
                }
            }

            // Pokud není žádný volný tah (deska plná), končíme
            if (candidates.Count == 0)
            {
                bestX = -1;
                bestY = -1;
                return;
            }

            // Seřadíme kandidáty sestupně podle Score
            candidates.Sort((a, b) => b.Score.CompareTo(a.Score));

            // Omezíme se na topN prvků (pokud jich je méně, vezmeme všechny)
            if (topN > candidates.Count)
                topN = candidates.Count;

            // Z té top skupiny vybereme náhodně jeden
            Random rnd = new Random();
            var chosen = candidates[rnd.Next(topN)];

            bestX = chosen.X;
            bestY = chosen.Y;
        }
    }
}
