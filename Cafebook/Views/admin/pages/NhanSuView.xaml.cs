using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using Cafebook.Views.Common; // Đảm bảo có using này

namespace Cafebook.Views.admin.pages
{
    public partial class NhanSuView : Page
    {
        private NhanSuBUS nhanSuBUS = new NhanSuBUS();
        private PhieuLuong phieuLuongTamTinh;

        public NhanSuView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            ClearFormNV();
            ClearFormChinhSach();

            calLichLamViec.SelectedDate = DateTime.Today;
            dpNgayChamCong.SelectedDate = DateTime.Today;
            dpTuNgay.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpDenNgay.SelectedDate = dpTuNgay.SelectedDate.Value.AddMonths(1).AddDays(-1);
        }

        private void LoadAllData()
        {
            var dsNhanVien = nhanSuBUS.GetDanhSachNhanVien();
            var dsChinhSach = nhanSuBUS.GetChinhSach();

            dgNhanVien.ItemsSource = dsNhanVien;
            cmbVaiTro.ItemsSource = nhanSuBUS.GetDanhSachVaiTro();
            cmbNhanVien_Lich.ItemsSource = dsNhanVien;
            cmbCaLamViec_Lich.ItemsSource = nhanSuBUS.GetDanhSachCaLamViec();
            dgThuongPhat.ItemsSource = dsChinhSach;
            cmbNhanVien_Luong.ItemsSource = dsNhanVien;
        }

        #region NhanVien
        private void ClearFormNV()
        {
            dgNhanVien.SelectedItem = null;
            txtHoTen.Text = "";
            txtMatKhau.Password = "";
            txtMucLuong.Text = "20000";
            cmbVaiTro.SelectedIndex = -1;
            chkTrangThai.IsChecked = true;
            btnThemNV.IsEnabled = true;
            btnLuuNV.IsEnabled = false;
        }

        private void DgNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien selected)
            {
                txtHoTen.Text = selected.HoTen;
                txtMatKhau.Password = selected.MatKhau;
                txtMucLuong.Text = selected.MucLuongTheoGio.ToString("G0");
                cmbVaiTro.SelectedValue = selected.IdVaiTro;
                chkTrangThai.IsChecked = selected.TrangThai;
                btnThemNV.IsEnabled = false;
                btnLuuNV.IsEnabled = true;
            }
        }

        private void BtnLamMoiNV_Click(object sender, RoutedEventArgs e) => ClearFormNV();

        private void BtnThemNV_Click(object sender, RoutedEventArgs e)
        {
            var nv = new NhanVien
            {
                HoTen = txtHoTen.Text,
                MatKhau = txtMatKhau.Password,
                MucLuongTheoGio = decimal.TryParse(txtMucLuong.Text, out var luong) ? luong : 20000,
                IdVaiTro = (int)cmbVaiTro.SelectedValue,
                TrangThai = chkTrangThai.IsChecked ?? false
            };
            if (nhanSuBUS.ThemNhanVien(nv))
            {
                MessageBox.Show("Thêm nhân viên thành công!");
                LoadAllData();
                ClearFormNV();
            }
        }

        private void BtnLuuNV_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien selected)
            {
                selected.HoTen = txtHoTen.Text;
                selected.MatKhau = txtMatKhau.Password;
                selected.MucLuongTheoGio = decimal.TryParse(txtMucLuong.Text, out var luong) ? luong : 20000;
                selected.IdVaiTro = (int)cmbVaiTro.SelectedValue;
                selected.TrangThai = chkTrangThai.IsChecked ?? false;

                if (nhanSuBUS.SuaNhanVien(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadAllData();
                    ClearFormNV();
                }
            }
        }
        #endregion

        #region LichLamViec
        private void CalLichLamViec_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calLichLamViec.SelectedDate.HasValue)
            {
                DateTime selectedDate = calLichLamViec.SelectedDate.Value;
                lblNgayDaChon.Text = "Lịch làm việc ngày: " + selectedDate.ToString("dd/MM/yyyy");
                dgLichLamViec.ItemsSource = nhanSuBUS.GetLichLamViec(selectedDate);
            }
        }

        private void BtnThemLich_Click(object sender, RoutedEventArgs e)
        {
            if (calLichLamViec.SelectedDate.HasValue && cmbNhanVien_Lich.SelectedValue != null && cmbCaLamViec_Lich.SelectedValue != null)
            {
                var llv = new LichLamViec
                {
                    NgayLam = calLichLamViec.SelectedDate.Value,
                    IdNhanVien = (int)cmbNhanVien_Lich.SelectedValue,
                    IdCa = (int)cmbCaLamViec_Lich.SelectedValue
                };

                if (nhanSuBUS.ThemLichLamViec(llv))
                {
                    CalLichLamViec_SelectedDatesChanged(null, null);
                }
                else
                {
                    MessageBox.Show("Thêm lịch làm việc thất bại.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày, nhân viên và ca làm việc.");
            }
        }

        private void BtnXoaLich_Click(object sender, RoutedEventArgs e)
        {
            if (dgLichLamViec.SelectedItem is LichLamViec selected)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa lịch làm việc này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (nhanSuBUS.XoaLichLamViec(selected.IdLichLamViec))
                    {
                        CalLichLamViec_SelectedDatesChanged(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lịch làm việc để xóa.");
            }
        }
        #endregion

        #region ChinhSachLuong
        private void ClearFormChinhSach()
        {
            dgThuongPhat.SelectedItem = null;
            txtTenChinhSach.Text = "";
            txtSoTienChinhSach.Text = "";
            cmbLoaiChinhSach.SelectedIndex = -1;
            btnThemCS.IsEnabled = true;
            btnLuuCS.IsEnabled = false;
            btnXoaCS.IsEnabled = false;
        }

        private void DgThuongPhat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgThuongPhat.SelectedItem is LoaiThuongPhat selected)
            {
                txtTenChinhSach.Text = selected.TenLoai;
                txtSoTienChinhSach.Text = selected.SoTien.ToString("G0");
                cmbLoaiChinhSach.Text = selected.Loai;
                btnThemCS.IsEnabled = false;
                btnLuuCS.IsEnabled = true;
                btnXoaCS.IsEnabled = true;
            }
        }

        private void BtnLamMoiCS_Click(object sender, RoutedEventArgs e) => ClearFormChinhSach();

        private void BtnThemCS_Click(object sender, RoutedEventArgs e)
        {
            var ltp = new LoaiThuongPhat
            {
                TenLoai = txtTenChinhSach.Text,
                SoTien = decimal.TryParse(txtSoTienChinhSach.Text, out var tien) ? tien : 0,
                Loai = (cmbLoaiChinhSach.SelectedItem as ComboBoxItem)?.Content.ToString()
            };
            if (nhanSuBUS.ThemChinhSach(ltp))
            {
                MessageBox.Show("Thêm chính sách thành công!");
                LoadAllData();
                ClearFormChinhSach();
            }
        }

        private void BtnLuuCS_Click(object sender, RoutedEventArgs e)
        {
            if (dgThuongPhat.SelectedItem is LoaiThuongPhat selected)
            {
                selected.TenLoai = txtTenChinhSach.Text;
                selected.SoTien = decimal.TryParse(txtSoTienChinhSach.Text, out var tien) ? tien : 0;
                selected.Loai = (cmbLoaiChinhSach.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (nhanSuBUS.SuaChinhSach(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadAllData();
                    ClearFormChinhSach();
                }
            }
        }

        private void BtnXoaCS_Click(object sender, RoutedEventArgs e)
        {
            if (dgThuongPhat.SelectedItem is LoaiThuongPhat selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa chính sách '{selected.TenLoai}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (nhanSuBUS.XoaChinhSach(selected.IdLoai))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadAllData();
                        ClearFormChinhSach();
                    }
                }
            }
        }
        #endregion

        #region ChamCong & Luong
        private void DpNgayChamCong_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayChamCong.SelectedDate.HasValue)
            {
                dgChamCong.ItemsSource = nhanSuBUS.GetBangChamCong(dpNgayChamCong.SelectedDate.Value);
            }
        }

        private void BtnTuDongPhat_Click(object sender, RoutedEventArgs e)
        {
            if (dpNgayChamCong.SelectedDate.HasValue)
            {
                int soNhanVienBiPhat = nhanSuBUS.TuDongPhatDiTre(dpNgayChamCong.SelectedDate.Value, 15); // Phạt nếu trễ > 15 phút
                if (soNhanVienBiPhat >= 0)
                    MessageBox.Show($"Đã tự động thêm {soNhanVienBiPhat} khoản phạt đi trễ cho ngày {dpNgayChamCong.SelectedDate.Value:dd/MM/yyyy}.", "Hoàn tất");
                else
                    MessageBox.Show("Chưa có chính sách 'Phạt đi trễ' trong hệ thống. Vui lòng thêm ở tab Thiết lập.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbNhanVien_Luong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV)
            {
                dgPhieuLuong.ItemsSource = nhanSuBUS.GetLichSuPhieuLuong(selectedNV.IdNhanVien);
            }
        }

        private void BtnTinhLuong_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV && dpTuNgay.SelectedDate.HasValue && dpDenNgay.SelectedDate.HasValue)
            {
                phieuLuongTamTinh = nhanSuBUS.TinhLuong(selectedNV.IdNhanVien, dpTuNgay.SelectedDate.Value, dpDenNgay.SelectedDate.Value);

                if (phieuLuongTamTinh != null)
                {
                    runLuongTheoGio.Text = $"{selectedNV.MucLuongTheoGio:N0} đ/giờ";
                    runTongGioLam.Text = $"{phieuLuongTamTinh.TongGioLam:N2} giờ";
                    runTongThuong.Text = $"+ {phieuLuongTamTinh.TongThuong:N0} đ";
                    runTongPhat.Text = $"- {phieuLuongTamTinh.TongPhat:N0} đ";
                    runThucLanh.Text = $"{phieuLuongTamTinh.ThucLanh:N0} VND";
                    btnChotLuong.IsEnabled = true;
                }
            }
        }

        private void BtnChotLuong_Click(object sender, RoutedEventArgs e)
        {
            if (phieuLuongTamTinh != null)
            {
                if (nhanSuBUS.ChotPhieuLuong(phieuLuongTamTinh))
                {
                    MessageBox.Show("Chốt và tạo phiếu lương thành công!", "Thành công");
                    phieuLuongTamTinh = null;
                    btnChotLuong.IsEnabled = false;
                    LoadLichSuPhieuLuong();
                }
                else
                {
                    MessageBox.Show("Chốt lương thất bại.", "Lỗi");
                }
            }
        }

        private void LoadLichSuPhieuLuong()
        {
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV)
            {
                dgPhieuLuong.ItemsSource = nhanSuBUS.GetLichSuPhieuLuong(selectedNV.IdNhanVien);
            }
        }

        // HÀM BỊ THIẾU ĐÃ ĐƯỢC BỔ SUNG
        private void BtnXemPhieuLuong_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is PhieuLuong selectedPhieuLuong &&
                cmbNhanVien_Luong.SelectedItem is NhanVien selectedNhanVien)
            {
                // Để hiển thị đầy đủ, ta cần tính lại chi tiết phiếu lương dựa trên ngày đã lưu
                var phieuLuongChiTiet = nhanSuBUS.TinhLuong(selectedNhanVien.IdNhanVien, selectedPhieuLuong.TuNgay, selectedPhieuLuong.DenNgay);
                phieuLuongChiTiet.NgayTinhLuong = selectedPhieuLuong.NgayTinhLuong; // Giữ lại ngày chốt cũ

                var previewWindow = new PhieuLuongPreviewWindow(phieuLuongChiTiet, selectedNhanVien);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
            }
        }
        #endregion
    }
}