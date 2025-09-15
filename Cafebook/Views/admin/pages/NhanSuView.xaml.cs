using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class NhanSuView : Page
    {
        private NhanSuBUS nhanSuBUS = new NhanSuBUS();

        public NhanSuView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            ClearFormNV();

            calLichLamViec.SelectedDate = DateTime.Today;
            dpNgayChamCong.SelectedDate = DateTime.Today;
            dpTuNgay.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpDenNgay.SelectedDate = DateTime.Today;
        }

        private void LoadAllData()
        {
            var dsNhanVien = nhanSuBUS.GetDanhSachNhanVien();
            dgNhanVien.ItemsSource = dsNhanVien;
            cmbNhanVien_Lich.ItemsSource = dsNhanVien;
            cmbNhanVien_Luong.ItemsSource = dsNhanVien;

            cmbVaiTro.ItemsSource = nhanSuBUS.GetDanhSachVaiTro();
            cmbCaLamViec_Lich.ItemsSource = nhanSuBUS.GetDanhSachCaLamViec();
        }

        #region NhanVien
        private void ClearFormNV()
        {
            dgNhanVien.SelectedItem = null;
            txtHoTen.Text = "";
            txtSdt.Text = "";
            txtEmail.Text = "";
            txtMatKhau.Password = "";
            txtDiaChi.Text = "";
            cmbVaiTro.SelectedIndex = -1;
            dpNgayVaoLam.SelectedDate = DateTime.Today;
            chkTrangThai.IsChecked = true;
            btnThemNV.IsEnabled = true;
            btnLuuNV.IsEnabled = false;
        }

        private void DgNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien selected)
            {
                txtHoTen.Text = selected.HoTen;
                txtSdt.Text = selected.SoDienThoai;
                txtEmail.Text = selected.Email;
                txtMatKhau.Password = selected.MatKhau;
                txtDiaChi.Text = selected.DiaChi;
                cmbVaiTro.SelectedValue = selected.IdVaiTro;
                dpNgayVaoLam.SelectedDate = selected.NgayVaoLam;
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
                SoDienThoai = txtSdt.Text,
                Email = txtEmail.Text,
                MatKhau = txtMatKhau.Password,
                DiaChi = txtDiaChi.Text,
                IdVaiTro = (int)cmbVaiTro.SelectedValue,
                NgayVaoLam = dpNgayVaoLam.SelectedDate ?? DateTime.Today,
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
                selected.SoDienThoai = txtSdt.Text;
                selected.Email = txtEmail.Text;
                selected.MatKhau = txtMatKhau.Password;
                selected.DiaChi = txtDiaChi.Text;
                selected.IdVaiTro = (int)cmbVaiTro.SelectedValue;
                selected.NgayVaoLam = dpNgayVaoLam.SelectedDate ?? DateTime.Today;
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
                    CalLichLamViec_SelectedDatesChanged(null, null); // Refresh datagrid
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
                        CalLichLamViec_SelectedDatesChanged(null, null); // Refresh datagrid
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

        #region ChamCong & Luong
        private void DpNgayChamCong_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpNgayChamCong.SelectedDate.HasValue)
            {
                dgChamCong.ItemsSource = nhanSuBUS.GetBangChamCong(dpNgayChamCong.SelectedDate.Value);
            }
        }

        private void BtnXemTongGio_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNhanVien_Luong.SelectedValue != null && dpTuNgay.SelectedDate.HasValue && dpDenNgay.SelectedDate.HasValue)
            {
                int idNV = (int)cmbNhanVien_Luong.SelectedValue;
                DateTime tuNgay = dpTuNgay.SelectedDate.Value;
                DateTime denNgay = dpDenNgay.SelectedDate.Value;

                decimal tongGio = nhanSuBUS.GetTongGioLam(idNV, tuNgay, denNgay);
                lblTongGioLam.Text = tongGio.ToString("N2") + " giờ";
            }
        }

        private void BtnTinhLuong_Click(object sender, RoutedEventArgs e)
        {
            decimal.TryParse(lblTongGioLam.Text.Replace(" giờ", ""), out decimal tongGio);
            decimal.TryParse(txtMucLuongGio.Text, out decimal mucLuong);

            decimal tienLuong = tongGio * mucLuong;
            lblTienLuong.Text = tienLuong.ToString("N0") + " VND";
        }
        #endregion
    }
}