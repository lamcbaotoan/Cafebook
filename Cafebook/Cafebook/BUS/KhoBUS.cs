using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class KhoBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region Nguyên Liệu
        public List<NguyenLieu> GetDanhSachNguyenLieu()
        {
            var ds = new List<NguyenLieu>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT idNguyenLieu, tenNguyenLieu, donViTinh, soLuongTon, nguongCanhBao FROM NguyenLieu", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new NguyenLieu
                        {
                            IdNguyenLieu = reader.GetInt32(0),
                            TenNguyenLieu = reader.GetString(1),
                            DonViTinh = reader.GetString(2),
                            SoLuongTon = reader.GetDecimal(3),
                            NguongCanhBao = reader.GetDecimal(4)
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemNguyenLieu(NguyenLieu nl)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO NguyenLieu (tenNguyenLieu, donViTinh, nguongCanhBao, soLuongTon) VALUES (@ten, @dvt, @nguong, 0)", conn);
                cmd.Parameters.AddWithValue("@ten", nl.TenNguyenLieu);
                cmd.Parameters.AddWithValue("@dvt", nl.DonViTinh);
                cmd.Parameters.AddWithValue("@nguong", nl.NguongCanhBao);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaNguyenLieu(NguyenLieu nl)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NguyenLieu SET tenNguyenLieu = @ten, donViTinh = @dvt, nguongCanhBao = @nguong WHERE idNguyenLieu = @id", conn);
                cmd.Parameters.AddWithValue("@ten", nl.TenNguyenLieu);
                cmd.Parameters.AddWithValue("@dvt", nl.DonViTinh);
                cmd.Parameters.AddWithValue("@nguong", nl.NguongCanhBao);
                cmd.Parameters.AddWithValue("@id", nl.IdNguyenLieu);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaNguyenLieu(int idNguyenLieu)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM CongThuc WHERE idNguyenLieu = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", idNguyenLieu);
                if ((int)checkCmd.ExecuteScalar() > 0) return false;

                var deleteCmd = new SqlCommand("DELETE FROM NguyenLieu WHERE idNguyenLieu = @id", conn);
                deleteCmd.Parameters.AddWithValue("@id", idNguyenLieu);
                return deleteCmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Tồn Kho
        public List<NguyenLieu> GetNguyenLieuCanhBao()
        {
            var ds = new List<NguyenLieu>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT idNguyenLieu, tenNguyenLieu, soLuongTon, donViTinh, nguongCanhBao FROM NguyenLieu WHERE soLuongTon <= nguongCanhBao", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new NguyenLieu
                        {
                            IdNguyenLieu = reader.GetInt32(0),
                            TenNguyenLieu = reader.GetString(1),
                            SoLuongTon = reader.GetDecimal(2),
                            DonViTinh = reader.GetString(3),
                            NguongCanhBao = reader.GetDecimal(4)
                        });
                    }
                }
            }
            return ds;
        }
        #endregion

        #region Nhà Cung Cấp
        public List<NhaCungCap> GetNhaCungCap()
        {
            var dsncc = new List<NhaCungCap>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM NhaCungCap", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsncc.Add(new NhaCungCap
                        {
                            IdNhaCungCap = reader.GetInt32(0),
                            TenNhaCungCap = reader.GetString(1),
                            SoDienThoai = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            DiaChi = reader.IsDBNull(3) ? "" : reader.GetString(3)
                        });
                    }
                }
            }
            return dsncc;
        }

        public bool ThemNhaCungCap(NhaCungCap ncc)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO NhaCungCap (tenNhaCungCap, soDienThoai, diaChi) VALUES (@ten, @sdt, @diachi)", conn);
                cmd.Parameters.AddWithValue("@ten", ncc.TenNhaCungCap);
                cmd.Parameters.AddWithValue("@sdt", ncc.SoDienThoai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@diachi", ncc.DiaChi ?? (object)DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaNhaCungCap(NhaCungCap ncc)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NhaCungCap SET tenNhaCungCap = @ten, soDienThoai = @sdt, diaChi = @diachi WHERE idNhaCungCap = @id", conn);
                cmd.Parameters.AddWithValue("@ten", ncc.TenNhaCungCap);
                cmd.Parameters.AddWithValue("@sdt", ncc.SoDienThoai ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@diachi", ncc.DiaChi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id", ncc.IdNhaCungCap);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaNhaCungCap(int idNhaCungCap)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM PhieuNhapKho WHERE idNhaCungCap = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", idNhaCungCap);
                if ((int)checkCmd.ExecuteScalar() > 0) return false;

                var deleteCmd = new SqlCommand("DELETE FROM NhaCungCap WHERE idNhaCungCap = @id", conn);
                deleteCmd.Parameters.AddWithValue("@id", idNhaCungCap);
                return deleteCmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Nhập Kho & Hóa Đơn
        public List<PhieuNhapKho> GetPhieuNhapKho()
        {
            var dspnk = new List<PhieuNhapKho>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT pnk.idPhieuNhap, pnk.ngayNhap, ncc.tenNhaCungCap, nv.hoTen, pnk.tongTien
                    FROM PhieuNhapKho pnk
                    JOIN NhaCungCap ncc ON pnk.idNhaCungCap = ncc.idNhaCungCap
                    JOIN NhanVien nv ON pnk.idNhanVien = nv.idNhanVien
                    ORDER BY pnk.ngayNhap DESC", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dspnk.Add(new PhieuNhapKho
                        {
                            IdPhieuNhap = reader.GetInt32(0),
                            NgayNhap = reader.GetDateTime(1),
                            TenNhaCungCap = reader.GetString(2),
                            TenNhanVien = reader.GetString(3),
                            TongTien = reader.GetDecimal(4)
                        });
                    }
                }
            }
            return dspnk;
        }

        public List<ChiTietPhieuNhap> GetChiTietPhieuNhap(int idPhieuNhap)
        {
            var dsct = new List<ChiTietPhieuNhap>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT ct.idNguyenLieu, nl.tenNguyenLieu, ct.soLuong, ct.donGia
                    FROM ChiTietPhieuNhap ct
                    JOIN NguyenLieu nl ON ct.idNguyenLieu = nl.idNguyenLieu
                    WHERE ct.idPhieuNhap = @idPhieuNhap", conn);
                cmd.Parameters.AddWithValue("@idPhieuNhap", idPhieuNhap);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsct.Add(new ChiTietPhieuNhap
                        {
                            IdNguyenLieu = reader.GetInt32(0),
                            TenNguyenLieu = reader.GetString(1),
                            SoLuong = reader.GetDecimal(2),
                            DonGia = reader.GetDecimal(3)
                        });
                    }
                }
            }
            return dsct;
        }

        public bool TaoPhieuNhap(PhieuNhapKho pnk, List<ChiTietPhieuNhap> chiTiet)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    var cmdPnk = new SqlCommand("INSERT INTO PhieuNhapKho (idNhanVien, idNhaCungCap, ngayNhap, tongTien) OUTPUT INSERTED.idPhieuNhap VALUES (@idNV, @idNCC, @ngayNhap, @tongTien)", conn, transaction);
                    cmdPnk.Parameters.AddWithValue("@idNV", pnk.IdNhanVien);
                    cmdPnk.Parameters.AddWithValue("@idNCC", pnk.IdNhaCungCap);
                    cmdPnk.Parameters.AddWithValue("@ngayNhap", pnk.NgayNhap);
                    cmdPnk.Parameters.AddWithValue("@tongTien", pnk.TongTien);
                    int newPhieuNhapId = (int)cmdPnk.ExecuteScalar();

                    foreach (var item in chiTiet)
                    {
                        var cmdCt = new SqlCommand("INSERT INTO ChiTietPhieuNhap (idPhieuNhap, idNguyenLieu, soLuong, donGia) VALUES (@idPN, @idNL, @soLuong, @donGia)", conn, transaction);
                        cmdCt.Parameters.AddWithValue("@idPN", newPhieuNhapId);
                        cmdCt.Parameters.AddWithValue("@idNL", item.IdNguyenLieu);
                        cmdCt.Parameters.AddWithValue("@soLuong", item.SoLuong);
                        cmdCt.Parameters.AddWithValue("@donGia", item.DonGia);
                        cmdCt.ExecuteNonQuery();

                        var cmdUpdateKho = new SqlCommand("UPDATE NguyenLieu SET soLuongTon = soLuongTon + @soLuongNhap WHERE idNguyenLieu = @idNL", conn, transaction);
                        cmdUpdateKho.Parameters.AddWithValue("@soLuongNhap", item.SoLuong);
                        cmdUpdateKho.Parameters.AddWithValue("@idNL", item.IdNguyenLieu);
                        cmdUpdateKho.ExecuteNonQuery();
                    }

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

        public HoaDonDauVao GetHoaDonDauVao(int idPhieuNhap)
        {
            HoaDonDauVao hddv = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM HoaDonDauVao WHERE idPhieuNhap = @idPhieuNhap", conn);
                cmd.Parameters.AddWithValue("@idPhieuNhap", idPhieuNhap);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hddv = new HoaDonDauVao
                        {
                            IdHoaDonNhap = reader.GetInt32(0),
                            IdPhieuNhap = reader.GetInt32(1),
                            maHoaDon = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ngayPhatHanh = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                            DuongDanFile = reader.IsDBNull(4) ? "" : reader.GetString(4)
                        };
                    }
                }
            }
            return hddv;
        }

        public bool LuuHoaDonDauVao(HoaDonDauVao hddv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    MERGE HoaDonDauVao AS target
                    USING (SELECT @idPhieuNhap AS idPhieuNhap) AS source
                    ON (target.idPhieuNhap = source.idPhieuNhap)
                    WHEN MATCHED THEN
                        UPDATE SET duongDanFile = @path
                    WHEN NOT MATCHED THEN
                        INSERT (idPhieuNhap, duongDanFile) VALUES (@idPhieuNhap, @path);", conn);

                cmd.Parameters.AddWithValue("@idPhieuNhap", hddv.IdPhieuNhap);
                cmd.Parameters.AddWithValue("@path", hddv.DuongDanFile);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}