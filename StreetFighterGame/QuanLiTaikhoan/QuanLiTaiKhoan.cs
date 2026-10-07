using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetFighterGame.QuanLiTaikhoan
{
    public class LichSuTranDauViewModel { public string TenNguoiChoi { get; set; } public string NhanVat1 { get; set; } public string NhanVat2 { get; set; } public string KetQua { get; set; } public DateTime? ThoiGianTranDau { get; set; } }
    public class TaiKhoan { public string Username; public string Password; public TaiKhoan(string us, string ps) { Username = us; Password = ps; } }
    public class TopPlayer { public string Username { get; set; } public int Wins { get; set; } }

    public static class QuanLiTaiKhoan
    {
        public static TaiKhoan taiKhoanDangChoi = new TaiKhoan(string.Empty, string.Empty);
        public static TaiKhoan Admin = new TaiKhoan("Admin", "TanHuyVippro05");

        public static int GetNhanVatIdByName(string tenNhanVat)
        {
            var character = CSVHelper.LoadNhanVat().FirstOrDefault(x => string.Equals(x.TenNhanVat, tenNhanVat, StringComparison.OrdinalIgnoreCase));
            return character == null ? 0 : character.NhanVatId;
        }

        public static int GetUserIdByUsername(string username)
        {
            var account = CSVHelper.LoadTaiKhoan().FirstOrDefault(x => x.Username == username);
            return account == null ? 0 : account.UserId;
        }

        public static void LuuTranDau(int nhanVat1Id, int nhanVat2Id, string ketQua)
        {
            var accounts = CSVHelper.LoadTaiKhoan();
            var matches = CSVHelper.LoadLichSuDau();
            var account = accounts.FirstOrDefault(x => x.Username == taiKhoanDangChoi.Username);
            if (account == null) { MessageBox.Show("Không tìm thấy tài khoản đang đăng nhập."); return; }

            matches.Add(new LichSuDauModel { TranDauId = matches.Count == 0 ? 1 : matches.Max(x => x.TranDauId) + 1, UserId = account.UserId, NhanVat1Id = nhanVat1Id, NhanVat2Id = nhanVat2Id, KetQua = ketQua, ThoiGianTranDau = DateTime.Now });
            if (ketQua == "Win") account.SoTranThang++;
            else if (ketQua == "Lose") account.SoTranThua++;
            else if (ketQua == "Draw") account.SoTranHoa++;

            CSVHelper.SaveLichSuDau(matches);
            CSVHelper.SaveTaiKhoan(accounts);
            CapNhatXepHang(accounts);
        }

        public static List<LichSuTranDauViewModel> LayLichSuTranDau()
        {
            var accounts = CSVHelper.LoadTaiKhoan();
            var characters = CSVHelper.LoadNhanVat();
            return CSVHelper.LoadLichSuDau()
                .Join(accounts, m => m.UserId, a => a.UserId, (m, a) => new { m, a })
                .Join(characters, x => x.m.NhanVat1Id, c => c.NhanVatId, (x, c1) => new { x.m, x.a, c1 })
                .Join(characters, x => x.m.NhanVat2Id, c => c.NhanVatId, (x, c2) => new LichSuTranDauViewModel { TenNguoiChoi = x.a.Username, NhanVat1 = x.c1.TenNhanVat, NhanVat2 = c2.TenNhanVat, KetQua = x.m.KetQua, ThoiGianTranDau = x.m.ThoiGianTranDau })
                .OrderByDescending(x => x.ThoiGianTranDau).ToList();
        }

        public static void HienThiLichSuTranDau(DataGridView dataGridView)
        {
            dataGridView.DataSource = LayLichSuTranDau();
            dataGridView.Columns["TenNguoiChoi"].HeaderText = "Người chơi";
            dataGridView.Columns["NhanVat1"].HeaderText = "Nhân vật 1";
            dataGridView.Columns["NhanVat2"].HeaderText = "Nhân vật 2";
            dataGridView.Columns["KetQua"].HeaderText = "Kết quả";
            dataGridView.Columns["ThoiGianTranDau"].HeaderText = "Thời gian";
        }

        public static List<LichSuTranDauViewModel> TimKiemLichSu(string tenNguoiChoi) { return LayLichSuTranDau().Where(x => x.TenNguoiChoi.Contains(tenNguoiChoi)).ToList(); }

        public static List<TopPlayer> LayTop3NguoiChoi()
        {
            return CSVHelper.LoadTaiKhoan().OrderByDescending(x => x.SoTranThang).Take(3).Select(x => new TopPlayer { Username = x.Username, Wins = x.SoTranThang }).ToList();
        }

        public static bool DoiMatKhau(string username, string oldPassword, string newPassword, string confirmPassword, Label message)
        {
            var accounts = CSVHelper.LoadTaiKhoan();
            var account = accounts.FirstOrDefault(x => x.Username == username && x.Password == oldPassword);
            if (account == null) { message.Visible = true; message.Text = "Tên tài khoản hoặc mật khẩu cũ không chính xác!"; return false; }
            if (newPassword.Length < 8) { message.Visible = true; message.Text = "Mật khẩu mới phải có ít nhất 8 ký tự!"; return false; }
            if (newPassword != confirmPassword) { message.Visible = true; message.Text = "Mật khẩu mới và xác nhận mật khẩu không khớp!"; return false; }
            account.Password = newPassword; CSVHelper.SaveTaiKhoan(accounts); MessageBox.Show("Đổi mật khẩu thành công!"); message.Visible = false; return true;
        }

        public static bool XoaTaiKhoan(string username)
        {
            if (username == Admin.Username) { MessageBox.Show("Không thể xóa tài khoản admin"); return false; }
            var accounts = CSVHelper.LoadTaiKhoan(); var account = accounts.FirstOrDefault(x => x.Username == username);
            if (account == null) { MessageBox.Show("Không tìm thấy tài khoản '" + username + "'!"); return false; }
            accounts.Remove(account);
            var matches = CSVHelper.LoadLichSuDau(); matches.RemoveAll(x => x.UserId == account.UserId);
            CSVHelper.SaveTaiKhoan(accounts); CSVHelper.SaveLichSuDau(matches); CapNhatXepHang(accounts);
            MessageBox.Show("Tài khoản '" + username + "' đã được xóa thành công!"); return true;
        }

        public static void TimKiemTaiKhoan(DataGridView grid, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) { LoadDanhSachTaiKhoanLenGridView(grid); return; }
            grid.DataSource = CSVHelper.LoadTaiKhoan().Where(x => x.Username.Contains(keyword)).Select(x => new { x.UserId, x.Username, x.Tien, x.SoTranThang, x.SoTranThua, x.SoTranHoa }).ToList();
        }

        public static void LoadDanhSachTaiKhoanLenGridView(DataGridView grid)
        {
            grid.Dock = DockStyle.None; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.DataSource = CSVHelper.LoadTaiKhoan().Select(x => new { x.UserId, x.Username, x.Tien, x.SoTranThang, x.SoTranThua, x.SoTranHoa }).ToList();
        }

        public static bool CheckIsAdmin() { return taiKhoanDangChoi.Username == "Admin"; }

        public static void DangNhap(string username, string password)
        {
            var account = CSVHelper.LoadTaiKhoan().FirstOrDefault(x => x.Username == username && x.Password == password);
            if (account == null) { MessageBox.Show("Tài khoản hoặc mật khẩu sai!"); return; }
            taiKhoanDangChoi.Username = account.Username; taiKhoanDangChoi.Password = account.Password;
            new FormStart().Show(); FormLogincs.instance.Hide();
        }

        public static bool CreatAccount(string username, string password, string confirmUsername, string password1, string password2, Label message)
        {
            if (!CheckUsername(message, confirmUsername) || !CheckPassword(message, password1, password2)) return false;
            var accounts = CSVHelper.LoadTaiKhoan();
            accounts.Add(new TaiKhoanModel { UserId = accounts.Count == 0 ? 1 : accounts.Max(x => x.UserId) + 1, Username = username, Password = password, Tien = 100 });
            CSVHelper.SaveTaiKhoan(accounts); MessageBox.Show("Tạo tài khoản thành công"); return true;
        }

        public static bool CheckUsername(Label message, string username)
        {
            if (CSVHelper.LoadTaiKhoan().Any(x => x.Username == username)) { message.Visible = true; message.Text = "Tài khoản đã tồn tại"; return false; }
            return true;
        }

        public static bool CheckPassword(Label message, string password1, string password2)
        {
            if (password1 == password2 && password1.Length >= 8) return true;
            message.Visible = true; message.Text = "Mật khẩu phải dài hơn 8 kí tự"; return false;
        }

        private static void CapNhatXepHang(List<TaiKhoanModel> accounts)
        {
            CSVHelper.SaveXepHang(accounts.OrderByDescending(x => x.SoTranThang).Select((x, i) => new XepHangModel { UserId = x.UserId, SoTranThang = x.SoTranThang, XepHang = i + 1 }).ToList());
        }
    }
}
