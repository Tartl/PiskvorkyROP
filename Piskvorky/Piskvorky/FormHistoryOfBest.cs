using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Piskvorky
{
    public partial class FormHistoryOfBest : Form
    {
        private readonly string leaderboardFilePath = Path.Combine(Application.StartupPath, "leaderboard.xml");
        private List<BestOfLeaderboard> leaderboard;

        public FormHistoryOfBest()
        {
            InitializeComponent();
            leaderboard = LoadLeaderboardBase64(leaderboardFilePath);
            InitializeLeaderboardGridView();
        }
        public List<BestOfLeaderboard> LoadLeaderboardBase64(string filePath)
        {
            if (!File.Exists(filePath)) return new List<BestOfLeaderboard>();

            string encodedContent = File.ReadAllText(filePath);
            string xmlContent = Encoding.UTF8.GetString(Convert.FromBase64String(encodedContent));

            XmlSerializer serializer = new XmlSerializer(typeof(List<BestOfLeaderboard>));
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                return (List<BestOfLeaderboard>)serializer.Deserialize(stringReader);
            }
        }

        private void InitializeLeaderboardGridView()
        {
            leaderboardGridView.DataSource = null;
            leaderboardGridView.DataSource = leaderboard;

            leaderboardGridView.Columns["PlayerName"].HeaderText = "Hráč";
            leaderboardGridView.Columns["Score"].HeaderText = "Skóre";
            leaderboardGridView.Columns["Wins"].HeaderText = "Výhry";
            leaderboardGridView.Columns["Losses"].HeaderText = "Prohry";
            leaderboardGridView.Columns["Draws"].HeaderText = "Remízy";
            leaderboardGridView.Columns["BestWinMoves"].HeaderText = "Nejméně tahů k výhře";
            leaderboardGridView.Columns["WinPercentage"].HeaderText = "Výhry %";

            leaderboardGridView.Columns["PlayerName"].Width = 150;
            leaderboardGridView.Columns["Score"].Width = 100;
            leaderboardGridView.Columns["Wins"].Width = 80;
            leaderboardGridView.Columns["Losses"].Width = 80;
            leaderboardGridView.Columns["Draws"].Width = 80;
            leaderboardGridView.Columns["BestWinMoves"].Width = 150;
            leaderboardGridView.Columns["WinPercentage"].Width = 100;

            leaderboardGridView.Columns["WinPercentage"].DefaultCellStyle.Format = "F2";
        }
    }
}

