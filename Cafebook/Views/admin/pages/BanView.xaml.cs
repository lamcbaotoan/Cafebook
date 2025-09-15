using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.admin.pages
{
    public partial class BanView : Page
    {
        private QuanLyBanBUS banBUS = new QuanLyBanBUS();

        public BanView()
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
            dgBan.ItemsSource = banBUS.GetDanhSachBan();
        }

        private void ClearForm()
        {
            dgBan.SelectedItem = null;
            txtSoBan.Text = "";
            txtSoGhe.Text = "";
            txtGhiChu.Text = ""; // Thêm
            cmbTrangThai.SelectedIndex = 0;
            btnThemBan.IsEnabled = true;
            btnLuuBan.IsEnabled = false;
            btnXoaBan.IsEnabled = false;
            cmbTrangThai.IsEnabled = false;
        }

        private void DgBan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBan.SelectedItem is Ban selected)
            {
                txtSoBan.Text = selected.SoBan;
                txtSoGhe.Text = selected.SoGhe.ToString();
                txtGhiChu.Text = selected.GhiChu; // Thêm
                cmbTrangThai.Text = selected.TrangThai;

                bool isEditable = (selected.TrangThai == "Trống" || selected.TrangThai == "Bảo trì");
                cmbTrangThai.IsEnabled = isEditable;
                btnXoaBan.IsEnabled = isEditable;

                btnThemBan.IsEnabled = false;
                btnLuuBan.IsEnabled = true;
            }
        }

        private void BtnLamMoiBan_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void BtnThemBan_Click(object sender, RoutedEventArgs e)
        {
            var ban = new Ban
            {
                SoBan = txtSoBan.Text,
                SoGhe = int.TryParse(txtSoGhe.Text, out var soGhe) ? soGhe : 0,
                GhiChu = txtGhiChu.Text // Thêm
            };
            if (banBUS.ThemBan(ban))
            {
                MessageBox.Show("Thêm bàn thành công!");
                LoadData();
                ClearForm();
            }
        }

        private void BtnLuuBan_Click(object sender, RoutedEventArgs e)
        {
            if (dgBan.SelectedItem is Ban selected)
            {
                selected.SoBan = txtSoBan.Text;
                selected.SoGhe = int.TryParse(txtSoGhe.Text, out var soGhe) ? soGhe : 0;
                selected.GhiChu = txtGhiChu.Text; // Thêm
                selected.TrangThai = (cmbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (banBUS.SuaBan(selected))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void BtnXoaBan_Click(object sender, RoutedEventArgs e)
        {
            if (dgBan.SelectedItem is Ban selected)
            {
                if (MessageBox.Show($"Bạn có chắc muốn xóa '{selected.SoBan}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (banBUS.XoaBan(selected.IdBan))
                    {
                        MessageBox.Show("Xóa bàn thành công!");
                        LoadData();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa bàn này vì đang có khách hoặc có phiếu đặt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}