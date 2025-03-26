using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Piskvorky
{
    public class Calculations
    {
        private int boardSize = 15; // Board size (e.g., 15x15)
        private GameSymbol[,] symbolsOnBoard; // Symbols on the board
        private short winLength = 5; // Number of symbols in a row needed to win
        private short[,,,] symbolsInRow; // Counts of symbols in each direction
        private short[,,,] openEnds; // Number of open ends (potential extensions of a sequence)
        private short[,] DirectionSigns; // Direction signs for horizontal, diagonal, vertical
        private int rowsLeftOnBoard; // Remaining possible winning rows on the board
        private int[,,] fieldValues; // Field values used to choose moves in heuristic approaches
        private int[] Values; // Values based on number of symbols in a row (e.g., 4, 20, 100, 500)
        private short winRowsCount = 0;
        private Random random = new Random();

        // Constructor: initializes key variables
        public Calculations(int boardSize)
        {
            this.boardSize = boardSize;

            // Directions: horizontal, diagonal1, vertical, diagonal2
            DirectionSigns = new short[(short)Direction.Diag2 + 1, (short)Coordinate.Y + 1]
            {
                { -1,  0 }, // Horizontal
                { -1, -1 }, // Diagonal 1
                {  0, -1 }, // Vertical
                {  1, -1 }  // Diagonal 2
            };

            // Values according to sequence length: the longer the sequence, the higher the value.
            Values = new int[7] { 0, 0, 4, 20, 100, 500, 0 };
        }

        // Sets a new board size and clears the board and sequence counts
        public void SetBoardSize(int newSize)
        {
            
            boardSize = newSize;
            ClearBoard();
            ClearSymbolsInRow();
        }

        // The win length needed
        public short WinLength
        {
            get { return winLength; }
            set { winLength = value; }
        }

        public short GetWinRowsCount
        {
            get { return winRowsCount; }
        }

        // Counts of symbols in each direction
        public short[,,,] SymbolsInRow
        {
            get
            {
                if (symbolsInRow == null) ClearSymbolsInRow();
                return symbolsInRow;
            }
        }

        // The board’s symbols
        public GameSymbol[,] SymbolsOnBoard
        {
            get
            {
                if (symbolsOnBoard == null) ClearBoard();
                return symbolsOnBoard;
            }
        }

        // Field values for heuristic evaluations
        public int[,,] FieldValues
        {
            get
            {
                if (fieldValues == null) ClearFieldValues();
                return fieldValues;
            }
        }

        // Clears the sequence counts arrays
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
            // Total possible winning sequences on the board
            rowsLeftOnBoard = 4 * (2 * boardSize - (winLength - 1)) * (boardSize - (winLength - 1));
        }

        // Clears the board (sets all cells to Free)
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

        // Clears the field values array (all to 0)
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

        // Checks whether coordinates (x,y) are on the board
        public bool CordsOnBoard(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        // Returns the opponent's symbol
        private GameSymbol GetOpponent(GameSymbol currentPlayer)
        {
            if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
            if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
            throw new Exception("Invalid player symbol!");
        }

        //Stores the winning row
        private List<Point> GetWinningRow(int x, int y, Direction dir, GameSymbol player)
        {
            List<Point> winRow = new List<Point>();

            short signX = DirectionSigns[(short)dir, (short)Coordinate.X];
            short signY = DirectionSigns[(short)dir, (short)Coordinate.Y];

            while (CordsOnBoard(x - signX, y - signY) && SymbolsOnBoard[x - signX, y-signY] == player)
            {
                x -= signX;
                y -= signY;
            }
            while (CordsOnBoard(x, y) && SymbolsOnBoard[x, y] == player)
            {
                winRow.Add(new Point(x, y));
                x += signX;
                y += signY;
            }

            return winRow;
        }

        private List<List<Point>> GetWinningRows(int x, int y, GameSymbol player)
        {
            List<List<Point>> winRows = new List<List<Point>>();

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                List<Point> currentWinRow = GetWinningRow(x, y, dir, player);
                // Only add the row if it has at least winLength points.
                if (currentWinRow.Count >= winLength)
                {
                    winRows.Add(currentWinRow);
                }
            }

            return winRows;
        }


        // Adds a symbol on the board at (x,y) and updates internal counters
        public GameResult AddSymbol(int x, int y, GameSymbol player, out List<List<Point>> winRows)
        {

            winRows = new List<List<Point>>();
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
                            if (result == GameResult.Win)
                            {
                                SymbolsOnBoard[x, y] = player;
                                winRows = GetWinningRows(x, y, player);
                                winRowsCount = (short)winRows.Count;
                            }
                                
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

            SymbolsOnBoard[x, y] = player; // Update the board
            if (result == GameResult.Continue && rowsLeftOnBoard <= 0)
                result = GameResult.Draw; // Draw if no moves left

            return result;
        }

        // Checks if position (posX, posY) is within bounds for a given direction
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

        // Increments the count of symbols in a row; returns Win if the sequence reaches winLength
        private GameResult IncludeDraw(ref short numberInRow)
        {
            numberInRow++;
            if (numberInRow == winLength)
                return GameResult.Win;
            if (numberInRow == 1)
                rowsLeftOnBoard--;
            return GameResult.Continue;
        }

        // Recalculates the heuristic value of a field based on current sequences
        private void RecalcValue(
            short symbolsInRowCurrentPlayer,
            short symbolsInRowOpponent,
            short openEndsCurrentPlayer,
            short openEndsOpponent,
            ref int fieldValueForCurrentPlayer,
            ref int fieldValueForOpponent)
        {
            if (symbolsInRowOpponent == 0)
            {
                int baseIncrement = Values[symbolsInRowCurrentPlayer + 1] - Values[symbolsInRowCurrentPlayer];
                if (openEndsCurrentPlayer == 0)
                {
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
            else if (symbolsInRowCurrentPlayer == 1)
            {
                fieldValueForOpponent -= Values[symbolsInRowOpponent];
            }
        }

        // Public interface to get the best move according to difficulty
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

        // Easy difficulty: uses chance and immediate win/block checks plus a suboptimal move
        public void GetBestMove_Easy(out int x, out int y, GameSymbol player)
        {
            Random random = new Random();
            int chance = random.Next(0, 100);
            GameSymbol opponent = GetOpponent(player);
            if (chance < 80)
            {
                if (TryFindWinningMove(player, out x, out y))
                    return;
                if (TryFindWinningMove(opponent, out x, out y))
                    return;
                if (TryFindOpenThreeOrFour(opponent, out x, out y))
                    return;
            }
            PickMoveSuboptimalByFieldValue(out x, out y, player, opponent, topN: 3);
        }

        // Medium difficulty: immediate win/block then a heuristic field value move
        public void GetBestMove_Medium(out int x, out int y, GameSymbol player)
        {
            if (TryFindWinningMove(player, out x, out y))
                return;
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;
            PickMoveByFieldValue_Medium(out x, out y, player, opponent);
        }

        public void GetBestMove_Hard(out int x, out int y, GameSymbol player)
        {
            if (TryFindWinningMove(player, out x, out y))
                return;
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;

            (int bestX, int bestY) = FindBestMoveIterative(player, 200);
            x = bestX;
            y = bestY;
        }

        // Searches for an immediate winning move for checkPlayer
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

        // Checks whether placing checkPlayer's symbol at (x,y) wins the game
        private bool WouldThisMoveWin(int x, int y, GameSymbol checkPlayer)
        {
            int[][] directions = new int[][]
            {
                new int[]{1, 0}, // Horizontal
                new int[]{0, 1}, // Vertical
                new int[]{1, 1}, // Diagonal \
                new int[]{1, -1} // Diagonal /
            };
            foreach (var d in directions)
            {
                int count = 1;
                count += CountDirection(x, y, d[0], d[1], checkPlayer);
                count += CountDirection(x, y, -d[0], -d[1], checkPlayer);
                if (count >= winLength)
                    return true;
            }
            return false;
        }

        // Counts consecutive symbols in a given direction
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

        // Tries to find a move that blocks an open three or four for the opponent
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

        // Checks whether placing a symbol creates an open three or four
        private bool IsOpenThreeOrFour(int placedX, int placedY, GameSymbol symbol)
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
                int leftCount = CountDirection(placedX, placedY, -d[0], -d[1], symbol);
                int rightCount = CountDirection(placedX, placedY, d[0], d[1], symbol);
                count += leftCount + rightCount;
                bool leftOpen = IsOpenEnd(placedX, placedY, -d[0], -d[1]);
                bool rightOpen = IsOpenEnd(placedX, placedY, d[0], d[1]);
                int openEnds = (leftOpen ? 1 : 0) + (rightOpen ? 1 : 0);
                if (count == 4 && openEnds >= 1)
                    return true;
                if (count == 3 && openEnds == 2)
                    return true;
            }
            return false;
        }

        // Checks whether the cell at (startX+dx, startY+dy) is open
        private bool IsOpenEnd(int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;
            if (!CordsOnBoard(x, y))
                return false;
            return (SymbolsOnBoard[x, y] == GameSymbol.Free);
        }

        // Returns a bonus based on closeness to the board center
        private int GetCenterBonus(int row, int col)
        {
            int center = boardSize / 2;
            int dx = Math.Abs(row - center);
            int dy = Math.Abs(col - center);
            int dist = dx + dy; // Manhattan distance
            int radius = center * 2;
            int bonus = radius - dist;
            return bonus < 0 ? 0 : bonus;
        }

        // Picks a move for medium difficulty using field values
        private void PickMoveByFieldValue_Medium(out int bestX, out int bestY, GameSymbol player, GameSymbol opponent)
        {
            int bestValue = int.MinValue;
            bestX = boardSize / 2;
            bestY = boardSize / 2;
            if (SymbolsOnBoard[bestX, bestY] == GameSymbol.Free)
                bestValue = 4;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        int value = (FieldValues[i, j, (short)player] * 16)
                                    + 1
                                    + (FieldValues[i, j, (short)opponent] * 32)
                                    + GetCenterBonus(i, j);
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

        // Picks a suboptimal move based on field values (used for easy mode)
        private void PickMoveSuboptimalByFieldValue(
            out int bestX,
            out int bestY,
            GameSymbol player,
            GameSymbol opponent,
            int topN = 3)
        {
            var candidates = new List<(int X, int Y, int Score)>();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        int value = (FieldValues[i, j, (short)player] * 16)
                                    + 1
                                    + (FieldValues[i, j, (short)opponent] * 4)
                                    + GetCenterBonus(i, j);
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
            candidates.Sort((a, b) => b.Score.CompareTo(a.Score));
            if (topN > candidates.Count)
                topN = candidates.Count;
            Random rnd = new Random();
            var chosen = candidates[rnd.Next(topN)];
            bestX = chosen.X;
            bestY = chosen.Y;
        }

        // ------------------ Minimax with Alpha–Beta Pruning and Iterative Deepening ------------------

        /// <summary>
        /// Minimax search with alpha-beta pruning.
        /// </summary>
        /// <param name="depth">Current depth.</param>
        /// <param name="maxDepth">Maximum depth to search.</param>
        /// <param name="currentPlayer">The player making the move in this call.</param>
        /// <param name="maximizingPlayer">The player for whom we are optimizing.</param>
        /// <param name="alpha">Alpha value for pruning.</param>
        /// <param name="beta">Beta value for pruning.</param>
        /// <param name="bestMove">Best move found at this node.</param>
        /// <returns>The evaluated score for this branch.</returns>
        
        private int Minimax(int depth, int maxDepth, GameSymbol currentPlayer, 
                            GameSymbol maximizingPlayer, int alpha, int beta, out (int x, int y) bestMove)
        {
            bestMove = (-1, -1);

            bool isFull = true;
            for (int i = 0; i < boardSize && isFull; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        isFull = false;
                        break;
                    }
                }
            }
            if (isFull) return EvaluateBoard(maximizingPlayer);
            if (depth == maxDepth) return EvaluateBoard(maximizingPlayer);

            List<(int x, int y)> candidates = GetCandidateMoves();
            if (candidates.Count == 0)
                return EvaluateBoard(maximizingPlayer);

            int bestScore;

            if (currentPlayer == maximizingPlayer)
            {
                bestScore = int.MinValue;
                foreach (var move in candidates)
                {
                    SymbolsOnBoard[move.x, move.y] = currentPlayer;
                    if (WouldThisMoveWin(move.x, move.y, currentPlayer))
                    {
                        SymbolsOnBoard[move.x, move.y] = GameSymbol.Free;
                        bestMove = move;
                        return 10000 - depth;
                    }
                    int score = Minimax(depth + 1, maxDepth, GetOpponent(currentPlayer), maximizingPlayer, alpha, beta, out _);
                    SymbolsOnBoard[move.x, move.y] = GameSymbol.Free;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha)
                        break;
                }
            }
            else
            {
                bestScore = int.MaxValue;
                foreach (var move in candidates)
                {
                    SymbolsOnBoard[move.x, move.y] = currentPlayer;
                    if (WouldThisMoveWin(move.x, move.y, currentPlayer))
                    {
                        SymbolsOnBoard[move.x, move.y] = GameSymbol.Free;
                        bestMove = move;
                        return -10000 + depth;
                    }
                    int score = Minimax(depth + 1, maxDepth, GetOpponent(currentPlayer), maximizingPlayer, alpha, beta, out _);
                    SymbolsOnBoard[move.x, move.y] = GameSymbol.Free;
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove = move;
                    }
                    beta = Math.Min(beta, score);
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// Returns a list of candidate moves – empty cells adjacent to an occupied cell.
        /// </summary>
        private List<(int x, int y)> GetCandidateMoves()
        {
            var candidates = new List<(int, int)>();

            // Determine the bounding box of all occupied cells.
            int minX = boardSize, maxX = -1, minY = boardSize, maxY = -1;
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] != GameSymbol.Free)
                    {
                        if (i < minX) minX = i;
                        if (i > maxX) maxX = i;
                        if (j < minY) minY = j;
                        if (j > maxY) maxY = j;
                    }
                }
            }

            // If the board is empty, return all free cells.
            if (maxX == -1)
            {
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                            candidates.Add((i, j));
                    }
                }
                return candidates;
            }

            // Add a margin (adjustable) around the occupied area.
            int margin = 6; // You can tweak this value
            int startX = Math.Max(0, minX - margin);
            int endX = Math.Min(boardSize - 1, maxX + margin);
            int startY = Math.Max(0, minY - margin);
            int endY = Math.Min(boardSize - 1, maxY + margin);

            // Iterate only within the bounding box.
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                    {
                        bool adjacent = false;
                        for (int dx = -1; dx <= 1 && !adjacent; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0)
                                    continue;
                                int nx = i + dx, ny = j + dy;
                                if (CordsOnBoard(nx, ny) && SymbolsOnBoard[nx, ny] != GameSymbol.Free)
                                {
                                    adjacent = true;
                                    break;
                                }
                            }
                        }
                        if (adjacent)
                            candidates.Add((i, j));
                    }
                }
            }

            // Fallback: if no candidate is found in the box, add all free cells in the bounding box.
            if (candidates.Count == 0)
            {
                for (int i = startX; i <= endX; i++)
                {
                    for (int j = startY; j <= endY; j++)
                    {
                        if (SymbolsOnBoard[i, j] == GameSymbol.Free)
                            candidates.Add((i, j));
                    }
                }
            }

            Shuffle(candidates);

            return candidates;
        }


        /// <summary>
        /// Advanced board evaluation: scans the board for sequences (lines) and computes a score.
        /// </summary>
        private int EvaluateBoardAdvanced(GameSymbol player)
        {
            int score = 0;
            int[][] directions = new int[][]
            {
                new int[]{1, 0},   // Horizontal
                new int[]{0, 1},   // Vertical
                new int[]{1, 1},   // Diagonal \
                new int[]{1, -1}   // Diagonal /
            };

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (SymbolsOnBoard[i, j] == player)
                    {
                        foreach (var d in directions)
                        {
                            int prevX = i - d[0], prevY = j - d[1];
                            if (CordsOnBoard(prevX, prevY) && SymbolsOnBoard[prevX, prevY] == player)
                                continue; // Only count from the beginning of the line

                            int count = 0;
                            int x = i, y = j;
                            while (CordsOnBoard(x, y) && SymbolsOnBoard[x, y] == player)
                            {
                                count++;
                                x += d[0];
                                y += d[1];
                            }
                            int openEnds = 0;
                            if (CordsOnBoard(i - d[0], j - d[1]) && SymbolsOnBoard[i - d[0], j - d[1]] == GameSymbol.Free)
                                openEnds++;
                            if (CordsOnBoard(x, y) && SymbolsOnBoard[x, y] == GameSymbol.Free)
                                openEnds++;

                            score += EvaluateLine(count, openEnds);
                        }
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// Returns a score for a line with a given count of consecutive symbols and number of open ends.
        /// </summary>
        private int EvaluateLine(int count, int openEnds)
        {
            if (count >= winLength)
                return 100000; // Immediate win
            if (count == 4)
            {
                if (openEnds == 2) return 10000;
                else if (openEnds == 1) return 1000;
            }
            if (count == 3)
            {
                if (openEnds == 2) return 1000;
                else if (openEnds == 1) return 100;
            }
            if (count == 2)
            {
                if (openEnds == 2) return 100;
                else if (openEnds == 1) return 10;
            }
            if (count == 1)
            {
                return 10;
            }
            return 0;
        }

        /// <summary>
        /// Overall board evaluation for minimax: difference between AI and opponent scores.
        /// </summary>
        private int EvaluateBoard(GameSymbol maximizingPlayer)
        {
            GameSymbol opponent = GetOpponent(maximizingPlayer);
            int myScore = EvaluateBoardAdvanced(maximizingPlayer);
            int opponentScore = EvaluateBoardAdvanced(opponent);
            return myScore - opponentScore;
        }

        /// <summary>
        /// Iterative deepening search: repeatedly calls minimax with increasing depth until time runs out.
        /// </summary>
        /// <param name="player">The player for whom to find the move.</param>
        /// <param name="timeLimitMs">Time limit in milliseconds.</param>
        /// <returns>The best move found within the time limit.</returns>
        private (int x, int y) FindBestMoveIterative(GameSymbol player, int timeLimitMs)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (int bestX, int bestY) bestMove = (-1, -1);
            int depth = 1;
            int lastCompletedScore = int.MinValue;

            // Iteratively deepen until time runs out.
            while (stopwatch.ElapsedMilliseconds < timeLimitMs)
            {
                int score = Minimax(0, depth, player, player, int.MinValue, int.MaxValue, out (int x, int y) currentBest);
                // Only update best move if the full search at this depth finished within time.
                if (stopwatch.ElapsedMilliseconds < timeLimitMs)
                {
                    lastCompletedScore = score;
                    bestMove = currentBest;
                }

                    depth++;
                
                
            }
            return bestMove;
        }
        // -------------------------------------------------------------------------------------
    }
}
