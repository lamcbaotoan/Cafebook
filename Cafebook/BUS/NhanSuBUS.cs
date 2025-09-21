using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Cafebook.BUS
{
    public class NhanSuBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region NhanVien & VaiTro & CaLamViec
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
                var cmd = new SqlCommand("INSERT INTO NhanVien (idVaiTro, hoTen, soDienThoai, email, diaChi, matKhau, ngayVaoLam, trangThai, mucLuongTheoGio) VALUES (@idVT, @hoTen, @sdt, @email, @diaChi, @matKhau, @ngayVaoLam, @trangThai, @luong)", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@sdt", (object)nv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object)nv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@diaChi", (object)nv.DiaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@ngayVaoLam", nv.NgayVaoLam);
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
                var cmd = new SqlCommand("UPDATE NhanVien SET idVaiTro = @idVT, hoTen = @hoTen, soDienThoai = @sdt, email = @email, diaChi = @diaChi, matKhau = @matKhau, mucLuongTheoGio = @luong, trangThai = @trangThai WHERE idNhanVien = @idNV", conn);
                cmd.Parameters.AddWithValue("@idVT", nv.IdVaiTro);
                cmd.Parameters.AddWithValue("@hoTen", nv.HoTen);
                cmd.Parameters.AddWithValue("@sdt", (object)nv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object)nv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@diaChi", (object)nv.DiaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@matKhau", nv.MatKhau);
                cmd.Parameters.AddWithValue("@luong", nv.MucLuongTheoGio);
                cmd.Parameters.AddWithValue("@trangThai", nv.TrangThai);
                cmd.Parameters.AddWithValue("@idNV", nv.IdNhanVien);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // **PHƯƠNG THỨC BỊ THIẾU ĐÃ ĐƯỢC THÊM LẠI**
        public bool CapNhatThongTinCaNhan(NhanVien nv, bool updatePassword)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
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
        #endregion

        #region LichLamViec (Đã nâng cấp)
        public List<LichLamViec> GetLichLamViec(DateTime ngay)
        {
            var ds = new List<LichLamViec>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT llv.idLichLamViec, nv.hoTen, llv.gioBatDau, llv.gioKetThuc, llv.trangThai 
                    FROM LichLamViec llv 
                    JOIN NhanVien nv ON llv.idNhanVien = nv.idNhanVien
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
                            GioBatDau = reader["gioBatDau"] as TimeSpan?,
                            GioKetThuc = reader["gioKetThuc"] as TimeSpan?,
                            TrangThai = reader["trangThai"] as string
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
                var cmd = new SqlCommand(@"INSERT INTO LichLamViec 
                                (idNhanVien, ngayLam, gioBatDau, gioKetThuc, trangThai) 
                                VALUES (@idNV, @ngayLam, @gioBD, @gioKT, @trangThai)", conn);
                cmd.Parameters.AddWithValue("@idNV", llv.IdNhanVien);
                cmd.Parameters.AddWithValue("@ngayLam", llv.NgayLam.Date);
                cmd.Parameters.AddWithValue("@gioBD", (object)llv.GioBatDau ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gioKT", (object)llv.GioKetThuc ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@trangThai", llv.TrangThai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaLichLamViec(int idLich, out string reason)
        {
            reason = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM BangChamCong WHERE idLichLamViec = @id", conn);
                cmdCheck.Parameters.AddWithValue("@id", idLich);
                if ((int)cmdCheck.ExecuteScalar() > 0)
                {
                    reason = "Không thể xóa: Nhân viên đã thực hiện chấm công cho lịch này.";
                    return false;
                }

                var cmdDelete = new SqlCommand("DELETE FROM LichLamViec WHERE idLichLamViec = @id", conn);
                cmdDelete.Parameters.AddWithValue("@id", idLich);
                if (cmdDelete.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    reason = "Xóa thất bại hoặc lịch không tồn tại.";
                    return false;
                }
            }
        }
        #endregion

        #region QuyTacLuong (Mới)
        public List<QuyTacLuong> GetQuyTacLuong()
        {
            var ds = new List<QuyTacLuong>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM QuyTacLuong", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new QuyTacLuong
                        {
                            IdQuyTac = (int)reader["idQuyTac"],
                            TenQuyTac = (string)reader["tenQuyTac"],
                            Loai = (string)reader["loai"],
                            LoaiQuyTac = (string)reader["loaiQuyTac"],
                            DieuKien = reader["dieuKien"] as decimal?,
                            // **NÂNG CẤP**: Đọc thêm đơn vị của điều kiện
                            DieuKienDonViTinh = reader["dieuKienDonViTinh"] as string,
                            GiaTriApDung = (decimal)reader["giaTriApDung"],
                            DonViTinh = (string)reader["donViTinh"]
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemQuyTac(QuyTacLuong qt)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"INSERT INTO QuyTacLuong 
                                           (tenQuyTac, loai, loaiQuyTac, dieuKien, dieuKienDonViTinh, giaTriApDung, donViTinh) 
                                           VALUES (@ten, @loai, @loaiQT, @dk, @dkdvt, @gt, @dvt)", conn);
                cmd.Parameters.AddWithValue("@ten", qt.TenQuyTac);
                cmd.Parameters.AddWithValue("@loai", qt.Loai);
                cmd.Parameters.AddWithValue("@loaiQT", qt.LoaiQuyTac);
                cmd.Parameters.AddWithValue("@dk", (object)qt.DieuKien ?? DBNull.Value);
                // **NÂNG CẤP**: Thêm tham số cho đơn vị của điều kiện
                cmd.Parameters.AddWithValue("@dkdvt", (object)qt.DieuKienDonViTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gt", qt.GiaTriApDung);
                cmd.Parameters.AddWithValue("@dvt", qt.DonViTinh);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Trong file: BUS/NhanSuBUS.cs

        public bool SuaQuyTac(QuyTacLuong qt)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE QuyTacLuong SET 
                                           tenQuyTac=@ten, 
                                           loai=@loai, 
                                           loaiQuyTac=@loaiQT, 
                                           dieuKien=@dk, 
                                           dieuKienDonViTinh=@dkdvt, 
                                           giaTriApDung=@gt, 
                                           donViTinh=@dvt 
                                           WHERE idQuyTac=@id", conn);
                cmd.Parameters.AddWithValue("@id", qt.IdQuyTac);
                cmd.Parameters.AddWithValue("@ten", qt.TenQuyTac);
                cmd.Parameters.AddWithValue("@loai", qt.Loai);
                cmd.Parameters.AddWithValue("@loaiQT", qt.LoaiQuyTac);
                cmd.Parameters.AddWithValue("@dk", (object)qt.DieuKien ?? DBNull.Value);
                // **NÂNG CẤP**: Thêm tham số cho đơn vị của điều kiện
                cmd.Parameters.AddWithValue("@dkdvt", (object)qt.DieuKienDonViTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@gt", qt.GiaTriApDung);
                cmd.Parameters.AddWithValue("@dvt", qt.DonViTinh);
                return cmd.ExecuteNonQuery() > 0;
            }
        }


        // Trong file: BUS/NhanSuBUS.cs

        public bool XoaQuyTac(int idQuyTac)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Sử dụng Transaction để đảm bảo toàn vẹn dữ liệu
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Xóa tất cả các bản ghi con trong ChiTietThuongPhat
                        var cmdDeleteDetails = new SqlCommand("DELETE FROM ChiTietThuongPhat WHERE idQuyTac = @id", conn, tran);
                        cmdDeleteDetails.Parameters.AddWithValue("@id", idQuyTac);
                        cmdDeleteDetails.ExecuteNonQuery(); // Thực thi xóa chi tiết

                        // Bước 2: Xóa bản ghi cha trong QuyTacLuong
                        var cmdDeleteRule = new SqlCommand("DELETE FROM QuyTacLuong WHERE idQuyTac = @id", conn, tran);
                        cmdDeleteRule.Parameters.AddWithValue("@id", idQuyTac);
                        cmdDeleteRule.ExecuteNonQuery(); // Thực thi xóa quy tắc

                        // Nếu cả hai lệnh đều thành công, commit transaction
                        tran.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        // Nếu có bất kỳ lỗi nào, rollback lại toàn bộ thay đổi
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        #endregion

        #region ChamCong & Luong (Đã nâng cấp)
        // Trong file: BUS/NhanSuBUS.cs
        // Trong file: BUS/NhanSuBUS.cs

        public PhieuLuong TinhLuong(int idNhanVien, DateTime tuNgay, DateTime denNgay)
        {
            var pl = new PhieuLuong { IdNhanVien = idNhanVien, TuNgay = tuNgay, DenNgay = denNgay };
            var quyTacList = GetQuyTacLuong();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmdLuong = new SqlCommand("SELECT mucLuongTheoGio FROM NhanVien WHERE idNhanVien = @idNV", conn);
                cmdLuong.Parameters.AddWithValue("@idNV", idNhanVien);
                decimal mucLuongTheoGio = (decimal)cmdLuong.ExecuteScalar();

                var cmdChamCong = new SqlCommand(@"
            SELECT bcc.soGioLam, bcc.gioVao, bcc.gioRa, llv.gioBatDau, llv.gioKetThuc
            FROM BangChamCong bcc 
            JOIN LichLamViec llv ON bcc.idLichLamViec = llv.idLichLamViec
            WHERE llv.idNhanVien = @idNV 
                AND llv.ngayLam BETWEEN @tuNgay AND @denNgay 
                AND bcc.soGioLam IS NOT NULL 
                AND llv.trangThai = N'Đi Làm'", conn);
                cmdChamCong.Parameters.AddWithValue("@idNV", idNhanVien);
                cmdChamCong.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmdChamCong.Parameters.AddWithValue("@denNgay", denNgay.Date);

                decimal tongGioLam = 0;
                decimal tongTienPhatDiTre = 0;
                decimal tongTienTangCa = 0;

                var listChamCong = new List<dynamic>();
                using (var reader = cmdChamCong.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listChamCong.Add(new
                        {
                            soGioLam = (decimal)reader["soGioLam"],
                            gioVao = (DateTime)reader["gioVao"],
                            gioRa = (DateTime)reader["gioRa"],
                            caBatDau = reader["gioBatDau"] as TimeSpan?,
                            caKetThuc = reader["gioKetThuc"] as TimeSpan?
                        });
                    }
                }

                // BƯỚC 1: TÍNH TỔNG GIỜ LÀM VÀ PHẠT ĐI TRỄ THEO TỪNG NGÀY
                foreach (var cc in listChamCong)
                {
                    tongGioLam += cc.soGioLam;

                    if (cc.caBatDau != null && cc.caKetThuc != null)
                    {
                        DateTime thoiGianCaBatDau = cc.gioVao.Date.Add((TimeSpan)cc.caBatDau);
                        DateTime thoiGianCaKetThuc = cc.gioVao.Date.Add((TimeSpan)cc.caKetThuc);
                        if (thoiGianCaKetThuc <= thoiGianCaBatDau) thoiGianCaKetThuc = thoiGianCaKetThuc.AddDays(1);

                        var quyTacTre = quyTacList.FirstOrDefault(q => q.LoaiQuyTac == "LATE");
                        if (quyTacTre != null && (cc.gioVao - thoiGianCaBatDau).TotalMinutes > (double)quyTacTre.DieuKien.GetValueOrDefault(0))
                        {
                            tongTienPhatDiTre += quyTacTre.GiaTriApDung;
                        }
                    }
                }

                // BƯỚC 2: KIỂM TRA ĐIỀU KIỆN TỔNG GIỜ VÀ ÁP DỤNG THƯỞNG TĂNG CA
                var quyTacTangCa = quyTacList.FirstOrDefault(q => q.LoaiQuyTac == "OVERTIME");
                if (quyTacTangCa != null && quyTacTangCa.DieuKien.HasValue)
                {
                    decimal dieuKienGio = quyTacTangCa.DieuKien.Value;
                    if (quyTacTangCa.DieuKienDonViTinh == "Phút")
                    {
                        dieuKienGio /= 60;
                    }

                    if (tongGioLam > dieuKienGio)
                    {
                        decimal soGioTangCa = tongGioLam - dieuKienGio;
                        if (quyTacTangCa.DonViTinh == "Multiplier")
                        {
                            tongTienTangCa = soGioTangCa * mucLuongTheoGio * (quyTacTangCa.GiaTriApDung - 1);
                        }
                        else
                        {
                            tongTienTangCa = soGioTangCa * quyTacTangCa.GiaTriApDung;
                        }
                    }
                }

                // Thêm các khoản TỰ ĐỘNG đã tổng hợp vào danh sách chi tiết
                if (tongTienTangCa > 0)
                {
                    pl.CacKhoanThuong.Add(new ChiTietDongLuong { IdChiTiet = null, NoiDung = "Thưởng tăng ca", SoTien = tongTienTangCa });
                }
                if (tongTienPhatDiTre > 0)
                {
                    pl.CacKhoanPhat.Add(new ChiTietDongLuong { IdChiTiet = null, NoiDung = "Tổng tiền phạt đi trễ", SoTien = tongTienPhatDiTre });
                }

                // BƯỚC 3: LẤY CÁC KHOẢN THỦ CÔNG VÀ TÍNH TỔNG
                var cmdThuongPhatThuCong = new SqlCommand(@"
            SELECT idChiTiet, ghiChu, soTien 
            FROM ChiTietThuongPhat
            WHERE idNhanVien = @idNV 
              AND ngayApDung BETWEEN @tuNgay AND @denNgay
              AND soTien IS NOT NULL", conn);
                cmdThuongPhatThuCong.Parameters.AddWithValue("@idNV", idNhanVien);
                cmdThuongPhatThuCong.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmdThuongPhatThuCong.Parameters.AddWithValue("@denNgay", denNgay.Date);

                using (var reader = cmdThuongPhatThuCong.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var idChiTiet = reader.GetInt32(0);
                        var ghiChu = reader.GetString(1);
                        var soTien = reader.GetDecimal(2);

                        if (soTien >= 0)
                        {
                            pl.CacKhoanThuong.Add(new ChiTietDongLuong { IdChiTiet = idChiTiet, NoiDung = ghiChu, SoTien = soTien });
                        }
                        else
                        {
                            pl.CacKhoanPhat.Add(new ChiTietDongLuong { IdChiTiet = idChiTiet, NoiDung = ghiChu, SoTien = -soTien });
                        }
                    }
                }

                pl.TongGioLam = tongGioLam;
                pl.LuongCoBan = pl.TongGioLam * mucLuongTheoGio;
                pl.TongThuong = pl.CacKhoanThuong.Sum(k => k.SoTien);
                pl.TongPhat = pl.CacKhoanPhat.Sum(k => k.SoTien);
                pl.ThucLanh = pl.LuongCoBan + pl.TongThuong - pl.TongPhat;
            }
            return pl;
        }        // Trong file: BUS/NhanSuBUS.cs

        public bool GhiNhanThuongPhatThuCong(int idNhanVien, string ghiChu, decimal soTien)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                int genericRuleId;
                string genericRuleName = "Thưởng/Phạt Thủ Công";

                // Bước 1: Tìm ID của quy tắc chung.
                var cmdFind = new SqlCommand("SELECT idQuyTac FROM QuyTacLuong WHERE tenQuyTac = @name", conn);
                cmdFind.Parameters.AddWithValue("@name", genericRuleName);
                object result = cmdFind.ExecuteScalar();

                if (result != null)
                {
                    // Nếu đã tồn tại, lấy ID của nó.
                    genericRuleId = (int)result;
                }
                else
                {
                    // Nếu chưa tồn tại, tạo mới quy tắc chung này.
                    var cmdCreate = new SqlCommand(@"
                INSERT INTO QuyTacLuong (tenQuyTac, loai, loaiQuyTac, giaTriApDung, donViTinh)
                OUTPUT INSERTED.idQuyTac
                VALUES (@name, 'Phat', 'FLAT', 0, 'VND')", conn);
                    cmdCreate.Parameters.AddWithValue("@name", genericRuleName);
                    genericRuleId = (int)cmdCreate.ExecuteScalar();
                }

                // Bước 2: Thêm chi tiết vào bảng ChiTietThuongPhat với ID quy tắc đã có.
                var cmdInsert = new SqlCommand(@"
            INSERT INTO ChiTietThuongPhat(idNhanVien, idQuyTac, ngayApDung, ghiChu, soTien)
            VALUES (@idNV, @idQuyTac, GETDATE(), @ghiChu, @soTien)", conn);

                cmdInsert.Parameters.AddWithValue("@idNV", idNhanVien);
                cmdInsert.Parameters.AddWithValue("@idQuyTac", genericRuleId); // **Cung cấp idQuyTac**
                cmdInsert.Parameters.AddWithValue("@ghiChu", ghiChu);
                cmdInsert.Parameters.AddWithValue("@soTien", soTien);

                return cmdInsert.ExecuteNonQuery() > 0;
            }
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
                        ds.Add(new PhieuLuong
                        {
                            IdPhieuLuong = (int)reader["idPhieuLuong"],
                            IdNhanVien = (int)reader["idNhanVien"],
                            TuNgay = (DateTime)reader["tuNgay"],
                            DenNgay = (DateTime)reader["denNgay"],
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
                    SELECT llv.*
                    FROM LichLamViec llv
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
                            NgayLam = (DateTime)reader["ngayLam"],
                            GioBatDau = reader["gioBatDau"] as TimeSpan?,
                            GioKetThuc = reader["gioKetThuc"] as TimeSpan?,
                            TrangThai = reader["trangThai"] as string
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
                        soGioLam = CAST(DATEDIFF(MINUTE, gioVao, @gioRa) AS DECIMAL(10,2)) / 60
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
                    SELECT llv.ngayLam, bcc.gioVao, bcc.gioRa, bcc.soGioLam, llv.gioBatDau, llv.gioKetThuc
                    FROM LichLamViec llv
                    LEFT JOIN BangChamCong bcc ON llv.idLichLamViec = bcc.idLichLamViec
                    WHERE llv.idNhanVien = @idNV
                    ORDER BY llv.ngayLam DESC, bcc.gioVao DESC", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tenCa = "N/A";
                        var batDauOrdinal = reader.GetOrdinal("gioBatDau");
                        var ketThucOrdinal = reader.GetOrdinal("gioKetThuc");
                        if (!reader.IsDBNull(batDauOrdinal) && !reader.IsDBNull(ketThucOrdinal))
                        {
                            tenCa = $"{reader.GetTimeSpan(batDauOrdinal):hh\\:mm} - {reader.GetTimeSpan(ketThucOrdinal):hh\\:mm}";
                        }

                        ds.Add(new BangChamCong
                        {
                            NgayLam = (DateTime)reader["ngayLam"],
                            TenCa = tenCa,
                            GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"],
                            GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"],
                            SoGioLam = reader.IsDBNull(reader.GetOrdinal("soGioLam")) ? (decimal?)null : (decimal)reader["soGioLam"]
                        });
                    }
                }
            }
            return ds;
        }

        // Trong file: BUS/NhanSuBUS.cs

        public List<BangChamCong> GetBangChamCong(DateTime ngay)
        {
            var ds = new List<BangChamCong>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // SỬA LẠI QUERY: Dùng LEFT JOIN để lấy tất cả nhân viên trong lịch
                var cmd = new SqlCommand(@"
            SELECT 
                nv.hoTen, 
                bcc.gioVao, 
                bcc.gioRa, 
                bcc.soGioLam
            FROM LichLamViec llv
            JOIN NhanVien nv ON llv.idNhanVien = nv.idNhanVien
            LEFT JOIN BangChamCong bcc ON llv.idLichLamViec = bcc.idLichLamViec
            WHERE llv.ngayLam = @ngayLam AND llv.trangThai = N'Đi Làm'", conn);

                cmd.Parameters.AddWithValue("@ngayLam", ngay.Date);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new BangChamCong
                        {
                            HoTenNhanVien = (string)reader["hoTen"],
                            GioVao = reader.IsDBNull(reader.GetOrdinal("gioVao")) ? (DateTime?)null : (DateTime)reader["gioVao"],
                            GioRa = reader.IsDBNull(reader.GetOrdinal("gioRa")) ? (DateTime?)null : (DateTime)reader["gioRa"],
                            SoGioLam = reader.IsDBNull(reader.GetOrdinal("soGioLam")) ? (decimal?)null : (decimal)reader["soGioLam"]
                        });
                    }
                }
            }
            return ds;
        }

        // Dán 3 hàm này vào file BUS/NhanSuBUS.cs

        public List<ChiTietThuongPhatDTO> GetChiTietThuongPhatThuCong(int idNhanVien, DateTime tuNgay, DateTime denNgay)
        {
            var ds = new List<ChiTietThuongPhatDTO>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
            SELECT idChiTiet, ghiChu, soTien 
            FROM ChiTietThuongPhat
            WHERE idNhanVien = @idNV 
              AND ngayApDung BETWEEN @tuNgay AND @denNgay
              AND soTien IS NOT NULL", conn);
                cmd.Parameters.AddWithValue("@idNV", idNhanVien);
                cmd.Parameters.AddWithValue("@tuNgay", tuNgay.Date);
                cmd.Parameters.AddWithValue("@denNgay", denNgay.Date);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ds.Add(new ChiTietThuongPhatDTO
                        {
                            IdChiTiet = (int)reader["idChiTiet"],
                            GhiChu = reader["ghiChu"] as string,
                            SoTien = reader["soTien"] as decimal?
                        });
                    }
                }
            }
            return ds;
        }

        public bool SuaChiTietThuongPhatThuCong(int idChiTiet, string ghiChu, decimal soTien)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE ChiTietThuongPhat SET ghiChu = @ghiChu, soTien = @soTien WHERE idChiTiet = @id", conn);
                cmd.Parameters.AddWithValue("@id", idChiTiet);
                cmd.Parameters.AddWithValue("@ghiChu", ghiChu);
                cmd.Parameters.AddWithValue("@soTien", soTien);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaChiTietThuongPhatThuCong(int idChiTiet)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM ChiTietThuongPhat WHERE idChiTiet = @id", conn);
                cmd.Parameters.AddWithValue("@id", idChiTiet);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #endregion
    }
}