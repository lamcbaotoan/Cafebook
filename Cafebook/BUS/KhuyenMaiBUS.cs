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
                var cmd = new SqlCommand(@"
                    SELECT km.*, sp.tenSanPham 
                    FROM KhuyenMai km
                    LEFT JOIN SanPham sp ON km.idSanPhamApDung = sp.idSanPham
                    ORDER BY km.ngayBatDau DESC", conn);
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
                            NgayKetThuc = (DateTime)reader["ngayKetThuc"],
                            GiaTriDonHangToiThieu = reader.IsDBNull(reader.GetOrdinal("giaTriDonHangToiThieu")) ? (decimal?)null : (decimal)reader["giaTriDonHangToiThieu"],
                            IdSanPhamApDung = reader.IsDBNull(reader.GetOrdinal("idSanPhamApDung")) ? (int?)null : (int)reader["idSanPhamApDung"],
                            TenSanPhamApDung = reader.IsDBNull(reader.GetOrdinal("tenSanPham")) ? "Không yêu cầu" : (string)reader["tenSanPham"]
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
                var cmd = new SqlCommand(@"INSERT INTO KhuyenMai 
                    (tenKhuyenMai, moTa, loaiGiamGia, giaTriGiam, ngayBatDau, ngayKetThuc, giaTriDonHangToiThieu, idSanPhamApDung) 
                    VALUES (@ten, @moTa, @loai, @giaTri, @ngayBD, @ngayKT, @gtToiThieu, @idSP)", conn);
                cmd.Parameters.AddWithValue("@ten", km.TenKhuyenMai);
                cmd.Parameters.AddWithValue("@moTa", km.MoTa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@loai", km.LoaiGiamGia);
                cmd.Parameters.AddWithValue("@giaTri", km.GiaTriGiam);
                cmd.Parameters.AddWithValue("@ngayBD", km.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngayKT", km.NgayKetThuc);
                cmd.Parameters.AddWithValue("@gtToiThieu", km.GiaTriDonHangToiThieu ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idSP", km.IdSanPhamApDung ?? (object)DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaKhuyenMai(KhuyenMai km)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE KhuyenMai SET 
                    tenKhuyenMai = @ten, moTa = @moTa, loaiGiamGia = @loai, giaTriGiam = @giaTri, ngayBatDau = @ngayBD, ngayKetThuc = @ngayKT, 
                    giaTriDonHangToiThieu = @gtToiThieu, idSanPhamApDung = @idSP 
                    WHERE idKhuyenMai = @id", conn);
                cmd.Parameters.AddWithValue("@ten", km.TenKhuyenMai);
                cmd.Parameters.AddWithValue("@moTa", km.MoTa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@loai", km.LoaiGiamGia);
                cmd.Parameters.AddWithValue("@giaTri", km.GiaTriGiam);
                cmd.Parameters.AddWithValue("@ngayBD", km.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngayKT", km.NgayKetThuc);
                cmd.Parameters.AddWithValue("@gtToiThieu", km.GiaTriDonHangToiThieu ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idSP", km.IdSanPhamApDung ?? (object)DBNull.Value);
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