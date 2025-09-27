using System;
using System.Collections.Generic;
namespace Cafebook.DTO
{
    public class PhieuLuong
    {
        public int IdPhieuLuong { get; set; }
        public int IdNhanVien { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public decimal TongGioLam { get; set; }
        public decimal LuongCoBan { get; set; }
        public decimal TongThuong { get; set; }
        public decimal TongPhat { get; set; }
        public decimal ThucLanh { get; set; }
        public DateTime NgayTinhLuong { get; set; }
        public List<ChiTietDongLuong> CacKhoanThuong { get; set; }
        public List<ChiTietDongLuong> CacKhoanPhat { get; set; }

        public PhieuLuong()
        {
            // Khởi tạo danh sách để tránh lỗi null
            CacKhoanThuong = new List<ChiTietDongLuong>();
            CacKhoanPhat = new List<ChiTietDongLuong>();
        }
    }
}