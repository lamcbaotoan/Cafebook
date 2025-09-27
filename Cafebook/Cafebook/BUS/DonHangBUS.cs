using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Cafebook.BUS
{
    public class DonHangBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        public List<HoaDon> GetDanhSachDonHang(DateTime tuNgay, DateTime denNgay, string trangThai)
        {
            var ds = new List<HoaDon>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var queryBuilder = new StringBuilder(@"
                    SELECT hd.idHoaDon, hd.thoiGianTao, hd.thanhTien, hd.trangThai, b.soBan, nv.hoTen
                    FROM HoaDon hd
                    JOIN Ban b ON hd.idBan = b.idBan
                    JOIN NhanVien nv ON hd.idNhanVien = nv.idNhanVien
                    WHERE hd.thoiGianTao BETWEEN @tuNgay AND @denNgay");

                if (trangThai != "Tất cả")
                {
                    queryBuilder.Append(" AND hd.trangThai = @trangThai");
                }
                queryBuilder.Append(" ORDER BY hd.thoiGianTao DESC");

                var cmd = new SqlCommand(queryBuilder.ToString(), conn);
                cmd.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@denNgay", denNgay.Date.AddDays(1).AddTicks(-1)); // Đến cuối ngày
                if (trangThai != "Tất cả")
                {
                    cmd.Parameters.AddWithValue("@trangThai", trangThai);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new HoaDon
                        {
                            IdHoaDon = reader.GetInt32(0),
                            ThoiGianTao = reader.GetDateTime(1),
                            ThanhTien = reader.GetDecimal(2),
                            TrangThai = reader.GetString(3),
                            SoBan = reader.GetString(4),
                            TenNhanVien = reader.GetString(5)
                        });
                    }
                }
            }
            return ds;
        }

        public bool HuyDonHang(int idHoaDon)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    // Chỉ cho phép hủy hóa đơn chưa thanh toán
                    var checkCmd = new SqlCommand("SELECT trangThai FROM HoaDon WHERE idHoaDon = @idHD", conn, transaction);
                    checkCmd.Parameters.AddWithValue("@idHD", idHoaDon);
                    string trangThai = checkCmd.ExecuteScalar() as string;
                    if (trangThai != "Chưa thanh toán")
                    {
                        return false; // Không thể hủy hóa đơn đã thanh toán
                    }

                    // Xóa chi tiết hóa đơn trước
                    var cmdChiTiet = new SqlCommand("DELETE FROM ChiTietHoaDon WHERE idHoaDon = @idHD", conn, transaction);
                    cmdChiTiet.Parameters.AddWithValue("@idHD", idHoaDon);
                    cmdChiTiet.ExecuteNonQuery();

                    // Xóa hóa đơn
                    var cmdHoaDon = new SqlCommand("DELETE FROM HoaDon WHERE idHoaDon = @idHD", conn, transaction);
                    cmdHoaDon.Parameters.AddWithValue("@idHD", idHoaDon);
                    cmdHoaDon.ExecuteNonQuery();

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
        // THÊM PHƯƠNG THỨC MỚI NÀY VÀO LỚP DonHangBUS
        public HoaDon GetHoaDonDayDuById(int idHoaDon)
        {
            HoaDon hoaDon = null;
            // Dòng này của bạn bị lặp lại, chỉ cần một dòng là đủ
            // string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // SỬA LẠI: Liệt kê rõ ràng các cột để tránh lỗi và dễ quản lý
                var cmd = new SqlCommand(@"
                    SELECT 
                        hd.idHoaDon, hd.idKhachHang, hd.idNhanVien, hd.idBan, hd.idKhuyenMai,
                        hd.thoiGianTao, hd.thoiGianThanhToan, 
                        hd.tongTien, hd.soTienGiam, hd.thanhTien, 
                        hd.trangThai, hd.phuongThucThanhToan,
                        b.soBan, 
                        nv.hoTen AS TenNhanVien
                    FROM HoaDon hd
                    JOIN Ban b ON hd.idBan = b.idBan
                    JOIN NhanVien nv ON hd.idNhanVien = nv.idNhanVien
                    WHERE hd.idHoaDon = @idHD", conn);
                cmd.Parameters.AddWithValue("@idHD", idHoaDon);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hoaDon = new HoaDon
                        {
                            IdHoaDon = (int)reader["idHoaDon"],
                            IdKhachHang = reader.IsDBNull(reader.GetOrdinal("idKhachHang")) ? (int?)null : (int)reader["idKhachHang"],
                            IdNhanVien = (int)reader["idNhanVien"],
                            IdBan = (int)reader["idBan"],
                            IdKhuyenMai = reader.IsDBNull(reader.GetOrdinal("idKhuyenMai")) ? (int?)null : (int)reader["idKhuyenMai"],
                            ThoiGianTao = (DateTime)reader["thoiGianTao"],
                            ThoiGianThanhToan = reader.IsDBNull(reader.GetOrdinal("thoiGianThanhToan")) ? (DateTime?)null : (DateTime)reader["thoiGianThanhToan"],
                            TongTien = (decimal)reader["tongTien"],
                            SoTienGiam = (decimal)reader["soTienGiam"],
                            ThanhTien = (decimal)reader["thanhTien"],
                            TrangThai = (string)reader["trangThai"],
                            PhuongThucThanhToan = reader.IsDBNull(reader.GetOrdinal("phuongThucThanhToan")) ? "" : (string)reader["phuongThucThanhToan"],
                            SoBan = (string)reader["soBan"],
                            TenNhanVien = (string)reader["TenNhanVien"]
                        };
                    }
                }
            }
            return hoaDon;
        }
    }
}