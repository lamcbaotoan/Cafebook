using System;

namespace Cafebook.DTO
{
    public class PhieuThueSach
    {
        public int IdPhieuThue { get; set; }
        public int IdSach { get; set; }
        public int IdKhachHang { get; set; }
        public int IdNhanVien { get; set; }
        public DateTime NgayThue { get; set; }
        public DateTime NgayHenTra { get; set; }
        public DateTime? NgayTraThucTe { get; set; } // Dùng ? để cho phép giá trị null
        public decimal TienPhat { get; set; }
        public string TrangThai { get; set; }

        // --- Thuộc tính bổ sung để hiển thị ---
        public string TieuDeSach { get; set; }
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }
    }
}