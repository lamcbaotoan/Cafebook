using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class GoiMonView : Page
    {
        private GoiMonBUS goiMonBUS = new GoiMonBUS();
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private Ban banHienTai;
        private NhanVien currentUser;
        private HoaDon hoaDonHienTai;
        private ObservableCollection<ChiTietHoaDon> chiTietHoaDonOC;

        public GoiMonView(Ban ban, NhanVien user)
        {
            InitializeComponent();
            this.banHienTai = ban;
            this.currentUser = user;
            chiTietHoaDonOC = new ObservableCollection<ChiTietHoaDon>();
            dgChiTietHoaDon.ItemsSource = chiTietHoaDonOC;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInitialData();
            hoaDonHienTai = goiMonBUS.GetHoaDonChuaThanhToan(banHienTai.IdBan);
            chiTietHoaDonOC.Clear();

            if (hoaDonHienTai == null)
            {
                hoaDonHienTai = new HoaDon { IdBan = banHienTai.IdBan, IdNhanVien = currentUser.IdNhanVien, ThoiGianTao = System.DateTime.Now };
            }
            else
            {
                var chiTiet = goiMonBUS.GetChiTietHoaDon(hoaDonHienTai.IdHoaDon);
                foreach (var item in chiTiet)
                {
                    chiTietHoaDonOC.Add(item);
                }
            }
            lblTieuDeHoaDon.Text = "Hóa đơn - " + banHienTai.SoBan;
            CapNhatTongTienVaKhuyenMai();
        }

        private void LoadInitialData()
        {
            var loaiSpList = sanPhamBUS.GetDanhSachLoaiSP();
            lbLoaiSP.ItemsSource = loaiSpList;

            this.Dispatcher.InvokeAsync(() =>
            {
                if (lbLoaiSP.Items.Count > 0)
                {
                    var firstItemContainer = lbLoaiSP.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
                    if (firstItemContainer != null)
                    {
                        var radioButton = FindVisualChild<RadioButton>(firstItemContainer);
                        if (radioButton != null)
                        {
                            radioButton.IsChecked = true;
                        }
                    }
                }
            });
        }

        private void Category_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton selectedRadioButton && selectedRadioButton.DataContext is LoaiSanPham selected)
            {
                icSanPham.ItemsSource = goiMonBUS.GetSanPhamTheoLoai(selected.IdLoaiSP);
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    return t;
                }
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = (sender as Button)?.DataContext as SanPham;
            if (selectedProduct == null) return;

            var existingItem = chiTietHoaDonOC.FirstOrDefault(item => item.IdSanPham == selectedProduct.IdSanPham);
            int soLuongHienTaiTrongBill = existingItem?.SoLuong ?? 0;
            if (soLuongHienTaiTrongBill >= selectedProduct.SoLuongCoThePhucVu)
            {
                MessageBox.Show($"Rất tiếc, nguyên liệu cho món '{selectedProduct.TenSanPham}' đã hết hoặc không đủ.", "Hết hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (existingItem != null)
            {
                existingItem.SoLuong++;
            }
            else
            {
                chiTietHoaDonOC.Add(new ChiTietHoaDon
                {
                    IdSanPham = selectedProduct.IdSanPham,
                    TenSanPham = selectedProduct.TenSanPham,
                    DonGiaLucBan = selectedProduct.DonGia,
                    SoLuong = 1
                });
            }
            CapNhatTongTienVaKhuyenMai();
        }

        private void BtnTangSL_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ChiTietHoaDon;
            if (item != null)
            {
                int soLuongCoThePhucVu = sanPhamBUS.KiemTraKhaNangPhucVu(item.IdSanPham);
                if (item.SoLuong >= soLuongCoThePhucVu)
                {
                    MessageBox.Show($"Không đủ nguyên liệu để thêm món '{item.TenSanPham}'. Chỉ còn phục vụ được {soLuongCoThePhucVu} phần.", "Hết hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                item.SoLuong++;
                CapNhatTongTienVaKhuyenMai();
            }
        }

        private void BtnGiamSL_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ChiTietHoaDon;
            if (item != null && item.SoLuong > 1)
            {
                item.SoLuong--;
                CapNhatTongTienVaKhuyenMai();
            }
        }

        private void BtnXoaMon_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ChiTietHoaDon;
            if (item != null)
            {
                chiTietHoaDonOC.Remove(item);
                CapNhatTongTienVaKhuyenMai();
            }
        }

        private void CmbKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbKhuyenMai.IsDropDownOpen)
            {
                TinhToanTienCuoiCung();
            }
        }

        private void CapNhatTongTienVaKhuyenMai()
        {
            decimal tongTien = chiTietHoaDonOC.Sum(item => item.ThanhTien);
            var idSanPhamTrongHoaDon = chiTietHoaDonOC.Select(item => item.IdSanPham).ToList();
            var dsKMPhuHop = goiMonBUS.GetKhuyenMaiCoTheApDung(tongTien, idSanPhamTrongHoaDon);
            dsKMPhuHop.Insert(0, new KhuyenMai { IdKhuyenMai = 0, TenKhuyenMai = "Không áp dụng" });

            int? currentSelectedId = (cmbKhuyenMai.SelectedItem as KhuyenMai)?.IdKhuyenMai ?? hoaDonHienTai.IdKhuyenMai;
            cmbKhuyenMai.ItemsSource = dsKMPhuHop;
            cmbKhuyenMai.SelectedValuePath = "IdKhuyenMai";

            if (currentSelectedId.HasValue && dsKMPhuHop.Any(km => km.IdKhuyenMai == currentSelectedId.Value))
            {
                cmbKhuyenMai.SelectedValue = currentSelectedId.Value;
            }
            else
            {
                cmbKhuyenMai.SelectedIndex = 0;
            }
            TinhToanTienCuoiCung();
        }

        private void TinhToanTienCuoiCung()
        {
            decimal tongTien = chiTietHoaDonOC.Sum(item => item.ThanhTien);
            decimal soTienGiam = 0;

            if (cmbKhuyenMai.SelectedItem is KhuyenMai km && km.IdKhuyenMai != 0)
            {
                if (km.LoaiGiamGia == "PhanTram") soTienGiam = tongTien * (km.GiaTriGiam / 100);
                else soTienGiam = km.GiaTriGiam;
                hoaDonHienTai.IdKhuyenMai = km.IdKhuyenMai;
            }
            else
            {
                hoaDonHienTai.IdKhuyenMai = null;
            }

            decimal thanhTien = tongTien - soTienGiam;
            hoaDonHienTai.TongTien = tongTien;
            hoaDonHienTai.SoTienGiam = soTienGiam;
            hoaDonHienTai.ThanhTien = thanhTien;

            lblTongTien.Text = tongTien.ToString("N0");
            lblTienGiam.Text = soTienGiam.ToString("N0");
            lblThanhTien.Text = thanhTien.ToString("N0") + " VND";
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            TinhToanTienCuoiCung();
            var result = goiMonBUS.LuuHoaDon(hoaDonHienTai, chiTietHoaDonOC.ToList());
            if (result != null)
            {
                hoaDonHienTai = result;
                MessageBox.Show("Lưu hóa đơn thành công!");
            }
            else
            {
                MessageBox.Show("Lưu hóa đơn thất bại!");
            }
        }

        private void BtnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (chiTietHoaDonOC.Count == 0)
            {
                MessageBox.Show("Hóa đơn trống, không thể thanh toán.");
                return;
            }
            BtnLuu_Click(null, null);
            // Cần đảm bảo ThanhToanView tồn tại và có constructor phù hợp
            this.NavigationService?.Navigate(new ThanhToanView(this.hoaDonHienTai, this.banHienTai, this.currentUser));
        }

        private void BtnInTamTinh_Click(object sender, RoutedEventArgs e)
        {
            // SỬA: Cập nhật thời gian của hóa đơn ngay trước khi in
            if (this.hoaDonHienTai != null)
            {
                this.hoaDonHienTai.ThoiGianTao = System.DateTime.Now;
            }

            var previewWindow = new HoaDonPreviewWindow(this.hoaDonHienTai, chiTietHoaDonOC.ToList(), this.currentUser, this.banHienTai.SoBan);
            previewWindow.Owner = Window.GetWindow(this);
            previewWindow.ShowDialog();
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }
    }
}