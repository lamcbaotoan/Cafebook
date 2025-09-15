// BUS/ThanhToanBUS.cs
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class ThanhToanBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        // Tái sử dụng lại hàm từ GoiMonBUS hoặc viết lại để lấy chi tiết hóa đơn
        public List<ChiTietHoaDon> GetChiTietHoaDon(int idHoaDon)
        {
            var ds = new List<ChiTietHoaDon>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"SELECT cthd.*, sp.tenSanPham FROM ChiTietHoaDon cthd 
                                           JOIN SanPham sp ON cthd.idSanPham = sp.idSanPham
                                           WHERE cthd.idHoaDon = @idHD", conn);
                cmd.Parameters.AddWithValue("@idHD", idHoaDon);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new ChiTietHoaDon
                        {
                            IdHoaDon = (int)reader["idHoaDon"],
                            IdSanPham = (int)reader["idSanPham"],
                            SoLuong = (int)reader["soLuong"],
                            DonGiaLucBan = (decimal)reader["donGiaLucBan"],
                            GhiChu = reader.IsDBNull(reader.GetOrdinal("GhiChu")) ? "" : (string)reader["GhiChu"],
                            TenSanPham = (string)reader["tenSanPham"]
                        });
                    }
                }
            }
            return ds;
        }

        // Hàm này sẽ được gọi khi bấm nút "XÁC NHẬN THANH TOÁN"
        // Nó sẽ cập nhật trạng thái hóa đơn và trừ kho
        public bool ThucHienThanhToan(int idHoaDon)
        {
            // Tái sử dụng hàm đã viết trong GoiMonBUS
            GoiMonBUS goiMonBUS = new GoiMonBUS();
            return goiMonBUS.ThanhToanHoaDon(idHoaDon);
        }
    }
}