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
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS(); // Thêm BUS này

        // Constructor không đổi
        public ThongTinCaNhanView(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Hiển thị thông tin cơ bản (không thể sửa)
            lblHoTen.Text = currentUser.HoTen;
            lblVaiTro.Text = currentUser.TenVaiTro;
            lblNgayVaoLam.Text = currentUser.NgayVaoLam.ToString("dd/MM/yyyy");
            lblLuong.Text = currentUser.MucLuongTheoGio.ToString("N0") + " VND/giờ";

            // Điền thông tin có thể sửa vào TextBox
            txtSdt.Text = currentUser.SoDienThoai;
            txtEmail.Text = currentUser.Email;
            txtDiaChi.Text = currentUser.DiaChi;
        }

        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            // Cập nhật thông tin từ các TextBox vào đối tượng currentUser
            currentUser.SoDienThoai = txtSdt.Text;
            currentUser.Email = txtEmail.Text;
            currentUser.DiaChi = txtDiaChi.Text;

            bool updatePassword = false;

            // Chỉ cập nhật mật khẩu nếu người dùng nhập mật khẩu mới
            if (!string.IsNullOrEmpty(txtMatKhauMoi.Password))
            {
                if (txtMatKhauMoi.Password != txtXacNhanMatKhau.Password)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                currentUser.MatKhau = txtMatKhauMoi.Password; // Mật khẩu chưa mã hóa theo yêu cầu
                updatePassword = true;
            }

            if (nhanSuBUS.CapNhatThongTinCaNhan(currentUser, updatePassword))
            {
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Xóa các ô mật khẩu sau khi cập nhật
                txtMatKhauMoi.Clear();
                txtXacNhanMatKhau.Clear();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuiThongBao_Click(object sender, RoutedEventArgs e)
        {
            string noiDung = txtNoiDungThongBao.Text.Trim();
            if (string.IsNullOrEmpty(noiDung))
            {
                // Sửa MessageBoxWarning thành MessageBoxImage
                MessageBox.Show("Vui lòng nhập nội dung báo cáo.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }

            var thongBao = new ThongBao
            {
                IdNhanVien = currentUser.IdNhanVien,
                NoiDung = noiDung
            };

            if (thongBaoBUS.GuiThongBao(thongBao))
            {
                MessageBox.Show("Gửi thông báo đến quản lý thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNoiDungThongBao.Clear(); // Xóa nội dung đã gửi
            }
            else
            {
                MessageBox.Show("Gửi thông báo thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}