using System;
namespace Cafebook.DTO
{
    public class LichLamViec
    {
        public int IdLichLamViec { get; set; }
        public int IdNhanVien { get; set; }
        public int IdCa { get; set; }
        public DateTime NgayLam { get; set; }

        // Thuộc tính hiển thị
        public string HoTenNhanVien { get; set; }
        public string TenCa { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }
}