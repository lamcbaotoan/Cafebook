namespace Cafebook.DTO
{
    public class LoaiThuongPhat
    {
        public int IdLoai { get; set; }
        public string TenLoai { get; set; }
        public decimal SoTien { get; set; }
        public string Loai { get; set; } // "Thuong" hoặc "Phat"
    }
}