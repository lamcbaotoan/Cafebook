using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Thuộc tính bổ sung
        public string TenLoaiSP { get; set; }
    }
}
