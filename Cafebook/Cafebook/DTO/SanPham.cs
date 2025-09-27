namespace Cafebook.DTO
{
    public class SanPham
    {
        public int IdSanPham { get; set; }
        public int IdLoaiSP { get; set; }
        public string TenSanPham { get; set; }
        public string MoTa { get; set; }
        public decimal DonGia { get; set; }
        public string HinhAnh { get; set; }
        public string TrangThai { get; set; }
        public string TenLoaiSP { get; set; }

        // THÊM THUỘC TÍNH NÀY
        // Dùng để lưu trữ số lượng có thể bán được dựa trên tồn kho
        public int SoLuongCoThePhucVu { get; set; }
    }
}