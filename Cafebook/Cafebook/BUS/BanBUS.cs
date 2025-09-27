using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class BanBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public List<Ban> GetDanhSachBan()
        {
            var dsBan = new List<Ban>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT 
                        b.idBan, b.soBan, b.soGhe, b.ghiChu,
                        CASE 
                            WHEN hd.idHoaDon IS NOT NULL THEN N'Có khách'
                            WHEN b.trangThai != N'Trống' THEN b.trangThai
                            ELSE N'Trống' 
                        END AS TrangThai,
                        hd.idHoaDon,
                        ISNULL(hd.thanhTien, 0) AS TongTien
                    FROM Ban b
                    LEFT JOIN HoaDon hd ON b.idBan = hd.idBan AND hd.trangThai = N'Chưa thanh toán'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsBan.Add(new Ban
                        {
                            IdBan = (int)reader["idBan"],
                            SoBan = (string)reader["soBan"],
                            SoGhe = (int)reader["soGhe"],
                            GhiChu = reader.IsDBNull(reader.GetOrdinal("ghiChu")) ? "" : (string)reader["ghiChu"],
                            TrangThai = (string)reader["TrangThai"],
                            IdHoaDonHienTai = reader.IsDBNull(reader.GetOrdinal("idHoaDon")) ? (int?)null : (int)reader["idHoaDon"],
                            TongTienHienTai = (decimal)reader["TongTien"]
                        });
                    }
                }
            }
            return dsBan;
        }

        public bool ChuyenBan(int idHoaDon, int idBanMoi)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE HoaDon SET idBan = @idBanMoi WHERE idHoaDon = @idHD", conn);
                cmd.Parameters.AddWithValue("@idBanMoi", idBanMoi);
                cmd.Parameters.AddWithValue("@idHD", idHoaDon);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // HÀM MỚI: XỬ LÝ GỘP BÀN
        public bool GopBan(int idHoaDonNguon, int idHoaDonDich)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Chuyển tất cả chi tiết từ hóa đơn nguồn sang hóa đơn đích
                    var cmdChuyenChiTiet = new SqlCommand("UPDATE ChiTietHoaDon SET idHoaDon = @idHDDich WHERE idHoaDon = @idHDNguon", conn, transaction);
                    cmdChuyenChiTiet.Parameters.AddWithValue("@idHDDich", idHoaDonDich);
                    cmdChuyenChiTiet.Parameters.AddWithValue("@idHDNguon", idHoaDonNguon);
                    cmdChuyenChiTiet.ExecuteNonQuery();

                    // 2. Xóa hóa đơn nguồn (nay đã trống)
                    var cmdXoaHDNguon = new SqlCommand("DELETE FROM HoaDon WHERE idHoaDon = @idHDNguon", conn, transaction);
                    cmdXoaHDNguon.Parameters.AddWithValue("@idHDNguon", idHoaDonNguon);
                    cmdXoaHDNguon.ExecuteNonQuery();

                    // 3. Tính toán và cập nhật lại tổng tiền cho hóa đơn đích
                    var cmdTinhTong = new SqlCommand("SELECT SUM(soLuong * donGiaLucBan) FROM ChiTietHoaDon WHERE idHoaDon = @idHDDich", conn, transaction);
                    cmdTinhTong.Parameters.AddWithValue("@idHDDich", idHoaDonDich);
                    decimal tongTienMoi = (decimal)cmdTinhTong.ExecuteScalar();

                    // (Giả sử chưa tính khuyến mãi ở bước này)
                    var cmdCapNhatHD = new SqlCommand("UPDATE HoaDon SET tongTien = @tong, thanhTien = @tong WHERE idHoaDon = @idHDDich", conn, transaction);
                    cmdCapNhatHD.Parameters.AddWithValue("@tong", tongTienMoi);
                    cmdCapNhatHD.Parameters.AddWithValue("@idHDDich", idHoaDonDich);
                    cmdCapNhatHD.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}