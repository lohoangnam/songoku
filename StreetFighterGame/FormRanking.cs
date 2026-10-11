using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StreetFighterGame
{
    public partial class FormRanking : Form
    {
        public FormRanking()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Load += FormRanking_Load;
        }

        private void FormRanking_Load(object sender, EventArgs e)
        {
            var statistics = CSVHelper.LoadThongKeNhanVat();
            var top3 = CSVHelper.LoadNhanVat()
                .Join(statistics, character => character.NhanVatId, statistic => statistic.NhanVatId, (character, statistic) => new TopCharacter
                {
                    Name = character.TenNhanVat,
                    Wins = statistic.SoTranThang
                })
                .OrderByDescending(x => x.Wins)
                .ThenBy(x => x.Name)
                .Take(3)
                .ToList();

            SetPlayer(lblTop1Name, lblTop1Wins, top3, 0);
            SetPlayer(lblTop2Name, lblTop2Wins, top3, 1);
            SetPlayer(lblTop3Name, lblTop3Wins, top3, 2);
        }

        private static void SetPlayer(Label name, Label wins, List<TopCharacter> characters, int index)
        {
            if (characters.Count <= index)
            {
                name.Text = "Không có";
                wins.Text = "0";
                return;
            }

            name.Text = characters[index].Name;
            wins.Text = characters[index].Wins.ToString();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            new FormStart().Show();
            Close();
        }

        private void buttonExit_Click_1(object sender, EventArgs e)
        {
            buttonExit_Click(sender, e);
        }

        private class TopCharacter
        {
            public string Name { get; set; }
            public int Wins { get; set; }
        }
    }
}
