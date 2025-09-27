// Trong file: DTO/ChiTietDongLuong.cs
namespace Cafebook.DTO
{
    public class ChiTietDongLuong
    {
        // ID này sẽ có giá trị đối với khoản thủ công, và là NULL đối với khoản tự động
        public int? IdChiTiet { get; set; }
        public string NoiDung { get; set; }
        public decimal SoTien { get; set; }
    }
}