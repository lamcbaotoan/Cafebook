using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class SachBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region Quản lý Sách (CRUD)
        public List<Sach> GetDanhSachSach()
        {
            var ds = new List<Sach>();
            string query = "SELECT idSach, tieuDe, tacGia, theLoai, moTa, tongSoLuong, soLuongCoSan, viTri, giaBia FROM Sach ORDER BY tieuDe";
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
                            IdSach = (int)reader["idSach"],
                            TieuDe = (string)reader["tieuDe"],
                            TacGia = reader["tacGia"] as string,
                            TheLoai = reader["theLoai"] as string,
                            MoTa = reader["moTa"] as string,
                            TongSoLuong = (int)reader["tongSoLuong"],
                            SoLuongCoSan = (int)reader["soLuongCoSan"],
                            ViTri = reader["viTri"] as string,
                            GiaBia = (decimal)reader["giaBia"]
                        });
                    }
                }
            }
            return ds;
        }

        public bool ThemSach(Sach sach)
        {
            string query = @"INSERT INTO Sach (tieuDe, tacGia, theLoai, moTa, tongSoLuong, soLuongCoSan, viTri, giaBia) 
                             VALUES (@tieuDe, @tacGia, @theLoai, @moTa, @tongSoLuong, @soLuongCoSan, @viTri, @giaBia)";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tieuDe", sach.TieuDe);
                cmd.Parameters.AddWithValue("@tacGia", (object)sach.TacGia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@theLoai", (object)sach.TheLoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@moTa", (object)sach.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tongSoLuong", sach.TongSoLuong);
                cmd.Parameters.AddWithValue("@soLuongCoSan", sach.TongSoLuong); // Khi thêm sách mới, số lượng có sẵn bằng tổng số
                cmd.Parameters.AddWithValue("@viTri", (object)sach.ViTri ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@giaBia", sach.GiaBia);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaSach(Sach sach)
        {
            string query = @"UPDATE Sach SET 
                                tieuDe = @tieuDe, tacGia = @tacGia, theLoai = @theLoai, 
                                moTa = @moTa, tongSoLuong = @tongSoLuong, viTri = @viTri, giaBia = @giaBia
                             WHERE idSach = @idSach";
            // Lưu ý: soLuongCoSan không được sửa trực tiếp ở đây. 
            // Nó được quản lý qua nghiệp vụ thuê/trả.
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tieuDe", sach.TieuDe);
                cmd.Parameters.AddWithValue("@tacGia", (object)sach.TacGia ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@theLoai", (object)sach.TheLoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@moTa", (object)sach.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tongSoLuong", sach.TongSoLuong);
                cmd.Parameters.AddWithValue("@viTri", (object)sach.ViTri ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@giaBia", sach.GiaBia);
                cmd.Parameters.AddWithValue("@idSach", sach.IdSach);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaSach(int idSach)
        {
            // Chỉ cho phép xóa khi số lượng có sẵn bằng tổng số lượng (nghĩa là không có cuốn nào đang được thuê)
            string query = "DELETE FROM Sach WHERE idSach = @idSach AND soLuongCoSan = tongSoLuong";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@idSach", idSach);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Lịch sử & Cài đặt
        public List<PhieuThueSach> GetLichSuThueSach()
        {
            var ds = new List<PhieuThueSach>();
            string query = @"SELECT TOP 100
                                s.tieuDe, kh.hoTen, pt.ngayThue, pt.ngayHenTra, pt.ngayTraThucTe, pt.tienPhat, pt.trangThai
                             FROM PhieuThueSach pt
                             JOIN Sach s ON pt.idSach = s.idSach
                             JOIN KhachHang kh ON pt.idKhachHang = kh.idKhachHang
                             ORDER BY pt.idPhieuThue DESC";
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
                            TieuDeSach = reader.GetString(0),
                            TenKhachHang = reader.GetString(1),
                            NgayThue = reader.GetDateTime(2),
                            NgayHenTra = reader.GetDateTime(3),
                            NgayTraThucTe = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            TienPhat = reader.GetDecimal(5),
                            TrangThai = reader.GetString(6)
                        });
                    }
                }
            }
            return ds;
        }

        public List<PhieuThueSach> GetDanhSachSachQuaHan()
        {
            var ds = new List<PhieuThueSach>();
            string query = @"SELECT 
                                s.tieuDe, kh.hoTen, pt.ngayHenTra, pt.trangThai
                             FROM PhieuThueSach pt
                             JOIN Sach s ON pt.idSach = s.idSach
                             JOIN KhachHang kh ON pt.idKhachHang = kh.idKhachHang
                             WHERE pt.trangThai = N'Quá hạn' OR (pt.trangThai = N'Đang thuê' AND pt.ngayHenTra < GETDATE())
                             ORDER BY pt.ngayHenTra ASC";
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
                            TieuDeSach = reader.GetString(0),
                            TenKhachHang = reader.GetString(1),
                            NgayHenTra = reader.GetDateTime(2),
                            TrangThai = reader.GetString(3)
                        });
                    }
                }
            }
            return ds;
        }

        public string GetCaiDat(string tenCaiDat)
        {
            string query = "SELECT giaTri FROM CaiDat WHERE tenCaiDat = @ten";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ten", tenCaiDat);
                conn.Open();
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        public bool SetCaiDat(string tenCaiDat, string giaTri)
        {
            string query = @"IF EXISTS (SELECT 1 FROM CaiDat WHERE tenCaiDat = @ten)
                                UPDATE CaiDat SET giaTri = @giaTri WHERE tenCaiDat = @ten
                             ELSE
                                INSERT INTO CaiDat (tenCaiDat, giaTri) VALUES (@ten, @giaTri)";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ten", tenCaiDat);
                cmd.Parameters.AddWithValue("@giaTri", giaTri);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}