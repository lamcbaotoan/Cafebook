using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class NhanSuView : Page
    {
        private NhanSuBUS nhanSuBUS = new NhanSuBUS();
        private PhieuLuong phieuLuongTamTinh;
        private ChiTietThuongPhatDTO selectedChiTiet = null;

        public NhanSuView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            ClearFormNV();
            ClearFormQuyTac();

            calLichLamViec.SelectedDate = DateTime.Today;
            dpNgayChamCong.SelectedDate = DateTime.Today;
            dpTuNgay.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dpDenNgay.SelectedDate = dpTuNgay.SelectedDate.Value.AddMonths(1).AddDays(-1);
        }

        private void LoadAllData()
        {
            var dsNhanVien = nhanSuBUS.GetDanhSachNhanVien();
            dgNhanVien.ItemsSource = dsNhanVien;
            cmbVaiTro.ItemsSource = nhanSuBUS.GetDanhSachVaiTro();
            cmbNhanVien_Lich.ItemsSource = dsNhanVien;
            cmbCaLamViec_Mau.ItemsSource = nhanSuBUS.GetDanhSachCaLamViec();
            cmbNhanVien_Luong.ItemsSource = dsNhanVien;
            LoadQuyTacData();
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
                txtMatKhau.Password = "";
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
                MatKhau = txtMatKhau.Password, // Cần mã hóa trong thực tế
                MucLuongTheoGio = decimal.TryParse(txtMucLuong.Text, out var luong) ? luong : 20000,
                IdVaiTro = (int)cmbVaiTro.SelectedValue,
                TrangThai = chkTrangThai.IsChecked ?? false,
                NgayVaoLam = DateTime.Now
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
                // Chỉ cập nhật mật khẩu nếu người dùng nhập
                if (!string.IsNullOrEmpty(txtMatKhau.Password))
                {
                    selected.MatKhau = txtMatKhau.Password;
                }
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
                LoadLichLamViecAdmin(calLichLamViec.SelectedDate.Value);
            }
        }

        private void LoadLichLamViecAdmin(DateTime date)
        {
            lblNgayDaChon.Text = "Lịch làm việc ngày: " + date.ToString("dd/MM/yyyy");
            dgLichLamViec.ItemsSource = nhanSuBUS.GetLichLamViec(date);
        }

        private void BtnThemLich_Click(object sender, RoutedEventArgs e)
        {
            if (calLichLamViec.SelectedDate.HasValue && cmbNhanVien_Lich.SelectedValue != null &&
                TimeSpan.TryParse(txtGioBatDau.Text, out var gioBatDau) &&
                TimeSpan.TryParse(txtGioKetThuc.Text, out var gioKetThuc))
            {
                var llv = new LichLamViec
                {
                    NgayLam = calLichLamViec.SelectedDate.Value,
                    IdNhanVien = (int)cmbNhanVien_Lich.SelectedValue,
                    GioBatDau = gioBatDau,
                    GioKetThuc = gioKetThuc,
                    TrangThai = (cmbTrangThaiLich.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Đi Làm"
                };

                if (nhanSuBUS.ThemLichLamViec(llv))
                {
                    LoadLichLamViecAdmin(calLichLamViec.SelectedDate.Value);
                }
                else
                {
                    MessageBox.Show("Thêm lịch làm việc thất bại.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngày, nhân viên và nhập đúng định dạng giờ (HH:mm).");
            }
        }

        private void BtnXoaLich_Click(object sender, RoutedEventArgs e)
        {
            if (dgLichLamViec.SelectedValue is int idLich)
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa lịch làm việc này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (nhanSuBUS.XoaLichLamViec(idLich, out string reason))
                    {
                        LoadLichLamViecAdmin(calLichLamViec.SelectedDate.Value);
                    }
                    else
                    {
                        MessageBox.Show(reason, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lịch để xóa.");
            }
        }
        #endregion

        #region QuyTacLuong
        private void LoadQuyTacData()
        {
            var dsQuyTac = nhanSuBUS.GetQuyTacLuong();
            dgQuyTacLuong.ItemsSource = dsQuyTac;

            // **THÊM DÒNG NÀY VÀO**
            cmbQuyTac_Mau.ItemsSource = dsQuyTac;
        }

        private void ClearFormQuyTac()
        {
            dgQuyTacLuong.SelectedItem = null;
            txtTenQuyTac.Clear();
            txtDieuKien.Clear();
            txtGiaTriApDung.Clear();
            cmbLoaiQuyTac.SelectedIndex = 0;
            cmbDonViTinh.SelectedIndex = 0;
            cmbLoaiThuongPhat.SelectedIndex = 0;
            btnThemQT.IsEnabled = true;
            btnLuuQT.IsEnabled = false;
            btnXoaQT.IsEnabled = false;
            txtDieuKien.IsEnabled = true; // Trả về trạng thái cho phép nhập mặc định
        }

        // Trong file: Views/admin/pages/NhanSuView.xaml.cs
        private void CmbLoaiQuyTac_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return;
            if (cmbLoaiQuyTac.SelectedItem is ComboBoxItem selectedItem)
            {
                string ruleType = selectedItem.Tag?.ToString();
                if (ruleType == "LATE" || ruleType == "OVERTIME" || ruleType == "MONTHLY_HOURS")
                {
                    txtDieuKien.IsEnabled = true;
                    cmbDieuKienDonViTinh.IsEnabled = true; // Mở khóa ComboBox đơn vị
                }
                else
                {
                    txtDieuKien.IsEnabled = false;
                    cmbDieuKienDonViTinh.IsEnabled = false; // Khóa ComboBox đơn vị
                    txtDieuKien.Clear();
                }
            }
        }

        // Trong file: Views/admin/pages/NhanSuView.xaml.cs
        private void DgQuyTacLuong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgQuyTacLuong.SelectedItem is QuyTacLuong selected)
            {
                txtTenQuyTac.Text = selected.TenQuyTac;

                foreach (ComboBoxItem item in cmbLoaiQuyTac.Items)
                {
                    if (item.Tag?.ToString() == selected.LoaiQuyTac)
                    {
                        cmbLoaiQuyTac.SelectedItem = item;
                        cmbDieuKienDonViTinh.Text = selected.DieuKienDonViTinh; // Hiển thị đơn vị điều kiện
                        break;
                    }
                }

                foreach (ComboBoxItem item in cmbDonViTinh.Items)
                {
                    if (item.Tag?.ToString() == selected.DonViTinh)
                    {
                        cmbDonViTinh.SelectedItem = item;
                        break;
                    }
                }

                txtDieuKien.Text = selected.DieuKien?.ToString("G0");

                if (selected.Loai == "Phat")
                {
                    // SỬA LỖI Ở ĐÂY: Bỏ số 2 khỏi "G2"
                    txtGiaTriApDung.Text = (-selected.GiaTriApDung).ToString("G");
                }
                else
                {
                    // SỬA LỖI Ở ĐÂY: Bỏ số 2 khỏi "G2"
                    txtGiaTriApDung.Text = selected.GiaTriApDung.ToString("G");
                }

                // THÊM ĐIỀU KIỆN "OVERTIME" VÀO ĐÂY
                if (selected.LoaiQuyTac == "LATE" || selected.LoaiQuyTac == "MONTHLY_HOURS" || selected.LoaiQuyTac == "OVERTIME")
                {
                    txtDieuKien.IsEnabled = true;
                }
                else
                {
                    txtDieuKien.IsEnabled = false;
                }

                btnThemQT.IsEnabled = false;
                btnLuuQT.IsEnabled = true;
                btnXoaQT.IsEnabled = true;
            }
        }


        private void BtnLamMoiQT_Click(object sender, RoutedEventArgs e) => ClearFormQuyTac();

        private void BtnThemQT_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtGiaTriApDung.Text, out var soTien))
            {
                MessageBox.Show("Số tiền không hợp lệ.");
                return;
            }

            var qt = new QuyTacLuong
            {
                TenQuyTac = txtTenQuyTac.Text,
                LoaiQuyTac = (cmbLoaiQuyTac.SelectedItem as ComboBoxItem)?.Tag.ToString(),
                DieuKien = decimal.TryParse(txtDieuKien.Text, out var dk) ? dk : (decimal?)null,
                DieuKienDonViTinh = (cmbDieuKienDonViTinh.SelectedItem as ComboBoxItem)?.Content.ToString(), // Lấy đơn vị điều kiện
                Loai = soTien >= 0 ? "Thuong" : "Phat",
                GiaTriApDung = Math.Abs(soTien),

                // **THÊM MỚI**: Lấy giá trị từ ComboBox Đơn vị tính
                DonViTinh = (cmbDonViTinh.SelectedItem as ComboBoxItem)?.Tag.ToString()
            };

            if (nhanSuBUS.ThemQuyTac(qt))
            {
                MessageBox.Show("Thêm quy tắc thành công.");
                LoadQuyTacData();
                ClearFormQuyTac();
            }
        }

        private void BtnLuuQT_Click(object sender, RoutedEventArgs e)
        {
            if (dgQuyTacLuong.SelectedItem is QuyTacLuong selected)
            {
                if (!decimal.TryParse(txtGiaTriApDung.Text, out var soTien))
                {
                    MessageBox.Show("Số tiền không hợp lệ.");
                    return;
                }

                selected.TenQuyTac = txtTenQuyTac.Text;
                selected.LoaiQuyTac = (cmbLoaiQuyTac.SelectedItem as ComboBoxItem)?.Tag.ToString();
                selected.DieuKien = decimal.TryParse(txtDieuKien.Text, out var dk) ? dk : (decimal?)null;
                selected.DieuKienDonViTinh = (cmbDieuKienDonViTinh.SelectedItem as ComboBoxItem)?.Content.ToString(); // Lấy đơn vị điều kiện
                selected.Loai = soTien >= 0 ? "Thuong" : "Phat";
                selected.GiaTriApDung = Math.Abs(soTien);

                // **THÊM MỚI**: Lấy giá trị từ ComboBox Đơn vị tính
                selected.DonViTinh = (cmbDonViTinh.SelectedItem as ComboBoxItem)?.Tag.ToString();

                if (nhanSuBUS.SuaQuyTac(selected))
                {
                    MessageBox.Show("Cập nhật quy tắc thành công.");
                    LoadQuyTacData();
                    ClearFormQuyTac();
                }
            }
        }

        private void BtnXoaQT_Click(object sender, RoutedEventArgs e)
        {
            if (dgQuyTacLuong.SelectedItem is QuyTacLuong selected)
            {
                if (MessageBox.Show("Bạn chắc chắn muốn xóa quy tắc này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (nhanSuBUS.XoaQuyTac(selected.IdQuyTac))
                    {
                        MessageBox.Show("Xóa thành công.");
                        LoadQuyTacData();
                        ClearFormQuyTac();
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

        private void LoadThuongPhatThuCong()
        {
            dgThuongPhatThuCong.ItemsSource = null;
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV && dpTuNgay.SelectedDate.HasValue && dpDenNgay.SelectedDate.HasValue)
            {
                dgThuongPhatThuCong.ItemsSource = nhanSuBUS.GetChiTietThuongPhatThuCong(selectedNV.IdNhanVien, dpTuNgay.SelectedDate.Value, dpDenNgay.SelectedDate.Value);
            }
        }

        private void CmbNhanVien_Luong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV)
            {
                dgPhieuLuong.ItemsSource = nhanSuBUS.GetLichSuPhieuLuong(selectedNV.IdNhanVien);
                LoadThuongPhatThuCong();
            }
        }

        private void DpLuong_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                LoadThuongPhatThuCong();
            }
        }

        private void DgThuongPhatThuCong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgThuongPhatThuCong.SelectedItem is ChiTietThuongPhatDTO selected)
            {
                // Chỉ cho phép Sửa/Xóa nếu là khoản thủ công (có IdChiTiet > 0)
                if (selected.IdChiTiet > 0)
                {
                    selectedChiTiet = selected;
                    txtNoiDungPhat.Text = selected.GhiChu;
                    txtSoTienPhat.Text = selected.SoTien?.ToString("G0");
                    btnLuuTPTC.Content = "Lưu Thay Đổi";
                    btnXoaTPTC.IsEnabled = true; // Mở nút Xóa
                }
                else // Nếu là khoản tự động
                {
                    selectedChiTiet = null; // Không cho phép sửa
                    txtNoiDungPhat.Text = selected.GhiChu;
                    txtSoTienPhat.Text = selected.SoTien?.ToString("N0");
                    btnLuuTPTC.Content = "Ghi Nhận Mới";
                    btnXoaTPTC.IsEnabled = false; // Khóa nút Xóa
                }
            }
        }

        private void CmbQuyTac_Mau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbQuyTac_Mau.SelectedItem is QuyTacLuong selectedRule)
            {
                txtNoiDungPhat.Text = selectedRule.TenQuyTac;
                if (selectedRule.Loai == "Phat")
                {
                    txtSoTienPhat.Text = (-selectedRule.GiaTriApDung).ToString("G0");
                }
                else
                {
                    txtSoTienPhat.Text = selectedRule.GiaTriApDung.ToString("G0");
                }
            }
        }

        private void BtnLamMoiTPTC_Click(object sender, RoutedEventArgs e)
        {
            selectedChiTiet = null;
            dgThuongPhatThuCong.SelectedItem = null;
            txtNoiDungPhat.Clear();
            txtSoTienPhat.Clear();
            cmbQuyTac_Mau.SelectedItem = null;
            btnLuuTPTC.Content = "Ghi Nhận Mới";
            btnXoaTPTC.IsEnabled = true; // Mở lại nút xóa ở chế độ thêm mới
        }

        private void BtnLuuTPTC_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV &&
                !string.IsNullOrWhiteSpace(txtNoiDungPhat.Text) &&
                decimal.TryParse(txtSoTienPhat.Text, out decimal soTien))
            {
                bool success;
                if (selectedChiTiet != null) // Sửa
                {
                    success = nhanSuBUS.SuaChiTietThuongPhatThuCong(selectedChiTiet.IdChiTiet, txtNoiDungPhat.Text, soTien);
                }
                else // Thêm mới
                {
                    success = nhanSuBUS.GhiNhanThuongPhatThuCong(selectedNV.IdNhanVien, txtNoiDungPhat.Text, soTien);
                }

                if (success)
                {
                    MessageBox.Show("Thao tác thành công!");
                    BtnLamMoiTPTC_Click(null, null);
                    LoadThuongPhatThuCong();
                    BtnTinhLuong_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Thao tác thất bại.", "Lỗi");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhân viên và nhập đầy đủ thông tin.", "Thiếu thông tin");
            }
        }

        // Cập nhật lại hàm Xóa để chắc chắn hơn
        private void BtnXoaTPTC_Click(object sender, RoutedEventArgs e)
        {
            if (dgThuongPhatThuCong.SelectedItem is ChiTietThuongPhatDTO selected)
            {
                // Kiểm tra lại một lần nữa để đảm bảo chỉ xóa được khoản thủ công
                if (selected.IdChiTiet == 0)
                {
                    MessageBox.Show("Không thể xóa khoản được tính tự động.", "Thao tác không được phép", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Bạn có chắc muốn xóa khoản: '{selected.GhiChu}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (nhanSuBUS.XoaChiTietThuongPhatThuCong(selected.IdChiTiet))
                    {
                        MessageBox.Show("Xóa thành công!");
                        BtnLamMoiTPTC_Click(null, null);
                        BtnTinhLuong_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại.", "Lỗi");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khoản để xóa.", "Chưa chọn mục");
            }
        }

        // **SỬA LẠI** hàm BtnTinhLuong_Click để hiển thị TẤT CẢ chi tiết thưởng/phạt
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

                    // **NÂNG CẤP**: Hiển thị TẤT CẢ các khoản thưởng/phạt trong DataGrid
                    var allItems = phieuLuongTamTinh.CacKhoanThuong
                        .Select(t => new ChiTietThuongPhatDTO { IdChiTiet = t.IdChiTiet ?? 0, GhiChu = t.NoiDung, SoTien = t.SoTien })
                        .ToList();

                    allItems.AddRange(phieuLuongTamTinh.CacKhoanPhat
                        .Select(p => new ChiTietThuongPhatDTO { IdChiTiet = p.IdChiTiet ?? 0, GhiChu = p.NoiDung, SoTien = -p.SoTien }));

                    dgThuongPhatThuCong.ItemsSource = allItems;
                }
            }
        }

        private void BtnChotLuong_Click(object sender, RoutedEventArgs e)
        {
            if (phieuLuongTamTinh != null && cmbNhanVien_Luong.SelectedItem is NhanVien selectedNV)
            {
                if (nhanSuBUS.ChotPhieuLuong(phieuLuongTamTinh))
                {
                    MessageBox.Show("Chốt và tạo phiếu lương thành công!", "Thành công");

                    // SỬA LỖI: Thêm đoạn code để hiển thị cửa sổ xem trước phiếu lương vừa chốt
                    // =======================================================================
                    // Lấy lại thông tin chi tiết của phiếu lương vừa được tạo để in
                    var phieuLuongMoiNhat = nhanSuBUS.GetLichSuPhieuLuong(selectedNV.IdNhanVien).OrderByDescending(p => p.IdPhieuLuong).FirstOrDefault();
                    if (phieuLuongMoiNhat != null)
                    {
                        var previewWindow = new PhieuLuongPreviewWindow(phieuLuongMoiNhat, selectedNV);
                        previewWindow.Owner = Window.GetWindow(this);
                        previewWindow.ShowDialog();
                    }
                    // =======================================================================

                    phieuLuongTamTinh = null;
                    btnChotLuong.IsEnabled = false;
                    dgPhieuLuong.ItemsSource = nhanSuBUS.GetLichSuPhieuLuong(selectedNV.IdNhanVien);
                }
                else
                {
                    MessageBox.Show("Chốt lương thất bại.", "Lỗi");
                }
            }
        }

        private void BtnXemPhieuLuong_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is PhieuLuong selectedPhieuLuong &&
                cmbNhanVien_Luong.SelectedItem is NhanVien selectedNhanVien)
            {
                var phieuLuongChiTiet = nhanSuBUS.TinhLuong(selectedNhanVien.IdNhanVien, selectedPhieuLuong.TuNgay, selectedPhieuLuong.DenNgay);
                phieuLuongChiTiet.IdPhieuLuong = selectedPhieuLuong.IdPhieuLuong;
                phieuLuongChiTiet.NgayTinhLuong = selectedPhieuLuong.NgayTinhLuong;

                var previewWindow = new PhieuLuongPreviewWindow(phieuLuongChiTiet, selectedNhanVien);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
            }
        }

        // Dán hàm này vào file Views/admin/pages/NhanSuView.xaml.cs

        private void CmbCaLamViec_Mau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem người dùng có đang chọn một ca làm việc hợp lệ không
            if (cmbCaLamViec_Mau.SelectedItem is CaLamViec selectedCa)
            {
                // Tự động điền giờ bắt đầu và kết thúc từ ca mẫu đã chọn
                txtGioBatDau.Text = selectedCa.GioBatDau.ToString("hh\\:mm");
                txtGioKetThuc.Text = selectedCa.GioKetThuc.ToString("hh\\:mm");
            }
        }
        #endregion
    }
    }