using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class ThanhToanView : Page
    {
        private ThanhToanBUS thanhToanBUS = new ThanhToanBUS();
        private HoaDon hoaDonGoc;
        private Ban banHienTai;
        private NhanVien currentUser;

        private ObservableCollection<ChiTietHoaDon> chiTietGoc;
        private ObservableCollection<ChiTietHoaDon> chiTietTach;

        public ThanhToanView(HoaDon hoaDon, Ban ban, NhanVien user)
        {
            InitializeComponent();
            this.hoaDonGoc = hoaDon;
            this.banHienTai = ban;
            this.currentUser = user;

            chiTietGoc = new ObservableCollection<ChiTietHoaDon>();
            chiTietTach = new ObservableCollection<ChiTietHoaDon>();
            dgGoc.ItemsSource = chiTietGoc;
            dgTach.ItemsSource = chiTietTach;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblTieuDeThanhToan.Text = $"Thanh toán cho Bàn {banHienTai.SoBan} - Hóa đơn #{hoaDonGoc.IdHoaDon}";

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
            TxtKhachDua_TextChanged(null, null);
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
            // Nếu chưa tách món nào, mặc định in toàn bộ hóa đơn gốc
            var listToPrint = chiTietTach.Any() ? chiTietTach.ToList() : chiTietGoc.ToList();
            if (!listToPrint.Any())
            {
                MessageBox.Show("Không có món nào để in tạm tính.", "Thông báo");
                return;
            }

            var hoaDonTamTinh = new HoaDon
            {
                IdBan = this.hoaDonGoc.IdBan,
                ThoiGianTao = DateTime.Now,
                TongTien = listToPrint.Sum(i => i.ThanhTien),
                SoTienGiam = 0,
                ThanhTien = listToPrint.Sum(i => i.ThanhTien)
            };

            var previewWindow = new HoaDonPreviewWindow(hoaDonTamTinh, listToPrint, this.currentUser, this.banHienTai.SoBan);
            previewWindow.Owner = Window.GetWindow(this);
            previewWindow.ShowDialog();
        }

        private void BtnXacNhanThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (chiTietGoc.Count > 0)
            {
                MessageBox.Show("Vẫn còn món trong hóa đơn gốc chưa được tách. Vui lòng chuyển tất cả các món qua 'Hóa đơn thanh toán' để hoàn tất.", "Thông báo");
                return;
            }
            if (chiTietTach.Count == 0)
            {
                MessageBox.Show("Không có món nào trong hóa đơn thanh toán.", "Thông báo");
                return;
            }

            if (MessageBox.Show("Xác nhận thanh toán cho toàn bộ hóa đơn này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (thanhToanBUS.ThucHienThanhToan(hoaDonGoc.IdHoaDon))
                {
                    MessageBox.Show("Thanh toán thành công!", "Thành công");

                    // HIỂN THỊ CỬA SỔ IN HÓA ĐƠN CUỐI CÙNG
                    var previewWindow = new HoaDonPreviewWindow(this.hoaDonGoc, chiTietTach.ToList(), this.currentUser, this.banHienTai.SoBan, "HÓA ĐƠN THANH TOÁN");
                    previewWindow.Owner = Window.GetWindow(this);
                    previewWindow.ShowDialog();

                    // Quay về Sơ đồ bàn
                    var window = Window.GetWindow(this);
                    if (window is ManHinhNhanVien main)
                    {
                        main.MainFrame.Navigate(new SoDoBanView(this.currentUser));
                    }
                }
                else
                {
                    MessageBox.Show("Thanh toán thất bại. Có lỗi xảy ra.", "Lỗi");
                }
            }
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}