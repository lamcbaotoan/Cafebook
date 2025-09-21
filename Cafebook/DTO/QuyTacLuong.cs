// File: DTO/QuyTacLuong.cs
namespace Cafebook.DTO
{
    public class QuyTacLuong
    {
        public int IdQuyTac { get; set; }
        public string TenQuyTac { get; set; }
        public string Loai { get; set; } // "Thuong" hoặc "Phat"
        public string LoaiQuyTac { get; set; } // FLAT, LATE, OVERTIME, MONTHLY_HOURS...
        public decimal? DieuKien { get; set; }
        // **THÊM THUỘC TÍNH MỚI DƯỚI ĐÂY**
        public string DieuKienDonViTinh { get; set; }
        public decimal GiaTriApDung { get; set; }
        public string DonViTinh { get; set; }
    }
}