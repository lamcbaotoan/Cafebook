using Cafebook.BUS;
using Cafebook.DTO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
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
            UpdateLiveShiftInfo(); // Cập nhật thông tin ca làm mỗi giây
        }

        private void LoadData()
        {
            lichLamViecHomNay = nhanSuBUS.GetLichLamViecHomNay(currentUser.IdNhanVien);
            chamCongHomNay = lichLamViecHomNay != null ? nhanSuBUS.GetTrangThaiChamCong(lichLamViecHomNay.IdLichLamViec) : null;

            UpdateUIState();
            dgLichSuChamCong.ItemsSource = nhanSuBUS.GetLichSuChamCong(currentUser.IdNhanVien);
        }

        private void UpdateUIState()
        {
            if (lichLamViecHomNay == null || lichLamViecHomNay.TrangThai != "Đi Làm")
            {
                string message = "Hôm nay bạn không có lịch làm việc.";
                if (lichLamViecHomNay != null)
                {
                    message = $"Trạng thái hôm nay: {lichLamViecHomNay.TrangThai}";
                }
                lblCaLamViec.Text = message;
                btnVaoCa.IsEnabled = false;
                btnRaCa.IsEnabled = false;
                lblTrangThaiChamCong.Text = "Không có ca làm";
            }
            else
            {
                lblCaLamViec.Text = $"Ca làm hôm nay: {lichLamViecHomNay.GioBatDau:hh\\:mm} - {lichLamViecHomNay.GioKetThuc:hh\\:mm}";
                if (chamCongHomNay == null)
                {
                    btnVaoCa.IsEnabled = true;
                    btnRaCa.IsEnabled = false;
                    lblTrangThaiChamCong.Text = "Trạng thái: Chưa vào ca.";
                    lblTrangThaiChamCong.Foreground = Brushes.Black;
                }
                else if (chamCongHomNay.GioRa == null)
                {
                    btnVaoCa.IsEnabled = false;
                    btnRaCa.IsEnabled = true;
                    lblTrangThaiChamCong.Text = $"Trạng thái: Đã vào ca lúc {chamCongHomNay.GioVao:HH:mm:ss}";
                    lblTrangThaiChamCong.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#27AE60"));
                }
                else
                {
                    btnVaoCa.IsEnabled = false;
                    btnRaCa.IsEnabled = false;
                    lblTrangThaiChamCong.Text = $"Trạng thái: Đã hoàn thành ca làm việc.";
                    lblTrangThaiChamCong.Foreground = Brushes.Gray;
                }
            }
        }

        private void BtnVaoCa_Click(object sender, RoutedEventArgs e)
        {
            if (lichLamViecHomNay == null) return;

            DateTime shiftStart = DateTime.Today.Add(lichLamViecHomNay.GioBatDau.GetValueOrDefault());
            if (DateTime.Now > shiftStart)
            {
                MessageBox.Show($"Bạn đang vào ca muộn {FormatDuration(DateTime.Now - shiftStart)}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            int newId = nhanSuBUS.ThucHienVaoCa(lichLamViecHomNay.IdLichLamViec);
            if (newId > 0)
            {
                chamCongHomNay = new BangChamCong { IdChamCong = newId, GioVao = DateTime.Now };
                UpdateUIState();
            }
        }

        private void BtnRaCa_Click(object sender, RoutedEventArgs e)
        {
            if (chamCongHomNay == null) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn kết thúc ca làm việc không?", "Xác nhận ra ca", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (nhanSuBUS.ThucHienRaCa(chamCongHomNay.IdChamCong))
                {
                    LoadData();
                }
            }
        }

        // ===============================================================
        // HÀM ĐIỀU PHỐI CHÍNH (Đã hoàn thiện)
        // ===============================================================
        private void UpdateLiveShiftInfo()
        {
            if (lichLamViecHomNay == null || lichLamViecHomNay.TrangThai != "Đi Làm")
            {
                UpdateUI_KhongCoCa();
                return;
            }

            DateTime shiftStart = DateTime.Today.Add(lichLamViecHomNay.GioBatDau.GetValueOrDefault());
            DateTime shiftEnd = DateTime.Today.Add(lichLamViecHomNay.GioKetThuc.GetValueOrDefault());
            if (shiftEnd <= shiftStart) shiftEnd = shiftEnd.AddDays(1);

            // Thiết lập giá trị tối đa cho progress bar là tổng thời gian ca làm
            pbThoiGianLamViec.Maximum = Math.Max(1, (shiftEnd - shiftStart).TotalSeconds);

            if (chamCongHomNay == null)
            {
                UpdateUI_ChuaVaoCa(shiftStart);
            }
            else if (chamCongHomNay.GioRa == null)
            {
                UpdateUI_DangTrongCa(shiftEnd);
            }
            else
            {
                UpdateUI_DaRaCa();
            }
        }

        // ===============================================================
        // CÁC HÀM HỖ TRỢ (Đã hoàn thiện)
        // ===============================================================

        private void UpdateUI_KhongCoCa()
        {
            lblGioVaoThucTe.Text = "--";
            lblThoiGianDaLam.Text = "--";
            lblThoiGianConLai.Text = "--";
            lblCanhBao.Text = "";
            pbThoiGianLamViec.Value = 0;
        }

        private void UpdateUI_ChuaVaoCa(DateTime shiftStart)
        {
            lblGioVaoThucTe.Text = "Chưa vào ca";
            lblThoiGianDaLam.Text = "0 giờ 0 phút";
            pbThoiGianLamViec.Value = 0;
            lblCanhBao.Text = "";

            TimeSpan timeUntilShift = shiftStart - DateTime.Now;
            if (timeUntilShift.TotalSeconds > 0)
            {
                lblThoiGianConLai.Text = $"Bắt đầu sau {FormatDuration(timeUntilShift)}";
            }
            else
            {
                lblThoiGianConLai.Text = "Đã đến giờ vào ca";
                lblCanhBao.Text = $"CẢNH BÁO: BẠN ĐANG TRỄ {FormatDuration(DateTime.Now - shiftStart)}!";
            }
        }

        private void UpdateUI_DangTrongCa(DateTime shiftEnd)
        {
            lblGioVaoThucTe.Text = $"{chamCongHomNay.GioVao:HH:mm:ss}";

            TimeSpan workedDuration = DateTime.Now - chamCongHomNay.GioVao.Value;
            lblThoiGianDaLam.Text = FormatDuration(workedDuration);
            pbThoiGianLamViec.Value = workedDuration.TotalSeconds;

            TimeSpan remainingDuration = shiftEnd - DateTime.Now;
            if (remainingDuration.TotalSeconds > 0)
            {
                lblThoiGianConLai.Text = FormatDuration(remainingDuration);
                pbThoiGianLamViec.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#27AE60")); // Màu xanh lá
            }
            else
            {
                TimeSpan overtimeDuration = DateTime.Now - shiftEnd;
                lblThoiGianConLai.Text = $"Tăng ca {FormatDuration(overtimeDuration)}";
                pbThoiGianLamViec.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498DB")); // Màu xanh dương
            }
        }

        private void UpdateUI_DaRaCa()
        {
            lblGioVaoThucTe.Text = $"{chamCongHomNay.GioVao:HH:mm:ss}";
            TimeSpan workedDuration = chamCongHomNay.GioRa.Value - chamCongHomNay.GioVao.Value;
            lblThoiGianDaLam.Text = FormatDuration(workedDuration);
            lblThoiGianConLai.Text = "Đã kết thúc ca";
            pbThoiGianLamViec.Value = pbThoiGianLamViec.Maximum;
        }

        private string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalDays >= 1) return $"{(int)ts.TotalDays} ngày {ts.Hours} giờ";
            if (ts.TotalHours >= 1) return $"{(int)ts.TotalHours} giờ {ts.Minutes} phút";
            if (ts.TotalMinutes >= 1) return $"{ts.Minutes} phút {ts.Seconds} giây";
            return $"{ts.Seconds} giây";
        }
    }
}