// Views/Common/PhieuLuongPreviewWindow.xaml.cs
using Cafebook.BUS;
using Cafebook.DTO;
// using System.Configuration; // Xóa
using System.Windows;
using System.Windows.Controls; // Thêm

namespace Cafebook.Views.Common
{
    public partial class PhieuLuongPreviewWindow : Window
    {
        public PhieuLuongPreviewWindow(PhieuLuong phieuLuong, NhanVien nhanVien)
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            lblDiaChi.Text = "Địa chỉ: " + thongTin["StoreAddress"];
            lblSdt.Text = "SĐT: " + thongTin["StorePhoneNumber"];

            lblNhanVien.Text = nhanVien.HoTen;
            lblKyLuong.Text = $"{phieuLuong.TuNgay:dd/MM/yyyy} - {phieuLuong.DenNgay:dd/MM/yyyy}";
            lblNgayChot.Text = phieuLuong.NgayTinhLuong.ToString("dd/MM/yyyy HH:mm");

            lblTongGioLam.Text = $"{phieuLuong.TongGioLam:N2} giờ";
            lblLuongCoBan.Text = phieuLuong.LuongCoBan.ToString("N0") + " đ";
            lblTongThuong.Text = "+ " + phieuLuong.TongThuong.ToString("N0") + " đ";
            lblTongPhat.Text = "- " + phieuLuong.TongPhat.ToString("N0") + " đ";

            lblThucLanh.Text = phieuLuong.ThucLanh.ToString("N0") + " VND";
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Thêm x:Name="printArea" vào Grid chính trong XAML
            var printArea = this.FindName("printArea") as FrameworkElement;
            if (printArea == null) return;

            PrintDialog printDialog = new PrintDialog();
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