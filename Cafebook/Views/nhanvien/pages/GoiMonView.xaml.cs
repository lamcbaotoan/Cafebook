using Cafebook.BUS;
using Cafebook.DTO;
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
        private HoaDon hoaDonHienTai;
        private ObservableCollection<ChiTietHoaDon> chiTietHoaDonOC;

        public GoiMonView(Ban ban)
        {
            InitializeComponent();
            this.banHienTai = ban;
            chiTietHoaDonOC = new ObservableCollection<ChiTietHoaDon>();
            dgChiTietHoaDon.ItemsSource = chiTietHoaDonOC;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInitialData();
            hoaDonHienTai = goiMonBUS.GetHoaDonChuaThanhToan(banHienTai.IdBan);

            if (hoaDonHienTai == null)
            {
                hoaDonHienTai = new HoaDon { IdBan = banHienTai.IdBan, IdNhanVien = 1 }; // Thay ID 1 bằng ID nhân viên đăng nhập
            }
            else
            {
                var chiTiet = goiMonBUS.GetChiTietHoaDon(hoaDonHienTai.IdHoaDon);
                foreach (var item in chiTiet) chiTietHoaDonOC.Add(item);
                cmbKhuyenMai.SelectedValue = hoaDonHienTai.IdKhuyenMai;
            }

            lblTieuDeHoaDon.Text = "Hóa đơn - " + banHienTai.SoBan;
            CapNhatTongTienHoaDon();
        }

        private void LoadInitialData()
        {
            lbLoaiSP.ItemsSource = sanPhamBUS.GetDanhSachLoaiSP();
            if (lbLoaiSP.Items.Count > 0) lbLoaiSP.SelectedIndex = 0;

            var dsKM = goiMonBUS.GetKhuyenMaiHopLe();
            dsKM.Insert(0, new KhuyenMai { IdKhuyenMai = 0, TenKhuyenMai = "Không áp dụng" });
            cmbKhuyenMai.ItemsSource = dsKM;
            cmbKhuyenMai.SelectedValuePath = "IdKhuyenMai";
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
            CapNhatTongTienHoaDon();
        }

        private void BtnTangSL_Click(object sender, RoutedEventArgs e) { /* ... Giữ nguyên ... */ }
        private void BtnGiamSL_Click(object sender, RoutedEventArgs e) { /* ... Giữ nguyên ... */ }
        private void BtnXoaMon_Click(object sender, RoutedEventArgs e) { /* ... Giữ nguyên ... */ }
        private void CmbKhuyenMai_SelectionChanged(object sender, SelectionChangedEventArgs e) => CapNhatTongTienHoaDon();

        private void CapNhatTongTienHoaDon()
        {
            decimal tongTien = chiTietHoaDonOC.Sum(item => item.ThanhTien);
            decimal soTienGiam = 0;

            if (cmbKhuyenMai.SelectedItem is KhuyenMai km && km.IdKhuyenMai != 0)
            {
                if (km.LoaiGiamGia == "PhanTram")
                {
                    soTienGiam = tongTien * (km.GiaTriGiam / 100);
                }
                else // SoTien
                {
                    soTienGiam = km.GiaTriGiam;
                }
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
            CapNhatTongTienHoaDon(); // Cập nhật lần cuối
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
            this.NavigationService?.Navigate(new ThanhToanView(this.hoaDonHienTai));
        }

        private void BtnQuayLai_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.GoBack();
        }
    }
}