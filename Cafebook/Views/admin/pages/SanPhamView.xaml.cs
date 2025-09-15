using Cafebook.BUS;
using Cafebook.DTO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class SanPhamView : Page
    {
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();

        public SanPhamView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            ClearFormSP();
            ClearFormLSP();
        }

        private void LoadAllData()
        {
            // Tải dữ liệu cho Tab 1
            dgSanPham.ItemsSource = sanPhamBUS.GetDanhSachSanPham();
            cmbLoaiSP.ItemsSource = sanPhamBUS.GetDanhSachLoaiSP();
            cmbNguyenLieu.ItemsSource = sanPhamBUS.GetDanhSachNguyenLieu();

            // Tải dữ liệu cho Tab 2
            dgLoaiSanPham.ItemsSource = sanPhamBUS.GetDanhSachLoaiSP();
        }

        #region Quản lý Sản phẩm (Tab 1)
        private void ClearFormSP()
        {
            dgSanPham.SelectedItem = null;
            txtTenSP.Text = "";
            cmbLoaiSP.SelectedIndex = -1;
            txtDonGia.Text = "";
            cmbTrangThai.SelectedIndex = -1;
            txtMoTa.Text = "";

            dgCongThuc.ItemsSource = null;
            cmbNguyenLieu.SelectedIndex = -1;
            txtSoLuongNL.Text = "";
            cmbDonViTinhNVL.ItemsSource = null; // Thêm dòng này để xóa các lựa chọn cũ

            btnThemSP.IsEnabled = true;
            btnLuuSP.IsEnabled = false;
            btnXoaSP.IsEnabled = false;
        }

        private void DgSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham selectedSP)
            {
                txtTenSP.Text = selectedSP.TenSanPham;
                cmbLoaiSP.SelectedValue = selectedSP.IdLoaiSP;
                txtDonGia.Text = selectedSP.DonGia.ToString();
                cmbTrangThai.Text = selectedSP.TrangThai;
                txtMoTa.Text = selectedSP.MoTa;
                cmbDonViTinhNVL.ItemsSource = null; // Thêm dòng này

                LoadCongThuc(selectedSP.IdSanPham);

                btnThemSP.IsEnabled = false;
                btnLuuSP.IsEnabled = true;
                btnXoaSP.IsEnabled = true;
            }
        }

        // THÊM HÀM MỚI NÀY
        private void CmbNguyenLieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNguyenLieu.SelectedItem is NguyenLieu selectedNL)
            {
                // Logic tự động gợi ý đơn vị tính
                var donViGoiY = new List<string>();
                string baseUnit = selectedNL.DonViTinh.ToLower();

                if (baseUnit == "kg")
                {
                    donViGoiY.AddRange(new[] { "g", "kg" });
                }
                else if (baseUnit == "lít" || baseUnit == "l")
                {
                    donViGoiY.AddRange(new[] { "ml", "lít" });
                }
                else
                {
                    donViGoiY.Add(selectedNL.DonViTinh); // Nếu là các đơn vị khác như 'quả', 'cái'
                }
                cmbDonViTinhNVL.ItemsSource = donViGoiY;
                cmbDonViTinhNVL.SelectedIndex = 0;
            }
        }

        private void LoadCongThuc(int idSanPham)
        {
            dgCongThuc.ItemsSource = sanPhamBUS.GetCongThuc(idSanPham);
        }

        private void BtnLamMoiSP_Click(object sender, RoutedEventArgs e)
        {
            ClearFormSP();
        }

        private void BtnThemSP_Click(object sender, RoutedEventArgs e)
        {
            if (cmbLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm.");
                return;
            }

            SanPham newSP = new SanPham
            {
                TenSanPham = txtTenSP.Text,
                IdLoaiSP = (int)cmbLoaiSP.SelectedValue,
                DonGia = decimal.TryParse(txtDonGia.Text, out var gia) ? gia : 0,
                TrangThai = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Đang bán",
                MoTa = txtMoTa.Text
            };

            if (sanPhamBUS.ThemSanPham(newSP))
            {
                MessageBox.Show("Thêm sản phẩm thành công!");
                LoadAllData();
                ClearFormSP();
            }
            else
            {
                MessageBox.Show("Thêm sản phẩm thất bại.");
            }
        }

        private void BtnLuuSP_Click(object sender, RoutedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham selectedSP)
            {
                selectedSP.TenSanPham = txtTenSP.Text;
                selectedSP.IdLoaiSP = (int)cmbLoaiSP.SelectedValue;
                selectedSP.DonGia = decimal.TryParse(txtDonGia.Text, out var gia) ? gia : 0;
                selectedSP.TrangThai = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Đang bán";
                selectedSP.MoTa = txtMoTa.Text;

                if (sanPhamBUS.SuaSanPham(selectedSP))
                {
                    MessageBox.Show("Cập nhật sản phẩm thành công!");
                    LoadAllData();
                    ClearFormSP();
                }
                else
                {
                    MessageBox.Show("Cập nhật sản phẩm thất bại.");
                }
            }
        }

        private void BtnXoaSP_Click(object sender, RoutedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham selectedSP)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm '{selectedSP.TenSanPham}'?", "Xác nhận xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (sanPhamBUS.XoaSanPham(selectedSP.IdSanPham))
                    {
                        MessageBox.Show("Xóa sản phẩm thành công!");
                        LoadAllData();
                        ClearFormSP();
                    }
                    else
                    {
                        MessageBox.Show("Xóa sản phẩm thất bại.");
                    }
                }
            }
        }

        // SỬA LẠI HÀM NÀY
        private void BtnLuuNVL_Click(object sender, RoutedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham selectedSP &&
                cmbNguyenLieu.SelectedValue != null &&
                !string.IsNullOrWhiteSpace(txtSoLuongNL.Text) &&
                cmbDonViTinhNVL.SelectedItem != null) // Kiểm tra đã chọn đơn vị tính chưa
            {
                CongThuc ct = new CongThuc
                {
                    IdSanPham = selectedSP.IdSanPham,
                    IdNguyenLieu = (int)cmbNguyenLieu.SelectedValue,
                    LuongCanThiet = decimal.TryParse(txtSoLuongNL.Text, out var sl) ? sl : 0,
                    DonViTinhSuDung = cmbDonViTinhNVL.SelectedItem.ToString() // Lấy đơn vị tính đã chọn
                };

                if (sanPhamBUS.LuuNguyenLieuVaoCongThuc(ct))
                {
                    LoadCongThuc(selectedSP.IdSanPham);
                    txtSoLuongNL.Text = "";
                    cmbNguyenLieu.SelectedIndex = -1;
                    cmbDonViTinhNVL.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show("Lưu nguyên liệu vào công thức thất bại.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sản phẩm, chọn nguyên liệu, nhập số lượng và chọn đơn vị tính.");
            }
        }

        private void BtnXoaNVL_Click(object sender, RoutedEventArgs e)
        {
            if (dgSanPham.SelectedItem is SanPham selectedSP && dgCongThuc.SelectedItem is CongThuc selectedCT)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa '{selectedCT.TenNguyenLieu}' khỏi công thức?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (sanPhamBUS.XoaNguyenLieuKhoiCongThuc(selectedSP.IdSanPham, selectedCT.IdNguyenLieu))
                    {
                        LoadCongThuc(selectedSP.IdSanPham);
                    }
                    else
                    {
                        MessageBox.Show("Xóa nguyên liệu khỏi công thức thất bại.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nguyên liệu trong bảng công thức để xóa.");
            }
        }
        #endregion

        #region Quản lý Loại Sản phẩm (Tab 2)
        private void ClearFormLSP()
        {
            dgLoaiSanPham.SelectedItem = null;
            txtTenLoaiSP.Text = "";
            btnThemLSP.IsEnabled = true;
            btnLuuLSP.IsEnabled = false;
            btnXoaLSP.IsEnabled = false;
        }

        private void DgLoaiSanPham_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgLoaiSanPham.SelectedItem is LoaiSanPham selected)
            {
                txtTenLoaiSP.Text = selected.TenLoaiSP;
                btnThemLSP.IsEnabled = false;
                btnLuuLSP.IsEnabled = true;
                btnXoaLSP.IsEnabled = true;
            }
        }

        private void BtnLamMoiLSP_Click(object sender, RoutedEventArgs e)
        {
            ClearFormLSP();
        }

        private void BtnThemLSP_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoaiSP.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại sản phẩm.");
                return;
            }
            var lsp = new LoaiSanPham { TenLoaiSP = txtTenLoaiSP.Text };
            if (sanPhamBUS.ThemLoaiSanPham(lsp))
            {
                MessageBox.Show("Thêm thành công!");
                LoadAllData();
                ClearFormLSP();
            }
            else
            {
                MessageBox.Show("Thêm thất bại.");
            }
        }

        private void BtnLuuLSP_Click(object sender, RoutedEventArgs e)
        {
            if (dgLoaiSanPham.SelectedItem is LoaiSanPham selected)
            {
                selected.TenLoaiSP = txtTenLoaiSP.Text;
                if (sanPhamBUS.SuaLoaiSanPham(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadAllData();
                    ClearFormLSP();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.");
                }
            }
        }

        private void BtnXoaLSP_Click(object sender, RoutedEventArgs e)
        {
            if (dgLoaiSanPham.SelectedItem is LoaiSanPham selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa loại '{selected.TenLoaiSP}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (sanPhamBUS.XoaLoaiSanPham(selected.IdLoaiSP))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadAllData();
                        ClearFormLSP();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại. Không thể xóa loại sản phẩm đang được sử dụng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion
    }
}