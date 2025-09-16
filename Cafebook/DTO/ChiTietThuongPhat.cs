using System;
namespace Cafebook.DTO
{
    public class ChiTietThuongPhat
    {
        public int IdChiTiet { get; set; }
        public int IdNhanVien { get; set; }
        public int IdLoai { get; set; }
        public DateTime NgayApDung { get; set; }
        public string GhiChu { get; set; }
    }
}