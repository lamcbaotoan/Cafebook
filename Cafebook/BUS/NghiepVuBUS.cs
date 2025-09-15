// BUS/NghiepVuBUS.cs
using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            // Viết câu lệnh SQL để lấy danh sách phiếu đặt bàn trong ngày
            return ds;
        }

        public bool TaoPhieuDatBan(PhieuDatBan pdb)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO PhieuDatBan (tenKhachVangLai, sdtKhachVangLai, idBan, thoiGianDat, soLuongKhach, ghiChu, trangThai) VALUES (@ten, @sdt, @idBan, @thoiGian, @sl, @ghiChu, @trangThai)", conn);
                // ... Add Parameters ...
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool CapNhatTrangThaiPhieuDat(int idPhieuDat, string trangThaiMoi)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE PhieuDatBan SET trangThai = @trangThai WHERE idPhieuDatBan = @id", conn);
                // ... Add Parameters ...
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Thuê Sách
        public List<Sach> GetSachCoTheChoThue()
        {
            // Viết câu lệnh SQL lấy sách có soLuongCoSan > 0
            return new List<Sach>();
        }

        public List<PhieuThueSach> GetPhieuDangThue()
        {
            // Viết câu lệnh SQL lấy phiếu có trạng thái 'Đang thuê' hoặc 'Quá hạn'
            return new List<PhieuThueSach>();
        }

        public bool ThucHienChoMuon(PhieuThueSach pts)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    // 1. Thêm vào PhieuThueSach
                    var cmdPts = new SqlCommand("INSERT INTO PhieuThueSach (idSach, idKhachHang, idNhanVien, ngayThue, ngayHenTra, trangThai) VALUES (@idSach, @idKH, @idNV, GETDATE(), @ngayHenTra, N'Đang thuê')", conn, transaction);
                    // ... Add Parameters ...
                    cmdPts.ExecuteNonQuery();

                    // 2. Cập nhật số lượng sách
                    var cmdSach = new SqlCommand("UPDATE Sach SET soLuongCoSan = soLuongCoSan - 1 WHERE idSach = @idSach", conn, transaction);
                    // ... Add Parameters ...
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

        public bool ThucHienTraSach(int idPhieuThue, decimal tienPhat)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    // 1. Lấy idSach trước khi cập nhật
                    var cmdGetId = new SqlCommand("SELECT idSach FROM PhieuThueSach WHERE idPhieuThue = @idPT", conn, transaction);
                    cmdGetId.Parameters.AddWithValue("@idPT", idPhieuThue);
                    int idSach = (int)cmdGetId.ExecuteScalar();

                    // 2. Cập nhật PhieuThueSach
                    var cmdPts = new SqlCommand("UPDATE PhieuThueSach SET ngayTraThucTe = GETDATE(), tienPhat = @tienPhat, trangThai = N'Đã trả' WHERE idPhieuThue = @idPT", conn, transaction);
                    // ... Add Parameters ...
                    cmdPts.ExecuteNonQuery();

                    // 3. Cập nhật số lượng sách
                    var cmdSach = new SqlCommand("UPDATE Sach SET soLuongCoSan = soLuongCoSan + 1 WHERE idSach = @idSach", conn, transaction);
                    // ... Add Parameters ...
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
        #endregion
    }
}