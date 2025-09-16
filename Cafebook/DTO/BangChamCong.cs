using System;
namespace Cafebook.DTO
{
    public class BangChamCong
    {
        public int IdChamCong { get; set; }
        public int IdLichLamViec { get; set; }
        public DateTime? GioVao { get; set; }
        public DateTime? GioRa { get; set; }
        public decimal? SoGioLam { get; set; }

        // Thuộc tính hiển thị
        public string HoTenNhanVien { get; set; }
        public DateTime NgayLam { get; set; }
        public string TenCa { get; set; }
    }
}