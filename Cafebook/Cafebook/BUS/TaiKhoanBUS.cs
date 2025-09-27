using Cafebook.DTO;
using System.Configuration;
using System.Data.SqlClient;
using System; // Thêm using này

namespace Cafebook.BUS
{
    public class TaiKhoanBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        /// <summary>
        /// Xác thực thông tin đăng nhập và trả về đối tượng NhanVien đầy đủ thông tin.
        /// </summary>
        /// <param name="username">Có thể là Email hoặc Số điện thoại</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Đối tượng NhanVien nếu đăng nhập thành công, ngược lại trả về null.</returns>
        public NhanVien DangNhap(string username, string password)
        {
            NhanVien user = null;

            // Câu lệnh SQL QUAN TRỌNG: JOIN với bảng VaiTro để lấy tenVaiTro
            string query = @"SELECT nv.*, vt.tenVaiTro 
                             FROM NhanVien nv 
                             JOIN VaiTro vt ON nv.idVaiTro = vt.idVaiTro 
                             WHERE (nv.email = @username OR nv.soDienThoai = @username) 
                               AND nv.matKhau = @password 
                               AND nv.trangThai = 1"; // Chỉ cho phép tài khoản đang hoạt động đăng nhập

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password); // Mật khẩu chưa mã hóa
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Nếu tìm thấy một dòng khớp
                    {
                        user = new NhanVien
                        {
                            IdNhanVien = (int)reader["idNhanVien"],
                            IdVaiTro = (int)reader["idVaiTro"],
                            HoTen = (string)reader["hoTen"],
                            SoDienThoai = reader["soDienThoai"] as string,
                            Email = reader["email"] as string,
                            DiaChi = reader["diaChi"] as string,
                            MatKhau = (string)reader["matKhau"],
                            NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                            TrangThai = (bool)reader["trangThai"],
                            MucLuongTheoGio = (decimal)reader["mucLuongTheoGio"],
                            TenVaiTro = (string)reader["tenVaiTro"] // Đây là dữ liệu chúng ta cần
                        };
                    }
                }
            }
            return user;
        }
    }
}