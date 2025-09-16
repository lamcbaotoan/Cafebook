// Views/Common/HoaDonPreviewWindow.xaml.cs

using Cafebook.BUS; // Thêm using
using Cafebook.DTO;
using System.Collections.Generic;
// using System.Configuration; // Xóa hoặc comment dòng này
using System.Windows;
using System.Windows.Controls; // Thêm cho PrintDialog

namespace Cafebook.Views.Common
{
    public partial class HoaDonPreviewWindow : Window
    {
        public HoaDonPreviewWindow(HoaDon hoaDon, List<ChiTietHoaDon> chiTiet, NhanVien nv, string soBan, string tieuDe = "HÓA ĐƠN TẠM TÍNH")
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            lblDiaChi.Text = "Địa chỉ: " + thongTin["StoreAddress"];
            lblSdt.Text = "SĐT: " + thongTin["StorePhoneNumber"];

            lblBanSo.Text = soBan;
            lblNgayGio.Text = hoaDon.ThoiGianTao.ToString("dd/MM/yyyy HH:mm");
            lblNhanVien.Text = nv.HoTen;
            lblTieuDeChinh.Text = tieuDe;

            icChiTietHoaDon.ItemsSource = chiTiet;

            lblTongTien.Text = hoaDon.TongTien.ToString("N0");
            lblGiamGia.Text = hoaDon.SoTienGiam.ToString("N0");
            lblThanhTien.Text = hoaDon.ThanhTien.ToString("N0") + " VND";
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Lấy khu vực cần in từ XAML
            var printArea = this.FindName("printArea") as FrameworkElement;
            if (printArea == null) return;

            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(printArea, "In Hóa đơn");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}