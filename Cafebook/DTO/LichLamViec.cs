// File: DTO/LichLamViec.cs
using System;

namespace Cafebook.DTO
{
    public class LichLamViec
    {
        public int IdLichLamViec { get; set; }
        public int IdNhanVien { get; set; }
        public int IdCa { get; set; }
        public DateTime NgayLam { get; set; }

        // Các thuộc tính join từ bảng khác
        public string HoTenNhanVien { get; set; }
        public string TenCa { get; set; }

        // CÁC THUỘC TÍNH MỚI CẦN THÊM VÀO
        public TimeSpan? GioBatDau { get; set; }
        public TimeSpan? GioKetThuc { get; set; }
        public string TrangThai { get; set; }
    }
}