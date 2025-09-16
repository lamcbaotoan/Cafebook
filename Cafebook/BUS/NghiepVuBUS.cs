// BUS/NghiepVuBUS.cs
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class NghiepVuBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region Đặt Bàn
        public List<PhieuDatBan> GetPhieuDatBan(DateTime ngay)
        {
            var ds = new List<PhieuDatBan>();
            // Câu lệnh SQL lấy danh sách phiếu đặt bàn trong ngày, JOIN với KhachHang và Ban để lấy tên hiển thị
            string query = @"SELECT 
                                p.idPhieuDatBan, p.idKhachHang, p.tenKhachVangLai, p.sdtKhachVangLai,
                                p.idBan, p.thoiGianDat, p.soLuongKhach, p.ghiChu, p.trangThai,
                                b.SoBan,
                                k.hoTen, k.soDienThoai
                             FROM PhieuDatBan p
                             LEFT JOIN Ban b ON p.idBan = b.idBan
                             LEFT JOIN KhachHang k ON p.idKhachHang = k.idKhachHang
                             WHERE CAST(p.thoiGianDat AS DATE) = @ngay
                             ORDER BY p.thoiGianDat ASC";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ngay", ngay.Date);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new PhieuDatBan
                        {
                            IdPhieuDatBan = reader.GetInt32(reader.GetOrdinal("idPhieuDatBan")),
                            IdKhachHang = reader.IsDBNull(reader.GetOrdinal("idKhachHang")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("idKhachHang")),
                            TenKhachVangLai = reader.IsDBNull(reader.GetOrdinal("tenKhachVangLai")) ? null : reader.GetString(reader.GetOrdinal("tenKhachVangLai")),
                            SdtKhachVangLai = reader.IsDBNull(reader.GetOrdinal("sdtKhachVangLai")) ? null : reader.GetString(reader.GetOrdinal("sdtKhachVangLai")),
                            IdBan = reader.IsDBNull(reader.GetOrdinal("idBan")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("idBan")),
                            ThoiGianDat = reader.GetDateTime(reader.GetOrdinal("thoiGianDat")),
                            SoLuongKhach = reader.GetInt32(reader.GetOrdinal("soLuongKhach")),
                            GhiChu = reader.IsDBNull(reader.GetOrdinal("ghiChu")) ? null : reader.GetString(reader.GetOrdinal("ghiChu")),
                            TrangThai = reader.GetString(reader.GetOrdinal("trangThai")),
                            SoBanHienThi = reader.IsDBNull(reader.GetOrdinal("SoBan")) ? "N/A" : reader.GetString(reader.GetOrdinal("SoBan")),
                            TenKhachHangThanhVien = reader.IsDBNull(reader.GetOrdinal("hoTen")) ? null : reader.GetString(reader.GetOrdinal("hoTen")),
                            SdtKhachHangThanhVien = reader.IsDBNull(reader.GetOrdinal("soDienThoai")) ? null : reader.GetString(reader.GetOrdinal("soDienThoai"))
                        });
                    }
                }
            }
            return ds;
        }

        // Trả về thông báo lỗi nếu có, null nếu thành công
        public string TaoPhieuDatBan(PhieuDatBan pdb)
        {
            // Kiểm tra xung đột thời gian (giả sử mỗi slot đặt bàn là 2 giờ)
            string conflictCheckQuery = @"SELECT COUNT(*) 
                                          FROM PhieuDatBan 
                                          WHERE idBan = @idBan 
                                          AND trangThai = N'Đã đặt' 
                                          AND @thoiGianDat < DATEADD(hour, 2, thoiGianDat) 
                                          AND DATEADD(hour, 2, @thoiGianDat) > thoiGianDat";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var checkCmd = new SqlCommand(conflictCheckQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@idBan", pdb.IdBan);
                    checkCmd.Parameters.AddWithValue("@thoiGianDat", pdb.ThoiGianDat);
                    int conflictCount = (int)checkCmd.ExecuteScalar();
                    if (conflictCount > 0)
                    {
                        return "Lỗi: Bàn này đã được đặt trong khung giờ đã chọn. Vui lòng chọn thời gian hoặc bàn khác.";
                    }
                }

                // Nếu không có xung đột, tiến hành tạo phiếu
                var cmd = new SqlCommand("INSERT INTO PhieuDatBan (idKhachHang, tenKhachVangLai, sdtKhachVangLai, idBan, thoiGianDat, soLuongKhach, ghiChu, trangThai) VALUES (@idKH, @ten, @sdt, @idBan, @thoiGian, @sl, @ghiChu, @trangThai)", conn);
                cmd.Parameters.AddWithValue("@idKH", (object)pdb.IdKhachHang ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ten", (object)pdb.TenKhachVangLai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@sdt", (object)pdb.SdtKhachVangLai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idBan", pdb.IdBan);
                cmd.Parameters.AddWithValue("@thoiGian", pdb.ThoiGianDat);
                cmd.Parameters.AddWithValue("@sl", pdb.SoLuongKhach);
                cmd.Parameters.AddWithValue("@ghiChu", (object)pdb.GhiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@trangThai", pdb.TrangThai);

                return cmd.ExecuteNonQuery() > 0 ? null : "Tạo phiếu đặt bàn thất bại.";
            }
        }


        public bool CapNhatTrangThaiPhieuDat(int idPhieuDat, string trangThaiMoi)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE PhieuDatBan SET trangThai = @trangThai WHERE idPhieuDatBan = @id", conn);
                cmd.Parameters.AddWithValue("@trangThai", trangThaiMoi);
                cmd.Parameters.AddWithValue("@id", idPhieuDat);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Thuê Sách
        public List<Sach> GetSachCoTheChoThue()
        {
            var ds = new List<Sach>();
            string query = "SELECT idSach, tieuDe FROM Sach WHERE soLuongCoSan > 0";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new Sach
                        {
                            IdSach = reader.GetInt32(0),
                            TieuDe = reader.GetString(1)
                        });
                    }
                }
            }
            return ds;
        }

        private List<PhieuThueSach> GetPhieuThue(string dieuKienTrangThai)
        {
            var ds = new List<PhieuThueSach>();
            string query = $@"SELECT 
                        pt.idPhieuThue, pt.ngayThue, pt.ngayHenTra, pt.trangThai, pt.phiThue, pt.tienCoc, pt.tienPhat,
                        s.tieuDe,
                        kh.hoTen, kh.soDienThoai
                     FROM PhieuThueSach pt
                     JOIN Sach s ON pt.idSach = s.idSach
                     JOIN KhachHang kh ON pt.idKhachHang = kh.idKhachHang
                     WHERE {dieuKienTrangThai}";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new PhieuThueSach
                        {
                            IdPhieuThue = reader.GetInt32(reader.GetOrdinal("idPhieuThue")),
                            NgayThue = reader.GetDateTime(reader.GetOrdinal("ngayThue")),
                            NgayHenTra = reader.GetDateTime(reader.GetOrdinal("ngayHenTra")),
                            TrangThai = reader.GetString(reader.GetOrdinal("trangThai")),
                            PhiThue = reader.GetDecimal(reader.GetOrdinal("phiThue")),
                            TienCoc = reader.GetDecimal(reader.GetOrdinal("tienCoc")),
                            TienPhat = reader.GetDecimal(reader.GetOrdinal("tienPhat")),
                            TieuDeSach = reader.GetString(reader.GetOrdinal("tieuDe")),
                            TenKhachHang = reader.GetString(reader.GetOrdinal("hoTen")),
                            SdtKhachHang = reader.GetString(reader.GetOrdinal("soDienThoai"))
                        });
                    }
                }
            }
            return ds;
        }

        public List<PhieuThueSach> GetPhieuDangThue()
        {
            return GetPhieuThue("pt.trangThai = N'Đang thuê'");
        }

        public List<PhieuThueSach> GetPhieuQuaHan()
        {
            return GetPhieuThue("pt.trangThai = N'Quá hạn' OR (pt.trangThai = N'Đang thuê' AND pt.ngayHenTra < GETDATE())");
        }

        public PhieuThueSach ThucHienChoMuon(PhieuThueSach pts)
        {
            decimal phiThue = CaiDatHelper.GetGiaTriDecimal("PhiThueSach");
            decimal tienCoc = 0;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Lấy tiền cọc (giá bìa) của sách
                var cmdGetCoc = new SqlCommand("SELECT giaBia FROM Sach WHERE idSach = @idSach", conn);
                cmdGetCoc.Parameters.AddWithValue("@idSach", pts.IdSach);
                var result = cmdGetCoc.ExecuteScalar();
                if (result != null) tienCoc = (decimal)result;

                var transaction = conn.BeginTransaction();
                try
                {
                    var cmdPts = new SqlCommand("INSERT INTO PhieuThueSach (idSach, idKhachHang, idNhanVien, ngayThue, ngayHenTra, trangThai, phiThue, tienCoc) OUTPUT INSERTED.idPhieuThue VALUES (@idSach, @idKH, @idNV, GETDATE(), @ngayHenTra, N'Đang thuê', @phiThue, @tienCoc)", conn, transaction);
                    cmdPts.Parameters.AddWithValue("@idSach", pts.IdSach);
                    cmdPts.Parameters.AddWithValue("@idKH", pts.IdKhachHang);
                    cmdPts.Parameters.AddWithValue("@idNV", pts.IdNhanVien);
                    cmdPts.Parameters.AddWithValue("@ngayHenTra", pts.NgayHenTra);
                    cmdPts.Parameters.AddWithValue("@phiThue", phiThue);
                    cmdPts.Parameters.AddWithValue("@tienCoc", tienCoc);
                    int newIdPhieuThue = (int)cmdPts.ExecuteScalar();

                    var cmdSach = new SqlCommand("UPDATE Sach SET soLuongCoSan = soLuongCoSan - 1 WHERE idSach = @idSach", conn, transaction);
                    cmdSach.Parameters.AddWithValue("@idSach", pts.IdSach);
                    cmdSach.ExecuteNonQuery();

                    transaction.Commit();

                    var cmdGetInfo = new SqlCommand(@"SELECT 
                                                pt.idPhieuThue, pt.ngayThue, pt.ngayHenTra, pt.phiThue, pt.tienCoc,
                                                s.tieuDe, s.viTri,
                                                kh.hoTen, kh.soDienThoai
                                             FROM PhieuThueSach pt
                                             JOIN Sach s ON pt.idSach = s.idSach
                                             JOIN KhachHang kh ON pt.idKhachHang = kh.idKhachHang
                                             WHERE pt.idPhieuThue = @id", conn);
                    cmdGetInfo.Parameters.AddWithValue("@id", newIdPhieuThue);
                    using (var reader = cmdGetInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PhieuThueSach
                            {
                                IdPhieuThue = reader.GetInt32(0),
                                NgayThue = reader.GetDateTime(1),
                                NgayHenTra = reader.GetDateTime(2),
                                PhiThue = reader.GetDecimal(3),
                                TienCoc = reader.GetDecimal(4),
                                TieuDeSach = reader.GetString(5),
                                ViTriSach = reader.GetString(6),
                                TenKhachHang = reader.GetString(7),
                                SdtKhachHang = reader.GetString(8)
                            };
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public bool ThucHienTraSach(int idPhieuThue, decimal tienPhat)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    var cmdGetId = new SqlCommand("SELECT idSach FROM PhieuThueSach WHERE idPhieuThue = @idPT", conn, transaction);
                    cmdGetId.Parameters.AddWithValue("@idPT", idPhieuThue);
                    int idSach = (int)cmdGetId.ExecuteScalar();

                    var cmdPts = new SqlCommand("UPDATE PhieuThueSach SET ngayTraThucTe = GETDATE(), tienPhat = @tienPhat, trangThai = N'Đã trả' WHERE idPhieuThue = @idPT", conn, transaction);
                    cmdPts.Parameters.AddWithValue("@tienPhat", tienPhat);
                    cmdPts.Parameters.AddWithValue("@idPT", idPhieuThue);
                    cmdPts.ExecuteNonQuery();

                    var cmdSach = new SqlCommand("UPDATE Sach SET soLuongCoSan = soLuongCoSan + 1 WHERE idSach = @idSach", conn, transaction);
                    cmdSach.Parameters.AddWithValue("@idSach", idSach);
                    cmdSach.ExecuteNonQuery();

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

        /// <summary>
        /// Tìm kiếm tất cả các phiếu đang thuê hoặc quá hạn dựa trên từ khóa.
        /// </summary>
        public List<PhieuThueSach> TimKiemPhieuThue(string keyword)
        {
            var ds = new List<PhieuThueSach>();
            string query = @"SELECT 
                        pt.idPhieuThue, pt.ngayThue, pt.ngayHenTra, pt.trangThai, pt.phiThue, pt.tienCoc, pt.tienPhat,
                        s.tieuDe,
                        kh.hoTen, kh.soDienThoai
                     FROM PhieuThueSach pt
                     JOIN Sach s ON pt.idSach = s.idSach
                     JOIN KhachHang kh ON pt.idKhachHang = kh.idKhachHang
                     WHERE (pt.trangThai = N'Đang thuê' OR pt.trangThai = N'Quá hạn' OR (pt.ngayHenTra < GETDATE() AND pt.trangThai != N'Đã trả'))
                     AND (
                         CAST(pt.idPhieuThue AS NVARCHAR(10)) = @keyword 
                         OR kh.hoTen LIKE @keywordLike
                         OR kh.soDienThoai LIKE @keywordLike
                     )";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@keyword", keyword);
                cmd.Parameters.AddWithValue("@keywordLike", "%" + keyword + "%");
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new PhieuThueSach
                        {
                            IdPhieuThue = reader.GetInt32(reader.GetOrdinal("idPhieuThue")),
                            NgayThue = reader.GetDateTime(reader.GetOrdinal("ngayThue")),
                            NgayHenTra = reader.GetDateTime(reader.GetOrdinal("ngayHenTra")),
                            TrangThai = reader.GetString(reader.GetOrdinal("trangThai")),
                            PhiThue = reader.GetDecimal(reader.GetOrdinal("phiThue")),
                            TienCoc = reader.GetDecimal(reader.GetOrdinal("tienCoc")),
                            TienPhat = reader.GetDecimal(reader.GetOrdinal("tienPhat")),
                            TieuDeSach = reader.GetString(reader.GetOrdinal("tieuDe")),
                            TenKhachHang = reader.GetString(reader.GetOrdinal("hoTen")),
                            SdtKhachHang = reader.GetString(reader.GetOrdinal("soDienThoai"))
                        });
                    }
                }
            }
            return ds;
        }
        #endregion
    }
}