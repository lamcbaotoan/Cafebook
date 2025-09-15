using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// BUS/NhanVienBUS.cs
using Cafebook.DTO;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class NhanVienBUS
    {
        private string connectionString;

        public NhanVienBUS()
        {
            // Lấy chuỗi kết nối từ file App.config
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }

        public NhanVien KiemTraDangNhap(string username, string password)
        {
            NhanVien nhanVien = null;

            // Sử dụng using để đảm bảo kết nối được đóng ngay cả khi có lỗi
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // **QUAN TRỌNG**: Dùng tham số hóa để chống lỗi SQL Injection
                string query = "SELECT idNhanVien, idVaiTro, hoTen, soDienThoai, email, matKhau, trangThai " +
                               "FROM NhanVien " +
                               "WHERE (soDienThoai = @username OR email = @username) AND matKhau = @password AND trangThai = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password); // Lưu ý: Trong thực tế, mật khẩu nên được mã hóa (hashed)

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Nếu tìm thấy một dòng dữ liệu
                    {
                        nhanVien = new NhanVien
                        {
                            IdNhanVien = reader.GetInt32(0),
                            IdVaiTro = reader.GetInt32(1),
                            HoTen = reader.GetString(2),
                            SoDienThoai = reader.GetString(3),
                            Email = reader.GetString(4),
                            MatKhau = reader.GetString(5),
                            TrangThai = reader.GetBoolean(6)
                        };
                    }
                }
            }
            return nhanVien; // Trả về null nếu không tìm thấy, hoặc đối tượng NhanVien nếu thành công
        }
    }
}