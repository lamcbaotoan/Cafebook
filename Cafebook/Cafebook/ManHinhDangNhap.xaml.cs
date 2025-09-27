using Cafebook.BUS;
using Cafebook.DTO;
using Cafebook.Views.admin;
using Cafebook.Views.nhanvien;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls; // NEW: Added for TextChangedEventArgs
using System.Windows.Input;
using Cafebook.Views.Common;

namespace Cafebook
{
    public partial class ManHinhDangNhap : Window
    {
        private TaiKhoanBUS taiKhoanBUS;
        private bool isProcessing = false;
        private bool _isPasswordSyncing = false; // NEW: Flag to prevent event loops

        public ManHinhDangNhap()
        {
            InitializeComponent();
            taiKhoanBUS = new TaiKhoanBUS();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessing) return;

            string username = txtUsername.Text?.Trim();
            // This line remains correct as txtPassword is always kept in sync
            string password = txtPassword.Password;

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

                NhanVien loggedInUser = await Task.Run(() => taiKhoanBUS.DangNhap(username, password));

                if (loggedInUser != null)
                {
                    this.Hide();
                    var welcome = new Cafebook.Views.Common.WelcomeWindow(loggedInUser, this, durationMs: 1400);
                    welcome.Owner = this;
                    welcome.ShowDialog();

                    if (this.IsVisible)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại.", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPassword.Clear();
                    txtVisiblePassword.Clear(); // NEW: Clear the visible password field as well
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
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát khỏi chương trình?",
                "Xác nhận Thoát",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Focus();
        }

        // ####################################################################
        // ## NEW: Methods to handle Show/Hide Password functionality        ##
        // ####################################################################

        private void ChkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            // Show the TextBox, hide the PasswordBox
            txtVisiblePassword.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Collapsed;
        }

        private void ChkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            // Hide the TextBox, show the PasswordBox
            txtVisiblePassword.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Sync password from PasswordBox to TextBox to ensure they match
            if (!_isPasswordSyncing)
            {
                _isPasswordSyncing = true;
                txtVisiblePassword.Text = txtPassword.Password;
                _isPasswordSyncing = false;
            }
        }

        private void TxtVisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Sync password from TextBox to PasswordBox to ensure they match
            if (!_isPasswordSyncing)
            {
                _isPasswordSyncing = true;
                txtPassword.Password = txtVisiblePassword.Text;
                _isPasswordSyncing = false;
            }
        }
    }
}