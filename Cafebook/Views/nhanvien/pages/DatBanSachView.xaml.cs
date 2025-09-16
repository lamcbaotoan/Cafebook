// Views/nhanvien/pages/DatBanSachView.xaml.cs

using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

// Placeholder for session management - in a real app, this would be in its own file
public static class LoginSession
{
    // Giả lập người dùng đăng nhập để code chạy được. Trong dự án thật, bạn sẽ lấy thông tin này sau khi đăng nhập thành công.
    public static NhanVien CurrentUser { get; set; } = new NhanVien { IdNhanVien = 1, HoTen = "Admin Demo" };
}


namespace Cafebook.Views.nhanvien.pages
{
    public partial class DatBanSachView : Page
    {
        private NghiepVuBUS nghiepVuBUS = new NghiepVuBUS();
        private KhachHangBUS khachHangBUS = new KhachHangBUS();
        private BanBUS banBUS = new BanBUS();

        private List<PhieuDatBan> danhSachPhieuDatBanHomNay = new List<PhieuDatBan>();
        private const decimal MucPhatMoiNgay = 5000;

        public DatBanSachView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dpChonNgayDatBan.SelectedDate = DateTime.Today;
            LoadDataDatBan();
            LoadDataThueSach();
        }

        #region Đặt Bàn
        private void LoadDataDatBan()
        {
            DateTime selectedDate = dpChonNgayDatBan.SelectedDate ?? DateTime.Today;
            danhSachPhieuDatBanHomNay = nghiepVuBUS.GetPhieuDatBan(selectedDate);
            dgPhieuDatBan.ItemsSource = danhSachPhieuDatBanHomNay;
            cmbBan.ItemsSource = banBUS.GetDanhSachBan();
        }

        // Phương thức này xử lý sự kiện khi người dùng chọn ngày khác
        private void DpChonNgayDatBan_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) // Chỉ chạy khi Page đã được load xong
            {
                LoadDataDatBan();
            }
        }

        // Phương thức này xử lý sự kiện click nút "Tạo Phiếu Đặt"
        private void BtnTaoPhieuDat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKhachDat.Text) || string.IsNullOrWhiteSpace(txtSdtKhachDat.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên và Số điện thoại khách hàng.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Regex.IsMatch(txtSdtKhachDat.Text, @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!TimeSpan.TryParse(txtGioPhutDat.Text, out TimeSpan gioDat))
            {
                MessageBox.Show("Giờ đặt không hợp lệ. Vui lòng nhập theo định dạng HH:mm (ví dụ: 19:30).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(txtSoLuongKhach.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng khách phải là một số lớn hơn 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (cmbBan.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một bàn.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime thoiGianDat = (dpChonNgayDatBan.SelectedDate ?? DateTime.Today).Date.Add(gioDat);
            if (thoiGianDat < DateTime.Now)
            {
                MessageBox.Show("Không thể đặt bàn cho thời gian trong quá khứ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pdb = new PhieuDatBan
            {
                TenKhachVangLai = txtTenKhachDat.Text,
                SdtKhachVangLai = txtSdtKhachDat.Text,
                ThoiGianDat = thoiGianDat,
                SoLuongKhach = soLuong,
                IdBan = (int)cmbBan.SelectedValue,
                GhiChu = txtGhiChuDat.Text,
                TrangThai = "Đã đặt"
            };

            string errorMessage = nghiepVuBUS.TaoPhieuDatBan(pdb);
            if (errorMessage == null)
            {
                MessageBox.Show("Tạo phiếu đặt bàn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataDatBan(); // Tải lại danh sách
                txtTenKhachDat.Clear();
                txtSdtKhachDat.Clear();
                txtSoLuongKhach.Clear();
                txtGhiChuDat.Clear();
                cmbBan.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức này xử lý sự kiện click nút "Xác nhận khách đã đến"
        private void BtnKhachDaDen_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhieuDatBan.SelectedItem is PhieuDatBan selected)
            {
                if (nghiepVuBUS.CapNhatTrangThaiPhieuDat(selected.IdPhieuDatBan, "Đã đến"))
                {
                    MessageBox.Show("Cập nhật trạng thái thành công.");
                    LoadDataDatBan();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu đặt để cập nhật.");
            }
        }

        // Phương thức này xử lý sự kiện click nút "Hủy Phiếu Đặt"
        private void BtnHuyPhieuDat_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhieuDatBan.SelectedItem is PhieuDatBan selected)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn hủy phiếu đặt này?", "Xác nhận hủy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (nghiepVuBUS.CapNhatTrangThaiPhieuDat(selected.IdPhieuDatBan, "Đã hủy"))
                    {
                        MessageBox.Show("Hủy phiếu thành công.");
                        LoadDataDatBan();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu đặt để hủy.");
            }
        }

        // Phương thức này xử lý sự kiện khi người dùng gõ vào ô tìm kiếm
        private void TxtTimKiemDatBan_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtTimKiemDatBan.Text.ToLower();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                dgPhieuDatBan.ItemsSource = danhSachPhieuDatBanHomNay;
            }
            else
            {
                dgPhieuDatBan.ItemsSource = danhSachPhieuDatBanHomNay
                    .Where(p => p.TenKhachHangHienThi.ToLower().Contains(keyword) || p.SdtHienThi.Contains(keyword))
                    .ToList();
            }
        }

        #endregion

        #region Thuê Sách
        // Lưu trữ danh sách khách hàng gốc để lọc
        private List<KhachHang> danhSachKhachHangDayDu;

        private void LoadDataThueSach()
        {
            // Lấy danh sách khách hàng đầy đủ và lưu vào biến tạm
            danhSachKhachHangDayDu = khachHangBUS.GetDanhSachKhachHang();
            cmbKhachHang_Thue.ItemsSource = danhSachKhachHangDayDu;

            cmbSach_Thue.ItemsSource = nghiepVuBUS.GetSachCoTheChoThue();

            // Tự động tải danh sách sách đang thuê và quá hạn
            dgPhieuDangThue.ItemsSource = nghiepVuBUS.GetPhieuDangThue();
            dgPhieuQuaHan.ItemsSource = nghiepVuBUS.GetPhieuQuaHan();

            dpNgayHenTra.SelectedDate = DateTime.Today.AddDays(7);
        }

        // === TÌM KIẾM KHÁCH HÀNG CHO THUÊ ===
        private void TxtTimKiemKhachHang_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtTimKiemKhachHang.Text.ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu ô tìm kiếm trống, hiển thị lại toàn bộ danh sách
                cmbKhachHang_Thue.ItemsSource = danhSachKhachHangDayDu;
            }
            else
            {
                // Lọc danh sách khách hàng gốc
                cmbKhachHang_Thue.ItemsSource = danhSachKhachHangDayDu
                    .Where(kh => kh.HoTen.ToLower().Contains(keyword) || kh.SoDienThoai.Contains(keyword))
                    .ToList();
            }
        }

        // === TÌM KIẾM PHIẾU TRẢ SÁCH ===
        private void TxtTimKiemPhieuTra_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = txtTimKiemPhieuTra.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                // Nếu không có từ khóa, tải lại danh sách mặc định
                // Chỉ tải lại nếu người dùng đã xóa hết chữ, tránh tải lại liên tục
                if (e.Changes.Any(c => c.RemovedLength > 0 && c.AddedLength == 0))
                {
                    dgPhieuDangThue.ItemsSource = nghiepVuBUS.GetPhieuDangThue();
                    dgPhieuQuaHan.ItemsSource = nghiepVuBUS.GetPhieuQuaHan();
                }
                return;
            }

            // Gọi hàm tìm kiếm từ BUS
            List<PhieuThueSach> ketQuaTimKiem = nghiepVuBUS.TimKiemPhieuThue(keyword);

            // Phân loại kết quả tìm kiếm vào 2 DataGrid
            dgPhieuDangThue.ItemsSource = ketQuaTimKiem
                .Where(p => p.TrangThai == "Đang thuê" && p.NgayHenTra.Date >= DateTime.Today.Date)
                .ToList();

            dgPhieuQuaHan.ItemsSource = ketQuaTimKiem
                .Where(p => p.TrangThai == "Quá hạn" || p.NgayHenTra.Date < DateTime.Today.Date)
                .ToList();
        }

        private void BtnXoaTimKiemPhieuTra_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemPhieuTra.Clear();
            // Việc clear textbox sẽ tự động kích hoạt sự kiện TextChanged và tải lại danh sách
        }

        private void ChkKhachHangMoi_Changed(object sender, RoutedEventArgs e)
        {
            bool isNewCustomer = chkKhachHangMoi.IsChecked == true;
            panelKhachCu.Visibility = isNewCustomer ? Visibility.Collapsed : Visibility.Visible;
            panelKhachMoi.Visibility = isNewCustomer ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DgPhieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null) return;

            if (grid.Name == "dgPhieuDangThue" && dgPhieuQuaHan.SelectedItem != null)
            {
                dgPhieuQuaHan.SelectedItem = null;
            }
            else if (grid.Name == "dgPhieuQuaHan" && dgPhieuDangThue.SelectedItem != null)
            {
                dgPhieuDangThue.SelectedItem = null;
            }

            if (grid.SelectedItem is PhieuThueSach selected)
            {
                gbTraSach.IsEnabled = true;
                lblChiTietThue.Text = $"Sách: {selected.TieuDeSach}\nKhách: {selected.TenKhachHang}\nHẹn trả: {selected.NgayHenTra:dd/MM/yyyy}";

                decimal tienPhat = 0;
                decimal mucPhat = CaiDatHelper.GetGiaTriDecimal("PhiPhatTreHan");
                if (DateTime.Today > selected.NgayHenTra)
                {
                    int soNgayTre = (DateTime.Today - selected.NgayHenTra).Days;
                    tienPhat = soNgayTre * mucPhat;
                }
                lblTienPhat.Text = tienPhat.ToString("N0") + " VND";
            }
            else
            {
                if (dgPhieuDangThue.SelectedItem == null && dgPhieuQuaHan.SelectedItem == null)
                {
                    gbTraSach.IsEnabled = false;
                    lblChiTietThue.Text = "Vui lòng chọn một phiếu từ danh sách bên trái.";
                    lblTienPhat.Text = "0 VND";
                }
            }
        }

        private void BtnXacNhanChoMuon_Click(object sender, RoutedEventArgs e)
        {
            int customerId = -1;

            if (chkKhachHangMoi.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtTenKhachMoi.Text) || string.IsNullOrWhiteSpace(txtSdtKhachMoi.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên và SĐT cho khách hàng mới."); return;
                }
                var newCustomer = new KhachHang { HoTen = txtTenKhachMoi.Text, SoDienThoai = txtSdtKhachMoi.Text };
                customerId = khachHangBUS.AddKhachHang(newCustomer);
                if (customerId == -1) { MessageBox.Show("Thêm khách hàng mới thất bại."); return; }
            }
            else
            {
                if (cmbKhachHang_Thue.SelectedItem == null) { MessageBox.Show("Vui lòng chọn một khách hàng thành viên."); return; }
                customerId = (int)cmbKhachHang_Thue.SelectedValue;
            }

            if (cmbSach_Thue.SelectedItem == null || dpNgayHenTra.SelectedDate == null) { MessageBox.Show("Vui lòng chọn Sách và Ngày hẹn trả."); return; }
            if (dpNgayHenTra.SelectedDate.Value.Date < DateTime.Today) { MessageBox.Show("Ngày hẹn trả không thể là một ngày trong quá khứ."); return; }

            var pts = new PhieuThueSach
            {
                IdKhachHang = customerId,
                IdSach = (int)cmbSach_Thue.SelectedValue,
                NgayHenTra = dpNgayHenTra.SelectedDate.Value,
                IdNhanVien = LoginSession.CurrentUser.IdNhanVien
            };

            PhieuThueSach phieuMoi = nghiepVuBUS.ThucHienChoMuon(pts);
            if (phieuMoi != null)
            {
                MessageBox.Show("Cho mượn sách thành công!", "Thành công");

                var previewWindow = new Common.PhieuThuePreviewWindow(phieuMoi);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();

                LoadDataThueSach();
                chkKhachHangMoi.IsChecked = false;
                txtTenKhachMoi.Clear();
                txtSdtKhachMoi.Clear();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra, không thể cho mượn sách.", "Lỗi");
            }
        }

        private void BtnXacNhanTraSach_Click(object sender, RoutedEventArgs e)
        {
            PhieuThueSach selected = (dgPhieuDangThue.SelectedItem as PhieuThueSach) ?? (dgPhieuQuaHan.SelectedItem as PhieuThueSach);

            if (selected != null)
            {
                decimal tienPhat = decimal.Parse(lblTienPhat.Text.Replace(" VND", "").Replace(",", ""));
                if (nghiepVuBUS.ThucHienTraSach(selected.IdPhieuThue, tienPhat))
                {
                    MessageBox.Show("Xác nhận trả sách thành công!", "Thành công");

                    selected.TienPhat = tienPhat;

                    var hoaDonWindow = new Common.HoaDonTraSachWindow(selected);
                    hoaDonWindow.Owner = Window.GetWindow(this);
                    hoaDonWindow.ShowDialog();

                    LoadDataThueSach();
                }
                else
                {
                    MessageBox.Show("Xác nhận trả sách thất bại.", "Lỗi");
                }
            }
        }
        #endregion
    }
}