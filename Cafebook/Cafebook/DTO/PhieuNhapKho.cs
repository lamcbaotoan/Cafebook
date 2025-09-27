using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Cafebook.DTO
{
    public class PhieuNhapKho
    {
        public int IdPhieuNhap { get; set; }
        public int IdNhanVien { get; set; }
        public int IdNhaCungCap { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }

        // Thuộc tính bổ sung
        public string TenNhaCungCap { get; set; }
        public string TenNhanVien { get; set; }
    }
}
