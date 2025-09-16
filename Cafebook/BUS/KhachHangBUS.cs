using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class KhachHangBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        /// <summary>
        /// Lấy toàn bộ danh sách khách hàng từ CSDL.
        /// </summary>
        public List<KhachHang> GetDanhSachKhachHang()
        {
            var ds = new List<KhachHang>();
            string query = "SELECT idKhachHang, hoTen, soDienThoai, ngayTao FROM KhachHang ORDER BY hoTen";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new KhachHang
                        {
                            IdKhachHang = (int)reader["idKhachHang"],
                            HoTen = reader["hoTen"].ToString(),
                            SoDienThoai = reader["soDienThoai"] as string,
                            NgayTao = (DateTime)reader["ngayTao"]
                        });
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Thêm khách hàng mới và trả về ID của khách hàng đó.
        /// Cần thiết cho chức năng cho thuê sách của nhân viên.
        /// </summary>
        /// <returns>ID của khách hàng mới, hoặc -1 nếu thất bại.</returns>
        public int AddKhachHang(KhachHang kh)
        {
            string query = "INSERT INTO KhachHang (hoTen, soDienThoai, ngayTao) VALUES (@hoTen, @sdt, GETDATE()); SELECT SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@hoTen", kh.HoTen);
                cmd.Parameters.AddWithValue("@sdt", (object)kh.SoDienThoai ?? DBNull.Value);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            return -1;
        }

        /// <summary>
        /// Thêm khách hàng mới (dành cho Admin, không cần trả về ID).
        /// </summary>
        public bool ThemKhachHang(KhachHang kh)
        {
            string query = "INSERT INTO KhachHang (hoTen, soDienThoai) VALUES (@hoTen, @sdt)";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@hoTen", kh.HoTen);
                cmd.Parameters.AddWithValue("@sdt", (object)kh.SoDienThoai ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Sửa thông tin khách hàng.
        /// </summary>
        public bool SuaKhachHang(KhachHang kh)
        {
            string query = "UPDATE KhachHang SET hoTen = @hoTen, soDienThoai = @sdt WHERE idKhachHang = @idKH";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@hoTen", kh.HoTen);
                cmd.Parameters.AddWithValue("@sdt", (object)kh.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idKH", kh.IdKhachHang);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<HoaDon> GetLichSuDonHang(int idKhachHang)
        {
            var ds = new List<HoaDon>();
            string query = @"SELECT idHoaDon, thoiGianTao, thanhTien, trangThai 
                             FROM HoaDon WHERE idKhachHang = @idKH ORDER BY thoiGianTao DESC";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@idKH", idKhachHang);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new HoaDon
                        {
                            IdHoaDon = (int)reader["idHoaDon"],
                            ThoiGianTao = (DateTime)reader["thoiGianTao"],
                            ThanhTien = (decimal)reader["thanhTien"],
                            TrangThai = reader["trangThai"].ToString()
                        });
                    }
                }
            }
            return ds;
        }

        public List<PhieuThueSach> GetLichSuThueSach(int idKhachHang)
        {
            var ds = new List<PhieuThueSach>();
            string query = @"SELECT pt.idPhieuThue, s.tieuDe, pt.ngayThue, pt.trangThai
                             FROM PhieuThueSach pt JOIN Sach s ON pt.idSach = s.idSach
                             WHERE pt.idKhachHang = @idKH ORDER BY pt.ngayThue DESC";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@idKH", idKhachHang);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new PhieuThueSach
                        {
                            IdPhieuThue = (int)reader["idPhieuThue"],
                            TieuDeSach = reader["tieuDe"].ToString(),
                            NgayThue = (DateTime)reader["ngayThue"],
                            TrangThai = reader["trangThai"].ToString()
                        });
                    }
                }
            }
            return ds;
        }
    }
}