using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.Common
{
    public partial class PhieuThuePreviewWindow : Window
    {
        public PhieuThuePreviewWindow(PhieuThueSach phieuThue)
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            lblDiaChi.Text = "Địa chỉ: " + thongTin["StoreAddress"];
            lblSdt.Text = "SĐT: " + thongTin["StorePhoneNumber"];

            lblMaPhieu.Text = $"Mã phiếu: {phieuThue.IdPhieuThue}";
            lblTenKhach.Text = $"  - Tên: {phieuThue.TenKhachHang}";
            lblSdtKhach.Text = $"  - SĐT: {phieuThue.SdtKhachHang}";
            lblTenSach.Text = $"  - Tên sách: {phieuThue.TieuDeSach}";
            lblViTri.Text = $"  - Vị trí: {phieuThue.ViTriSach}";
            lblNgayThue.Text = $"Ngày thuê: {phieuThue.NgayThue:dd/MM/yyyy HH:mm}";
            lblNgayHenTra.Text = $"Ngày hẹn trả: {phieuThue.NgayHenTra:dd/MM/yyyy}";

            lblPhiThue.Text = phieuThue.PhiThue.ToString("N0");
            lblTienCoc.Text = phieuThue.TienCoc.ToString("N0") + " VND";
        }

        private void InPhieu_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(printArea, "In Phiếu Thuê Sách");
            }
        }
    }
}