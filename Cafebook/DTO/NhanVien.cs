using System;

namespace Cafebook.DTO
{
    public class NhanVien
    {
        public int IdNhanVien { get; set; }
        public int IdVaiTro { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; } // Thuộc tính bị thiếu
        public string MatKhau { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public bool TrangThai { get; set; }

        // Thuộc tính bổ sung
        public string TenVaiTro { get; set; }
    }
}