using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class ThongTinCaNhanView : Page
    {
        private NhanVien currentUser;
        private NhanSuBUS nhanSuBUS = new NhanSuBUS();

        public ThongTinCaNhanView(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblHoTen.Text = currentUser.HoTen;
            lblVaiTro.Text = currentUser.TenVaiTro;
            lblSdt.Text = currentUser.SoDienThoai;
            lblEmail.Text = currentUser.Email;
            txtDiaChi.Text = currentUser.DiaChi;
        }

        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            currentUser.DiaChi = txtDiaChi.Text;

            // Chỉ cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
            if (!string.IsNullOrEmpty(txtMatKhauMoi.Password))
            {
                if (txtMatKhauMoi.Password != txtXacNhanMatKhau.Password)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                currentUser.MatKhau = txtMatKhauMoi.Password;
            }

            if (nhanSuBUS.CapNhatThongTinCaNhan(currentUser))
            {
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}