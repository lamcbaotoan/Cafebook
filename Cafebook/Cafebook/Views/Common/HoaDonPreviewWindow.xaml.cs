using Cafebook.BUS;
using Cafebook.DTO;
using System; // Thêm using này
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.Common
{
    public partial class HoaDonPreviewWindow : Window
    {
        public HoaDonPreviewWindow(HoaDon hoaDon, List<ChiTietHoaDon> chiTiet, NhanVien nv, string soBan, string tieuDe = "HÓA ĐƠN TẠM TÍNH", decimal tienKhachDua = 0)
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            lblDiaChi.Text = "Địa chỉ: " + (thongTin.ContainsKey("StoreAddress") ? thongTin["StoreAddress"] : "...");
            lblSdt.Text = "SĐT: " + (thongTin.ContainsKey("StorePhoneNumber") ? thongTin["StorePhoneNumber"] : "...");

            lblBanSo.Text = soBan;

            // SỬA LỖI HIỂN THỊ NGÀY GIỜ
            // Ưu tiên hiển thị thời gian thanh toán nếu có, nếu không thì hiển thị thời gian tạo
            DateTime displayTime = hoaDon.ThoiGianThanhToan.HasValue ? hoaDon.ThoiGianThanhToan.Value : hoaDon.ThoiGianTao;
            lblNgayGio.Text = displayTime.ToString("dd/MM/yyyy HH:mm");

            lblNhanVien.Text = nv.HoTen;
            lblTieuDeChinh.Text = tieuDe;

            if (!string.IsNullOrEmpty(hoaDon.PhuongThucThanhToan))
            {
                lblPhuongThucThanhToan.Text = hoaDon.PhuongThucThanhToan;
                panelPhuongThucThanhToan.Visibility = Visibility.Visible;
            }

            icChiTietHoaDon.ItemsSource = chiTiet;

            lblTongTien.Text = hoaDon.TongTien.ToString("N0");
            lblGiamGia.Text = hoaDon.SoTienGiam.ToString("N0");
            lblThanhTien.Text = hoaDon.ThanhTien.ToString("N0") + " VND";

            if (tieuDe == "HÓA ĐƠN THANH TOÁN" && (hoaDon.PhuongThucThanhToan == "Tiền mặt" || tienKhachDua > 0))
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