namespace Cafebook.DTO
{
    public class Ban
    {
        public int IdBan { get; set; }
        public string SoBan { get; set; }
        public int SoGhe { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        // --- BỔ SUNG 2 THUỘC TÍNH CÒN THIẾU VÀO ĐÂY ---
        public int? IdHoaDonHienTai { get; set; } // Dùng int? để cho phép giá trị null
        public decimal TongTienHienTai { get; set; }
    }
}