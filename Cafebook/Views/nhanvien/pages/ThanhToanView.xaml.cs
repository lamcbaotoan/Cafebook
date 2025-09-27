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
        private GoiMonBUS goiMonBUS = new GoiMonBUS();
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

            chiTietGoc = new ObservableCollection<ChiTietHoaDon>(thanhToanBUS.GetChiTietHoaDon(hoaDon.IdHoaDon));
            chiTietTach = new ObservableCollection<ChiTietHoaDon>();

            dgGoc.ItemsSource = chiTietGoc;
            dgTach.ItemsSource = chiTietTach;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblTieuDeThanhToan.Text = $"Thanh toán cho Bàn {banHienTai.SoBan} - Hóa đơn #{hoaDonGoc.IdHoaDon}";
            BtnChuyenQuaTatCa_Click(null, null);
        }

        #region TinhToan

        // SỬA LẠI PHƯƠNG THỨC NÀY
        private void CapNhatTienVaKhuyenMai()
        {
            decimal tongTienTach = chiTietTach.Sum(item => item.ThanhTien);
            var idSanPhamTrongHoaDonTach = chiTietTach.Select(item => item.IdSanPham).ToList();

            var dsKMPhuHop = goiMonBUS.GetKhuyenMaiCoTheApDung(tongTienTach, idSanPhamTrongHoaDonTach);
            dsKMPhuHop.Insert(0, new KhuyenMai { IdKhuyenMai = 0, TenKhuyenMai = "Không áp dụng" });

            // Ưu tiên lấy ID khuyến mãi đang được chọn trên ComboBox.
            // Nếu chưa có gì được chọn (lần đầu tải trang), thì lấy ID từ hoaDonGoc truyền qua.
            int? idCanChon = (cmbKhuyenMai.SelectedItem as KhuyenMai)?.IdKhuyenMai;
            if (idCanChon == null)
            {
                idCanChon = this.hoaDonGoc.IdKhuyenMai;
            }

            cmbKhuyenMai.ItemsSource = dsKMPhuHop;
            cmbKhuyenMai.SelectedValuePath = "IdKhuyenMai";

            // Cố gắng chọn lại khuyến mãi dựa trên ID đã xác định
            if (idCanChon.HasValue && dsKMPhuHop.Any(km => km.IdKhuyenMai == idCanChon.Value))
            {
                cmbKhuyenMai.SelectedValue = idCanChon.Value;
            }
            else
            {
                cmbKhuyenMai.SelectedIndex = 0;
            }

            TinhToanTienCuoiCung();
        }

        private void TinhToanTienCuoiCung()
        {
            decimal tongTien = chiTietTach.Sum(item => item.ThanhTien);
            decimal soTienGiam = 0;

            if (cmbKhuyenMai.SelectedItem is KhuyenMai km && km.IdKhuyenMai != 0)
            {
                if (km.LoaiGiamGia == "PhanTram") soTienGiam = tongTien * (km.GiaTriGiam / 100);
                else soTienGiam = km.GiaTriGiam;
            }

            decimal thanhTien = tongTien - soTienGiam;

            lblTienCanThanhToan.Text = tongTien.ToString("N0") + " đ";
            lblTienGiam.Text = "- " + soTienGiam.ToString("N0") + " đ";
            lblThanhTien.Text = thanhTien.ToString("N0") + " đ";

            TxtKhachDua_TextChanged(null, null);
        }

        private void TxtKhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal.TryParse(txtKhachDua.Text.Replace(",", ""), out decimal khachDua);
            decimal.TryParse(lblThanhTien.Text.Replace(" đ", "").Replace(",", ""), out decimal tienCanTra);
            decimal tienThua = khachDua - tienCanTra;

            if (khachDua > 0 && tienCanTra > 0 && tienThua < 0)
            {
                lblTienThua.Text = "Còn thiếu " + (tienThua * -1).ToString("N0") + " đ";
                lblTienThua.Foreground = Brushes.Red;
            }
            else
            {
                lblTienThua.Text = tienThua.ToString("N0") + " đ";
                lblTienThua.Foreground = (SolidColorBrush)FindResource("ActionBlueBrush");
            }
        }
        #endregion

        #region ChuyenMon
        private void BtnChuyenQua_Click(object sender, RoutedEventArgs e)
        {
            if (dgGoc.SelectedItem is ChiTietHoaDon selected)
            {
                chiTietGoc.Remove(selected);
                chiTietTach.Add(selected);
                CapNhatTienVaKhuyenMai();
            }
        }

        private void BtnChuyenLai_Click(object sender, RoutedEventArgs e)
        {
            if (dgTach.SelectedItem is ChiTietHoaDon selected)
            {
                chiTietTach.Remove(selected);
                chiTietGoc.Add(selected);
                CapNhatTienVaKhuyenMai();
            }
        }

        private void BtnChuyenQuaTatCa_Click(object sender, RoutedEventArgs e)
        {
            if (!chiTietGoc.Any()) return;
            foreach (var item in chiTietGoc.ToList())
            {
                chiTietTach.Add(item);
            }
            chiTietGoc.Clear();
            CapNhatTienVaKhuyenMai();
        }

        private void BtnChuyenLaiTatCa_Click(object sender, RoutedEventArgs e)
        {
            if (!chiTietTach.Any()) return;
            foreach (var item in chiTietTach.ToList())
            {
                chiTietGoc.Add(item);
            }
            chiTietTach.Clear();
            CapNhatTienVaKhuyenMai();
        }
        #endregion

        #region HanhDongChinh
        private void BtnInTamTinh_Click(object sender, RoutedEventArgs e)
        {
            if (!chiTietTach.Any())
            {
                MessageBox.Show("Vui lòng chuyển món vào 'Hóa đơn thanh toán' để in tạm tính.", "Thông báo");
                return;
            }

            var hoaDonTamTinh = new HoaDon();
            CapNhatThongTinHoaDon(hoaDonTamTinh, false);

            var previewWindow = new HoaDonPreviewWindow(hoaDonTamTinh, chiTietTach.ToList(), this.currentUser, this.banHienTai.SoBan);
            previewWindow.Owner = Window.GetWindow(this);
            previewWindow.ShowDialog();
        }

        private void BtnXacNhanThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (!chiTietTach.Any())
            {
                MessageBox.Show("Vui lòng chuyển món vào 'Hóa đơn thanh toán'.", "Thông báo");
                return;
            }

            if (!chiTietGoc.Any())
            {
                ThanhToanToanBo();
            }
            else
            {
                ThanhToanTachMon();
            }
        }

        private void ThanhToanToanBo()
        {
            if (MessageBox.Show("Xác nhận thanh toán cho toàn bộ hóa đơn này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            CapNhatThongTinHoaDon(this.hoaDonGoc, true);

            if (thanhToanBUS.ThucHienThanhToan(hoaDonGoc))
            {
                MessageBox.Show("Thanh toán thành công!", "Thành công");
                InHoaDonCuoiCung(hoaDonGoc);
                NavigateToSoDoBan();
            }
            else
            {
                MessageBox.Show("Thanh toán thất bại.", "Lỗi");
            }
        }

        private void ThanhToanTachMon()
        {
            if (MessageBox.Show("Xác nhận thanh toán cho các món đã tách và tạo hóa đơn mới cho các món còn lại?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            var hoaDonThanhToan = new HoaDon { IdBan = this.hoaDonGoc.IdBan, ThoiGianTao = this.hoaDonGoc.ThoiGianTao };
            CapNhatThongTinHoaDon(hoaDonThanhToan, true);

            if (thanhToanBUS.ThucHienTachHoaDon(hoaDonGoc.IdHoaDon, hoaDonThanhToan, chiTietTach.ToList(), currentUser.IdNhanVien))
            {
                MessageBox.Show("Tách và thanh toán hóa đơn thành công!", "Thành công");
                InHoaDonCuoiCung(hoaDonThanhToan);
                NavigateToSoDoBan();
            }
            else
            {
                MessageBox.Show("Tách hóa đơn thất bại.", "Lỗi");
            }
        }

        private void InHoaDonCuoiCung(HoaDon hoaDonDaThanhToan)
        {
            decimal.TryParse(txtKhachDua.Text.Replace(",", ""), out decimal khachDua);
            var previewWindow = new HoaDonPreviewWindow(hoaDonDaThanhToan, chiTietTach.ToList(), currentUser, banHienTai.SoBan, "HÓA ĐƠN THANH TOÁN", khachDua);
            previewWindow.Owner = Window.GetWindow(this);
            previewWindow.ShowDialog();
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void NavigateToSoDoBan()
        {
            var window = Window.GetWindow(this);
            if (window is ManHinhNhanVien main)
            {
                main.MainFrame.Navigate(new SoDoBanView(this.currentUser));
            }
        }

        private void CmbKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) TinhToanTienCuoiCung();
        }

        private void CapNhatThongTinHoaDon(HoaDon hoaDon, bool laThanhToan)
        {
            var selectedKM = cmbKhuyenMai.SelectedItem as KhuyenMai;
            if (selectedKM != null && selectedKM.IdKhuyenMai != 0)
            {
                hoaDon.IdKhuyenMai = selectedKM.IdKhuyenMai;
            }
            else
            {
                hoaDon.IdKhuyenMai = null;
            }

            hoaDon.TongTien = chiTietTach.Sum(i => i.ThanhTien);
            decimal.TryParse(lblTienGiam.Text.Replace("-", "").Replace("đ", "").Replace(",", "").Trim(), out decimal tienGiam);
            decimal.TryParse(lblThanhTien.Text.Replace("đ", "").Replace(",", "").Trim(), out decimal thanhTien);
            hoaDon.SoTienGiam = tienGiam;
            hoaDon.ThanhTien = thanhTien;

            if (rbTienMat.IsChecked == true) hoaDon.PhuongThucThanhToan = "Tiền mặt";
            else if (rbChuyenKhoan.IsChecked == true) hoaDon.PhuongThucThanhToan = "Chuyển khoản";
            else if (rbThe.IsChecked == true) hoaDon.PhuongThucThanhToan = "Thẻ";

            if (laThanhToan)
            {
                hoaDon.TrangThai = "Đã thanh toán";
                hoaDon.ThoiGianThanhToan = DateTime.Now;
            }
            else
            {
                hoaDon.ThoiGianTao = DateTime.Now;
            }
        }

        private void PaymentMethod_Changed(object sender, RoutedEventArgs e)
        {
            if (panelTienMat != null)
            {
                panelTienMat.Visibility = rbTienMat.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion
    }
}