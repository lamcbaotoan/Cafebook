// BUS/ThanhToanBUS.cs
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

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

        public bool LuuKhuyenMaiChoHoaDon(int idHoaDon, int? idKhuyenMai)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE HoaDon SET idKhuyenMai = @idKM WHERE idHoaDon = @idHD", conn);
                cmd.Parameters.AddWithValue("@idKM", (object)idKhuyenMai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idHD", idHoaDon);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // Hàm này sẽ được gọi khi bấm nút "XÁC NHẬN THANH TOÁN"
        // Nó sẽ cập nhật trạng thái hóa đơn và trừ kho
        // Hàm này chỉ cập nhật trạng thái hóa đơn và bàn, không thay đổi tiền nong
        // Trong file: BUS/ThanhToanBUS.cs

        public bool ThucHienThanhToan(HoaDon hoaDon)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var cmdUpdateHD = new SqlCommand(@"UPDATE HoaDon SET 
                                                    idKhuyenMai = @idKM, 
                                                    tongTien = @tongTien, 
                                                    soTienGiam = @tienGiam, 
                                                    thanhTien = @thanhTien,
                                                    trangThai = N'Đã thanh toán'
                                                WHERE idHoaDon = @idHD", conn, tran);

                        // **SỬA LỖI Ở ĐÂY: Xử lý đúng giá trị cho idKhuyenMai**
                        // Nếu idKhuyenMai là 0 (tức là "Không áp dụng"), ta sẽ lưu NULL vào CSDL.
                        object idKhuyenMaiParam = DBNull.Value;
                        if (hoaDon.IdKhuyenMai.HasValue && hoaDon.IdKhuyenMai.Value != 0)
                        {
                            idKhuyenMaiParam = hoaDon.IdKhuyenMai.Value;
                        }
                        cmdUpdateHD.Parameters.AddWithValue("@idKM", idKhuyenMaiParam);

                        cmdUpdateHD.Parameters.AddWithValue("@tongTien", hoaDon.TongTien);
                        cmdUpdateHD.Parameters.AddWithValue("@tienGiam", hoaDon.SoTienGiam);
                        cmdUpdateHD.Parameters.AddWithValue("@thanhTien", hoaDon.ThanhTien);
                        cmdUpdateHD.Parameters.AddWithValue("@idHD", hoaDon.IdHoaDon);
                        cmdUpdateHD.ExecuteNonQuery();

                        // Cập nhật trạng thái bàn về 'Trống'
                        var cmdUpdateBan = new SqlCommand("UPDATE Ban SET trangThai = N'Trống' WHERE idBan = @idBan", conn, tran);
                        cmdUpdateBan.Parameters.AddWithValue("@idBan", hoaDon.IdBan);
                        cmdUpdateBan.ExecuteNonQuery();

                        tran.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        // **HÀM TÁCH HÓA ĐƠN ĐÃ ĐƯỢC THIẾT KẾ LẠI HOÀN TOÀN**
        // SỬA LẠI HOÀN TOÀN HÀM NÀY
        public bool ThucHienTachHoaDon(int idHoaDonGoc, HoaDon hoaDonThanhToan, List<ChiTietHoaDon> chiTietCanTach, int idNhanVien)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo hóa đơn MỚI cho các món ĐÃ TÁCH và thanh toán nó
                        var cmdInsertHD = new SqlCommand(@"INSERT INTO HoaDon (idBan, idNhanVien, thoiGianTao, thoiGianThanhToan, trangThai, idKhuyenMai, tongTien, soTienGiam, thanhTien)
                                                         OUTPUT INSERTED.idHoaDon
                                                         VALUES (@idBan, @idNV, @time, @timePay, @tt, @idKM, @tong, @giam, @thanhTien)", conn, tran);
                        cmdInsertHD.Parameters.AddWithValue("@idBan", hoaDonThanhToan.IdBan);
                        cmdInsertHD.Parameters.AddWithValue("@idNV", idNhanVien);
                        cmdInsertHD.Parameters.AddWithValue("@time", hoaDonThanhToan.ThoiGianTao);
                        cmdInsertHD.Parameters.AddWithValue("@timePay", hoaDonThanhToan.ThoiGianThanhToan);
                        cmdInsertHD.Parameters.AddWithValue("@tt", hoaDonThanhToan.TrangThai);
                        cmdInsertHD.Parameters.AddWithValue("@idKM", (object)hoaDonThanhToan.IdKhuyenMai ?? DBNull.Value);
                        cmdInsertHD.Parameters.AddWithValue("@tong", hoaDonThanhToan.TongTien);
                        cmdInsertHD.Parameters.AddWithValue("@giam", hoaDonThanhToan.SoTienGiam);
                        cmdInsertHD.Parameters.AddWithValue("@thanhTien", hoaDonThanhToan.ThanhTien);
                        int newHoaDonId = (int)cmdInsertHD.ExecuteScalar();

                        // Thêm chi tiết cho hóa đơn vừa tách
                        foreach (var item in chiTietCanTach)
                        {
                            var cmdInsertCT = new SqlCommand("INSERT INTO ChiTietHoaDon (idHoaDon, idSanPham, soLuong, donGiaLucBan) VALUES (@idHD, @idSP, @sl, @dg)", conn, tran);
                            cmdInsertCT.Parameters.AddWithValue("@idHD", newHoaDonId);
                            cmdInsertCT.Parameters.AddWithValue("@idSP", item.IdSanPham);
                            cmdInsertCT.Parameters.AddWithValue("@sl", item.SoLuong);
                            cmdInsertCT.Parameters.AddWithValue("@dg", item.DonGiaLucBan);
                            cmdInsertCT.ExecuteNonQuery();
                        }

                        // 2. Xóa các chi tiết đã tách khỏi hóa đơn gốc
                        foreach (var item in chiTietCanTach)
                        {
                            var cmdDeleteCT = new SqlCommand("DELETE FROM ChiTietHoaDon WHERE idHoaDon = @idHD AND idSanPham = @idSP", conn, tran);
                            cmdDeleteCT.Parameters.AddWithValue("@idHD", idHoaDonGoc);
                            cmdDeleteCT.Parameters.AddWithValue("@idSP", item.IdSanPham);
                            cmdDeleteCT.ExecuteNonQuery();
                        }

                        // 3. Cập nhật lại tổng tiền cho hóa đơn gốc
                        var cmdUpdateHDGoc = new SqlCommand(@"
                            UPDATE HoaDon 
                            SET 
                                tongTien = (SELECT ISNULL(SUM(soLuong * donGiaLucBan), 0) FROM ChiTietHoaDon WHERE idHoaDon = @idHD),
                                soTienGiam = 0,
                                thanhTien = (SELECT ISNULL(SUM(soLuong * donGiaLucBan), 0) FROM ChiTietHoaDon WHERE idHoaDon = @idHD),
                                idKhuyenMai = NULL
                            WHERE idHoaDon = @idHD", conn, tran);
                        cmdUpdateHDGoc.Parameters.AddWithValue("@idHD", idHoaDonGoc);
                        cmdUpdateHDGoc.ExecuteNonQuery();

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}