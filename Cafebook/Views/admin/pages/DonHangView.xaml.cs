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
            if (dgDonHang.SelectedItem is HoaDon selectedSummary)
            {
                // SỬA LỖI: Tải lại thông tin ĐẦY ĐỦ của hóa đơn từ BUS
                HoaDon hoaDonDayDu = donHangBUS.GetHoaDonDayDuById(selectedSummary.IdHoaDon);

                if (hoaDonDayDu == null)
                {
                    MessageBox.Show("Không thể tải chi tiết hóa đơn.", "Lỗi");
                    return;
                }

                // Lấy danh sách chi tiết món ăn
                var chiTiet = goiMonBUS.GetChiTietHoaDon(hoaDonDayDu.IdHoaDon);

                // Lấy thông tin nhân viên (đã có sẵn trong hoaDonDayDu)
                var nv = new NhanVien { HoTen = hoaDonDayDu.TenNhanVien };

                // Tạo cửa sổ xem trước với đối tượng hóa đơn ĐẦY ĐỦ
                var preview = new HoaDonPreviewWindow(hoaDonDayDu, chiTiet, nv, hoaDonDayDu.SoBan, "HÓA ĐƠN BÁN LẺ");
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