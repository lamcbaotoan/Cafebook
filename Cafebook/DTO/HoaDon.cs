// DTO/HoaDon.cs
using System;

namespace Cafebook.DTO
{
    public class HoaDon
    {
        public int IdHoaDon { get; set; }
        public int? IdKhachHang { get; set; }
        public int IdNhanVien { get; set; }
        public int IdBan { get; set; }
        public int? IdKhuyenMai { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public decimal TongTien { get; set; }
        public decimal SoTienGiam { get; set; }
        public decimal ThanhTien { get; set; }
        public string TrangThai { get; set; }
    }
}