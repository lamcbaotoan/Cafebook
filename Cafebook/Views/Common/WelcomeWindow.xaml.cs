using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Cafebook.DTO;
using Cafebook.Views.admin;
using Cafebook.Views.nhanvien;

namespace Cafebook.Views.Common
{
    public partial class WelcomeWindow : Window
    {
        private readonly NhanVien _user;
        private readonly Window _parent;
        private readonly int _displayMs;

        public WelcomeWindow(NhanVien user, Window parent = null, int durationMs = 2500)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _parent = parent;
            _displayMs = durationMs;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Cập nhật tên người dùng từ object NhanVien
            string hoTen = _user.HoTen ?? "bạn";
            txtWelcome.Text = $"CHÀO MỪNG";
            txtUserGreeting.Text = $"Xin chào, {hoTen}!";

            // Khởi chạy các hiệu ứng
            StartShimmerAnimation();

            // Bắt đầu tiến trình chuyển trang
            _ = ContinueToTargetAsync();
        }

        private void StartShimmerAnimation()
        {
            // Lấy màu từ Resources trong XAML thay vì code cứng
            var darkColor = ((SolidColorBrush)this.Resources["DarkTextBrush"]).Color;
            var highlightColor = ((SolidColorBrush)this.Resources["LightCreamBrush"]).Color;

            var gBrush = new LinearGradientBrush
            {
                StartPoint = new Point(-1, 0), // Bắt đầu từ ngoài màn hình bên trái
                EndPoint = new Point(2, 0)      // Kết thúc ở ngoài màn hình bên phải
            };

            // Tạo dải màu chuyển sắc
            gBrush.GradientStops.Add(new GradientStop(darkColor, 0.0));
            gBrush.GradientStops.Add(new GradientStop(darkColor, 0.4));
            gBrush.GradientStops.Add(new GradientStop(highlightColor, 0.5));
            gBrush.GradientStops.Add(new GradientStop(darkColor, 0.6));
            gBrush.GradientStops.Add(new GradientStop(darkColor, 1.0));

            txtWelcome.Foreground = gBrush;

            // Animation cho StartPoint
            var startPointAnim = new PointAnimation(new Point(-1, 0), new Point(1, 0), new Duration(TimeSpan.FromMilliseconds(2000)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            gBrush.BeginAnimation(LinearGradientBrush.StartPointProperty, startPointAnim);

            // Animation cho EndPoint
            var endPointAnim = new PointAnimation(new Point(0, 0), new Point(2, 0), new Duration(TimeSpan.FromMilliseconds(2000)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            gBrush.BeginAnimation(LinearGradientBrush.EndPointProperty, endPointAnim);
        }

        private async Task ContinueToTargetAsync()
        {
            await Task.Delay(_displayMs);

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Window nextWindow;
                    if (_user.IdVaiTro == 1) // Giả sử 1 là Admin
                    {
                        nextWindow = new ManHinhAdmin();
                    }
                    else
                    {
                        nextWindow = new ManHinhNhanVien(_user);
                    }

                    nextWindow.Show();
                    this.Close();

                    if (_parent != null)
                    {
                        try { _parent.Close(); }
                        catch { /* ignore */ }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi mở màn hình chính: " + ex.Message, "Lỗi nghiêm trọng", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    if (_parent != null)
                        _parent.Show();
                }
            });
        }
    }
}