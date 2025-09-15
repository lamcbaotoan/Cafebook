using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class KhuyenMaiView : Page
    {
        private KhuyenMaiBUS khuyenMaiBUS = new KhuyenMaiBUS();

        public KhuyenMaiView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            ClearForm();
        }

        private void LoadData()
        {
            dgKhuyenMai.ItemsSource = khuyenMaiBUS.GetDanhSachKhuyenMai();
        }

        private void ClearForm()
        {
            dgKhuyenMai.SelectedItem = null;
            txtTenKM.Text = "";
            txtMoTa.Text = "";
            cmbLoaiGiamGia.SelectedIndex = -1;
            txtGiaTriGiam.Text = "";
            dpNgayBatDau.SelectedDate = DateTime.Today;
            dpNgayKetThuc.SelectedDate = DateTime.Today.AddDays(7);

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
            if (cmbLoaiGiamGia.SelectedItem == null || dpNgayBatDau.SelectedDate == null || dpNgayKetThuc.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            var km = new KhuyenMai
            {
                TenKhuyenMai = txtTenKM.Text,
                MoTa = txtMoTa.Text,
                LoaiGiamGia = (cmbLoaiGiamGia.SelectedItem as ComboBoxItem)?.Content.ToString(),
                GiaTriGiam = decimal.TryParse(txtGiaTriGiam.Text, out var gia) ? gia : 0,
                NgayBatDau = dpNgayBatDau.SelectedDate.Value,
                NgayKetThuc = dpNgayKetThuc.SelectedDate.Value
            };

            if (khuyenMaiBUS.ThemKhuyenMai(km))
            {
                MessageBox.Show("Thêm chương trình khuyến mãi thành công!");
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm thất bại.");
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (dgKhuyenMai.SelectedItem is KhuyenMai selected)
            {
                selected.TenKhuyenMai = txtTenKM.Text;
                selected.MoTa = txtMoTa.Text;
                selected.LoaiGiamGia = (cmbLoaiGiamGia.SelectedItem as ComboBoxItem)?.Content.ToString();
                selected.GiaTriGiam = decimal.TryParse(txtGiaTriGiam.Text, out var gia) ? gia : 0;
                selected.NgayBatDau = dpNgayBatDau.SelectedDate.Value;
                selected.NgayKetThuc = dpNgayKetThuc.SelectedDate.Value;

                if (khuyenMaiBUS.SuaKhuyenMai(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.");
                }
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgKhuyenMai.SelectedItem is KhuyenMai selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa chương trình '{selected.TenKhuyenMai}'?", "Xác nhận xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (khuyenMaiBUS.XoaKhuyenMai(selected.IdKhuyenMai))
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadData();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại.");
                    }
                }
            }
        }
    }
}