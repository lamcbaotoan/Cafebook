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
        public bool ThucHienTachHoaDon(HoaDon hoaDonGoc, List<ChiTietHoaDon> chiTietCanTach, int idNhanVien)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo một hóa đơn MỚI cho các món ĐÃ TÁCH và thanh toán nó
                        var hoaDonDaTach = new HoaDon
                        {
                            IdBan = hoaDonGoc.IdBan,
                            IdNhanVien = idNhanVien,
                            ThoiGianTao = DateTime.Now,
                            TrangThai = "Đã thanh toán", // Hóa đơn này được thanh toán ngay
                            TongTien = hoaDonGoc.TongTien, // Thông tin tiền đã được cập nhật từ View
                            SoTienGiam = hoaDonGoc.SoTienGiam,
                            ThanhTien = hoaDonGoc.ThanhTien,
                            IdKhuyenMai = hoaDonGoc.IdKhuyenMai
                        };

                        var cmdInsertHD = new SqlCommand(@"INSERT INTO HoaDon (idBan, idNhanVien, thoiGianTao, trangThai, idKhuyenMai, tongTien, soTienGiam, thanhTien)
                                                         OUTPUT INSERTED.idHoaDon
                                                         VALUES (@idBan, @idNV, @time, @tt, @idKM, @tong, @giam, @thanhTien)", conn, tran);
                        cmdInsertHD.Parameters.AddWithValue("@idBan", hoaDonDaTach.IdBan);
                        cmdInsertHD.Parameters.AddWithValue("@idNV", hoaDonDaTach.IdNhanVien);
                        cmdInsertHD.Parameters.AddWithValue("@time", hoaDonDaTach.ThoiGianTao);
                        cmdInsertHD.Parameters.AddWithValue("@tt", hoaDonDaTach.TrangThai);
                        cmdInsertHD.Parameters.AddWithValue("@idKM", (object)hoaDonDaTach.IdKhuyenMai ?? DBNull.Value);
                        cmdInsertHD.Parameters.AddWithValue("@tong", hoaDonDaTach.TongTien);
                        cmdInsertHD.Parameters.AddWithValue("@giam", hoaDonDaTach.SoTienGiam);
                        cmdInsertHD.Parameters.AddWithValue("@thanhTien", hoaDonDaTach.ThanhTien);
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
                            cmdDeleteCT.Parameters.AddWithValue("@idHD", hoaDonGoc.IdHoaDon);
                            cmdDeleteCT.Parameters.AddWithValue("@idSP", item.IdSanPham);
                            cmdDeleteCT.ExecuteNonQuery();
                        }

                        // 3. Cập nhật lại tổng tiền cho hóa đơn gốc (giờ chỉ còn các món chưa thanh toán)
                        var chiTietConLai = GetChiTietHoaDon(hoaDonGoc.IdHoaDon);
                        decimal tongTienConLai = chiTietConLai.Sum(i => i.ThanhTien);

                        var cmdUpdateHDGoc = new SqlCommand("UPDATE HoaDon SET tongTien = @tong, soTienGiam=0, thanhTien=@thanhTien, idKhuyenMai=NULL WHERE idHoaDon = @idHD", conn, tran);
                        cmdUpdateHDGoc.Parameters.AddWithValue("@tong", tongTienConLai);
                        cmdUpdateHDGoc.Parameters.AddWithValue("@thanhTien", tongTienConLai);
                        cmdUpdateHDGoc.Parameters.AddWithValue("@idHD", hoaDonGoc.IdHoaDon);
                        cmdUpdateHDGoc.ExecuteNonQuery();

                        // Bàn vẫn ở trạng thái "Đang phục vụ" vì còn hóa đơn gốc
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