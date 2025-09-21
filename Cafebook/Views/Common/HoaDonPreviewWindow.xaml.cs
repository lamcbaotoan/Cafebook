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
        public HoaDonPreviewWindow(HoaDon hoaDon, List<ChiTietHoaDon> chiTiet, NhanVien nv, string soBan, string tieuDe = "HÓA ĐƠN TẠM TÍNH", decimal tienKhachDua = 0)
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            // Sử dụng ContainsKey để tránh lỗi
            lblDiaChi.Text = "Địa chỉ: " + (thongTin.ContainsKey("StoreAddress") ? thongTin["StoreAddress"] : "...");
            lblSdt.Text = "SĐT: " + (thongTin.ContainsKey("StorePhoneNumber") ? thongTin["StorePhoneNumber"] : "...");

            lblBanSo.Text = soBan;
            lblNgayGio.Text = hoaDon.ThoiGianTao.ToString("dd/MM/yyyy HH:mm");
            lblNhanVien.Text = nv.HoTen;
            lblTieuDeChinh.Text = tieuDe;

            icChiTietHoaDon.ItemsSource = chiTiet;

            lblTongTien.Text = hoaDon.TongTien.ToString("N0");
            lblGiamGia.Text = hoaDon.SoTienGiam.ToString("N0");
            lblThanhTien.Text = hoaDon.ThanhTien.ToString("N0") + " VND";

            // **LOGIC MỚI: Chỉ hiển thị tiền khách đưa và tiền thối khi thanh toán cuối cùng**
            if (tieuDe == "HÓA ĐƠN THANH TOÁN" && tienKhachDua > 0)
            {
                lblTienKhachDuaTitle.Visibility = Visibility.Visible;
                lblTienKhachDua.Visibility = Visibility.Visible;
                lblTienThoiLaiTitle.Visibility = Visibility.Visible;
                lblTienThoiLai.Visibility = Visibility.Visible;

                lblTienKhachDua.Text = tienKhachDua.ToString("N0");
                decimal tienThoiLai = tienKhachDua - hoaDon.ThanhTien;
                lblTienThoiLai.Text = tienThoiLai.ToString("N0");
            }
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