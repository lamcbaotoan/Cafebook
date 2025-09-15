// DTO/ChiTietHoaDon.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cafebook.DTO
{
    public class ChiTietHoaDon : INotifyPropertyChanged
    {
        public int IdHoaDon { get; set; }
        public int IdSanPham { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get { return _soLuong; }
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    OnPropertyChanged(); // Thông báo cho UI là 'SoLuong' đã thay đổi
                    OnPropertyChanged(nameof(ThanhTien)); // Thông báo luôn là 'ThanhTien' cũng thay đổi
                }
            }
        }

        public decimal DonGiaLucBan { get; set; }
        public string GhiChu { get; set; }

        public string TenSanPham { get; set; }
        public decimal ThanhTien => SoLuong * DonGiaLucBan;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}