using System;
using System.Windows.Forms;

namespace StreetFighterGame
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            new ChonNhanVat(false).Show();
            Hide();
        }

        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Application.Exit();
        }

        private void buttonRanking_Click(object sender, EventArgs e)
        {
            new FormRanking().Show();
            Hide();
        }

        private void buttonLichSuDau_Click(object sender, EventArgs e)
        {
            new FormLichSuDau().Show();
            Hide();
        }

        private void buttonOption_Click(object sender, EventArgs e)
        {
            new FormThanhVien().Show();
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ChonNhanVat(true).Show();
            Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new Huongdan().Show();
            Hide();
        }

        private void FormStart_Load(object sender, EventArgs e)
        {
        }
    }
}
