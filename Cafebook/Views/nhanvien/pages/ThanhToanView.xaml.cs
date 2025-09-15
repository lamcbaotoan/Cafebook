using Cafebook.BUS;
using Cafebook.DTO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // Thêm using này để dùng Brushes

namespace Cafebook.Views.nhanvien.pages
{
    public partial class ThanhToanView : Page
    {
        private ThanhToanBUS thanhToanBUS = new ThanhToanBUS();
        private HoaDon hoaDonGoc;

        private ObservableCollection<ChiTietHoaDon> chiTietGoc;
        private ObservableCollection<ChiTietHoaDon> chiTietTach; // Hóa đơn để thanh toán

        public ThanhToanView(HoaDon hoaDon)
        {
            InitializeComponent();
            this.hoaDonGoc = hoaDon;
            chiTietGoc = new ObservableCollection<ChiTietHoaDon>();
            chiTietTach = new ObservableCollection<ChiTietHoaDon>();
            dgGoc.ItemsSource = chiTietGoc;
            dgTach.ItemsSource = chiTietTach;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Giả sử IdBan đã có trong đối tượng hoaDonGoc được truyền vào
            // Nếu chưa có, bạn cần JOIN để lấy từ CSDL
            lblTieuDeThanhToan.Text = $"Thanh toán cho Hóa đơn #{hoaDonGoc.IdHoaDon} - Tổng cộng: {hoaDonGoc.ThanhTien:N0} VND";

            var dsChiTiet = thanhToanBUS.GetChiTietHoaDon(hoaDonGoc.IdHoaDon);
            foreach (var item in dsChiTiet)
            {
                chiTietGoc.Add(item);
            }
            CapNhatTienCanThanhToan();
        }

        private void CapNhatTienCanThanhToan()
        {
            decimal tongTienTach = chiTietTach.Sum(item => item.ThanhTien);
            lblTienCanThanhToan.Text = tongTienTach.ToString("N0") + " VND";
            TxtKhachDua_TextChanged(null, null); // Cập nhật lại tiền thừa
        }

        private void BtnChuyenQua_Click(object sender, RoutedEventArgs e)
        {
            if (dgGoc.SelectedItem is ChiTietHoaDon selected)
            {
                chiTietGoc.Remove(selected);
                chiTietTach.Add(selected);
                CapNhatTienCanThanhToan();
            }
        }

        private void BtnChuyenLai_Click(object sender, RoutedEventArgs e)
        {
            if (dgTach.SelectedItem is ChiTietHoaDon selected)
            {
                chiTietTach.Remove(selected);
                chiTietGoc.Add(selected);
                CapNhatTienCanThanhToan();
            }
        }

        private void TxtKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal.TryParse(txtKhachDua.Text, out decimal khachDua);
            decimal tienCanTra = chiTietTach.Sum(item => item.ThanhTien);
            decimal tienThua = khachDua - tienCanTra;

            if (tienThua < 0)
            {
                lblTienThua.Text = "Còn thiếu...";
                lblTienThua.Foreground = Brushes.Red;
            }
            else
            {
                lblTienThua.Text = tienThua.ToString("N0") + " VND";
                lblTienThua.Foreground = Brushes.Blue;
            }
        }

        private void BtnInTamTinh_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Đang mô phỏng in hóa đơn tạm tính...", "In Hóa đơn");
        }

        private void BtnXacNhanThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (chiTietGoc.Count > 0)
            {
                MessageBox.Show("Vẫn còn món trong hóa đơn gốc. Vui lòng chuyển tất cả các món qua 'Hóa đơn thanh toán' hoặc thanh toán toàn bộ hóa đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Xác nhận thanh toán cho toàn bộ hóa đơn này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (thanhToanBUS.ThucHienThanhToan(hoaDonGoc.IdHoaDon))
                {
                    MessageBox.Show("Thanh toán thành công!");
                    MessageBox.Show("Đang mô phỏng in hóa đơn thanh toán...", "In Hóa đơn");

                    // SỬA LỖI: Quay về màn hình sơ đồ bàn
                    // Cách đúng để điều hướng từ một Page là dùng NavigationService của chính nó.
                    if (this.NavigationService.CanGoBack)
                    {
                        // Xóa lịch sử trang thanh toán và gọi món để không quay lại được
                        this.NavigationService.RemoveBackEntry(); // Xóa trang thanh toán
                        this.NavigationService.RemoveBackEntry(); // Xóa trang gọi món

                        // Điều hướng về trang mới của sơ đồ bàn để cập nhật trạng thái
                        this.NavigationService.Navigate(new SoDoBanView());
                    }
                }
                else
                {
                    MessageBox.Show("Thanh toán thất bại. Có lỗi xảy ra trong quá trình xử lý.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            // SỬA LỖI: Dùng NavigationService của Page
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}