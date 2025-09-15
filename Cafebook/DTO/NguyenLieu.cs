using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafebook.DTO
{
    public class NguyenLieu
    {
        public int IdNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; }
        public string DonViTinh { get; set; }
        public decimal SoLuongTon { get; set; }
        public decimal NguongCanhBao { get; set; } // << THÊM DÒNG NÀY
    }
}