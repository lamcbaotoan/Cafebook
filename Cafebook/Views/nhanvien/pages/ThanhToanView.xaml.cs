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

            chiTietGoc = new ObservableCollection<ChiTietHoaDon>();
            chiTietTach = new ObservableCollection<ChiTietHoaDon>();
            dgGoc.ItemsSource = chiTietGoc;
            dgTach.ItemsSource = chiTietTach;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblTieuDeThanhToan.Text = $"Thanh toán cho Bàn {banHienTai.SoBan} - Hóa đơn #{hoaDonGoc.IdHoaDon}";
            var dsChiTiet = thanhToanBUS.GetChiTietHoaDon(hoaDonGoc.IdHoaDon);

            chiTietGoc.Clear();
            foreach (var item in dsChiTiet)
            {
                chiTietGoc.Add(item);
            }
            BtnChuyenQuaTatCa_Click(null, null);
        }

        #region TinhToan
        private void CapNhatTienVaKhuyenMai()
        {
            decimal tongTienTach = chiTietTach.Sum(item => item.ThanhTien);
            var idSanPhamTrongHoaDonTach = chiTietTach.Select(item => item.IdSanPham).ToList();

            var dsKMPhuHop = goiMonBUS.GetKhuyenMaiCoTheApDung(tongTienTach, idSanPhamTrongHoaDonTach);
            dsKMPhuHop.Insert(0, new KhuyenMai { IdKhuyenMai = 0, TenKhuyenMai = "Không áp dụng" });

            int? currentSelectedId = (cmbKhuyenMai.SelectedItem as KhuyenMai)?.IdKhuyenMai;
            cmbKhuyenMai.ItemsSource = dsKMPhuHop;
            cmbKhuyenMai.SelectedValuePath = "IdKhuyenMai";

            if (currentSelectedId.HasValue && dsKMPhuHop.Any(km => km.IdKhuyenMai == currentSelectedId.Value))
            {
                cmbKhuyenMai.SelectedValue = currentSelectedId.Value;
            }
            else
            {
                cmbKhuyenMai.SelectedValue = hoaDonGoc.IdKhuyenMai ?? 0;
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
            decimal.TryParse(txtKhachDua.Text, out decimal khachDua);
            decimal.TryParse(lblThanhTien.Text.Replace(" đ", "").Replace(",", ""), out decimal tienCanTra);
            decimal tienThua = khachDua - tienCanTra;

            if (tienThua < 0)
            {
                lblTienThua.Text = "Còn thiếu...";
                lblTienThua.Foreground = Brushes.Red;
            }
            else
            {
                lblTienThua.Text = tienThua.ToString("N0") + " đ";
                lblTienThua.Foreground = Brushes.Blue;
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

            decimal.TryParse(lblTienGiam.Text.Replace("-", "").Replace("đ", "").Replace(",", "").Trim(), out decimal tienGiam);
            decimal.TryParse(lblThanhTien.Text.Replace("đ", "").Replace(",", "").Trim(), out decimal thanhTien);

            var hoaDonTamTinh = new HoaDon
            {
                ThoiGianTao = DateTime.Now,
                TongTien = chiTietTach.Sum(i => i.ThanhTien),
                SoTienGiam = tienGiam,
                ThanhTien = thanhTien
            };

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

            CapNhatThongTinHoaDonGoc();

            if (thanhToanBUS.ThucHienThanhToan(hoaDonGoc))
            {
                MessageBox.Show("Thanh toán thành công!", "Thành công");
                decimal.TryParse(txtKhachDua.Text, out decimal khachDua);
                var previewWindow = new HoaDonPreviewWindow(hoaDonGoc, chiTietTach.ToList(), currentUser, banHienTai.SoBan, "HÓA ĐƠN THANH TOÁN", khachDua);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
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

            CapNhatThongTinHoaDonGoc();

            if (thanhToanBUS.ThucHienTachHoaDon(hoaDonGoc, chiTietTach.ToList(), currentUser.IdNhanVien))
            {
                MessageBox.Show("Tách và thanh toán hóa đơn thành công!", "Thành công");
                decimal.TryParse(txtKhachDua.Text, out decimal khachDua);

                // Tạo hóa đơn tạm để in, vì hoaDonGoc giờ đã bị thay đổi
                var hoaDonDaTachDeIn = new HoaDon
                {
                    TongTien = hoaDonGoc.TongTien,
                    SoTienGiam = hoaDonGoc.SoTienGiam,
                    ThanhTien = hoaDonGoc.ThanhTien,
                    ThoiGianTao = DateTime.Now
                };
                var previewWindow = new HoaDonPreviewWindow(hoaDonDaTachDeIn, chiTietTach.ToList(), currentUser, banHienTai.SoBan, "HÓA ĐƠN THANH TOÁN", khachDua);
                previewWindow.Owner = Window.GetWindow(this);
                previewWindow.ShowDialog();
                NavigateToSoDoBan();
            }
            else
            {
                MessageBox.Show("Tách hóa đơn thất bại.", "Lỗi");
            }
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

        // Hàm mới giúp cập nhật thông tin cho đối tượng hoaDonGoc trước khi xử lý
        private void CapNhatThongTinHoaDonGoc()
        {
            // Lấy ID khuyến mãi từ ComboBox
            hoaDonGoc.IdKhuyenMai = (cmbKhuyenMai.SelectedItem as KhuyenMai)?.IdKhuyenMai;

            // Lấy các giá trị tiền đã được tính toán trên giao diện
            hoaDonGoc.TongTien = chiTietTach.Sum(i => i.ThanhTien);
            decimal.TryParse(lblTienGiam.Text.Replace("-", "").Replace("đ", "").Replace(",", "").Trim(), out decimal tienGiam);
            decimal.TryParse(lblThanhTien.Text.Replace("đ", "").Replace(",", "").Trim(), out decimal thanhTien);
            hoaDonGoc.SoTienGiam = tienGiam;
            hoaDonGoc.ThanhTien = thanhTien;
        }


        #endregion
    }
}