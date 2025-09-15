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
        public string TrangThai { get; set; }

        // --- Thuộc tính bổ sung để hiển thị ---
        public string TenKhachHangHienThi => IdKhachHang.HasValue ? "(Thành viên)" : TenKhachVangLai;
        public string SdtHienThi { get; set; } // Sẽ gán trong BUS
        public string SoBanHienThi { get; set; } // Sẽ gán trong BUS
    }
}