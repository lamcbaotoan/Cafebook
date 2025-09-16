using System;

namespace Cafebook.DTO
{
    /// <summary>
    /// Đây là một lớp DTO/ViewModel chuyên dụng để chứa thông tin chi tiết
    /// của một dòng doanh thu (tương ứng với một hóa đơn đã thanh toán)
    /// phục vụ cho việc xuất báo cáo.
    /// </summary>
    public class DoanhThu
    {
        public int MaHD { get; set; }
        public string NhanVien { get; set; }
        public string Ban { get; set; }
        public DateTime ThoiGian { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}