using Cafebook.BUS;
using Cafebook.DTO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cafebook.Views.Common
{
    public partial class ThongBaoChiTietWindow : Window
    {
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS();
        public bool DaThayDoi { get; private set; } = false; // Thuộc tính để báo cho Dashboard biết cần tải lại

        public ThongBaoChiTietWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            icThongBao.ItemsSource = thongBaoBUS.GetThongBaoChuaDoc();
        }

        // Đánh dấu 1 item đã đọc
        private void BtnDanhDauDaDocItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is ThongBao selected)
            {
                if (thongBaoBUS.DanhDauDaDoc(selected.IdThongBao))
                {
                    DaThayDoi = true;
                    LoadData(); // Tải lại danh sách trong cửa sổ này
                }
            }
        }

        // Đánh dấu tất cả đã đọc
        private void BtnDanhDauTatCaDaDoc_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn đánh dấu tất cả thông báo là đã đọc?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (thongBaoBUS.DanhDauTatCaDaDoc())
                {
                    DaThayDoi = true;
                    LoadData();
                }
            }
        }
    }
}