using Cafebook.DTO;
using Cafebook.Views.nhanvien.pages;
using System.Linq; // Thêm vào để dùng Linq
using System.Windows;
using System.Windows.Controls; // Thêm vào để dùng ToggleButton
using System.Windows.Controls.Primitives; // << Dòng quan trọng nhất

namespace Cafebook.Views.nhanvien
{
    public partial class ManHinhNhanVien : Window
    {
        private NhanVien currentUser;
        private ToggleButton currentButton; // Biến để theo dõi nút đang được chọn

        public ManHinhNhanVien(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;

            // Cập nhật giao diện với thông tin người dùng
            txtUserName.Text = user.HoTen;
            txtUserRole.Text = (user.IdVaiTro == 1) ? "Quản lý" : "Nhân viên"; // Giả sử IdVaiTro = 1 là quản lý

            // Tải trang mặc định và đặt nút tương ứng là được chọn
            MainFrame.Navigate(new SoDoBanView(currentUser));
            currentButton = btnSoDoBan;
            currentButton.IsChecked = true;
        }

        // Hàm helper để quản lý trạng thái của các nút
        private void UpdateSelectedButton(ToggleButton newButton)
        {
            if (currentButton != null && currentButton != newButton)
            {
                currentButton.IsChecked = false;
            }
            currentButton = newButton;
            currentButton.IsChecked = true;
        }

        private void BtnSoDoBan_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedButton(sender as ToggleButton);
            MainFrame.Navigate(new SoDoBanView(currentUser));
        }

        private void BtnDatBanSach_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedButton(sender as ToggleButton);
            MainFrame.Navigate(new DatBanSachView()); // Nhớ truyền currentUser nếu cần
        }

        private void BtnThongTinCaNhan_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedButton(sender as ToggleButton);
            MainFrame.Navigate(new ThongTinCaNhanView(currentUser));
        }

        private void BtnChamCong_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedButton(sender as ToggleButton);
            MainFrame.Navigate(new ChamCongView(currentUser));
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
    }
}