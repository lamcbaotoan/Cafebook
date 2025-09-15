using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class DatBanSachView : Page
    {
        private NghiepVuBUS nghiepVuBUS = new NghiepVuBUS();
        // Giả sử có 1 biến lưu mức phạt mỗi ngày
        private const decimal MucPhatMoiNgay = 5000;

        public DatBanSachView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataDatBan();
            LoadDataThueSach();
        }

        #region Đặt Bàn
        private void LoadDataDatBan()
        {
            // Tải danh sách phiếu đặt bàn cho ngày hôm nay
            // Tải danh sách bàn trống vào cmbBan
        }
        // Các hàm xử lý sự kiện cho Tab Đặt Bàn
        #endregion

        #region Thuê Sách
        private void LoadDataThueSach()
        {
            // Tải danh sách khách hàng vào cmbKhachHang_Thue
            // Tải danh sách sách có thể thuê vào cmbSach_Thue
            dgPhieuThueSach.ItemsSource = nghiepVuBUS.GetPhieuDangThue();

            // Tự động gán ngày hẹn trả là 7 ngày sau
            dpNgayHenTra.SelectedDate = DateTime.Today.AddDays(7);
        }

        private void DgPhieuThueSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gbTraSach.IsEnabled = dgPhieuThueSach.SelectedItem != null;
            if (dgPhieuThueSach.SelectedItem is PhieuThueSach selected)
            {
                lblChiTietThue.Text = $"Sách: {selected.TieuDeSach}\nKhách: {selected.TenKhachHang}\nHẹn trả: {selected.NgayHenTra:dd/MM/yyyy}";

                // Tính tiền phạt
                decimal tienPhat = 0;
                if (DateTime.Today > selected.NgayHenTra)
                {
                    int soNgayTre = (DateTime.Today - selected.NgayHenTra).Days;
                    tienPhat = soNgayTre * MucPhatMoiNgay;
                }
                lblTienPhat.Text = tienPhat.ToString("N0") + " VND";
            }
        }

        private void BtnXacNhanChoMuon_Click(object sender, RoutedEventArgs e)
        {
            // Logic lấy thông tin từ các ComboBox và DatePicker
            // Tạo đối tượng PhieuThueSach
            // Gọi nghiepVuBUS.ThucHienChoMuon(pts)
            // Tải lại dữ liệu
        }

        private void BtnXacNhanTraSach_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhieuThueSach.SelectedItem is PhieuThueSach selected)
            {
                decimal tienPhat = decimal.Parse(lblTienPhat.Text.Replace(" VND", "").Replace(",", ""));
                if (nghiepVuBUS.ThucHienTraSach(selected.IdPhieuThue, tienPhat))
                {
                    MessageBox.Show("Xác nhận trả sách thành công!");
                    LoadDataThueSach();
                }
                else
                {
                    MessageBox.Show("Xác nhận trả sách thất bại.");
                }
            }
        }
        #endregion
    }
}