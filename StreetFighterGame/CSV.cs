using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace StreetFighterGame
{
    public class LichSuDauModel
    {
        public int TranDauId { get; set; }
        public int NhanVatNguoiChoiId { get; set; }
        public int NhanVatMayId { get; set; }
        public string KetQua { get; set; }
        public string CheDo { get; set; }
        public DateTime ThoiGianTranDau { get; set; }
    }

    public class ThongKeNhanVatModel
    {
        public int NhanVatId { get; set; }
        public int SoTranThang { get; set; }
    }

    public class NhanVatModel
    {
        public int NhanVatId { get; set; }
        public string TenNhanVat { get; set; }
    }

    public static class CSVHelper
    {
        private static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static List<LichSuDauModel> LoadLichSuDau()
        {
            return Read("LSTD.csv").Skip(1).Where(x => x.Length >= 6).Select(x => new LichSuDauModel
            {
                TranDauId = ToInt(x[0]),
                NhanVatNguoiChoiId = ToInt(x[1]),
                NhanVatMayId = ToInt(x[2]),
                KetQua = x[3],
                CheDo = x[4],
                ThoiGianTranDau = DateTime.Parse(x[5], CultureInfo.InvariantCulture)
            }).ToList();
        }

        public static List<ThongKeNhanVatModel> LoadThongKeNhanVat()
        {
            return Read("THONG_KE_NHAN_VAT.csv").Skip(1).Where(x => x.Length >= 2).Select(x => new ThongKeNhanVatModel
            {
                NhanVatId = ToInt(x[0]),
                SoTranThang = ToInt(x[1])
            }).ToList();
        }

        public static List<NhanVatModel> LoadNhanVat()
        {
            return Read("NV.csv").Skip(1).Where(x => x.Length >= 2).Select(x => new NhanVatModel
            {
                NhanVatId = ToInt(x[0]),
                TenNhanVat = x[1]
            }).ToList();
        }

        public static void SaveLichSuDau(List<LichSuDauModel> data)
        {
            Save("LSTD.csv", "TranDauId,NhanVatNguoiChoiId,NhanVatMayId,KetQua,CheDo,ThoiGianTranDau",
                data.Select(x => string.Join(",", x.TranDauId, x.NhanVatNguoiChoiId, x.NhanVatMayId,
                    Escape(x.KetQua), Escape(x.CheDo), x.ThoiGianTranDau.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture))));
        }

        public static void SaveThongKeNhanVat(List<ThongKeNhanVatModel> data)
        {
            Save("THONG_KE_NHAN_VAT.csv", "NhanVatId,SoTranThang",
                data.Select(x => string.Join(",", x.NhanVatId, x.SoTranThang)));
        }

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
            var values = new List<string>();
            var value = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"') quoted = !quoted;
                else if (line[i] == ',' && !quoted)
                {
                    values.Add(value.ToString().Trim());
                    value.Clear();
                }
                else value.Append(line[i]);
            }
            values.Add(value.ToString().Trim());
            return values.ToArray();
        }

        private static int ToInt(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        private static string Escape(string value)
        {
            if (value == null) return string.Empty;
            return value.Contains(",") || value.Contains("\"") ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
        }
    }
}