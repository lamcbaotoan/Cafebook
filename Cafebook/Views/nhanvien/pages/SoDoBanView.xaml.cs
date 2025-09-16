using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class SoDoBanView : Page
    {
        private BanBUS banBUS = new BanBUS();
        private ObservableCollection<Ban> danhSachBan;
        private NhanVien currentUser;

        // Các biến trạng thái
        private bool dangChuyenBan = false;
        private bool dangGopBan = false;
        private Ban banDuocChon; // Bàn đang được chọn (bàn nguồn)

        public SoDoBanView(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;
            danhSachBan = new ObservableCollection<Ban>();
            icBan.ItemsSource = danhSachBan;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSoDoBan();
        }

        private void LoadSoDoBan()
        {
            var ds = banBUS.GetDanhSachBan();
            danhSachBan.Clear();
            foreach (var ban in ds)
            {
                danhSachBan.Add(ban);
            }
            ClearSelection();
        }

        private void ClearSelection()
        {
            banDuocChon = null;
            dangChuyenBan = false;
            dangGopBan = false;

            panelChuaChon.Visibility = Visibility.Visible;
            (panelChuaChon.Children[1] as TextBlock).Text = "Chọn một bàn để bắt đầu";
            panelDaChon.Visibility = Visibility.Collapsed;
        }

        private void BtnBan_Click(object sender, RoutedEventArgs e)
        {
            var clickedBan = (sender as Button)?.DataContext as Ban;
            if (clickedBan == null) return;

            if (dangChuyenBan) { HandleChuyenBan(clickedBan); }
            else if (dangGopBan) { HandleGopBan(clickedBan); }
            else { HandleChonBan(clickedBan); }
        }

        private void HandleChonBan(Ban selectedBan)
        {
            banDuocChon = selectedBan;
            panelChuaChon.Visibility = Visibility.Collapsed;
            panelDaChon.Visibility = Visibility.Visible;

            runSoBan.Text = banDuocChon.SoBan;
            runTrangThai.Text = banDuocChon.TrangThai;
            tbGhiChu.Text = banDuocChon.GhiChu;
            tbGhiChu.Visibility = string.IsNullOrEmpty(banDuocChon.GhiChu) ? Visibility.Collapsed : Visibility.Visible;
            tbTongTienWrapper.Visibility = banDuocChon.TrangThai == "Có khách" ? Visibility.Visible : Visibility.Collapsed;
            runTongTien.Text = banDuocChon.TongTienHienTai.ToString("N0") + " VND";

            bool coKhach = banDuocChon.TrangThai == "Có khách";
            btnChuyenBan.IsEnabled = coKhach;
            btnGopBan.IsEnabled = coKhach;
        }

        private void HandleChuyenBan(Ban banDich)
        {
            if (banDich.IdBan == banDuocChon.IdBan)
            {
                ClearSelection();
                MessageBox.Show("Đã hủy thao tác chuyển bàn.");
                return;
            }
            if (banDich.TrangThai != "Trống")
            {
                MessageBox.Show("Vui lòng chọn một bàn còn trống để chuyển đến.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Chuyển khách từ bàn '{banDuocChon.SoBan}' đến bàn '{banDich.SoBan}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (banBUS.ChuyenBan(banDuocChon.IdHoaDonHienTai.Value, banDich.IdBan))
                {
                    MessageBox.Show("Chuyển bàn thành công!");
                    LoadSoDoBan();
                }
            }
            ClearSelection();
        }

        private void HandleGopBan(Ban banDich)
        {
            if (banDich.IdBan == banDuocChon.IdBan)
            {
                ClearSelection();
                MessageBox.Show("Đã hủy thao tác gộp bàn.");
                return;
            }
            if (banDich.TrangThai != "Có khách")
            {
                MessageBox.Show("Vui lòng chọn một bàn cũng đang có khách để gộp vào.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Gộp tất cả món từ bàn '{banDuocChon.SoBan}' vào bàn '{banDich.SoBan}'?\n(Bàn '{banDuocChon.SoBan}' sẽ trở thành bàn trống)", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (banBUS.GopBan(banDuocChon.IdHoaDonHienTai.Value, banDich.IdHoaDonHienTai.Value))
                {
                    MessageBox.Show("Gộp bàn thành công!");
                    LoadSoDoBan();
                }
            }
            ClearSelection();
        }
        // Trong file SoDoBanView.xaml.cs

        private void BtnGoiMon_Click(object sender, RoutedEventArgs e)
        {
            if (banDuocChon != null)
            {
                var mainWindow = Window.GetWindow(this) as ManHinhNhanVien;
                if (mainWindow != null)
                {
                    // SỬA LẠI DÒNG NÀY: Truyền thêm 'this.currentUser'
                    mainWindow.MainFrame.Navigate(new GoiMonView(banDuocChon, this.currentUser));
                }
            }
        }

        private void BtnChuyenBan_Click(object sender, RoutedEventArgs e)
        {
            if (banDuocChon != null && banDuocChon.TrangThai == "Có khách")
            {
                dangChuyenBan = true;
                panelDaChon.Visibility = Visibility.Collapsed;
                panelChuaChon.Visibility = Visibility.Visible;
                (panelChuaChon.Children[1] as TextBlock).Text = $"Đang chuyển từ bàn '{banDuocChon.SoBan}'.\nVui lòng chọn bàn TRỐNG muốn chuyển đến.\n(Nhấn lại bàn '{banDuocChon.SoBan}' để hủy)";
            }
        }

        private void BtnGopBan_Click(object sender, RoutedEventArgs e)
        {
            if (banDuocChon != null && banDuocChon.TrangThai == "Có khách")
            {
                dangGopBan = true;
                panelDaChon.Visibility = Visibility.Collapsed;
                panelChuaChon.Visibility = Visibility.Visible;
                (panelChuaChon.Children[1] as TextBlock).Text = $"Đang gộp từ bàn '{banDuocChon.SoBan}'.\nVui lòng chọn bàn CÓ KHÁCH khác để gộp vào.\n(Nhấn lại bàn '{banDuocChon.SoBan}' để hủy)";
            }
        }

        private void BtnBaoCaoSuCo_Click(object sender, RoutedEventArgs e)
        {
            if (banDuocChon != null)
            {
                var reportWindow = new BaoCaoSuCoWindow(banDuocChon, this.currentUser);
                reportWindow.Owner = Window.GetWindow(this);
                reportWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bàn trước khi báo cáo sự cố.", "Thông báo");
            }
        }

        private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
        {
            LoadSoDoBan();
        }
    }
}