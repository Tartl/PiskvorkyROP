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
            {
                { -1,  0 }, // Hor
                { -1, -1 }, // Diag1
                {  0, -1 }, // Ver
                {  1, -1 }  // Diag2
            };

            // 1..4 in a row => [4, 20, 100, 500], etc.
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

            // Potential lines count
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
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        private GameSymbol GetOpponent(GameSymbol currentPlayer)
        {
            if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
            if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
            throw new Exception("Invalid player symbol!");
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
                        result = IncludeDraw(ref symbolsInRow[posX, posY, (short)dir, (short)player]);
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
                                symbolsInRow[posX, posY, (short)dir, (short)player],
                                symbolsInRow[posX, posY, (short)dir, opponent],
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
            bool withinHorizontalBounds =
                (dirHor == -1 && posX >= 0 && posX <= boardSize - winLength)
                || (dirHor == 1 && posX >= winLength - 1 && posX < boardSize)
                || (dirHor == 0);

            bool withinVerticalBounds =
                (dirVer == -1 && posY >= 0 && posY <= boardSize - winLength)
                || (dirVer == 1 && posY >= winLength - 1 && posY < boardSize)
                || (dirVer == 0);

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

        /// <summary>
        /// Example: if openEnds == 0, we reduce (but not always set to zero).
        /// Tweak as needed.
        /// </summary>
        private void RecalcValue(
            short symbolsInRowCurrentPlayer,
            short symbolsInRowOpponent,
            short openEndsCurrentPlayer,
            short openEndsOpponent,
            ref int fieldValueForCurrentPlayer,
            ref int fieldValueForOpponent)
        {
            // If opponent has no stones in that row, we can add "attack" points
            if (symbolsInRowOpponent == 0)
            {
                int baseIncrement = Values[symbolsInRowCurrentPlayer + 1] - Values[symbolsInRowCurrentPlayer];

                if (openEndsCurrentPlayer == 0)
                {
                    // Instead of 0, let's just slash it by 5 for instance:
                    baseIncrement /= 5;
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
            // If we have exactly 1 symbol in a row, we reduce the opponent's
            else if (symbolsInRowCurrentPlayer == 1)
            {
                fieldValueForOpponent -= Values[symbolsInRowOpponent];
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

            // Usually tries to find own immediate win or block
            GameSymbol opponent = GetOpponent(player);

            // 80% tries to find immediate win & block
            if (chance < 80)
            {
                // 1) If we can win immediately
                if (TryFindWinningMove(player, out x, out y))
                    return;

                // 2) If the opponent can win next turn, block
                if (TryFindWinningMove(opponent, out x, out y))
                    return;

                // 3) Block open 3 or 4 from opponent
                if (TryFindOpenThreeOrFour(opponent, out x, out y))
                {
                    return;
                }
            }

            // If we didn't do that, pick a suboptimal move
            PickMoveSuboptimalByFieldValue(out x, out y, player, opponent, topN: 5);
        }

        public void GetBestMove_Medium(out int x, out int y, GameSymbol player)
        {
            // 1) Check immediate win
            if (TryFindWinningMove(player, out x, out y))
                return;

            // 2) Block opponent's immediate win
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;

            // 3) Otherwise pick best by FieldValues + center bonus
            PickMoveByFieldValue(out x, out y, player, opponent);
        }

        public void GetBestMove_Hard(out int x, out int y, GameSymbol player)
        {
            // For now, same as Medium
            if (TryFindWinningMove(player, out x, out y))
                return;

            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;

            PickMoveByFieldValue(out x, out y, player, opponent);
        }

        /// <summary>
        /// If a move leads to an immediate win for 'checkPlayer', return true + coords.
        /// </summary>
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
                        SymbolsOnBoard[x, y] = checkPlayer;
                        bool isWinning = WouldThisMoveWin(x, y, checkPlayer);
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

            foreach (var d in directions)
            {
                int count = 1;
                count += CountDirection(x, y, d[0], d[1], checkPlayer);
                count += CountDirection(x, y, -d[0], -d[1], checkPlayer);

                if (count >= winLength) return true;
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

        private bool TryFindOpenThreeOrFour(GameSymbol opponent, out int blockX, out int blockY)
        {
            blockX = -1;
            blockY = -1;

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (SymbolsOnBoard[x, y] == GameSymbol.Free)
                    {
                        // Simulate opponent placing here
                        SymbolsOnBoard[x, y] = opponent;
                        bool dangerous = IsOpenThreeOrFour(x, y, opponent);
                        SymbolsOnBoard[x, y] = GameSymbol.Free;

                        if (dangerous)
                        {
                            blockX = x;
                            blockY = y;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsOpenThreeOrFour(int placedX, int placedY, GameSymbol symbol)
        {
            // Already placed the stone at (placedX, placedY)
            int[][] directions = new int[][]
            {
                new int[]{1, 0},
                new int[]{0, 1},
                new int[]{1, 1},
                new int[]{1, -1}
            };

            foreach (var d in directions)
            {
                int count = 1;
                int leftCount = CountDirection(placedX, placedY, -d[0], -d[1], symbol);
                int rightCount = CountDirection(placedX, placedY, d[0], d[1], symbol);
                count += leftCount + rightCount;

                bool leftOpen = IsOpenEnd(placedX, placedY, -d[0], -d[1]);
                bool rightOpen = IsOpenEnd(placedX, placedY, d[0], d[1]);
                int openEndsCount = (leftOpen ? 1 : 0) + (rightOpen ? 1 : 0);

                // "Open 4" => 4 in a row, at least 1 open end
                if (count == 4 && openEndsCount >= 1) return true;

                // "Open 3" => 3 in a row, 2 open ends
                if (count == 3 && openEndsCount == 2) return true;
            }
            return false;
        }

        private bool IsOpenEnd(int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;
            if (!CordsOnBoard(x, y)) return false;
            return (SymbolsOnBoard[x, y] == GameSymbol.Free);
        }

        /// <summary>
        /// Gives a small bonus for squares closer to the center
        /// so the AI doesn't randomly pick corners when all else is equal.
        /// Tweak the formula as you like!
        /// </summary>
        private int GetCenterBonus(int row, int col)
        {
            int center = boardSize / 2;
            // Manhattan distance from center
            int dx = Math.Abs(row - center);
            int dy = Math.Abs(col - center);
            int dist = dx + dy;

            // The smaller dist is, the bigger the bonus. For example:
            // radius = center => maximum distance is roughly center*2 from a corner.
            int radius = center * 2;
            // We'll do "radius - dist" so center gets the highest bonus.
            int bonus = radius - dist;
            if (bonus < 0) bonus = 0;
            return bonus;
        }

        private void PickMoveByFieldValue(out int bestX, out int bestY, GameSymbol player, GameSymbol opponent)
        {
            int bestValue = int.MinValue;

            bestX = boardSize / 2;
            bestY = boardSize / 2;
            if (SymbolsOnBoard[bestX, bestY] == GameSymbol.Free)
            {
                // Just a small baseline so we don't keep int.MinValue
                bestValue = 4;
            }

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        // Original heuristic
                        int value =
                            (FieldValues[i, j, (short)player] * 16)
                            + 1
                            + (FieldValues[i, j, (short)opponent] * 8);

                        // Add the center bonus
                        value += GetCenterBonus(i, j);

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

        private void PickMoveSuboptimalByFieldValue(
            out int bestX,
            out int bestY,
            GameSymbol player,
            GameSymbol opponent,
            int topN = 5)
        {
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

                        // If you want the suboptimal approach also to consider center bonus:
                        value += GetCenterBonus(i, j);

                        candidates.Add((i, j, value));
                    }
                }
            }

            if (candidates.Count == 0)
            {
                bestX = -1;
                bestY = -1;
                return;
            }

            // Sort descending by Score
            candidates.Sort((a, b) => b.Score.CompareTo(a.Score));

            if (topN > candidates.Count)
                topN = candidates.Count;

            Random rnd = new Random();
            var chosen = candidates[rnd.Next(topN)];
            bestX = chosen.X;
            bestY = chosen.Y;
        }
    }
}
