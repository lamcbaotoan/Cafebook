using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Cafebook.Views.nhanvien.pages
{
    public partial class ChamCongView : Page
    {
        private NhanVien currentUser;
        private NhanSuBUS nhanSuBUS = new NhanSuBUS();
        private DispatcherTimer timer;
        private LichLamViec lichLamViecHomNay;
        private BangChamCong chamCongHomNay;

        public ChamCongView(NhanVien user)
        {
            InitializeComponent();
            this.currentUser = user;

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Start();
            LoadData();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss");
        }

        private void LoadData()
        {
            lichLamViecHomNay = nhanSuBUS.GetLichLamViecHomNay(currentUser.IdNhanVien);
            if (lichLamViecHomNay == null)
            {
                lblCaLamViec.Text = "Hôm nay bạn không có lịch làm việc.";
                btnVaoCa.IsEnabled = false;
                btnRaCa.IsEnabled = false;
                lblTrangThaiChamCong.Text = "Không có lịch làm";
            }
            else
            {
                lblCaLamViec.Text = $"Hôm nay: {lichLamViecHomNay.TenCa} ({lichLamViecHomNay.GioBatDau:hh\\:mm} - {lichLamViecHomNay.GioKetThuc:hh\\:mm})";
                chamCongHomNay = nhanSuBUS.GetTrangThaiChamCong(lichLamViecHomNay.IdLichLamViec);
                UpdateUIState();
            }
            dgLichSuChamCong.ItemsSource = nhanSuBUS.GetLichSuChamCong(currentUser.IdNhanVien);
        }

        private void UpdateUIState()
        {
            if (chamCongHomNay == null)
            {
                btnVaoCa.IsEnabled = true;
                btnRaCa.IsEnabled = false;
                lblTrangThaiChamCong.Text = "Trạng thái: Chưa vào ca.";
            }
            else if (chamCongHomNay.GioRa == null)
            {
                btnVaoCa.IsEnabled = false;
                btnRaCa.IsEnabled = true;
                lblTrangThaiChamCong.Text = $"Trạng thái: Đã vào ca lúc {chamCongHomNay.GioVao:HH:mm:ss}";
            }
            else
            {
                btnVaoCa.IsEnabled = false;
                btnRaCa.IsEnabled = false;
                lblTrangThaiChamCong.Text = $"Trạng thái: Đã hoàn thành ca làm việc.";
            }
        }

        private void BtnVaoCa_Click(object sender, RoutedEventArgs e)
        {
            int newId = nhanSuBUS.ThucHienVaoCa(lichLamViecHomNay.IdLichLamViec);
            if (newId > 0)
            {
                chamCongHomNay = new BangChamCong { IdChamCong = newId, GioVao = DateTime.Now };
                UpdateUIState();
            }
        }

        private void BtnRaCa_Click(object sender, RoutedEventArgs e)
        {
            if (nhanSuBUS.ThucHienRaCa(chamCongHomNay.IdChamCong))
            {
                chamCongHomNay.GioRa = DateTime.Now;
                UpdateUIState();
                dgLichSuChamCong.ItemsSource = nhanSuBUS.GetLichSuChamCong(currentUser.IdNhanVien);
            }
        }
    }
}