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

        public WelcomeWindow(NhanVien user, Window parent = null, int durationMs = 1400)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _parent = parent;
            _displayMs = durationMs;

            // ensure PrimaryBrush resource exists locally (optional)
            if (!this.Resources.Contains("PrimaryBrush"))
            {
                this.Resources["PrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC"));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtWelcome.Text = $"CHÀO MỪNG, {_user.HoTen?.ToUpper()}";
            txtUserGreeting.Text = $"{_user.HoTen}";
            StartShimmerAnimation();
            StartSpinner();

            _ = ContinueToTargetAsync();
        }

        private void StartSpinner()
        {
            // Use a simple linear double animation
            var spin = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromMilliseconds(900)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            SpinnerRotate.BeginAnimation(RotateTransform.AngleProperty, spin);
        }

        private void StartShimmerAnimation()
        {
            var gBrush = new LinearGradientBrush();
            gBrush.StartPoint = new Point(0, 0);
            gBrush.EndPoint = new Point(1, 0);

            gBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#333333"), 0.0));
            gBrush.GradientStops.Add(new GradientStop(Colors.White, 0.45));
            gBrush.GradientStops.Add(new GradientStop(Colors.White, 0.55));
            gBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#333333"), 1.0));

            txtWelcome.Foreground = gBrush;

            // Animate the two middle gradient stops to create the shimmer
            var anim1 = new DoubleAnimation(-0.6, 1.6, new Duration(TimeSpan.FromMilliseconds(1200)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            gBrush.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, anim1);

            var anim2 = new DoubleAnimation(-0.4, 1.8, new Duration(TimeSpan.FromMilliseconds(1200)))
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            gBrush.GradientStops[2].BeginAnimation(GradientStop.OffsetProperty, anim2);
        }

        // Trong file Views/Common/WelcomeWindow.xaml.cs

        private async Task ContinueToTargetAsync()
        {
            await Task.Delay(_displayMs);

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Window nextWindow;
                    if (_user.IdVaiTro == 1)
                    {
                        // Tương tự, nếu ManHinhAdmin cần thông tin user, bạn cũng nên truyền vào
                        // nextWindow = new ManHinhAdmin(_user); 
                        nextWindow = new ManHinhAdmin(); // Tạm thời giữ nguyên nếu chưa cần
                    }
                    else
                    {
                        // SỬA DÒNG NÀY:
                        // Thay vì: nextWindow = new ManHinhNhanVien();
                        // Sửa thành:
                        nextWindow = new ManHinhNhanVien(_user);
                    }

                    nextWindow.Show();

                    // Close welcome
                    this.Close();

                    // Close/Hide login (parent)
                    if (_parent != null)
                    {
                        try
                        {
                            _parent.Close();
                        }
                        catch
                        {
                            // ignore
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi mở màn hình: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    if (_parent != null)
                        _parent.Show();
                }
            });
        }
    }
}
