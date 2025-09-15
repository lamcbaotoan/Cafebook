// Trong DTO/CongThuc.cs
namespace Cafebook.DTO
{
    public class CongThuc
    {
        public int IdSanPham { get; set; }
        public int IdNguyenLieu { get; set; }
        public decimal LuongCanThiet { get; set; }
        public string DonViTinhSuDung { get; set; } // << THÊM DÒNG NÀY

        // Thuộc tính bổ sung để hiển thị trên DataGrid
        public string TenNguyenLieu { get; set; }
        // public string DonViTinh { get; set; } // Có thể bỏ dòng này nếu DonViTinhSuDung thay thế
    }
}