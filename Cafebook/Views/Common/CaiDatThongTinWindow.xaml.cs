using Cafebook.BUS; // Thêm using này
using System.Collections.Generic; // Thêm using này
using System.Windows;

namespace Cafebook.Views.Common
{
    public partial class CaiDatThongTinWindow : Window
    {
        private CaiDatBUS caiDatBUS;

        public CaiDatThongTinWindow()
        {
            InitializeComponent();
            caiDatBUS = new CaiDatBUS();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Đọc giá trị hiện tại từ CSDL và hiển thị
            Dictionary<string, string> thongTin = caiDatBUS.GetThongTinCuaHang();
            txtDiaChi.Text = thongTin["StoreAddress"];
            txtSoDienThoai.Text = thongTin["StorePhoneNumber"];
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (caiDatBUS.LuuThongTinCuaHang(txtDiaChi.Text, txtSoDienThoai.Text))
            {
                MessageBox.Show("Đã cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Báo hiệu lưu thành công
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi khi lưu cài đặt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}