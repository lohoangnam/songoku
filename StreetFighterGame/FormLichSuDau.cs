using System;
using System.Windows.Forms;
using StreetFighterGame.QuanLiTaikhoan;

namespace StreetFighterGame
{
    public partial class FormLichSuDau : Form
    {
        public FormLichSuDau()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void FormLichSuDau_Load(object sender, EventArgs e)
        {
            QuanLiTaiKhoan.HienThiLichSuTranDau(dataGridView1);
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = QuanLiTaiKhoan.TimKiemLichSu(textBoxKeyWord.Text);
        }

        private void buttonLamMoi_Click(object sender, EventArgs e)
        {
            QuanLiTaiKhoan.HienThiLichSuTranDau(dataGridView1);
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
    }
}
