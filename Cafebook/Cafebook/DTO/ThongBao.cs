using System;

namespace Cafebook.DTO
{
    public class ThongBao
    {
        public int IdThongBao { get; set; }
        public int IdNhanVien { get; set; }
        public string NoiDung { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public bool DaDoc { get; set; }

        // --- Thuộc tính bổ sung để hiển thị ---
        public string TenNhanVien { get; set; }
    }
}