using Cafebook.DTO;
using Cafebook.Views.nhanvien.pages;
using System.Windows;

namespace Cafebook.Views.nhanvien
{
    public partial class ManHinhNhanVien : Window
    {
        private NhanVien currentUser;

        // Constructor đã đúng, nhận vào user
        public ManHinhNhanVien(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;
            this.Title = "Cafebook - Nhân viên: " + user.HoTen;

            // SỬA LẠI ĐÂY: Truyền currentUser vào SoDoBanView ngay khi tải
            MainFrame.Navigate(new SoDoBanView(currentUser));
        }

        private void BtnSoDoBan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SoDoBanView(currentUser));
        }

        private void BtnDatBanSach_Click(object sender, RoutedEventArgs e)
        {
            // Tương lai khi trang này cần, cũng sẽ truyền currentUser vào đây
            MainFrame.Navigate(new DatBanSachView());
        }

        private void BtnThongTinCaNhan_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ThongTinCaNhanView(currentUser));
        }

        private void BtnChamCong_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChamCongView(currentUser));
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
    }
}