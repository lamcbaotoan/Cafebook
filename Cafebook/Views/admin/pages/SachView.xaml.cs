using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class SachView : Page
    {
        private SachBUS sachBUS = new SachBUS();

        public SachView()
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
            dgSach.ItemsSource = sachBUS.GetDanhSachSach();
            dgPhieuThueSach.ItemsSource = sachBUS.GetLichSuThueSach();
        }

        #region Quản lý Sách
        private void ClearForm()
        {
            dgSach.SelectedItem = null;
            txtTieuDe.Text = "";
            txtTacGia.Text = "";
            txtTheLoai.Text = "";
            txtTongSoLuong.Text = "";
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
                txtTongSoLuong.Text = selected.TongSoLuong.ToString();
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
            var sach = new Sach
            {
                TieuDe = txtTieuDe.Text,
                TacGia = txtTacGia.Text,
                TheLoai = txtTheLoai.Text,
                MoTa = txtMoTa.Text,
                TongSoLuong = int.TryParse(txtTongSoLuong.Text, out var sl) ? sl : 0
            };
            if (sachBUS.ThemSach(sach))
            {
                MessageBox.Show("Thêm sách thành công!");
                LoadData();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm sách thất bại.");
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (dgSach.SelectedItem is Sach selected)
            {
                selected.TieuDe = txtTieuDe.Text;
                selected.TacGia = txtTacGia.Text;
                selected.TheLoai = txtTheLoai.Text;
                selected.MoTa = txtMoTa.Text;
                selected.TongSoLuong = int.TryParse(txtTongSoLuong.Text, out var sl) ? sl : 0;

                if (sachBUS.SuaSach(selected))
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
                        MessageBox.Show("Xóa sách thất bại. Sách có thể đang được cho thuê.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region Chính sách
        private void BtnLuuChinhSach_Click(object sender, RoutedEventArgs e)
        {
            // Trong một ứng dụng thực tế, giá trị từ txtPhiThue và txtPhiPhat
            // sẽ được lưu vào một file cấu hình (ví dụ: App.config, settings.json)
            // hoặc một bảng 'Settings' trong CSDL.
            MessageBox.Show("Đã lưu chính sách cho thuê.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}