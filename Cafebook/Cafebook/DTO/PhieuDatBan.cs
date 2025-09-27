// DTO/PhieuDatBan.cs
using System;

namespace Cafebook.DTO
{
    public class PhieuDatBan
    {
        public int IdPhieuDatBan { get; set; }
        public int? IdKhachHang { get; set; }
        public string TenKhachVangLai { get; set; }
        public string SdtKhachVangLai { get; set; }
        public int? IdBan { get; set; }
        public DateTime ThoiGianDat { get; set; }
        public int SoLuongKhach { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; } // Ví dụ: "Đã đặt", "Đã hủy", "Đã đến"

        // Thuộc tính bổ sung để hiển thị
        public string TenKhachHangHienThi
        {
            get => IdKhachHang.HasValue ? TenKhachHangThanhVien : TenKhachVangLai;
        }

        public string SdtHienThi
        {
            get => IdKhachHang.HasValue ? SdtKhachHangThanhVien : SdtKhachVangLai;
        }

        public string SoBanHienThi { get; set; }

        // --- HAI THUỘC TÍNH BỊ THIẾU LÀ ĐÂY ---
        public string TenKhachHangThanhVien { get; set; } // Để JOIN từ bảng KhachHang
        public string SdtKhachHangThanhVien { get; set; } // Để JOIN từ bảng KhachHang
    }
}