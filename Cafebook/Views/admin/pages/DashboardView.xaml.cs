using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.Common;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Cafebook.Views.admin.pages
{
    public partial class DashboardView : Page
    {
        private ThongKeBUS thongKeBUS = new ThongKeBUS();
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS();
        private BaoCaoBUS baoCaoBUS = new BaoCaoBUS();

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public DashboardView()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
            YFormatter = value => value.ToString("N0") + " VND";
            DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
           // LoadThongBao();
            LoadChartData();
        }

        private void AnimateCountUp(TextBlock textBlock, double finalValue, bool isCurrency)
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = finalValue,
                Duration = new Duration(TimeSpan.FromSeconds(1.2)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            animation.CurrentTimeInvalidated += (s, ev) =>
            {
                var currentValue = ((AnimationClock)s).CurrentProgress.Value * finalValue;
                textBlock.Text = isCurrency ? ((decimal)currentValue).ToString("N0") + " VND" : ((int)currentValue).ToString();
            };
            textBlock.BeginAnimation(OpacityProperty, animation, HandoffBehavior.Compose);
        }

        private void LoadDashboardData()
        {
            AnimateCountUp(lblDoanhThu, (double)thongKeBUS.GetDoanhThuHomNay(), true);
            AnimateCountUp(lblDonHang, thongKeBUS.GetSoDonHangHomNay(), false);
            lblSanPhamBanChay.Text = thongKeBUS.GetSanPhamBanChayNhatHomNay();
        }
        /*
        // CHỈ CÓ MỘT HÀM LoadThongBao() DUY NHẤT
        private void LoadThongBao()
        {
            lbThongBao.ItemsSource = thongBaoBUS.GetThongBaoChuaDoc();
        }

        private void BtnDanhDauDaDoc_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is ThongBao selectedThongBao)
            {
                if (thongBaoBUS.DanhDauDaDoc(selectedThongBao.IdThongBao))
                {
                    LoadThongBao();
                }
            }
        }
        */
        private void LoadChartData()
        {
            var doanhThuData = thongKeBUS.GetDoanhThu30NgayQua();
            SeriesCollection.Clear();
            SeriesCollection.Add(new LineSeries
            {
                Title = "Doanh thu",
                Values = new ChartValues<decimal>(doanhThuData.Values),
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 10
            });
            Labels = doanhThuData.Keys.Select(d => d.ToString("dd/MM")).ToArray();
        }
/*
        private void ThongBao_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var detailWindow = new ThongBaoChiTietWindow();
            detailWindow.Owner = Window.GetWindow(this);
            detailWindow.ShowDialog();
            if (detailWindow.DaThayDoi)
            {
                LoadThongBao();
            }
        }
        */
        // ... Các hàm BtnExport... và BtnCaiDat... giữ nguyên
        private void BtnExportRevenue_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = $"BaoCao_DoanhThu_{DateTime.Now:yyyyMMdd}.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                if (baoCaoBUS.XuatBaoCaoDoanhThu(sfd.FileName, firstDayOfMonth, lastDayOfMonth))
                    MessageBox.Show("Xuất báo cáo thành công!", "Thành công");
            }
        }

        // ĐỔI TÊN HÀM NÀY VÀ SỬA LẠI LỜI GỌI
        private void BtnExportTonKhoSach_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"BaoCao_TonKhoSach_{DateTime.Now:yyyyMMdd}.xlsx"
            };
            if (sfd.ShowDialog() == true)
            {
                if (baoCaoBUS.XuatBaoCaoTonKhoSach(sfd.FileName)) // Sửa tên hàm gọi
                    MessageBox.Show($"Đã xuất báo cáo tồn kho sách thành công!", "Thành công");
                else
                    MessageBox.Show("Xuất báo cáo thất bại.", "Lỗi");
            }
        }

        // THÊM SỰ KIỆN MỚI NÀY
        private void BtnExportNguyenLieu_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"BaoCao_TonKhoNguyenLieu_{DateTime.Now:yyyyMMdd}.xlsx"
            };
            if (sfd.ShowDialog() == true)
            {
                if (baoCaoBUS.XuatBaoCaoTonKhoNguyenLieu(sfd.FileName))
                    MessageBox.Show($"Đã xuất báo cáo tồn kho nguyên liệu thành công!", "Thành công");
                else
                    MessageBox.Show("Xuất báo cáo thất bại.", "Lỗi");
            }
        }

        private void BtnExportPerformance_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx", FileName = $"BaoCao_HieuSuatNV_{DateTime.Now:yyyyMMdd}.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                if (baoCaoBUS.XuatBaoCaoHieuSuatNV(sfd.FileName, firstDayOfMonth, lastDayOfMonth))
                    MessageBox.Show("Xuất báo cáo thành công!", "Thành công");
            }
        }

        private void BtnCaiDat_Click(object sender, RoutedEventArgs e)
        {
            var settingWindow = new CaiDatThongTinWindow();
            settingWindow.Owner = Window.GetWindow(this);
            settingWindow.ShowDialog();
        }
    }
}