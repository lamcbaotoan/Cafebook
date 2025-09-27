using Cafebook.BUS;
using Cafebook.DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cafebook.Views.admin.pages
{
    public partial class KhoView : Page
    {
        private KhoBUS khoBUS = new KhoBUS();
        private ObservableCollection<ChiTietPhieuNhap> chiTietPhieuNhapHienTai;
        private string duongDanFileHienTai;

        public KhoView()
        {
            InitializeComponent();
            chiTietPhieuNhapHienTai = new ObservableCollection<ChiTietPhieuNhap>();
            dgChiTietPhieuNhap.ItemsSource = chiTietPhieuNhapHienTai;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllData();
        }

        private void LoadAllData()
        {
            // Tải dữ liệu cho tất cả các tab và làm mới
            LoadTonKho();
            LoadNguyenLieu();
            LoadPhieuNhap();
            LoadNhaCungCap();
            ClearFormNCC();
            ClearFormNL();
            BtnTaoPhieuMoi_Click(null, null);
        }

        #region Tồn Kho
        private void LoadTonKho()
        {
            dgTonKho.ItemsSource = khoBUS.GetDanhSachNguyenLieu();
            dgCanhBao.ItemsSource = khoBUS.GetNguyenLieuCanhBao();
        }
        #endregion

        #region Quản lý Nguyên liệu
        private void LoadNguyenLieu()
        {
            var dsNguyenLieu = khoBUS.GetDanhSachNguyenLieu();
            dgNguyenLieu.ItemsSource = dsNguyenLieu;
            cmbNguyenLieu_PhieuNhap.ItemsSource = dsNguyenLieu;
        }

        private void ClearFormNL()
        {
            dgNguyenLieu.SelectedItem = null;
            txtTenNL_crud.Text = "";
            cmbDonViTinh_crud.SelectedIndex = -1;
            txtNguongCanhBao_crud.Text = "5";
            btnThemNL.IsEnabled = true;
            btnLuuNL.IsEnabled = false;
            btnXoaNL.IsEnabled = false;
        }

        private void DgNguyenLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNguyenLieu.SelectedItem is NguyenLieu selected)
            {
                txtTenNL_crud.Text = selected.TenNguyenLieu;
                cmbDonViTinh_crud.Text = selected.DonViTinh;
                txtNguongCanhBao_crud.Text = selected.NguongCanhBao.ToString();
                btnThemNL.IsEnabled = false;
                btnLuuNL.IsEnabled = true;
                btnXoaNL.IsEnabled = true;
            }
        }

        private void BtnLamMoiNL_Click(object sender, RoutedEventArgs e) => ClearFormNL();

        private void BtnThemNL_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenNL_crud.Text) || string.IsNullOrWhiteSpace(cmbDonViTinh_crud.Text) || string.IsNullOrWhiteSpace(txtNguongCanhBao_crud.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }
            var nl = new NguyenLieu
            {
                TenNguyenLieu = txtTenNL_crud.Text,
                DonViTinh = cmbDonViTinh_crud.Text,
                NguongCanhBao = decimal.TryParse(txtNguongCanhBao_crud.Text, out var nguong) ? nguong : 5
            };
            if (khoBUS.ThemNguyenLieu(nl))
            {
                MessageBox.Show("Thêm nguyên liệu thành công!");
                LoadAllData();
            }
        }

        private void BtnLuuNL_Click(object sender, RoutedEventArgs e)
        {
            if (dgNguyenLieu.SelectedItem is NguyenLieu selected)
            {
                selected.TenNguyenLieu = txtTenNL_crud.Text;
                selected.DonViTinh = cmbDonViTinh_crud.Text;
                selected.NguongCanhBao = decimal.TryParse(txtNguongCanhBao_crud.Text, out var nguong) ? nguong : 5;
                if (khoBUS.SuaNguyenLieu(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadAllData();
                }
            }
        }

        private void BtnXoaNL_Click(object sender, RoutedEventArgs e)
        {
            if (dgNguyenLieu.SelectedItem is NguyenLieu selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa nguyên liệu '{selected.TenNguyenLieu}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (khoBUS.XoaNguyenLieu(selected.IdNguyenLieu))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadAllData();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại. Nguyên liệu này đang được sử dụng trong công thức, không thể xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region Nhà Cung Cấp
        private void LoadNhaCungCap()
        {
            var dsncc = khoBUS.GetNhaCungCap();
            dgNhaCungCap.ItemsSource = dsncc;
            cmbNhaCungCap_PhieuNhap.ItemsSource = dsncc;
        }

        private void ClearFormNCC()
        {
            dgNhaCungCap.SelectedItem = null;
            txtTenNCC.Text = "";
            txtSdtNCC.Text = "";
            txtDiaChiNCC.Text = "";
            btnLuuNCC.IsEnabled = false;
            btnThemNCC.IsEnabled = true;
            btnXoaNCC.IsEnabled = false;
        }

        private void DgNhaCungCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNhaCungCap.SelectedItem is NhaCungCap selected)
            {
                txtTenNCC.Text = selected.TenNhaCungCap;
                txtSdtNCC.Text = selected.SoDienThoai;
                txtDiaChiNCC.Text = selected.DiaChi;
                btnLuuNCC.IsEnabled = true;
                btnThemNCC.IsEnabled = false;
                btnXoaNCC.IsEnabled = true;
            }
        }

        private void BtnThemNCC_Click(object sender, RoutedEventArgs e)
        {
            var ncc = new NhaCungCap { TenNhaCungCap = txtTenNCC.Text, SoDienThoai = txtSdtNCC.Text, DiaChi = txtDiaChiNCC.Text };
            if (khoBUS.ThemNhaCungCap(ncc))
            {
                MessageBox.Show("Thêm nhà cung cấp thành công!");
                LoadNhaCungCap();
                ClearFormNCC();
            }
        }

        private void BtnLuuNCC_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhaCungCap.SelectedItem is NhaCungCap selected)
            {
                selected.TenNhaCungCap = txtTenNCC.Text;
                selected.SoDienThoai = txtSdtNCC.Text;
                selected.DiaChi = txtDiaChiNCC.Text;
                if (khoBUS.SuaNhaCungCap(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadNhaCungCap();
                    ClearFormNCC();
                }
            }
        }
        private void BtnLamMoiNCC_Click(object sender, RoutedEventArgs e) => ClearFormNCC();

        private void BtnXoaNCC_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhaCungCap.SelectedItem is NhaCungCap selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa NCC '{selected.TenNhaCungCap}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (khoBUS.XoaNhaCungCap(selected.IdNhaCungCap))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadNhaCungCap();
                        ClearFormNCC();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại. NCC này đã có phiếu nhập, không thể xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region Nhập Kho
        private void LoadPhieuNhap()
        {
            dgPhieuNhap.ItemsSource = khoBUS.GetPhieuNhapKho();
        }

        private void DgPhieuNhap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnLuuPhieuNhap.IsEnabled = false;
            btnThemVaoPhieu.IsEnabled = false;
            //lỗi khi chọn nguyên liệu ở nhập kho
            //imgHoaDon.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/placeholder.png"));
            lblDuongDanFile.Text = "Chưa có ảnh hóa đơn.";
            duongDanFileHienTai = null;

            if (dgPhieuNhap.SelectedItem is PhieuNhapKho selected)
            {
                var chiTiet = khoBUS.GetChiTietPhieuNhap(selected.IdPhieuNhap);
                chiTietPhieuNhapHienTai.Clear();
                foreach (var item in chiTiet) { chiTietPhieuNhapHienTai.Add(item); }
                lblTongTien.Text = selected.TongTien.ToString("N0") + " VND";

                var hddv = khoBUS.GetHoaDonDauVao(selected.IdPhieuNhap);
                if (hddv != null && !string.IsNullOrEmpty(hddv.DuongDanFile))
                {
                    try
                    {
                        imgHoaDon.Source = new BitmapImage(new Uri(hddv.DuongDanFile));
                        lblDuongDanFile.Text = hddv.DuongDanFile;
                        duongDanFileHienTai = hddv.DuongDanFile;
                    }
                    catch { /* Bỏ qua lỗi nếu không tìm thấy file ảnh */ }
                }
            }
        }

        private void BtnTaoPhieuMoi_Click(object sender, RoutedEventArgs e)
        {
            dgPhieuNhap.SelectedItem = null;
            chiTietPhieuNhapHienTai.Clear();
            dpNgayNhap.SelectedDate = DateTime.Now;
            cmbNhaCungCap_PhieuNhap.SelectedIndex = -1;
            lblTongTien.Text = "0 VND";
            btnLuuPhieuNhap.IsEnabled = true;
            btnThemVaoPhieu.IsEnabled = true;
        }

        private void CmbNguyenLieu_PhieuNhap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNguyenLieu_PhieuNhap.SelectedItem is NguyenLieu selectedNL)
            {
                var donViGoiY = new List<string>();
                string baseUnit = selectedNL.DonViTinh.ToLower();
                if (baseUnit == "kg") donViGoiY.AddRange(new[] { "g", "kg" });
                else if (baseUnit == "lít" || baseUnit == "l") donViGoiY.AddRange(new[] { "ml", "lít" });
                else donViGoiY.Add(selectedNL.DonViTinh);
                cmbDonViNhap.ItemsSource = donViGoiY;
                cmbDonViNhap.SelectedIndex = 0;
            }
        }

        private decimal ConvertToBaseUnit(decimal quantity, string selectedUnit, string baseUnit)
        {
            selectedUnit = selectedUnit.ToLower();
            baseUnit = baseUnit.ToLower();

            if (selectedUnit == baseUnit) return quantity;
            if (selectedUnit == "g" && baseUnit == "kg") return quantity / 1000;
            if (selectedUnit == "ml" && (baseUnit == "lít" || baseUnit == "l")) return quantity / 1000;

            return quantity;
        }

        private void BtnThemVaoPhieu_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNguyenLieu_PhieuNhap.SelectedItem is NguyenLieu selectedNL &&
                decimal.TryParse(txtSoLuongNL.Text, out var soLuong) &&
                decimal.TryParse(txtDonGiaNL.Text, out var donGia) &&
                cmbDonViNhap.SelectedItem != null)
            {
                decimal soLuongChuan = ConvertToBaseUnit(soLuong, cmbDonViNhap.SelectedItem.ToString(), selectedNL.DonViTinh);

                var chiTietMoi = new ChiTietPhieuNhap
                {
                    IdNguyenLieu = selectedNL.IdNguyenLieu,
                    TenNguyenLieu = selectedNL.TenNguyenLieu,
                    SoLuong = soLuongChuan,
                    DonGia = donGia
                };
                chiTietPhieuNhapHienTai.Add(chiTietMoi);
                CapNhatTongTien();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu, nhập số lượng, đơn giá và chọn đơn vị tính hợp lệ.");
            }
        }

        private void CapNhatTongTien()
        {
            decimal tongTien = chiTietPhieuNhapHienTai.Sum(ct => ct.ThanhTien);
            lblTongTien.Text = tongTien.ToString("N0") + " VND";
        }

        private void BtnLuuPhieuNhap_Click(object sender, RoutedEventArgs e)
        {
            if (dpNgayNhap.SelectedDate == null || cmbNhaCungCap_PhieuNhap.SelectedValue == null || chiTietPhieuNhapHienTai.Count == 0)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin và thêm ít nhất một nguyên liệu.");
                return;
            }

            var pnk = new PhieuNhapKho
            {
                NgayNhap = dpNgayNhap.SelectedDate.Value,
                IdNhaCungCap = (int)cmbNhaCungCap_PhieuNhap.SelectedValue,
                IdNhanVien = 1, // Giả sử admin đăng nhập có ID = 1
                TongTien = chiTietPhieuNhapHienTai.Sum(ct => ct.ThanhTien)
            };

            if (khoBUS.TaoPhieuNhap(pnk, chiTietPhieuNhapHienTai.ToList()))
            {
                MessageBox.Show("Tạo phiếu nhập kho thành công!");
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Tạo phiếu nhập kho thất bại. Đã xảy ra lỗi.");
            }
        }

        private void BtnChonAnhHD_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                duongDanFileHienTai = openFileDialog.FileName;
                lblDuongDanFile.Text = duongDanFileHienTai;
                imgHoaDon.Source = new BitmapImage(new Uri(duongDanFileHienTai));
            }
        }

        private void BtnLuuHD_Click(object sender, RoutedEventArgs e)
        {
            if (dgPhieuNhap.SelectedItem is PhieuNhapKho selectedPN && !string.IsNullOrEmpty(duongDanFileHienTai))
            {
                var hddv = new HoaDonDauVao
                {
                    IdPhieuNhap = selectedPN.IdPhieuNhap,
                    DuongDanFile = duongDanFileHienTai
                };
                if (khoBUS.LuuHoaDonDauVao(hddv))
                {
                    MessageBox.Show("Lưu ảnh hóa đơn thành công!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu nhập và chọn ảnh hóa đơn.");
            }
        }
        #endregion
    }
}