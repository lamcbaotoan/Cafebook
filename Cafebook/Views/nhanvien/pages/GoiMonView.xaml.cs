using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class GoiMonView : Page
    {
        private GoiMonBUS goiMonBUS = new GoiMonBUS();
        private SanPhamBUS sanPhamBUS = new SanPhamBUS();
        private Ban banHienTai;
        private NhanVien currentUser; // << THÊM BIẾN NÀY ĐỂ LƯU NHÂN VIÊN
        private HoaDon hoaDonHienTai;
        private ObservableCollection<ChiTietHoaDon> chiTietHoaDonOC;

        // SỬA LẠI CONSTRUCTOR
        public GoiMonView(Ban ban, NhanVien user)
        {
            InitializeComponent();
            this.banHienTai = ban;
            this.currentUser = user; // << LƯU LẠI NHÂN VIÊN
            chiTietHoaDonOC = new ObservableCollection<ChiTietHoaDon>();
            dgChiTietHoaDon.ItemsSource = chiTietHoaDonOC;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lbLoaiSP.ItemsSource = sanPhamBUS.GetDanhSachLoaiSP();
            if (lbLoaiSP.Items.Count > 0) lbLoaiSP.SelectedIndex = 0;

            hoaDonHienTai = goiMonBUS.GetHoaDonChuaThanhToan(banHienTai.IdBan);

            if (hoaDonHienTai == null)
            {
                hoaDonHienTai = new HoaDon { IdBan = banHienTai.IdBan, IdNhanVien = 1, ThoiGianTao = System.DateTime.Now };
            }
            else
            {
                var chiTiet = goiMonBUS.GetChiTietHoaDon(hoaDonHienTai.IdHoaDon);
                foreach (var item in chiTiet) chiTietHoaDonOC.Add(item);
            }

            lblTieuDeHoaDon.Text = "Hóa đơn - " + banHienTai.SoBan;
            CapNhatTongTienVaKhuyenMai();
        }

        private void LbLoaiSP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbLoaiSP.SelectedItem is LoaiSanPham selected)
            {
                icSanPham.ItemsSource = goiMonBUS.GetSanPhamTheoLoai(selected.IdLoaiSP);
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = (sender as Button)?.DataContext as SanPham;
            if (selectedProduct == null) return;

            var existingItem = chiTietHoaDonOC.FirstOrDefault(item => item.IdSanPham == selectedProduct.IdSanPham);
            int soLuongHienTaiTrongBill = existingItem?.SoLuong ?? 0;

            // KIỂM TRA LẠI TỒN KHO TRƯỚC KHI THÊM
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
                // KIỂM TRA TỒN KHO KHI TĂNG SỐ LƯỢNG
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
            if (item != null && item.SoLuong > 1) item.SoLuong--;
            CapNhatTongTienVaKhuyenMai();
        }
        private void BtnXoaMon_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement)?.DataContext as ChiTietHoaDon;
            if (item != null) chiTietHoaDonOC.Remove(item);
            CapNhatTongTienVaKhuyenMai();
        }

        private void CmbKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbKhuyenMai.IsDropDownOpen)
                TinhToanTienCuoiCung();
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

        // SỬA LẠI HÀM NÀY
        private void BtnThanhToan_Click(object sender, RoutedEventArgs e)
        {
            if (chiTietHoaDonOC.Count == 0)
            {
                MessageBox.Show("Hóa đơn trống, không thể thanh toán.");
                return;
            }
            BtnLuu_Click(null, null);

            // SỬA LẠI DÒNG NÀY: Truyền thêm 'this.currentUser'
            this.NavigationService?.Navigate(new ThanhToanView(this.hoaDonHienTai, this.banHienTai, this.currentUser));
        }

        private void BtnInTamTinh_Click(object sender, RoutedEventArgs e)
        {
            // Sửa lại để dùng currentUser
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