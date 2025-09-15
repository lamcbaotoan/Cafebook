using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class QuanLyBanBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public List<Ban> GetDanhSachBan()
        {
            var ds = new List<Ban>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT idBan, soBan, soGhe, trangThai, ghiChu FROM Ban", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new Ban
                        {
                            IdBan = reader.GetInt32(0),
                            SoBan = reader.GetString(1),
                            SoGhe = reader.GetInt32(2),
                            TrangThai = reader.GetString(3),
                            GhiChu = reader.IsDBNull(4) ? "" : reader.GetString(4) // Thêm dòng này
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemBan(Ban ban)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Ban (soBan, soGhe, trangThai, ghiChu) VALUES (@soBan, @soGhe, @trangThai, @ghiChu)", conn);
                cmd.Parameters.AddWithValue("@soBan", ban.SoBan);
                cmd.Parameters.AddWithValue("@soGhe", ban.SoGhe);
                cmd.Parameters.AddWithValue("@trangThai", "Trống"); // Bàn mới luôn trống
                cmd.Parameters.AddWithValue("@ghiChu", ban.GhiChu ?? (object)DBNull.Value); // Thêm dòng này
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaBan(Ban ban)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Ban SET soBan = @soBan, soGhe = @soGhe, trangThai = @trangThai, ghiChu = @ghiChu WHERE idBan = @idBan", conn);
                cmd.Parameters.AddWithValue("@soBan", ban.SoBan);
                cmd.Parameters.AddWithValue("@soGhe", ban.SoGhe);
                cmd.Parameters.AddWithValue("@trangThai", ban.TrangThai);
                cmd.Parameters.AddWithValue("@idBan", ban.IdBan);
                cmd.Parameters.AddWithValue("@ghiChu", ban.GhiChu ?? (object)DBNull.Value); // Thêm dòng này
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaBan(int idBan)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Kiểm tra xem bàn có đang được sử dụng trong hóa đơn chưa thanh toán không
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM HoaDon WHERE idBan = @idBan AND trangThai = N'Chưa thanh toán'", conn);
                checkCmd.Parameters.AddWithValue("@idBan", idBan);
                if ((int)checkCmd.ExecuteScalar() > 0)
                {
                    return false; // Không cho xóa nếu đang có khách
                }

                // Nếu không, tiến hành xóa
                var deleteCmd = new SqlCommand("DELETE FROM Ban WHERE idBan = @idBan", conn);
                deleteCmd.Parameters.AddWithValue("@idBan", idBan);
                return deleteCmd.ExecuteNonQuery() > 0;
            }
        }
    }
}