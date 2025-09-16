using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class DonHangView : Page
    {
        private DonHangBUS donHangBUS = new DonHangBUS();
        private GoiMonBUS goiMonBUS = new GoiMonBUS(); // Dùng để lấy chi tiết hóa đơn

        public DonHangView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dpTuNgay.SelectedDate = DateTime.Today;
            dpDenNgay.SelectedDate = DateTime.Today;
            cmbTrangThai.SelectedIndex = 0; // Tất cả
            LoadDonHang();
        }

        private void LoadDonHang()
        {
            DateTime tuNgay = dpTuNgay.SelectedDate ?? DateTime.MinValue;
            DateTime denNgay = dpDenNgay.SelectedDate ?? DateTime.MaxValue;
            string trangThai = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Tất cả";

            dgDonHang.ItemsSource = donHangBUS.GetDanhSachDonHang(tuNgay, denNgay, trangThai);
            ClearDetails();
        }

        private void ClearDetails()
        {
            dgChiTietDonHang.ItemsSource = null;
            btnInLaiHoaDon.IsEnabled = false;
            btnHuyDonHang.IsEnabled = false;
        }

        private void BtnLoc_Click(object sender, RoutedEventArgs e)
        {
            LoadDonHang();
        }

        private void BtnXoaLoc_Click(object sender, RoutedEventArgs e)
        {
            Page_Loaded(null, null);
        }

        private void DgDonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDonHang.SelectedItem is HoaDon selected)
            {
                dgChiTietDonHang.ItemsSource = goiMonBUS.GetChiTietHoaDon(selected.IdHoaDon);
                btnInLaiHoaDon.IsEnabled = true;
                btnHuyDonHang.IsEnabled = selected.TrangThai == "Chưa thanh toán";
            }
            else
            {
                ClearDetails();
            }
        }

        private void BtnInLaiHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is HoaDon selected)
            {
                // Cần lấy lại thông tin đầy đủ của hóa đơn và nhân viên để in
                // Tạm thời mô phỏng với thông tin có sẵn
                var chiTiet = goiMonBUS.GetChiTietHoaDon(selected.IdHoaDon);
                var nv = new NhanVien { HoTen = selected.TenNhanVien };

                var preview = new HoaDonPreviewWindow(selected, chiTiet, nv, selected.SoBan, "HÓA ĐƠN BÁN LẺ");
                preview.Owner = Window.GetWindow(this);
                preview.ShowDialog();
            }
        }

        private void BtnHuyDonHang_Click(object sender, RoutedEventArgs e)
        {
            if (dgDonHang.SelectedItem is HoaDon selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn hủy vĩnh viễn hóa đơn #{selected.IdHoaDon}?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (donHangBUS.HuyDonHang(selected.IdHoaDon))
                    {
                        MessageBox.Show("Hủy đơn hàng thành công!");
                        LoadDonHang();
                    }
                    else
                    {
                        MessageBox.Show("Hủy đơn hàng thất bại. Hóa đơn có thể đã được thanh toán hoặc có lỗi xảy ra.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}