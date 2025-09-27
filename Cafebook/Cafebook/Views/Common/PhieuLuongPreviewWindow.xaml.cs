using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.Common
{
    public partial class PhieuLuongPreviewWindow : Window
    {
        // Trong file Views/Common/PhieuLuongPreviewWindow.xaml.cs

        public PhieuLuongPreviewWindow(PhieuLuong phieuLuong, NhanVien nhanVien)
        {
            InitializeComponent();

            // Gán DataContext để XAML tự động binding dữ liệu
            this.DataContext = phieuLuong;

            // Lấy thông tin cửa hàng
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            // **SỬA LỖI Ở ĐÂY: Dùng ContainsKey để kiểm tra trước khi lấy giá trị**
            string diaChi = thongTin.ContainsKey("StoreAddress") ? thongTin["StoreAddress"] : "...";
            string sdt = thongTin.ContainsKey("StorePhoneNumber") ? thongTin["StorePhoneNumber"] : "...";

            lblDiaChi.Text = "Địa chỉ: " + diaChi;
            lblSdt.Text = "SĐT: " + sdt;

            // Gán các thông tin không có trong PhieuLuong DTO
            lblNhanVien.Text = nhanVien.HoTen;
            lblKyLuong.Text = $"{phieuLuong.TuNgay:dd/MM/yyyy} - {phieuLuong.DenNgay:dd/MM/yyyy}";
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(printArea, "In Phiếu lương");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}