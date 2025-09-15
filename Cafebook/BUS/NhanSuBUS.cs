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

        #region NhanVien
        public List<NhanVien> GetDanhSachNhanVien()
        {
            var ds = new List<NhanVien>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT nv.*, vt.tenVaiTro FROM NhanVien nv JOIN VaiTro vt ON nv.idVaiTro = vt.idVaiTro", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new NhanVien
                        {
                            IdNhanVien = (int)reader["idNhanVien"],
                            HoTen = (string)reader["hoTen"],
                            SoDienThoai = reader["soDienThoai"] as string,
                            Email = reader["email"] as string,
                            DiaChi = reader["diaChi"] as string,
                            NgayVaoLam = (DateTime)reader["ngayVaoLam"],
                            TrangThai = (bool)reader["trangThai"],
                            IdVaiTro = (int)reader["idVaiTro"],
                            TenVaiTro = (string)reader["tenVaiTro"],
                            MatKhau = (string)reader["matKhau"]
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
                var cmd = new SqlCommand("INSERT INTO NhanVien (idVaiTro, hoTen, soDienThoai, email, diaChi, matKhau, ngayVaoLam, trangThai) VALUES (@idVT, @hoTen, @sdt, @email, @diaChi, @matKhau, @ngayVaoLam, @trangThai)", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@sdt", nv.SoDienThoai);
                cmd.Parameters.AddWithValue("@email", nv.Email);
                cmd.Parameters.AddWithValue("@diaChi", nv.DiaChi);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@ngayVaoLam", nv.NgayVaoLam);
                cmd.Parameters.AddWithValue("@trangThai", nv.TrangThai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaNhanVien(NhanVien nv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE NhanVien SET idVaiTro = @idVT, hoTen = @hoTen, soDienThoai = @sdt, email = @email, diaChi = @diaChi, matKhau = @matKhau, ngayVaoLam = @ngayVaoLam, trangThai = @trangThai WHERE idNhanVien = @idNV", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@sdt", nv.SoDienThoai);
                cmd.Parameters.AddWithValue("@email", nv.Email);
                cmd.Parameters.AddWithValue("@diaChi", nv.DiaChi);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@ngayVaoLam", nv.NgayVaoLam);
                cmd.Parameters.AddWithValue("@trangThai", nv.TrangThai);
                cmd.Parameters.AddWithValue("@idNV", nv.IdNhanVien);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region VaiTro & CaLamViec
        public List<VaiTro> GetDanhSachVaiTro()
        {
            var ds = new List<VaiTro>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM VaiTro", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new VaiTro { IdVaiTro = (int)reader["idVaiTro"], TenVaiTro = (string)reader["tenVaiTro"] });
                    }
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
                    while (reader.Read())
                    {
                        ds.Add(new CaLamViec { IdCa = (int)reader["idCa"], TenCa = (string)reader["tenCa"], GioBatDau = (TimeSpan)reader["gioBatDau"], GioKetThuc = (TimeSpan)reader["gioKetThuc"] });
                    }
                }
            }
            return ds;
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
                        ds.Add(new LichLamViec
                        {
                            IdLichLamViec = (int)reader["idLichLamViec"],
                            HoTenNhanVien = (string)reader["hoTen"],
                            TenCa = (string)reader["tenCa"],
                            GioBatDau = (TimeSpan)reader["gioBatDau"],
                            GioKetThuc = (TimeSpan)reader["gioKetThuc"]
                        });
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

        public decimal GetTongGioLam(int idNhanVien, DateTime tuNgay, DateTime denNgay)
        {
            decimal tongGio = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT SUM(bcc.soGioLam)
                    FROM BangChamCong bcc
                    JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
                    WHERE llv.idNhanVien = @idNV AND llv.ngayLam BETWEEN @tuNgay AND @denNgay", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                cmd.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@denNgay", denNgay.Date);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                {
                    tongGio = Convert.ToDecimal(result);
                }
            }
            return tongGio;
        }
        #endregion


        #region Personal Info
        public bool CapNhatThongTinCaNhan(NhanVien nv)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Chỉ cập nhật các trường nhân viên được phép thay đổi
                var cmd = new SqlCommand("UPDATE NhanVien SET diaChi = @diaChi, matKhau = @matKhau WHERE idNhanVien = @idNV", conn);
                cmd.Parameters.AddWithValue("@diaChi", nv.DiaChi);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@idNV", nv.IdNhanVien);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        // ... (Các hàm LichLamViec đã có) ...

        #region Chấm Công (Hàm mới cần bổ sung)
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
                        llv = new LichLamViec
                        {
                            IdLichLamViec = (int)reader["idLichLamViec"],
                            IdNhanVien = (int)reader["idNhanVien"],
                            IdCa = (int)reader["idCa"],
                            NgayLam = (DateTime)reader["ngayLam"],
                            TenCa = (string)reader["tenCa"],
                            GioBatDau = (TimeSpan)reader["gioBatDau"],
                            GioKetThuc = (TimeSpan)reader["gioKetThuc"]
                        };
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
                        bcc = new BangChamCong
                        {
                            IdChamCong = (int)reader["idChamCong"],
                            IdLichLamViec = (int)reader["idLichLamViec"],
                            GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"],
                            GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"]
                        };
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
                // Vừa cập nhật giờ ra, vừa tính toán số giờ làm
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