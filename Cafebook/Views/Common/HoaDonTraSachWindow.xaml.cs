using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.Common
{
    public partial class HoaDonTraSachWindow : Window
    {
        public HoaDonTraSachWindow(PhieuThueSach phieuTra)
        {
            InitializeComponent();
            var caiDatBUS = new CaiDatBUS();
            var thongTin = caiDatBUS.GetThongTinCuaHang();

            lblDiaChi.Text = "Địa chỉ: " + thongTin["StoreAddress"];
            lblSdt.Text = "SĐT: " + thongTin["StorePhoneNumber"];

            lblMaPhieu.Text = $"Mã phiếu thuê: {phieuTra.IdPhieuThue}";
            lblTenKhach.Text = $"Khách hàng: {phieuTra.TenKhachHang}";
            lblTenSach.Text = $"Sách đã thuê: {phieuTra.TieuDeSach}";

            lblPhiThue.Text = phieuTra.PhiThue.ToString("N0");
            lblTienPhat.Text = phieuTra.TienPhat.ToString("N0");
            lblTienCoc.Text = phieuTra.TienCoc.ToString("N0");

            decimal tongTien = phieuTra.PhiThue + phieuTra.TienPhat - phieuTra.TienCoc;

            if (tongTien >= 0)
            {
                lblTongTienTitle.Text = "Khách trả thêm:";
                lblTongTien.Text = tongTien.ToString("N0") + " VND";
            }
            else
            {
                lblTongTienTitle.Text = "Hoàn lại cho khách:";
                lblTongTien.Text = Math.Abs(tongTien).ToString("N0") + " VND";
                lblTongTien.Foreground = System.Windows.Media.Brushes.Green;
            }

            lblNgayTra.Text = $"Ngày trả: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        private void InHoaDon_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(printArea, "In Hóa Đơn Trả Sách");
            }
        }
    }
}