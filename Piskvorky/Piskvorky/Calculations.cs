using System;
using System.Collections.Generic;

namespace Piskvorky
{
    public class Calculations
    {
        private int boardSize = 15; // Velikost hrací plochy (např. 15x15 políček)
        private GameSymbol[,] symbolsOnBoard; // Symboly na hrací ploše
        private short winLength = 5; // Počet symbolů v řadě potřebný k výhře
        private short[,,,] symbolsInRow; // Počty symbolů v jednotlivých směrech
        private short[,,,] openEnds; // Počet otevřených konců (možné pokračování v řadě)
        private short[,] DirectionSigns; // Značky určující směr (horizontální, diagonální, vertikální)
        private int rowsLeftOnBoard; // Počet zbývajících možných řad na ploše
        private int[,,] fieldValues; // Hodnoty polí pro výpočet nejlepších tahů
        private int[] Values; // Hodnoty podle počtu symbolů v řadě (např. 4, 20, 100, 500)

        // Konstruktor třídy, inicializace základních proměnných
        public Calculations(int boardSize)
        {
            this.boardSize = boardSize;

            // Směry: horizontální, vertikální, diagonální 1, diagonální 2
            DirectionSigns = new short[(short)Direction.Diag2 + 1, (short)Coordinate.Y + 1]
            {
                { -1,  0 }, // Horizontální
                { -1, -1 }, // Diagonální 1
                {  0, -1 }, // Vertikální
                {  1, -1 }  // Diagonální 2
            };

            // Hodnoty podle délky řady: čím delší řada, tím vyšší hodnota
            Values = new int[7] { 0, 0, 4, 20, 100, 500, 0 };
        }

        // Nastavení nové velikosti hrací plochy
        public void SetBoardSize(int newSize)
        {
            boardSize = newSize;
            ClearBoard(); // Vyčištění hrací plochy
            ClearSymbolsInRow(); // Vyčištění počtů symbolů v řadě
        }

        // Délka řady potřebná k výhře
        public short WinLength
        {
            get { return winLength; }
            set { winLength = value; }
        }

        // Počty symbolů v jednotlivých směrech
        public short[,,,] SymbolsInRow
        {
            get
            {
                if (symbolsInRow == null) ClearSymbolsInRow();
                return symbolsInRow;
            }
        }

        // Symboly na hrací ploše
        public GameSymbol[,] SymbolsOnBoard
        {
            get
            {
                if (symbolsOnBoard == null) ClearBoard();
                return symbolsOnBoard;
            }
        }

        // Hodnoty polí na hrací ploše
        public int[,,] FieldValues
        {
            get
            {
                if (fieldValues == null) ClearFieldValues();
                return fieldValues;
            }
        }

        // Vyčištění počtů symbolů v řadě
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

            // Počet možných řad na ploše
            rowsLeftOnBoard = 4 * (2 * boardSize - (winLength - 1)) * (boardSize - (winLength - 1));
        }

        // Vyčištění hrací plochy (nastavení všech políček na "volné")
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

        // Vyčištění hodnot polí (nastavení všech hodnot na 0)
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

        // Kontrola, zda jsou souřadnice na hrací ploše
        public bool CordsOnBoard(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }

        // Vrací symbol soupeře aktuálního hráče
        private GameSymbol GetOpponent(GameSymbol currentPlayer)
        {
            if (currentPlayer == GameSymbol.Symbol1) return GameSymbol.Symbol2;
            if (currentPlayer == GameSymbol.Symbol2) return GameSymbol.Symbol1;
            throw new Exception("Neplatný symbol hráče!");
        }

        // Přidání symbolu na dané políčko hrací plochy
        public GameResult AddSymbol(int x, int y, GameSymbol player)
        {
            GameResult result = GameResult.Continue; // Výchozí stav hry
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

            SymbolsOnBoard[x, y] = player; // Aktualizace symbolu na ploše
            if (result == GameResult.Continue && rowsLeftOnBoard <= 0)
                result = GameResult.Draw; // Remíza, pokud nejsou tahy

            return result;
        }

        // Kontrola, zda je pozice v rámci hranic plochy
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

        // Zvýšení počtu symbolů v řadě, kontrola výhry/remízy
        private GameResult IncludeDraw(ref short numberInRow)
        {
            numberInRow++;
            if (numberInRow == winLength)
                return GameResult.Win; // Výhra, pokud je dosažena požadovaná délka řady

            if (numberInRow == 1)
                rowsLeftOnBoard--; // Snížení počtu zbývajících řad

            return GameResult.Continue;
        }

        // Přepočítání hodnoty políčka v závislosti na aktuálním stavu řady
        private void RecalcValue(
            short symbolsInRowCurrentPlayer,
            short symbolsInRowOpponent,
            short openEndsCurrentPlayer,
            short openEndsOpponent,
            ref int fieldValueForCurrentPlayer,
            ref int fieldValueForOpponent)
        {
            // Pokud soupeř nemá kameny v této řadě, přidáme "útočné" body
            if (symbolsInRowOpponent == 0)
            {
                int baseIncrement = Values[symbolsInRowCurrentPlayer + 1] - Values[symbolsInRowCurrentPlayer];

                if (openEndsCurrentPlayer == 0)
                {
                    // Pokud nejsou otevřené konce, bodový přírůstek se zmenší
                    baseIncrement /= 5;
                }
                else if (openEndsCurrentPlayer == 2)
                {
                    // Pokud jsou dva otevřené konce, bodový přírůstek se zdvojnásobí
                    baseIncrement *= 2;
                }
                else if (openEndsCurrentPlayer == 1)
                {
                    // Jeden otevřený konec přidá 50 % bodů navíc
                    baseIncrement += baseIncrement / 2;
                }

                fieldValueForCurrentPlayer += baseIncrement;
            }
            // Pokud máme přesně jeden symbol v řadě, snižujeme hodnotu soupeřova políčka
            else if (symbolsInRowCurrentPlayer == 1)
            {
                fieldValueForOpponent -= Values[symbolsInRowOpponent];
            }
        }

        // Získání nejlepšího tahu podle úrovně obtížnosti
        public void GetBestMove(Difficulty difficulty, out int x, out int y, GameSymbol player)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    GetBestMove_Easy(out x, out y, player); // Jednoduchá AI
                    break;
                case Difficulty.Medium:
                    GetBestMove_Medium(out x, out y, player); // Středně obtížná AI
                    break;
                case Difficulty.Hard:
                    GetBestMove_Hard(out x, out y, player); // Obtížná AI
                    break;
                default:
                    GetBestMove_Medium(out x, out y, player); // Výchozí střední obtížnost
                    break;
            }
        }

        // Nejlepší tah pro jednoduchou obtížnost
        public void GetBestMove_Easy(out int x, out int y, GameSymbol player)
        {
            Random random = new Random();
            int chance = random.Next(0, 100);

            GameSymbol opponent = GetOpponent(player); // Symbol soupeře

            // 80% šance na okamžitou výhru nebo blokování soupeře
            if (chance < 80)
            {
                // Pokud můžeme okamžitě vyhrát
                if (TryFindWinningMove(player, out x, out y))
                    return;

                // Pokud soupeř může vyhrát v dalším tahu, blokujeme
                if (TryFindWinningMove(opponent, out x, out y))
                    return;

                // Blokování otevřené trojky nebo čtyřky soupeře
                if (TryFindOpenThreeOrFour(opponent, out x, out y))
                    return;
            }

            // Pokud nic z výše uvedeného neplatí, vybereme suboptimální tah
            PickMoveSuboptimalByFieldValue(out x, out y, player, opponent, topN: 3);
        }

        // Nejlepší tah pro střední obtížnost
        public void GetBestMove_Medium(out int x, out int y, GameSymbol player)
        {
            if (TryFindWinningMove(player, out x, out y))
                return;

            GameSymbol opponent = GetOpponent(player); 

            if (TryFindWinningMove(opponent, out x, out y))
                return;


            PickMoveByFieldValue_Medium(out x, out y, player, opponent);
        }

        // Nejlepší tah pro obtížnou obtížnost
        public void GetBestMove_Hard(out int x, out int y, GameSymbol player)
        {
            if (TryFindWinningMove(player, out x, out y))
                return;

            GameSymbol opponent = GetOpponent(player); 

            if (TryFindWinningMove(opponent, out x, out y))
                return;

            PickMoveByFieldValue_Hard(out x, out y, player, opponent);
        }

        // Hledání tahu, který vede k okamžité výhře
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
                        SymbolsOnBoard[x, y] = checkPlayer; // Simulace tahu
                        bool isWinning = WouldThisMoveWin(x, y, checkPlayer); // Kontrola výhry
                        SymbolsOnBoard[x, y] = GameSymbol.Free; // Vrácení do původního stavu

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

        // Kontrola, zda tah vede k výhře
        private bool WouldThisMoveWin(int x, int y, GameSymbol checkPlayer)
        {
            int[][] directions = new int[][]
            {
                new int[]{1, 0}, // Horizontální
                new int[]{0, 1}, // Vertikální
                new int[]{1, 1}, // Diagonální 1
                new int[]{1, -1} // Diagonální 2
            };

            foreach (var d in directions)
            {
                int count = 1;
                count += CountDirection(x, y, d[0], d[1], checkPlayer); // Počítání symbolů v jednom směru
                count += CountDirection(x, y, -d[0], -d[1], checkPlayer); // Počítání symbolů v opačném směru

                if (count >= winLength) return true; // Pokud je dosažena délka řady
            }
            return false;
        }

        // Počítání symbolů v určitém směru
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

        private void PickMoveByFieldValue_Medium(out int bestX, out int bestY, GameSymbol player, GameSymbol opponent)
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
        private void PickMoveByFieldValue_Hard(out int bestX, out int bestY, GameSymbol player, GameSymbol opponent)
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
                            + (FieldValues[i, j, (short)opponent] * 16);

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
            int topN = 3)
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
                            + (FieldValues[i, j, (short)opponent] * 4);

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
