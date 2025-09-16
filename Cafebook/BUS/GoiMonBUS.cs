using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Cafebook.BUS
{
    public class GoiMonBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        // Trong BUS/GoiMonBUS.cs

        // SỬA LẠI HÀM NÀY
        public List<SanPham> GetSanPhamTheoLoai(int idLoaiSP)
        {
            var ds = new List<SanPham>();
            var sanPhamBUS = new SanPhamBUS();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM SanPham WHERE idLoaiSP = @idLoaiSP AND trangThai = N'Đang bán'", conn);
                cmd.Parameters.AddWithValue("@idLoaiSP", idLoaiSP);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sp = new SanPham
                        {
                            IdSanPham = (int)reader["idSanPham"],
                            TenSanPham = (string)reader["tenSanPham"],
                            DonGia = (decimal)reader["donGia"]
                        };

                        sp.SoLuongCoThePhucVu = sanPhamBUS.KiemTraKhaNangPhucVu(sp.IdSanPham);
                        ds.Add(sp);
                    }
                }
            }
            return ds;
        }

        public HoaDon GetHoaDonChuaThanhToan(int idBan)
        {
            HoaDon hd = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HoaDon WHERE idBan = @idBan AND trangThai = N'Chưa thanh toán'", conn);
                cmd.Parameters.AddWithValue("@idBan", idBan);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hd = new HoaDon
                        {
                            IdHoaDon = (int)reader["idHoaDon"],
                            IdBan = (int)reader["idBan"],
                            IdNhanVien = (int)reader["idNhanVien"],
                            IdKhuyenMai = reader.IsDBNull(reader.GetOrdinal("idKhuyenMai")) ? (int?)null : (int)reader["idKhuyenMai"],
                            TongTien = (decimal)reader["tongTien"],
                            SoTienGiam = (decimal)reader["soTienGiam"],
                            ThanhTien = (decimal)reader["thanhTien"],
                            TrangThai = (string)reader["trangThai"]
                        };
                    }
                }
            }
            return hd;
        }

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

        // HÀM BỊ THIẾU ĐÃ ĐƯỢC BỔ SUNG
        public List<KhuyenMai> GetKhuyenMaiCoTheApDung(decimal tongTien, List<int> idSanPhamTrongHoaDon)
        {
            var ds = new List<KhuyenMai>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM KhuyenMai WHERE GETDATE() BETWEEN ngayBatDau AND ngayKetThuc", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var km = new KhuyenMai
                        {
                            IdKhuyenMai = (int)reader["idKhuyenMai"],
                            TenKhuyenMai = (string)reader["tenKhuyenMai"],
                            LoaiGiamGia = (string)reader["loaiGiamGia"],
                            GiaTriGiam = (decimal)reader["giaTriGiam"],
                            GiaTriDonHangToiThieu = reader.IsDBNull(reader.GetOrdinal("giaTriDonHangToiThieu")) ? (decimal?)null : (decimal)reader["giaTriDonHangToiThieu"],
                            IdSanPhamApDung = reader.IsDBNull(reader.GetOrdinal("idSanPhamApDung")) ? (int?)null : (int)reader["idSanPhamApDung"]
                        };

                        bool dieuKienOk = true;
                        if (km.GiaTriDonHangToiThieu.HasValue && tongTien < km.GiaTriDonHangToiThieu.Value)
                        {
                            dieuKienOk = false;
                        }
                        if (dieuKienOk && km.IdSanPhamApDung.HasValue && !idSanPhamTrongHoaDon.Contains(km.IdSanPhamApDung.Value))
                        {
                            dieuKienOk = false;
                        }

                        if (dieuKienOk)
                        {
                            ds.Add(km);
                        }
                    }
                }
            }
            return ds;
        }

        public HoaDon LuuHoaDon(HoaDon hoaDon, List<ChiTietHoaDon> chiTiet)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    if (hoaDon.IdHoaDon == 0) // Hóa đơn mới
                    {
                        var cmdHD = new SqlCommand("INSERT INTO HoaDon (idNhanVien, idBan, idKhuyenMai, thoiGianTao, tongTien, soTienGiam, thanhTien, trangThai) OUTPUT INSERTED.idHoaDon VALUES (@idNV, @idBan, @idKM, GETDATE(), @tongTien, @giam, @thanhTien, N'Chưa thanh toán')", conn, transaction);
                        cmdHD.Parameters.AddWithValue("@idNV", hoaDon.IdNhanVien);
                        cmdHD.Parameters.AddWithValue("@idBan", hoaDon.IdBan);
                        cmdHD.Parameters.AddWithValue("@idKM", hoaDon.IdKhuyenMai ?? (object)DBNull.Value);
                        cmdHD.Parameters.AddWithValue("@tongTien", hoaDon.TongTien);
                        cmdHD.Parameters.AddWithValue("@giam", hoaDon.SoTienGiam);
                        cmdHD.Parameters.AddWithValue("@thanhTien", hoaDon.ThanhTien);
                        hoaDon.IdHoaDon = (int)cmdHD.ExecuteScalar();
                    }
                    else // Cập nhật hóa đơn cũ
                    {
                        var cmdHD = new SqlCommand("UPDATE HoaDon SET idKhuyenMai = @idKM, tongTien = @tongTien, soTienGiam = @giam, thanhTien = @thanhTien WHERE idHoaDon = @idHD", conn, transaction);
                        cmdHD.Parameters.AddWithValue("@idKM", hoaDon.IdKhuyenMai ?? (object)DBNull.Value);
                        cmdHD.Parameters.AddWithValue("@tongTien", hoaDon.TongTien);
                        cmdHD.Parameters.AddWithValue("@giam", hoaDon.SoTienGiam);
                        cmdHD.Parameters.AddWithValue("@thanhTien", hoaDon.ThanhTien);
                        cmdHD.Parameters.AddWithValue("@idHD", hoaDon.IdHoaDon);
                        cmdHD.ExecuteNonQuery();
                    }

                    var cmdXoaCT = new SqlCommand("DELETE FROM ChiTietHoaDon WHERE idHoaDon = @idHD", conn, transaction);
                    cmdXoaCT.Parameters.AddWithValue("@idHD", hoaDon.IdHoaDon);
                    cmdXoaCT.ExecuteNonQuery();

                    foreach (var item in chiTiet)
                    {
                        var cmdThemCT = new SqlCommand("INSERT INTO ChiTietHoaDon (idHoaDon, idSanPham, soLuong, donGiaLucBan, GhiChu) VALUES (@idHD, @idSP, @soLuong, @donGia, @ghiChu)", conn, transaction);
                        cmdThemCT.Parameters.AddWithValue("@idHD", hoaDon.IdHoaDon);
                        cmdThemCT.Parameters.AddWithValue("@idSP", item.IdSanPham);
                        cmdThemCT.Parameters.AddWithValue("@soLuong", item.SoLuong);
                        cmdThemCT.Parameters.AddWithValue("@donGia", item.DonGiaLucBan);
                        cmdThemCT.Parameters.AddWithValue("@ghiChu", item.GhiChu ?? (object)DBNull.Value);
                        cmdThemCT.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return hoaDon;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public bool ThanhToanHoaDon(int idHoaDon)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    var chiTietList = new List<ChiTietHoaDon>();
                    var cmdGetChiTiet = new SqlCommand("SELECT idSanPham, soLuong FROM ChiTietHoaDon WHERE idHoaDon = @idHD", conn, transaction);
                    cmdGetChiTiet.Parameters.AddWithValue("@idHD", idHoaDon);
                    using (var reader = cmdGetChiTiet.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chiTietList.Add(new ChiTietHoaDon
                            {
                                IdSanPham = reader.GetInt32(0),
                                SoLuong = reader.GetInt32(1)
                            });
                        }
                    }

                    foreach (var item in chiTietList)
                    {
                        var cmdTruKho = new SqlCommand(@"
                            UPDATE NguyenLieu
                            SET soLuongTon = soLuongTon - (ct.luongCanThiet * @soLuongSP)
                            FROM NguyenLieu nl
                            JOIN CongThuc ct ON nl.idNguyenLieu = ct.idNguyenLieu
                            WHERE ct.idSanPham = @idSP", conn, transaction);

                        cmdTruKho.Parameters.AddWithValue("@soLuongSP", item.SoLuong);
                        cmdTruKho.Parameters.AddWithValue("@idSP", item.IdSanPham);
                        cmdTruKho.ExecuteNonQuery();
                    }

                    var cmdUpdateHD = new SqlCommand("UPDATE HoaDon SET trangThai = N'Đã thanh toán' WHERE idHoaDon = @idHD", conn, transaction);
                    cmdUpdateHD.Parameters.AddWithValue("@idHD", idHoaDon);
                    cmdUpdateHD.ExecuteNonQuery();

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