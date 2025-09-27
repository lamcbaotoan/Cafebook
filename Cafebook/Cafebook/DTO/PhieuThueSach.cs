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
        public DateTime? NgayTraThucTe { get; set; }
        public decimal TienPhat { get; set; }
        public string TrangThai { get; set; }

        // THÊM 2 THUỘC TÍNH NÀY
        public decimal PhiThue { get; set; }
        public decimal TienCoc { get; set; }

        // Thuộc tính bổ sung để hiển thị
        public string TieuDeSach { get; set; }
        public string ViTriSach { get; set; } // Thêm để lưu vị trí khi in phiếu
        public string TenKhachHang { get; set; }
        public string SdtKhachHang { get; set; }
        public string TenNhanVien { get; set; }
    }
}