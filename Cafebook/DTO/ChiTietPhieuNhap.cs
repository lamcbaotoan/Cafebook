using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafebook.DTO
{
    public class ChiTietPhieuNhap
    {
        public int IdPhieuNhap { get; set; }
        public int IdNguyenLieu { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // Thuộc tính bổ sung
        public string TenNguyenLieu { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}