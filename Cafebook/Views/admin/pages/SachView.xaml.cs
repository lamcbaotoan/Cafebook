using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cafebook.Views.admin.pages
{
    public partial class SachView : Page
    {
        private SachBUS sachBUS = new SachBUS();
        private List<Sach> _fullSachList;
        private List<string> _uniqueTheLoai;

        public SachView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            ClearForm();
            LoadCaiDat();
        }

        private void LoadData()
        {
            _fullSachList = sachBUS.GetDanhSachSach();
            dgSach.ItemsSource = _fullSachList;
            PopulateTheLoaiFilter();

            dgPhieuThueSach.ItemsSource = sachBUS.GetLichSuThueSach();
            dgSachQuaHan.ItemsSource = sachBUS.GetDanhSachSachQuaHan();
        }

        private void PopulateTheLoaiFilter()
        {
            _uniqueTheLoai = _fullSachList.Select(s => s.TheLoai).Distinct().OrderBy(t => t).ToList();
            cbFilterTheLoai.ItemsSource = _uniqueTheLoai;
            cbFilterTheLoai.SelectedIndex = -1;
            cbFilterTheLoai.Tag = "Initialized";
        }

        private void LoadCaiDat()
        {
            txtPhiThue.Text = sachBUS.GetCaiDat("PhiThueSach") ?? "10000";
            txtPhiPhat.Text = sachBUS.GetCaiDat("PhiPhatTreHan") ?? "5000";
        }

        #region Quản lý Sách

        private void ClearForm()
        {
            dgSach.SelectedItem = null;
            txtTieuDe.Text = "";
            txtTacGia.Text = "";
            txtTheLoai.Text = "";
            txtViTri.Text = "";
            txtTongSoLuong.Text = "";
            txtGiaBia.Text = ""; // SỬA: Thêm dòng này
            txtMoTa.Text = "";
            btnThem.IsEnabled = true;
            btnLuu.IsEnabled = false;
            btnXoa.IsEnabled = false;
        }

        private void DgSach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSach.SelectedItem is Sach selected)
            {
                txtTieuDe.Text = selected.TieuDe;
                txtTacGia.Text = selected.TacGia;
                txtTheLoai.Text = selected.TheLoai;
                txtViTri.Text = selected.ViTri;
                txtTongSoLuong.Text = selected.TongSoLuong.ToString();
                txtGiaBia.Text = selected.GiaBia.ToString("G"); // SỬA: Thêm dòng này ("G" để bỏ .00)
                txtMoTa.Text = selected.MoTa;
                btnThem.IsEnabled = false;
                btnLuu.IsEnabled = true;
                btnXoa.IsEnabled = true;
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSachInput()) return;

            var sach = new Sach
            {
                TieuDe = txtTieuDe.Text,
                TacGia = txtTacGia.Text,
                TheLoai = txtTheLoai.Text,
                MoTa = txtMoTa.Text,
                ViTri = txtViTri.Text,
                TongSoLuong = int.Parse(txtTongSoLuong.Text),
                GiaBia = decimal.Parse(txtGiaBia.Text) // SỬA: Thêm dòng này
            };
            if (sachBUS.ThemSach(sach))
            {
                MessageBox.Show("Thêm sách thành công!");
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm sách thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is Sach selected)
            {
                if (!ValidateSachInput()) return;

                selected.TieuDe = txtTieuDe.Text;
                selected.TacGia = txtTacGia.Text;
                selected.TheLoai = txtTheLoai.Text;
                selected.MoTa = txtMoTa.Text;
                selected.ViTri = txtViTri.Text;
                selected.TongSoLuong = int.Parse(txtTongSoLuong.Text);
                selected.GiaBia = decimal.Parse(txtGiaBia.Text); // SỬA: Thêm dòng này

                if (sachBUS.SuaSach(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is Sach selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa sách '{selected.TieuDe}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (sachBUS.XoaSach(selected.IdSach))
                    {
                        MessageBox.Show("Xóa sách thành công!");
                        LoadData();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Xóa sách thất bại. Sách có thể đang được cho thuê hoặc số lượng có sẵn không bằng tổng số lượng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private bool ValidateSachInput()
        {
            if (string.IsNullOrWhiteSpace(txtTieuDe.Text))
            {
                MessageBox.Show("Tiêu đề sách không được để trống.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTieuDe.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTongSoLuong.Text) || !int.TryParse(txtTongSoLuong.Text, out int sl) || sl <= 0)
            {
                MessageBox.Show("Tổng số lượng phải là một số nguyên dương.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTongSoLuong.Focus();
                return false;
            }
            // SỬA: Thêm validation cho Giá bìa
            if (string.IsNullOrWhiteSpace(txtGiaBia.Text) || !decimal.TryParse(txtGiaBia.Text, out decimal giaBia) || giaBia < 0)
            {
                MessageBox.Show("Giá bìa (tiền cọc) phải là một số không âm hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGiaBia.Focus();
                return false;
            }
            return true;
        }

        #endregion

        #region Tìm kiếm và Lọc Sách
        private void ApplySachFilter()
        {
            IEnumerable<Sach> filteredList = _fullSachList;
            if (!string.IsNullOrWhiteSpace(txtTimKiemSach.Text))
            {
                string searchTerm = txtTimKiemSach.Text.ToLower();
                filteredList = filteredList.Where(s =>
                    s.TieuDe.ToLower().Contains(searchTerm) ||
                    (s.TacGia != null && s.TacGia.ToLower().Contains(searchTerm)) ||
                    (s.TheLoai != null && s.TheLoai.ToLower().Contains(searchTerm)) ||
                    (s.ViTri != null && s.ViTri.ToLower().Contains(searchTerm)));
            }
            if (cbFilterTheLoai.SelectedItem is string selectedTheLoai && !string.IsNullOrWhiteSpace(selectedTheLoai))
            {
                filteredList = filteredList.Where(s => s.TheLoai == selectedTheLoai);
            }
            dgSach.ItemsSource = filteredList.ToList();
        }

        private void TxtTimKiemSach_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySachFilter();
        }

        private void CbFilterTheLoai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFilterTheLoai.Tag != null && cbFilterTheLoai.Tag.ToString() == "Initialized")
            {
                ApplySachFilter();
            }
        }

        private void BtnLamMoiLoc_Click(object sender, RoutedEventArgs e)
        {
            txtTimKiemSach.Text = "";
            cbFilterTheLoai.SelectedIndex = -1;
            ApplySachFilter();
        }
        #endregion

        #region Chính sách
        private void BtnLuuChinhSach_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhiThue.Text) || !decimal.TryParse(txtPhiThue.Text, out _))
            {
                MessageBox.Show("Phí thuê không hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }
            if (string.IsNullOrWhiteSpace(txtPhiPhat.Text) || !decimal.TryParse(txtPhiPhat.Text, out _))
            {
                MessageBox.Show("Phí phạt không hợp lệ.", "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return;
            }

            bool successPhiThue = sachBUS.SetCaiDat("PhiThueSach", txtPhiThue.Text);
            bool successPhiPhat = sachBUS.SetCaiDat("PhiPhatTreHan", txtPhiPhat.Text);

            if (successPhiThue && successPhiPhat)
            {
                MessageBox.Show("Đã lưu chính sách cho thuê thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Lưu chính sách thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Cho phép số và dấu chấm thập phân
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}