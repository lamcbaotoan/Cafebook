using Cafebook.DTO;
using Cafebook.Views.nhanvien.pages;
using System.Windows;

namespace Cafebook.Views.nhanvien
{
    public partial class ManHinhNhanVien : Window
    {
        private NhanVien currentUser; // Biến này sẽ được gán giá trị trong constructor

        // BỎ constructor cũ public ManHinhNhanVien()
        // THAY BẰNG CONSTRUCTOR MỚI NÀY
        public ManHinhNhanVien(NhanVien user)
        {
            InitializeComponent();

            // Gán giá trị cho currentUser ngay khi cửa sổ được tạo
            this.currentUser = user;

            // Cập nhật tiêu đề cửa sổ để chào mừng nhân viên
            this.Title = "Cafebook - Nhân viên: " + user.HoTen;

            // Tải trang mặc định khi cửa sổ được mở
            MainFrame.Navigate(new SoDoBanView());
        }

        private void BtnSoDoBan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SoDoBanView());
        }

        private void BtnDatBanSach_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DatBanSachView());
        }

        private void BtnThongTinCaNhan_Click(object sender, RoutedEventArgs e)
        {
            // Bây giờ currentUser sẽ không còn là null nữa
            MainFrame.Navigate(new ThongTinCaNhanView(currentUser));
        }

        private void BtnChamCong_Click(object sender, RoutedEventArgs e)
        {
            // Bây giờ currentUser sẽ không còn là null nữa
            MainFrame.Navigate(new ChamCongView(currentUser));
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            ManHinhDangNhap loginWindow = new ManHinhDangNhap();
            loginWindow.Show();
            this.Close();
        }
    }
}