using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class NhanSuBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region NhanVien & VaiTro & CaLamViec
        // SỬA LẠI: Lấy đầy đủ các cột từ bảng NhanVien
        public List<NhanVien> GetDanhSachNhanVien()
        {
            var ds = new List<NhanVien>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Lấy tất cả các cột cần thiết từ bảng NhanVien
                var cmd = new SqlCommand("SELECT nv.*, vt.tenVaiTro FROM NhanVien nv JOIN VaiTro vt ON nv.idVaiTro = vt.idVaiTro", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new NhanVien
                        {
                            IdNhanVien = (int)reader["idNhanVien"],
                            IdVaiTro = (int)reader["idVaiTro"],
                            HoTen = (string)reader["hoTen"],
                            SoDienThoai = reader["soDienThoai"] as string,
                            Email = reader["email"] as string,
                            DiaChi = reader["diaChi"] as string,
                            MatKhau = (string)reader["matKhau"],
                            NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                            TrangThai = (bool)reader["trangThai"],
                            MucLuongTheoGio = (decimal)reader["mucLuongTheoGio"],
                            TenVaiTro = (string)reader["tenVaiTro"]
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemNhanVien(NhanVien nv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO NhanVien (idVaiTro, hoTen, matKhau, ngayVaoLam, trangThai, mucLuongTheoGio) VALUES (@idVT, @hoTen, @matKhau, GETDATE(), @trangThai, @luong)", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@trangThai", nv.TrangThai);
                cmd.Parameters.AddWithValue("@luong", nv.MucLuongTheoGio);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaNhanVien(NhanVien nv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NhanVien SET idVaiTro = @idVT, hoTen = @hoTen, matKhau = @matKhau, mucLuongTheoGio = @luong, trangThai = @trangThai WHERE idNhanVien = @idNV", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@luong", nv.MucLuongTheoGio);
                cmd.Parameters.AddWithValue("@trangThai", nv.TrangThai);
                cmd.Parameters.AddWithValue("@idNV", nv.IdNhanVien);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<VaiTro> GetDanhSachVaiTro()
        {
            var ds = new List<VaiTro>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM VaiTro", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) { ds.Add(new VaiTro { IdVaiTro = (int)reader["idVaiTro"], TenVaiTro = (string)reader["tenVaiTro"] }); }
                }
            }
            return ds;
        }

        public List<CaLamViec> GetDanhSachCaLamViec()
        {
            var ds = new List<CaLamViec>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM CaLamViec", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) { ds.Add(new CaLamViec { IdCa = (int)reader["idCa"], TenCa = (string)reader["tenCa"], GioBatDau = (TimeSpan)reader["gioBatDau"], GioKetThuc = (TimeSpan)reader["gioKetThuc"] }); }
                }
            }
            return ds;
        }

        // SỬA LẠI: Cho phép nhân viên cập nhật nhiều thông tin hơn
        public bool CapNhatThongTinCaNhan(NhanVien nv, bool updatePassword)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Tạo câu lệnh SQL linh hoạt: chỉ cập nhật mật khẩu khi cần
                string query = @"UPDATE NhanVien 
                                 SET soDienThoai = @sdt, email = @email, diaChi = @diaChi"
                               + (updatePassword ? ", matKhau = @matKhau " : " ")
                               + "WHERE idNhanVien = @idNV";

                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sdt", (object)nv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object)nv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@diaChi", (object)nv.DiaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idNV", nv.IdNhanVien);

                if (updatePassword)
                {
                    cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                }

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region LichLamViec
        public List<LichLamViec> GetLichLamViec(DateTime ngay)
        {
            var ds = new List<LichLamViec>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT llv.idLichLamViec, nv.hoTen, clv.tenCa, clv.gioBatDau, clv.gioKetThuc 
                    FROM LichLamViec llv 
                    JOIN NhanVien nv ON llv.idNhanVien = nv.idNhanVien
                    JOIN CaLamViec clv ON llv.idCa = clv.idCa
                    WHERE llv.ngayLam = @ngayLam", conn);
                cmd.Parameters.AddWithValue("@ngayLam", ngay.Date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new LichLamViec { IdLichLamViec = (int)reader["idLichLamViec"], HoTenNhanVien = (string)reader["hoTen"], TenCa = (string)reader["tenCa"], GioBatDau = (TimeSpan)reader["gioBatDau"], GioKetThuc = (TimeSpan)reader["gioKetThuc"] });
                    }
                }
            }
            return ds;
        }

        public bool ThemLichLamViec(LichLamViec llv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO LichLamViec (idNhanVien, idCa, ngayLam) VALUES (@idNV, @idCa, @ngayLam)", conn);
                cmd.Parameters.AddWithValue("@idNV", llv.IdNhanVien);
                cmd.Parameters.AddWithValue("@idCa", llv.IdCa);
                cmd.Parameters.AddWithValue("@ngayLam", llv.NgayLam.Date);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaLichLamViec(int idLichLamViec)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM LichLamViec WHERE idLichLamViec = @id", conn);
                cmd.Parameters.AddWithValue("@id", idLichLamViec);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region ChinhSachLuong
        public List<LoaiThuongPhat> GetChinhSach()
        {
            var ds = new List<LoaiThuongPhat>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM LoaiThuongPhat", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) { ds.Add(new LoaiThuongPhat { IdLoai = (int)reader["idLoai"], TenLoai = (string)reader["tenLoai"], SoTien = (decimal)reader["soTien"], Loai = (string)reader["loai"] }); }
                }
            }
            return ds;
        }

        public bool ThemChinhSach(LoaiThuongPhat ltp)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO LoaiThuongPhat (tenLoai, soTien, loai) VALUES (@ten, @tien, @loai)", conn);
                cmd.Parameters.AddWithValue("@ten", ltp.TenLoai);
                cmd.Parameters.AddWithValue("@tien", ltp.SoTien);
                cmd.Parameters.AddWithValue("@loai", ltp.Loai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaChinhSach(LoaiThuongPhat ltp)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE LoaiThuongPhat SET tenLoai = @ten, soTien = @tien, loai = @loai WHERE idLoai = @id", conn);
                cmd.Parameters.AddWithValue("@ten", ltp.TenLoai);
                cmd.Parameters.AddWithValue("@tien", ltp.SoTien);
                cmd.Parameters.AddWithValue("@loai", ltp.Loai);
                cmd.Parameters.AddWithValue("@id", ltp.IdLoai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaChinhSach(int idLoai)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM LoaiThuongPhat WHERE idLoai = @id", conn);
                cmd.Parameters.AddWithValue("@id", idLoai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region ChamCong & Luong
        public List<BangChamCong> GetBangChamCong(DateTime ngay)
        {
            var ds = new List<BangChamCong>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT bcc.idChamCong, nv.hoTen, llv.ngayLam, clv.tenCa, bcc.gioVao, bcc.gioRa, bcc.soGioLam
                    FROM BangChamCong bcc
                    JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
                    JOIN NhanVien nv ON llv.idNhanVien = nv.idNhanVien
                    JOIN CaLamViec clv ON llv.idCa = clv.idCa
                    WHERE llv.ngayLam = @ngayLam", conn);
                cmd.Parameters.AddWithValue("@ngayLam", ngay.Date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new BangChamCong
                        {
                            IdChamCong = (int)reader["idChamCong"],
                            HoTenNhanVien = (string)reader["hoTen"],
                            NgayLam = (DateTime)reader["ngayLam"],
                            TenCa = (string)reader["tenCa"],
                            GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"],
                            GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"],
                            SoGioLam = reader.IsDBNull(reader.GetOrdinal("soGioLam")) ? (decimal?)null : (decimal)reader["soGioLam"]
                        });
                    }
                }
            }
            return ds;
        }

        public int TuDongPhatDiTre(DateTime ngay, int soPhutChoPhep)
        {
            int recordsAffected = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmdGetIdLoai = new SqlCommand("SELECT TOP 1 idLoai FROM LoaiThuongPhat WHERE tenLoai LIKE N'%Phạt đi trễ%' AND loai = 'Phat'", conn);
                object idLoaiPhatObj = cmdGetIdLoai.ExecuteScalar();
                if (idLoaiPhatObj == null) return -1; // -1 báo lỗi không có chính sách
                int idLoaiPhat = (int)idLoaiPhatObj;

                var cmdGetLate = new SqlCommand(@"
                    SELECT llv.idNhanVien
                    FROM BangChamCong bcc
                    JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
                    JOIN CaLamViec clv ON llv.idCa = clv.idCa
                    WHERE llv.ngayLam = @ngay AND DATEDIFF(MINUTE, clv.gioBatDau, CAST(bcc.gioVao AS TIME)) > @soPhut", conn);
                cmdGetLate.Parameters.AddWithValue("@ngay", ngay.Date);
                cmdGetLate.Parameters.AddWithValue("@soPhut", soPhutChoPhep);

                var listTre = new List<int>();
                using (var reader = cmdGetLate.ExecuteReader())
                {
                    while (reader.Read()) listTre.Add(reader.GetInt32(0));
                }

                foreach (int idNhanVien in listTre)
                {
                    var cmdCheckExist = new SqlCommand("SELECT COUNT(*) FROM ChiTietThuongPhat WHERE idNhanVien = @idNV AND idLoai = @idLoai AND ngayApDung = @ngay", conn);
                    cmdCheckExist.Parameters.AddWithValue("@idNV", idNhanVien);
                    cmdCheckExist.Parameters.AddWithValue("@idLoai", idLoaiPhat);
                    cmdCheckExist.Parameters.AddWithValue("@ngay", ngay.Date);

                    if ((int)cmdCheckExist.ExecuteScalar() == 0)
                    {
                        var cmdInsertPhat = new SqlCommand("INSERT INTO ChiTietThuongPhat (idNhanVien, idLoai, ngayApDung, ghiChu) VALUES (@idNV, @idLoai, @ngay, @ghiChu)", conn);
                        cmdInsertPhat.Parameters.AddWithValue("@idNV", idNhanVien);
                        cmdInsertPhat.Parameters.AddWithValue("@idLoai", idLoaiPhat);
                        cmdInsertPhat.Parameters.AddWithValue("@ngay", ngay.Date);
                        cmdInsertPhat.Parameters.AddWithValue("@ghiChu", $"Tự động phạt đi trễ ca ngày {ngay:dd/MM}");
                        if (cmdInsertPhat.ExecuteNonQuery() > 0) recordsAffected++;
                    }
                }
            }
            return recordsAffected;
        }

        public PhieuLuong TinhLuong(int idNhanVien, DateTime tuNgay, DateTime denNgay)
        {
            var pl = new PhieuLuong { IdNhanVien = idNhanVien, TuNgay = tuNgay, DenNgay = denNgay };
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmdLuong = new SqlCommand("SELECT mucLuongTheoGio FROM NhanVien WHERE idNhanVien = @idNV", conn);
                cmdLuong.Parameters.AddWithValue("@idNV", idNhanVien);
                decimal mucLuong = (decimal)cmdLuong.ExecuteScalar();

                var cmdGio = new SqlCommand(@"SELECT ISNULL(SUM(bcc.soGioLam), 0) FROM BangChamCong bcc JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
                                            WHERE llv.idNhanVien = @idNV AND llv.ngayLam BETWEEN @tuNgay AND @denNgay", conn);
                cmdGio.Parameters.AddWithValue("@idNV", idNhanVien);
                cmdGio.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmdGio.Parameters.AddWithValue("@denNgay", denNgay.Date);
                pl.TongGioLam = (decimal)cmdGio.ExecuteScalar();

                pl.LuongCoBan = pl.TongGioLam * mucLuong;

                var cmdThuongPhat = new SqlCommand(@"SELECT ltp.loai, ISNULL(SUM(ltp.soTien), 0) FROM ChiTietThuongPhat cttp JOIN LoaiThuongPhat ltp ON cttp.idLoai = ltp.idLoai
                                                    WHERE cttp.idNhanVien = @idNV AND cttp.ngayApDung BETWEEN @tuNgay AND @denNgay GROUP BY ltp.loai", conn);
                cmdThuongPhat.Parameters.AddWithValue("@idNV", idNhanVien);
                cmdThuongPhat.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmdThuongPhat.Parameters.AddWithValue("@denNgay", denNgay.Date);
                using (var reader = cmdThuongPhat.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(0) == "Thuong") pl.TongThuong = reader.GetDecimal(1);
                        else pl.TongPhat = reader.GetDecimal(1);
                    }
                }
                pl.ThucLanh = pl.LuongCoBan + pl.TongThuong - pl.TongPhat;
            }
            return pl;
        }

        public bool ChotPhieuLuong(PhieuLuong pl)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO PhieuLuong (idNhanVien, tuNgay, denNgay, tongGioLam, luongCoBan, tongThuong, tongPhat, thucLanh) VALUES (@idNV, @tuNgay, @denNgay, @tongGio, @luongCB, @thuong, @phat, @thucLanh)", conn);
                cmd.Parameters.AddWithValue("@idNV", pl.IdNhanVien);
                cmd.Parameters.AddWithValue("@tuNgay", pl.TuNgay);
                cmd.Parameters.AddWithValue("@denNgay", pl.DenNgay);
                cmd.Parameters.AddWithValue("@tongGio", pl.TongGioLam);
                cmd.Parameters.AddWithValue("@luongCB", pl.LuongCoBan);
                cmd.Parameters.AddWithValue("@thuong", pl.TongThuong);
                cmd.Parameters.AddWithValue("@phat", pl.TongPhat);
                cmd.Parameters.AddWithValue("@thucLanh", pl.ThucLanh);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<PhieuLuong> GetLichSuPhieuLuong(int idNhanVien)
        {
            var ds = new List<PhieuLuong>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM PhieuLuong WHERE idNhanVien = @idNV ORDER BY tuNgay DESC", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // SỬA LẠI HÀM NÀY ĐỂ LẤY ĐẦY ĐỦ DỮ LIỆU
                        ds.Add(new PhieuLuong
                        {
                            IdPhieuLuong = (int)reader["idPhieuLuong"],
                            IdNhanVien = (int)reader["idNhanVien"],
                            TuNgay = (DateTime)reader["tuNgay"],
                            DenNgay = (DateTime)reader["denNgay"], // << Dòng bị thiếu
                            TongGioLam = (decimal)reader["tongGioLam"],
                            LuongCoBan = (decimal)reader["luongCoBan"],
                            TongThuong = (decimal)reader["tongThuong"],
                            TongPhat = (decimal)reader["tongPhat"],
                            ThucLanh = (decimal)reader["thucLanh"],
                            NgayTinhLuong = (DateTime)reader["ngayTinhLuong"]
                        });
                    }
                }
            }
            return ds;
        }

        public LichLamViec GetLichLamViecHomNay(int idNhanVien)
        {
            LichLamViec llv = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT llv.*, clv.tenCa, clv.gioBatDau, clv.gioKetThuc 
                    FROM LichLamViec llv JOIN CaLamViec clv ON llv.idCa = clv.idCa
                    WHERE llv.idNhanVien = @idNV AND llv.ngayLam = CONVERT(date, GETDATE())", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        llv = new LichLamViec { IdLichLamViec = (int)reader["idLichLamViec"], IdNhanVien = (int)reader["idNhanVien"], IdCa = (int)reader["idCa"], NgayLam = (DateTime)reader["ngayLam"], TenCa = (string)reader["tenCa"], GioBatDau = (TimeSpan)reader["gioBatDau"], GioKetThuc = (TimeSpan)reader["gioKetThuc"] };
                    }
                }
            }
            return llv;
        }

        public BangChamCong GetTrangThaiChamCong(int idLichLamViec)
        {
            BangChamCong bcc = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT TOP 1 * FROM BangChamCong WHERE idLichLamViec = @idLLV ORDER BY idChamCong DESC", conn);
                cmd.Parameters.AddWithValue("@idLLV", idLichLamViec);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bcc = new BangChamCong { IdChamCong = (int)reader["idChamCong"], IdLichLamViec = (int)reader["idLichLamViec"], GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"], GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"] };
                    }
                }
            }
            return bcc;
        }

        public int ThucHienVaoCa(int idLichLamViec)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO BangChamCong (idLichLamViec, gioVao) OUTPUT INSERTED.idChamCong VALUES (@idLLV, @gioVao)", conn);
                cmd.Parameters.AddWithValue("@idLLV", idLichLamViec);
                cmd.Parameters.AddWithValue("@gioVao", DateTime.Now);
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool ThucHienRaCa(int idChamCong)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    UPDATE BangChamCong 
                    SET gioRa = @gioRa, 
                        soGioLam = CAST(DATEDIFF(MINUTE, gioVao, @gioRa) AS DECIMAL(4,2)) / 60
                    WHERE idChamCong = @idCC", conn);
                cmd.Parameters.AddWithValue("@gioRa", DateTime.Now);
                cmd.Parameters.AddWithValue("@idCC", idChamCong);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<BangChamCong> GetLichSuChamCong(int idNhanVien)
        {
            var ds = new List<BangChamCong>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT llv.ngayLam, clv.tenCa, bcc.gioVao, bcc.gioRa, bcc.soGioLam
                    FROM BangChamCong bcc
                    JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
                    JOIN CaLamViec clv ON llv.idCa = clv.idCa
                    WHERE llv.idNhanVien = @idNV
                    ORDER BY llv.ngayLam DESC, bcc.gioVao DESC", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new BangChamCong
                        {
                            NgayLam = (DateTime)reader["ngayLam"],
                            TenCa = (string)reader["tenCa"],
                            GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"],
                            GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"],
                            SoGioLam = reader.IsDBNull(reader.GetOrdinal("soGioLam")) ? (decimal?)null : (decimal)reader["soGioLam"]
                        });
                    }
                }
            }
            return ds;
        }
        #endregion
    }
}