using System.Windows;
// Thêm using đến các trang con (Pages) mà bạn sẽ tạo
// using Cafebook.Views.admin.pages;
using Cafebook.Views.admin.pages;

namespace Cafebook.Views.admin
{
    public partial class ManHinhAdmin : Window
    {
        public ManHinhAdmin()
        {
            InitializeComponent();
            // Tải trang mặc định khi cửa sổ được mở
            // MainFrame.Navigate(new DashboardPage());
            MainFrame.Navigate(new DashboardView()); // Sửa dòng này
        }

        private void BtnTongQuan_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new DashboardPage());
            MainFrame.Navigate(new DashboardView()); // Và dòng này
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new SanPhamPage());
            MainFrame.Navigate(new SanPhamView());
        }

        private void BtnKho_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new KhoPage());
            MainFrame.Navigate(new KhoView());
        }

        private void BtnSach_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new SachPage());
            MainFrame.Navigate(new SachView());
        }

        private void BtnNhanSu_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new NhanSuPage());
            MainFrame.Navigate(new NhanSuView());
        }

        private void BtnKhachHang_Click(object sender, RoutedEventArgs e)
        {
            // MainFrame.Navigate(new KhachHangKMPage());
            MainFrame.Navigate(new KhuyenMaiView());
        }

        private void BtnBan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BanView());
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            ManHinhDangNhap loginWindow = new ManHinhDangNhap();
            loginWindow.Show();
            this.Close();
        }
    }
}