using Cafebook.BUS; // Thêm using này
using Cafebook.DTO; // Thêm using này
using Cafebook.Views.admin.pages;
using Cafebook.Views.Common;
using System.Collections.Generic; // Thêm using này
using System.Linq; // Thêm using này
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Thêm using này

namespace Cafebook.Views.admin
{
    public partial class ManHinhAdmin : Window
    {
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS();

        public ManHinhAdmin()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardView());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThongBao();
        }

        #region Xử lý Thông báo

        private void LoadThongBao()
        {
            List<ThongBao> dsThongBao = thongBaoBUS.GetThongBaoChuaDoc();
            icThongBaoPopup.ItemsSource = dsThongBao;

            if (dsThongBao.Any())
            {
                lblSoThongBao.Text = dsThongBao.Count.ToString();
                BadgeThongBao.Visibility = Visibility.Visible;
            }
            else
            {
                BadgeThongBao.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnThongBao_Click(object sender, RoutedEventArgs e)
        {
            PopupThongBao.IsOpen = !PopupThongBao.IsOpen;
        }

        private void ThongBaoItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 1. Đóng popup ngay sau khi người dùng click
            PopupThongBao.IsOpen = false;

            // 2. Mở cửa sổ chi tiết thông báo
            var detailWindow = new ThongBaoChiTietWindow();
            detailWindow.Owner = this; // Đặt cửa sổ admin làm chủ để cửa sổ con luôn nổi lên trên
            detailWindow.ShowDialog(); // Hiển thị dưới dạng Dialog để người dùng phải tương tác xong mới quay lại

            // 3. Sau khi cửa sổ chi tiết được đóng, kiểm tra xem có thay đổi nào không
            //    và tải lại danh sách thông báo trên popup nếu cần.
            if (detailWindow.DaThayDoi)
            {
                LoadThongBao();
            }
        }

        #endregion

        #region Navigation
        private void BtnTongQuan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardView());
        }

        private void BtnSanPham_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SanPhamView());
        }

        private void BtnKho_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new KhoView());
        }

        private void BtnSach_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SachView());
        }

        private void BtnNhanSu_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new NhanSuView());
        }

        private void BtnKhachHang_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new KhuyenMaiView());
        }

        private void BtnBan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BanView());
        }

        private void BtnDonHang_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DonHangView());
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            // 1. Hiển thị hộp thoại xác nhận
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?", // Nội dung câu hỏi
                "Xác nhận Đăng xuất",              // Tiêu đề cửa sổ
                MessageBoxButton.YesNo,            // Các nút lựa chọn
                MessageBoxImage.Question           // Icon hiển thị
            );

            // 2. Chỉ thực hiện đăng xuất nếu người dùng chọn "Yes"
            if (result == MessageBoxResult.Yes)
            {
                ManHinhDangNhap loginWindow = new ManHinhDangNhap();
                loginWindow.Show();
                this.Close();
            }
            // Nếu người dùng chọn "No", không làm gì cả.
        }
        #endregion
    }
}