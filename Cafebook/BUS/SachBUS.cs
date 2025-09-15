// BUS/SachBUS.cs
using Cafebook.DTO; // <--- THÊM DÒNG NÀY
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace Cafebook.BUS
{
    public class SachBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        #region CRUD Sách
        public List<Sach> GetDanhSachSach()
        {
            var dsSach = new List<Sach>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM Sach", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsSach.Add(new Sach
                        {
                            IdSach = reader.GetInt32(0),
                            TieuDe = reader.GetString(1),
                            TacGia = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            TheLoai = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            MoTa = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            TongSoLuong = reader.GetInt32(5),
                            SoLuongCoSan = reader.GetInt32(6)
                        });
                    }
                }
            }
            return dsSach;
        }

        public bool ThemSach(Sach sach)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Sach (tieuDe, tacGia, theLoai, moTa, tongSoLuong, soLuongCoSan) VALUES (@tieuDe, @tacGia, @theLoai, @moTa, @tongSL, @slCoSan)", conn);
                cmd.Parameters.AddWithValue("@tieuDe", sach.TieuDe);
                cmd.Parameters.AddWithValue("@tacGia", sach.TacGia);
                cmd.Parameters.AddWithValue("@theLoai", sach.TheLoai);
                cmd.Parameters.AddWithValue("@moTa", sach.MoTa);
                cmd.Parameters.AddWithValue("@tongSL", sach.TongSoLuong);
                cmd.Parameters.AddWithValue("@slCoSan", sach.TongSoLuong); // Mới thêm vào thì số lượng có sẵn bằng tổng số
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaSach(Sach sach)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Lưu ý: Không cho sửa số lượng có sẵn trực tiếp. Chỉ cho sửa tổng số lượng.
                // Việc điều chỉnh số lượng có sẵn sẽ dựa vào nghiệp vụ thuê/trả.
                var cmd = new SqlCommand("UPDATE Sach SET tieuDe = @tieuDe, tacGia = @tacGia, theLoai = @theLoai, moTa = @moTa, tongSoLuong = @tongSL WHERE idSach = @id", conn);
                cmd.Parameters.AddWithValue("@tieuDe", sach.TieuDe);
                cmd.Parameters.AddWithValue("@tacGia", sach.TacGia);
                cmd.Parameters.AddWithValue("@theLoai", sach.TheLoai);
                cmd.Parameters.AddWithValue("@moTa", sach.MoTa);
                cmd.Parameters.AddWithValue("@tongSL", sach.TongSoLuong);
                cmd.Parameters.AddWithValue("@id", sach.IdSach);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaSach(int idSach)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Chỉ xóa sách nếu tất cả các bản sao đều có sẵn (không có ai đang thuê)
                var cmd = new SqlCommand("DELETE FROM Sach WHERE idSach = @id AND soLuongCoSan = tongSoLuong", conn);
                cmd.Parameters.AddWithValue("@id", idSach);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region Lịch sử cho thuê
        public List<PhieuThueSach> GetLichSuThueSach()
        {
            var dsLichSu = new List<PhieuThueSach>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT pts.idPhieuThue, s.tieuDe, kh.hoTen, nv.hoTen, pts.ngayThue, pts.ngayHenTra, pts.ngayTraThucTe, pts.tienPhat, pts.trangThai
                    FROM PhieuThueSach pts
                    JOIN Sach s ON pts.idSach = s.idSach
                    JOIN KhachHang kh ON pts.idKhachHang = kh.idKhachHang
                    JOIN NhanVien nv ON pts.idNhanVien = nv.idNhanVien
                    ORDER BY pts.ngayThue DESC", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsLichSu.Add(new PhieuThueSach
                        {
                            IdPhieuThue = reader.GetInt32(0),
                            TieuDeSach = reader.GetString(1),
                            TenKhachHang = reader.GetString(2),
                            TenNhanVien = reader.GetString(3),
                            NgayThue = reader.GetDateTime(4),
                            NgayHenTra = reader.GetDateTime(5),
                            NgayTraThucTe = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6),
                            TienPhat = reader.GetDecimal(7),
                            TrangThai = reader.GetString(8)
                        });
                    }
                }
            }
            return dsLichSu;
        }
        #endregion
    }
}