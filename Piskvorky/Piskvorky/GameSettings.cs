using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class GameSettings
    {
        public static int WinLength { get; set; } = 5;
        public static int BoardSize { get; set; } = 15;
        public static int GameLength { get; set; } = 3;
        public static string Player1Symbol { get; set; } = "❌";
        public static string Player2Symbol { get; set; } = "⭕";
        public static bool IsAgainstAI { get; set; } = false;
        public static string AI_Difficulty { get; set; } = "střední";
        public static string Player1Name { get; set; } = "Hráč 1";
        public static string Player2Name { get; set; } = "Hráč 2";

    }
}
