using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class ThongBaoBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public bool GuiThongBao(ThongBao thongBao)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO ThongBao (idNhanVien, noiDung) VALUES (@idNV, @noiDung)", conn);
                cmd.Parameters.AddWithValue("@idNV", thongBao.IdNhanVien);
                cmd.Parameters.AddWithValue("@noiDung", thongBao.NoiDung);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<ThongBao> GetThongBaoChuaDoc()
        {
            var ds = new List<ThongBao>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT tb.idThongBao, tb.noiDung, tb.thoiGianTao, nv.hoTen
                    FROM ThongBao tb
                    JOIN NhanVien nv ON tb.idNhanVien = nv.idNhanVien
                    WHERE tb.daDoc = 0
                    ORDER BY tb.thoiGianTao DESC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new ThongBao
                        {
                            IdThongBao = reader.GetInt32(0),
                            NoiDung = reader.GetString(1),
                            ThoiGianTao = reader.GetDateTime(2),
                            TenNhanVien = reader.GetString(3)
                        });
                    }
                }
            }
            return ds;
        }

        public List<ThongBao> GetThongBaoBanChuaDoc()
        {
            var ds = new List<ThongBao>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT tb.idThongBao, tb.noiDung, tb.thoiGianTao, nv.hoTen
                    FROM ThongBao tb
                    JOIN NhanVien nv ON tb.idNhanVien = nv.idNhanVien
                    -- SỬA LẠI CÂU LỆNH LIKE DƯỚI ĐÂY --
                    WHERE tb.daDoc = 0 AND tb.noiDung LIKE N'[[]Bàn %]%' 
                    ORDER BY tb.thoiGianTao DESC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new ThongBao
                        {
                            IdThongBao = reader.GetInt32(0),
                            NoiDung = reader.GetString(1),
                            ThoiGianTao = reader.GetDateTime(2),
                            TenNhanVien = reader.GetString(3)
                        });
                    }
                }
            }
            return ds;
        }


        public bool DanhDauDaDoc(int idThongBao)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE ThongBao SET daDoc = 1 WHERE idThongBao = @id", conn);
                cmd.Parameters.AddWithValue("@id", idThongBao);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // HÀM MỚI CẦN THÊM VÀO ĐÂY
        public bool DanhDauTatCaDaDoc()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Câu lệnh cập nhật tất cả các thông báo chưa đọc
                var cmd = new SqlCommand("UPDATE ThongBao SET daDoc = 1 WHERE daDoc = 0", conn);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}