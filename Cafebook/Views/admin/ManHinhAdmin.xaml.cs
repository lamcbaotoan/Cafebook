using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.admin.pages;
using Cafebook.Views.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives; // Thêm using này
using System.Windows.Input;

namespace Cafebook.Views.admin
{
    public partial class ManHinhAdmin : Window
    {
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS();
        private ToggleButton currentNavButton; // Biến để theo dõi nút đang được chọn

        public ManHinhAdmin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadThongBao();
            // Mặc định chọn nút Tổng quan khi cửa sổ được tải
            NavButton_Click(btnTongQuan, null);
        }

        #region Xử lý Thông báo
        // ... (Phần code xử lý thông báo của bạn giữ nguyên) ...
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
            PopupThongBao.IsOpen = false;
            var detailWindow = new ThongBaoChiTietWindow();
            detailWindow.Owner = this;
            detailWindow.ShowDialog();
            if (detailWindow.DaThayDoi)
            {
                LoadThongBao();
            }
        }
        #endregion

        #region Navigation

        // SỬA: Gộp tất cả các sự kiện click điều hướng vào một hàm
        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as ToggleButton;
            if (clickedButton == null) return;

            // Bỏ chọn nút cũ
            if (currentNavButton != null && currentNavButton != clickedButton)
            {
                currentNavButton.IsChecked = false;
            }
            // Chọn nút mới
            currentNavButton = clickedButton;
            currentNavButton.IsChecked = true;

            // Điều hướng đến trang tương ứng
            if (clickedButton == btnTongQuan)
                MainFrame.Navigate(new DashboardView());
            else if (clickedButton == btnSanPham)
                MainFrame.Navigate(new SanPhamView());
            else if (clickedButton == btnKho)
                MainFrame.Navigate(new KhoView());
            else if (clickedButton == btnSach)
                MainFrame.Navigate(new SachView());
            else if (clickedButton == btnNhanSu)
                MainFrame.Navigate(new NhanSuView());
            else if (clickedButton == btnKhachHang)
                MainFrame.Navigate(new KhuyenMaiView());
            else if (clickedButton == btnBan)
                MainFrame.Navigate(new BanView());
            else if (clickedButton == btnDonHang)
                MainFrame.Navigate(new DonHangView());
        }

        private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận Đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                ManHinhDangNhap loginWindow = new ManHinhDangNhap();
                loginWindow.Show();
                this.Close();
            }
        }
        #endregion

        private void btnKhachHang_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}