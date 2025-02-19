using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piskvorky
{
    public class SaveData
    {
        public int BoardSize { get; set; }
        public short WinLength { get; set; }
        public int NextPlayer { get; set; }
        public int GamesPlayed { get; set; }
        public int PlayerWins { get; set; }
        public int PlayerLosses { get; set; }
        public int PlayerDraws { get; set; }
        public string ScoreText { get; set; }
        public bool IsAI { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public string AIDiff { get; set; }
        public int MovesCount { get; set; }
        public List<string> BoardRows { get; set; }
    }
}

