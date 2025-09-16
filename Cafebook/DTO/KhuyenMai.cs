using System;

namespace Cafebook.DTO
{
    public class KhuyenMai
    {
        public int IdKhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public string LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }

        // --- Thuộc tính điều kiện mới ---
        public decimal? GiaTriDonHangToiThieu { get; set; }
        public int? IdSanPhamApDung { get; set; }

        // --- Thuộc tính bổ sung để hiển thị ---
        public string TenSanPhamApDung { get; set; }
        public string TrangThai
        {
            get
            {
                if (DateTime.Now < NgayBatDau) return "Sắp diễn ra";
                if (DateTime.Now > NgayKetThuc) return "Đã kết thúc";
                return "Đang diễn ra";
            }
        }
    }
}