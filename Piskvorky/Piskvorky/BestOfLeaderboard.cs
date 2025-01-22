using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    public class BestOfLeaderboard
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public int BestWinMoves { get; set; }
        public double WinPercentage { get; set; }

        public override string ToString()
        {
            return $"{PlayerName} - Score: {Score}, Wins: {Wins}, Losses: {Losses}, Draws: {Draws}, Best Moves: {BestWinMoves}, Win%: {WinPercentage:F2}";
        }
    }
}
