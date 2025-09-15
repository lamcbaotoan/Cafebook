// BUS/KhuyenMaiBUS.cs
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class KhuyenMaiBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public List<KhuyenMai> GetDanhSachKhuyenMai()
        {
            var ds = new List<KhuyenMai>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM KhuyenMai ORDER BY ngayBatDau DESC", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new KhuyenMai
                        {
                            IdKhuyenMai = (int)reader["idKhuyenMai"],
                            TenKhuyenMai = (string)reader["tenKhuyenMai"],
                            MoTa = reader.IsDBNull(reader.GetOrdinal("moTa")) ? "" : (string)reader["moTa"],
                            LoaiGiamGia = (string)reader["loaiGiamGia"],
                            GiaTriGiam = (decimal)reader["giaTriGiam"],
                            NgayBatDau = (DateTime)reader["ngayBatDau"],
                            NgayKetThuc = (DateTime)reader["ngayKetThuc"]
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemKhuyenMai(KhuyenMai km)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO KhuyenMai (tenKhuyenMai, moTa, loaiGiamGia, giaTriGiam, ngayBatDau, ngayKetThuc) VALUES (@ten, @moTa, @loai, @giaTri, @ngayBD, @ngayKT)", conn);
                cmd.Parameters.AddWithValue("@ten", km.TenKhuyenMai);
                cmd.Parameters.AddWithValue("@moTa", km.MoTa);
                cmd.Parameters.AddWithValue("@loai", km.LoaiGiamGia);
                cmd.Parameters.AddWithValue("@giaTri", km.GiaTriGiam);
                cmd.Parameters.AddWithValue("@ngayBD", km.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngayKT", km.NgayKetThuc);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaKhuyenMai(KhuyenMai km)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE KhuyenMai SET tenKhuyenMai = @ten, moTa = @moTa, loaiGiamGia = @loai, giaTriGiam = @giaTri, ngayBatDau = @ngayBD, ngayKetThuc = @ngayKT WHERE idKhuyenMai = @id", conn);
                cmd.Parameters.AddWithValue("@ten", km.TenKhuyenMai);
                cmd.Parameters.AddWithValue("@moTa", km.MoTa);
                cmd.Parameters.AddWithValue("@loai", km.LoaiGiamGia);
                cmd.Parameters.AddWithValue("@giaTri", km.GiaTriGiam);
                cmd.Parameters.AddWithValue("@ngayBD", km.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngayKT", km.NgayKetThuc);
                cmd.Parameters.AddWithValue("@id", km.IdKhuyenMai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaKhuyenMai(int idKhuyenMai)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM KhuyenMai WHERE idKhuyenMai = @id", conn);
                cmd.Parameters.AddWithValue("@id", idKhuyenMai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}