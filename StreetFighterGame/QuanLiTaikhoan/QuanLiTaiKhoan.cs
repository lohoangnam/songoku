using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StreetFighterGame.QuanLiTaikhoan
{
    public class LichSuTranDauViewModel
    {
        public string NhanVatNguoiChoi { get; set; }
        public string NhanVatMay { get; set; }
        public string KetQua { get; set; }
        public string CheDo { get; set; }
        public DateTime? ThoiGianTranDau { get; set; }
    }

    public class TopPlayer
    {
        public string Username { get; set; }
        public int Wins { get; set; }
    }

    public static class QuanLiTaiKhoan
    {
        public static int GetNhanVatIdByName(string tenNhanVat)
        {
            var character = CSVHelper.LoadNhanVat().FirstOrDefault(x => string.Equals(x.TenNhanVat, tenNhanVat, StringComparison.OrdinalIgnoreCase));
            return character == null ? 0 : character.NhanVatId;
        }

        public static void LuuTranDau(int nhanVatNguoiChoiId, int nhanVatMayId, string ketQua, bool isVsAI)
        {
            if (!isVsAI) return;

            var matches = CSVHelper.LoadLichSuDau();
            var statistics = CSVHelper.LoadThongKeNhanVat();
            var statistic = statistics.FirstOrDefault(x => x.NhanVatId == nhanVatNguoiChoiId);

            if (statistic == null)
            {
                statistic = new ThongKeNhanVatModel { NhanVatId = nhanVatNguoiChoiId, SoTranThang = 0 };
                statistics.Add(statistic);
            }

            matches.Add(new LichSuDauModel
            {
                TranDauId = matches.Count == 0 ? 1 : matches.Max(x => x.TranDauId) + 1,
                NhanVatNguoiChoiId = nhanVatNguoiChoiId,
                NhanVatMayId = nhanVatMayId,
                KetQua = ketQua,
                CheDo = "VsComputer",
                ThoiGianTranDau = DateTime.Now
            });

            if (ketQua == "Win") statistic.SoTranThang++;

            CSVHelper.SaveLichSuDau(matches);
            CSVHelper.SaveThongKeNhanVat(statistics);
        }

        public static List<LichSuTranDauViewModel> LayLichSuTranDau()
        {
            var characters = CSVHelper.LoadNhanVat();
            return CSVHelper.LoadLichSuDau()
                .Where(x => x.CheDo == "VsComputer")
                .Join(characters, x => x.NhanVatNguoiChoiId, c => c.NhanVatId, (x, c1) => new { x, c1 })
                .Join(characters, x => x.x.NhanVatMayId, c => c.NhanVatId, (x, c2) => new LichSuTranDauViewModel
                {
                    NhanVatNguoiChoi = x.c1.TenNhanVat,
                    NhanVatMay = c2.TenNhanVat,
                    KetQua = x.x.KetQua,
                    CheDo = x.x.CheDo,
                    ThoiGianTranDau = x.x.ThoiGianTranDau
                })
                .OrderByDescending(x => x.ThoiGianTranDau)
                .ToList();
        }

        public static void HienThiLichSuTranDau(DataGridView dataGridView)
        {
            dataGridView.DataSource = LayLichSuTranDau();
            dataGridView.Columns["NhanVatNguoiChoi"].HeaderText = "Nhân vật người chơi";
            dataGridView.Columns["NhanVatMay"].HeaderText = "Nhân vật máy";
            dataGridView.Columns["KetQua"].HeaderText = "Kết quả";
            dataGridView.Columns["CheDo"].HeaderText = "Chế độ";
            dataGridView.Columns["ThoiGianTranDau"].HeaderText = "Thời gian";
        }

        public static List<LichSuTranDauViewModel> TimKiemLichSu(string keyword)
        {
            return LayLichSuTranDau().Where(x => x.NhanVatNguoiChoi.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 || x.NhanVatMay.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        public static List<TopPlayer> LayTop3NguoiChoi()
        {
            var characters = CSVHelper.LoadNhanVat();
            var statistics = CSVHelper.LoadThongKeNhanVat();
            return characters.Join(statistics, c => c.NhanVatId, s => s.NhanVatId, (c, s) => new TopPlayer { Username = c.TenNhanVat, Wins = s.SoTranThang }).OrderByDescending(x => x.Wins).ThenBy(x => x.Username).ToList();
        }
    }
}
