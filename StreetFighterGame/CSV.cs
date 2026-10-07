using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace StreetFighterGame
{
    public class TaiKhoanModel { public int UserId { get; set; } public string Username { get; set; } public string Password { get; set; } public int Tien { get; set; } public int SoTranThang { get; set; } public int SoTranThua { get; set; } public int SoTranHoa { get; set; } }
    public class LichSuDauModel { public int TranDauId { get; set; } public int UserId { get; set; } public int NhanVat1Id { get; set; } public int NhanVat2Id { get; set; } public string KetQua { get; set; } public DateTime ThoiGianTranDau { get; set; } }
    public class NhanVatModel { public int NhanVatId { get; set; } public string TenNhanVat { get; set; } }
    public class XepHangModel { public int UserId { get; set; } public int SoTranThang { get; set; } public int XepHang { get; set; } }

    public static class CSVHelper
    {
        private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static List<TaiKhoanModel> LoadTaiKhoan() { return Read("TKoan.csv").Skip(1).Where(x => x.Length >= 7).Select(x => new TaiKhoanModel { UserId = ToInt(x[0]), Username = x[1], Password = x[2], Tien = ToInt(x[3]), SoTranThang = ToInt(x[4]), SoTranThua = ToInt(x[5]), SoTranHoa = ToInt(x[6]) }).ToList(); }
        public static List<LichSuDauModel> LoadLichSuDau() { return Read("LSTD.csv").Skip(1).Where(x => x.Length >= 6).Select(x => new LichSuDauModel { TranDauId = ToInt(x[0]), UserId = ToInt(x[1]), NhanVat1Id = ToInt(x[2]), NhanVat2Id = ToInt(x[3]), KetQua = x[4], ThoiGianTranDau = DateTime.Parse(x[5], CultureInfo.InvariantCulture) }).ToList(); }
        public static List<NhanVatModel> LoadNhanVat() { return Read("NV.csv").Skip(1).Where(x => x.Length >= 2).Select(x => new NhanVatModel { NhanVatId = ToInt(x[0]), TenNhanVat = x[1] }).ToList(); }
        public static List<XepHangModel> LoadXepHang() { return Read("XH.csv").Skip(1).Where(x => x.Length >= 3).Select(x => new XepHangModel { UserId = ToInt(x[0]), SoTranThang = ToInt(x[1]), XepHang = ToInt(x[2]) }).ToList(); }

        public static void SaveTaiKhoan(List<TaiKhoanModel> data) { Save("TKoan.csv", "UserId,Username,Password,Tien,SoTranThang,SoTranThua,SoTranHoa", data.Select(x => string.Join(",", x.UserId, Escape(x.Username), Escape(x.Password), x.Tien, x.SoTranThang, x.SoTranThua, x.SoTranHoa))); }
        public static void SaveLichSuDau(List<LichSuDauModel> data) { Save("LSTD.csv", "TranDauId,UserId,NhanVat1Id,NhanVat2Id,KetQua,ThoiGianTranDau", data.Select(x => string.Join(",", x.TranDauId, x.UserId, x.NhanVat1Id, x.NhanVat2Id, Escape(x.KetQua), x.ThoiGianTranDau.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)))); }
        public static void SaveXepHang(List<XepHangModel> data) { Save("XH.csv", "UserId,SoTranThang,XepHang", data.Select(x => string.Join(",", x.UserId, x.SoTranThang, x.XepHang))); }

        private static List<string[]> Read(string name)
        {
            string path = Path.Combine(BasePath, name);
            if (!File.Exists(path)) throw new FileNotFoundException("Không tìm thấy file CSV: " + path);
            return File.ReadAllLines(path, Encoding.UTF8).Where(x => !string.IsNullOrWhiteSpace(x)).Select(Parse).ToList();
        }

        private static void Save(string name, string header, IEnumerable<string> rows)
        {
            string path = Path.Combine(BasePath, name);
            string temp = path + ".tmp";
            File.WriteAllLines(temp, new[] { header }.Concat(rows), Encoding.UTF8);
            if (File.Exists(path)) File.Delete(path);
            File.Move(temp, path);
        }

        private static string[] Parse(string line)
        {
            var values = new List<string>(); var value = new StringBuilder(); bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"') quoted = !quoted;
                else if (line[i] == ',' && !quoted) { values.Add(value.ToString().Trim()); value.Clear(); }
                else value.Append(line[i]);
            }
            values.Add(value.ToString().Trim()); return values.ToArray();
        }

        private static int ToInt(string value) { int result; return int.TryParse(value, out result) ? result : 0; }
        private static string Escape(string value) { if (value == null) return string.Empty; return value.Contains(",") || value.Contains("\"") ? "\"" + value.Replace("\"", "\"\"") + "\"" : value; }
    }
}