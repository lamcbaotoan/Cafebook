using Cafebook.BUS;
using Cafebook.DTO;
using System.Windows;

namespace Cafebook.Views.Common
{
    public partial class BaoCaoSuCoWindow : Window
    {
        private Ban _ban;
        private NhanVien _nhanVien;
        private ThongBaoBUS thongBaoBUS = new ThongBaoBUS();

        public BaoCaoSuCoWindow(Ban ban, NhanVien nhanVien)
        {
            InitializeComponent();
            _ban = ban;
            _nhanVien = nhanVien;
            lblTieuDe.Text = $"Báo cáo sự cố cho Bàn {_ban.SoBan}";
            txtNoiDungSuCo.Focus();
        }

        private void BtnGuiBaoCao_Click(object sender, RoutedEventArgs e)
        {
            string noiDung = txtNoiDungSuCo.Text;
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                MessageBox.Show("Vui lòng nhập nội dung sự cố.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var thongBao = new ThongBao
            {
                IdNhanVien = _nhanVien.IdNhanVien,
                NoiDung = $"[Bàn {_ban.SoBan}]: {noiDung}"
            };

            if (thongBaoBUS.GuiThongBao(thongBao))
            {
                MessageBox.Show("Gửi báo cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Gửi báo cáo thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}