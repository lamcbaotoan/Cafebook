using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.admin;
using Cafebook.Views.nhanvien;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cafebook.Views.Common;

namespace Cafebook
{
    public partial class ManHinhDangNhap : Window
    {
        private NhanVienBUS nhanVienBUS;
        private bool isProcessing = false;

        public ManHinhDangNhap()
        {
            InitializeComponent();
            nhanVienBUS = new NhanVienBUS();
            // Optional: set default values for testing (comment out in production)
            // txtUsername.Text = "demo@cafebook.vn";
            // txtPassword.Password = "123456";
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessing) return;

            string username = txtUsername.Text?.Trim();
            string password = txtPassword.Password;

            // 1. Kiểm tra đầu vào
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                isProcessing = true;
                btnLogin.IsEnabled = false;
                btnLogin.Content = "Đang đăng nhập...";

                // 2. Gọi lớp BUS để kiểm tra (giả sử đây là gọi sync). Nếu là gọi DB nặng, chạy async.
                // Mình giả sử KiemTraDangNhap là sync; nếu bạn có phiên bản async thay vào await nhanVienBUS.KiemTraDangNhapAsync(...)
                NhanVien loggedInUser = await Task.Run(() => nhanVienBUS.KiemTraDangNhap(username, password));

                // 3. Xử lý kết quả
                if (loggedInUser != null)
                {
                    // 1) Thay vì hiện MessageBox đơn giản, gọi màn hình chào mừng có animation + loading
                    // Ẩn login window trước để tránh flicker
                    this.Hide();

                    // Tạo và hiển thị welcome window (Modal) — nó sẽ tự mở màn hình tiếp theo rồi đóng login
                    var welcome = new Cafebook.Views.Common.WelcomeWindow(loggedInUser, this, durationMs: 1400);
                    welcome.Owner = this; // đảm bảo center on owner
                    welcome.ShowDialog();

                    // Note: WelcomeWindow sẽ tự Close() login window khi đã mở next window.
                    // Nếu welcome bị đóng và login vẫn còn hiển thị (trường hợp lỗi), show lại hoặc đóng.
                    if (this.IsVisible)
                    {
                        this.Close();
                    }
                }

                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng nhập: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isProcessing = false;
                btnLogin.IsEnabled = true;
                btnLogin.Content = "Đăng nhập";
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Bắt phím Enter để đăng nhập
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Khi đang xử lý thì bỏ qua
                if (!isProcessing)
                {
                    BtnLogin_Click(btnLogin, new RoutedEventArgs());
                }
            }
            else if (e.Key == Key.Escape)
            {
                BtnExit_Click(btnExit, new RoutedEventArgs());
            }
        }

        // Thực hiện 1 chút hiệu ứng khi load (nếu muốn)
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Ví dụ: focus vào username ngay khi mở
            txtUsername.Focus();
            // Nếu muốn, bạn có thể thêm animation programmatically hoặc qua XAML (mình đã thêm shadow/fade in qua XAML resources).
        }
    }
}
