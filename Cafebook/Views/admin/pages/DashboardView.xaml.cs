using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Cafebook.BUS;

namespace Cafebook.Views.admin.pages
{
    public partial class DashboardView : Page
    {
        private ThongKeBUS thongKeBUS;

        public DashboardView()
        {
            InitializeComponent();
            thongKeBUS = new ThongKeBUS();
        }

        // Sự kiện này sẽ chạy mỗi khi trang được tải
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // Lấy và hiển thị Doanh thu
            decimal doanhThu = thongKeBUS.GetDoanhThuHomNay();
            lblDoanhThu.Text = doanhThu.ToString("N0") + " VND"; // "N0" để format số có dấu phẩy

            // Lấy và hiển thị số đơn hàng
            int soDonHang = thongKeBUS.GetSoDonHangHomNay();
            lblDonHang.Text = soDonHang.ToString();

            // Lấy và hiển thị sản phẩm bán chạy nhất
            string spBanChay = thongKeBUS.GetSanPhamBanChayNhatHomNay();
            lblSanPhamBanChay.Text = spBanChay;
        }

        // Các sự kiện cho nút báo cáo (hiện tại chỉ hiển thị thông báo)
        private void BtnExportRevenue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng 'Xuất Báo cáo Doanh thu' đang được phát triển.", "Thông báo");
        }

        private void BtnExportInventory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng 'Xuất Báo cáo Tồn kho' đang được phát triển.", "Thông báo");
        }

        private void BtnExportPerformance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng 'Xuất Báo cáo Hiệu suất NV' đang được phát triển.", "Thông báo");
        }

        private void BtnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Chức năng 'Xem Lịch sử Hệ thống' đang được phát triển.", "Thông báo");
        }
    }
}