namespace Cafebook.DTO
{
    /// <summary>
    /// DTO/ViewModel để chứa thông tin tổng hợp về hiệu suất làm việc của một nhân viên
    /// trong một khoảng thời gian, dùng cho việc xuất báo cáo.
    /// </summary>
    public class HieuSuatNhanVien
    {
        public string TenNhanVien { get; set; }
        public int SoDonHang { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}