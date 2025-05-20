using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Piskvorky
{
    public class Calculations
    {
        private int boardSize = 15; // Velikost desky
        private GameSymbol[,] symbolsOnBoard; // Symboly na desce (x, y)
        private short winLength = 5; // Počet symbolů v řadě potřebný k výhře
        private short[,,,] symbolsInRow; // Počty symbolů v řadách ve všech směrech (x, y, směr, symbol)
        private short[,,,] openEnds; // Počet volných konců (možná prodloužení řady) (x, y, směr, symbol)
        private short[,] DirectionSigns; // Směrová znaménka pro horizontální, diagonální a vertikální (směr, souřadnice)
        private int rowsLeftOnBoard; // Zbývající počet možných vítězných řad na desce
        private int[,,] fieldValues; // Hodnoty polí používané při výběru tahů v heuristických přístupech (x, y, symbol)
        private int[] Values; // Hodnoty na základě počtu symbolů v řadě (0, 0, 4, 20, 100, 500)
        private short winRowsCount = 0; // Počet nalezených vítězných řad
        private Random random = new Random(); // Instance pro generování náhodných čísel

        /// <summary>
        /// Konstruktor, který inicializuje hlavní proměnné, nastaví velikost desky,
        /// směrová znaménka a hodnoty podle délky řady.
        /// </summary>
        /// <param name="boardSize">Velikost desky</param>
        public Calculations(int boardSize)
        {
            this.boardSize = boardSize;
            DirectionSigns = new short[(short)Direction.Diag2 + 1, (short)Coordinate.Y + 1]
            {
                { -1,  0 },  // horizontálně
                { -1, -1 },  // diagonálně (směr 1)
                {  0, -1 },  // vertikálně
                {  1, -1 }   // diagonálně (směr 2)
            };
            Values = new int[7] { 0, 0, 4, 20, 100, 500, 0 };
        }

        /// <summary>
        /// Nastaví novou velikost desky a vymaže desku i počítadla řad.
        /// </summary>
        /// <param name="newSize">Nová velikost desky</param>
        public void SetBoardSize(int newSize)
        {
            boardSize = newSize;
            ClearBoard();
            ClearSymbolsInRow();
        }

        /// <summary>
        /// Vlastnost pro získání či nastavení počtu symbolů potřebných k výhře.
        /// </summary>
        public short WinLength
        {
            get { return winLength; }
            set { winLength = value; }
        }

        /// <summary>
        /// Vrací počet nalezených vítězných řad.
        /// </summary>
        public short GetWinRowsCount
        {
            get { return winRowsCount; }
        }

        /// <summary>
        /// Vrací počty symbolů v řadách ve všech směrech.
        /// </summary>
        public short[,,,] SymbolsInRow
        {
            get
            {
                if (symbolsInRow == null) ClearSymbolsInRow();
                return symbolsInRow;
            }
        }

        /// <summary>
        /// Vrací aktuální rozmístění symbolů na desce.
        /// </summary>
        public GameSymbol[,] SymbolsOnBoard
        {
            get
            {
                if (symbolsOnBoard == null) ClearBoard();
                return symbolsOnBoard;
            }
        }

        /// <summary>
        /// Vrací hodnoty polí pro heuristické vyhodnocení tahů.
        /// </summary>
        public int[,,] FieldValues
        {
            get
            {
                if (fieldValues == null) ClearFieldValues();
                return fieldValues;
            }
        }

        /// <summary>
        /// Vymaže pole s počty symbolů v řadě a volné konce a inicializuje počet možných vítězných řad.
        /// </summary>
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

        /// <summary>
        /// Nastaví všechna pole na volné.
        /// </summary>
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

        /// <summary>
        /// nastavení všechny hodnoty na nulu.
        /// </summary>
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

        /// <summary>
        /// Ověří, zda souřadnice (x, y) leží na herní desce.
        /// </summary>
        public bool CordsOnBoard(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        /// <summary>
        /// Vrací symbol soupeře na základě symbolu aktuálního hráče.
        /// </summary>
        /// <param name="currentPlayer">Symbol aktuálního hráče</param>
        /// <returns>Symbol soupeře</returns>
        private GameSymbol GetOpponent(GameSymbol currentPlayer)
        {
            if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
            if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
            throw new Exception("Neplatný symbol hráče!");
        }

        /// <summary>
        /// Vrací seznam bodů tvořících vítěznou řadu v daném směru.
        /// Posune se zpět na začátek řady a poté postupuje směrem ke konci.
        /// </summary>
        private List<Point> GetWinningRow(int x, int y, Direction dir, GameSymbol player)
        {
            List<Point> winRow = new List<Point>();

            short signX = DirectionSigns[(short)dir, (short)Coordinate.X];
            short signY = DirectionSigns[(short)dir, (short)Coordinate.Y];

            while (CordsOnBoard(x - signX, y - signY) && SymbolsOnBoard[x - signX, y - signY] == player)
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

        /// <summary>
        /// Vrací všechny vítězné řady (seznam seznamů bodů), které obsahují alespoň požadovaný počet symbolů.
        /// </summary>
        private List<List<Point>> GetWinningRows(int x, int y, GameSymbol player)
        {
            List<List<Point>> winRows = new List<List<Point>>();

            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {
                List<Point> currentWinRow = GetWinningRow(x, y, dir, player);
                if (currentWinRow.Count >= winLength)
                {
                    winRows.Add(currentWinRow);
                }
            }

            return winRows;
        }

        /// <summary>
        /// Přidá symbol hráče na pozici (x, y), aktualizuje interní počítadla,
        /// zkontroluje případné vítězství a aktualizuje heuristické hodnoty polí.
        /// </summary>
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

            SymbolsOnBoard[x, y] = player;
            if (result == GameResult.Continue && rowsLeftOnBoard <= 0)
                result = GameResult.Draw;

            return result;
        }

        /// <summary>
        /// Ověří, zda je pozice (posX, posY) v rámci desky pro daný směr.
        /// </summary>
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

        /// <summary>
        /// Zvýší počet symbolů v řadě; pokud dosáhne požadované délky, vrátí výhru.
        /// Při prvním symbolu sníží počet zbývajících vítězných řad.
        /// </summary>
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
        /// Přepočítá heuristickou hodnotu pole na základě aktuálních řad symbolů,
        /// volných konců a přidá hodnotu pro aktuálního hráče či upraví hodnotu soupeře.
        /// </summary>
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

        /// <summary>
        /// Veřejná metoda, která na základě zvolené obtížnosti určí nejlepší tah pro hráče.
        /// </summary>
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

        /// <summary>
        /// Metoda pro snadnou obtížnost. Používá náhodu, kontroluje okamžité vítězství či blokování
        /// a volí suboptimální tah.
        /// </summary>
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

        /// <summary>
        /// Metoda pro střední obtížnost. Nejprve kontroluje vítězné tahy a poté vybírá tah na základě
        /// heuristické hodnoty polí.
        /// </summary>
        public void GetBestMove_Medium(out int x, out int y, GameSymbol player)
        {
            if (TryFindWinningMove(player, out x, out y))
                return;
            GameSymbol opponent = GetOpponent(player);
            if (TryFindWinningMove(opponent, out x, out y))
                return;
            PickMoveByFieldValue_Medium(out x, out y, player, opponent);
        }

        /// <summary>
        /// Metoda pro těžkou obtížnost. Pokud není okamžité vítězství či blok,
        /// hledá nejlepší tah pomocí iterativního prohledávání (Minimax).
        /// </summary>
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

        /// <summary>
        /// Hledá okamžitě vítězný tah pro zadaného hráče.
        /// Pro každý volný bod vyzkouší tah a ověří, zda vede k výhře.
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

        /// <summary>
        /// Ověří, zda tah hráče na pozici (x, y) vede k výhře.
        /// Pro každé směry sečte souvislé symboly.
        /// </summary>
        private bool WouldThisMoveWin(int x, int y, GameSymbol checkPlayer)
        {
            int[][] directions = new int[][]
            {
                new int[]{1, 0},    // horizontálně
                new int[]{0, 1},    // vertikálně
                new int[]{1, 1},    // diagonálně "\"
                new int[]{1, -1}    // diagonálně "/"
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

        /// <summary>
        /// Spočítá počet sousedních symbolů hráče ve zvoleném směru.
        /// </summary>
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

        /// <summary>
        /// Hledá tah, který blokuje potenciálně nebezpečnou situaci (otevřenou řadu tří nebo čtyř symbolů) soupeře.
        /// </summary>
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

        /// <summary>
        /// Ověří, zda umístění symbolu vytváří otevřenou řadu tří nebo čtyř symbolů,
        /// tedy situaci s možností prodloužení řady volnými konci.
        /// </summary>
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

        /// <summary>
        /// Zjistí, zda je pole na dané pozici volné (možnost prodloužení řady).
        /// </summary>
        private bool IsOpenEnd(int startX, int startY, int dx, int dy)
        {
            int x = startX + dx;
            int y = startY + dy;
            if (!CordsOnBoard(x, y))
                return false;
            return (SymbolsOnBoard[x, y] == GameSymbol.Free);
        }

        /// <summary>
        /// Vrací bonusovou hodnotu podle vzdálenosti od středu desky (Manhattanská vzdálenost).
        /// </summary>
        private int GetCenterBonus(int row, int col)
        {
            int center = boardSize / 2;
            int dx = Math.Abs(row - center);
            int dy = Math.Abs(col - center);
            int dist = dx + dy;
            int radius = center * 2;
            int bonus = radius - dist;
            return bonus < 0 ? 0 : bonus;
        }

        /// <summary>
        /// Vybere tah pro střední obtížnost na základě heuristické hodnoty polí.
        /// Prohledá desku a zvolí volné pole s nejvyšším skóre.
        /// </summary>
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

        /// <summary>
        /// Vybere suboptimální tah na základě heuristické hodnoty polí (používá se pro snadnou obtížnost).
        /// Provede výběr z topN kandidátů.
        /// </summary>
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

        /// <summary>
        /// Prohledávání stromu tahů pomocí algoritmu Minimax s alfa-beta ořezáváním.
        /// </summary>
        /// <param name="depth">Aktuální hloubka hledání.</param>
        /// <param name="maxDepth">Maximální hloubka hledání.</param>
        /// <param name="currentPlayer">Hráč, který provádí tah v aktuálním uzlu.</param>
        /// <param name="maximizingPlayer">Hráč, pro kterého hledáme nejlepší tah.</param>
        /// <param name="alpha">Hodnota alfa pro ořezávání.</param>
        /// <param name="beta">Hodnota beta pro ořezávání.</param>
        /// <param name="bestMove">Nejlepší nalezený tah v daném uzlu.</param>
        /// <returns>Hodnocení větve.</returns>
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
                    int score = Minimax(depth + 1, maxDepth, GetOpponent(currentPlayer), 
                        maximizingPlayer, alpha, beta, out _);
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
                    int score = Minimax(depth + 1, maxDepth, GetOpponent(currentPlayer), 
                        maximizingPlayer, alpha, beta, out _);
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

        /// <summary>
        /// Promíchá seznam prvků pomocí algoritmu Fisher–Yates.
        /// </summary>
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
        /// Vrací seznam kandidátních tahů – volná pole sousedící s již obsazeným polem.
        /// Nejprve se určí ohraničující obdélník obsazených polí, přidá se okraj a kandidáti se promíchají.
        /// </summary>
        private List<(int x, int y)> GetCandidateMoves()
        {
            var candidates = new List<(int, int)>();

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

            int margin = 6;
            int startX = Math.Max(0, minX - margin);
            int endX = Math.Min(boardSize - 1, maxX + margin);
            int startY = Math.Max(0, minY - margin);
            int endY = Math.Min(boardSize - 1, maxY + margin);

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
        /// Pokročilé vyhodnocení desky: prohledá desku pro sekvence (řady) symbolů a spočítá skóre podle délky řady a počtu volných konců.
        /// </summary>
        private int EvaluateBoardAdvanced(GameSymbol player)
        {
            int score = 0;
            int[][] directions = new int[][]
            {
                new int[]{1, 0},    // horizontálně
                new int[]{0, 1},    // vertikálně
                new int[]{1, 1},    // diagonálně "\"
                new int[]{1, -1}    // diagonálně "/"
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
                                continue; // počítá pouze od začátku řady

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
        /// Vyhodnocení řady: na základě počtu souvislých symbolů a volných konců vrací skóre.
        /// Pokud řada dosáhne potřebné délky, vrací okamžitou výhru.
        /// </summary>
        private int EvaluateLine(int count, int openEnds)
        {
            if (count >= winLength)
                return 100000;
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
        /// Celkové vyhodnocení desky pro algoritmus minimax: rozdíl mezi skóre hráče a soupeře.
        /// </summary>
        private int EvaluateBoard(GameSymbol maximizingPlayer)
        {
            GameSymbol opponent = GetOpponent(maximizingPlayer);
            int myScore = EvaluateBoardAdvanced(maximizingPlayer);
            int opponentScore = EvaluateBoardAdvanced(opponent);
            return myScore - opponentScore;
        }

        /// <summary>
        /// Iterativní prohledávání: opakovaně volá algoritmus Minimax s narůstající hloubkou,
        /// dokud nevyprší časový limit, a vrací nejlepší nalezený tah.
        /// </summary>
        /// <param name="player">Hráč, pro kterého se tah hledá.</param>
        /// <param name="timeLimitMs">Časový limit v milisekundách.</param>
        /// <returns>Nejlepší nalezený tah.</returns>
        private (int x, int y) FindBestMoveIterative(GameSymbol player, int timeLimitMs)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            (int bestX, int bestY) bestMove = (-1, -1);
            int depth = 1;
            int lastCompletedScore = int.MinValue;

            while (stopwatch.ElapsedMilliseconds < timeLimitMs)
            {
                int score = Minimax(0, depth, player, player, int.MinValue, int.MaxValue, out (int x, int y) currentBest);
                if (stopwatch.ElapsedMilliseconds < timeLimitMs)
                {
                    lastCompletedScore = score;
                    bestMove = currentBest;
                }
                depth++;
            }
            return bestMove;
        }
    }
}
