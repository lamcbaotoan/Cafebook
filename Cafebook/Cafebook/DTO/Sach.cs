namespace Cafebook.DTO
{
    public class Sach
    {
        public int IdSach { get; set; }
        public string TieuDe { get; set; }
        public string TacGia { get; set; }
        public string TheLoai { get; set; }
        public string MoTa { get; set; }
        public int TongSoLuong { get; set; }
        public int SoLuongCoSan { get; set; }
        public string ViTri { get; set; }
        public decimal GiaBia { get; set; } // THÊM THUỘC TÍNH NÀY
    }
}