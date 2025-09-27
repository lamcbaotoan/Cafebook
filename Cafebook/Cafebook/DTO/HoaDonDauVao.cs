// Trong file DTO/HoaDonDauVao.cs
using System;

namespace Cafebook.DTO
{
    public class HoaDonDauVao
    {
        public int IdHoaDonNhap { get; set; }
        public int IdPhieuNhap { get; set; }

        // BỔ SUNG 2 THUỘC TÍNH CÒN THIẾU
        public string maHoaDon { get; set; }
        public DateTime? ngayPhatHanh { get; set; }

        public string DuongDanFile { get; set; }
    }
}