using Cafebook.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Cafebook.BUS
{
    public class SanPhamBUS
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        // Hàm hỗ trợ quy đổi đơn vị
        private decimal ConvertToDonViCoBan(decimal quantity, string selectedUnit, string baseUnit)
        {
            if (string.IsNullOrEmpty(selectedUnit)) return quantity; // Nếu công thức cũ chưa có DVT, coi như là DVT cơ bản
            selectedUnit = selectedUnit.ToLower();
            baseUnit = baseUnit.ToLower();
            if (selectedUnit == baseUnit) return quantity;
            if (selectedUnit == "g" && baseUnit == "kg") return quantity / 1000;
            if (selectedUnit == "ml" && (baseUnit == "lít" || baseUnit == "l")) return quantity / 1000;
            return quantity;
        }

        // HÀM MỚI QUAN TRỌNG: Kiểm tra xem có thể làm được bao nhiêu sản phẩm từ kho
        public int KiemTraKhaNangPhucVu(int idSanPham)
        {
            int soLuongCoThePhucVu = int.MaxValue;
            var congThuc = GetCongThuc(idSanPham);
            if (!congThuc.Any()) return 1000; // Món không có công thức, coi như luôn bán được

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (var thanhPhan in congThuc)
                {
                    var cmd = new SqlCommand("SELECT soLuongTon, donViTinh FROM NguyenLieu WHERE idNguyenLieu = @idNL", conn);
                    cmd.Parameters.AddWithValue("@idNL", thanhPhan.IdNguyenLieu);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal soLuongTon = reader.GetDecimal(0);
                            string donViTinhCoBan = reader.GetString(1);

                            decimal luongCanThietChuan = ConvertToDonViCoBan(thanhPhan.LuongCanThiet, thanhPhan.DonViTinhSuDung, donViTinhCoBan);

                            if (luongCanThietChuan > 0)
                            {
                                int coTheLamDuoc = (int)(soLuongTon / luongCanThietChuan);
                                if (coTheLamDuoc < soLuongCoThePhucVu)
                                {
                                    soLuongCoThePhucVu = coTheLamDuoc;
                                }
                            }
                        }
                        else // Nguyên liệu không tồn tại trong kho
                        {
                            return 0;
                        }
                    }
                }
            }
            return soLuongCoThePhucVu;
        }

        // Lấy danh sách TẤT CẢ sản phẩm
        public List<SanPham> GetDanhSachSanPham()
        {
            List<SanPham> dssp = new List<SanPham>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT sp.idSanPham, sp.tenSanPham, sp.donGia, lsp.tenLoaiSP, sp.trangThai, sp.moTa, sp.idLoaiSP 
                                 FROM SanPham sp JOIN LoaiSanPham lsp ON sp.idLoaiSP = lsp.idLoaiSP";
                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dssp.Add(new SanPham
                        {
                            IdSanPham = reader.GetInt32(0),
                            TenSanPham = reader.GetString(1),
                            DonGia = reader.GetDecimal(2),
                            TenLoaiSP = reader.GetString(3),
                            TrangThai = reader.GetString(4),
                            MoTa = reader.IsDBNull(5) ? "" : reader.GetString(5),
                            IdLoaiSP = reader.GetInt32(6)
                        });
                    }
                }
            }
            return dssp;
        }

        // Lấy danh sách loại sản phẩm
        public List<LoaiSanPham> GetDanhSachLoaiSP()
        {
            List<LoaiSanPham> dslsp = new List<LoaiSanPham>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT idLoaiSP, tenLoaiSP FROM LoaiSanPham", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dslsp.Add(new LoaiSanPham { IdLoaiSP = reader.GetInt32(0), TenLoaiSP = reader.GetString(1) });
                    }
                }
            }
            return dslsp;
        }

        // Thêm sản phẩm mới
        public bool ThemSanPham(SanPham sp)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO SanPham (idLoaiSP, tenSanPham, moTa, donGia, trangThai) VALUES (@idLoaiSP, @tenSP, @moTa, @donGia, @trangThai)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idLoaiSP", sp.IdLoaiSP);
                cmd.Parameters.AddWithValue("@tenSP", sp.TenSanPham);
                cmd.Parameters.AddWithValue("@moTa", sp.MoTa ?? "");
                cmd.Parameters.AddWithValue("@donGia", sp.DonGia);
                cmd.Parameters.AddWithValue("@trangThai", sp.TrangThai);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Sửa sản phẩm
        public bool SuaSanPham(SanPham sp)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE SanPham SET idLoaiSP = @idLoaiSP, tenSanPham = @tenSP, moTa = @moTa, donGia = @donGia, trangThai = @trangThai WHERE idSanPham = @idSP";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idLoaiSP", sp.IdLoaiSP);
                cmd.Parameters.AddWithValue("@tenSP", sp.TenSanPham);
                cmd.Parameters.AddWithValue("@moTa", sp.MoTa ?? "");
                cmd.Parameters.AddWithValue("@donGia", sp.DonGia);
                cmd.Parameters.AddWithValue("@trangThai", sp.TrangThai);
                cmd.Parameters.AddWithValue("@idSP", sp.IdSanPham);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Xóa sản phẩm (cần xóa công thức trước)
        public bool XoaSanPham(int idSanPham)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // Xóa các dòng liên quan trong CongThuc trước
                    SqlCommand cmdCongThuc = new SqlCommand("DELETE FROM CongThuc WHERE idSanPham = @idSP", conn, transaction);
                    cmdCongThuc.Parameters.AddWithValue("@idSP", idSanPham);
                    cmdCongThuc.ExecuteNonQuery();

                    // Xóa sản phẩm
                    SqlCommand cmdSanPham = new SqlCommand("DELETE FROM SanPham WHERE idSanPham = @idSP", conn, transaction);
                    cmdSanPham.Parameters.AddWithValue("@idSP", idSanPham);
                    cmdSanPham.ExecuteNonQuery();

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

        // Lấy danh sách nguyên liệu
        public List<NguyenLieu> GetDanhSachNguyenLieu()
        {
            List<NguyenLieu> dsnl = new List<NguyenLieu>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT idNguyenLieu, tenNguyenLieu, donViTinh FROM NguyenLieu", conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsnl.Add(new NguyenLieu { IdNguyenLieu = reader.GetInt32(0), TenNguyenLieu = reader.GetString(1), DonViTinh = reader.GetString(2) });
                    }
                }
            }
            return dsnl;
        }

        // Lấy công thức của một sản phẩm
        public List<CongThuc> GetCongThuc(int idSanPham)
        {
            List<CongThuc> dsct = new List<CongThuc>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ct.idSanPham, ct.idNguyenLieu, ct.luongCanThiet, nl.tenNguyenLieu, ct.donViTinhSuDung 
                         FROM CongThuc ct JOIN NguyenLieu nl ON ct.idNguyenLieu = nl.idNguyenLieu 
                         WHERE ct.idSanPham = @idSP";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSP", idSanPham);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dsct.Add(new CongThuc
                        {
                            IdSanPham = reader.GetInt32(0),
                            IdNguyenLieu = reader.GetInt32(1),
                            LuongCanThiet = reader.GetDecimal(2),
                            TenNguyenLieu = reader.GetString(3),
                            DonViTinhSuDung = reader.IsDBNull(4) ? "" : reader.GetString(4) // Lấy đơn vị tính sử dụng
                        });
                    }
                }
            }
            return dsct;
        }

        // Thêm/Cập nhật nguyên liệu vào công thức
        public bool LuuNguyenLieuVaoCongThuc(CongThuc ct)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            MERGE CongThuc AS target
            USING (SELECT @idSP AS idSanPham, @idNL AS idNguyenLieu) AS source
            ON (target.idSanPham = source.idSanPham AND target.idNguyenLieu = source.idNguyenLieu)
            WHEN MATCHED THEN
                UPDATE SET luongCanThiet = @luong, donViTinhSuDung = @dvt
            WHEN NOT MATCHED THEN
                INSERT (idSanPham, idNguyenLieu, luongCanThiet, donViTinhSuDung) VALUES (@idSP, @idNL, @luong, @dvt);";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSP", ct.IdSanPham);
                cmd.Parameters.AddWithValue("@idNL", ct.IdNguyenLieu);
                cmd.Parameters.AddWithValue("@luong", ct.LuongCanThiet);
                cmd.Parameters.AddWithValue("@dvt", ct.DonViTinhSuDung); // Thêm parameter cho đơn vị tính
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Xóa nguyên liệu khỏi công thức
        public bool XoaNguyenLieuKhoiCongThuc(int idSanPham, int idNguyenLieu)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM CongThuc WHERE idSanPham = @idSP AND idNguyenLieu = @idNL";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idSP", idSanPham);
                cmd.Parameters.AddWithValue("@idNL", idNguyenLieu);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        #region CRUD Loại Sản Phẩm
        public bool ThemLoaiSanPham(LoaiSanPham lsp)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO LoaiSanPham (tenLoaiSP) VALUES (@ten)", conn);
                cmd.Parameters.AddWithValue("@ten", lsp.TenLoaiSP);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SuaLoaiSanPham(LoaiSanPham lsp)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE LoaiSanPham SET tenLoaiSP = @ten WHERE idLoaiSP = @id", conn);
                cmd.Parameters.AddWithValue("@ten", lsp.TenLoaiSP);
                cmd.Parameters.AddWithValue("@id", lsp.IdLoaiSP);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool XoaLoaiSanPham(int idLoaiSP)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Kiểm tra xem có sản phẩm nào đang dùng loại này không
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM SanPham WHERE idLoaiSP = @id", conn);
                checkCmd.Parameters.AddWithValue("@id", idLoaiSP);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Nếu có, không cho xóa
                    return false;
                }

                // Nếu không, tiến hành xóa
                var deleteCmd = new SqlCommand("DELETE FROM LoaiSanPham WHERE idLoaiSP = @id", conn);
                deleteCmd.Parameters.AddWithValue("@id", idLoaiSP);
                return deleteCmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}