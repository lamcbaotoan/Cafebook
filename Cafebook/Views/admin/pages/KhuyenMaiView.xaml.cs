using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class KhuyenMaiView : Page
    {
        // Khai báo các lớp BUS cần thiết
        private KhachHangBUS khachHangBUS = new KhachHangBUS();
        private KhuyenMaiBUS khuyenMaiBUS = new KhuyenMaiBUS();
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();

        public KhuyenMaiView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataKhachHang();
            LoadDataKhuyenMai();
        }

        #region Quản lý Khách hàng
        private void LoadDataKhachHang()
        {
            dgKhachHang.ItemsSource = khachHangBUS.GetDanhSachKhachHang();
            ClearFormKH();
        }

        private void ClearFormKH()
        {
            dgKhachHang.SelectedItem = null;
            txtHoTenKH.Clear();
            txtSdtKH.Clear();
            dgLichSuDonHang.ItemsSource = null;
            dgLichSuThueSach.ItemsSource = null;
            btnThemKH.IsEnabled = true;
            btnLuuKH.IsEnabled = false;
        }

        private void DgKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKhachHang.SelectedItem is KhachHang selected)
            {
                txtHoTenKH.Text = selected.HoTen;
                txtSdtKH.Text = selected.SoDienThoai;
                dgLichSuDonHang.ItemsSource = khachHangBUS.GetLichSuDonHang(selected.IdKhachHang);
                dgLichSuThueSach.ItemsSource = khachHangBUS.GetLichSuThueSach(selected.IdKhachHang);
                btnThemKH.IsEnabled = false;
                btnLuuKH.IsEnabled = true;
            }
        }

        private void BtnLamMoiKH_Click(object sender, RoutedEventArgs e) => ClearFormKH();

        private void BtnThemKH_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTenKH.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng.", "Thiếu thông tin"); return;
            }
            if (!string.IsNullOrEmpty(txtSdtKH.Text) && !Regex.IsMatch(txtSdtKH.Text, @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi"); return;
            }

            var kh = new KhachHang { HoTen = txtHoTenKH.Text, SoDienThoai = txtSdtKH.Text };
            if (khachHangBUS.ThemKhachHang(kh))
            {
                MessageBox.Show("Thêm khách hàng thành công!");
                LoadDataKhachHang();
            }
            else { MessageBox.Show("Thêm thất bại. SĐT có thể đã tồn tại."); }
        }

        private void BtnLuuKH_Click(object sender, RoutedEventArgs e)
        {
            if (dgKhachHang.SelectedItem is KhachHang selected)
            {
                if (string.IsNullOrWhiteSpace(txtHoTenKH.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên khách hàng.", "Thiếu thông tin"); return;
                }
                if (!string.IsNullOrEmpty(txtSdtKH.Text) && !Regex.IsMatch(txtSdtKH.Text, @"^0\d{9,10}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ.", "Lỗi"); return;
                }
                selected.HoTen = txtHoTenKH.Text;
                selected.SoDienThoai = txtSdtKH.Text;
                if (khachHangBUS.SuaKhachHang(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadDataKhachHang();
                }
                else { MessageBox.Show("Cập nhật thất bại. SĐT có thể đã tồn tại."); }
            }
        }
        #endregion

        #region Quản lý Khuyến mãi
        private void LoadDataKhuyenMai()
        {
            dgKhuyenMai.ItemsSource = khuyenMaiBUS.GetDanhSachKhuyenMai();
            var dsSP = sanPhamBUS.GetDanhSachSanPham();
            dsSP.Insert(0, new SanPham { IdSanPham = 0, TenSanPham = "Không yêu cầu" });
            cmbSanPhamApDung.ItemsSource = dsSP;
            ClearFormKM();
        }

        private void ClearFormKM()
        {
            dgKhuyenMai.SelectedItem = null;
            txtTenKM.Text = "";
            txtMoTa.Text = "";
            cmbLoaiGiamGia.SelectedIndex = -1;
            txtGiaTriGiam.Text = "";
            dpNgayBatDau.SelectedDate = DateTime.Today;
            dpNgayKetThuc.SelectedDate = DateTime.Today.AddDays(7);
            txtDonHangToiThieu.Text = "";
            cmbSanPhamApDung.SelectedIndex = 0;
            btnThem.IsEnabled = true;
            btnLuu.IsEnabled = false;
            btnXoa.IsEnabled = false;
        }

        private void DgKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKhuyenMai.SelectedItem is KhuyenMai selected)
            {
                txtTenKM.Text = selected.TenKhuyenMai;
                txtMoTa.Text = selected.MoTa;
                cmbLoaiGiamGia.Text = selected.LoaiGiamGia;
                txtGiaTriGiam.Text = selected.GiaTriGiam.ToString();
                dpNgayBatDau.SelectedDate = selected.NgayBatDau;
                dpNgayKetThuc.SelectedDate = selected.NgayKetThuc;
                txtDonHangToiThieu.Text = selected.GiaTriDonHangToiThieu?.ToString();
                cmbSanPhamApDung.SelectedValue = selected.IdSanPhamApDung ?? 0;
                btnThem.IsEnabled = false;
                btnLuu.IsEnabled = true;
                btnXoa.IsEnabled = true;
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e) => ClearFormKM();

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            var km = new KhuyenMai();
            if (!GetDataFromForm(km)) return;
            if (khuyenMaiBUS.ThemKhuyenMai(km))
            {
                MessageBox.Show("Thêm chương trình khuyến mãi thành công!");
                LoadDataKhuyenMai();
                ClearFormKM();
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKhuyenMai.SelectedItem is KhuyenMai selected)
            {
                if (!GetDataFromForm(selected)) return;
                if (khuyenMaiBUS.SuaKhuyenMai(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadDataKhuyenMai();
                    ClearFormKM();
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgKhuyenMai.SelectedItem is KhuyenMai selected)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa khuyến mãi này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (khuyenMaiBUS.XoaKhuyenMai(selected.IdKhuyenMai))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadDataKhuyenMai();
                        ClearFormKM();
                    }
                }
            }
        }

        private bool GetDataFromForm(KhuyenMai km)
        {
            if (string.IsNullOrWhiteSpace(txtTenKM.Text) || cmbLoaiGiamGia.SelectedItem == null || dpNgayBatDau.SelectedDate == null || dpNgayKetThuc.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ các trường bắt buộc.");
                return false;
            }
            km.TenKhuyenMai = txtTenKM.Text;
            km.MoTa = txtMoTa.Text;
            km.LoaiGiamGia = (cmbLoaiGiamGia.SelectedItem as ComboBoxItem)?.Content.ToString();
            km.GiaTriGiam = decimal.TryParse(txtGiaTriGiam.Text, out var gia) ? gia : 0;
            km.NgayBatDau = dpNgayBatDau.SelectedDate.Value;
            km.NgayKetThuc = dpNgayKetThuc.SelectedDate.Value;
            km.GiaTriDonHangToiThieu = decimal.TryParse(txtDonHangToiThieu.Text, out var min) ? min : (decimal?)null;
            km.IdSanPhamApDung = (int)cmbSanPhamApDung.SelectedValue == 0 ? (int?)null : (int)cmbSanPhamApDung.SelectedValue;
            return true;
        }
        #endregion
    }
}